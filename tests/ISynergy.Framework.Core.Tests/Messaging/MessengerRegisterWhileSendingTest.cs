using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerRegisterWhileSendingTest
{
    private const string TestContentString = "Hello world";
    private const string TestContentStringNested = "Hello earth";

    [TestMethod]
    public void TestMessengerRegisteringWhileSending()
    {
        MessageService.Reset();
        TestRecipient.Reset();

        List<TestRecipient1> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient1(true));
        }

        MessageService.Default.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        MessageService.Default.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringForMessageBaseWhileSending()
    {
        MessageService.Reset();
        TestRecipient.Reset();

        List<TestRecipient2> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient2(true));
        }

        MessageService.Default.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        MessageService.Default.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceiving()
    {
        MessageService.Default.Register<string>(
            this,
            m => MessageService.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessageService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceiving()
    {
        MessageService.Default.Register<string>(
            this,
            m => MessageService.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessageService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceivingMessageBase()
    {
        MessageService.Default.Register<string>(
            this,
            true,
            m => MessageService.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessageService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceivingMessageBase()
    {
        MessageService.Default.Register<string>(
            this,
            true,
            m => MessageService.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessageService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerUnregisteringWhileReceiving()
    {
        MessageService.Default.Register<string>(
            this,
            m => MessageService.Default.Unregister(this));

        MessageService.Default.Send("Hello world");
    }

    [TestMethod]
    public void TestMessengerUnregisteringFromMessageBaseWhileReceiving()
    {
        MessageService.Default.Register<string>(
            this,
            true,
            m => MessageService.Default.Unregister(this));

        MessageService.Default.Send("Hello world");
    }

    public abstract class TestRecipient
    {
        public static string? LastReceivedString
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
                MessageService.Default.Register<MessageFixture>(this, ReceiveString);
            }
        }

        protected virtual void ReceiveString(MessageFixture m)
        {
            MessageService.Default.Register<MessageFixture>(this, ReceiveStringNested);
        }

        protected void ReceiveStringNested(MessageFixture m)
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
                MessageService.Default.Register<BaseMessage>(this, true, ReceiveString);
            }
        }

        public virtual void ReceiveString(BaseMessage m)
        {
            var message = m as MessageFixture;
            if (message is not null)
            {
                MessageService.Default.Register<BaseMessage>(this, true, ReceiveStringNested);
            }
        }

        public void ReceiveStringNested(BaseMessage m)
        {
            var message = m as MessageFixture;
            if (message is not null)
            {
                ReceivedStringMessages++;
                LastReceivedString = message.Content;
            }
        }
    }
}
