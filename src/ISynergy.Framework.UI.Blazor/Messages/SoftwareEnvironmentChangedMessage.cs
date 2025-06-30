using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.UI.Messages;
public class SoftwareEnvironmentChangedMessage(SoftwareEnvironments environment) : BaseMessage<SoftwareEnvironments>(environment);
