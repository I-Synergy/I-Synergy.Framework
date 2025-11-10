using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerSendWithTokenTest
{
    private readonly IMessengerService _messenger;
    private readonly ILogger<MessengerService> _logger;

    public MessengerSendWithTokenTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestSendWithToken()
    {
        string? receivedContent1 = null;
        string? receivedContent2 = null;
        string? receivedContent3 = null;

        object token1 = new();
        object token2 = new();

        _messenger.Register<string>(this, m => receivedContent1 = m);
        _messenger.Register<string>(this, token1, m => receivedContent2 = m);
        _messenger.Register<string>(this, token2, m => receivedContent3 = m);

        string message1 = "Hello world";
        string message2 = "And again";
        string message3 = "Third one";

        _messenger.Send(message1, token1);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message1, receivedContent2);
        Assert.IsNull(receivedContent3);

        _messenger.Send(message2, token2);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message1, receivedContent2);
        Assert.AreEqual(message2, receivedContent3);

        _messenger.Send(message3);

        Assert.AreEqual(message3, receivedContent1);
        Assert.AreEqual(message1, receivedContent2);
        Assert.AreEqual(message2, receivedContent3);
    }

    [TestMethod]
    public void TestSendMessageBaseWithToken()
    {
        Exception? receivedContent1 = null;
        Exception? receivedContent2 = null;
        Exception? receivedContent3 = null;

        object token1 = new();
        object token2 = new();

        _messenger.Register<Exception>(this, true, m => receivedContent1 = m);
        _messenger.Register<Exception>(this, token1, true, m => receivedContent2 = m);
        _messenger.Register<Exception>(this, token2, true, m => receivedContent3 = m);

        InvalidOperationException message = new();

        _messenger.Send(message, token1);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message, receivedContent2);
        Assert.IsNull(receivedContent3);
    }

    [TestMethod]
    public void TestSendWithIntToken()
    {
        InvalidOperationException? receivedContent1 = null;
        InvalidOperationException? receivedContent2 = null;
        InvalidOperationException? receivedContent3 = null;

        int token1 = 123;
        int token2 = 456;

        _messenger.Register<InvalidOperationException>(this, m => receivedContent1 = m);
        _messenger.Register<InvalidOperationException>(this, token1, m => receivedContent2 = m);
        _messenger.Register<InvalidOperationException>(this, token2, m => receivedContent3 = m);

        InvalidOperationException message = new();

        _messenger.Send(message, token1);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message, receivedContent2);
        Assert.IsNull(receivedContent3);
    }

    [TestMethod]
    public void TestSendNullObjectWithToken()
    {
        bool itemReceived = false;

        object token1 = new();

        _messenger.Register<string>(
            this,
            token1,
            m =>
            {
                itemReceived = true;
            });

        Assert.IsFalse(itemReceived);

        _messenger.Send<string>(null!, token1);

        Assert.IsFalse(itemReceived);
    }
}
