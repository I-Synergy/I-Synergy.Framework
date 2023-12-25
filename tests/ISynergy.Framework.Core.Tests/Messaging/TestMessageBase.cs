namespace ISynergy.Framework.Core.Messaging.Tests;

public class TestMessageBase(object sender) : Message(sender), ITestMessage
{
    public string Content
    {
        get;
        set;
    }
}
