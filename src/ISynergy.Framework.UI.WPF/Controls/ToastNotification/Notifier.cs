using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Events;
using ISynergy.Framework.UI.Controls.ToastNotification.Supervisors;

namespace ISynergy.Framework.UI.Controls.ToastNotification;

public class Notifier : IDisposable
{
    private readonly object _syncRoot = new object();

    private readonly Action<NotifierConfiguration> _configureAction;
    private NotifierConfiguration? _configuration;
    private NotificationsDisplaySupervisor? _displaySupervisor;

    public Notifier(Action<NotifierConfiguration> configureAction)
    {
        _configureAction = configureAction;
    }

    public void Notify<T>(Func<T> createNotificationFunc)
        where T : INotification
    {
        if (_configuration is not null)
            return;

        _configuration = CreateConfiguration();

        var keyboardEventHandler = _configuration.KeyboardEventHandler ?? new BlockAllKeyInputEventHandler();

        if (_configuration is not null && _configuration.PositionProvider is not null && _configuration.LifetimeSupervisor is not null)
        {
            _displaySupervisor = new NotificationsDisplaySupervisor(
                _configuration.PositionProvider,
                _configuration.LifetimeSupervisor,
                _configuration.DisplayOptions,
                keyboardEventHandler);

            _configuration.LifetimeSupervisor.PushNotification(createNotificationFunc());
        }
    }

    private NotifierConfiguration CreateConfiguration()
    {
        var configuration = new NotifierConfiguration();

        _configureAction(configuration);

        if (configuration.LifetimeSupervisor == null)
            throw new ArgumentNullException(nameof(configuration.LifetimeSupervisor), "Missing configuration argument");

        if (configuration.PositionProvider == null)
            throw new ArgumentNullException(nameof(configuration.PositionProvider), "Missing configuration argument");

        return configuration;
    }

    public void ClearMessages(IClearStrategy clearStrategy)
    {
        if (_configuration is not null && _configuration.LifetimeSupervisor is not null)
            _configuration?.LifetimeSupervisor?.ClearMessages(clearStrategy);
    }

    private bool _disposed = false;

    public object SyncRoot => _syncRoot;

    public void Dispose()
    {
        if (_disposed == false)
        {
            _disposed = true;
            _configuration?.PositionProvider?.Dispose();
            _displaySupervisor?.Dispose();
        }
    }
}
