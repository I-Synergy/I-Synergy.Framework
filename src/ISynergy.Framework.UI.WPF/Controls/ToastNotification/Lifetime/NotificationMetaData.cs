using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;

public struct NotificationMetaData
{
    public INotification Notification { get; }
    public int Id { get; }
    public TimeSpan CreateTime { get; }

    public NotificationMetaData(INotification notification, int id, TimeSpan createTime)
    {
        Notification = notification;
        Notification.Id = id;
        Id = id;
        CreateTime = createTime;
    }
}
