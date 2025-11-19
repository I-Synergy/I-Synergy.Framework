using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerSendToTypeTest
{
    private readonly IMessengerService _messenger;
    private readonly ILogger<MessengerService> _logger;

    public MessengerSendToTypeTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestBroadcastToOneType()
    {
        TestRecipient1 recipient11 = new();
        TestRecipient1 recipient12 = new();
        TestRecipient2 recipient21 = new();
        TestRecipient2 recipient22 = new();

        _messenger.Register<string>(recipient11, recipient11.ReceiveMessage);
        _messenger.Register<string>(recipient12, recipient12.ReceiveMessage);
        _messenger.Register<string>(recipient21, recipient21.ReceiveMessage);
        _messenger.Register<string>(recipient22, recipient22.ReceiveMessage);

        const string testContent1 = "abcd";
        const string testContent2 = "efgh";

        Assert.AreEqual(null, recipient11.ReceivedContentString);
        Assert.AreEqual(null, recipient12.ReceivedContentString);
        Assert.AreEqual(null, recipient21.ReceivedContentString);
        Assert.AreEqual(null, recipient22.ReceivedContentString);

        _messenger.Send<string, TestRecipient1>(testContent1);

        Assert.AreEqual(testContent1, recipient11.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient12.ReceivedContentString);
        Assert.AreEqual(null, recipient21.ReceivedContentString);
        Assert.AreEqual(null, recipient22.ReceivedContentString);

        _messenger.Send<string, TestRecipient2>(testContent2);

        Assert.AreEqual(testContent1, recipient11.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient12.ReceivedContentString);
        Assert.AreEqual(testContent2, recipient21.ReceivedContentString);
        Assert.AreEqual(testContent2, recipient22.ReceivedContentString);
    }

    [TestMethod]
    public void TestBroadcastToOneInterface()
    {
        TestRecipient1 recipient11 = new();
        TestRecipient1 recipient12 = new();
        TestRecipient2 recipient21 = new();
        TestRecipient2 recipient22 = new();
        TestRecipient3 recipient31 = new();
        TestRecipient3 recipient32 = new();

        _messenger.Register<string>(recipient11, recipient11.ReceiveMessage);
        _messenger.Register<string>(recipient12, recipient12.ReceiveMessage);
        _messenger.Register<string>(recipient21, recipient21.DoSomething);
        _messenger.Register<string>(recipient22, recipient22.DoSomething);
        _messenger.Register<string>(recipient31, recipient31.DoSomething);
        _messenger.Register<string>(recipient32, recipient32.DoSomething);

        const string testContent1 = "abcd";

        Assert.AreEqual(null, recipient11.ReceivedContentString);
        Assert.AreEqual(null, recipient12.ReceivedContentString);
        Assert.AreEqual(null, recipient21.ReceivedContentString);
        Assert.AreEqual(null, recipient22.ReceivedContentString);
        Assert.AreEqual(null, recipient31.ReceivedContentString);
        Assert.AreEqual(null, recipient32.ReceivedContentString);

        _messenger.Send<string, ITestRecipient>(testContent1);

        Assert.AreEqual(null, recipient11.ReceivedContentString);
        Assert.AreEqual(null, recipient12.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient21.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient22.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient31.ReceivedContentString);
        Assert.AreEqual(testContent1, recipient32.ReceivedContentString);
    }

    //// Helpers
    private class TestRecipient1
    {
        public string? ReceivedContentString
        {
            get;
            private set;
        }

        public void ReceiveMessage(string message)
        {
            ReceivedContentString = message;
        }
    }

    private class TestRecipient2 : ITestRecipient
    {
        public string? ReceivedContentString
        {
            get;
            private set;
        }

        public void ReceiveMessage(string message)
        {
            ReceivedContentString = message;
        }

        public void DoSomething(string message)
        {
            ReceivedContentString = message;
        }
    }

    public class TestRecipient3 : ITestRecipient
    {
        public string? ReceivedContentString
        {
            get;
            private set;
        }

        public void DoSomething(string message)
        {
            ReceivedContentString = message;
        }
    }

    private interface ITestRecipient
    {
        void DoSomething(string message);
    }
}
