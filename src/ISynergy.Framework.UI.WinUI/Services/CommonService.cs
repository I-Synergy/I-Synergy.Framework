using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class BaseCommonService.
/// Implements the <see cref="ICommonServices" />
/// </summary>
/// <seealso cref="ICommonServices" />
public class CommonService : ICommonServices
{
    public ILanguageService LanguageService { get; }
    public IInfoService InfoService { get; }
    public IBusyService BusyService { get; }
    public IDialogService DialogService { get; }
    public INavigationService NavigationService { get; }
    public IAuthenticationService AuthenticationService { get; }
    public IScopedContextService ScopedContextService { get; }
    public IFileService<FileResult> FileService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonService"/> class.
    /// </summary>
    /// <param name="languageService"></param>
    /// <param name="infoService"></param>
    /// <param name="busyService">The busy.</param>
    /// <param name="dialogService">The dialog.</param>
    /// <param name="navigationService">The navigation.</param>
    /// <param name="authenticationService"></param>
    /// <param name="scopedContextService"></param>
    /// <param name="fileService"></param>
    protected CommonService(
        ILanguageService languageService,
        IInfoService infoService,
        IBusyService busyService,
        IDialogService dialogService,
        INavigationService navigationService,
        IAuthenticationService authenticationService,
        IScopedContextService scopedContextService,
        IFileService<FileResult> fileService)
    {
        LanguageService = languageService;
        InfoService = infoService;
        BusyService = busyService;
        DialogService = dialogService;
        NavigationService = navigationService;
        AuthenticationService = authenticationService;
        ScopedContextService = scopedContextService;
        FileService = fileService;
    }

    /// <summary>
    /// Restarts application.
    /// </summary>
    public void RestartApplication()
    {
        if (!string.IsNullOrEmpty(Environment.ProcessPath))
        {
            // Start a new instance of the application
            Process.Start(Environment.ProcessPath);
            // Close the current process
            QuitApplication();
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitApplication() =>
        Environment.Exit(Environment.ExitCode);
}
