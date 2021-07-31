﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerSendToTypeTest
    {
        [TestMethod]
        public void TestBroadcastToOneType()
        {
            Messenger.Reset();

            var recipient11 = new TestRecipient1();
            var recipient12 = new TestRecipient1();
            var recipient21 = new TestRecipient2();
            var recipient22 = new TestRecipient2();

            Messenger.Default.Register<string>(recipient11, recipient11.ReceiveMessage);
            Messenger.Default.Register<string>(recipient12, recipient12.ReceiveMessage);
            Messenger.Default.Register<string>(recipient21, recipient21.ReceiveMessage);
            Messenger.Default.Register<string>(recipient22, recipient22.ReceiveMessage);

            const string testContent1 = "abcd";
            const string testContent2 = "efgh";

            Assert.AreEqual(null, recipient11.ReceivedContentString);
            Assert.AreEqual(null, recipient12.ReceivedContentString);
            Assert.AreEqual(null, recipient21.ReceivedContentString);
            Assert.AreEqual(null, recipient22.ReceivedContentString);

            Messenger.Default.Send<string, TestRecipient1>(testContent1);

            Assert.AreEqual(testContent1, recipient11.ReceivedContentString);
            Assert.AreEqual(testContent1, recipient12.ReceivedContentString);
            Assert.AreEqual(null, recipient21.ReceivedContentString);
            Assert.AreEqual(null, recipient22.ReceivedContentString);

            Messenger.Default.Send<string, TestRecipient2>(testContent2);

            Assert.AreEqual(testContent1, recipient11.ReceivedContentString);
            Assert.AreEqual(testContent1, recipient12.ReceivedContentString);
            Assert.AreEqual(testContent2, recipient21.ReceivedContentString);
            Assert.AreEqual(testContent2, recipient22.ReceivedContentString);
        }

        [TestMethod]
        public void TestBroadcastToOneInterface()
        {
            Messenger.Reset();

            var recipient11 = new TestRecipient1();
            var recipient12 = new TestRecipient1();
            var recipient21 = new TestRecipient2();
            var recipient22 = new TestRecipient2();
            var recipient31 = new TestRecipient3();
            var recipient32 = new TestRecipient3();

            Messenger.Default.Register<string>(recipient11, recipient11.ReceiveMessage);
            Messenger.Default.Register<string>(recipient12, recipient12.ReceiveMessage);
            Messenger.Default.Register<string>(recipient21, recipient21.DoSomething);
            Messenger.Default.Register<string>(recipient22, recipient22.DoSomething);
            Messenger.Default.Register<string>(recipient31, recipient31.DoSomething);
            Messenger.Default.Register<string>(recipient32, recipient32.DoSomething);

            const string testContent1 = "abcd";

            Assert.AreEqual(null, recipient11.ReceivedContentString);
            Assert.AreEqual(null, recipient12.ReceivedContentString);
            Assert.AreEqual(null, recipient21.ReceivedContentString);
            Assert.AreEqual(null, recipient22.ReceivedContentString);
            Assert.AreEqual(null, recipient31.ReceivedContentString);
            Assert.AreEqual(null, recipient32.ReceivedContentString);

            Messenger.Default.Send<string, ITestRecipient>(testContent1);

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
            public string ReceivedContentString
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
            public string ReceivedContentString
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
            public string ReceivedContentString
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
}
