using ISynergy.Framework.Core.Abstractions.Services;
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
    /// Gets the language service.
    /// </summary>
    public ILanguageService LanguageService { get; }
    /// <summary>
    /// Gets the info service.
    /// </summary>
    public IInfoService InfoService { get; }
    /// <summary>
    /// Gets the busy service.
    /// </summary>
    /// <value>The busy service.</value>
    public IBusyService BusyService { get; }

    /// <summary>
    /// Gets the messenger service.
    /// </summary>
    public IMessengerService MessengerService { get; }

    /// <summary>
    /// Gets the scoped context service.
    /// </summary>
    public IScopedContextService ScopedContextService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonServices"/> class.
    /// </summary>
    /// <param name="languageService"></param>
    /// <param name="infoService"></param>
    /// <param name="busyService">The busy.</param>
    /// <param name="messengerService"></param>
    /// <param name="scopedContextService"></param>
    public CommonServices(
        ILanguageService languageService,
        IInfoService infoService,
        IBusyService busyService,
        IMessengerService messengerService,
        IScopedContextService scopedContextService)
    {
        LanguageService = languageService;
        InfoService = infoService;
        BusyService = busyService;
        MessengerService = messengerService;
        ScopedContextService = scopedContextService;
    }

    /// <summary>
    /// Restarts application.
    /// </summary>
    public void RestartApplication()
    {
        if (Environment.ProcessPath == null)
            return;

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
