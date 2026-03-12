using ISynergy.Framework.Core.Data.TestClasses;

namespace ISynergy.Framework.Core.Events;

[TestClass]
public class WeakEventsTests
{
    [TestMethod]
    public void ShouldHandleEventWhenBothReferencesAreAlive()
    {
        var alarm = new Alarm();
        var sleepy = new Sleepy(alarm);
        alarm.Beep();
        alarm.Beep();

        Assert.AreEqual(2, sleepy.SnoozeCount);
    }

    [TestMethod]
    public void ShouldAllowSubscriberReferenceToBeCollected()
    {
        var alarm = new Alarm();
        var sleepyReference = null as WeakReference;
        new Action(() =>
        {
            // Run this in a delegate to that the local variable gets garbage collected
            using var sleepy = new Sleepy(alarm);
            alarm.Beep();
            alarm.Beep();
            Assert.AreEqual(2, sleepy.SnoozeCount);
            sleepyReference = new WeakReference(sleepy);
        })();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.IsNull(sleepyReference!.Target);
    }

    [TestMethod]
    public void SubscriberShouldNotBeUnsubscribedUntilCollection()
    {
        var alarm = new Alarm();
        var sleepy = new Sleepy(alarm);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        alarm.Beep();
        alarm.Beep();
        Assert.AreEqual(2, sleepy.SnoozeCount);
    }

    // Counter used by the static handler test below
    private static int s_staticHandlerCallCount;

    private static void StaticEventHandler(object? sender, EventArgs e)
    {
        s_staticHandlerCallCount++;
    }

    [TestMethod]
    public void StaticHandlerShouldBeInvokedCorrectly()
    {
        s_staticHandlerCallCount = 0;
        var source = new WeakEventSource<EventArgs>();
        source.Subscribe(StaticEventHandler);

        source.Raise(this, EventArgs.Empty);
        source.Raise(this, EventArgs.Empty);

        Assert.AreEqual(2, s_staticHandlerCallCount);
    }

    [TestMethod]
    public void StaticHandlerShouldBeUnsubscribable()
    {
        s_staticHandlerCallCount = 0;
        var source = new WeakEventSource<EventArgs>();
        source.Subscribe(StaticEventHandler);
        source.Raise(this, EventArgs.Empty);

        source.Unsubscribe(StaticEventHandler);
        source.Raise(this, EventArgs.Empty);

        Assert.AreEqual(1, s_staticHandlerCallCount);
    }
}
