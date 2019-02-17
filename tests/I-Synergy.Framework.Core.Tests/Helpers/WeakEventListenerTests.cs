using ISynergy.Framework.Tests.Base;
using ISynergy.Helpers;
using System;
using Xunit;

namespace ISynergy.Helpers.Tests
{
    [Collection("WeakEventListener")]
    public class WeakEventListenerTests : IntegrationTest
    {
        public class SampleClass
        {
            public event EventHandler<EventArgs> Raisevent;

            public void DoSomething()
            {
                OnRaiseEvent();
            }

            protected virtual void OnRaiseEvent()
            {
                Raisevent?.Invoke(this, EventArgs.Empty);
            }
        }

        [Fact]
        public void WeakEventListenerEventTest()
        {
            bool isOnEventTriggered = false;
            bool isOnDetachTriggered = false;

            SampleClass sample = new SampleClass();

            WeakEventListener<SampleClass, object, EventArgs> weak = new WeakEventListener<SampleClass, object, EventArgs>(sample)
            {
                OnEventAction = (instance, source, eventArgs) => { isOnEventTriggered = true; },
                OnDetachAction = (listener) => { isOnDetachTriggered = true; }
            };

            sample.Raisevent += weak.OnEvent;

            sample.DoSomething();
            Assert.True(isOnEventTriggered);

            weak.Detach();
            Assert.True(isOnDetachTriggered);
        }
    }
}
