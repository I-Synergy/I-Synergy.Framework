using System;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static int Age(this DateTime self)
        {
            var result = DateTime.Now.Year - self.Year;

            if (DateTime.Now.Month < self.Month || (DateTime.Now.Month == self.Month && DateTime.Now.Day < self.Day))
                result--;

            return result;
        }

        public static int AgeInDays(this DateTime self)
        {
            var result = (DateTime.Now - self).TotalDays;
            return Convert.ToInt32(Math.Floor(result));
        }

        public static string ToJsonString(this DateTime self)
        {
            var result = self.ToUniversalTime().ToString(GenericConstants.DateTimeOffsetFormat);
            return result;
        }

        public static DateTime ToStartOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, 0);
        }

        public static DateTime ToEndOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, 0).AddDays(1).AddTicks(-1);
        }

        public static DateTime ToStartOfMonth(this DateTime self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, 0);
        }

        public static DateTime ToEndOfMonth(this DateTime self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, 0).AddMonths(1).AddTicks(-1);
        }

        public static DateTime ToStartOfYear(this int year)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, 0);
        }

        public static DateTime ToEndOfYear(this int year)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, 0).AddYears(1).AddTicks(-1);
        }

        public static DateTime ToStartOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = (7 + (self.DayOfWeek - startOfWeek)) % 7;
            return self.AddDays(-1 * diff).Date;
        }

        public static DateTime ToEndOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return self.ToStartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
        }

        public static DateTime ToStartOfWorkWeek(this DateTime self)
        {
            var diff = (7 + (self.DayOfWeek - DayOfWeek.Monday)) % 7;
            return self.AddDays(-1 * diff).Date;
        }

        public static DateTime ToEndOfWorkWeek(this DateTime self)
        {
            return self.ToStartOfWeek(DayOfWeek.Monday).AddDays(5).AddTicks(-1);
        }

        public static DateTime First(this DateTime current)
        {
            return current.AddDays(1 - current.Day);
        }

        public static DateTime FirstOfYear(this DateTime current)
        {
            return new DateTime(current.Year, 1, 1);
        }

        public static DateTime Last(this DateTime current)
        {
            var daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);
            return current.First().AddDays(daysInMonth - 1);
        }

        public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek)
        {
            var last = current.Last();
            var diff = dayOfWeek - last.DayOfWeek;

            if (diff > 0)
                diff -= 7;

            return last.AddDays(diff);
        }

        public static DateTime ThisOrNext(this DateTime current, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays < 0)
                offsetDays += 7;

            return current.AddDays(offsetDays);
        }

        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays <= 0)
                offsetDays += 7;

            return current.AddDays(offsetDays);
        }

        public static DateTime NextNWeekday(this DateTime current, int toAdvance)
        {
            while (toAdvance >= 1)
            {
                toAdvance--;
                current = current.AddDays(1);

                while (!current.IsWeekday())
                    current = current.AddDays(1);
            }
            return current;
        }

        public static bool IsWeekday(this DateTime current)
        {
            switch (current.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return false;
                default:
                    return true;
            }
        }

        public static DateTime ClearMinutesAndSeconds(this DateTime current)
        {
            return current.AddMinutes(-1 * current.Minute)
                .AddSeconds(-1 * current.Second)
                .AddMilliseconds(-1 * current.Millisecond);
        }

        public static DateTime ToWeek(this DateTime current, Week week)
        {
            switch (week)
            {
                case Week.Second:
                    return current.First().AddDays(7);
                case Week.Third:
                    return current.First().AddDays(14);
                case Week.Fourth:
                    return current.First().AddDays(21);
                case Week.Last:
                    return current.Last().AddDays(-7);
            }
            return current.First();
        }
    }
}
