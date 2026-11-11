using ISynergy.Framework.UI.Styles;
using System.Reflection;
using Windows.UI;

namespace ISynergy.Framework.UI.WinUI.Styles;

/// <summary>
/// Tests for the color-conversion helpers extracted from
/// <see cref="DynamicThemeResourceDictionary"/> while splitting the theme palette construction
/// into <c>BuildInnerThemeDictionary</c>, <c>AddTintedBrushes</c> and <c>AddAppDesignTokens</c>.
/// <para>
/// The dictionary itself derives from the COM-backed WinUI <c>ResourceDictionary</c> (and creates
/// <c>SolidColorBrush</c> instances), which cannot be activated in a plain unit-test host, so the
/// pure-color math (<c>FromFloats</c>/<c>ToFloats</c>) that every palette value flows through is
/// verified directly.
/// </para>
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class DynamicThemeResourceDictionaryTests
{
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
    private const float Tolerance = 1f / 255f;

    [TestMethod]
    [DataRow(0f, 0f, 0f, 0, 0, 0)]
    [DataRow(1f, 1f, 1f, 255, 255, 255)]
    [DataRow(1f, 0f, 0f, 255, 0, 0)]
    [DataRow(0f, 1f, 0f, 0, 255, 0)]
    [DataRow(0f, 0f, 1f, 0, 0, 255)]
    [DataRow(0.2f, 0.4f, 0.6f, 51, 102, 153)]
    public void FromFloats_ScalesUnitRangeToBytes(float red, float green, float blue, int expectedRed, int expectedGreen, int expectedBlue)
    {
        var color = InvokeFromFloats(red, green, blue);

        Assert.AreEqual(255, color.A, "Theme colors are always fully opaque");
        Assert.AreEqual((byte)expectedRed, color.R);
        Assert.AreEqual((byte)expectedGreen, color.G);
        Assert.AreEqual((byte)expectedBlue, color.B);
    }

    [TestMethod]
    public void FromFloats_WithTuple_MatchesScalarOverload()
    {
        var fromTuple = InvokeFromFloats(new ValueTuple<float, float, float>(0.2f, 0.4f, 0.6f));
        var fromScalars = InvokeFromFloats(0.2f, 0.4f, 0.6f);

        Assert.AreEqual(fromScalars, fromTuple);
    }

    [TestMethod]
    public void ToFloats_ScalesBytesToUnitRange()
    {
        var color = Color.FromArgb(255, 255, 128, 0);

        var (red, green, blue) = InvokeToFloats(color);

        Assert.AreEqual(1f, red);
        Assert.AreEqual(128f / 255f, green);
        Assert.AreEqual(0f, blue);
    }

    [TestMethod]
    public void ToFloats_IgnoresAlphaChannel()
    {
        var transparent = Color.FromArgb(0, 255, 128, 0);
        var opaque = Color.FromArgb(255, 255, 128, 0);

        Assert.AreEqual(InvokeToFloats(opaque), InvokeToFloats(transparent));
    }

    [TestMethod]
    public void ToFloats_RoundTripsThroughFromFloats()
    {
        var (red, green, blue) = InvokeToFloats(InvokeFromFloats(0.1f, 0.5f, 0.9f));

        Assert.AreEqual(0.1f, red, Tolerance);
        Assert.AreEqual(0.5f, green, Tolerance);
        Assert.AreEqual(0.9f, blue, Tolerance);
    }

    private static Color InvokeFromFloats(float red, float green, float blue)
    {
        var method = typeof(DynamicThemeResourceDictionary).GetMethod(
            "FromFloats", PrivateStatic, null, [typeof(float), typeof(float), typeof(float)], null);

        Assert.IsNotNull(method, "FromFloats(float, float, float) must remain a private static helper");

        return (Color)method.Invoke(null, [red, green, blue])!;
    }

    private static Color InvokeFromFloats((float Red, float Green, float Blue) color)
    {
        var method = typeof(DynamicThemeResourceDictionary).GetMethod(
            "FromFloats", PrivateStatic, null, [typeof((float, float, float))], null);

        Assert.IsNotNull(method, "FromFloats((float, float, float)) must remain a private static helper");

        return (Color)method.Invoke(null, [color])!;
    }

    private static (float Red, float Green, float Blue) InvokeToFloats(Color color)
    {
        var method = typeof(DynamicThemeResourceDictionary).GetMethod(
            "ToFloats", PrivateStatic, null, [typeof(Color)], null);

        Assert.IsNotNull(method, "ToFloats(Color) must remain a private static helper");

        return ((float Red, float Green, float Blue))method.Invoke(null, [color])!;
    }
}
