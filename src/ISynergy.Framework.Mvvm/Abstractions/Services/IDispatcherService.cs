namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Dispatcher service.
/// </summary>
public interface IDispatcherService
{
    object Dispatcher { get; }

    bool Invoke(Action action);
}
