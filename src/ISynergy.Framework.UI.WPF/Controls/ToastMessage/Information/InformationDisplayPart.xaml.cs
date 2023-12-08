using ISynergy.Framework.UI.Controls.ToastNotification;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Information;

/// <summary>
/// Interaction logic for InformationDisplayPart.xaml
/// </summary>
public partial class InformationDisplayPart : NotificationDisplayPart
{
    public InformationDisplayPart(InformationMessage information, MessageOptions options)
    {
        InitializeComponent();
        Bind(information);
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Notification.Close();
    }
}
