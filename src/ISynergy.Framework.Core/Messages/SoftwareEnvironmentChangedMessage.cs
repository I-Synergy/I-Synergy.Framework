using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Message published when software environment changes.
/// </summary>
public sealed class SoftwareEnvironmentChangedMessage : BaseMessage<SoftwareEnvironments>
{
 /// <summary>
 /// Creates a new instance with the environment content.
 /// </summary>
 public SoftwareEnvironmentChangedMessage(SoftwareEnvironments environment)
 : base(environment)
 {
 }

 /// <summary>
 /// Creates a new instance with sender and environment content.
 /// </summary>
 public SoftwareEnvironmentChangedMessage(object sender, SoftwareEnvironments environment)
 : base(sender, environment)
 {
 }

 /// <summary>
 /// Creates a new instance with sender, target and environment content.
 /// </summary>
 public SoftwareEnvironmentChangedMessage(object sender, object target, SoftwareEnvironments environment)
 : base(sender, target, environment)
 {
 }
}
