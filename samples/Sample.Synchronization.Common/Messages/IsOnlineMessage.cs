using ISynergy.Framework.Core.Messaging;

namespace Sample.Synchronization.Common.Messages
{
    public class IsOnlineMessage : Message<bool>
    {
        public IsOnlineMessage(bool value)
            : base(value)
        {
        }
    }
}
