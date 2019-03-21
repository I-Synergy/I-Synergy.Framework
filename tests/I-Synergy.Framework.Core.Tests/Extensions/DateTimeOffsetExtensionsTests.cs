using ISynergy.Framework.Tests.Base;
using System;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class DateTimeOffsetExtensionsTests : UnitTest
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
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToEndOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToStartOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToEndOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 31, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToStartOfYearLocalTest()
        {
            var result = DateTimeOffset.Now.ToStartOfYear(1975, TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        public void ToEndOfYearLocalTest()
        {
            var result = DateTimeOffset.Now.ToEndOfYear(1975, TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 12, 31, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }
    }
}