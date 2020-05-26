namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IBaseCommonServices
    {
        IBusyService BusyService { get; }
        ILanguageService LanguageService { get; }
        ILoginService LoginService { get; }
        IApplicationSettingsService ApplicationSettingsService { get; }
        ITelemetryService TelemetryService { get; }
        IDialogService DialogService { get; }
        IUIVisualizerService UIVisualizerService { get; }
        INavigationService NavigationService { get; }
        IInfoService InfoService { get; }
        IConverterService ConverterService { get; }
    }
}
