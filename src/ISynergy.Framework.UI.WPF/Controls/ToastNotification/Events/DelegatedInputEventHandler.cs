using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Events;

public class DelegatedInputEventHandler : IKeyboardEventHandler
{
    private readonly Action<KeyEventArgs> _action;

    public DelegatedInputEventHandler(Action<KeyEventArgs> action)
    {
        _action = action;
    }

    public void Handle(KeyEventArgs eventArgs)
    {
        _action(eventArgs);
    }
}
