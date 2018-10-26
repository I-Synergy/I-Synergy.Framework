using System;

namespace ISynergy.Services
{
    public abstract class BaseService : IBaseService
    {
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
