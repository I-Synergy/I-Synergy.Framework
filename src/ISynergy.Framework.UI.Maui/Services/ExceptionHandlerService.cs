using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.UI.Services;

public class ExceptionHandlerService : IExceptionHandlerService, IDisposable
{
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<ExceptionHandlerService> _logger;
    private readonly Queue<Exception> _startupExceptions = new Queue<Exception>();
    private readonly Dictionary<string, Exception> _processedExceptions = new Dictionary<string, Exception>();

    private bool _isApplicationInitialized = false;
    private bool _isHandlingException = false;
    private bool _isDisposed = false;
    private int _lastErrorMessage = 0;
    private readonly int _maxStartupExceptions = 50;
    private bool _dialogServiceAvailable = true; // Track if dialog service is working
    private int _dialogFailureCount = 0;
    private DateTime _lastDialogFailure = DateTime.MinValue;
    private readonly TimeSpan _dialogRecoveryPeriod = TimeSpan.FromMinutes(5);

    public ExceptionHandlerService(
        IBusyService busyService,
        IDialogService dialogService,
        ILogger<ExceptionHandlerService> logger)
    {
        _busyService = busyService ?? throw new ArgumentNullException(nameof(busyService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Register ALL exception handlers immediately upon service creation
        RegisterGlobalExceptionHandlers();
    }

    /// <summary>
    /// Registers all global exception handlers for .NET MAUI
    /// </summary>
    private void RegisterGlobalExceptionHandlers()
    {
        // This is THE ONLY place where exception handlers are registered
        AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        // Platform-specific handlers
#if ANDROID
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#elif WINDOWS
        if (Microsoft.UI.Xaml.Application.Current != null)
            Microsoft.UI.Xaml.Application.Current.UnhandledException += OnWindowsUnhandledException;
#endif

        _logger.LogTrace("Global exception handlers registered - single point of control");
    }

    /// <summary>
    /// Sets the application as initialized and processes any queued startup exceptions
    /// </summary>
    public void SetApplicationInitialized()
    {
        _isApplicationInitialized = true;
        _ = Task.Run(ProcessStartupExceptionsAsync);
    }

    private async Task ProcessStartupExceptionsAsync()
    {
        try
        {
            while (_startupExceptions.Count > 0)
            {
                var exception = _startupExceptions.Dequeue();
                await HandleExceptionAsync(exception, true);
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing startup exceptions");
        }
    }

    public virtual Task HandleExceptionAsync(Exception exception) =>
        HandleExceptionAsync(exception, false);

    private async Task HandleExceptionAsync(Exception exception, bool isStartupExceptionReplay = false)
    {
        Argument.IsNotNull(exception);

        // Check if we're already handling an exception to prevent recursion
        if (_isHandlingException || _isDisposed)
        {
            _logger.LogTrace("Exception handling skipped - already handling or service disposed");
            return;
        }

        // Queue startup exceptions if app isn't ready yet
        if (!_isApplicationInitialized && !isStartupExceptionReplay)
        {
            _logger.LogTrace("Application not fully initialized. Queuing exception for later handling.");

            if (_startupExceptions.Count < _maxStartupExceptions)
                _startupExceptions.Enqueue(exception);

            return;
        }

        // Prevent duplicate processing using the same logic as Application.cs
        string exceptionKey = $"{exception.GetType().FullName}:{exception.Message}:{exception.StackTrace?.GetHashCode()}";

        if (_processedExceptions.ContainsKey(exceptionKey))
            return;

        if (_processedExceptions.Count > 100)
            _processedExceptions.Clear();

        _processedExceptions[exceptionKey] = exception;

        // Check for duplicate error messages
        var isDuplicateException = exception.HResult.Equals(_lastErrorMessage);

        if (isDuplicateException && !ShouldAlwaysProcess(exception))
            return;

        // Filter out exceptions that should be ignored
        if (ShouldIgnoreException(exception))
            return;

        try
        {
            _isHandlingException = true;
            _lastErrorMessage = exception.HResult;

            _logger.LogError(exception, exception.ToMessage(Environment.StackTrace));

            // Safely stop busy indicator
            try
            {
                _busyService?.StopBusy();
            }
            catch (Exception busyEx)
            {
                _logger.LogError(busyEx, "Error stopping busy indicator");
            }

            // Show appropriate dialog based on exception type with improved error handling
            await EnsureMainThreadAsync(async () =>
            {
                await ShowExceptionDialogAsync(exception);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in exception handler");
        }
        finally
        {
            _isHandlingException = false;
        }
    }

    private static bool ShouldAlwaysProcess(Exception exception) =>
        exception is NotImplementedException ||
        exception is UnauthorizedAccessException ||
        exception is IOException ||
        exception is ArgumentException ||
        exception is ArgumentNullException;

    private static bool ShouldIgnoreException(Exception exception)
    {
        // Ignore cancellation exceptions
        if (exception is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested)
            return true;

        if (exception is OperationCanceledException)
            return true;

        // Ignore TaskCanceledException inside AggregateException
        if (exception is AggregateException ae &&
            ae.InnerExceptions?.Any(ex => ex is TaskCanceledException cancelEx && cancelEx.CancellationToken.IsCancellationRequested) == true)
            return true;

        // Ignore specific IO exceptions
        if (exception is IOException io && io.Message.Contains("The I/O operation has been aborted"))
            return true;

        // MAUI-specific exception filtering
        if (ShouldIgnoreMauiException(exception))
            return true;

        if (exception.InnerException is WebSocketException)
            return true;

        if (exception.Message.Equals(@"A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ()"))
            return true;

        // Ignore WinRT dialog-related exceptions that we're trying to handle
        if (IsWinRTDialogException(exception))
            return true;

        // Ignore dialog service exceptions to prevent recursion
        if (IsDialogServiceException(exception))
            return true;

        return false;
    }

    private static bool IsWinRTDialogException(Exception exception)
    {
        // Check for WinRT exceptions related to dialog operations
        if (exception is InvalidOperationException &&
            exception.Source == "WinRT.Runtime" &&
            exception.Message.Contains("Operation is not valid due to the current state of the object"))
        {
            return true;
        }

        if (exception is COMException comEx &&
            (comEx.Message.Contains("dialog") || comEx.Message.Contains("window")))
        {
            return true;
        }

        return false;
    }

    private static bool IsDialogServiceException(Exception exception)
    {
        // Check if exception originates from dialog service operations
        if (exception.StackTrace?.Contains("DialogService") == true)
            return true;

        if (exception.StackTrace?.Contains("DisplayAlert") == true)
            return true;

        // MAUI-specific dialog exceptions
        if (exception is InvalidOperationException &&
            (exception.Message.Contains("MainPage") ||
             exception.Message.Contains("Navigation")))
            return true;

        return false;
    }

    private static bool ShouldIgnoreMauiException(Exception exception)
    {
        // Platform-specific exception filtering
        if (exception is COMException comEx)
        {
            if (comEx.Message.Contains("Cannot find credential in Vault") ||
                comEx.Message.Contains("No such interface supported") ||
                comEx.Message.Contains("Element not found") ||
                comEx.Message.Contains("The parameter is incorrect"))
                return true;
        }

#if ANDROID
        if (exception is Java.Lang.Exception javaEx)
        {
            if (javaEx.Message?.Contains("Activity has been destroyed") == true)
                return true;
        }
#elif IOS
        if (exception.Message?.Contains("UIKit") == true && 
            exception.Message.Contains("main thread"))
            return true;
#endif

        return false;
    }

    private async Task ShowExceptionDialogAsync(Exception exception)
    {
        // Check if dialog service should attempt recovery
        if (!_dialogServiceAvailable && DateTime.Now - _lastDialogFailure > _dialogRecoveryPeriod)
        {
            _logger.LogInformation("Attempting to recover dialog service after {RecoveryPeriod}", _dialogRecoveryPeriod);
            _dialogServiceAvailable = true;
            _dialogFailureCount = 0;
        }

        // If dialog service is not available, just log and return
        if (!_dialogServiceAvailable)
        {
            _logger.LogError(exception, "Exception occurred but dialog service is unavailable: {Message}", exception.Message);
            await TryFallbackNotificationAsync(exception);
            return;
        }

        try
        {
            await ShowExceptionDialogInternalAsync(exception);

            // Reset failure count on successful dialog operation
            _dialogFailureCount = 0;
        }
        catch (Exception dialogEx) when (IsDialogFailureException(dialogEx))
        {
            await HandleDialogServiceFailureAsync(dialogEx, exception);
        }
        catch (Exception dialogEx)
        {
            _logger.LogError(dialogEx, "Unexpected error showing error dialog");
            _logger.LogError(exception, "Original exception: {Message}", exception.Message);
            await TryFallbackNotificationAsync(exception);
        }
    }

    private static bool IsDialogFailureException(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException when exception.Message.Contains("MainPage") => true,
            InvalidOperationException when exception.Message.Contains("Navigation") => true,
            InvalidOperationException when exception.Source == "WinRT.Runtime" => true,
            NullReferenceException when exception.StackTrace?.Contains("DisplayAlert") == true => true,
            TimeoutException => true,
            _ => false
        };
    }

    private async Task HandleDialogServiceFailureAsync(Exception dialogEx, Exception originalException)
    {
        _dialogFailureCount++;
        _lastDialogFailure = DateTime.Now;

        if (_dialogFailureCount >= 3)
        {
            _dialogServiceAvailable = false;
            _logger.LogError(dialogEx, "Dialog service disabled after {FailureCount} consecutive failures", _dialogFailureCount);
        }
        else
        {
            _logger.LogWarning(dialogEx, "Dialog service failure {FailureCount}/3", _dialogFailureCount);
        }

        _logger.LogError(originalException, "Original exception that couldn't be shown in dialog: {Message}", originalException.Message);
        await TryFallbackNotificationAsync(originalException);
    }

    private async Task TryFallbackNotificationAsync(Exception exception)
    {
        try
        {
            // Progressive fallback strategy
            var message = $"ERROR: {exception.GetType().Name}: {exception.Message}";

            // Try console output first
            try
            {
                Console.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);
                _logger.LogInformation("Fallback notification sent to console/debug for: {ExceptionType}", exception.GetType().Name);
                return;
            }
            catch (Exception consoleEx)
            {
                _logger.LogWarning(consoleEx, "Console fallback failed");
            }

            // Try file output as last resort
            try
            {
                var logPath = Path.Combine(FileSystem.AppDataDirectory, "exception_fallback.log");
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                await File.AppendAllTextAsync(logPath, logEntry);
                _logger.LogInformation("Fallback notification written to file: {LogPath}", logPath);
            }
            catch (Exception fileEx)
            {
                _logger.LogError(fileEx, "All fallback notification methods failed for: {ExceptionType}", exception.GetType().Name);
            }
        }
        catch (Exception fallbackEx)
        {
            // Last resort: just log that we tried
            _logger.LogCritical(fallbackEx, "Critical failure in fallback notification system");
        }
    }

    private async Task ShowExceptionDialogInternalAsync(Exception exception)
    {
        switch (exception)
        {
            case NotImplementedException:
                await _dialogService.ShowInformationAsync(LanguageService.Default.GetString(ExceptionConstants.WARNING_FUTURE_MODULE), "Features");
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                var message = unauthorizedAccessException.Message;
                if (message.StartsWith("[") && message.EndsWith("]"))
                    message = LanguageService.Default.GetString(message.Substring(1, message.Length - 2));
                await _dialogService.ShowErrorAsync(message);
                break;

            case IOException ioException:
                if (ioException.Message.Contains("The process cannot access the file") &&
                    ioException.Message.Contains("because it is being used by another process"))
                {
                    await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_FILE_IN_USE));
                }
                else
                {
#if !DEBUG
                    await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT));
#else
                    await _dialogService.ShowErrorAsync(ioException.Message);
#endif
                }
                break;

            case ArgumentException argumentException:
                await _dialogService.ShowWarningAsync(argumentException.Message);
                break;

            default:
#if !DEBUG
                await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT));
#else
                await _dialogService.ShowErrorAsync(exception.Message);
#endif
                break;
        }
    }

    private async Task EnsureMainThreadAsync(Func<Task> action)
    {
        try
        {
            if (MainThread.IsMainThread)
            {
                await action();
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(action);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action on main thread");
        }
    }

    #region Exception Event Handlers

    private async void OnFirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        if (_isDisposed) return;

        try
        {
            await HandleExceptionAsync(e.Exception);
        }
        catch (Exception handlerEx)
        {
            _logger?.LogError(handlerEx, "Error in first chance exception handler");
        }
    }

    private async void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (_isDisposed) return;

        if (e.ExceptionObject is Exception exception)
        {
            try
            {
                await HandleExceptionAsync(exception);
            }
            catch (Exception handlerEx)
            {
                _logger?.LogError(handlerEx, "Error in unhandled exception handler");
            }
        }
    }

    private async void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_isDisposed)
        {
            e.SetObserved();
            return;
        }

        try
        {
            await HandleExceptionAsync(e.Exception);
        }
        catch (Exception handlerEx)
        {
            _logger?.LogError(handlerEx, "Error in unobserved task exception handler");
        }

        e.SetObserved();
    }

#if ANDROID
    private async void OnAndroidUnhandledException(object sender, Android.Runtime.RaiseThrowableEventArgs e)
    {
        if (_isDisposed)
        {
            e.Handled = true;
            return;
        }

        e.Handled = true;

        var exception = new Exception(e.Exception?.Message ?? "Unknown Android exception", e.Exception);
        try
        {
            await HandleExceptionAsync(exception);
        }
        catch (Exception handlerEx)
        {
            _logger?.LogError(handlerEx, "Error in Android exception handler");
        }
    }
#elif WINDOWS
    private async void OnWindowsUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if (_isDisposed)
        {
            e.Handled = true;
            return;
        }

        e.Handled = true;

        try
        {
            await HandleExceptionAsync(e.Exception);
        }
        catch (Exception handlerEx)
        {
            _logger?.LogError(handlerEx, "Error in Windows exception handler");
        }
    }
#endif

    #endregion

    public void Dispose()
    {
        if (_isDisposed) return;

        _isDisposed = true;

        // Unregister ALL handlers from the single point of control
        AppDomain.CurrentDomain.FirstChanceException -= OnFirstChanceException;
        AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

#if ANDROID
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser -= OnAndroidUnhandledException;
#elif WINDOWS
        if (Microsoft.UI.Xaml.Application.Current != null)
        {
            Microsoft.UI.Xaml.Application.Current.UnhandledException -= OnWindowsUnhandledException;
        }
#endif

        _logger.LogTrace("Global exception handlers unregistered");
    }
}
