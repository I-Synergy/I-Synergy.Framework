using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

internal class DispatcherService : IDispatcherService
{
    private readonly IDispatcher _dispatcher;

    public DispatcherService()
    {
        Argument.IsNotNull(Application.Current);
        Argument.IsNotNull(Application.Current.MainPage);

        _dispatcher = Application.Current.MainPage.Dispatcher;
    }

    public object Dispatcher { get => _dispatcher; }

    public bool Invoke(Action action) =>
        _dispatcher.Dispatch(action);
}
