﻿using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime.Clear;

public class ClearLast : IClearStrategy
{
    public IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications)
    {
        if (notifications.IsEmpty)
        {
            return Enumerable.Empty<INotification>();
        }

        var lastMessage = notifications.LastOrDefault().Value.Notification;

        return new[] { lastMessage };
    }
}
