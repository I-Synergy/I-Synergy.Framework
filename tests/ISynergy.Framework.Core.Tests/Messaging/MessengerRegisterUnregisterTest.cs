using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerRegisterUnregisterTest
{
    public DateTime ReceivedContentDateTime1 { get; set; }
    public DateTime ReceivedContentDateTime2 { get; set; }
    public Exception? ReceivedContentException { get; set; }
    public int ReceivedContentInt { get; set; }
    public string? ReceivedContentStringA1 { get; set; }
    public string? ReceivedContentStringA2 { get; set; }
    public string? ReceivedContentStringB { get; set; }

    [TestMethod]
    public void TestRegisterForGenericMessageBase()
    {
        InvalidOperationException testContentException = new();
        DateTime testContentDateTime = DateTime.Now;
        const string testContentString = "abcd";

        MessengerService.Reset();
        Reset();

        MessengerService.Default.Register<TestMessageGenericBase>(
            this,
            true,
            m =>
            {
                var exceptionMessage = m as TestMessageGeneric<Exception>;
                if (exceptionMessage is not null)
                {
                    ReceivedContentException =
                        exceptionMessage.Content;
                    return;
                }

                var dateTimeMessage = m as TestMessageGeneric<DateTime>;
                if (dateTimeMessage is not null)
                {
                    ReceivedContentDateTime1 =
                        dateTimeMessage.Content;
                    return;
                }

                var stringMessage = m as TestMessageGeneric<string>;
                if (stringMessage is not null)
                {
                    ReceivedContentStringA1 = stringMessage.Content;
                    return;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<string>
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

        MessengerService.Reset();
        Reset();

        MessengerService.Default.Register<TestMessageGeneric<DateTime>>(this, m => ReceivedContentDateTime1 = m.Content);
        MessengerService.Default.Register<TestMessageGeneric<Exception>>(this, m => ReceivedContentException = m.Content);
        MessengerService.Default.Register<TestMessageGeneric<string>>(this, m => ReceivedContentStringA1 = m.Content);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        MessengerService.Default.Send(new TestMessageGeneric<string>
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

        MessengerService.Reset();
        Reset();

        int receivedMessages = 0;

        MessengerService.Default.Register<TestMessageGenericBase>(
            this,
            true,
            m =>
            {
                receivedMessages++;

                var dateTimeMessage = m as TestMessageGeneric<DateTime>;
                if (dateTimeMessage is not null)
                {
                    ReceivedContentDateTime1 =
                        dateTimeMessage.Content;
                    return;
                }

                var stringMessage = m as TestMessageGeneric<string>;
                if (stringMessage is not null)
                {
                    ReceivedContentStringA1 = stringMessage.Content;
                    return;
                }
            });

        MessengerService.Default.Register<TestMessageGeneric<DateTime>>(this,
                                                                 m =>
                                                                 {
                                                                     receivedMessages++;
                                                                     ReceivedContentDateTime2 = m.Content;
                                                                 });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime2);

        MessengerService.Default.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(2, receivedMessages);
        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime2);

        MessengerService.Default.Send(new TestMessageGeneric<string>
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
        public T? Content
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
        MessengerService.Reset();

        MessengerService.Default.Register<object>(
            this,
            true,
            m =>
            {
                var messageA = m as TestMessageA;
                if (messageA is not null)
                {
                    ReceivedContentStringA1 = messageA.Content;
                    return;
                }

                var messageB = m as TestMessageB;
                if (messageB is not null)
                {
                    ReceivedContentStringB = messageB.Content;
                    return;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessengerService.Default.Send(new TestMessageB
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
        MessengerService.Reset();

        MessengerService.Default.Register<TestMessageA>(
            this,
            true,
            m =>
            {
                TestMessageA messageA = m;
                if (messageA is not null)
                {
                    ReceivedContentStringA1 = messageA.Content;
                }

                var messageAa = m as TestMessageAa;
                if (messageAa is not null)
                {
                    ReceivedContentStringA2 = messageAa.Content;
                }
            });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessengerService.Default.Send(new TestMessageAa
        {
            Content = testContentAa
        });

        Assert.AreEqual(testContentAa, ReceivedContentStringA1);
        Assert.AreEqual(testContentAa, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        MessengerService.Default.Send(new TestMessageB
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
        var testContentDateTime = new DateTime(2025, 5, 20, 13, 34, 1);
        const int testContentInt = 42;

        MessengerService.Reset();
        Reset();

        MessengerService.Default.Register<string>(this, m => ReceivedContentStringA1 = m);
        MessengerService.Default.Register<DateTime>(this, m => ReceivedContentDateTime1 = m);
        MessengerService.Default.Register<int>(this, m => ReceivedContentInt = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessengerService.Default.Send(testContentString);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessengerService.Default.Send(testContentDateTime);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        MessengerService.Default.Send(testContentInt);

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
        MessengerService.Reset();

        Action<TestMessageA> actionA1 = m => ReceivedContentStringA1 = m.Content;

        MessengerService.Default.Register(this, actionA1);
        MessengerService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessengerService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessengerService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessengerService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessengerService.Default.Unregister(this, actionA1);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessengerService.Default.Send(new TestMessageB
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
        MessengerService.Reset();

        MessengerService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        MessengerService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessengerService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessengerService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessengerService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessengerService.Default.Unregister(this);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessengerService.Default.Send(new TestMessageB
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
        MessengerService.Reset();

        MessengerService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        MessengerService.Default.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        MessengerService.Default.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(MessengerService.Default);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA1
        });

        MessengerService.Default.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        MessengerService.Default.Unregister<TestMessageA>(this);

        MessengerService.Default.Send(new TestMessageA
        {
            Content = testContentA2
        });

        MessengerService.Default.Send(new TestMessageB
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
        MessengerService.Reset();

        MessengerService.Default.Register<IMessage>(this, true, m => ReceivedContentStringA1 = m.GetValue());

        Assert.AreEqual(null, ReceivedContentStringA1);

        MessengerService.Default.Send(new TestMessageImplementsIMessage(testContent1));

        Assert.AreEqual(testContent1, ReceivedContentStringA1);

        MessengerService.Default.Unregister<IMessage>(this);

        MessengerService.Default.Send(new TestMessageImplementsIMessage(testContent2));

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
        MessengerService.Reset();

        Action<string> action1 = m => ReceivedContentStringA1 = m;
        Action<string> action2 = m => ReceivedContentStringA2 = m;
        Action<string> action3 = m => ReceivedContentStringB = m;

        MessengerService.Default.Register(this, token1, action1);
        MessengerService.Default.Register(this, token2, action2);
        MessengerService.Default.Register(this, token2, action3);

        MessengerService.Default.Send(testContent1, token1);
        MessengerService.Default.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
        Assert.AreEqual(testContent2, ReceivedContentStringB);

        MessengerService.Default.Unregister(this, token2, action3);
        MessengerService.Default.Send(testContent3, token1);
        MessengerService.Default.Send(testContent4, token2);

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
        MessengerService.Reset();

        MessengerService.Default.Register<string>(this, token1, m => ReceivedContentStringA1 = m);
        MessengerService.Default.Register<string>(this, token2, m => ReceivedContentStringA2 = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        MessengerService.Default.Send(testContent1, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        MessengerService.Default.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);

        MessengerService.Default.Unregister<string>(this, token1);
        MessengerService.Default.Send(testContent3, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
    }

    [TestMethod]
    public void TestRegisterStaticHandler()
    {
        Reset();
        MessengerService.Reset();

        _context = this;

        MessengerService.Default.Register<TestMessageImpl>(
            this,
            msg =>
            {
                if (msg.Sender != _context || !msg.Result)
                    return;

                _result = msg.Result;
            });

        Assert.IsFalse(_result);
        MessengerService.Default.Send(
            new TestMessageImpl(_context)
            {
                Result = true
            });
        Assert.IsTrue(_result);
    }

    private static bool _result;
    private static object? _context;

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

        _context = null!;
        _result = false;
    }

    public class TestMessageA
    {
        public string? Content
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
        public string? Content
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
        public string? ReceivedContentA
        {
            get;
            private set;
        }

        public string? ReceivedContentB
        {
            get;
            private set;
        }

        internal void RegisterWith(IMessengerService messenger)
        {
            messenger.Register<TestMessageA>(this, m => ReceivedContentA = m.Content);
            messenger.Register<TestMessageB>(this, m => ReceivedContentB = m.Content);
        }
    }

    private interface IMessage
    {
        string GetValue();
    }

    public class TestMessageImplementsIMessage(string value) : IMessage
    {
        private string Value
        {
            get;
            set;
        } = value;

        public string GetValue()
        {
            return Value;
        }
    }

}
