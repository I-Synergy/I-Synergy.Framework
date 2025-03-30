using ISynergy.Framework.Core.Events;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Abstractions.Services;
public interface IScopedContextService : IDisposable
{
    event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;
    void CreateNewScope();
    object GetService(Type serviceType);
    [return: NotNull] object GetRequiredService(Type serviceType);
    TService GetService<TService>();
    [return: NotNull] TService GetRequiredService<TService>() where TService : notnull;
    IServiceProvider ServiceProvider { get; }
}
