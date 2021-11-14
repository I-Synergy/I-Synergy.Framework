namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class DateTimeExtensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Ages the specified self.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.Int32.</returns>
        public static int Age(this DateTime self)
        {
            var result = DateTime.Now.Year - self.Year;

            if (DateTime.Now.Month < self.Month || (DateTime.Now.Month == self.Month && DateTime.Now.Day < self.Day))
                result--;

            return result;
        }

        /// <summary>
        /// Ages the in days.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.Int32.</returns>
        public static int AgeInDays(this DateTime self)
        {
            var result = (DateTime.Now - self).TotalDays;
            return Convert.ToInt32(Math.Floor(result));
        }

        /// <summary>
        /// Converts to jsonstring.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string ToJsonString(this DateTime self)
        {
            var result = self.ToUniversalTime().ToString(GenericConstants.DateTimeOffsetFormat);
            return result;
        }

        /// <summary>
        /// Converts to startofday.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToStartOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Converts to endofday.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToEndOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, 0).AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofmonth.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToStartOfMonth(this DateTime self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// Converts to endofmonth.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToEndOfMonth(this DateTime self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, 0).AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofyear.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToStartOfYear(this int year)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// Converts to endofyear.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToEndOfYear(this int year)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, 0).AddYears(1).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="startOfWeek">The start of week.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToStartOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = (7 + (self.DayOfWeek - startOfWeek)) % 7;
            return self.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Converts to endofweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="startOfWeek">The start of week.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToEndOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return self.ToStartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofworkweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToStartOfWorkWeek(this DateTime self)
        {
            var diff = (7 + (self.DayOfWeek - DayOfWeek.Monday)) % 7;
            return self.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Converts to endofworkweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToEndOfWorkWeek(this DateTime self)
        {
            return self.ToStartOfWeek(DayOfWeek.Monday).AddDays(5).AddTicks(-1);
        }

        /// <summary>
        /// Firsts the specified current.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>DateTime.</returns>
        public static DateTime First(this DateTime current)
        {
            return current.AddDays(1 - current.Day);
        }

        /// <summary>
        /// Firsts the of year.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FirstOfYear(this DateTime current)
        {
            return new DateTime(current.Year, 1, 1);
        }

        /// <summary>
        /// Lasts the specified current.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Last(this DateTime current)
        {
            var daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);
            return current.First().AddDays(daysInMonth - 1);
        }

        /// <summary>
        /// Lasts the specified day of week.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek)
        {
            var last = current.Last();
            var diff = dayOfWeek - last.DayOfWeek;

            if (diff > 0)
                diff -= 7;

            return last.AddDays(diff);
        }

        /// <summary>
        /// Thises the or next.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ThisOrNext(this DateTime current, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays < 0)
                offsetDays += 7;

            return current.AddDays(offsetDays);
        }

        /// <summary>
        /// Nexts the specified day of week.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek)
        {
            var offsetDays = dayOfWeek - current.DayOfWeek;

            if (offsetDays <= 0)
                offsetDays += 7;

            return current.AddDays(offsetDays);
        }

        /// <summary>
        /// Nexts the n weekday.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="toAdvance">To advance.</param>
        /// <returns>DateTime.</returns>
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

        /// <summary>
        /// Determines whether the specified current is weekday.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns><c>true</c> if the specified current is weekday; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Clears the minutes and seconds.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ClearMinutesAndSeconds(this DateTime current)
        {
            return current.AddMinutes(-1 * current.Minute)
                .AddSeconds(-1 * current.Second)
                .AddMilliseconds(-1 * current.Millisecond);
        }

        /// <summary>
        /// Converts to week.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="week">The week.</param>
        /// <returns>DateTime.</returns>
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
