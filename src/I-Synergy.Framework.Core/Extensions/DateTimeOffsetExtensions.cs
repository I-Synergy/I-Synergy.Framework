using ISynergy;

namespace System
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToUniversalTimeString(this DateTimeOffset self) =>
            self.ToUniversalTime().ToString(Constants.DateTimeOffsetFormat);

        public static bool IsInRangeOfDate(this DateTimeOffset self, DateTimeOffset comparer)
        {
            var start = comparer.ToOffset(self.Offset);
            var end = start.AddDays(1).AddMilliseconds(-1);

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

        public static DateTimeOffset ToStartOfDay(this DateTimeOffset self, TimeZoneInfo timezone)
        {
            if(self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, 0, timezone.BaseUtcOffset);
        }

        public static DateTimeOffset ToEndOfDay(this DateTimeOffset self, TimeZoneInfo timezone)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 23, 59, 59, 999, timezone.BaseUtcOffset);
        }

        public static DateTimeOffset ToStartOfMonth(this DateTimeOffset self, TimeZoneInfo timezone)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 0, 0, 0, 0, timezone.BaseUtcOffset);
        }

        public static DateTimeOffset ToEndOfMonth(this DateTimeOffset self, TimeZoneInfo timezone)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 23, 59, 59, 999, timezone.BaseUtcOffset).AddMonths(1).AddDays(-1);
        }

        public static DateTimeOffset ToStartOfYear(this DateTimeOffset self, int year, TimeZoneInfo timezone)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(year, 1, 1, 0, 0, 0, 0, timezone.BaseUtcOffset);
        }

        public static DateTimeOffset ToEndOfYear(this DateTimeOffset self, int year, TimeZoneInfo timezone)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(year, 12, 31, 23, 59, 59, 999, timezone.BaseUtcOffset);
        }
    }
}