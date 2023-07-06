using ISynergy.Framework.UI.Abstractions.Controls.Toasts;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Events
{
    public class CloseNotificationEventArgs : EventArgs
    {
        public INotification Notification { get; }

        public CloseNotificationEventArgs(INotification notification)
        {
            Notification = notification;
        }
    }
}
