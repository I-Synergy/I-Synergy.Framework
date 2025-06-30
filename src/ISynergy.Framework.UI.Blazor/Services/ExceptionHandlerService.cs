using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Messages;
using ISynergy.Framework.UI.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace ISynergy.Framework.UI.Services;

public class ExceptionHandlerService : IExceptionHandlerService
{
    private readonly IBusyService _busyService;
    private readonly ILogger<ExceptionHandlerService> _logger;

    private int _lastErrorMessage = 0;
    private bool _isHandlingException = false;

    public ExceptionHandlerService(
        IBusyService busyService,
        ILogger<ExceptionHandlerService> logger)
    {
        _busyService = busyService ?? throw new ArgumentNullException(nameof(busyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Add method to signal that app is initialized and ready for dialogs
    public void SetApplicationInitialized()
    {
    }

    public virtual async Task HandleExceptionAsync(Exception exception)
    {
        Argument.IsNotNull(exception);

        // Check if we're already handling an exception to prevent recursion
        if (_isHandlingException)
        {
            _logger.LogTrace("Recursive exception handling detected. Skipping dialog display.");
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
            exception is ArgumentNullException ||
            exception is SocketException))
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

        if (exception.InnerException is WebSocketException)
            return;

        if (exception.Message.Equals(@"A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ()"))
            return;

        try
        {
            _isHandlingException = true;

            _busyService?.StopBusy();

            if (exception is NotImplementedException)
            {
                MessageService.Default.Send(new ShowInformationMessage(new MessageBoxRequest(ExceptionConstants.WARNING_FUTURE_MODULE, "Features")));
            }
            else if (exception is UnauthorizedAccessException unauthorizedAccessException)
            {
                var message = unauthorizedAccessException.Message;

                if (message.StartsWith("[") && message.EndsWith("]"))
                    message = LanguageService.Default.GetString(message.Substring(1, message.Length - 2));

                MessageService.Default.Send(new ShowErrorMessage(new MessageBoxRequest(message)));
            }
            else if (exception is IOException iOException)
            {
                if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                {
                    MessageService.Default.Send(new ShowErrorMessage(new MessageBoxRequest(LanguageService.Default.GetString(ExceptionConstants.ERROR_FILE_IN_USE))));
                }
                else
                {
                    MessageService.Default.Send(new ShowErrorMessage(new MessageBoxRequest(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT))));
                }
            }
            else if (exception is SocketException socketException)
            {
                MessageService.Default.Send(new ShowErrorMessage(new MessageBoxRequest(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT))));
            }
            else if (exception is ArgumentException argumentException)
            {
                MessageService.Default.Send(new ShowWarningMessage(new MessageBoxRequest(argumentException.Message)));
            }
            else if (exception is ArgumentNullException argumentNullException)
            {
                MessageService.Default.Send(new ShowWarningMessage(new MessageBoxRequest(argumentNullException.Message)));
            }
            else
            {
                MessageService.Default.Send(new ShowErrorMessage(new MessageBoxRequest(LanguageService.Default.GetString(ExceptionConstants.ERROR_DEFAULT))));
            }

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
