using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.Mvvm.Messages;

public class MessageBoxRequest
{
    public string Message { get; private set; }
    public string? Title { get; private set; }
    public NotificationTypes NotificationTypes { get; private set; }
    public MessageBoxButtons MessageBoxButtons { get; private set; }
    public MessageBoxImages MessageBoxImages { get; private set; }

    public MessageBoxRequest(string message, string? title = "")
    {
        Message = message;
        Title = title;
        NotificationTypes = NotificationTypes.Information;
        MessageBoxButtons = MessageBoxButtons.OK;
        MessageBoxImages = MessageBoxImages.None;
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

    public MessageBoxRequest(string message, string? title, NotificationTypes notificationTypes, MessageBoxButtons messageBoxButtons, MessageBoxImages messageBoxImage)
        : this(message, title, notificationTypes, messageBoxButtons)
    {
        MessageBoxImages = messageBoxImage;
    }
}