using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class AuthenticationChangedMessage : EventMessage
    {
        public AuthenticationChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}
