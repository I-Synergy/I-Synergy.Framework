using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Converters;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace ISynergy.Framework.UI.Maui.Converters;

/// <summary>
/// Tests for the pure MAUI value converters. These have no platform dependencies, so
/// they are exercised directly (both the happy path and the null / unconvertible input
/// paths).
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class ConverterTests
{
    private static readonly CultureInfo s_invariant = CultureInfo.InvariantCulture;
    private static readonly Type s_bool = typeof(bool);

    // ── PageVisibilityConverter ─────────────────────────────────────────────

    [TestMethod]
    public void PageVisibility_MatchingPageAndParameter_ReturnsTrue()
    {
        var converter = new PageVisibilityConverter();

        var result = converter.Convert("2", s_bool, "2", s_invariant);

        Assert.IsTrue((bool)result);
    }

    [TestMethod]
    public void PageVisibility_DifferentPageAndParameter_ReturnsFalse()
    {
        var converter = new PageVisibilityConverter();

        var result = converter.Convert("2", s_bool, "3", s_invariant);

        Assert.IsFalse((bool)result);
    }

    [TestMethod]
    public void PageVisibility_NonNumericInput_ReturnsFalse()
    {
        var converter = new PageVisibilityConverter();

        var result = converter.Convert("not-a-number", s_bool, "2", s_invariant);

        Assert.IsFalse((bool)result);
    }

    // ── SelectionModeConverter ──────────────────────────────────────────────

    [TestMethod]
    public void SelectionMode_MultipleMapsToMultiple()
    {
        var converter = new SelectionModeConverter();

        var result = converter.Convert(SelectionModes.Multiple, typeof(SelectionMode), null, s_invariant);

        Assert.AreEqual(SelectionMode.Multiple, result);
    }

    [TestMethod]
    public void SelectionMode_SingleMapsToSingle()
    {
        var converter = new SelectionModeConverter();

        var result = converter.Convert(SelectionModes.Single, typeof(SelectionMode), null, s_invariant);

        Assert.AreEqual(SelectionMode.Single, result);
    }

    [TestMethod]
    public void SelectionMode_NonEnumValue_DefaultsToSingle()
    {
        var converter = new SelectionModeConverter();

        var result = converter.Convert("unexpected", typeof(SelectionMode), null, s_invariant);

        Assert.AreEqual(SelectionMode.Single, result);
    }

    // ── NullToVisibilityConverter / inverse ─────────────────────────────────

    [TestMethod]
    public void NullToVisibility_Null_ReturnsFalse()
    {
        var converter = new NullToVisibilityConverter();
        Assert.IsFalse((bool)converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToVisibility_NonNull_ReturnsTrue()
    {
        var converter = new NullToVisibilityConverter();
        Assert.IsTrue((bool)converter.Convert("value", s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToInversedVisibility_Null_ReturnsTrue()
    {
        var converter = new NullToInversedVisibilityConverter();
        Assert.IsTrue((bool)converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToInversedVisibility_NonNull_ReturnsFalse()
    {
        var converter = new NullToInversedVisibilityConverter();
        Assert.IsFalse((bool)converter.Convert("value", s_bool, null, s_invariant));
    }

    // ── NullToBooleanConverter ───────────────────────────────────────────────

    [TestMethod]
    public void NullToBoolean_Null_ReturnsFalse()
    {
        var converter = new NullToBooleanConverter();
        Assert.IsFalse((bool)converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToBoolean_NonNull_ReturnsTrue()
    {
        var converter = new NullToBooleanConverter();
        Assert.IsTrue((bool)converter.Convert(42, s_bool, null, s_invariant));
    }

    // ── BooleanNegationConverter ─────────────────────────────────────────────

    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void BooleanNegation_InvertsBoolean(bool input, bool expected)
    {
        var converter = new BooleanNegationConverter();
        Assert.AreEqual(expected, converter.Convert(input, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void BooleanNegation_NonBoolean_ReturnsTrue()
    {
        // !(value is bool x && x) short-circuits to true for non-bool input.
        var converter = new BooleanNegationConverter();
        Assert.IsTrue((bool)converter.Convert("not-a-bool", s_bool, null, s_invariant));
    }

    // ── InverseBooleanConverter ──────────────────────────────────────────────

    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void InverseBoolean_InvertsBoolean(bool input, bool expected)
    {
        var converter = new InverseBooleanConverter();
        Assert.AreEqual(expected, converter.Convert(input, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void InverseBoolean_NonBoolean_Throws()
    {
        var converter = new InverseBooleanConverter();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.Convert("not-a-bool", s_bool, null, s_invariant));
    }

    // ── DateTimeToTimeSpanConverter.ConvertBack ───────────────────────────────

    [TestMethod]
    public void DateTimeToTimeSpan_ConvertBack_TimeSpanInput_ReappliesTimeOfDayToOriginalDate()
    {
        var converter = new DateTimeToTimeSpanConverter();
        var date = new DateTime(2024, 3, 4, 13, 45, 30, DateTimeKind.Unspecified);

        // Convert remembers the original date used by ConvertBack.
        var timeOfDay = (TimeSpan)converter.Convert(date, typeof(TimeSpan), null, s_invariant);
        var result = converter.ConvertBack(new TimeSpan(8, 15, 0), typeof(DateTime), null, s_invariant);

        Assert.AreEqual(new DateTime(2024, 3, 4, 8, 15, 0, DateTimeKind.Unspecified), result);
        Assert.AreEqual(new TimeSpan(13, 45, 30), timeOfDay);
    }

    [TestMethod]
    public void DateTimeToTimeSpan_ConvertBack_WithoutPriorConvert_UsesDefaultOriginalDate()
    {
        var converter = new DateTimeToTimeSpanConverter();

        var result = converter.ConvertBack(new TimeSpan(6, 30, 0), typeof(DateTime), null, s_invariant);

        Assert.AreEqual(DateTime.MinValue.Date.AddHours(6).AddMinutes(30), result);
    }

    [TestMethod]
    public void DateTimeToTimeSpan_ConvertBack_InvalidInput_ReturnsDateTimeMinValue()
    {
        var converter = new DateTimeToTimeSpanConverter();

        var result = converter.ConvertBack("not-a-timespan", typeof(DateTime), null, s_invariant);

        Assert.AreEqual(DateTime.MinValue, result);
    }

    [TestMethod]
    public void DateTimeToTimeSpan_ConvertBack_NullInput_ReturnsDateTimeMinValue()
    {
        var converter = new DateTimeToTimeSpanConverter();

        Assert.AreEqual(DateTime.MinValue, converter.ConvertBack(null, typeof(DateTime), null, s_invariant));
    }

    // ── DateTimeOffsetToTimeSpanConverter.ConvertBack ─────────────────────────

    [TestMethod]
    public void DateTimeOffsetToTimeSpan_ConvertBack_TimeSpanInput_ReappliesTimeOfDayAndOffset()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();
        var date = new DateTimeOffset(2024, 3, 4, 13, 45, 30, TimeSpan.FromHours(2));

        var timeOfDay = (TimeSpan)converter.Convert(date, typeof(TimeSpan), null, s_invariant);
        var result = converter.ConvertBack(new TimeSpan(8, 15, 0), typeof(DateTimeOffset), null, s_invariant);

        Assert.AreEqual(
            new DateTimeOffset(2024, 3, 4, 8, 15, 0, TimeSpan.FromHours(2)),
            result);
        Assert.AreEqual(new TimeSpan(13, 45, 30), timeOfDay);
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpan_ConvertBack_WithoutPriorConvert_UsesDefaultOriginal()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();

        var result = converter.ConvertBack(new TimeSpan(6, 30, 0), typeof(DateTimeOffset), null, s_invariant);

        Assert.AreEqual(new DateTimeOffset(1, 1, 1, 6, 30, 0, TimeSpan.Zero), result);
    }

    [TestMethod]
    public void DateTimeOffsetToTimeSpan_ConvertBack_InvalidInput_ReturnsDateTimeOffsetMinValue()
    {
        var converter = new DateTimeOffsetToTimeSpanConverter();

        var result = converter.ConvertBack("not-a-timespan", typeof(DateTimeOffset), null, s_invariant);

        Assert.AreEqual(DateTimeOffset.MinValue, result);
    }

    // ── DecimalNumberFormatter.ConvertBack ────────────────────────────────────

    [TestMethod]
    [DataRow("42.5", 42.5)]
    [DataRow("42", 42)]
    [DataRow("-13.75", -13.75)]
    public void DecimalNumberFormatter_ConvertBack_NumericString_ParsesValue(string input, double expected)
    {
        var converter = new DecimalNumberFormatter();

        var result = converter.ConvertBack(input, typeof(decimal), null, s_invariant);

        Assert.AreEqual((decimal)expected, result);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("not-a-number")]
    public void DecimalNumberFormatter_ConvertBack_NonNumericInput_ReturnsZero(string input)
    {
        var converter = new DecimalNumberFormatter();

        Assert.AreEqual(0m, converter.ConvertBack(input, typeof(decimal), null, s_invariant));
    }

    [TestMethod]
    public void DecimalNumberFormatter_ConvertBack_NullInput_ReturnsZero()
    {
        var converter = new DecimalNumberFormatter();

        Assert.AreEqual(0m, converter.ConvertBack(null, typeof(decimal), null, s_invariant));
    }

    // ── IntegerNumberFormatter.ConvertBack ────────────────────────────────────

    [TestMethod]
    [DataRow("7", 7)]
    [DataRow("-21", -21)]
    public void IntegerNumberFormatter_ConvertBack_NumericString_ParsesValue(string input, int expected)
    {
        var converter = new IntegerNumberFormatter();

        var result = converter.ConvertBack(input, typeof(int), null, s_invariant);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("not-a-number")]
    public void IntegerNumberFormatter_ConvertBack_NonNumericInput_ReturnsZero(string input)
    {
        var converter = new IntegerNumberFormatter();

        Assert.AreEqual(0, converter.ConvertBack(input, typeof(int), null, s_invariant));
    }

    [TestMethod]
    public void IntegerNumberFormatter_ConvertBack_NullInput_ReturnsZero()
    {
        var converter = new IntegerNumberFormatter();

        Assert.AreEqual(0, converter.ConvertBack(null, typeof(int), null, s_invariant));
    }
}
