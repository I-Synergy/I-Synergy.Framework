using ISynergy.Framework.UI.Abstractions.Controls.Toasts;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Events
{
    public class ShowNotificationEventArgs : EventArgs
    {
        public INotification Notification { get; }

        public ShowNotificationEventArgs(INotification notification)
        {
            Notification = notification;
        }
    }
}
