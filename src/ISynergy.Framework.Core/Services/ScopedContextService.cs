using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Core.Services;

public class ScopedContextService : IScopedContextService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;

    public ScopedContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        if (_serviceScope is null)
            _serviceScope = _serviceProvider.CreateScope();
    }

    public void CreateNewScope()
    {
        // Dispose existing scope if it exists
        Dispose();

        // Create new scope
        _serviceScope = _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    public object GetService(Type serviceType) =>
        _serviceScope.ServiceProvider.GetService(serviceType) ?? throw new InvalidOperationException();

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <typeparam name="TService">The type of the t service.</typeparam>
    /// <returns>TService.</returns>
    public TService GetService<TService>() =>
        _serviceScope.ServiceProvider.GetService<TService>() ?? throw new InvalidOperationException();

    /// <summary>
    /// Gets an instance of the service provider.
    /// </summary>
    /// <returns></returns>
    public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

    public void Dispose()
    {
        if (_serviceScope is null)
            return;

        var services = _serviceScope.ServiceProvider.GetRegisteredServices().EnsureNotNull();
        foreach (var descriptor in services)
        {
#if NET6_0_OR_GREATER
            if (descriptor.ServiceType.IsAssignableTo(typeof(IDisposable)))
#else
            if (typeof(IDisposable).IsAssignableFrom(descriptor.ServiceType))
#endif
            {
                var service = _serviceScope.ServiceProvider.GetService(descriptor.ServiceType);
                (service as IDisposable)?.Dispose();
            }
        }

        _serviceScope.Dispose();
        _serviceScope = null;
    }
}
