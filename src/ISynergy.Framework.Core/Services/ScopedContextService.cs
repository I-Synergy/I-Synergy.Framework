using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Services;

public class ScopedContextService : IScopedContextService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope? _serviceScope;
    private bool _disposed;

    public event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;

    private void RaiseScopedChanged() => ScopedChanged?.Invoke(this, new ReturnEventArgs<bool>(true));

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
        RaiseScopedChanged();

        // Dispose old scope if it exists
        if (oldScope != null)
        {
            var services = oldScope.ServiceProvider.GetRegisteredServices().EnsureNotNull();
            foreach (var descriptor in services)
            {
                if (descriptor.ServiceType.IsAssignableTo(typeof(IDisposable)))
                {
                    var service = oldScope.ServiceProvider.GetService(descriptor.ServiceType);
                    (service as IDisposable)?.Dispose();
                    service = null;
                }
            }
            oldScope.Dispose();
            oldScope = null;
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

        if (_serviceScope is null)
            throw new InvalidOperationException("Service scope is not initialized");

        return _serviceScope.ServiceProvider.GetService(serviceType) ??
               throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    public object GetRequiredService([NotNull] Type serviceType)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        if (_serviceScope is null)
            throw new InvalidOperationException("Service scope is not initialized");

        return _serviceScope.ServiceProvider.GetRequiredService(serviceType) ??
               throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");
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

        if (_serviceScope is null)
            throw new InvalidOperationException("Service scope is not initialized");

        return _serviceScope.ServiceProvider.GetService<TService>() ??
               throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered");
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <typeparam name="TService">The type of the t service.</typeparam>
    /// <returns>TService.</returns>
    [return: NotNull]
    public TService GetRequiredService<TService>() where TService : notnull
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        if (_serviceScope is null)
            throw new InvalidOperationException("Service scope is not initialized");

        return _serviceScope.ServiceProvider.GetRequiredService<TService>() ??
               throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered");
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

            if (_serviceScope is null)
                throw new InvalidOperationException("Service scope is not initialized");

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
            if (descriptor.ServiceType.IsAssignableTo(typeof(IDisposable)))
            {
                var service = _serviceScope.ServiceProvider.GetService(descriptor.ServiceType);
                (service as IDisposable)?.Dispose();
                service = null;
            }
        }

        _serviceScope.Dispose();
        _serviceScope = null;

        _disposed = true;
    }
}
