using System;

namespace ISynergy.Services
{
    public abstract class BaseService : IBaseService
    {
        public IBusyService Busy { get; }
        public ILanguageService Language { get; }
        public ISettingsServiceBase ApplicationSettings { get; }
        public ITelemetryService Telemetry { get; }
        public IDialogService Dialog { get; }
        public IUIVisualizerService UIVisualizer { get; }
        public INavigationService Navigation { get; }
        public IAuthenticationService Authentication { get; }
        public IInfoService Info { get; }
        public IConverterService Converter { get; }

        public BaseService(
            IBusyService busy,
            ILanguageService language,
            IAuthenticationService authentication,
            ISettingsServiceBase settings,
            ITelemetryService telemetry,
            IDialogService dialog,
            IUIVisualizerService uiVisualizer,
            INavigationService navigation,
            IInfoService info,
            IConverterService converter)
        {
            Busy = busy;
            Language = language;
            ApplicationSettings = settings;
            Telemetry = telemetry;
            Dialog = dialog;
            UIVisualizer = uiVisualizer;
            Navigation = navigation;
            Authentication = authentication;
            Info = Info;
            Converter = converter;
        }
    }
}
