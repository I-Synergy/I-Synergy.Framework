using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerRegisterWhileSendingTest
    {
        private const string TestContentString = "Hello world";
        private const string TestContentStringNested = "Hello earth";

        [TestMethod]
        public void TestMessengerRegisteringWhileSending()
        {
            Messenger.Reset();
            TestRecipient.Reset();

            var list = new List<TestRecipient1>();

            for (var index = 0; index < 10; index++)
            {
                list.Add(new TestRecipient1(true));
            }

            Messenger.Default.Send(new Message<string>(TestContentString));

            Assert.AreEqual(null, TestRecipient.LastReceivedString);
            Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

            Messenger.Default.Send(new Message<string>(TestContentStringNested));

            Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
            Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
        }

        [TestMethod]
        public void TestMessengerRegisteringForMessageBaseWhileSending()
        {
            Messenger.Reset();
            TestRecipient.Reset();

            var list = new List<TestRecipient2>();

            for (var index = 0; index < 10; index++)
            {
                list.Add(new TestRecipient2(true));
            }

            Messenger.Default.Send(new Message<string>(TestContentString));

            Assert.AreEqual(null, TestRecipient.LastReceivedString);
            Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

            Messenger.Default.Send(new Message<string>(TestContentStringNested));

            Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
            Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
        }

        [TestMethod]
        public void TestMessengerRegisteringInlineWhileReceiving()
        {
            Messenger.Default.Register<string>(
                this,
                m => Messenger.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
                {
                }));

            const string SentContent = "Hello world";
            Messenger.Default.Send(SentContent);
        }

        [TestMethod]
        public void TestMessengerRegisteringMessageBaseInlineWhileReceiving()
        {
            Messenger.Default.Register<string>(
                this,
                m => Messenger.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
                {
                }));

            const string SentContent = "Hello world";
            Messenger.Default.Send(SentContent);
        }

        [TestMethod]
        public void TestMessengerRegisteringInlineWhileReceivingMessageBase()
        {
            Messenger.Default.Register<string>(
                this,
                true,
                m => Messenger.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
                {
                }));

            const string SentContent = "Hello world";
            Messenger.Default.Send(SentContent);
        }

        [TestMethod]
        public void TestMessengerRegisteringMessageBaseInlineWhileReceivingMessageBase()
        {
            Messenger.Default.Register<string>(
                this,
                true,
                m => Messenger.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
                {
                }));

            const string SentContent = "Hello world";
            Messenger.Default.Send(SentContent);
        }

        [TestMethod]
        public void TestMessengerUnregisteringWhileReceiving()
        {
            Messenger.Default.Register<string>(
                this,
                m => Messenger.Default.Unregister(this));

            Messenger.Default.Send("Hello world");
        }

        [TestMethod]
        public void TestMessengerUnregisteringFromMessageBaseWhileReceiving()
        {
            Messenger.Default.Register<string>(
                this,
                true,
                m => Messenger.Default.Unregister(this));

            Messenger.Default.Send("Hello world");
        }

        public abstract class TestRecipient
        {
            public static string LastReceivedString
            {
                get;
                protected set;
            }

            public static int ReceivedStringMessages
            {
                get;
                protected set;
            }

            public static void Reset()
            {
                LastReceivedString = null;
                ReceivedStringMessages = 0;
            }
        }

        public class TestRecipient1 : TestRecipient
        {
            public TestRecipient1(bool register)
            {
                if (register)
                {
                    Messenger.Default.Register<Message<string>>(this, ReceiveString);
                }
            }

            protected virtual void ReceiveString(Message<string> m)
            {
                Messenger.Default.Register<Message<string>>(this, ReceiveStringNested);
            }

            protected void ReceiveStringNested(Message<string> m)
            {
                ReceivedStringMessages++;
                LastReceivedString = m.Content;
            }
        }

        public class TestRecipient2 : TestRecipient
        {
            public TestRecipient2(bool register)
            {
                if (register)
                {
                    Messenger.Default.Register<Message>(this, true, ReceiveString);
                }
            }

            public virtual void ReceiveString(Message m)
            {
                var message = m as Message<string>;
                if (message != null)
                {
                    Messenger.Default.Register<Message>(this, true, ReceiveStringNested);
                }
            }

            public void ReceiveStringNested(Message m)
            {
                var message = m as Message<string>;
                if (message != null)
                {
                    ReceivedStringMessages++;
                    LastReceivedString = message.Content;
                }
            }
        }
    }
}
