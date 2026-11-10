using ISynergy.Framework.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ISynergy.Framework.UI.WPF.Converters;

/// <summary>
/// Tests for the WPF boolean converters. <see cref="InverseBooleanConverter.ConvertBack"/>
/// simply delegates to <see cref="InverseBooleanConverter.Convert"/>, so both directions share the
/// same behaviour (invert a boolean, throw for anything else).
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class BooleanConvertersTests
{
    private static readonly CultureInfo s_invariant = CultureInfo.InvariantCulture;

    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void InverseBoolean_ConvertBack_InvertsBoolean(bool input, bool expected)
    {
        var converter = new InverseBooleanConverter();

        var result = converter.ConvertBack(input, typeof(bool), null!, s_invariant);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void InverseBoolean_ConvertBack_NonBoolean_Throws()
    {
        var converter = new InverseBooleanConverter();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.ConvertBack("not-a-bool", typeof(bool), null!, s_invariant));
    }

    [TestMethod]
    [DataRow(true, Visibility.Visible)]
    [DataRow(false, Visibility.Collapsed)]
    public void BooleanToVisibility_ConvertBack_MapsVisibilityBackToBoolean(bool value, Visibility visibility)
    {
        var converter = new BooleanToVisibilityConverter();

        Assert.AreEqual(value, converter.ConvertBack(visibility, typeof(bool), null!, s_invariant));
    }
}
