using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Core.Services;

public class ScopedContextService : IScopedContextService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;
    private bool _disposed;

    public ScopedContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        if (_serviceScope is null)
            _serviceScope = _serviceProvider.CreateScope();
    }

    public void CreateNewScope()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        // Create new scope before disposing old one
        var newScope = _serviceProvider.CreateScope();

        // Store old scope for disposal
        var oldScope = _serviceScope;

        // Set new scope
        _serviceScope = newScope;

        // Notify of scope change before disposal
        MessageService.Default.Send(new ScopeChangedMessage(true));

        // Dispose old scope if it exists
        if (oldScope != null)
        {
            var services = oldScope.ServiceProvider.GetRegisteredServices().EnsureNotNull();
            foreach (var descriptor in services)
            {
#if NET6_0_OR_GREATER
                if (descriptor.ServiceType.IsAssignableTo(typeof(IDisposable)))
#else
                if (typeof(IDisposable).IsAssignableFrom(descriptor.ServiceType))
#endif
                {
                    var service = oldScope.ServiceProvider.GetService(descriptor.ServiceType);
                    (service as IDisposable)?.Dispose();
                }
            }
            oldScope.Dispose();
        }
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    public object GetService(Type serviceType)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        return _serviceScope.ServiceProvider.GetService(serviceType) ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <typeparam name="TService">The type of the t service.</typeparam>
    /// <returns>TService.</returns>
    public TService GetService<TService>()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        return _serviceScope.ServiceProvider.GetService<TService>() ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Gets an instance of the service provider.
    /// </summary>
    /// <returns></returns>
    public IServiceProvider ServiceProvider
    {
        get
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ScopedContextService));

            return _serviceScope.ServiceProvider;
        }
    }

    public void Dispose()
    {
        if (_disposed || _serviceScope is null)
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

        _disposed = true;
    }
}
