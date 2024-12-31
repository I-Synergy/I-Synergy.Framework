using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.Mvvm.Abstractions.Services.Base;

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
    /// Dispatcher service.
    /// </summary>
    IDispatcherService DispatcherService { get; }
    /// <summary>
    /// Restarts application.
    /// </summary>
    void RestartApplication();
    /// <summary>
    /// Quits the application.
    /// </summary>
    void QuitApplication();
}
