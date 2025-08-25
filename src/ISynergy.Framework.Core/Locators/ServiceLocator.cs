using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.Core.Locators;

/// <summary>
/// This class provides the ambient container for this application. If your
/// framework defines such an ambient container, use ServiceLocator.Current
/// to get it.
/// </summary>
public class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;
    private static ServiceLocator? _default;
    private readonly IScopedContextService _scopedContextService;

    public event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;

    public ServiceLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _scopedContextService = new ScopedContextService(serviceProvider);
        _scopedContextService.ScopedChanged += (s, e) => ScopedChanged?.Invoke(s, e);

        _default = this;
    }

    public static ServiceLocator Default => _default ?? new ServiceLocator(_serviceProvider ??
        throw new InvalidOperationException("ServiceProvider has not been initialized. Call SetLocatorProvider first."));

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _default = new ServiceLocator(serviceProvider);
    }

    public TService GetRequiredService<TService>() where TService : notnull => _scopedContextService.GetRequiredService<TService>();

    public object GetRequiredService(Type serviceType) => _scopedContextService.GetRequiredService(serviceType);

    public void CreateNewScope() => _scopedContextService.CreateNewScope();

    public IServiceProvider ServiceProvider => _scopedContextService.ServiceProvider;

    public void Dispose() => _scopedContextService.Dispose();
}
