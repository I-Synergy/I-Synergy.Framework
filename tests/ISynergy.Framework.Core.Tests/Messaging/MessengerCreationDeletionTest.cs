﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerCreationDeletionTest
    {
        public string StringContent
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
            var recipient1 = new TestRecipient1();
            var recipient2 = new TestRecipient1();

            const string TestContent1 = "abcd";
            const string TestContent2 = "efgh";

            Messenger.Reset();

            Messenger.Default.Register<string>(recipient1, recipient1.ReceiveMessage);
            Messenger.Default.Register<string>(recipient2, recipient2.ReceiveMessage);

            Assert.AreEqual(null, recipient1.ReceivedContentString);
            Assert.AreEqual(null, recipient2.ReceivedContentString);

            Messenger.Default.Send(TestContent1);

            Assert.AreEqual(TestContent1, recipient1.ReceivedContentString);
            Assert.AreEqual(TestContent1, recipient2.ReceivedContentString);

            recipient1 = null;
            GC.Collect();

            Messenger.Default.Send(TestContent2);

            Assert.AreEqual(TestContent2, recipient2.ReceivedContentString);

            recipient2 = null;
            GC.Collect();

            Messenger.Default.Send(TestContent2);
        }

        [TestMethod]
        public void TestInstanceCreation()
        {
            const string TestContent = "abcd";

            Reset();
            var messenger = new Messenger();

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
            Messenger.Reset();

            Messenger.Default.Register<TestMessage>(this, m => StringContent = m.Content);

            Assert.AreEqual(null, StringContent);

            Messenger.Default.Send(new TestMessage
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
            Messenger.Reset();

            Messenger.Default.Register<TestMessage>(this, m => StringContent = m.Content);

            Assert.AreEqual(null, StringContent);

            Messenger.Reset();
            Messenger.Default.Send(new TestMessage
            {
                Content = TestContent
            });

            Assert.AreEqual(null, StringContent);
        }

        public class TestMessage
        {
            public string Content
            {
                get;
                set;
            }
        }

        private class TestRecipient1
        {
            public string ReceivedContentString
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
}
