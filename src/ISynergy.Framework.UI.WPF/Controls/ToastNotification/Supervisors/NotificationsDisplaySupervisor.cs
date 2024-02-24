using ISynergy.Framework.Core.Events;
using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Events;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using ISynergy.Framework.UI.Controls.ToastNotification.Utilities;
using ISynergy.Framework.UI.Controls.Toasts;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Supervisors;

public class NotificationsDisplaySupervisor : IDisposable
{
    private readonly object _syncRoot = new object();
    private readonly Dispatcher _dispatcher;
    private readonly IPositionProvider _positionProvider;
    private readonly DisplayOptions _displayOptions;
    private readonly IKeyboardEventHandler _keyboardEventHandler;

    private INotificationsLifetimeSupervisor _lifetimeSupervisor;
    private NotificationsWindow _window;

    public NotificationsDisplaySupervisor(Dispatcher dispatcher,
        IPositionProvider positionProvider,
        INotificationsLifetimeSupervisor lifetimeSupervisor,
        DisplayOptions displayOptions,
        IKeyboardEventHandler keyboardEventHandler)
    {
        _dispatcher = dispatcher;
        _positionProvider = positionProvider;
        _lifetimeSupervisor = lifetimeSupervisor;
        _displayOptions = displayOptions;
        _keyboardEventHandler = keyboardEventHandler;

        _lifetimeSupervisor.ShowNotificationRequested += new WeakEventHandler<ShowNotificationEventArgs>(LifetimeSupervisorOnShowNotificationRequested).Handler;
        _lifetimeSupervisor.CloseNotificationRequested += new WeakEventHandler<CloseNotificationEventArgs>(LifetimeSupervisorOnCloseNotificationRequested).Handler;

        _positionProvider.UpdatePositionRequested += new WeakEventHandler<EventArgs>(PositionProviderOnUpdatePositionRequested).Handler;
        _positionProvider.UpdateEjectDirectionRequested += new WeakEventHandler<EventArgs>(PositionProviderOnUpdateEjectDirectionRequested).Handler;
        _positionProvider.UpdateHeightRequested += new WeakEventHandler<EventArgs>(PositionProviderOnUpdateHeightRequested).Handler;
    }

    public void DisplayNotification(INotification notification)
    {
        Dispatch(() => InternalDisplayNotification(notification));
    }

    private void InternalDisplayNotification(INotification notification)
    {
        InitializeWindow();

        ShowNotification(notification);
        ShowWindow();
        UpdateEjectDirection();
        UpdateWindowPosition();
    }

    private void Close(INotification notification)
    {
        Dispatch(() => InternalClose(notification));
    }

    private void InternalClose(INotification notification)
    {
        _lifetimeSupervisor.CloseNotification(notification);
        UpdateWindowPosition();
    }

    private void Dispatch(Action action)
    {
        _dispatcher.Invoke(action);
    }

    private void InitializeWindow()
    {
        lock (_syncRoot)
        {
            if (_window != null)
                return;

            _window = new NotificationsWindow(_positionProvider.ParentWindow);
            _window.SetDisplayOptions(_displayOptions);
            _window.MinHeight = _positionProvider.GetHeight();
            _window.Height = _positionProvider.GetHeight();
            _window.SetPosition(new Point(double.NaN, double.NaN));
            _window.SetKeyboardEventHandler(_keyboardEventHandler);
        }
    }

    private void UpdateWindowPosition()
    {
        if (_window == null || !_window.IsLoaded)
            return;
        Point position = _positionProvider.GetPosition(_window.GetWidth(), _window.GetHeight());
        _window.SetPosition(position);
    }

    private void UpdateEjectDirection()
    {
        if (_window == null || !_window.IsLoaded)
            return;
        _window.SetEjectDirection(_positionProvider.EjectDirection);
    }

    private void UpdateHeight()
    {
        if (_window == null || !_window.IsLoaded)
            return;
        var height = _positionProvider.GetHeight();
        _window.MinHeight = height;
        _window.Height = height;
    }

    private void ShowNotification(INotification notification)
    {
        notification.Bind(Close);
        _window?.ShowNotification(notification.DisplayPart);
    }

    private void CloseNotification(INotification notification)
    {
        if (notification != null)
        {
            notification.DisplayPart.OnClose();
            DelayAction.Execute(TimeSpan.FromMilliseconds(300),
                () => _window?.CloseNotification(notification.DisplayPart),
                _dispatcher);
        }
    }

    private void ShowWindow()
    {
        _window.Show();
    }

    private void LifetimeSupervisorOnShowNotificationRequested(object sender, ShowNotificationEventArgs eventArgs)
    {
        DisplayNotification(eventArgs.Notification);
    }

    private void LifetimeSupervisorOnCloseNotificationRequested(object sender, CloseNotificationEventArgs eventArgs)
    {
        CloseNotification(eventArgs.Notification);
    }

    private void PositionProviderOnUpdatePositionRequested(object sender, EventArgs eventArgs)
    {
        UpdateWindowPosition();
    }

    private void PositionProviderOnUpdateEjectDirectionRequested(object sender, EventArgs eventArgs)
    {
        UpdateEjectDirection();
    }

    private void PositionProviderOnUpdateHeightRequested(object sender, EventArgs eventArgs)
    {
        UpdateHeight();
    }

    public void Dispose()
    {
        _window?.Close();
        _window = null;
        _lifetimeSupervisor = null;
    }
}
