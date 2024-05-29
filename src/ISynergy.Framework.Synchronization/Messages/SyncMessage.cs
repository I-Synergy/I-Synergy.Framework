using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Synchronization.Messages;

public sealed class SyncMessage : BaseMessage<string>
{
    public SyncMessage(string message)
        : base(message)
    {
    }
}
