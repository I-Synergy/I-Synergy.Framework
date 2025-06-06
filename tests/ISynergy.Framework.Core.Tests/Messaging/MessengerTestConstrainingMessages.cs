﻿using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class MessengerTestConstrainingMessages
{
    private static readonly string TestContent = Guid.NewGuid().ToString();

    private bool _messageWasReceived;
    private bool _messageWasReceivedInITestMessage;
    private bool _messageWasReceivedInTestMessageBase;
    private bool _messageWasReceivedInMessageBase;

    private void Reset()
    {
        _messageWasReceived = false;
        _messageWasReceivedInITestMessage = false;
        _messageWasReceivedInTestMessageBase = false;
        _messageWasReceivedInMessageBase = false;
    }

    [TestMethod]
    public void TestConstrainingMessageByInterface()
    {
        Reset();
        MessageService.Reset();
        MessageService.Default.Register<ITestMessage>(this, ReceiveITestMessage);

        TestMessageImpl testMessage = new(this)
        {
            Content = TestContent
        };

        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
        MessageService.Default.Send(testMessage);
        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);

        MessageService.Default.Unregister<ITestMessage>(this);
        MessageService.Default.Register<ITestMessage>(this, true, ReceiveITestMessage);

        MessageService.Default.Send(testMessage);
        Assert.IsTrue(_messageWasReceived);
        Assert.IsTrue(_messageWasReceivedInITestMessage);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
    }

    [TestMethod]
    public void TestConstrainingMessageByBaseClass()
    {
        Reset();
        MessageService.Reset();
        MessageService.Default.Register<TestMessageBase>(this, ReceiveTestMessageBase);

        TestMessageImpl testMessage = new(this)
        {
            Content = TestContent
        };

        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
        MessageService.Default.Send(testMessage);
        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);

        MessageService.Default.Unregister<ITestMessage>(this);
        MessageService.Default.Register<TestMessageBase>(this, true, ReceiveTestMessageBase);

        MessageService.Default.Send(testMessage);
        Assert.IsTrue(_messageWasReceived);
        Assert.IsTrue(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
    }

    [TestMethod]
    public void TestConstrainingMessageByBaseClassAndReceiveWithInterface()
    {
        Reset();
        MessageService.Reset();
        MessageService.Default.Register<TestMessageBase>(this, ReceiveITestMessage);

        TestMessageImpl testMessage = new(this)
        {
            Content = TestContent
        };

        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
        MessageService.Default.Send(testMessage);
        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);

        MessageService.Default.Unregister<ITestMessage>(this);
        MessageService.Default.Register<TestMessageBase>(this, true, ReceiveITestMessage);

        MessageService.Default.Send(testMessage);
        Assert.IsTrue(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsTrue(_messageWasReceivedInITestMessage);
    }

    [TestMethod]
    public void TestConstrainingMessageByBaseBaseClass()
    {
        Reset();
        MessageService.Reset();
        MessageService.Default.Register<BaseMessage>(this, ReceiveMessageBase);

        TestMessageImpl testMessage = new(this)
        {
            Content = TestContent
        };

        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
        MessageService.Default.Send(testMessage);
        Assert.IsFalse(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsFalse(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);

        MessageService.Default.Unregister<ITestMessage>(this);
        MessageService.Default.Register<BaseMessage>(this, true, ReceiveMessageBase);

        MessageService.Default.Send(testMessage);
        Assert.IsTrue(_messageWasReceived);
        Assert.IsFalse(_messageWasReceivedInTestMessageBase);
        Assert.IsTrue(_messageWasReceivedInMessageBase);
        Assert.IsFalse(_messageWasReceivedInITestMessage);
    }

    public void ReceiveITestMessage(ITestMessage testMessage)
    {
        Assert.IsNotNull(testMessage);
        Assert.AreEqual(TestContent, testMessage.Content);
        _messageWasReceived = true;
        _messageWasReceivedInITestMessage = true;
    }

    public void ReceiveTestMessageBase(TestMessageBase testMessage)
    {
        Assert.IsNotNull(testMessage);
        Assert.AreEqual(TestContent, testMessage.Content);
        _messageWasReceived = true;
        _messageWasReceivedInTestMessageBase = true;
    }

    public void ReceiveMessageBase(BaseMessage testMessage)
    {
        Assert.IsNotNull(testMessage);

        var castedMessage = testMessage as ITestMessage;

        if (castedMessage is not null)
        {
            Assert.AreEqual(TestContent, castedMessage.Content);
            _messageWasReceived = true;
            _messageWasReceivedInMessageBase = true;
        }
    }
}
