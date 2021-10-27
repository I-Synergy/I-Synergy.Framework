using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        private readonly CoreDispatcherPriority _priority;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DispatcherService()
        {
            _priority = CoreDispatcherPriority.Normal;
        }

        /// <summary>
        /// Dispatcher asynchronous invoke task.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task InvokeAsync(Action action)
        {
            var dispatcher = CoreApplication.MainView.Dispatcher;

            if (dispatcher is null || dispatcher.HasThreadAccess)
                action();
            else
                await dispatcher.RunAndAwaitAsync(_priority, action);
        }
    }
}
