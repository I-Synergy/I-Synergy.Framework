using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerRegisterUnregisterTest
{
    public DateTime ReceivedContentDateTime1
    {
        get;
        private set;
    }

    public DateTime ReceivedContentDateTime2
    {
        get;
        private set;
    }

    public Exception ReceivedContentException
    {
        get;
        private set;
    }

    public int ReceivedContentInt
    {
        get;
        private set;
    }

    public string ReceivedContentStringA1
    {
        get;
        private set;
    }

    public string ReceivedContentStringA2
    {
        get;
        private set;
    }

    public string ReceivedContentStringB
    {
        get;
        private set;
    }

    [TestMethod]
    public void TestRegisterForGenericMessageBase()
    {
        InvalidOperationException testContentException = new();
        DateTime testContentDateTime = DateTime.Now;
        const string testContentString = "abcd";

        MessageService.Reset();
        Reset();

        MessageService.Default.Register<TestMessageGenericBase>(
            this,
            true,
            m =>
            {
                TestMessageGeneric<Exception> exceptionMessage =
                    m as TestMessageGeneric<Exception>;
                if (exceptionMessage is not null)
                {
                    ReceivedContentException =
                        exceptionMessage.Content;
                    return;
                }

                TestMessageGeneric<DateTime> dateTimeMessage =
                    m as TestMessageGeneric<DateTime>;
                if (dateTimeMessage is not null)
                {
                    ReceivedContentDateTime1 =
                        dateTimeMessage.Content;
                    return;
                }

                TestMessageGeneric<string> stringMessage = m as TestMessageGeneric<string>;
                if (stringMessage is not null)
                {
                    ReceivedContentStringA1 = stringMessage.Content;
                    return;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<string>
        {
            Content = testContentString
        });

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
    }

    [TestMethod]
    public void TestRegisterForGenericMessageSpecific()
    {
        InvalidOperationException testContentException = new();
        DateTime testContentDateTime = DateTime.Now;
        const string testContentString = "abcd";

        MessageService.Reset();
        Reset();

        MessageService.Default.Register<TestMessageGeneric<DateTime>>(this, m => ReceivedContentDateTime1 = m.Content);
        MessageService.Default.Register<TestMessageGeneric<Exception>>(this, m => ReceivedContentException = m.Content);
        MessageService.Default.Register<TestMessageGeneric<string>>(this, m => ReceivedContentStringA1 = m.Content);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        MessageService.Default.Send(new TestMessageGeneric<string>
        {
            Content = testContentString
        });

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
    }

    [TestMethod]
    public void TestRegisterStrictAndNonStrictRecipients()
    {
        DateTime testContentDateTime = DateTime.Now;
        const string testContentString = "abcd";

        MessageService.Reset();
        Reset();

        int receivedMessages = 0;

        MessageService.Default.Register<TestMessageGenericBase>(
            this,
            true,
            m =>
            {
                receivedMessages++;

                TestMessageGeneric<DateTime> dateTimeMessage =
                    m as TestMessageGeneric<DateTime>;
                if (dateTimeMessage is not null)
                {
                    ReceivedContentDateTime1 =
                        dateTimeMessage.Content;
                    return;
                }

                TestMessageGeneric<string> stringMessage = m as TestMessageGeneric<string>;
                if (stringMessage is not null)
                {
                    ReceivedContentStringA1 = stringMessage.Content;
                    return;
                }
            });

        MessageService.Default.Register<TestMessageGeneric<DateTime>>(this,
                                                                 m =>
                                                                 {
                                                                     receivedMessages++;
                                                                     ReceivedContentDateTime2 = m.Content;
                                                                 });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime2);

        MessageService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(2, receivedMessages);
        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime2);

        MessageService.Default.Send(new TestMessageGeneric<string>
        {
            Content = testContentString
        });

        Assert.AreEqual(3, receivedMessages);
        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime2);
    }

    public class TestMessageGeneric<T> : TestMessageGenericBase
    {
        public T Content
        {
            get;
            set;
        }
    }

    [TestMethod]
    public void TestRegisterForSubclassesOfObject()
    {
        const string testContentA = "abcd";
        const string testContentB = "efgh";

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<object>(
            this,
            true,
            m =>
            {
                TestMessageA messageA = m as TestMessageA;
                if (messageA is not null)
                {
                    ReceivedContentStringA1 = messageA.Content;
                    return;
                }

                TestMessageB messageB = m as TestMessageB;
                if (messageB is not null)
                {
                    ReceivedContentStringB = messageB.Content;
                    return;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(testContentB, ReceivedContentStringB);
    }

    [TestMethod]
    public void TestRegisterForSubclassesOfTestMessageA()
    {
        const string testContentA = "abcd";
        const string testContentAa = "1234";
        const string testContentB = "efgh";

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<TestMessageA>(
            this,
            true,
            m =>
            {
                TestMessageA messageA = m;
                if (messageA is not null)
                {
                    ReceivedContentStringA1 = messageA.Content;
                }

                TestMessageAa messageAa = m as TestMessageAa;
                if (messageAa is not null)
                {
                    ReceivedContentStringA2 = messageAa.Content;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessageService.Default.Send(new TestMessageAa
        {
            Content = testContentAa
        });

        Assert.AreEqual(testContentAa, ReceivedContentStringA1);
        Assert.AreEqual(testContentAa, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB
        });

        Assert.AreEqual(testContentAa, ReceivedContentStringA1);
        Assert.AreEqual(testContentAa, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
    }

    [TestMethod]
    public void TestRegisterSimpleTypes()
    {
        const string testContentString = "abcd";
        DateTime testContentDateTime = DateTime.Now;
        const int testContentInt = 42;

        MessageService.Reset();
        Reset();

        MessageService.Default.Register<string>(this, m => ReceivedContentStringA1 = m);
        MessageService.Default.Register<DateTime>(this, m => ReceivedContentDateTime1 = m);
        MessageService.Default.Register<int>(this, m => ReceivedContentInt = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessageService.Default.Send(testContentString);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessageService.Default.Send(testContentDateTime);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessageService.Default.Send(testContentInt);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(testContentInt, ReceivedContentInt);
    }

    [TestMethod]
    public void TestUnregisterOneAction()
    {
        const string testContentA1 = "abcd";
        const string testContentB1 = "1234";
        const string testContentA2 = "efgh";
        const string testContentB2 = "5678";

        Reset();
        MessageService.Reset();

        Action<TestMessageA> actionA1 = m => ReceivedContentStringA1 = m.Content;

        MessageService.Default.Register(this, actionA1);
        MessageService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessageService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessageService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessageService.Default.Unregister(this, actionA1);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB2
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA2, ReceivedContentStringA2);
        Assert.AreEqual(testContentB2, ReceivedContentStringB);
        Assert.AreEqual(testContentA2, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB2, externalRecipient.ReceivedContentB);
    }

    [TestMethod]
    public void TestUnregisterOneInstance()
    {
        const string testContentA1 = "abcd";
        const string testContentB1 = "1234";
        const string testContentA2 = "efgh";
        const string testContentB2 = "5678";

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        MessageService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessageService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessageService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessageService.Default.Unregister(this);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB2
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA2, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB2, externalRecipient.ReceivedContentB);
    }

    [TestMethod]
    public void TestUnregisterOneMessageType()
    {
        const string testContentA1 = "abcd";
        const string testContentB1 = "1234";
        const string testContentA2 = "efgh";
        const string testContentB2 = "5678";

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        MessageService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessageService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessageService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessageService.Default.Unregister<TestMessageA>(this);

        MessageService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessageService.Default.Send(new TestMessageB
        {
            Content = testContentB2
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB2, ReceivedContentStringB);
        Assert.AreEqual(testContentA2, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB2, externalRecipient.ReceivedContentB);
    }

    [TestMethod]
    public void TestRegisterUnregisterInterfaceMessage()
    {
        const string testContent1 = "abcd";
        const string testContent2 = "efgh";

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<IMessage>(this, true, m => ReceivedContentStringA1 = m.GetValue());

        Assert.AreEqual(null, ReceivedContentStringA1);

        MessageService.Default.Send(new TestMessageImplementsIMessage(testContent1));

        Assert.AreEqual(testContent1, ReceivedContentStringA1);

        MessageService.Default.Unregister<IMessage>(this);

        MessageService.Default.Send(new TestMessageImplementsIMessage(testContent2));

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
    }

    [TestMethod]
    public void TestRegisterUnregisterOneActionWithToken()
    {
        const string testContent1 = "abcd";
        const string testContent2 = "efgh";
        const string testContent3 = "ijkl";
        const string testContent4 = "mnop";
        const int token1 = 1234;
        const int token2 = 4567;

        Reset();
        MessageService.Reset();

        Action<string> action1 = m => ReceivedContentStringA1 = m;
        Action<string> action2 = m => ReceivedContentStringA2 = m;
        Action<string> action3 = m => ReceivedContentStringB = m;

        MessageService.Default.Register(this, token1, action1);
        MessageService.Default.Register(this, token2, action2);
        MessageService.Default.Register(this, token2, action3);

        MessageService.Default.Send(testContent1, token1);
        MessageService.Default.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
        Assert.AreEqual(testContent2, ReceivedContentStringB);

        MessageService.Default.Unregister(this, token2, action3);
        MessageService.Default.Send(testContent3, token1);
        MessageService.Default.Send(testContent4, token2);

        Assert.AreEqual(testContent3, ReceivedContentStringA1);
        Assert.AreEqual(testContent4, ReceivedContentStringA2);
        Assert.AreEqual(testContent2, ReceivedContentStringB);
    }

    [TestMethod]
    public void TestRegisterUnregisterWithToken()
    {
        const string testContent1 = "abcd";
        const string testContent2 = "efgh";
        const string testContent3 = "ijkl";
        const int token1 = 1234;
        const int token2 = 4567;

        Reset();
        MessageService.Reset();

        MessageService.Default.Register<string>(this, token1, m => ReceivedContentStringA1 = m);
        MessageService.Default.Register<string>(this, token2, m => ReceivedContentStringA2 = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        MessageService.Default.Send(testContent1, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        MessageService.Default.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);

        MessageService.Default.Unregister<string>(this, token1);
        MessageService.Default.Send(testContent3, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
    }

    [TestMethod]
    public void TestRegisterStaticHandler()
    {
        Reset();
        MessageService.Reset();

        _context = this;

        MessageService.Default.Register<TestMessageImpl>(
            this,
            msg =>
            {
                if (msg.Sender != _context || !msg.Result)
                    return;

                _result = msg.Result;
            });

        Assert.IsFalse(_result);
        MessageService.Default.Send(
            new TestMessageImpl(_context)
            {
                Result = true
            });
        Assert.IsTrue(_result);
    }

    private static bool _result;
    private static object _context;

    public static void DoSomethingStatic(bool result)
    {
        _result = result;
    }

    //// Helpers

    private void Reset()
    {
        ReceivedContentStringA1 = null;
        ReceivedContentStringA2 = null;
        ReceivedContentStringB = null;
        ReceivedContentInt = default;

        ReceivedContentDateTime1 = DateTime.MinValue;
        ReceivedContentDateTime2 = DateTime.MinValue;
        ReceivedContentException = null;

        _context = null;
        _result = false;
    }

    public class TestMessageA
    {
        public string Content
        {
            get;
            set;
        }
    }

    public class TestMessageAa : TestMessageA
    {
    }

    public class TestMessageB
    {
        public string Content
        {
            get;
            set;
        }
    }

    public class TestMessageGenericBase
    {
    }

    private class TestRecipient
    {
        public string ReceivedContentA
        {
            get;
            private set;
        }

        public string ReceivedContentB
        {
            get;
            private set;
        }

        internal void RegisterWith(IMessageService messenger)
        {
            messenger.Register<TestMessageA>(this, m => ReceivedContentA = m.Content);
            messenger.Register<TestMessageB>(this, m => ReceivedContentB = m.Content);
        }
    }

    private interface IMessage
    {
        string GetValue();
    }

    public class TestMessageImplementsIMessage : IMessage
    {
        private string Value
        {
            get;
            set;
        }

        public TestMessageImplementsIMessage(string value)
        {
            Value = value;
        }

        public string GetValue()
        {
            return Value;
        }
    }

}
