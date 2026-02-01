using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Tests.Fixtures;

public class MessageFixture : BaseMessage<string>
{
    public MessageFixture(string content) : base(content)
    {
    }
}
