using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.UI.Services;

public class ExceptionHandlerService : IExceptionHandlerService
{
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger;

    // Add application state tracking
    private bool _isApplicationInitialized = false;
    private readonly Queue<Exception> _startupExceptions = new Queue<Exception>();

    private int _lastErrorMessage = 0;
    private bool _isHandlingException = false;

    public ExceptionHandlerService(
        IBusyService busyService,
        IDialogService dialogService,
        ILogger<ExceptionHandlerService> logger)
    {
        _busyService = busyService;
        _dialogService = dialogService;
        _logger = logger;
    }

    // Add method to signal that app is initialized and ready for dialogs
    public void SetApplicationInitialized()
    {
        _isApplicationInitialized = true;

        // Process any exceptions that occurred during startup
        ProcessStartupExceptions();
    }

    private async void ProcessStartupExceptions()
    {
        while (_startupExceptions.Count > 0)
        {
            var exception = _startupExceptions.Dequeue();
            await HandleExceptionAsync(exception, true);
        }
    }

    public virtual async Task HandleExceptionAsync(Exception exception)
    {
        await HandleExceptionAsync(exception, false);
    }

    private async Task HandleExceptionAsync(Exception exception, bool isStartupExceptionReplay)
    {
        // Check if we're already handling an exception to prevent recursion
        if (_isHandlingException)
        {
            _logger.LogWarning("Recursive exception handling detected. Skipping dialog display.");
            _logger.LogError(exception, exception.ToMessage(Environment.StackTrace));
            return;
        }

        // Queue startup exceptions for later processing
        if (!_isApplicationInitialized && !isStartupExceptionReplay)
        {
            _logger.LogWarning("Application not fully initialized. Queuing exception for later handling.");
            _logger.LogError(exception, exception.ToMessage(Environment.StackTrace));
            _startupExceptions.Enqueue(exception);
            return;
        }

        if (exception.HResult.Equals(_lastErrorMessage))
            return;

        // Ignore the exception if it is a TaskCanceledException and the cancellation token is requested
        if (exception is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested)
            return;

        // Ignore the exception if it is an OperationCanceledException or a TaskCanceledException
        if (exception is OperationCanceledException)
            return;

        // Ignore the exception if it is a TaskCanceledException inside an aggregated exception and the cancellation token is requested
        if (exception is AggregateException ae && ae.InnerExceptions?.Any(ex => ex is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested) == true)
            return;

        if (exception is IOException io && io.Message.Contains("The I/O operation has been aborted"))
            return;

        // Ignore specific COMExceptions
        if (exception is COMException comEx)
        {
            // Ignore credential vault errors
            if (comEx.Message.Contains("Cannot find credential in Vault"))
                return;

            // Ignore WinRT interface errors that cause loops
            if (comEx.Message.Contains("No such interface supported"))
                return;

            // Add WinUI/WinRT specific exception handling
            if (comEx.Message.Contains("Element not found") ||
                comEx.Message.Contains("The parameter is incorrect"))
                return;
        }

        if (exception.InnerException is WebSocketException)
            return;

        if (exception.Message.Equals(@"A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ()"))
            return;

        try
        {
            _isHandlingException = true;

            _logger.LogError(exception, exception.ToMessage(Environment.StackTrace));

            _busyService.StopBusy();

            // Ensure UI operations run on UI thread
            await EnsureUiThreadAsync(async () =>
            {
                if (exception is NotImplementedException)
                {
                    await _dialogService.ShowInformationAsync(LanguageService.Default.GetString(ExceptionConstants.WARNING_FUTURE_MODULE), "Features");
                }
                else if (exception is UnauthorizedAccessException unauthorizedAccessException)
                {
                    var message = unauthorizedAccessException.Message;

                    if (message.StartsWith("[") && message.EndsWith("]"))
                        message = LanguageService.Default.GetString(message.Substring(1, message.Length - 2));

                    await _dialogService.ShowErrorAsync(message);
                }
                else if (exception is IOException iOException)
                {
                    if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                    {
                        await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_FILE_IN_USE));
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT));
                    }
                }
                else if (exception is ArgumentException argumentException)
                {
                    await _dialogService.ShowWarningAsync(argumentException.Message);
                }
                else if (exception is ArgumentNullException argumentNullException)
                {
                    await _dialogService.ShowWarningAsync(argumentNullException.Message);
                }
                else
                {
                    await _dialogService.ShowErrorAsync(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT));
                }
            });

            if (!exception.HResult.Equals(_lastErrorMessage))
                _lastErrorMessage = exception.HResult;
        }
        finally
        {
            _isHandlingException = false;
        }
    }

    // Helper method to ensure code runs on UI thread for WPF
    private async Task EnsureUiThreadAsync(Func<Task> action)
    {
        try
        {
            // Get the current dispatcher
            var dispatcher = System.Windows.Application.Current?.Dispatcher;

            if (dispatcher == null)
            {
                _logger.LogWarning("Could not get UI dispatcher. Dialog may not display correctly.");
                await action();
                return;
            }

            if (dispatcher.CheckAccess())
            {
                // We're already on the UI thread
                await action();
            }
            else
            {
                // We need to switch to the UI thread
                var taskCompletionSource = new TaskCompletionSource<bool>();

                await dispatcher.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        await action();
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
                    }
                }));

                await taskCompletionSource.Task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring UI thread execution");
            await action(); // Fallback to direct execution
        }
    }

}
