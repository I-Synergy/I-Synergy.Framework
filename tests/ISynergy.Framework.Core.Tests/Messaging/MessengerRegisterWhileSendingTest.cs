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
        MessengerService.Reset();
        TestRecipient.Reset();

        List<TestRecipient1> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient1(true));
        }

        MessengerService.Default.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        MessengerService.Default.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringForMessageBaseWhileSending()
    {
        MessengerService.Reset();
        TestRecipient.Reset();

        List<TestRecipient2> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient2(true));
        }

        MessengerService.Default.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        MessengerService.Default.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceiving()
    {
        MessengerService.Default.Register<string>(
            this,
            m => MessengerService.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessengerService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceiving()
    {
        MessengerService.Default.Register<string>(
            this,
            m => MessengerService.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessengerService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceivingMessageBase()
    {
        MessengerService.Default.Register<string>(
            this,
            true,
            m => MessengerService.Default.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessengerService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceivingMessageBase()
    {
        MessengerService.Default.Register<string>(
            this,
            true,
            m => MessengerService.Default.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        MessengerService.Default.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerUnregisteringWhileReceiving()
    {
        MessengerService.Default.Register<string>(
            this,
            m => MessengerService.Default.Unregister(this));

        MessengerService.Default.Send("Hello world");
    }

    [TestMethod]
    public void TestMessengerUnregisteringFromMessageBaseWhileReceiving()
    {
        MessengerService.Default.Register<string>(
            this,
            true,
            m => MessengerService.Default.Unregister(this));

        MessengerService.Default.Send("Hello world");
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
                MessengerService.Default.Register<MessageFixture>(this, ReceiveString);
            }
        }

        protected virtual void ReceiveString(MessageFixture m)
        {
            MessengerService.Default.Register<MessageFixture>(this, ReceiveStringNested);
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
                MessengerService.Default.Register<BaseMessage>(this, true, ReceiveString);
            }
        }

        public virtual void ReceiveString(BaseMessage m)
        {
            var message = m as MessageFixture;
            if (message is not null)
            {
                MessengerService.Default.Register<BaseMessage>(this, true, ReceiveStringNested);
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
