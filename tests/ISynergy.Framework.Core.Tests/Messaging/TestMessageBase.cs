using ISynergy.Framework.Core.Messaging;

namespace ISynergy.Framework.Core.Tests.Messaging
{
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
}
