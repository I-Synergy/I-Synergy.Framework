using System;

namespace ISynergy.Services
{
    public interface IBaseService
    {
        IServiceProvider ServiceProvider { get; }
        IBusyService Busy { get; }
        ILanguageService Language { get; }
        IAuthenticationService Authentication { get; }
        ISettingsServiceBase ApplicationSettings { get; }
        ITelemetryService Telemetry { get; }
        IDialogService Dialog { get; }
        IUIVisualizerService UIVisualizer { get; }
        INavigationService Navigation { get; }
        IInfoService Info { get; }
        IConverterService Converter { get; }
    }
}
