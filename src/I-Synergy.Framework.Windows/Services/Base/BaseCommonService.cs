using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.Windows.Services
{
    public abstract class BaseCommonService : IBaseCommonServices
    {
        public IMessenger Messenger { get; }
        public IBusyService BusyService { get; }
        public ILanguageService LanguageService { get; }
        public IApplicationSettingsService ApplicationSettingsService { get; }
        public ITelemetryService TelemetryService { get; }
        public IDialogService DialogService { get; }
        public IUIVisualizerService UIVisualizerService { get; }
        public INavigationService NavigationService { get; }
        public ILoginService LoginService { get; }
        public IInfoService InfoService { get; }
        public IConverterService ConverterService { get; }

        protected BaseCommonService(
            IMessenger messenger,
            IBusyService busy,
            ILanguageService language,
            ILoginService loginService,
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
            LoginService = loginService;
            InfoService = info;
            ConverterService = converter;
        }
    }
}
