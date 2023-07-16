using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages
{
    public interface IClearStrategy
    {
        IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications);
    }
}
