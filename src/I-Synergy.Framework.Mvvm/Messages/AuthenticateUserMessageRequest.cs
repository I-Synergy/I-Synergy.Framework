using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class AuthenticateUserMessageRequest : EventMessage
    {
        public object Property { get; }
        public bool EnableLogin { get; }
        public bool ShowLogin { get; }

        public AuthenticateUserMessageRequest(object sender, bool showLogin, object property = null, bool enableLogin = true)
            : base(sender)
        {
            Property = property;
            EnableLogin = enableLogin;
            ShowLogin = showLogin;
        }
    }
}
