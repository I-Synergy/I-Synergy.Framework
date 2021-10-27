using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Dispatcher asynchronous invoke task.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task InvokeAsync(Action action);
    }
}
