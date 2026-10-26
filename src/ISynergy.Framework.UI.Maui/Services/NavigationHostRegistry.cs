using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Default singleton implementation of <see cref="INavigationHostRegistry"/>. It keeps a weak
/// reference to the attached host so the long-lived registry never keeps a shell control (and its
/// whole visual tree) alive after it has been removed from the visual tree.
/// </summary>
public sealed class NavigationHostRegistry : INavigationHostRegistry
{
    private readonly object _sync = new();
    private WeakReference? _host;

    /// <inheritdoc />
    public INavigationHost? Current
    {
        get
        {
            lock (_sync)
            {
                if (_host?.Target is INavigationHost host)
                    return host;

                return null;
            }
        }
    }

    /// <inheritdoc />
    public void Register(INavigationHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        lock (_sync)
        {
            _host = new WeakReference(host);
        }
    }

    /// <inheritdoc />
    public void Unregister(INavigationHost host)
    {
        if (host is null)
            return;

        lock (_sync)
        {
            if (_host?.Target is not null && !ReferenceEquals(_host.Target, host))
                return;

            _host = null;
        }
    }

    /// <summary>
    /// Resolves the registered host, tracing a lookup failure instead of throwing so navigation
    /// can report a missing shell rather than crashing on a disposed service provider.
    /// </summary>
    internal static INavigationHost? Resolve()
    {
        try
        {
            return IPlatformApplication.Current?.Services?.GetService<INavigationHostRegistry>()?.Current;
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Could not resolve the navigation host: {ex}");
            return null;
        }
    }
}
