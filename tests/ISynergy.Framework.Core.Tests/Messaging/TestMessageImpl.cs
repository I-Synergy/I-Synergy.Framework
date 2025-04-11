namespace ISynergy.Framework.Core.Messaging.Tests;

public class TestMessageImpl(object sender) : TestMessageBase(sender)
{
    public bool Result { get; set; }
}
