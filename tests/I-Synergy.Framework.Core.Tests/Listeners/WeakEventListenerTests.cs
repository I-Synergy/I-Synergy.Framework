using System;
using Xunit;

namespace ISynergy.Framework.Core.Listeners.Tests
{
    public class WeakEventListenerTests
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
            var isOnEventTriggered = false;
            var isOnDetachTriggered = false;

            var sample = new SampleClass();

            var weak = new WeakEventListener<SampleClass, object, EventArgs>(sample)
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
