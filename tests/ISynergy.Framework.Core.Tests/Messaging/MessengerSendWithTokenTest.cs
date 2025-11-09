using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerSendWithTokenTest
{
    [TestMethod]
    public void TestSendWithToken()
    {
        string? receivedContent1 = null;
        string? receivedContent2 = null;
        string? receivedContent3 = null;

        var messenger = new MessengerService();

        object token1 = new();
        object token2 = new();

        messenger.Register<string>(this, m => receivedContent1 = m);
        messenger.Register<string>(this, token1, m => receivedContent2 = m);
        messenger.Register<string>(this, token2, m => receivedContent3 = m);

        string message1 = "Hello world";
        string message2 = "And again";
        string message3 = "Third one";

        messenger.Send(message1, token1);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message1, receivedContent2);
        Assert.IsNull(receivedContent3);

        messenger.Send(message2, token2);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message1, receivedContent2);
        Assert.AreEqual(message2, receivedContent3);

        messenger.Send(message3);

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

        var messenger = new MessengerService();

        object token1 = new();
        object token2 = new();

        messenger.Register<Exception>(this, true, m => receivedContent1 = m);
        messenger.Register<Exception>(this, token1, true, m => receivedContent2 = m);
        messenger.Register<Exception>(this, token2, true, m => receivedContent3 = m);

        InvalidOperationException message = new();

        messenger.Send(message, token1);

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

        var messenger = new MessengerService();

        int token1 = 123;
        int token2 = 456;

        messenger.Register<InvalidOperationException>(this, m => receivedContent1 = m);
        messenger.Register<InvalidOperationException>(this, token1, m => receivedContent2 = m);
        messenger.Register<InvalidOperationException>(this, token2, m => receivedContent3 = m);

        InvalidOperationException message = new();

        messenger.Send(message, token1);

        Assert.IsNull(receivedContent1);
        Assert.AreEqual(message, receivedContent2);
        Assert.IsNull(receivedContent3);
    }

    [TestMethod]
    public void TestSendNullObjectWithToken()
    {
        bool itemReceived = false;

        var messenger = new MessengerService();

        object token1 = new();

        messenger.Register<string>(
            this,
            token1,
            m =>
            {
                itemReceived = true;
            });

        Assert.IsFalse(itemReceived);

        messenger.Send<string>(null!, token1);

        Assert.IsFalse(itemReceived);
    }
}
