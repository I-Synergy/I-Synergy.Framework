﻿using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Events;
using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;
using ISynergy.Framework.UI.Controls.ToastNotification.Utilities;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Supervisors;

public class TimeAndCountBasedLifetimeSupervisor : INotificationsLifetimeSupervisor
{
    private readonly TimeSpan _notificationLifetime;
    private readonly int _maximumNotificationCount;

    private Queue<INotification>? _notificationsPending;

    private NotificationsList _notifications = new NotificationsList();
    private IInterval _interval = new Interval();

    public TimeAndCountBasedLifetimeSupervisor(TimeSpan notificationLifetime, MaximumNotificationCount maximumNotificationCount)
    {
        _notificationLifetime = notificationLifetime;
        _maximumNotificationCount = maximumNotificationCount.Count;
    }

    public void PushNotification(INotification notification)
    {
        if (_disposed)
        {
            Debug.WriteLine($"Warn ToastNotifications {this}.{nameof(PushNotification)} is already disposed");
            return;
        }

        if (_interval.IsRunning == false)
            TimerStart();

        if (_notifications.Count == _maximumNotificationCount)
        {
            if (_notificationsPending == null)
            {
                _notificationsPending = new Queue<INotification>();
            }
            _notificationsPending.Enqueue(notification);
            return;
        }

        int numberOfNotificationsToClose = Math.Max(_notifications.Count - _maximumNotificationCount + 1, 0);

        var notificationsToRemove = _notifications
            .OrderBy(x => x.Key)
            .Take(numberOfNotificationsToClose)
            .Select(x => x.Value)
            .ToList();

        foreach (var n in notificationsToRemove.EnsureNotNull())
            CloseNotification(n.Notification);

        _notifications.Add(notification);
        RequestShowNotification(new ShowNotificationEventArgs(notification));
    }

    public void CloseNotification(INotification notification)
    {
        _notifications.TryRemove(notification.Id, out var removedNotification);
        RequestCloseNotification(new CloseNotificationEventArgs(removedNotification.Notification));

        if (_notificationsPending is not null && _notificationsPending.Any())
        {
            var not = _notificationsPending.Dequeue();
            PushNotification(not);
        }
    }


    private bool _disposed = false;
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _interval?.Stop();
        _notifications?.Clear();
        _notificationsPending?.Clear();
    }

    protected virtual void RequestShowNotification(ShowNotificationEventArgs e)
    {
        ShowNotificationRequested?.Invoke(this, e);
    }

    protected virtual void RequestCloseNotification(CloseNotificationEventArgs e)
    {
        CloseNotificationRequested?.Invoke(this, e);
    }

    private void TimerStart()
    {
        _interval.Invoke(
            TimeSpan.FromMilliseconds(200),
            OnTimerTick,
            Application.Current.Dispatcher);
    }

    private void TimerStop()
    {
        _interval.Stop();
    }

    private void OnTimerTick()
    {
        TimeSpan now = DateTime.Now.TimeOfDay;

        var notificationsToRemove = _notifications
            .Where(x => x.Value.Notification.CanClose && x.Value.CreateTime + _notificationLifetime <= now)
            .Select(x => x.Value)
            .ToList();

        foreach (var n in notificationsToRemove.EnsureNotNull())
            CloseNotification(n.Notification);

        if (_notifications.IsEmpty)
            TimerStop();
    }

    public void ClearMessages(IClearStrategy clearStrategy)
    {
        var notifications = clearStrategy.GetNotificationsToRemove(_notifications);
        foreach (var notification in notifications.EnsureNotNull())
        {
            CloseNotification(notification);
        }
    }

    public event EventHandler<ShowNotificationEventArgs>? ShowNotificationRequested;
    public event EventHandler<CloseNotificationEventArgs>? CloseNotificationRequested;
}
