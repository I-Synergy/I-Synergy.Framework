using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using System;
using System.Collections.Concurrent;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support
{
    public class TestSystemClock : ISystemClock
    {
        private readonly ConcurrentDictionary<DefaultLockTimeRoundingMode, ILockTimeRounding> _roundingForMode = new ConcurrentDictionary<DefaultLockTimeRoundingMode, ILockTimeRounding>();

        private TimeSpan _diff;

        public DateTime UtcNow => DateTime.UtcNow + _diff;

        public void Set(DateTime dt)
        {
            var current = DateTime.UtcNow;
            _diff = dt - current;
        }

        public void RoundTo(DefaultLockTimeRoundingMode roundingMode)
        {
            var now = DateTime.UtcNow;
            var rounded = GetRoundedDate(now, roundingMode);
            _diff = rounded - now;
        }

        private DateTime GetRoundedDate(DateTime dt, DefaultLockTimeRoundingMode roundingMode)
        {
            var rounding = _roundingForMode.GetOrAdd(roundingMode, mode => new DefaultLockTimeRounding(mode));
            return rounding.Round(dt);
        }
    }
}
