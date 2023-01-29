using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class MessengerOverrideDefaultTest
    {
        private string ReceivedContent
        {
            get;
            set;
        }

        [TestMethod]
        public void TestOverrideDefault()
        {
            const string TestContent1 = "Test content 1";
            const string TestContent2 = "Test content 2";
            const string TestContent3 = "Test content 3";

            MessageService.Reset();

            MessageService.Default.Register<string>(this, m => ReceivedContent = m);
            Assert.IsNull(ReceivedContent);
            MessageService.Default.Send(TestContent1);
            Assert.AreEqual(TestContent1, ReceivedContent);

            MessageService.Reset();
            MessageService.OverrideDefault(new TestMessenger());

            MessageService.Default.Send(TestContent2);
            Assert.AreEqual(1, ((TestMessenger)MessageService.Default).MessagesTransmitted);
            Assert.AreEqual(TestContent1, ReceivedContent);

            MessageService.Default.Register<string>(this, m => ReceivedContent = m);
            Assert.AreEqual(1, ((TestMessenger)MessageService.Default).MessagesTransmitted);
            MessageService.Default.Send(TestContent3);
            Assert.AreEqual(TestContent3, ReceivedContent);
            Assert.AreEqual(2, ((TestMessenger)MessageService.Default).MessagesTransmitted);
        }

        // Helpers

        private class TestMessenger : MessageService
        {
            private readonly List<Delegate> _recipients = new List<Delegate>();

            public int MessagesTransmitted
            {
                get;
                private set;
            }

            public override void Send<TMessage>(TMessage message)
            {
                MessagesTransmitted++;

                base.Send(message);
            }
        }
    }
}
