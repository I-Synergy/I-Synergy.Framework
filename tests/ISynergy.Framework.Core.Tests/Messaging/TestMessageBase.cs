using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messaging;

public class TestMessageBase(object sender) : BaseMessage(sender), ITestMessage
{
    public string Content { get; set; } = string.Empty;
}
