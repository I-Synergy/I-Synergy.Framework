using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Application = Microsoft.UI.Xaml.Application;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Dispatcher service.
/// </summary>
public class DispatcherService : IDispatcherService
{
    private readonly ILogger _logger;

    public DispatcherService(ILogger<DispatcherService> logger)
    {
        _logger = logger;
        _logger.LogDebug($"DispatcherService instance created with ID: {Guid.NewGuid()}");
    }

    public object Dispatcher
    {
        get
        {
            if (Application.Current is BaseApplication baseApplication &&
                baseApplication.MainWindow is not null &&
                baseApplication.MainWindow.DispatcherQueue is { } dispatcherQueue)
                return dispatcherQueue;
            return DispatcherQueue.GetForCurrentThread();
        }
    }

    /// <summary>
    /// Invokes action with the dispatcher.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool Invoke(Action action)
    {
        if (Dispatcher is DispatcherQueue dispatcherQueue)
            return dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        return false;
    }
}
