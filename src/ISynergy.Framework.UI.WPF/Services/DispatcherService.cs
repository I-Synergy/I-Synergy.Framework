using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Dispatcher service.
/// </summary>
public class DispatcherService : IDispatcherService
{
    private readonly Dispatcher _dispatcher;

    public DispatcherService()
    {
        Argument.IsNotNull(Application.Current);
        _dispatcher = Application.Current.Dispatcher;
    }

    public object Dispatcher { get => _dispatcher; }

    /// <summary>
    /// Invokes action with the dispatcher.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool Invoke(Action action)
    {
        _dispatcher.Invoke(action);
        return true;
    }
}
