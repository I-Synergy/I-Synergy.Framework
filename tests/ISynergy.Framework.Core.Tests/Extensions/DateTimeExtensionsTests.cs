using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

/// <summary>
/// Class DateTimeExtensionsTests.
/// </summary>
[TestClass]
public class DateTimeExtensionsTests
{
    /// <summary>
    /// Defines the test method ToStartOfDayTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfDayTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfDay();
        Assert.AreEqual(new DateTime(1975, 10, 29, 0, 0, 0), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfDayTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfDayTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfDay();
        Assert.AreEqual(new DateTime(1975, 10, 29, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfMontTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfMontTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfMonth();
        Assert.AreEqual(new DateTime(1975, 10, 1, 0, 0, 0), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfMonthTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfMonthTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfMonth();
        Assert.AreEqual(new DateTime(1975, 10, 31, 0, 0, 0, 0).AddDays(1).AddTicks(-1), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfYearTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfYearTest()
    {
        DateTime result = new DateTime(1975, 1, 1).Year.ToStartOfYear();
        Assert.AreEqual(new DateTime(1975, 1, 1, 0, 0, 0), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfYearTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfYearTest()
    {
        DateTime result = new DateTime(1975, 1, 1).Year.ToEndOfYear();
        Assert.AreEqual(new DateTime(1975, 1, 1).AddYears(1).AddTicks(-1), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWeekOnMondayTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWeekOnMondayTest()
    {
        DateTime result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Monday);
        Assert.AreEqual(new DateTime(1975, 9, 29).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWeekOnMondayTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWeekOnMondayTest()
    {
        DateTime result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Monday);
        Assert.AreEqual(new DateTime(1975, 10, 5).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWeekOnSundayTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWeekOnSundayTest()
    {
        DateTime result = new DateTime(1975, 10, 1, 14, 43, 35).ToStartOfWeek(DayOfWeek.Sunday);
        Assert.AreEqual(new DateTime(1975, 9, 28).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWeekOnSundayTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWeekOnSundayTest()
    {
        DateTime result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWeek(DayOfWeek.Sunday);
        Assert.AreEqual(new DateTime(1975, 10, 4).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfWorkWeekTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfWorkWeekTest()
    {
        DateTime result = new DateTime(1975, 10, 4, 14, 43, 35).ToStartOfWorkWeek();
        Assert.AreEqual(new DateTime(1975, 9, 29).ToStartOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfWorkWeekTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfWorkWeekTest()
    {
        DateTime result = new DateTime(1975, 10, 1, 14, 43, 35).ToEndOfWorkWeek();
        Assert.AreEqual(new DateTime(1975, 10, 3).ToEndOfDay(), result);
    }

    /// <summary>
    /// Defines the test method ToStartOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToStartOfQuarterLocalTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfQuarter();
        Assert.AreEqual(new DateTime(1975, 10, 1, 0, 0, 0), result);
    }

    /// <summary>
    /// Defines the test method ToEndOfYearLocalTest.
    /// </summary>
    [TestMethod]
    public void ToEndOfQuarterLocalTest()
    {
        DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfQuarter();
        Assert.AreEqual(new DateTime(1975, 12, 1, 0, 0, 0, 0).AddMonths(1).AddTicks(-1), result);
    }
}
