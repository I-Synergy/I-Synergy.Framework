using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Messages;

public class MessageBoxRequest
{
    public string Message { get; private set; }
    public string? Title { get; private set; }
    public NotificationTypes NotificationTypes { get; private set; }
    public MessageBoxButtons MessageBoxButtons { get; private set; }
    public MessageBoxImage MessageBoxImage { get; private set; }

    public MessageBoxRequest(string message, string? title = "")
    {
        Message = message;
        Title = title;
        NotificationTypes = NotificationTypes.Information;
        MessageBoxButtons = MessageBoxButtons.OK;
        MessageBoxImage = MessageBoxImage.None;
    }

    public MessageBoxRequest(string message, string? title, NotificationTypes notificationTypes)
        : this(message, title)
    {
        NotificationTypes = notificationTypes;
    }

    public MessageBoxRequest(string message, string? title, NotificationTypes notificationTypes, MessageBoxButtons messageBoxButtons)
        : this(message, title, notificationTypes)
    {
        MessageBoxButtons = messageBoxButtons;
    }

    public MessageBoxRequest(string message, string? title, NotificationTypes notificationTypes, MessageBoxButtons messageBoxButtons, MessageBoxImage messageBoxImage)
        : this(message, title, notificationTypes, messageBoxButtons)
    {
        MessageBoxImage = messageBoxImage;
    }
}