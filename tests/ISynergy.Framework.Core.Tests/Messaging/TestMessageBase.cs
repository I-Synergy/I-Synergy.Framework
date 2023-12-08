namespace ISynergy.Framework.Core.Messaging.Tests;

public class TestMessageBase : Message, ITestMessage
{
    public TestMessageBase(object sender)
        : base(sender)
    {
    }

    public string Content
    {
        get;
        set;
    }
}
