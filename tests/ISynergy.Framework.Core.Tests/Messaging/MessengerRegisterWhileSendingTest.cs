using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerRegisterWhileSendingTest
{
    private const string TestContentString = "Hello world";
    private const string TestContentStringNested = "Hello earth";

    private readonly IMessengerService _messenger;
    private readonly ILogger<MessengerService> _logger;

    public MessengerRegisterWhileSendingTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestMessengerRegisteringWhileSending()
    {
        TestRecipient.Reset();

        List<TestRecipient1> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient1(_messenger, true));
        }

        _messenger.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        _messenger.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringForMessageBaseWhileSending()
    {
        var _messenger = new MessengerService(_logger);
        TestRecipient.Reset();

        List<TestRecipient2> list = [];

        for (int index = 0; index < 10; index++)
        {
            list.Add(new TestRecipient2(_messenger, true));
        }

        _messenger.Send(new MessageFixture(TestContentString));

        Assert.AreEqual(null, TestRecipient.LastReceivedString);
        Assert.AreEqual(0, TestRecipient.ReceivedStringMessages);

        _messenger.Send(new MessageFixture(TestContentStringNested));

        Assert.AreEqual(TestContentStringNested, TestRecipient.LastReceivedString);
        Assert.AreEqual(10, TestRecipient.ReceivedStringMessages);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceiving()
    {
        _messenger.Register<string>(
            this,
            m => _messenger.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        _messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceiving()
    {
        _messenger.Register<string>(
            this,
            m => _messenger.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        _messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringInlineWhileReceivingMessageBase()
    {
        _messenger.Register<string>(
            this,
            true,
            m => _messenger.Register<PropertyChangedMessage<string>>(this, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        _messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerRegisteringMessageBaseInlineWhileReceivingMessageBase()
    {
        _messenger.Register<string>(
            this,
            true,
            m => _messenger.Register<PropertyChangedMessage<string>>(this, true, m2 =>
            {
            }));

        const string SentContent = "Hello world";
        _messenger.Send(SentContent);
    }

    [TestMethod]
    public void TestMessengerUnregisteringWhileReceiving()
    {
        _messenger.Register<string>(
            this,
            m => _messenger.Unregister(this));

        _messenger.Send("Hello world");
    }

    [TestMethod]
    public void TestMessengerUnregisteringFromMessageBaseWhileReceiving()
    {
        _messenger.Register<string>(
            this,
            true,
            m => _messenger.Unregister(this));

        _messenger.Send("Hello world");
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
