namespace ISynergy.Framework.Core.Abstractions.Services;

public interface IDispatcherService
{
    bool Invoke(Action action);
}
