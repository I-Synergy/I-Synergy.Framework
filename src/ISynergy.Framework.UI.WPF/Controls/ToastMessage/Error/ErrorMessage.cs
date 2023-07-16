using ISynergy.Framework.UI.Controls.ToastMessage.Base;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Error
{
    public class ErrorMessage : MessageBase<ErrorDisplayPart>
    {
        public ErrorMessage(string message) : this(message, new MessageOptions())
        {
        }

        public ErrorMessage(string message, MessageOptions options) : base(message, options)
        {
        }

        protected override ErrorDisplayPart CreateDisplayPart()
        {
            return new ErrorDisplayPart(this);
        }

        protected override void UpdateDisplayOptions(ErrorDisplayPart displayPart, MessageOptions options)
        {
            if (options.FontSize != null)
                displayPart.Text.FontSize = options.FontSize.Value;

            displayPart.CloseButton.Visibility = options.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
