using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.CQRS.Messages;
public class CommandMessage<T> : BaseMessage<T>
{
    public CommandMessage(T content) : base(content)
    {
    }

    public CommandMessage(object sender, T content) : base(sender, content)
    {
    }

    public CommandMessage(object sender, object target, T content) : base(sender, target, content)
    {
    }
}
