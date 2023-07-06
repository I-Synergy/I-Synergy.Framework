using ISynergy.Framework.UI.Controls.ToastNotification.Events;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.Abstractions.Controls.Toasts
{
    public interface INotificationsLifetimeSupervisor : IDisposable
    {
        void PushNotification(INotification notification);
        void CloseNotification(INotification notification);

        void UseDispatcher(Dispatcher dispatcher);

        event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
        event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;

        void ClearMessages(IClearStrategy clearStrategy);
    }
}
