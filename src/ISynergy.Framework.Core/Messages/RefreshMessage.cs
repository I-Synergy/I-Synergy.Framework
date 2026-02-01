using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

public sealed class RefreshMessage : BaseMessage
{
    public RefreshMessage()
        : base()
    {
    }

    public RefreshMessage(object sender)
        : base(sender)
    {
    }

    public RefreshMessage(object sender, object target)
        : base(sender, target)
    {
    }
}
