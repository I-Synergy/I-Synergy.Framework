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
    private static IServiceProvider _staticServiceProvider;
    private static ServiceLocator _default;
    private readonly IScopedContextService _scopedContextService;

    public event EventHandler<ReturnEventArgs<bool>> ScopedChanged;

    public ServiceLocator(IServiceProvider currentServiceProvider)
    {
        _staticServiceProvider = currentServiceProvider;

        _scopedContextService = new ScopedContextService(currentServiceProvider);
        _scopedContextService.ScopedChanged += (s, e) => ScopedChanged?.Invoke(s, e);

        _default = this;
    }

    public static ServiceLocator Default => _default ?? new ServiceLocator(_staticServiceProvider);

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _staticServiceProvider = serviceProvider;
        _default = new ServiceLocator(serviceProvider);
    }

    // Delegate methods to _scopedContextService
    public TService GetService<TService>() => _scopedContextService.GetService<TService>();

    public object GetService(Type serviceType) => _scopedContextService.GetService(serviceType);

    public void CreateNewScope() => _scopedContextService.CreateNewScope();

    public IServiceProvider ServiceProvider => _scopedContextService.ServiceProvider;

    public void Dispose() => _scopedContextService.Dispose();
}
