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
        private readonly DispatcherQueue _dispatcher;

        public DispatcherService()
        {
            Argument.IsNotNull(Application.Current);

            if (Application.Current is BaseApplication baseApplication && baseApplication.MainWindow.DispatcherQueue is DispatcherQueue dispatcherQueue)
                _dispatcher = dispatcherQueue;
        }

        public object Dispatcher { get => _dispatcher; }

        /// <summary>
        /// Invokes action with the dispatcher.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Invoke(Action action)
        {
            return _dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () => action());
        }
    }
}
