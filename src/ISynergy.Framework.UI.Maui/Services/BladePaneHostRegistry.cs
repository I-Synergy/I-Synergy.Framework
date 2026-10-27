namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Default singleton implementation of <see cref="IBladePaneHost"/>. It keeps a list of
/// weak references to the attached blade pane hosts so that a long-lived service
/// (registered as a singleton) does not keep a host control, and its whole visual tree,
/// alive after it has been removed from the visual tree.
/// </summary>
public sealed class BladePaneHostRegistry : IBladePaneHost
{
    private readonly object _sync = new();
    private readonly List<WeakReference> _hosts = [];

    /// <inheritdoc />
    public bool IsHostAttached
    {
        get
        {
            lock (_sync)
            {
                PruneDeadEntries();

                foreach (var reference in _hosts)
                {
                    if (reference.Target is not null)
                        return true;
                }

                return false;
            }
        }
    }

    /// <inheritdoc />
    public void Register(object host)
    {
        ArgumentNullException.ThrowIfNull(host);

        lock (_sync)
        {
            PruneDeadEntries();

            // Avoid duplicate registrations of the same instance.
            foreach (var reference in _hosts)
            {
                if (ReferenceEquals(reference.Target, host))
                    return;
            }

            _hosts.Add(new WeakReference(host));
        }
    }

    /// <inheritdoc />
    public void Unregister(object host)
    {
        if (host is null)
            return;

        lock (_sync)
        {
            for (var i = _hosts.Count - 1; i >= 0; i--)
            {
                var target = _hosts[i].Target;

                if (target is null || ReferenceEquals(target, host))
                    _hosts.RemoveAt(i);
            }
        }
    }

    private void PruneDeadEntries()
    {
        for (var i = _hosts.Count - 1; i >= 0; i--)
        {
            if (!_hosts[i].IsAlive)
                _hosts.RemoveAt(i);
        }
    }
}
