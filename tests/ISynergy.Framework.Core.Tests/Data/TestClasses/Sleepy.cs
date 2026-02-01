using System.ComponentModel;

namespace ISynergy.Framework.Core.Tests.Data.TestClasses;

public class Sleepy : IDisposable
{
    private readonly Alarm _alarm;
    private int _snoozeCount;

    public Sleepy(Alarm alarm)
    {
        _alarm = alarm;
        _alarm.Beeped += Alarm_Beeped;
    }

    private void Alarm_Beeped(object? sender, PropertyChangedEventArgs e)
    {
        _snoozeCount++;
    }

    public int SnoozeCount
    {
        get { return _snoozeCount; }
    }

    public void Dispose()
    {
        if (_alarm is not null)
            _alarm.Beeped -= Alarm_Beeped;
    }
}
