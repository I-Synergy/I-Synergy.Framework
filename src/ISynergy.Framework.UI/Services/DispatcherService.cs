using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Dispatching;
using Application = Microsoft.UI.Xaml.Application;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Invokes action with the dispatcher.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task InvokeAsync(Action action)
        {
            if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is DispatcherQueue dispatcherQueue)
            {
                return Task.Run(() =>
                {
                    dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
                });
            }

            return Task.CompletedTask;
        }
    }
}
