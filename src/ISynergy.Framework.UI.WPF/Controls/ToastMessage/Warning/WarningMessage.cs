using ISynergy.Framework.UI.Controls.ToastMessage.Base;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Warning;

public class WarningMessage : MessageBase<WarningDisplayPart>
{
    public WarningMessage(string message) : this(message, new MessageOptions())
    {
    }

    public WarningMessage(string message, MessageOptions options) : base(message, options)
    {
    }

    protected override WarningDisplayPart CreateDisplayPart()
    {
        return new WarningDisplayPart(this);
    }

    protected override void UpdateDisplayOptions(WarningDisplayPart displayPart, MessageOptions options)
    {
        if (options.FontSize is not null)
            displayPart.Text.FontSize = options.FontSize.Value;

        displayPart.CloseButton.Visibility = options.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
    }
}
