using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

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

        // Try to get IScopedContextService from DI container first (DIP compliance)
        // If not registered, create a new instance as fallback
        _scopedContextService = serviceProvider.GetService<IScopedContextService>() 
            ?? new ScopedContextService(serviceProvider);
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

#pragma warning disable CS8603
    [return: NotNull]
    public TService GetRequiredService<TService>()
        where TService : notnull
    {
        return _scopedContextService.GetRequiredService<TService>()
            ?? throw new InvalidOperationException($"Service of type {typeof(TService).Name} could not be resolved.");
    }
#pragma warning restore CS8603

#pragma warning disable CS8603
    [return: NotNull]
    public object GetRequiredService(Type serviceType)
    {
        return _scopedContextService.GetRequiredService(serviceType)
            ?? throw new InvalidOperationException($"Service of type {serviceType.Name} could not be resolved.");
    }
#pragma warning restore CS8603

    public void CreateNewScope() => _scopedContextService.CreateNewScope();

    public IServiceProvider ServiceProvider => _scopedContextService.ServiceProvider;

    public void Dispose() => _scopedContextService.Dispose();
}
