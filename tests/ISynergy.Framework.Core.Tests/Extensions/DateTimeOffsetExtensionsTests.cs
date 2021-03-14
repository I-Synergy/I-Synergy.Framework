using System;
using System.Globalization;
using ISynergy.Framework.Core.Constants;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class DateTimeOffsetExtensionsTests.
    /// </summary>
    public class DateTimeOffsetExtensionsTests
    {
        /// <summary>
        /// Defines the test method IsInRangeOfDateSameOffsetTest.
        /// </summary>
        [Fact]
        public void IsInRangeOfDateSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDateDifferentOffsetTest.
        /// </summary>
        [Fact]
        public void IsInRangeOfDateDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(comparer));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDatesSameOffsetTest.
        /// </summary>
        [Fact]
        public void IsInRangeOfDatesSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var end = new DateTimeOffset(2017, 10, 11, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDatesDifferentOffsetTest.
        /// </summary>
        [Fact]
        public void IsInRangeOfDatesDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));
            var end = new DateTimeOffset(2017, 10, 11, 21, 59, 59, new TimeSpan(0, 0, 0));

            Assert.True(self.IsInRangeOfDate(start, end));
        }

        /// <summary>
        /// Defines the test method ToStartOfDayLocalTest.
        /// </summary>
        [Fact]
        public void ToStartOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay();
            var test = new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime();
            Assert.Equal(test, result);
        }

        /// <summary>
        /// Defines the test method ToEndOfDayLocalTest.
        /// </summary>
        [Fact]
        public void ToEndOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay();
            Assert.Equal(new DateTimeOffset(1975, 10, 29, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfMonthLocalTest.
        /// </summary>
        [Fact]
        public void ToStartOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth();
            Assert.Equal(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfMonthLocalTest.
        /// </summary>
        [Fact]
        public void ToEndOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth();
            Assert.Equal(new DateTimeOffset(1975, 10, 31, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfYearLocalTest.
        /// </summary>
        [Fact]
        public void ToStartOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToStartOfYear();
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfYearLocalTest.
        /// </summary>
        [Fact]
        public void ToEndOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToEndOfYear();
            Assert.Equal(new DateTimeOffset(1975, 1, 1, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddYears(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnMondayTest.
        /// </summary>
        [Fact]
        public void ToStartOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnMondayTest.
        /// </summary>
        [Fact]
        public void ToEndOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Monday);
            Assert.Equal(new DateTimeOffset(1975, 10, 5, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnSundayTest.
        /// </summary>
        [Fact]
        public void ToStartOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTimeOffset(1975, 9, 28, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnSundayTest.
        /// </summary>
        [Fact]
        public void ToEndOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Sunday);
            Assert.Equal(new DateTimeOffset(1975, 10, 4, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWorkWeekTest.
        /// </summary>
        [Fact]
        public void ToStartOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 4, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWorkWeek();
            Assert.Equal(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWorkWeekTest.
        /// </summary>
        [Fact]
        public void ToEndOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWorkWeek();
            Assert.Equal(new DateTimeOffset(1975, 10, 3, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToUniversalTimeStringTest.
        /// </summary>
        [Fact]
        public void ToUniversalTimeStringTest()
        {
            var timezoneWesternEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35,  timezoneWesternEurope.BaseUtcOffset).ToUniversalTimeString();
            var expected = new DateTimeOffset(1975, 10, 1, 13, 43, 35, TimeSpan.FromHours(0)).ToString(GenericConstants.DateTimeOffsetFormat); ; 
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Defines the test method ToLocalDateStringTest.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="culture">The culture.</param>
        [Theory]
        [InlineData("d", "6/15/2009", "en-US")]
        [InlineData("D", "Monday, June 15, 2009", "en-US")]
        [InlineData("f", "maandag 15 juni 2009 13:45", "nl-NL")]
        [InlineData("F", "Monday, June 15, 2009 1:45:30 PM", "en-US")]
        [InlineData("g", "6/15/2009 1:45 PM", "en-US")]
        [InlineData("G", "15.06.2009 13:45:30", "de-DE")]
        [InlineData("t", "1:45 PM", "en-US")]
        [InlineData("T", "1:45:30 PM", "en-US")]
        public void ToLocalDateStringTest(string format, string expected, string culture)
        {
            var sourceDate =  new DateTime(2009, 6, 15, 13, 45, 30, DateTimeKind.Utc);
            var date = new DateTimeOffset(sourceDate);
            var result = date.ToLocalDateString(format, TimeSpan.FromHours(0), new CultureInfo(culture));
            Assert.Equal(expected, result);
        }


        /// <summary>
        /// Defines the test method ToLocalDateTimeOffsetTest.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="hour">The hour.</param>
        [Theory]
        [InlineData("2021-01-29 19:56:18.3907747 +00:00", 1, 20)]
        [InlineData("2021-01-29 20:44:55.0977507 +00:00", 1, 21)]
        [InlineData("2021-01-29 20:55:47.8056804 +01:00", 1, 20)]
        public void ToLocalDateTimeOffsetTest(string datetime, int offset, int hour)
        {
            if(DateTimeOffset.TryParse(datetime, out var actualDateTime))
            {
                var localDateTime = actualDateTime.ToOffset(TimeSpan.FromHours(offset));
                Assert.Equal(hour, localDateTime.Hour);
            }
        }
    }
}
