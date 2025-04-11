using ISynergy.Framework.UI.Controls.ToastNotification;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Success;

/// <summary>
/// Interaction logic for SuccessDisplayPart.xaml
/// </summary>
public partial class SuccessDisplayPart : NotificationDisplayPart
{
    public SuccessDisplayPart(SuccessMessage success)
    {
        InitializeComponent();

        Bind(success);
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Notification?.Close();
    }
}
