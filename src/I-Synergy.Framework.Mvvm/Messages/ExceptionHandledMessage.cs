using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class ExceptionHandledMessage : EventMessage
    {
        public ExceptionHandledMessage(object sender)
            : base(sender)
        {
        }
    }
}
