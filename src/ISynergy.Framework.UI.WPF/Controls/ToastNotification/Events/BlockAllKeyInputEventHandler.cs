using ISynergy.Framework.UI.Abstractions.Controls.Toasts;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Events
{
    public class BlockAllKeyInputEventHandler : IKeyboardEventHandler
    {
        public void Handle(KeyEventArgs eventArgs)
        {
            eventArgs.Handled = true;
        }
    }
}
