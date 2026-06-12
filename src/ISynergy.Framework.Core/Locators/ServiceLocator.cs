using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable S3010 // static fields intentionally set in constructor (ambient singleton pattern)

namespace ISynergy.Framework.Core.Locators;

/// <summary>
/// This class provides the ambient container for this application. If your
/// framework defines such an ambient container, use ServiceLocator.Current
/// to get it.
/// </summary>
public class ServiceLocator : IDisposable
{
    private static IServiceProvider? _serviceProvider;
    private static ServiceLocator? _default;
    private bool _disposed = false;

    public ServiceLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider; // NOSONAR - intentional singleton pattern; static field updated by design
        _default = this; // NOSONAR - intentional singleton pattern; static field updated by design
    }

    public static ServiceLocator Default => _default ?? new ServiceLocator(_serviceProvider ??
        throw new InvalidOperationException("ServiceProvider has not been initialized. Call SetLocatorProvider first."));

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _default = new ServiceLocator(serviceProvider);
    }

    /// <summary>
    /// Throws if the locator has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ServiceLocator));
    }

    public TService? GetService<TService>()
        where TService : class
    {
        ThrowIfDisposed();
        return _serviceProvider!.GetService<TService>();
    }

    public object? GetService(Type serviceType)
    {
        ThrowIfDisposed();
        return _serviceProvider!.GetService(serviceType);
    }

#pragma warning disable CS8603
    [return: NotNull]
    public TService GetRequiredService<TService>()
        where TService : notnull
    {
        ThrowIfDisposed();
        return _serviceProvider!.GetRequiredService<TService>()
            ?? throw new InvalidOperationException($"Service of type {typeof(TService).Name} could not be resolved.");
    }
#pragma warning restore CS8603

#pragma warning disable CS8603
    [return: NotNull]
    public object GetRequiredService(Type serviceType)
    {
        ThrowIfDisposed();
        return _serviceProvider!.GetRequiredService(serviceType)
            ?? throw new InvalidOperationException($"Service of type {serviceType.Name} could not be resolved.");
    }
#pragma warning restore CS8603

    public IServiceProvider ServiceProvider
    {
        get
        {
            ThrowIfDisposed();
            return _serviceProvider!;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _serviceProvider = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
