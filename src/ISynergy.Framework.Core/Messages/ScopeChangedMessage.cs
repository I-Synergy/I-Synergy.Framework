using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;
public sealed class ScopeChangedMessage : BaseMessage<bool>
{
    public ScopeChangedMessage(bool isNewScope) : base(isNewScope) { }
}
