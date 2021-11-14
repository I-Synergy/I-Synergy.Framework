namespace ISynergy.Framework.Core.Tests.Messaging
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
