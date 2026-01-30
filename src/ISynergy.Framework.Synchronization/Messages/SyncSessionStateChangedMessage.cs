using Dotmim.Sync;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Synchronization.Messages;

public sealed class SyncSessionStateChangedMessage : BaseMessage<SyncSessionStateEventArgs>
{
    public SyncSessionStateChangedMessage(SyncSessionStateEventArgs sessionStateArgs)
        : base(sessionStateArgs)
    {
    }
}
