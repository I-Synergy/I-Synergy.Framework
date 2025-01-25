using ISynergy.Framework.Core.Events;

namespace ISynergy.Framework.Core.Abstractions.Services;
public interface IScopedContextService : IDisposable
{
    event EventHandler<ReturnEventArgs<bool>> ScopedChanged;
    void CreateNewScope();
    object GetService(Type serviceType);
    TService GetService<TService>();
    IServiceProvider ServiceProvider { get; }
}
