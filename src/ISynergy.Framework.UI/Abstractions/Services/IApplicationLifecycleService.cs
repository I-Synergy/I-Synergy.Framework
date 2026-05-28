namespace ISynergy.Framework.UI.Abstractions.Services;
public interface IApplicationLifecycleService : IDisposable
{
    event EventHandler<EventArgs>? ApplicationUIReady;
    event EventHandler<EventArgs>? ApplicationInitialized;
    event EventHandler<EventArgs>? ApplicationLoaded;

    void SignalApplicationUIReady();
    void SignalApplicationInitialized();
    void SignalApplicationLoaded();

    bool IsApplicationUIReady { get; }
    bool IsApplicationInitialized { get; }
    bool IsApplicationLoaded { get; }
}
