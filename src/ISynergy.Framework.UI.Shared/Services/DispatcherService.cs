using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

#if WINDOWS_UWP || HAS_UNO
using Windows.UI.Xaml;
#else
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
#endif

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
#if WINDOWS_UWP || HAS_UNO
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(
                CoreDispatcherPriority.Normal, action);
#else
            if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is DispatcherQueue dispatcherQueue)
            {
                return Task.Run(() =>
                {
                    dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
                });
            }

            return Task.CompletedTask;
#endif
        }
    }
}
