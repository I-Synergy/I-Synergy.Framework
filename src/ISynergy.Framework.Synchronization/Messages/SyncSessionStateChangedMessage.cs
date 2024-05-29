using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Synchronization.Messages;

public sealed class SyncSessionStateChangedMessage : BaseMessage<SyncSessionState>
{
    public SyncSessionStateChangedMessage(SyncSessionState sessionState)
        : base(sessionState)
    {
    }
}
