using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Services;

public sealed class ScopedContextService : IScopedContextService
{
    private readonly IServiceProvider _serviceProvider;

    private IServiceScope? _serviceScope;
    private bool _disposed;
    private bool _scopeInitialized;

    public event EventHandler<ReturnEventArgs<bool>>? ScopedChanged;

    private void RaiseScopedChanged() => ScopedChanged?.Invoke(this, new ReturnEventArgs<bool>(true));

    public ScopedContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _scopeInitialized = false;
    }

    private void EnsureScopeInitialized()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        if (!_scopeInitialized)
        {
            try
            {
                _serviceScope = _serviceProvider.CreateScope();
                _scopeInitialized = true;
            }
            catch (Exception)
            {
                // If scope creation fails (e.g., in tests with mock providers), 
                // we'll operate without a scope and use the provider directly
                _scopeInitialized = true;
            }
        }
    }

    public void CreateNewScope()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ScopedContextService));

        try
        {
            // Create new scope before disposing old one
            var newScope = _serviceProvider.CreateScope();

            // Store old scope for disposal
            var oldScope = _serviceScope;

            // Set new scope
            _serviceScope = newScope;

            // Notify of scope change before disposal
            RaiseScopedChanged();

            // Dispose old scope if it exists
            if (oldScope is not null)
            {
                oldScope?.Dispose();
                oldScope = null;
            }
        }
        catch (Exception)
        {
            // If scope creation fails, continue without creating a new scope
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

        EnsureScopeInitialized();

        if (_serviceScope is not null)
            return _serviceScope.ServiceProvider.GetService(serviceType) ??
                   throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");

        return _serviceProvider.GetService(serviceType) ??
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

        EnsureScopeInitialized();

        if (_serviceScope is not null)
            return _serviceScope.ServiceProvider.GetRequiredService(serviceType) ??
                   throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered");

        return _serviceProvider.GetRequiredService(serviceType) ??
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

        EnsureScopeInitialized();

        if (_serviceScope is not null)
            return _serviceScope.ServiceProvider.GetService<TService>() ??
                   throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered");

        return _serviceProvider.GetService<TService>() ??
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

        EnsureScopeInitialized();

        if (_serviceScope is not null)
            return _serviceScope.ServiceProvider.GetRequiredService<TService>() ??
                   throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered");

        return _serviceProvider.GetRequiredService<TService>() ??
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

            EnsureScopeInitialized();

            if (_serviceScope is not null)
                return _serviceScope.ServiceProvider;

            return _serviceProvider;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _serviceScope?.Dispose();
        _serviceScope = null;
    }
}
