using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerSendToAllTest
    {
        public string StringContent1
        {
            get;
            private set;
        }

        public string StringContent2
        {
            get;
            private set;
        }

        [TestMethod]
        public void TestSendingToAllRecipients()
        {
            const string TestContent = "abcd";

            Reset();
            MessageService.Reset();

            MessageService.Default.Register<TestMessage>(this, m => StringContent1 = m.Content);

            MessageService.Default.Register<TestMessage>(this, m => StringContent2 = m.Content);

            TestRecipient externalRecipient = new();
            externalRecipient.RegisterWith(MessageService.Default);

            Assert.AreEqual(null, StringContent1);
            Assert.AreEqual(null, StringContent2);
            Assert.AreEqual(null, externalRecipient.StringContent);

            MessageService.Default.Send(new TestMessage
            {
                Content = TestContent
            });

            Assert.AreEqual(TestContent, StringContent1);
            Assert.AreEqual(TestContent, StringContent2);
            Assert.AreEqual(TestContent, externalRecipient.StringContent);
        }

        //[TestMethod]
        public void TestSendingNullMessage()
        {
            MessageService.Reset();
            MessageService.Default.Send<TestMessageImpl>(null);
        }

        //// Helpers

        private void Reset()
        {
            StringContent1 = null;
            StringContent2 = null;
        }

        public class TestMessage
        {
            public string Content
            {
                get;
                set;
            }
        }

        private class TestRecipient
        {
            public string StringContent
            {
                get;
                private set;
            }

            internal void RegisterWith(IMessageService messenger)
            {
                messenger.Register<TestMessage>(this, m => StringContent = m.Content);
            }
        }
    }
}
