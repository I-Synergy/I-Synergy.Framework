using ISynergy.Framework.Core.Messaging;

namespace ISynergy.Framework.Synchronization.Messages;

public class SyncMessage : Message<string>
{
    public SyncMessage(string message)
        : base(message)
    {
    }
}
