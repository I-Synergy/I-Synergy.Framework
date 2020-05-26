using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class AuthenticateUserMessageResult : EventMessage
    {
        public object Property { get; }
        public bool IsAuthenticated { get; }

        public AuthenticateUserMessageResult(object sender, object property, bool authenticated)
            : base(sender)
        {
            Property = property;
            IsAuthenticated = authenticated;
        }
    }
}
