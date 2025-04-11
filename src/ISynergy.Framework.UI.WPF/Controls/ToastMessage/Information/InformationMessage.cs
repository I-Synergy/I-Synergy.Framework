using ISynergy.Framework.UI.Controls.ToastMessage.Base;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Information;

public class InformationMessage : MessageBase<InformationDisplayPart>
{
    public InformationMessage(string message) : this(message, new MessageOptions())
    {
    }

    public InformationMessage(string message, MessageOptions options) : base(message, options)
    {
    }

    protected override InformationDisplayPart CreateDisplayPart()
    {
        return new InformationDisplayPart(this, Options);
    }

    protected override void UpdateDisplayOptions(InformationDisplayPart displayPart, MessageOptions options)
    {
        if (options.FontSize is not null)
            displayPart.Text.FontSize = options.FontSize.Value;

        displayPart.CloseButton.Visibility = options.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
    }
}
