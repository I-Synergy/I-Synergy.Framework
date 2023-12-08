using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.UI.Converters.Tests;

[TestClass()]
public class DateTimeOffsetConverterTests
{
    /// Create a unit test for <see cref="DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(DateTimeOffset)"/> method.
    [TestMethod]
    public void DateTimeOffsetToTimeSpanTest()
    {
        // Arrange
        var dateTimeOffset = DateTimeOffset.Now;
        // Act
        var result = DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(dateTimeOffset);
        // Assert
        Assert.AreEqual(dateTimeOffset.TimeOfDay, result);
    }

    /// Create a unit test for <see cref="DateTimeOffsetConverter.TimeSpanToDateTimeOffset(TimeSpan)"/> method.
    [TestMethod]
    public void TimeSpanToDateTimeOffsetTest()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(1975, 10, 29, 15, 0, 0, TimeSpan.Zero);
        var timeSpan = new TimeSpan(1, 2, 3);
        // Act
        var result = DateTimeOffsetConverter.TimeSpanToDateTimeOffset(dateTimeOffset, timeSpan);
        // Assert
        Assert.AreEqual(new DateTimeOffset(1975,10,29,1,2,3,TimeSpan.Zero), result);
    }
}