using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class SynergyService : ISynergyService
    {
        public IServiceProvider ServiceProvider { get; }
        public IBusyService Busy { get; }
        public ILanguageService Language { get; }
        public ISettingsServiceBase Settings { get; }
        public ITelemetryService Telemetry { get; }
        public IDialogService Dialog { get; }
        public IUIVisualizerService UIVisualizer { get; }
        public INavigationService Navigation { get; }
        public IAuthenticationService Authentication { get; }
        public IFileService File { get; }
        public IWindowService Window { get; }
        public IInfoService Info { get; }

        public SynergyService(
            IServiceProvider serviceProvider,
            IBusyService busy,
            ILanguageService language,
            ISettingsServiceBase settings,
            ITelemetryService telemetry,
            IDialogService dialog,
            IUIVisualizerService uiVisualizer,
            INavigationService navigation,
            IAuthenticationService authentication,
            IFileService file,
            IWindowService window,
            IInfoService info)
        {
            ServiceProvider = serviceProvider;
            Busy = busy;
            Language = language;
            Settings = settings;
            Telemetry = telemetry;
            Dialog = dialog;
            UIVisualizer = uiVisualizer;
            Navigation = navigation;
            Authentication = authentication;
            File = file;
            Window = window;
            Info = Info;
        }
    }
}
