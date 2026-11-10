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

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void PageVisibility_DifferentPageAndParameter_ReturnsFalse()
    {
        var converter = new PageVisibilityConverter();

        var result = converter.Convert("2", s_bool, "3", s_invariant);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void PageVisibility_NonNumericInput_ReturnsFalse()
    {
        var converter = new PageVisibilityConverter();

        var result = converter.Convert("not-a-number", s_bool, "2", s_invariant);

        Assert.AreEqual(false, result);
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
        Assert.AreEqual(false, converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToVisibility_NonNull_ReturnsTrue()
    {
        var converter = new NullToVisibilityConverter();
        Assert.AreEqual(true, converter.Convert("value", s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToInversedVisibility_Null_ReturnsTrue()
    {
        var converter = new NullToInversedVisibilityConverter();
        Assert.AreEqual(true, converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToInversedVisibility_NonNull_ReturnsFalse()
    {
        var converter = new NullToInversedVisibilityConverter();
        Assert.AreEqual(false, converter.Convert("value", s_bool, null, s_invariant));
    }

    // ── NullToBooleanConverter ───────────────────────────────────────────────

    [TestMethod]
    public void NullToBoolean_Null_ReturnsFalse()
    {
        var converter = new NullToBooleanConverter();
        Assert.AreEqual(false, converter.Convert(null, s_bool, null, s_invariant));
    }

    [TestMethod]
    public void NullToBoolean_NonNull_ReturnsTrue()
    {
        var converter = new NullToBooleanConverter();
        Assert.AreEqual(true, converter.Convert(42, s_bool, null, s_invariant));
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
        Assert.AreEqual(true, converter.Convert("not-a-bool", s_bool, null, s_invariant));
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
}
