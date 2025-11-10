using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerMultipleInstancesTest
{
    private readonly IMessengerService _messenger1;
    private readonly IMessengerService _messenger2;
    private readonly ILogger<MessengerService> _logger;

    public MessengerMultipleInstancesTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger1 = new MessengerService(_logger);
        _messenger2 = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestMultipleMessengerInstances()
    {
        TestRecipient1 recipient11 = new();
        TestRecipient1 recipient12 = new();
        TestRecipient2 recipient21 = new();
        TestRecipient2 recipient22 = new();

        _messenger1.Register<string>(recipient11, recipient11.ReceiveMessage);
        _messenger2.Register<string>(recipient12, recipient12.ReceiveMessage);
        _messenger1.Register<string>(recipient21, recipient21.ReceiveMessage);
        _messenger2.Register<string>(recipient22, recipient22.ReceiveMessage);

        const string TestContent1 = "abcd";
        const string TestContent2 = "efgh";

        Assert.AreEqual(null, recipient11.ReceivedContentString);
        Assert.AreEqual(null, recipient12.ReceivedContentString);
        Assert.AreEqual(null, recipient21.ReceivedContentString);
        Assert.AreEqual(null, recipient22.ReceivedContentString);

        _messenger1.Send(TestContent1);

        Assert.AreEqual(TestContent1, recipient11.ReceivedContentString);
        Assert.AreEqual(null, recipient12.ReceivedContentString);
        Assert.AreEqual(TestContent1, recipient21.ReceivedContentString);
        Assert.AreEqual(null, recipient22.ReceivedContentString);

        _messenger2.Send(TestContent2);

        Assert.AreEqual(TestContent1, recipient11.ReceivedContentString);
        Assert.AreEqual(TestContent2, recipient12.ReceivedContentString);
        Assert.AreEqual(TestContent1, recipient21.ReceivedContentString);
        Assert.AreEqual(TestContent2, recipient22.ReceivedContentString);
    }

    //// Helpers

    private class TestRecipient1
    {
        public string? ReceivedContentString
        {
            get;
            set;
        }

        public void ReceiveMessage(string message)
        {
            ReceivedContentString = message;
        }
    }

    private class TestRecipient2
    {
        public string? ReceivedContentString
        {
            get;
            set;
        }

        public void ReceiveMessage(string message)
        {
            ReceivedContentString = message;
        }
    }
}
