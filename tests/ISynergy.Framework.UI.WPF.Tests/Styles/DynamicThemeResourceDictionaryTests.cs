using ISynergy.Framework.UI.Styles;
using System.Windows.Media;

namespace ISynergy.Framework.UI.WPF.Styles;

[TestClass]
public class DynamicThemeResourceDictionaryTests
{
    private static readonly System.Windows.Media.Color WindowsBlue =
        System.Windows.Media.Color.FromArgb(255, 0, 0x78, 0xD7);

    private static readonly string[] TintedGrayKeys =
        ["000", "100", "200", "300", "400", "500", "600", "700", "800", "900", "950"];

    private static readonly string[] SemanticBrushKeys =
        ["TintedBackgroundBrush", "TintedSurfaceBrush", "TintedControlBrush",
         "TintedDisabledBrush", "TintedBorderBrush", "TintedSecondaryBrush"];

    // ── Key presence ─────────────────────────────────────────────────────────

    [TestMethod]
    public void ContainsPrimaryColorKey()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        Assert.IsTrue(dict.Contains("Primary"));
    }

    [TestMethod]
    public void ContainsSecondaryAndTertiaryColorKeys()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        Assert.IsTrue(dict.Contains("Secondary"));
        Assert.IsTrue(dict.Contains("Tertiary"));
    }

    [TestMethod]
    public void ContainsPrimarySecondaryTertiaryBrushKeys()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        Assert.IsTrue(dict.Contains("PrimaryBrush"));
        Assert.IsTrue(dict.Contains("SecondaryBrush"));
        Assert.IsTrue(dict.Contains("TertiaryBrush"));
    }

    [TestMethod]
    public void ContainsAllTintedGrayColorKeys()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in TintedGrayKeys)
            Assert.IsTrue(dict.Contains($"TintedGray{key}"), $"Missing key: TintedGray{key}");
    }

    [TestMethod]
    public void ContainsAllTintedGrayBrushKeys()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in TintedGrayKeys)
            Assert.IsTrue(dict.Contains($"TintedGray{key}Brush"), $"Missing key: TintedGray{key}Brush");
    }

    [TestMethod]
    public void ContainsAllSemanticBrushKeys()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in SemanticBrushKeys)
            Assert.IsTrue(dict.Contains(key), $"Missing semantic brush key: {key}");
    }

    // ── Correct types ────────────────────────────────────────────────────────

    [TestMethod]
    public void PrimaryValue_IsColor()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        Assert.IsInstanceOfType(dict["Primary"], typeof(System.Windows.Media.Color));
    }

    [TestMethod]
    public void PrimaryBrushValue_IsSolidColorBrush()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        Assert.IsInstanceOfType(dict["PrimaryBrush"], typeof(SolidColorBrush));
    }

    [TestMethod]
    public void AllTintedGrayBrushValues_AreSolidColorBrush()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in TintedGrayKeys)
            Assert.IsInstanceOfType(dict[$"TintedGray{key}Brush"], typeof(SolidColorBrush), $"TintedGray{key}Brush should be SolidColorBrush");
    }

    [TestMethod]
    public void AllSemanticBrushValues_AreSolidColorBrush()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in SemanticBrushKeys)
            Assert.IsInstanceOfType(dict[key], typeof(SolidColorBrush), $"{key} should be SolidColorBrush");
    }

    // ── Alpha = 255 ──────────────────────────────────────────────────────────

    [TestMethod]
    public void PrimaryColor_HasFullAlpha()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var color = (System.Windows.Media.Color)dict["Primary"];
        Assert.AreEqual(255, color.A, "Primary color alpha must be 255");
    }

    [TestMethod]
    public void SecondaryColor_HasFullAlpha()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var color = (System.Windows.Media.Color)dict["Secondary"];
        Assert.AreEqual(255, color.A, "Secondary color alpha must be 255");
    }

    [TestMethod]
    public void TertiaryColor_HasFullAlpha()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var color = (System.Windows.Media.Color)dict["Tertiary"];
        Assert.AreEqual(255, color.A, "Tertiary color alpha must be 255");
    }

    [TestMethod]
    public void AllTintedGrayColors_HaveFullAlpha()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in TintedGrayKeys)
        {
            var color = (System.Windows.Media.Color)dict[$"TintedGray{key}"];
            Assert.AreEqual(255, color.A, $"TintedGray{key} alpha must be 255");
        }
    }

    [TestMethod]
    public void AllTintedGrayBrushes_HaveFullAlpha()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        foreach (var key in TintedGrayKeys)
        {
            var brush = (SolidColorBrush)dict[$"TintedGray{key}Brush"];
            Assert.AreEqual(255, brush.Color.A, $"TintedGray{key}Brush alpha must be 255");
        }
    }

    // ── Primary color is preserved ───────────────────────────────────────────

    [TestMethod]
    public void PrimaryColorValue_MatchesInputColor()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var color = (System.Windows.Media.Color)dict["Primary"];

        Assert.AreEqual(WindowsBlue.R, color.R);
        Assert.AreEqual(WindowsBlue.G, color.G);
        Assert.AreEqual(WindowsBlue.B, color.B);
    }

    [TestMethod]
    public void PrimaryBrushColor_MatchesInputColor()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var brush = (SolidColorBrush)dict["PrimaryBrush"];

        Assert.AreEqual(WindowsBlue.R, brush.Color.R);
        Assert.AreEqual(WindowsBlue.G, brush.Color.G);
        Assert.AreEqual(WindowsBlue.B, brush.Color.B);
    }

    // ── Secondary/Tertiary are derived correctly ─────────────────────────────

    [TestMethod]
    public void SecondaryColor_IsLighterThanPrimary()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var primary = (System.Windows.Media.Color)dict["Primary"];
        var secondary = (System.Windows.Media.Color)dict["Secondary"];

        float primaryLum = (MathF.Max(primary.R, MathF.Max(primary.G, primary.B)) + MathF.Min(primary.R, MathF.Min(primary.G, primary.B))) / 2f;
        float secondaryLum = (MathF.Max(secondary.R, MathF.Max(secondary.G, secondary.B)) + MathF.Min(secondary.R, MathF.Min(secondary.G, secondary.B))) / 2f;

        Assert.IsTrue(secondaryLum > primaryLum, $"Secondary ({secondaryLum:F1}) should be lighter than primary ({primaryLum:F1})");
    }

    [TestMethod]
    public void TertiaryColor_IsDarkerThanPrimary()
    {
        var dict = new DynamicThemeResourceDictionary(WindowsBlue);
        var primary = (System.Windows.Media.Color)dict["Primary"];
        var tertiary = (System.Windows.Media.Color)dict["Tertiary"];

        float primaryLum = (MathF.Max(primary.R, MathF.Max(primary.G, primary.B)) + MathF.Min(primary.R, MathF.Min(primary.G, primary.B))) / 2f;
        float tertiaryLum = (MathF.Max(tertiary.R, MathF.Max(tertiary.G, tertiary.B)) + MathF.Min(tertiary.R, MathF.Min(tertiary.G, tertiary.B))) / 2f;

        Assert.IsTrue(tertiaryLum < primaryLum, $"Tertiary ({tertiaryLum:F1}) should be darker than primary ({primaryLum:F1})");
    }

    // ── Different primaries produce different dictionaries ───────────────────

    [TestMethod]
    public void DifferentPrimaryColors_ProduceDifferentPrimaryBrushes()
    {
        var blueDict = new DynamicThemeResourceDictionary(System.Windows.Media.Color.FromArgb(255, 0, 0, 255));
        var redDict = new DynamicThemeResourceDictionary(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));

        var blueBrush = (SolidColorBrush)blueDict["PrimaryBrush"];
        var redBrush = (SolidColorBrush)redDict["PrimaryBrush"];

        Assert.AreNotEqual(blueBrush.Color, redBrush.Color);
    }
}
