using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters.Tests;

[TestClass()]
public class DateTimeOffsetConverterTests
{
    private readonly DateTimeOffsetToLocalDateTimeConverter _dateTimeOffsetToLocalDateTimeConverter;

    public DateTimeOffsetConverterTests()
    {
        _dateTimeOffsetToLocalDateTimeConverter = new DateTimeOffsetToLocalDateTimeConverter();
    }

    /// Create a unit test for <see cref="DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(DateTimeOffset)"/> method.
    [TestMethod]
    public void DateTimeOffsetToTimeSpanTest()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        // Act
        TimeSpan? result = DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(dateTimeOffset);
        // Assert
        Assert.AreEqual(dateTimeOffset.TimeOfDay, result);
    }

    /// Create a unit test for <see cref="DateTimeOffsetConverter.TimeSpanToDateTimeOffset(DateTimeOffset, TimeSpan)"/> method.
    [TestMethod]
    public void TimeSpanToDateTimeOffsetTest()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.Zero);
        TimeSpan timeSpan = new TimeSpan(1, 2, 3);
        // Act
        DateTimeOffset? result = DateTimeOffsetConverter.TimeSpanToDateTimeOffset(dateTimeOffset, timeSpan);
        // Assert
        Assert.AreEqual(new DateTimeOffset(1975, 10, 29, 1, 2, 3, TimeSpan.Zero), result);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeConverter_Convert_ShouldReturnLocalDateTime()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2022, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var result = _dateTimeOffsetToLocalDateTimeConverter.Convert(dateTimeOffset, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(dateTimeOffset.ToLocalTime().DateTime, result);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeConverter_Convert_ShouldReturnCurrentLocalDateTime_WhenValueIsNotDateTimeOffset()
    {
        // Arrange
        var expectedDateTime = DateTime.Now.ToLocalTime();

        // Act
        var result = _dateTimeOffsetToLocalDateTimeConverter.Convert("invalid value", typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(expectedDateTime.Year, ((DateTime)result).Year);
        Assert.AreEqual(expectedDateTime.Month, ((DateTime)result).Month);
        Assert.AreEqual(expectedDateTime.Day, ((DateTime)result).Day);
        Assert.AreEqual(expectedDateTime.Hour, ((DateTime)result).Hour);
        Assert.AreEqual(expectedDateTime.Minute, ((DateTime)result).Minute);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeConverter_ConvertBack_ShouldReturnUniversalDateTimeOffset()
    {
        // Arrange
        var dateTime = new DateTime(2022, 1, 1, 12, 0, 0);

        // Act
        var result = _dateTimeOffsetToLocalDateTimeConverter.ConvertBack(dateTime, typeof(DateTimeOffset), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(dateTime.ToUniversalTime().Year, ((DateTimeOffset)result).Year);
        Assert.AreEqual(dateTime.ToUniversalTime().Month, ((DateTimeOffset)result).Month);
        Assert.AreEqual(dateTime.ToUniversalTime().Day, ((DateTimeOffset)result).Day);
        Assert.AreEqual(dateTime.ToUniversalTime().Hour, ((DateTimeOffset)result).Hour);
        Assert.AreEqual(dateTime.ToUniversalTime().Minute, ((DateTimeOffset)result).Minute);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeConverter_ConvertBack_ShouldReturnCurrentUniversalDateTimeOffset_WhenValueIsNotDateTime()
    {
        // Arrange
        var expectedDateTimeOffset = DateTimeOffset.Now.ToUniversalTime();

        // Act
        var result = _dateTimeOffsetToLocalDateTimeConverter.ConvertBack("invalid value", typeof(DateTimeOffset), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(expectedDateTimeOffset.Year, ((DateTimeOffset)result).Year);
        Assert.AreEqual(expectedDateTimeOffset.Month, ((DateTimeOffset)result).Month);
        Assert.AreEqual(expectedDateTimeOffset.Day, ((DateTimeOffset)result).Day);
        Assert.AreEqual(expectedDateTimeOffset.Hour, ((DateTimeOffset)result).Hour);
        Assert.AreEqual(expectedDateTimeOffset.Minute, ((DateTimeOffset)result).Minute);
    }
}