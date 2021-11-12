namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Invokes action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task InvokeAsync(Action action);
    }
}
