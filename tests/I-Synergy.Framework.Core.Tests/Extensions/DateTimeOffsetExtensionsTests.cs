using ISynergy.Library;
using System;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void IsInRangeOfDateSameOffsetTest()
        {
            DateTimeOffset self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            DateTimeOffset comparer = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void IsInRangeOfDateDifferentOffsetTest()
        {
            DateTimeOffset self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            DateTimeOffset comparer = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void IsInRangeOfDatesSameOffsetTest()
        {
            DateTimeOffset self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            DateTimeOffset start = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);
            DateTimeOffset end = new DateTimeOffset(2017, 10, 11, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void IsInRangeOfDatesDifferentOffsetTest()
        {
            DateTimeOffset self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            DateTimeOffset start = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));
            DateTimeOffset end = new DateTimeOffset(2017, 10, 11, 21, 59, 59, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToStartOfDayLocalTest()
        {
            DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToEndOfDayLocalTest()
        {
            DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToStartOfMonthLocalTest()
        {
            DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToEndOfMonthLocalTest()
        {
            DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth(TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 10, 31, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToStartOfYearLocalTest()
        {
            DateTimeOffset result = DateTimeOffset.Now.ToStartOfYear(1975, TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        [Fact]
        [Trait(nameof(DateTimeOffsetExtensions), Test.Unit)]
        public void ToEndOfYearLocalTest()
        {
            DateTimeOffset result = DateTimeOffset.Now.ToEndOfYear(1975, TimeZoneInfo.Local);
            Assert.Equal(new DateTimeOffset(1975, 12, 31, 23, 59, 59, 999, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }
    }
}