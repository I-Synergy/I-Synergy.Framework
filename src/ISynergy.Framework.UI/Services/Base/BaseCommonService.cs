using ISynergy.Framework.Mvvm.Messaging;
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
        /// Gets the messenger.
        /// </summary>
        /// <value>The messenger.</value>
        public IMessenger Messenger { get; }
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
        /// Gets the application settings service.
        /// </summary>
        /// <value>The application settings service.</value>
        public IApplicationSettingsService ApplicationSettingsService { get; }
        /// <summary>
        /// Gets the telemetry service.
        /// </summary>
        /// <value>The telemetry service.</value>
        public ITelemetryService TelemetryService { get; }
        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        public IDialogService DialogService { get; }
        /// <summary>
        /// Gets the UI visualizer service.
        /// </summary>
        /// <value>The UI visualizer service.</value>
        public IUIVisualizerService UIVisualizerService { get; }
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
        /// <param name="messenger">The messenger.</param>
        /// <param name="busy">The busy.</param>
        /// <param name="language">The language.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="dialog">The dialog.</param>
        /// <param name="uiVisualizer">The UI visualizer.</param>
        /// <param name="navigation">The navigation.</param>
        /// <param name="info">The information.</param>
        /// <param name="converter">The converter.</param>
        protected BaseCommonService(
            IMessenger messenger,
            IBusyService busy,
            ILanguageService language,
            IApplicationSettingsService settings,
            ITelemetryService telemetry,
            IDialogService dialog,
            IUIVisualizerService uiVisualizer,
            INavigationService navigation,
            IInfoService info,
            IConverterService converter)
        {
            Messenger = messenger;
            BusyService = busy;
            LanguageService = language;
            ApplicationSettingsService = settings;
            TelemetryService = telemetry;
            DialogService = dialog;
            UIVisualizerService = uiVisualizer;
            NavigationService = navigation;
            InfoService = info;
            ConverterService = converter;
        }
    }
}
