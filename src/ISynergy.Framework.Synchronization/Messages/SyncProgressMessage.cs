using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Synchronization.Messages;
public sealed class SyncProgressMessage : BaseMessage<double>
{
    public SyncProgressMessage(double progress)
        : base(progress)
    {
    }
}
