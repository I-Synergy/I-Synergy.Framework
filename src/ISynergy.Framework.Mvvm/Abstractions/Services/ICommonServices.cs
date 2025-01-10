using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface IBaseCommonServices
/// </summary>
public interface ICommonServices
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
    /// Gets the authentication service.
    /// </summary>
    IAuthenticationService AuthenticationService { get; }
    /// <summary>
    /// Gets the scoped context service.
    /// </summary>
    IScopedContextService ScopedContextService { get; }
    /// <summary>
    /// Gets the file service.
    /// </summary>
    /// <value>The file service.</value>
    IFileService<FileResult> FileService { get; }
    /// <summary>
    /// Restarts application.
    /// </summary>
    void RestartApplication();
    /// <summary>
    /// Quits the application.
    /// </summary>
    void QuitApplication();
}
