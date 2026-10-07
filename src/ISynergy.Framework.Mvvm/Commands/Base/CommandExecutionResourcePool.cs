using System.Collections.Concurrent;

namespace ISynergy.Framework.Mvvm.Commands.Base;

/// <summary>
/// Owns the process-wide, allocation-reducing resources shared by every <see cref="BaseAsyncRelayCommand"/>:
/// pooled weak references and continuation token sources, and the rounded timeout token cache.
/// </summary>
/// <remarks>
/// Extracted from <see cref="BaseAsyncRelayCommand"/> so that per-command lifecycle logic is not
/// entangled with the static pooling/caching machinery (including its cleanup timer and process-exit hook).
/// The public and protected surface of <see cref="BaseAsyncRelayCommand"/> is unchanged.
/// </remarks>
internal static class CommandExecutionResourcePool
{
    private const int MaxPoolSize = 50;

    private static readonly ConcurrentQueue<WeakReference<BaseAsyncRelayCommand>> _weakReferencePool = new();
    private static readonly ConcurrentQueue<CancellationTokenSource> _continuationCtsPool = new();

    private static readonly ConcurrentDictionary<TimeSpan, Lazy<CancellationTokenSource>> _timeoutTokenCache = new();

    private static readonly Timer _cacheCleanupTimer;

    static CommandExecutionResourcePool()
    {
        // Create a timer to clean up the token cache every minute
        _cacheCleanupTimer = new Timer(_ => CleanupTokenCache(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        // Ensure static resources are cleaned up when the process exits
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Cleanup();
    }

    /// <summary>
    /// Rents a weak reference that tracks <paramref name="command"/>.
    /// </summary>
    public static WeakReference<BaseAsyncRelayCommand> RentWeakReference(BaseAsyncRelayCommand command)
    {
        if (!_weakReferencePool.TryDequeue(out var weakReference) || weakReference is null)
            return new WeakReference<BaseAsyncRelayCommand>(command);

        weakReference.SetTarget(command);
        return weakReference;
    }

    /// <summary>
    /// Rents a continuation cancellation token source.
    /// </summary>
    public static CancellationTokenSource RentContinuationTokenSource() =>
        _continuationCtsPool.TryDequeue(out var cts) && cts is not null
            ? cts
            : new CancellationTokenSource();

    /// <summary>
    /// Returns pooled resources to their respective pools.
    /// </summary>
    public static void Return(WeakReference<BaseAsyncRelayCommand> weakReference, CancellationTokenSource cts)
    {
        // Reset the CTS if it was used
        if (cts.IsCancellationRequested)
        {
            cts = new CancellationTokenSource();
        }

        // Return to pools if not too large
        if (_weakReferencePool.Count < MaxPoolSize)
        {
            _weakReferencePool.Enqueue(weakReference);
        }

        if (_continuationCtsPool.Count < MaxPoolSize)
        {
            _continuationCtsPool.Enqueue(cts);
        }
        else
        {
            cts.Dispose();
        }
    }

    /// <summary>
    /// Gets or lazily creates the shared timeout token source for a rounded timeout.
    /// </summary>
    public static CancellationTokenSource GetOrCreateTimeoutTokenSource(TimeSpan roundedTimeout) =>
        _timeoutTokenCache
            .GetOrAdd(roundedTimeout, key => new Lazy<CancellationTokenSource>(() => new CancellationTokenSource(key)))
            .Value;

    /// <summary>
    /// Removes a timeout token source from the cache (for example after its token source was disposed).
    /// </summary>
    public static void RemoveTimeoutTokenSource(TimeSpan roundedTimeout) =>
        _timeoutTokenCache.TryRemove(roundedTimeout, out _);

    /// <summary>
    /// Releases all process-wide resources: the cleanup timer, the timeout cache, and the pools.
    /// </summary>
    public static void Cleanup()
    {
        _cacheCleanupTimer?.Dispose();

        foreach (var entry in _timeoutTokenCache)
        {
            if (entry.Value.IsValueCreated)
            {
                try
                {
                    entry.Value.Value.Dispose();
                }
                catch (Exception ex)
                {
                    // Ignore disposal errors during static cleanup
                    System.Diagnostics.Debug.WriteLine($"Error disposing timeout token: {ex.Message}");
                }
            }
        }

        _timeoutTokenCache.Clear();

        // Clean up pooled resources
        while (_weakReferencePool.TryDequeue(out _)) { }

        while (_continuationCtsPool.TryDequeue(out var cts))
        {
            cts.Dispose();
        }
    }

    /// <summary>
    /// Removes canceled timeout token sources from the cache periodically.
    /// </summary>
    private static void CleanupTokenCache()
    {
        foreach (var key in _timeoutTokenCache.Keys)
        {
            if (_timeoutTokenCache.TryGetValue(key, out var lazyCts) &&
                lazyCts.IsValueCreated &&
                lazyCts.Value.IsCancellationRequested)
            {
                _timeoutTokenCache.TryRemove(key, out _);
            }
        }
    }
}
