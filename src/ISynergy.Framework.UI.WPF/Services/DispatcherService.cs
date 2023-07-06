using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Windows;

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
        public bool Invoke(Action action)
        {
            Application.Current?.Dispatcher.Invoke(action);
            return true;
        }
    }
}
