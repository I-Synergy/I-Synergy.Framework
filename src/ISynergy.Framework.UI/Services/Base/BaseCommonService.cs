using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class BaseCommonService.
    /// Implements the <see cref="IBaseCommonServices" />
    /// </summary>
    /// <seealso cref="IBaseCommonServices" />
    public abstract class BaseCommonService : IBaseCommonServices
    {
        /// <summary>
        /// Gets the busy service.
        /// </summary>
        /// <value>The busy service.</value>
        public IBusyService BusyService { get; }
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; }
        /// <summary>
        /// Gets the telemetry service.
        /// </summary>
        /// <value>The telemetry service.</value>
        public ITelemetryService TelemetryService { get; }
        /// <summary>
        /// Gets the messaging service.
        /// </summary>
        public IMessageService MessageService { get; }
        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        public IDialogService DialogService { get; }
        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        public INavigationService NavigationService { get; }
        /// <summary>
        /// Gets the information service.
        /// </summary>
        /// <value>The information service.</value>
        public IInfoService InfoService { get; }
        /// <summary>
        /// Gets the converter service.
        /// </summary>
        /// <value>The converter service.</value>
        public IConverterService ConverterService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommonService"/> class.
        /// </summary>
        /// <param name="busyService">The busy.</param>
        /// <param name="messageService"></param>
        /// <param name="languageService">The language.</param>
        /// <param name="telemetryService">The telemetry.</param>
        /// <param name="dialogService">The dialog.</param>
        /// <param name="navigationService">The navigation.</param>
        /// <param name="infoService">The information.</param>
        /// <param name="converterService">The converter.</param>
        protected BaseCommonService(
            IBusyService busyService,
            IMessageService messageService,
            ILanguageService languageService,
            ITelemetryService telemetryService,
            IDialogService dialogService,
            INavigationService navigationService,
            IInfoService infoService,
            IConverterService converterService)
        {
            BusyService = busyService;
            MessageService = messageService;
            LanguageService = languageService;
            TelemetryService = telemetryService;
            DialogService = dialogService;
            NavigationService = navigationService;
            InfoService = infoService;
            ConverterService = converterService;
        }
    }
}
