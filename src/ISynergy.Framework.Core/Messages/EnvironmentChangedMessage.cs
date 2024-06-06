using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

public sealed class EnvironmentChangedMessage : BaseMessage<SoftwareEnvironments>
{
    public EnvironmentChangedMessage(SoftwareEnvironments content) : base(content) { }
}
