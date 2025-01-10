using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using System.Diagnostics;

namespace Sample.Services;

/// <summary>
/// Class BaseCommonService.
/// Implements the <see cref="ICommonServices" />
/// </summary>
/// <seealso cref="ICommonServices" />
public class CommonServices : ICommonServices
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
    /// Gets the authentication service.
    /// </summary>
    /// <value>The authentication service.</value>
    public IAuthenticationService AuthenticationService { get; }

    /// <summary>
    /// Gets the scoped context service.
    /// </summary>
    public IScopedContextService ScopedContextService { get; }

    /// <summary>
    /// Gets the file service.
    /// </summary>
    /// <value>The file service.</value>
    public IFileService<FileResult> FileService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonServices"/> class.
    /// </summary>
    /// <param name="busyService">The busy.</param>
    /// <param name="dialogService">The dialog.</param>
    /// <param name="navigationService">The navigation.</param>
    /// <param name="authenticationService"></param>
    /// <param name="scopedContextService"></param>
    /// <param name="fileService"></param>
    /// <param name="dispatcherService"></param>
    public CommonServices(
        IBusyService busyService,
        IDialogService dialogService,
        INavigationService navigationService,
        IAuthenticationService authenticationService,
        IScopedContextService scopedContextService,
        IFileService<FileResult> fileService,
        IDispatcherService dispatcherService)
    {
        BusyService = busyService;
        DialogService = dialogService;
        NavigationService = navigationService;
        AuthenticationService = authenticationService;
        ScopedContextService = scopedContextService;
        FileService = fileService;
        DispatcherService = dispatcherService;
    }

    /// <summary>
    /// Restarts application.
    /// </summary>
    public void RestartApplication()
    {
        // Start a new instance of the application
        Process.Start(Environment.ProcessPath);
        // Close the current process
        QuitApplication();
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication() =>
        Environment.Exit(Environment.ExitCode);
}
