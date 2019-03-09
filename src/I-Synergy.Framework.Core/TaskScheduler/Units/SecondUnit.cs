using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks.Units
{
    public sealed class SecondUnit : ITimeRestrictableUnit
    {
        private readonly int _duration;

        internal SecondUnit(Schedule schedule, int duration)
        {
            _duration = duration;
            Schedule = schedule;
            Schedule.CalculateNextRun = x => x.AddSeconds(_duration);
        }

        internal Schedule Schedule { get; private set; }

        Schedule ITimeRestrictableUnit.Schedule { get { return this.Schedule; } }
    }
}
