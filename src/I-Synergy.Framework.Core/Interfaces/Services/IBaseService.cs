using System;

namespace ISynergy.Services
{
    public interface IBaseService
    {
        IBusyService BusyService { get; }
        ILanguageService LanguageService { get; }
        IAuthenticationService AuthenticationService { get; }
        IBaseSettingsService BaseSettingsService { get; }
        ITelemetryService TelemetryService { get; }
        IDialogService DialogService { get; }
        IUIVisualizerService UIVisualizerService { get; }
        INavigationService NavigationService { get; }
        IInfoService InfoService { get; }
        IConverterService ConverterService { get; }
    }
}
