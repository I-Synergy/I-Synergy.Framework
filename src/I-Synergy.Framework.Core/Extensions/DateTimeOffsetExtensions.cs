using ISynergy.Framework.Core.Constants;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToUniversalTimeString(this DateTimeOffset self) =>
            self.ToUniversalTime().ToString(GenericConstants.DateTimeOffsetFormat);

        public static bool IsInRangeOfDate(this DateTimeOffset self, DateTimeOffset comparer)
        {
            var start = comparer.ToOffset(self.Offset);
            var end = start.AddDays(1).AddTicks(-1);

            if (self.CompareTo(start) >= 0 && self.CompareTo(end) <= 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsInRangeOfDate(this DateTimeOffset self, DateTimeOffset comparer_start, DateTimeOffset comparer_end)
        {
            var start = comparer_start.ToOffset(self.Offset);
            var end = comparer_end.ToOffset(self.Offset);

            if (self.CompareTo(start) >= 0 && self.CompareTo(end) <= 0)
            {
                return true;
            }

            return false;
        }

        public static int ComparedToDateTime(this DateTimeOffset self, DateTimeOffset comparer)
        {
            var source = self.ToOffset(comparer.Offset);
            var target = comparer;

            var result = DateTimeOffset.Compare(source, target);

            return result;
        }

        public static DateTimeOffset ToStartOfDay(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, 0, self.Offset);
        }

        public static DateTimeOffset ToEndOfDay(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, 0, self.Offset).AddDays(1).AddTicks(-1);
        }

        public static DateTimeOffset ToStartOfMonth(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 0, 0, 0, 0, self.Offset);
        }

        public static DateTimeOffset ToEndOfMonth(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 0, 0, 0, 0, self.Offset).AddMonths(1).AddTicks(-1);
        }

        public static DateTimeOffset ToStartOfWeek(this DateTimeOffset self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = (7 + (self.DayOfWeek - startOfWeek)) % 7;
            return self.AddDays(-1 * diff).ToStartOfDay();
        }

        public static DateTimeOffset ToEndOfWeek(this DateTimeOffset self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return self.ToStartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
        }

        public static DateTimeOffset ToStartOfWorkWeek(this DateTimeOffset self)
        {
            var diff = (7 + (self.DayOfWeek - DayOfWeek.Monday)) % 7;
            return self.AddDays(-1 * diff).ToStartOfDay();
        }

        public static DateTimeOffset ToEndOfWorkWeek(this DateTimeOffset self)
        {
            return self.ToStartOfWeek(DayOfWeek.Monday).AddDays(5).AddTicks(-1);
        }

        public static string GetPossibleTimeZones(this DateTimeOffset self, string seperator = ", ")
        {
            var offset = self.Offset;
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var result = new List<string>();

            foreach (var timeZone in timeZones)
            {
                if (timeZone.GetUtcOffset(self.DateTime).Equals(offset))
                    result.Add(timeZone.DisplayName);
            }

            return string.Join(seperator, result);
        }
    }
}
