using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Core.Messaging;

namespace ISynergy.Framework.Synchronization.Messages;

public class SyncSessionStateChangedMessage : Message<SyncSessionState>
{
    public SyncSessionStateChangedMessage(SyncSessionState sessionState)
        : base(sessionState)
    {
    }
}
