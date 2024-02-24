using ISynergy.Framework.Core.Events;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Tests.Data.TestClasses;

public class Sleepy
{
    private readonly Alarm _alarm;
    private int _snoozeCount;

    public Sleepy(Alarm alarm)
    {
        _alarm = alarm;
        _alarm.Beeped += new WeakEventHandler<PropertyChangedEventArgs>(Alarm_Beeped).Handler;
    }

    private void Alarm_Beeped(object sender, PropertyChangedEventArgs e)
    {
        _snoozeCount++;
    }

    public int SnoozeCount
    {
        get { return _snoozeCount; }
    }
}
