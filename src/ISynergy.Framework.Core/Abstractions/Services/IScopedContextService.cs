using ISynergy.Framework.Core.Events;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Abstractions.Services;
public interface IScopedContextService : IDisposable
{
    event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;
    void CreateNewScope();
    object GetService(Type serviceType);
    TService GetService<TService>();

    [return: NotNull]
    object GetRequiredService(Type serviceType);

    [return: NotNull] TService GetRequiredService<TService>() where TService : notnull;
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// The application root service provider (not scoped).
    /// Use this to create child scopes whose lifetimes are independent of
    /// the ScopedContextService's internal scope.
    /// </summary>
    IServiceProvider RootServiceProvider { get; }
}
