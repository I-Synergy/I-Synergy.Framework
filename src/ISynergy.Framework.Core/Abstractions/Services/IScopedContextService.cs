using ISynergy.Framework.Core.Events;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Abstractions.Services;
public interface IScopedContextService : IDisposable
{
    event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;
    void CreateNewScope();

    [RequiresUnreferencedCode("Non-generic service resolution by runtime Type is not AOT-safe. Use GetService<TService>() instead.")]
    [RequiresDynamicCode("Non-generic service resolution by runtime Type requires dynamic code. Use GetService<TService>() instead.")]
    object GetService(Type serviceType);

    [return: NotNull]
    [RequiresUnreferencedCode("Non-generic service resolution by runtime Type is not AOT-safe. Use GetRequiredService<TService>() instead.")]
    [RequiresDynamicCode("Non-generic service resolution by runtime Type requires dynamic code. Use GetRequiredService<TService>() instead.")]
    object GetRequiredService(Type serviceType);

    TService GetService<TService>();
    [return: NotNull] TService GetRequiredService<TService>() where TService : notnull;
    IServiceProvider ServiceProvider { get; }
}
