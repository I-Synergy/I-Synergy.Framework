using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Dispatching;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// This dispatcher service is used to dispatch actions to the UI thread.
/// Because of the use of Application.Current.Dispatcher, this dispatcher cannot be used in a multi-window environment.
/// For multi-window environments, use the DispatcherService with the Window parameter.
/// DO NOT use EITHER MainThread nor Application.Current.Dispatcher in a multi-window app.
/// You MUST get hold of a UI object on the window being touched.
/// If you are writing code in a UI element, use this.Dispatcher. [this. is optional; shown for clarity.] OR pass in to your method a reference to any UI element of that window (someUIElement.Dispatcher).
/// </summary>
/// <summary>
/// Dispatcher service.
/// </summary>
public class DispatcherService : IDispatcherService
{
    public object Dispatcher
    {
        get
        {
            if (Application.Current is Application baseApplication && Application.MainWindow?.DispatcherQueue is { } dispatcherQueue)
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
        if (Application.Current is Application baseApplication && Application.MainWindow?.DispatcherQueue is { } dispatcherQueue)
            return dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        if (DispatcherQueue.GetForCurrentThread() is { } dispatcher)
            return dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        return false;
    }
}

