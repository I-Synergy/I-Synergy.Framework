using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Controls.ToastMessage.Error;
using ISynergy.Framework.UI.Controls.ToastMessage.Information;
using ISynergy.Framework.UI.Controls.ToastMessage.Success;
using ISynergy.Framework.UI.Controls.ToastMessage.Warning;
using ISynergy.Framework.UI.Controls.ToastNotification;
using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime;
using ISynergy.Framework.UI.Controls.ToastNotification.Lifetime.Clear;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using ISynergy.Framework.UI.Controls.ToastNotification.Position;
using ISynergy.Framework.UI.Controls.ToastNotification.Supervisors;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.UI.Services;

public class ToastMessageService : IToastMessageService
{
    private readonly Notifier _notifier;
    private readonly ToastMessageOptions _options;

    public ToastMessageService(IOptions<ToastMessageOptions> options)
    {
        _options = options.Value;

        _notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: _options.Corner,
                offsetX: _options.OffsetX,
                offsetY: _options.OffsetY);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(_options.NotificationLifetimeInSeconds),
                maximumNotificationCount: MaximumNotificationCount.FromCount(_options.MaximumNotificationCount));

            cfg.DisplayOptions.TopMost = _options.TopMost;
            cfg.DisplayOptions.Width = _options.Width;
        });

        _notifier.ClearMessages(new ClearAll());
    }

    public void ShowError(string message)
    {
        _notifier.Notify<ErrorMessage>(() => new ErrorMessage(message));
    }

    public void ShowError(string message, MessageOptions displayOptions)
    {
        _notifier.Notify<ErrorMessage>(() => new ErrorMessage(message, displayOptions));
    }

    public void ShowInformation(string message)
    {
        _notifier.Notify(() => new InformationMessage(message));
    }

    public void ShowInformation(string message, MessageOptions displayOptions)
    {
        _notifier.Notify(() => new InformationMessage(message, displayOptions));
    }

    public void ShowSuccess(string message)
    {
        _notifier.Notify<SuccessMessage>(() => new SuccessMessage(message));
    }

    public void ShowSuccess(string message, MessageOptions displayOptions)
    {
        _notifier.Notify<SuccessMessage>(() => new SuccessMessage(message, displayOptions));
    }

    public void ShowWarning(string message)
    {
        _notifier.Notify<WarningMessage>(() => new WarningMessage(message));
    }

    public void ShowWarning(string message, MessageOptions displayOptions)
    {
        _notifier.Notify<WarningMessage>(() => new WarningMessage(message, displayOptions));
    }
}
