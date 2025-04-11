using ISynergy.Framework.Core.Tests.Data.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Events.Tests;

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
}
