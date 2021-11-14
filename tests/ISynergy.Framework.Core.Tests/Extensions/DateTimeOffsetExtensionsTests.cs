using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.Core.Tests.Extensions
{
    /// <summary>
    /// Class DateTimeOffsetExtensionsTests.
    /// </summary>
    [TestClass]
    public class DateTimeOffsetExtensionsTests
    {
        /// <summary>
        /// Defines the test method IsInRangeOfDateSameOffsetTest.
        /// </summary>
        [TestMethod]
        public void IsInRangeOfDateSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.IsTrue(self.IsInRangeOfDate(comparer));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDateDifferentOffsetTest.
        /// </summary>
        [TestMethod]
        public void IsInRangeOfDateDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var comparer = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));

            Assert.IsTrue(self.IsInRangeOfDate(comparer));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDatesSameOffsetTest.
        /// </summary>
        [TestMethod]
        public void IsInRangeOfDatesSameOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var end = new DateTimeOffset(2017, 10, 11, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset);

            Assert.IsTrue(self.IsInRangeOfDate(start, end));
        }

        /// <summary>
        /// Defines the test method IsInRangeOfDatesDifferentOffsetTest.
        /// </summary>
        [TestMethod]
        public void IsInRangeOfDatesDifferentOffsetTest()
        {
            var self = new DateTimeOffset(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
            var start = new DateTimeOffset(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));
            var end = new DateTimeOffset(2017, 10, 11, 21, 59, 59, new TimeSpan(0, 0, 0));

            Assert.IsTrue(self.IsInRangeOfDate(start, end));
        }

        /// <summary>
        /// Defines the test method ToStartOfDayLocalTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay();
            var test = new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime();
            Assert.AreEqual(test, result);
        }

        /// <summary>
        /// Defines the test method ToEndOfDayLocalTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfDayLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay();
            Assert.AreEqual(new DateTimeOffset(1975, 10, 29, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfMonthLocalTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth();
            Assert.AreEqual(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfMonthLocalTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfMonthLocalTest()
        {
            var result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth();
            Assert.AreEqual(new DateTimeOffset(1975, 10, 31, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfYearLocalTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToStartOfYear();
            Assert.AreEqual(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfYearLocalTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfYearLocalTest()
        {
            var result = new DateTime(1975, 10, 29).Year.ToEndOfYear();
            Assert.AreEqual(new DateTimeOffset(1975, 1, 1, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddYears(1).AddTicks(-1).ToUniversalTime(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnMondayTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnMondayTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWeekOnMondayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(new DateTimeOffset(1975, 10, 5, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWeekOnSundayTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(new DateTimeOffset(1975, 9, 28, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWeekOnSundayTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWeekOnSundayTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(new DateTimeOffset(1975, 10, 4, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToStartOfWorkWeekTest.
        /// </summary>
        [TestMethod]
        public void ToStartOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 4, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWorkWeek();
            Assert.AreEqual(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToEndOfWorkWeekTest.
        /// </summary>
        [TestMethod]
        public void ToEndOfWorkWeekTest()
        {
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWorkWeek();
            Assert.AreEqual(new DateTimeOffset(1975, 10, 3, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
        }

        /// <summary>
        /// Defines the test method ToUniversalTimeStringTest.
        /// </summary>
        [TestMethod]
        public void ToUniversalTimeStringTest()
        {
            var timezoneWesternEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, timezoneWesternEurope.BaseUtcOffset).ToUniversalTimeString();
            var expected = new DateTimeOffset(1975, 10, 1, 13, 43, 35, TimeSpan.FromHours(0)).ToString(GenericConstants.DateTimeOffsetFormat); ;
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method ToLocalDateStringTest.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="culture">The culture.</param>
        [DataTestMethod]
        [DataRow("d", "6/15/2009", "en-US")]
        [DataRow("D", "Monday, June 15, 2009", "en-US")]
        [DataRow("f", "maandag 15 juni 2009 13:45", "nl-NL")]
        [DataRow("F", "Monday, June 15, 2009 1:45:30 PM", "en-US")]
        [DataRow("g", "6/15/2009 1:45 PM", "en-US")]
        [DataRow("G", "15.06.2009 13:45:30", "de-DE")]
        [DataRow("t", "1:45 PM", "en-US")]
        [DataRow("T", "1:45:30 PM", "en-US")]
        public void ToLocalDateStringTest(string format, string expected, string culture)
        {
            var sourceDate = new DateTime(2009, 6, 15, 13, 45, 30, DateTimeKind.Utc);
            var date = new DateTimeOffset(sourceDate);
            var result = date.ToLocalDateString(format, TimeSpan.FromHours(0), new CultureInfo(culture));
            Assert.AreEqual(expected, result);
        }


        /// <summary>
        /// Defines the test method ToLocalDateTimeOffsetTest.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="hour">The hour.</param>
        [DataTestMethod]
        [DataRow("2021-01-29 19:56:18.3907747 +00:00", 1, 20)]
        [DataRow("2021-01-29 20:44:55.0977507 +00:00", 1, 21)]
        [DataRow("2021-01-29 20:55:47.8056804 +01:00", 1, 20)]
        public void ToLocalDateTimeOffsetTest(string datetime, int offset, int hour)
        {
            if (DateTimeOffset.TryParse(datetime, out var actualDateTime))
            {
                var localDateTime = actualDateTime.ToOffset(TimeSpan.FromHours(offset));
                Assert.AreEqual(hour, localDateTime.Hour);
            }
        }
    }
}
