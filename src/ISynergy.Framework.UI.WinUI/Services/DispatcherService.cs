﻿using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Dispatching;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Dispatcher service.
/// </summary>
public class DispatcherService : IDispatcherService
{
    public object Dispatcher
    {
        get
        {
            if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is { } dispatcherQueue)
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
        if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is { } dispatcherQueue)
            return dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        if (DispatcherQueue.GetForCurrentThread() is { } dispatcher)
            return dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        return false;
    }
}
