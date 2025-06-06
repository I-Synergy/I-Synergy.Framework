﻿using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime.Clear;

public class ClearByTag : IClearStrategy
{
    private readonly object _tag;

    public ClearByTag(object tag)
    {
        _tag = tag;
    }

    public IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications)
    {
        var notificationsToRemove = notifications
            .Select(x => x.Value.Notification)
            .Where(x =>
            {
                var otherTag = x.Options.Tag;
                if (ReferenceEquals(otherTag, null))
                {
                    return ReferenceEquals(_tag, null);
                }

                return otherTag.Equals(_tag);
            })
            .ToList();

        return notificationsToRemove;
    }
}
