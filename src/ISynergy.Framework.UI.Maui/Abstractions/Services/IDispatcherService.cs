namespace ISynergy.Framework.UI.Abstractions.Services;

public interface IDispatcherService
{
    bool Invoke(Action action);
}
