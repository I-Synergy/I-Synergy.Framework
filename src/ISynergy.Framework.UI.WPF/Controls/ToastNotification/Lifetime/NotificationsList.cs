using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using System.Collections.Concurrent;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;

public class NotificationsList : ConcurrentDictionary<int, NotificationMetaData>
{
    private int _id = 0;

    public NotificationMetaData Add(INotification notification)
    {
        var id = Interlocked.Increment(ref _id);
        var metaData = new NotificationMetaData(notification, id, DateTime.Now.TimeOfDay);
        this[id] = metaData;
        return metaData;
    }
}
