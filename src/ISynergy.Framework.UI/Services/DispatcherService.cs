using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

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
