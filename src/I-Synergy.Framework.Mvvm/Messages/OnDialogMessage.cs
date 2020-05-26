using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class OnDialogMessage : EventMessage
    {
        public string Message { get; }
        public NotificationTypes Type { get; }

        public OnDialogMessage(object sender, string message, NotificationTypes type)
            : base(sender)
        {
            Message = message;
            Type = type;
            Handled = false;
        }
    }
}
