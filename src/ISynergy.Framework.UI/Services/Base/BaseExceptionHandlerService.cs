using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace ISynergy.Framework.UI.Services.Base;

public abstract class BaseExceptionHandlerService : IExceptionHandlerService
{
    protected readonly IBusyService _busyService;
    protected readonly IDialogService _dialogService;
    protected readonly ILogger _logger;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="busyService"></param>
    /// <param name="dialogService"></param>
    /// <param name="logger"></param>
    protected BaseExceptionHandlerService(
        IBusyService busyService,
        IDialogService dialogService,
        ILogger<BaseExceptionHandlerService> logger)
    {
        _busyService = busyService;
        _dialogService = dialogService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="exception"></param>
    public virtual async Task HandleExceptionAsync(Exception exception)
    {
        try
        {
            _logger.LogError(exception, exception.ToMessage(Environment.StackTrace));

            if (exception.InnerException is WebSocketException)
                return;

            if (exception.Message.Equals(@"A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ()"))
                return;


            // Set busyIndicator to false if it's true.
            _busyService.StopBusy();

            if (exception is NotImplementedException)
            {
                await _dialogService.ShowInformationAsync(LanguageService.Default.GetString("WarningFutureModule"), "Features");
            }
            else if (exception is UnauthorizedAccessException accessException)
            {
                await _dialogService.ShowErrorAsync(accessException.Message);
            }
            else if (exception is IOException iOException)
            {
                if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                {
                    await _dialogService.ShowErrorAsync(LanguageService.Default.GetString("ExceptionFileInUse"));
                }
                else
                {
                    await _dialogService.ShowErrorAsync(LanguageService.Default.GetString("ExceptionDefault"));
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
                await _dialogService.ShowErrorAsync(LanguageService.Default.GetString("ExceptionDefault"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.ToMessage(Environment.StackTrace));
        }
    }
}
