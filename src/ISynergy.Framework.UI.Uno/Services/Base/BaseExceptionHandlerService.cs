using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Windows.Networking.Connectivity;

namespace ISynergy.Framework.UI.Services
{
    public class BaseExceptionHandlerService : IExceptionHandlerService
    {
        private readonly IBusyService _busyService;
        private readonly ILanguageService _languageService;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="busyService"></param>
        /// <param name="dialogService"></param>
        /// <param name="languageService"></param>
        /// <param name="logger"></param>
        public BaseExceptionHandlerService(
            IBusyService busyService,
            IDialogService dialogService,
            ILanguageService languageService,
            ILogger<BaseExceptionHandlerService> logger)
        {
            _busyService = busyService;
            _languageService = languageService;
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
                _logger.LogError(exception.Message);

                if (exception.InnerException is WebSocketException)
                    return;

                if (exception.Message.Equals(@"A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ()"))
                    return;


                // Set busyIndicator to false if it's true.
                _busyService.EndBusy();

                var connections = NetworkInformation.GetInternetConnectionProfile();

                if (connections?.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
                {
                    await _dialogService.ShowInformationAsync(
                        _languageService.GetString("ExceptionInternetConnection"));
                }
                else
                {
                    if (exception is ApiException apiException)
                    {
                        await _dialogService.ShowErrorAsync(apiException.Message);
                    }
                    else if (exception is NotImplementedException)
                    {
                        await _dialogService.ShowInformationAsync(_languageService.GetString("ExceptionFutureModule"));
                    }
                    else if (exception is UnauthorizedAccessException accessException)
                    {
                        await _dialogService.ShowErrorAsync(accessException.Message);
                    }
                    else if (exception is IOException iOException)
                    {
                        if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                        {
                            await _dialogService.ShowErrorAsync(_languageService.GetString("ExceptionFileInUse"));
                        }
                        else
                        {
                            await _dialogService.ShowErrorAsync(_languageService.GetString("ExceptionDefault"));
                        }
                    }
                    else if (exception is ArgumentException argumentException)
                    {
                        await _dialogService.ShowWarningAsync(string.Format(_languageService.GetString("ExceptionArgumentIsNull"), argumentException.ParamName));
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(_languageService.GetString("ExceptionDefault"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
            }
        }
    }
}
