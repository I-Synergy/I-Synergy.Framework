using ISynergy.Framework.UI.Controls.ToastNotification.Events;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

public interface INotificationsLifetimeSupervisor : IDisposable
{
    void PushNotification(INotification notification);
    void CloseNotification(INotification notification);

    event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
    event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;

    void ClearMessages(IClearStrategy clearStrategy);
}
