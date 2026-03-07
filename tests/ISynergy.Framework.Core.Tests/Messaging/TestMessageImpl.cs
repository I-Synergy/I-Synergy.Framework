namespace ISynergy.Framework.Core.Messaging;

public class TestMessageImpl(object sender) : TestMessageBase(sender)
{
    public bool Result { get; set; }
}
