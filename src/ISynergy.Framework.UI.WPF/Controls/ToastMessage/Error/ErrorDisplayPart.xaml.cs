using ISynergy.Framework.UI.Controls.ToastNotification;
using System.Windows;

namespace ISynergy.Framework.UI.Controls.ToastMessage.Error
{
    /// <summary>
    /// Interaction logic for ErrorDisplayPart.xaml
    /// </summary>
    public partial class ErrorDisplayPart : NotificationDisplayPart
    {
        public ErrorDisplayPart(ErrorMessage error)
        {
            InitializeComponent();
            Bind(error);
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Notification.Close();
        }
    }
}
