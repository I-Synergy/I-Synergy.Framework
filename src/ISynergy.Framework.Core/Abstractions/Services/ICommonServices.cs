namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Interface IBaseCommonServices
/// </summary>
public interface ICommonServices
{
    /// <summary>
    /// Gets the language service.
    /// </summary>
    ILanguageService LanguageService { get; }
    /// <summary>
    /// Gets the info service.
    /// </summary>
    IInfoService InfoService { get; }
    /// <summary>
    /// Gets the busy service.
    /// </summary>
    /// <value>The busy service.</value>
    IBusyService BusyService { get; }
    /// <summary>
    /// Gets the messenger service.
    /// </summary>
    IMessengerService MessengerService { get; }
    /// <summary>
    /// Gets the exception handler service.
    /// </summary>
    IExceptionHandlerService ExceptionHandlerService { get; }
    /// <summary>
    /// Gets the scoped context service.
    /// </summary>
    IScopedContextService ScopedContextService { get; }
    /// <summary>
    /// Restarts application.
    /// </summary>
    void RestartApplication();
    /// <summary>
    /// Quits the application.
    /// </summary>
    void QuitApplication();
}
