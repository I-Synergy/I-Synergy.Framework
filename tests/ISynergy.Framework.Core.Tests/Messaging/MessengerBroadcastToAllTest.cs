using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerBroadcastToAllTest
{
    public string? StringContent1
    {
        get;
        private set;
    }

    public string? StringContent2
    {
        get;
        private set;
    }

    private readonly IMessengerService _messenger;
    private readonly ILogger<MessengerService> _logger;

    public MessengerBroadcastToAllTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestSendingToAllRecipients()
    {
        const string TestContent = "abcd";

        Reset();

        _messenger.Register<TestMessage>(this, m => StringContent1 = m.Content);
        _messenger.Register<TestMessage>(this, m => StringContent2 = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(_messenger);

        Assert.AreEqual(null, StringContent1);
        Assert.AreEqual(null, StringContent2);
        Assert.AreEqual(null, externalRecipient.StringContent);

        _messenger.Send(new TestMessage
        {
            Content = TestContent
        });

        Assert.AreEqual(TestContent, StringContent1);
        Assert.AreEqual(TestContent, StringContent2);
        Assert.AreEqual(TestContent, externalRecipient.StringContent);
    }

    //// Helpers

    private void Reset()
    {
        StringContent1 = null;
        StringContent2 = null;
    }

    public class TestMessage
    {
        public string? Content
        {
            get;
            set;
        }
    }

    private class TestRecipient
    {
        public string? StringContent
        {
            get;
            private set;
        }

        internal void RegisterWith(IMessengerService _messenger)
        {
            _messenger.Register<TestMessage>(this, m => StringContent = m.Content);
        }
    }
}
