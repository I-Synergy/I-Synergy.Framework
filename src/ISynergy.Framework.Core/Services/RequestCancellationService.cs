using System.Collections.Concurrent;

namespace ISynergy.Framework.Core.Services;

public class RequestCancellationService
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _activeRequests = new();

    // Used by BaseRestService to register requests
    public void RegisterRequest(string requestId, CancellationTokenSource cts)
    {
        _activeRequests[requestId] = cts;
    }

    // Used by BaseRestService to remove completed requests
    public void RemoveRequest(string requestId)
    {
        if (_activeRequests.TryRemove(requestId, out var cts))
        {
            // Don't dispose here, as the BaseRestService handles disposal
        }
    }

    // Called during sign-out to cancel all pending requests
    public void CancelAllRequests()
    {
        foreach (var kvp in _activeRequests)
        {
            try
            {
                kvp.Value.Cancel();
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }
}