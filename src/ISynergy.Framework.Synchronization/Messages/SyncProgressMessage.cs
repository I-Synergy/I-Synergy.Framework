using ISynergy.Framework.Core.Messaging;

namespace ISynergy.Framework.Synchronization.Messages;
public class SyncProgressMessage : Message<double>
{
    public SyncProgressMessage(double progress)
        : base(progress)
    {
    }
}
