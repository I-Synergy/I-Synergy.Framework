using System.Windows.Input;

namespace ISynergy.Framework.UI.Abstractions.Controls.Toasts
{
    public interface IKeyboardEventHandler
    {
        void Handle(KeyEventArgs eventArgs);
    }
}
