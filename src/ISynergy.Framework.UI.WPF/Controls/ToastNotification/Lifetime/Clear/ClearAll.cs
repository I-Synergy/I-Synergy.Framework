using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime.Clear
{
    public class ClearAll : IClearStrategy
    {
        public IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications)
        {
            return notifications.Select(x => x.Value.Notification);
        }
    }
}
