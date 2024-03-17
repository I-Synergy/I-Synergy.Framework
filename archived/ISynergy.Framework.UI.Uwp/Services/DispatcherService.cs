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
        /// <summary>
        /// Invokes action with the dispatcher.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task InvokeAsync(Action action) =>
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, action);
    }
}
