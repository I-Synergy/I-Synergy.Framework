using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;

namespace ISynergy.Framework.UI.Controls.ToastNotification.Base;

public abstract class NotificationBase : INotification
{
    protected NotificationBase(string message, MessageOptions options)
    {
        Message = message;
        Options = options ?? new MessageOptions();
    }

    public string Message { get; }

    private Action<INotification>? _closeAction;

    public bool CanClose { get; set; } = true;

    public MessageOptions Options { get; }

    public abstract NotificationDisplayPart DisplayPart { get; }

    public int Id { get; set; }

    public virtual void Bind(Action<INotification> closeAction)
    {
        _closeAction = closeAction;
    }

    public virtual void Close()
    {
        Options?.CloseClickAction?.Invoke(this);
        _closeAction?.Invoke(this);
    }

}
