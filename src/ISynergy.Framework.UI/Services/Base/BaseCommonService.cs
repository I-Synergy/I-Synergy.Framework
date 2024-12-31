using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class BaseCommonService.
/// Implements the <see cref="IBaseCommonServices" />
/// </summary>
/// <seealso cref="IBaseCommonServices" />
public abstract class BaseCommonService : IBaseCommonServices
{
    /// <summary>
    /// Gets the busy service.
    /// </summary>
    /// <value>The busy service.</value>
    public IBusyService BusyService { get; }

    /// <summary>
    /// Gets the dialog service.
    /// </summary>
    /// <value>The dialog service.</value>
    public IDialogService DialogService { get; }

    /// <summary>
    /// Gets the navigation service.
    /// </summary>
    /// <value>The navigation service.</value>
    public INavigationService NavigationService { get; }

    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public IDispatcherService DispatcherService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCommonService"/> class.
    /// </summary>
    /// <param name="busyService">The busy.</param>
    /// <param name="dialogService">The dialog.</param>
    /// <param name="navigationService">The navigation.</param>
    /// <param name="dispatcherService"></param>
    protected BaseCommonService(
        IBusyService busyService,
        IDialogService dialogService,
        INavigationService navigationService,
        IDispatcherService dispatcherService)
    {
        BusyService = busyService;
        DialogService = dialogService;
        NavigationService = navigationService;
        DispatcherService = dispatcherService;
    }

    /// <summary>
    /// Restarts application.
    /// </summary>
    public abstract void RestartApplication();

    /// <summary>
    /// Quits the application.
    /// </summary>
    public abstract void QuitApplication();
}
