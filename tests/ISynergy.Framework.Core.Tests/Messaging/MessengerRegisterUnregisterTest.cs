using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

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

    private readonly IMessengerService _messenger;
    private readonly ILogger<MessengerService> _logger;

    public MessengerRegisterUnregisterTest()
    {
        _logger = Mock.Of<ILogger<MessengerService>>();
        _messenger = new MessengerService(_logger);
    }

    [TestMethod]
    public void TestRegisterForGenericMessageBase()
    {
        InvalidOperationException testContentException = new();
        DateTime testContentDateTime = DateTime.Now;
        const string testContentString = "abcd";

        Reset();

        _messenger.Register<TestMessageGenericBase>(
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

        _messenger.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        _messenger.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        _messenger.Send(new TestMessageGeneric<string>
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

        Reset();

        _messenger.Register<TestMessageGeneric<DateTime>>(this, m => ReceivedContentDateTime1 = m.Content);
        _messenger.Register<TestMessageGeneric<Exception>>(this, m => ReceivedContentException = m.Content);
        _messenger.Register<TestMessageGeneric<string>>(this, m => ReceivedContentStringA1 = m.Content);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        _messenger.Send(new TestMessageGeneric<Exception>
        {
            Content = testContentException
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);

        _messenger.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentException, ReceivedContentException);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);

        _messenger.Send(new TestMessageGeneric<string>
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

        Reset();

        int receivedMessages = 0;

        _messenger.Register<TestMessageGenericBase>(
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

        _messenger.Register<TestMessageGeneric<DateTime>>(this,
                                                                 m =>
                                                                 {
                                                                     receivedMessages++;
                                                                     ReceivedContentDateTime2 = m.Content;
                                                                 });

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime2);

        _messenger.Send(new TestMessageGeneric<DateTime>
        {
            Content = testContentDateTime
        });

        Assert.AreEqual(2, receivedMessages);
        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime2);

        _messenger.Send(new TestMessageGeneric<string>
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

        _messenger.Register<object>(
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

        _messenger.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringB);

        _messenger.Send(new TestMessageB
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

        _messenger.Register<TestMessageA>(
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

        _messenger.Send(new TestMessageA
        {
            Content = testContentA
        });

        Assert.AreEqual(testContentA, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        _messenger.Send(new TestMessageAa
        {
            Content = testContentAa
        });

        Assert.AreEqual(testContentAa, ReceivedContentStringA1);
        Assert.AreEqual(testContentAa, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);

        _messenger.Send(new TestMessageB
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

        Reset();

        _messenger.Register<string>(this, m => ReceivedContentStringA1 = m);
        _messenger.Register<DateTime>(this, m => ReceivedContentDateTime1 = m);
        _messenger.Register<int>(this, m => ReceivedContentInt = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        _messenger.Send(testContentString);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(DateTime.MinValue, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        _messenger.Send(testContentDateTime);

        Assert.AreEqual(testContentString, ReceivedContentStringA1);
        Assert.AreEqual(testContentDateTime, ReceivedContentDateTime1);
        Assert.AreEqual(default, ReceivedContentInt);

        _messenger.Send(testContentInt);

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

        Action<TestMessageA> actionA1 = m => ReceivedContentStringA1 = m.Content;

        _messenger.Register(this, actionA1);
        _messenger.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        _messenger.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(_messenger);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA1
        });

        _messenger.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        _messenger.Unregister(this, actionA1);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA2
        });

        _messenger.Send(new TestMessageB
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

        _messenger.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        _messenger.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        _messenger.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(_messenger);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA1
        });

        _messenger.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        _messenger.Unregister(this);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA2
        });

        _messenger.Send(new TestMessageB
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

        _messenger.Register<TestMessageA>(this, m => ReceivedContentStringA1 = m.Content);
        _messenger.Register<TestMessageA>(this, m => ReceivedContentStringA2 = m.Content);
        _messenger.Register<TestMessageB>(this, m => ReceivedContentStringB = m.Content);

        TestRecipient externalRecipient = new();
        externalRecipient.RegisterWith(_messenger);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);
        Assert.AreEqual(null, ReceivedContentStringB);
        Assert.AreEqual(null, externalRecipient.ReceivedContentA);
        Assert.AreEqual(null, externalRecipient.ReceivedContentB);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA1
        });

        _messenger.Send(new TestMessageB
        {
            Content = testContentB1
        });

        Assert.AreEqual(testContentA1, ReceivedContentStringA1);
        Assert.AreEqual(testContentA1, ReceivedContentStringA2);
        Assert.AreEqual(testContentB1, ReceivedContentStringB);
        Assert.AreEqual(testContentA1, externalRecipient.ReceivedContentA);
        Assert.AreEqual(testContentB1, externalRecipient.ReceivedContentB);

        _messenger.Unregister<TestMessageA>(this);

        _messenger.Send(new TestMessageA
        {
            Content = testContentA2
        });

        _messenger.Send(new TestMessageB
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

        _messenger.Register<IMessage>(this, true, m => ReceivedContentStringA1 = m.GetValue());

        Assert.AreEqual(null, ReceivedContentStringA1);

        _messenger.Send(new TestMessageImplementsIMessage(testContent1));

        Assert.AreEqual(testContent1, ReceivedContentStringA1);

        _messenger.Unregister<IMessage>(this);

        _messenger.Send(new TestMessageImplementsIMessage(testContent2));

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

        Action<string> action1 = m => ReceivedContentStringA1 = m;
        Action<string> action2 = m => ReceivedContentStringA2 = m;
        Action<string> action3 = m => ReceivedContentStringB = m;

        _messenger.Register(this, token1, action1);
        _messenger.Register(this, token2, action2);
        _messenger.Register(this, token2, action3);

        _messenger.Send(testContent1, token1);
        _messenger.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
        Assert.AreEqual(testContent2, ReceivedContentStringB);

        _messenger.Unregister(this, token2, action3);
        _messenger.Send(testContent3, token1);
        _messenger.Send(testContent4, token2);

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

        _messenger.Register<string>(this, token1, m => ReceivedContentStringA1 = m);
        _messenger.Register<string>(this, token2, m => ReceivedContentStringA2 = m);

        Assert.AreEqual(null, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        _messenger.Send(testContent1, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(null, ReceivedContentStringA2);

        _messenger.Send(testContent2, token2);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);

        _messenger.Unregister<string>(this, token1);
        _messenger.Send(testContent3, token1);

        Assert.AreEqual(testContent1, ReceivedContentStringA1);
        Assert.AreEqual(testContent2, ReceivedContentStringA2);
    }

    [TestMethod]
    public void TestRegisterStaticHandler()
    {
        Reset();

        _context = this;

        _messenger.Register<TestMessageImpl>(
            this,
            msg =>
            {
                if (msg.Sender != _context || !msg.Result)
                    return;

                _result = msg.Result;
            });

        Assert.IsFalse(_result);
        _messenger.Send(
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

        internal void RegisterWith(IMessengerService _messenger)
        {
            _messenger.Register<TestMessageA>(this, m => ReceivedContentA = m.Content);
            _messenger.Register<TestMessageB>(this, m => ReceivedContentB = m.Content);
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
