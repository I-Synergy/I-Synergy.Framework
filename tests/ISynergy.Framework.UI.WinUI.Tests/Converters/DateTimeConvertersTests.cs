using ISynergy.Framework.UI.Converters;

namespace ISynergy.Framework.UI.WinUI.Converters;

/// <summary>
/// Tests for the WinUI date/time value converters, covering the thin adapters that now delegate to
/// the shared <see cref="DateTimeConverter"/> / <see cref="DateTimeOffsetConverter"/> helpers and
/// fall back to the <c>MinValue</c> defaults through <c>GetValueOrDefault</c>.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DateTimeConvertersTests
{
    private static readonly DateTime SampleDate = new(2024, 5, 17, 13, 45, 30, DateTimeKind.Utc);
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(1);

    #region DateTimeToTimeSpanConverter

    [TestMethod]
    public void DateTimeToTimeSpanConverter_Convert_ReturnsTimeOfDay()
    {
        var converter = new DateTimeToTimeSpanConverter();

        var result = converter.Convert(SampleDate, typeof(TimeSpan), null!, null!);

        Assert.AreEqual(new TimeSpan(13, 45, 30), result);
    }

    [TestMethod]
    public void DateTimeToTimeSpanConverter_Convert_MinValue_ReturnsZero()
    {
        var converter = new DateTimeToTimeSpanConverter();

        Assert.AreEqual(TimeSpan.Zero, converter.Convert(DateTime.MinValue, typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeToTimeSpanConverter_Convert_MaxValue_ReturnsZero()
    {
        var converter = new DateTimeToTimeSpanConverter();

        Assert.AreEqual(TimeSpan.Zero, converter.Convert(DateTime.MaxValue, typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeToTimeSpanConverter_Convert_WithNonDateTime_ReturnsMinValue()
    {
        var converter = new DateTimeToTimeSpanConverter();

        Assert.AreEqual(TimeSpan.MinValue, converter.Convert("not a date", typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeToTimeSpanConverter_ConvertBack_KeepsOriginalDateAndKind()
    {
        var converter = new DateTimeToTimeSpanConverter();
        converter.Convert(SampleDate, typeof(TimeSpan), null!, null!);

        var result = converter.ConvertBack(new TimeSpan(10, 30, 0), typeof(DateTime), null!, null!);

        Assert.AreEqual(new DateTime(2024, 5, 17, 10, 30, 0, DateTimeKind.Utc), result);
    }

    [TestMethod]
    public void DateTimeToTimeSpanConverter_ConvertBack_WithNonTimeSpan_ReturnsMinValue()
    {
        var converter = new DateTimeToTimeSpanConverter();

        Assert.AreEqual(DateTime.MinValue, converter.ConvertBack("not a time span", typeof(DateTime), null!, null!));
    }

    #endregion

    #region DateTimeToDateTimeOffsetConverter

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_Convert_ReturnsOffsetFromDateTime()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();

        var result = converter.Convert(SampleDate, typeof(DateTimeOffset), null!, null!);

        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(SampleDate, ((DateTimeOffset)result).DateTime);
    }

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_Convert_WithNonDateTime_ReturnsNow()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();

        var result = converter.Convert(42, typeof(DateTimeOffset), null!, null!);

        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(DateTimeOffset.Now.Offset, ((DateTimeOffset)result).Offset,
            "The fallback must be a valid local DateTimeOffset");
    }

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_ConvertBack_WithUtcOffset_ReturnsUtcDateTime()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();
        var value = new DateTimeOffset(2024, 5, 17, 12, 0, 0, TimeSpan.Zero);

        var result = converter.ConvertBack(value, typeof(DateTime), null!, null!);

        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(DateTimeKind.Utc, ((DateTime)result).Kind);
        Assert.AreEqual(value.UtcDateTime, result);
    }

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_ConvertBack_WithLocalOffset_ReturnsLocalDateTime()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();
        var clockTime = new DateTime(2024, 5, 17, 12, 0, 0, DateTimeKind.Unspecified);
        var localOffset = TimeZoneInfo.Local.GetUtcOffset(clockTime);

        // On a UTC runner the local offset is zero, which is indistinguishable from the
        // converter's explicit-UTC branch; the local branch cannot be exercised there.
        if (localOffset == TimeSpan.Zero)
            Assert.Inconclusive("The local time zone is UTC; the Local branch equals the UTC branch.");

        var value = new DateTimeOffset(clockTime, localOffset);

        var result = converter.ConvertBack(value, typeof(DateTime), null!, null!);

        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(DateTimeKind.Local, ((DateTime)result).Kind);
        Assert.AreEqual(clockTime, result);
    }

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_ConvertBack_WithForeignOffset_ReturnsUnspecifiedDateTime()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();

        // Pick an offset that is definitely not the runner's local offset.
        var foreignOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2024, 5, 17, 12, 0, 0))
            == TimeSpan.FromHours(5)
                ? TimeSpan.FromHours(6)
                : TimeSpan.FromHours(5);
        var value = new DateTimeOffset(2024, 5, 17, 12, 0, 0, foreignOffset);

        var result = converter.ConvertBack(value, typeof(DateTime), null!, null!);

        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(DateTimeKind.Unspecified, ((DateTime)result).Kind);
        Assert.AreEqual(value.DateTime, result);
    }

    [TestMethod]
    public void DateTimeToDateTimeOffsetConverter_ConvertBack_WithNonDateTimeOffset_ReturnsNow()
    {
        var converter = new DateTimeToDateTimeOffsetConverter();

        var result = converter.ConvertBack("not a date", typeof(DateTime), null!, null!);

        Assert.IsInstanceOfType<DateTime>(result);
        Assert.AreEqual(DateTimeKind.Local, ((DateTime)result).Kind);
        Assert.IsTrue((DateTime.Now - (DateTime)result).Duration() < Tolerance);
    }

    #endregion

    #region DateTimeToLocalDateStringConverter

    [TestMethod]
    public void DateTimeToLocalDateStringConverter_Convert_WithParameter_UsesParameterFormat()
    {
        var converter = new DateTimeToLocalDateStringConverter();

        var result = converter.Convert(SampleDate, typeof(string), "d", null!);

        Assert.AreEqual(SampleDate.ToLocalTime().ToString("d"), result);
    }

    [TestMethod]
    public void DateTimeToLocalDateStringConverter_Convert_WithoutParameter_UsesFullFormat()
    {
        var converter = new DateTimeToLocalDateStringConverter();

        var result = converter.Convert(SampleDate, typeof(string), null!, null!);

        Assert.AreEqual(SampleDate.ToLocalTime().ToString("f"), result);
    }

    [TestMethod]
    public void DateTimeToLocalDateStringConverter_Convert_WithNonDateTime_ReturnsNowString()
    {
        var converter = new DateTimeToLocalDateStringConverter();
        var before = DateTime.Now.ToString("f");

        var result = converter.Convert("not a date", typeof(string), null!, null!);

        var after = DateTime.Now.ToString("f");
        Assert.IsTrue(result is string value && (value == before || value == after));
    }

    [TestMethod]
    public void DateTimeToLocalDateStringConverter_ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new DateTimeToLocalDateStringConverter();

        Assert.ThrowsExactly<NotImplementedException>(
            () => converter.ConvertBack("2024-05-17", typeof(DateTime), null!, null!));
    }

    #endregion

    #region DateTimeOffsetToTimeSpanConverter

    [TestMethod]
    public void DateTimeOffsetToTimeSpanConverter_Convert_ReturnsTimeOfDay()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();
        var value = new DateTimeOffset(2024, 5, 17, 13, 45, 30, TimeSpan.FromHours(2));

        Assert.AreEqual(new TimeSpan(13, 45, 30), converter.Convert(value, typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpanConverter_Convert_MinValue_ReturnsZero()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();

        Assert.AreEqual(TimeSpan.Zero, converter.Convert(DateTimeOffset.MinValue, typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpanConverter_Convert_WithNonDateTimeOffset_ReturnsMinValue()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();

        Assert.AreEqual(TimeSpan.MinValue, converter.Convert("not a date", typeof(TimeSpan), null!, null!));
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpanConverter_ConvertBack_KeepsDateAndOffset()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();
        converter.Convert(new DateTimeOffset(2024, 5, 17, 13, 45, 30, TimeSpan.FromHours(2)), typeof(TimeSpan), null!, null!);

        var result = converter.ConvertBack(new TimeSpan(10, 30, 0), typeof(DateTimeOffset), null!, null!);

        Assert.AreEqual(new DateTimeOffset(2024, 5, 17, 10, 30, 0, TimeSpan.FromHours(2)), result);
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpanConverter_ConvertBack_WithNonTimeSpan_ReturnsMinValue()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();

        Assert.AreEqual(DateTimeOffset.MinValue, converter.ConvertBack("not a time span", typeof(DateTimeOffset), null!, null!));
    }

    #endregion

    #region DateTimeOffsetToLocalDateTimeOffsetConverter

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeOffsetConverter_Convert_ReturnsLocalTime()
    {
        var converter = new DateTimeOffsetToLocalDateTimeOffsetConverter();
        var value = new DateTimeOffset(2024, 5, 17, 13, 45, 30, TimeSpan.Zero);

        var result = converter.Convert(value, typeof(DateTimeOffset), null!, null!);

        Assert.AreEqual(value.ToLocalTime(), result);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeOffsetConverter_Convert_WithNonDateTimeOffset_ReturnsLocalNow()
    {
        var converter = new DateTimeOffsetToLocalDateTimeOffsetConverter();

        var result = converter.Convert("not a date", typeof(DateTimeOffset), null!, null!);

        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(DateTimeOffset.Now.Offset, ((DateTimeOffset)result).Offset,
            "The fallback must be a local DateTimeOffset");
        Assert.IsTrue((DateTimeOffset.Now - (DateTimeOffset)result).Duration() < Tolerance);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeOffsetConverter_ConvertBack_ReturnsUniversalTime()
    {
        var converter = new DateTimeOffsetToLocalDateTimeOffsetConverter();
        var value = new DateTimeOffset(2024, 5, 17, 13, 45, 30, TimeSpan.FromHours(2));

        var result = converter.ConvertBack(value, typeof(DateTimeOffset), null!, null!);

        Assert.AreEqual(value.ToUniversalTime(), result);
    }

    [TestMethod]
    public void DateTimeOffsetToLocalDateTimeOffsetConverter_ConvertBack_WithNonDateTimeOffset_ReturnsUniversalNow()
    {
        var converter = new DateTimeOffsetToLocalDateTimeOffsetConverter();
        var before = DateTimeOffset.UtcNow;

        var result = converter.ConvertBack("not a date", typeof(DateTimeOffset), null!, null!);

        Assert.IsInstanceOfType<DateTimeOffset>(result);
        Assert.AreEqual(TimeSpan.Zero, ((DateTimeOffset)result).Offset);
        Assert.IsTrue(((DateTimeOffset)result - before).Duration() < Tolerance);
    }

    #endregion
}
