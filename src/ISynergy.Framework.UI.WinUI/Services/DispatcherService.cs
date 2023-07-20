using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Dispatching;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        public object Dispatcher
        {
            get
            {
                if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is DispatcherQueue dispatcherQueue)
                    return dispatcherQueue;
                else
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
            if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is DispatcherQueue dispatcherQueue)
                return dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
            else if (DispatcherQueue.GetForCurrentThread() is DispatcherQueue dispatcher)
                return dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
            return false;
        }
    }
}
