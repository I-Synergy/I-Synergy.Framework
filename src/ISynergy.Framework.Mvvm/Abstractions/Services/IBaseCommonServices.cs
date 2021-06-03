using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IBaseCommonServices
    /// </summary>
    public interface IBaseCommonServices
    {
        /// <summary>
        /// Gets the busy service.
        /// </summary>
        /// <value>The busy service.</value>
        IBusyService BusyService { get; }
        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        ILanguageService LanguageService { get; }
        /// <summary>
        /// Gets the application settings service.
        /// </summary>
        /// <value>The application settings service.</value>
        ISettingsService SettingsService { get; }
        /// <summary>
        /// Gets the telemetry service.
        /// </summary>
        /// <value>The telemetry service.</value>
        ITelemetryService TelemetryService { get; }
        /// <summary>
        /// Gets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        IDialogService DialogService { get; }
        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        INavigationService NavigationService { get; }
        /// <summary>
        /// Gets the information service.
        /// </summary>
        /// <value>The information service.</value>
        IInfoService InfoService { get; }
        /// <summary>
        /// Gets the converter service.
        /// </summary>
        /// <value>The converter service.</value>
        IConverterService ConverterService { get; }
    }
}
