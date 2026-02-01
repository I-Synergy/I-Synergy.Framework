namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface IDispatcherService
{
    bool Invoke(Action action);
}
