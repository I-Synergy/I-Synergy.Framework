using System;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        public void IsInRangeOfDateSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        [Fact]
        public void IsInRangeOfDateDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        [Fact]
        public void IsInRangeOfDatesSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var end = new DateTimeOffset(2017, 10, 11, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        [Fact]
        public void IsInRangeOfDatesDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));
            var end = new DateTimeOffset(2017, 10, 11, 21, 59, 59, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        [Fact]
        public void ToStartOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay();
            var test = new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime();
            Assert.Equal(test, result);
        }

        [Fact]
        public void ToEndOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay();
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        [Fact]
        public void ToStartOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth();
            Assert.Equal(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToEndOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth();
            Assert.Equal(new DateTimeOffset(1975, 10, 31, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        [Fact]
        public void ToStartOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToStartOfYear();
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToEndOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToEndOfYear();
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddYears(1).AddTicks(-1).ToUniversalTime(), result);
        }

        [Fact]
        public void ToStartOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTimeOffset(1975, 10, 5, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        [Fact]
        public void ToStartOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTimeOffset(1975, 9, 28, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTimeOffset(1975, 10, 4, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        [Fact]
        public void ToStartOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 4, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWorkWeek();
            Assert.Equal(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        [Fact]
        public void ToEndOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWorkWeek();
            Assert.Equal(new DateTimeOffset(1975, 10, 3, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }
    }
}
