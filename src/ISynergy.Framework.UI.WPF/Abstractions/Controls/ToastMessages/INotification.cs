using ISynergy.Framework.UI.Controls.ToastNotification;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;

namespace ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;

public interface INotification
{
    int Id { get; set; }

    NotificationDisplayPart DisplayPart { get; }

    void Bind(Action<INotification> closeAction);

    void Close();

    string Message { get; }
    bool CanClose { get; set; }

    MessageOptions Options { get; }
}
