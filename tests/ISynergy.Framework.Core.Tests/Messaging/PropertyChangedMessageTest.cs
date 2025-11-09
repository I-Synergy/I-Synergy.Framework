using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Messages.Base;
using ISynergy.Framework.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Messaging.Tests;

[TestClass]
public class PropertyChangedMessageTest
{
    [TestMethod]
    public void TestPropertyChangedMessage()
    {
        ExecuteTest(null, null!);
    }

    [TestMethod]
    public void TestPropertyChangedMessageBaseFromViewModelBase()
    {
        DateTime previousDateTime = DateTime.Now - TimeSpan.FromDays(1);
        DateTime currentDateTime = DateTime.Now + TimeSpan.FromDays(1);
        const Exception? PreviousException = null;
        InvalidOperationException currentException = new();

        DateTime receivedPreviousDateTime = DateTime.MinValue;
        DateTime receivedCurrentDateTime = DateTime.MinValue;
        Exception? receivedPreviousException = null;
        Exception? receivedCurrentException = null;

        object? receivedSender = null;
        object? receivedTarget = null;

        bool messageWasReceived = false;

        TestViewModel testViewModel = new(previousDateTime, (InvalidOperationException)PreviousException!);

        var messenger = new MessengerService();

        messenger.Register<BasePropertyChangedMessage>(
            this,
            true,
            m =>
            {
                receivedSender = m.Sender;
                receivedTarget = m.Target;
                messageWasReceived = true;

                var exceptionMessage =
                    m as PropertyChangedMessage<InvalidOperationException>;

                if (exceptionMessage is not null && exceptionMessage.PropertyName == nameof(TestViewModel.MyException))
                {
                    receivedPreviousException =
                        exceptionMessage.OldValue;
                    receivedCurrentException =
                        exceptionMessage.NewValue;
                    return;
                }

                var dateMessage = m as PropertyChangedMessage<DateTime>;

                if (dateMessage is not null && dateMessage.PropertyName == nameof(TestViewModel.MyDate))
                {
                    receivedPreviousDateTime =
                        dateMessage.OldValue;
                    receivedCurrentDateTime =
                        dateMessage.NewValue;
                    return;
                }
            });

        Assert.AreEqual(DateTime.MinValue, receivedPreviousDateTime);
        Assert.AreEqual(DateTime.MinValue, receivedCurrentDateTime);
        Assert.AreEqual(null, receivedPreviousException);
        Assert.AreEqual(null, receivedCurrentException);

        testViewModel.MyDate = currentDateTime;

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(null, receivedPreviousException);
        Assert.AreEqual(null, receivedCurrentException);

        receivedSender = null;
        receivedTarget = null;
        messageWasReceived = false;

        testViewModel.MyException = currentException;

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(PreviousException, receivedPreviousException);
        Assert.AreEqual(currentException, receivedCurrentException);

        receivedSender = null;
        receivedTarget = null;
        messageWasReceived = false;

        testViewModel.AnotherDate = currentDateTime + TimeSpan.FromDays(3);

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(PreviousException, receivedPreviousException);
        Assert.AreEqual(currentException, receivedCurrentException);
    }

    [TestMethod]
    public void TestPropertyChangedMessageFomViewModelBase()
    {
        DateTime previousDateTime = DateTime.Now - TimeSpan.FromDays(1);
        DateTime currentDateTime = DateTime.Now + TimeSpan.FromDays(1);
        const Exception? PreviousException = null;
        InvalidOperationException currentException = new();

        DateTime receivedPreviousDateTime = DateTime.MinValue;
        DateTime receivedCurrentDateTime = DateTime.MinValue;
        Exception? receivedPreviousException = null;
        Exception? receivedCurrentException = null;

        object? receivedSender = null;
        object? receivedTarget = null;

        bool messageWasReceived = false;

        TestViewModel testViewModel = new(previousDateTime, (InvalidOperationException)PreviousException!);

        var messenger = new MessengerService();

        messenger.Register<PropertyChangedMessage<DateTime>>(
            this,
            m =>
            {
                receivedSender = m.Sender;
                receivedTarget = m.Target;
                messageWasReceived = true;

                if (m.PropertyName == nameof(TestViewModel.MyDate))
                {
                    receivedPreviousDateTime = m.OldValue;
                    receivedCurrentDateTime = m.NewValue;
                    return;
                }
            });

        messenger.Register<PropertyChangedMessage<InvalidOperationException>>(
            this,
            m =>
            {
                receivedSender = m.Sender;
                receivedTarget = m.Target;
                messageWasReceived = true;

                if (m.PropertyName == nameof(TestViewModel.MyException))
                {
                    receivedPreviousException = m.OldValue;
                    receivedCurrentException = m.NewValue;
                    return;
                }
            });

        Assert.AreEqual(DateTime.MinValue, receivedPreviousDateTime);
        Assert.AreEqual(DateTime.MinValue, receivedCurrentDateTime);
        Assert.AreEqual(null, receivedPreviousException);
        Assert.AreEqual(null, receivedCurrentException);

        testViewModel.MyDate = currentDateTime;

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(null, receivedPreviousException);
        Assert.AreEqual(null, receivedCurrentException);

        receivedSender = null;
        receivedTarget = null;
        messageWasReceived = false;

        testViewModel.MyException = currentException;

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(PreviousException, receivedPreviousException);
        Assert.AreEqual(currentException, receivedCurrentException);

        receivedSender = null;
        receivedTarget = null;
        messageWasReceived = false;

        testViewModel.AnotherDate = currentDateTime + TimeSpan.FromDays(3);

        Assert.IsTrue(messageWasReceived);
        Assert.AreEqual(testViewModel, receivedSender);
        Assert.AreEqual(null, receivedTarget);
        Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
        Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
        Assert.AreEqual(PreviousException, receivedPreviousException);
        Assert.AreEqual(currentException, receivedCurrentException);
    }

    [TestMethod]
    public void TestPropertyChangedMessageWithSender()
    {
        ExecuteTest(this, null!);
    }

    [TestMethod]
    public void TestPropertyChangedMessageWithSenderAndTarget()
    {
        ExecuteTest(this, this);
    }

    // Helpers

    private void ExecuteTest(object? sender, object target)
    {
        const string PropertyName1 = "MyProperty1";
        const string PropertyName2 = "MyProperty2";
        const string TestNewContent1 = "abcd";
        const string TestNewContent2 = "efgh";
        const string TestOldContent1 = "ijkl";
        const string TestOldContent2 = "mnop";

        string? receivedNewContent1 = null;
        string? receivedNewContent2 = null;
        string? receivedOldContent1 = null;
        string? receivedOldContent2 = null;

        object? receivedSender = null;
        object? receivedTarget = null;

        var messenger = new MessengerService();

        messenger.Register<PropertyChangedMessage<string>>(this,
                                                                   m =>
                                                                   {
                                                                       receivedSender = m.Sender;
                                                                       receivedTarget = m.Target;

                                                                       if (m.PropertyName == PropertyName1)
                                                                       {
                                                                           receivedNewContent1 = m.NewValue;
                                                                           receivedOldContent1 = m.OldValue;
                                                                           return;
                                                                       }

                                                                       if (m.PropertyName == PropertyName2)
                                                                       {
                                                                           receivedNewContent2 = m.NewValue;
                                                                           receivedOldContent2 = m.OldValue;
                                                                           return;
                                                                       }
                                                                   });

        Assert.AreEqual(null, receivedNewContent1);
        Assert.AreEqual(null, receivedOldContent1);
        Assert.AreEqual(null, receivedNewContent2);
        Assert.AreEqual(null, receivedOldContent2);

        PropertyChangedMessage<string> propertyMessage1;
        PropertyChangedMessage<string> propertyMessage2;

        if (sender is null)
        {
            propertyMessage1 = new PropertyChangedMessage<string>(TestOldContent1, TestNewContent1, PropertyName1);
            propertyMessage2 = new PropertyChangedMessage<string>(TestOldContent2, TestNewContent2, PropertyName2);
        }
        else
        {
            if (target is null)
            {
                propertyMessage1 = new PropertyChangedMessage<string>(sender,
                                                                      TestOldContent1,
                                                                      TestNewContent1,
                                                                      PropertyName1);
                propertyMessage2 = new PropertyChangedMessage<string>(sender,
                                                                      TestOldContent2,
                                                                      TestNewContent2,
                                                                      PropertyName2);
            }
            else
            {
                propertyMessage1 = new PropertyChangedMessage<string>(sender,
                                                                      target,
                                                                      TestOldContent1,
                                                                      TestNewContent1,
                                                                      PropertyName1);
                propertyMessage2 = new PropertyChangedMessage<string>(sender,
                                                                      target,
                                                                      TestOldContent2,
                                                                      TestNewContent2,
                                                                      PropertyName2);
            }
        }

        messenger.Send(propertyMessage1);

        Assert.AreEqual(sender, receivedSender);
        Assert.AreEqual(target, receivedTarget);
        Assert.AreEqual(TestOldContent1, receivedOldContent1);
        Assert.AreEqual(TestNewContent1, receivedNewContent1);
        Assert.AreEqual(null, receivedOldContent2);
        Assert.AreEqual(null, receivedNewContent2);

        receivedTarget = null;
        receivedSender = null;

        messenger.Send(propertyMessage2);

        Assert.AreEqual(sender, receivedSender);
        Assert.AreEqual(target, receivedTarget);
        Assert.AreEqual(TestOldContent1, receivedOldContent1);
        Assert.AreEqual(TestNewContent1, receivedNewContent1);
        Assert.AreEqual(TestOldContent2, receivedOldContent2);
        Assert.AreEqual(TestNewContent2, receivedNewContent2);
    }

    public class TestViewModel : ObservableValidatedClass
    {
        public TestViewModel(DateTime initialValueDateTime, InvalidOperationException initialValueException)
        {
            MyDate = initialValueDateTime;
            MyException = initialValueException;
        }

        /// <summary>
        /// Gets the AnotherDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime AnotherDate
        {
            get { return GetValue<DateTime>(); }
            set
            {
                DateTime oldValue = GetValue<DateTime>();
                SetValue(value);
                PropertyChangedMessage<DateTime> message = new PropertyChangedMessage<DateTime>(this, oldValue, value, nameof(AnotherDate));
                messenger.Send(message);
            }
        }

        /// <summary>
        /// Gets the MyDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime MyDate
        {
            get { return GetValue<DateTime>(); }
            set
            {
                DateTime oldValue = GetValue<DateTime>();
                SetValue(value);
                PropertyChangedMessage<DateTime> message = new PropertyChangedMessage<DateTime>(this, oldValue, value, nameof(MyDate));
                messenger.Send(message);
            }
        }

        /// <summary>
        /// Gets or sets the MyException property value.
        /// </summary>
        public InvalidOperationException MyException
        {
            get { return GetValue<InvalidOperationException>(); }
            set
            {
                InvalidOperationException oldValue = GetValue<InvalidOperationException>();
                SetValue(value);
                PropertyChangedMessage<InvalidOperationException> message = new PropertyChangedMessage<InvalidOperationException>(this, oldValue, value, nameof(MyException));
                messenger.Send(message);
            }
        }
    }
}
