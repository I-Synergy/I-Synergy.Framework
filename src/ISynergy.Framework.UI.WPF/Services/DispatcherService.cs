using ISynergy.Framework.Core.Abstractions.Services;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// WPF dispatcher service for dispatching actions to the UI thread.
/// </summary>
public class DispatcherService : IDispatcherService
{
    public bool Invoke(Action action)
    {
        if (System.Windows.Application.Current?.Dispatcher is { } dispatcher)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
            return true;
        }
        return false;
    }
}
