using GalaSoft.MvvmLight.Messaging;

namespace ISynergy.Services
{
    public abstract class BaseService : IBaseService
    {
        public IMessenger Messenger { get; }
        public IBusyService BusyService { get; }
        public ILanguageService LanguageService { get; }
        public IBaseSettingsService BaseSettingsService { get; }
        public ITelemetryService TelemetryService { get; }
        public IDialogService DialogService { get; }
        public IUIVisualizerService UIVisualizerService { get; }
        public INavigationService NavigationService { get; }
        public ILoginService LoginService { get; }
        public IInfoService InfoService { get; }
        public IConverterService ConverterService { get; }

        public BaseService(
            IMessenger messenger,
            IBusyService busy,
            ILanguageService language,
            ILoginService loginService,
            IBaseSettingsService settings,
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
            BaseSettingsService = settings;
            TelemetryService = telemetry;
            DialogService = dialog;
            UIVisualizerService = uiVisualizer;
            NavigationService = navigation;
            LoginService = loginService;
            InfoService = InfoService;
            ConverterService = converter;
        }
    }
}
