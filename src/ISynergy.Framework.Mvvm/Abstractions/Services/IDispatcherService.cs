using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public interface IDispatcherService
    {
        Task InvokeAsync(Action action);
    }
}
