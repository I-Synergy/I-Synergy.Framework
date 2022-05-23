namespace ISynergy.Framework.Core.Messaging.Tests
{
    public class TestMessageImpl : TestMessageBase
    {
        public TestMessageImpl(object sender)
            : base(sender)
        {
        }

        public bool Result
        {
            get;
            set;
        }
    }
}
