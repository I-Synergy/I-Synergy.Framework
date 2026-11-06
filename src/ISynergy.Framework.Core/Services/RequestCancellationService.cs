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
            try
            {
                cts.Cancel();
            }
            catch
            {
                // Ignore errors during cancellation
            }

            cts.Dispose();
        }
    }

    // Called during sign-out to cancel all pending requests
    public void CancelAllRequests()
    {
        foreach (var cancellationTokenSource in _activeRequests.Select(kvp => kvp.Value))
        {
            try
            {
                cancellationTokenSource.Cancel();
            }
            catch
            {
                // Ignore errors during cleanup
            }

            cancellationTokenSource.Dispose();
        }

        _activeRequests.Clear();
    }
}