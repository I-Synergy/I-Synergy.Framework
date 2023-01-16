using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ISynergy.Framework.Core.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Core.Messaging.Tests
{
    [TestClass]
    public class PropertyChangedMessageTest
    {
        [TestMethod]
        public void TestPropertyChangedMessageBaseFromViewModelBase()
        {
            var previousDateTime = DateTime.Now - TimeSpan.FromDays(1);
            var currentDateTime = DateTime.Now + TimeSpan.FromDays(1);
            const Exception PreviousException = null;
            var currentException = new InvalidOperationException();

            var receivedPreviousDateTime = DateTime.MinValue;
            var receivedCurrentDateTime = DateTime.MinValue;
            Exception receivedPreviousException = null;
            Exception receivedCurrentException = null;

            object receivedSender = null;
            object receivedTarget = null;

            var messageWasReceived = false;

            var testViewModel = new TestViewModel(previousDateTime, (InvalidOperationException)PreviousException);

            WeakReferenceMessenger.Default.Reset();

            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<InvalidOperationException>>(
                this,
                (s, e) =>
                {
                    receivedSender = e.Sender;
                    messageWasReceived = true;

                    var exceptionMessage = e;

                    if (exceptionMessage is not null && exceptionMessage.PropertyName == nameof(TestViewModel.MyException))
                    {
                        receivedPreviousException =
                            exceptionMessage.OldValue;
                        receivedCurrentException =
                            exceptionMessage.NewValue;
                        return;
                    }
                });

            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<DateTime>>(
                this,
                (s, e) =>
                {
                    receivedSender = e.Sender;
                    messageWasReceived = true;

                    var dateMessage = e;

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
            var previousDateTime = DateTime.Now - TimeSpan.FromDays(1);
            var currentDateTime = DateTime.Now + TimeSpan.FromDays(1);
            const Exception PreviousException = null;
            var currentException = new InvalidOperationException();

            var receivedPreviousDateTime = DateTime.MinValue;
            var receivedCurrentDateTime = DateTime.MinValue;
            Exception receivedPreviousException = null;
            Exception receivedCurrentException = null;

            object receivedSender = null;

            var messageWasReceived = false;

            var testViewModel = new TestViewModel(previousDateTime, (InvalidOperationException)PreviousException);

            WeakReferenceMessenger.Default.Reset();

            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<DateTime>>(this,
                                                                         (s, e) =>
                                                                         {
                                                                             receivedSender = e.Sender;
                                                                             messageWasReceived = true;

                                                                             if (e.PropertyName == nameof(TestViewModel.MyDate))
                                                                             {
                                                                                 receivedPreviousDateTime = e.OldValue;
                                                                                 receivedCurrentDateTime = e.NewValue;
                                                                             }
                                                                         });

            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<InvalidOperationException>>(this,
                (s, e) =>
                {
                    receivedSender = e.Sender;
                    messageWasReceived = true;

                    if (e.PropertyName == nameof(TestViewModel.MyException))
                    {
                        receivedPreviousException = e.OldValue;
                        receivedCurrentException = e.NewValue;
                    }
                });

            Assert.AreEqual(DateTime.MinValue, receivedPreviousDateTime);
            Assert.AreEqual(DateTime.MinValue, receivedCurrentDateTime);
            Assert.AreEqual(null, receivedPreviousException);
            Assert.AreEqual(null, receivedCurrentException);

            testViewModel.MyDate = currentDateTime;

            Assert.IsTrue(messageWasReceived);
            Assert.AreEqual(testViewModel, receivedSender);
            Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
            Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
            Assert.AreEqual(null, receivedPreviousException);
            Assert.AreEqual(null, receivedCurrentException);

            receivedSender = null;
            messageWasReceived = false;

            testViewModel.MyException = currentException;

            Assert.IsTrue(messageWasReceived);
            Assert.AreEqual(testViewModel, receivedSender);
            Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
            Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
            Assert.AreEqual(PreviousException, receivedPreviousException);
            Assert.AreEqual(currentException, receivedCurrentException);

            receivedSender = null;
            messageWasReceived = false;

            testViewModel.AnotherDate = currentDateTime + TimeSpan.FromDays(3);

            Assert.IsTrue(messageWasReceived);
            Assert.AreEqual(testViewModel, receivedSender);
            Assert.AreEqual(previousDateTime, receivedPreviousDateTime);
            Assert.AreEqual(currentDateTime, receivedCurrentDateTime);
            Assert.AreEqual(PreviousException, receivedPreviousException);
            Assert.AreEqual(currentException, receivedCurrentException);
        }

        [TestMethod]
        public void TestPropertyChangedMessageWithSender()
        {
            ExecuteTest(this);
        }

        // Helpers

        private void ExecuteTest(object sender)
        {
            const string PropertyName1 = "MyProperty1";
            const string PropertyName2 = "MyProperty2";
            const string TestNewContent1 = "abcd";
            const string TestNewContent2 = "efgh";
            const string TestOldContent1 = "ijkl";
            const string TestOldContent2 = "mnop";

            string receivedNewContent1 = null;
            string receivedNewContent2 = null;
            string receivedOldContent1 = null;
            string receivedOldContent2 = null;

            object receivedSender = null;

            WeakReferenceMessenger.Default.Reset();

            WeakReferenceMessenger.Default.Register<PropertyChangedMessage<string>>(this,
                                                                       (s,e )=>
                                                                       {
                                                                           receivedSender = e.Sender;

                                                                           if (e.PropertyName == PropertyName1)
                                                                           {
                                                                               receivedNewContent1 = e.NewValue;
                                                                               receivedOldContent1 = e.OldValue;
                                                                           }

                                                                           if (e.PropertyName == PropertyName2)
                                                                           {
                                                                               receivedNewContent2 = e.NewValue;
                                                                               receivedOldContent2 = e.OldValue;
                                                                           }
                                                                       });

            Assert.AreEqual(null, receivedNewContent1);
            Assert.AreEqual(null, receivedOldContent1);
            Assert.AreEqual(null, receivedNewContent2);
            Assert.AreEqual(null, receivedOldContent2);

            var propertyMessage1 = new PropertyChangedMessage<string>(this, PropertyName1, TestOldContent1, TestNewContent1); ;
            var propertyMessage2 = new PropertyChangedMessage<string>(this, PropertyName2, TestOldContent2, TestNewContent2);
            
            WeakReferenceMessenger.Default.Send(propertyMessage1);

            Assert.AreEqual(sender, receivedSender);
            Assert.AreEqual(TestOldContent1, receivedOldContent1);
            Assert.AreEqual(TestNewContent1, receivedNewContent1);
            Assert.AreEqual(null, receivedOldContent2);
            Assert.AreEqual(null, receivedNewContent2);

            receivedSender = null;

            WeakReferenceMessenger.Default.Send(propertyMessage2);

            Assert.AreEqual(sender, receivedSender);
            Assert.AreEqual(TestOldContent1, receivedOldContent1);
            Assert.AreEqual(TestNewContent1, receivedNewContent1);
            Assert.AreEqual(TestOldContent2, receivedOldContent2);
            Assert.AreEqual(TestNewContent2, receivedNewContent2);
        }

        public class TestViewModel : ObservableClass
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
                set { SetValue(value, true); }
            }

            /// <summary>
            /// Gets the MyDate property.
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public DateTime MyDate
            {
                get { return GetValue<DateTime>(); }
                set { SetValue(value, true); }
            }

            /// <summary>
            /// Gets or sets the MyException property value.
            /// </summary>
            public InvalidOperationException MyException
            {
                get { return GetValue<InvalidOperationException>(); }
                set { SetValue(value, true); }
            }
        }
    }
}
