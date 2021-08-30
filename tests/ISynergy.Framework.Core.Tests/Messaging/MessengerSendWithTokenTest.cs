using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerSendWithTokenTest
    {
        [TestMethod]
        public void TestSendWithToken()
        {
            string receivedContent1 = null;
            string receivedContent2 = null;
            string receivedContent3 = null;

            MessageService.Reset();

            var token1 = new object();
            var token2 = new object();

            MessageService.Default.Register<string>(this, m => receivedContent1 = m);
            MessageService.Default.Register<string>(this, token1, m => receivedContent2 = m);
            MessageService.Default.Register<string>(this, token2, m => receivedContent3 = m);

            var message1 = "Hello world";
            var message2 = "And again";
            var message3 = "Third one";

            MessageService.Default.Send(message1, token1);

            Assert.IsNull(receivedContent1);
            Assert.AreEqual(message1, receivedContent2);
            Assert.IsNull(receivedContent3);

            MessageService.Default.Send(message2, token2);

            Assert.IsNull(receivedContent1);
            Assert.AreEqual(message1, receivedContent2);
            Assert.AreEqual(message2, receivedContent3);

            MessageService.Default.Send(message3);

            Assert.AreEqual(message3, receivedContent1);
            Assert.AreEqual(message1, receivedContent2);
            Assert.AreEqual(message2, receivedContent3);
        }

        [TestMethod]
        public void TestSendMessageBaseWithToken()
        {
            Exception receivedContent1 = null;
            Exception receivedContent2 = null;
            Exception receivedContent3 = null;

            MessageService.Reset();

            var token1 = new object();
            var token2 = new object();

            MessageService.Default.Register<Exception>(this, true, m => receivedContent1 = m);
            MessageService.Default.Register<Exception>(this, token1, true, m => receivedContent2 = m);
            MessageService.Default.Register<Exception>(this, token2, true, m => receivedContent3 = m);

            var message = new InvalidOperationException();

            MessageService.Default.Send(message, token1);

            Assert.IsNull(receivedContent1);
            Assert.AreEqual(message, receivedContent2);
            Assert.IsNull(receivedContent3);
        }

        [TestMethod]
        public void TestSendWithIntToken()
        {
            InvalidOperationException receivedContent1 = null;
            InvalidOperationException receivedContent2 = null;
            InvalidOperationException receivedContent3 = null;

            MessageService.Reset();

            var token1 = 123;
            var token2 = 456;

            MessageService.Default.Register<InvalidOperationException>(this, m => receivedContent1 = m);
            MessageService.Default.Register<InvalidOperationException>(this, token1, m => receivedContent2 = m);
            MessageService.Default.Register<InvalidOperationException>(this, token2, m => receivedContent3 = m);

            var message = new InvalidOperationException();

            MessageService.Default.Send(message, token1);

            Assert.IsNull(receivedContent1);
            Assert.AreEqual(message, receivedContent2);
            Assert.IsNull(receivedContent3);
        }

        [TestMethod]
        public void TestSendNullObjectWithToken()
        {
            bool itemReceived = false;
            bool itemIsNull = false;

            MessageService.Reset();

            var token1 = new object();

            MessageService.Default.Register<string>(
                this,
                token1,
                m =>
                {
                    itemReceived = true;

                    if (m == null)
                    {
                        itemIsNull = true;
                    }
                });

            Assert.IsFalse(itemReceived);
            Assert.IsFalse(itemIsNull);

            MessageService.Default.Send<string>(null, token1);

            Assert.IsTrue(itemReceived);
            Assert.IsTrue(itemIsNull);
        }
    }
}
