namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public interface IDispatcherService
    {
        bool Invoke(Action action);
    }
}
