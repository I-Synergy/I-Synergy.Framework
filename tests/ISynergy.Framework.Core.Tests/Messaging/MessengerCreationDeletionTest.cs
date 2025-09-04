using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerCreationDeletionTest
{
    public string? StringContent
    {
        get;
        private set;
    }

    public void Reset()
    {
        StringContent = null;
    }

    [TestMethod]
    public void TestDeletingRecipient()
    {
        TestRecipient1 recipient1 = new();
        TestRecipient1 recipient2 = new();

        const string TestContent1 = "abcd";
        const string TestContent2 = "efgh";

        MessengerService.Reset();

        MessengerService.Default.Register<string>(recipient1, recipient1.ReceiveMessage);
        MessengerService.Default.Register<string>(recipient2, recipient2.ReceiveMessage);

        Assert.AreEqual(null, recipient1.ReceivedContentString);
        Assert.AreEqual(null, recipient2.ReceivedContentString);

        MessengerService.Default.Send(TestContent1);

        Assert.AreEqual(TestContent1, recipient1.ReceivedContentString);
        Assert.AreEqual(TestContent1, recipient2.ReceivedContentString);

        recipient1 = null!;
        GC.Collect();

        MessengerService.Default.Send(TestContent2);

        Assert.AreEqual(TestContent2, recipient2.ReceivedContentString);

        recipient2 = null!;
        GC.Collect();

        MessengerService.Default.Send(TestContent2);
    }

    [TestMethod]
    public void TestInstanceCreation()
    {
        const string TestContent = "abcd";

        Reset();
        MessengerService messenger = new();

        messenger.Register<TestMessage>(this, m => StringContent = m.Content);

        Assert.AreEqual(null, StringContent);

        messenger.Send(new TestMessage
        {
            Content = TestContent
        });

        Assert.AreEqual(TestContent, StringContent);
    }

    [TestMethod]
    public void TestStaticCreation()
    {
        const string TestContent = "abcd";

        Reset();
        MessengerService.Reset();

        MessengerService.Default.Register<TestMessage>(this, m => StringContent = m.Content);

        Assert.AreEqual(null, StringContent);

        MessengerService.Default.Send(new TestMessage
        {
            Content = TestContent
        });

        Assert.AreEqual(TestContent, StringContent);
    }

    [TestMethod]
    public void TestStaticDeletion()
    {
        const string TestContent = "abcd";

        Reset();
        MessengerService.Reset();

        MessengerService.Default.Register<TestMessage>(this, m => StringContent = m.Content);

        Assert.AreEqual(null, StringContent);

        MessengerService.Reset();
        MessengerService.Default.Send(new TestMessage
        {
            Content = TestContent
        });

        Assert.AreEqual(null, StringContent);
    }

    public class TestMessage
    {
        public string? Content
        {
            get;
            set;
        }
    }

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
}
