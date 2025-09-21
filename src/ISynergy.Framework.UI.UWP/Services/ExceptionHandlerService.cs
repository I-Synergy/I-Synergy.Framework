using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ISynergy.Framework.UI.Services;

public class ExceptionHandlerService : IExceptionHandlerService
{
    private readonly IBusyService _busyService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<ExceptionHandlerService> _logger;
    private readonly Queue<Exception> _startupExceptions = new Queue<Exception>();

    private int _lastErrorMessage = 0;
    private bool _isHandlingException = false;
    private bool _isApplicationInitialized = false;
    private readonly int _maxStartupExceptions = 50; // Limit stored exceptions

    public ExceptionHandlerService(
        IBusyService busyService,
        IDialogService dialogService,
        ILogger<ExceptionHandlerService> logger)
    {
        _busyService = busyService ?? throw new ArgumentNullException(nameof(busyService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        try
        {
            while (_startupExceptions.Count > 0)
            {
                var exception = _startupExceptions.Dequeue();

                await HandleExceptionAsync(exception, true);

                // Add a small delay to prevent UI freezing when processing many exceptions
                await Task.Delay(50);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing startup exceptions");
        }
    }

    public virtual Task HandleExceptionAsync(Exception exception) =>
        HandleExceptionAsync(exception, false);

    private async Task HandleExceptionAsync(Exception exception, bool isStartupExceptionReplay)
    {
        Argument.IsNotNull(exception);

        // Check if we're already handling an exception to prevent recursion
        if (_isHandlingException)
        {
            _logger.LogTrace("Recursive exception handling detected. Skipping dialog display.");
            return;
        }

        // Queue startup exceptions for later processing
        if (!_isApplicationInitialized && !isStartupExceptionReplay)
        {
            _logger.LogTrace("Application not fully initialized. Queuing exception for later handling.");

            // Limit the number of queued exceptions to prevent memory issues
            if (_startupExceptions.Count < _maxStartupExceptions)
                _startupExceptions.Enqueue(exception);

            return;
        }

        // Check for duplicate error messages
        var isDuplicateException = exception.HResult.Equals(_lastErrorMessage) && exception.Message == exception.Message;

        // Skip duplicate checks for NotImplementedException
        if (isDuplicateException && !(
            exception is NotImplementedException ||
            exception is UnauthorizedAccessException ||
            exception is IOException ||
            exception is ArgumentException ||
            exception is ArgumentNullException))
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

            // Safely stop busy indicator
            try
            {
                _busyService?.StopBusy();
            }
            catch (Exception busyEx)
            {
                _logger.LogError(busyEx, "Error stopping busy indicator");
            }

            // Ensure UI operations run on UI thread with timeout
            var uiTask = CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
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
                }
                catch (Exception dialogEx)
                {
                    _logger.LogError(dialogEx, "Error showing error dialog");
                }
            });

            // Add timeout to prevent hanging
            var timeoutTask = Task.Delay(5000);
            var completedTask = await Task.WhenAny(uiTask, timeoutTask);

            if (completedTask == timeoutTask)
                _logger.LogWarning("Dialog display timed out after 5 seconds");

            if (!exception.HResult.Equals(_lastErrorMessage))
                _lastErrorMessage = exception.HResult;
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
}
