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
        var messenger = new MessengerService();
        TestRecipient.Reset();

        List<TestRecipient1> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient1(messenger, true));
        }

        messenger.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        messenger.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringForMessageBaseWhileSending()
    {
        var messenger = new MessengerService();
        TestRecipient.Reset();

        List<TestRecipient2> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient2(messenger, true));
        }

        messenger.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        messenger.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceiving()
    {
        messenger.Register<string>(
            this,
            m => messenger.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceiving()
    {
        messenger.Register<string>(
            this,
            m => messenger.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceivingMessageBase()
    {
        messenger.Register<string>(
            this,
            true,
            m => messenger.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceivingMessageBase()
    {
        messenger.Register<string>(
            this,
            true,
            m => messenger.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerUnregisteringWhileReceiving()
    {
        messenger.Register<string>(
            this,
            m => messenger.Unregister(this));

        messenger.Send("Hello world");
    }

    [TestMethod]
    public void TestMessengerUnregisteringFromMessageBaseWhileReceiving()
    {
        messenger.Register<string>(
            this,
            true,
            m => messenger.Unregister(this));

        messenger.Send("Hello world");
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
        private readonly IMessengerService _messenger;

        public TestRecipient1(IMessengerService messenger, bool register)
        {
            _messenger = messenger;
            if (register)
            {
                _messenger.Register<MessageFixture>(this, ReceiveString);
            }
        }

        protected virtual void ReceiveString(MessageFixture m)
        {
            _messenger.Register<MessageFixture>(this, ReceiveStringNested);
        }

        protected void ReceiveStringNested(MessageFixture m)
        {
            ReceivedStringMessages++;
            LastReceivedString = m.Content;
        }
    }

    public class TestRecipient2 : TestRecipient
    {
        private readonly IMessengerService _messenger;

        public TestRecipient2(IMessengerService messenger, bool register)
        {
            _messenger = messenger;
            if (register)
            {
                _messenger.Register<BaseMessage>(this, true, ReceiveString);
            }
        }

        public virtual void ReceiveString(BaseMessage m)
        {
            var message = m as MessageFixture;
            if (message is not null)
            {
                _messenger.Register<BaseMessage>(this, true, ReceiveStringNested);
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
