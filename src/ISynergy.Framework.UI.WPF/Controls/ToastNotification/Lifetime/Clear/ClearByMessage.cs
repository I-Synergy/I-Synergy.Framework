using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime.Clear;

public class ClearByMessage : IClearStrategy
{
    private readonly string _message;

    public ClearByMessage(string message)
    {
        _message = message;
    }

    public IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications)
    {
        var notificationsToRemove = notifications
            .Select(x => x.Value.Notification)
            .Where(x => x.Message == _message)
            .ToList();

        return notificationsToRemove;
    }
}
