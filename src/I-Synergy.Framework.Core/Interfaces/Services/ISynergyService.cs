using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Services
{
    public interface ISynergyService
    {
        IServiceProvider ServiceProvider { get; }
        IBusyService Busy { get; }
        ILanguageService Language { get; }
        ISettingsServiceBase Settings { get; }
        ITelemetryService Telemetry { get; }
        IDialogService Dialog { get; }
        IUIVisualizerService UIVisualizer { get; }
        INavigationService Navigation { get; }
        IWindowService Window { get; }
        IInfoService Info { get; }
    }
}
