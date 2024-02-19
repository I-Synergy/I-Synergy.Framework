using ISynergy.Framework.Core.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.Core.Extensions.Tests;

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
        DateTimeOffset self = new(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
        DateTimeOffset comparer = new(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);

        Assert.IsTrue(self.IsInRangeOfDate(comparer));
    }

    /// <summary>
    /// Defines the test method IsInRangeOfDateDifferentOffsetTest.
    /// </summary>
    [TestMethod]
    public void IsInRangeOfDateDifferentOffsetTest()
    {
        DateTimeOffset self = new(2017, 10, 11, 14, 30, 0, new TimeSpan(-8, 0, 0));
        DateTimeOffset comparer = new(2017, 10, 10, 23, 0, 0, new TimeSpan(0, 0, 0));

        Assert.IsFalse(self.IsInRangeOfDate(comparer));
    }

    /// <summary>
    /// Defines the test method IsInRangeOfDatesSameOffsetTest.
    /// </summary>
    [TestMethod]
    public void IsInRangeOfDatesSameOffsetTest()
    {
        DateTimeOffset self = new(2017, 10, 11, 14, 30, 0, TimeZoneInfo.Local.BaseUtcOffset);
        DateTimeOffset start = new(2017, 10, 11, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset);
        DateTimeOffset end = new(2017, 10, 11, 23, 59, 59, TimeZoneInfo.Local.BaseUtcOffset);

        Assert.IsTrue(self.IsInRangeOfDate(start, end));
    }

    /// <summary>
    /// Defines the test method IsInRangeOfDatesDifferentOffsetTest.
    /// </summary>
    [TestMethod]
    public void IsInRangeOfDatesDifferentOffsetTest()
    {
        DateTimeOffset self = new(2017, 10, 11, 14, 30, 0, new TimeSpan(-8, 0, 0));
        DateTimeOffset start = new(2017, 10, 10, 22, 0, 0, new TimeSpan(0, 0, 0));
        DateTimeOffset end = new(2017, 10, 11, 21, 59, 59, new TimeSpan(0, 0, 0));

        Assert.IsFalse(self.IsInRangeOfDate(start, end));
    }

    /// <summary>
    /// Defines the test method ToStartOfDayLocalTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfDayLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay();
        DateTimeOffset test = new DateTimeOffset(1975, 10, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime();
        Assert.AreEqual(test, result);
    }

    /// <summary>
    /// Defines the test method ToEndOfDayLocalTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfDayLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay();
        Assert.AreEqual(new DateTimeOffset(1975, 10, 29, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfMonthLocalTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfMonthLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfMonth();
        Assert.AreEqual(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfMonthLocalTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfMonthLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfMonth();
        Assert.AreEqual(new DateTimeOffset(1975, 10, 31, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddDays(1).AddTicks(-1).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfYearLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfYear();
        Assert.AreEqual(new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfYearLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfYear();
        Assert.AreEqual(new DateTimeOffset(1975, 1, 1, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddYears(1).AddTicks(-1).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfQuarterLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfQuarter();
        Assert.AreEqual(new DateTimeOffset(1975, 10, 1, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfQuarterLocalTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 29, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfQuarter();
        Assert.AreEqual(new DateTimeOffset(1975, 12, 1, 0, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddMonths(1).AddTicks(-1).ToUniversalTime(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWeekOnMondayTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWeekOnMondayTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Monday);
        Assert.AreEqual(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWeekOnMondayTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWeekOnMondayTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Monday);
        Assert.AreEqual(new DateTimeOffset(1975, 10, 5, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWeekOnSundayTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWeekOnSundayTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWeek(DayOfWeek.Sunday);
        Assert.AreEqual(new DateTimeOffset(1975, 9, 28, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWeekOnSundayTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWeekOnSundayTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWeek(DayOfWeek.Sunday);
        Assert.AreEqual(new DateTimeOffset(1975, 10, 4, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWorkWeekTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWorkWeekTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 4, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfWorkWeek();
        Assert.AreEqual(new DateTimeOffset(1975, 9, 29, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWorkWeekTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWorkWeekTest()
    {
        DateTimeOffset result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfWorkWeek();
        Assert.AreEqual(new DateTimeOffset(1975, 10, 3, 0, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToUniversalTimeStringTest.
    /// </summary>
    [TestMethod]
    public void ToUniversalTimeStringTest()
    {
        TimeZoneInfo timezoneWesternEurope = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        string result = new DateTimeOffset(1975, 10, 1, 14, 43, 35, timezoneWesternEurope.BaseUtcOffset).ToUniversalTimeString();
        string expected = new DateTimeOffset(1975, 10, 1, 13, 43, 35, TimeSpan.FromHours(0)).ToString(GenericConstants.DateTimeOffsetFormat); ;
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
        DateTime sourceDate = new(2009, 6, 15, 13, 45, 30, DateTimeKind.Utc);
        DateTimeOffset date = new(sourceDate);
        string result = date.ToLocalDateString(format, TimeSpan.FromHours(0), new CultureInfo(culture));
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
        if (DateTimeOffset.TryParse(datetime, out DateTimeOffset actualDateTime))
        {
            DateTimeOffset localDateTime = actualDateTime.ToOffset(TimeSpan.FromHours(offset));
            Assert.AreEqual(hour, localDateTime.Hour);
        }
    }
}
