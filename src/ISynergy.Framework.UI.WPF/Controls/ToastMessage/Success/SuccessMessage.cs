using ISynergy.Framework.UI.Controls.ToastMessage.Base;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Success
{
    public class SuccessMessage : MessageBase<SuccessDisplayPart>
    {
        public SuccessMessage(string message) : this(message, new MessageOptions())
        {
        }

        public SuccessMessage(string message, MessageOptions options) : base(message, options)
        {
        }

        protected override SuccessDisplayPart CreateDisplayPart()
        {
            return new SuccessDisplayPart(this);
        }

        protected override void UpdateDisplayOptions(SuccessDisplayPart displayPart, MessageOptions options)
        {
            if (options.FontSize != null)
                displayPart.Text.FontSize = options.FontSize.Value;

            displayPart.CloseButton.Visibility = options.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
