using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class OnlineModeChangedMessage : EventMessage
    {
        public OnlineModeChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}
