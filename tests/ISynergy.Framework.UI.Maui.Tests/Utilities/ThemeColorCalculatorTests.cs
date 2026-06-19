using Microsoft.Maui.Graphics;
using ISynergy.Framework.UI.Utilities;

namespace ISynergy.Framework.UI.Maui;

[TestClass]
public class ThemeColorCalculatorTests
{
    private const float Tolerance = 0.005f;

    private static float Luminosity(float red, float green, float blue) =>
        (MathF.Max(red, MathF.Max(green, blue)) + MathF.Min(red, MathF.Min(green, blue))) / 2f;

    private static float Saturation(float red, float green, float blue)
    {
        float max = MathF.Max(red, MathF.Max(green, blue));
        float min = MathF.Min(red, MathF.Min(green, blue));
        float delta = max - min;
        float lightness = (max + min) / 2f;
        return delta == 0f ? 0f : delta / (1f - MathF.Abs(2f * lightness - 1f));
    }

    private static float Hue(float red, float green, float blue)
    {
        float max = MathF.Max(red, MathF.Max(green, blue));
        float min = MathF.Min(red, MathF.Min(green, blue));
        float delta = max - min;
        if (delta == 0f) return 0f;
        float hue;
        if (max == red)       hue = 60f * (((green - blue) / delta) % 6f);
        else if (max == green) hue = 60f * ((blue - red) / delta + 2f);
        else                   hue = 60f * ((red - green) / delta + 4f);
        if (hue < 0f) hue += 360f;
        return hue;
    }

    // ── WhiteBlend ───────────────────────────────────────────────────────────

    [TestMethod]
    public void WhiteBlend_WithFactorZero_ReturnsOriginalColor()
    {
        var primary = Color.FromArgb("#0078D7");

        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(primary.Red, primary.Green, primary.Blue, 0f);

        Assert.AreEqual(primary.Red, red, Tolerance);
        Assert.AreEqual(primary.Green, green, Tolerance);
        Assert.AreEqual(primary.Blue, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_WithFactorOne_ReturnsWhite()
    {
        var primary = Color.FromArgb("#0078D7");

        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(primary.Red, primary.Green, primary.Blue, 1f);

        Assert.AreEqual(1.0f, red, Tolerance);
        Assert.AreEqual(1.0f, green, Tolerance);
        Assert.AreEqual(1.0f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_ProducesLighterColor()
    {
        var primary = Color.FromArgb("#0078D7");
        var originalLum = Luminosity(primary.Red, primary.Green, primary.Blue);

        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(primary.Red, primary.Green, primary.Blue, 0.25f);

        Assert.IsTrue(Luminosity(red, green, blue) > originalLum);
    }

    // ── BlackBlend ───────────────────────────────────────────────────────────

    [TestMethod]
    public void BlackBlend_WithFactorZero_ReturnsOriginalColor()
    {
        var primary = Color.FromArgb("#0078D7");

        var (red, green, blue) = ThemeColorCalculator.BlackBlend(primary.Red, primary.Green, primary.Blue, 0f);

        Assert.AreEqual(primary.Red, red, Tolerance);
        Assert.AreEqual(primary.Green, green, Tolerance);
        Assert.AreEqual(primary.Blue, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_WithFactorOne_ReturnsBlack()
    {
        var primary = Color.FromArgb("#0078D7");

        var (red, green, blue) = ThemeColorCalculator.BlackBlend(primary.Red, primary.Green, primary.Blue, 1f);

        Assert.AreEqual(0.0f, red, Tolerance);
        Assert.AreEqual(0.0f, green, Tolerance);
        Assert.AreEqual(0.0f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_ProducesDarkerColor()
    {
        var primary = Color.FromArgb("#0078D7");
        var originalLum = Luminosity(primary.Red, primary.Green, primary.Blue);

        var (red, green, blue) = ThemeColorCalculator.BlackBlend(primary.Red, primary.Green, primary.Blue, 0.25f);

        Assert.IsTrue(Luminosity(red, green, blue) < originalLum);
    }

    // ── CalculateSecondary ───────────────────────────────────────────────────

    [TestMethod]
    public void PureBlue_Secondary_IsLighterTowardWhite()
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(0f, 0f, 1f);

        // R = 0 + (1-0)*0.25 = 0.25, G = 0.25, B = 1.0
        Assert.AreEqual(0.25f, secondaryRed, Tolerance, "Red should be 0.25");
        Assert.AreEqual(0.25f, secondaryGreen, Tolerance, "Green should be 0.25");
        Assert.AreEqual(1.0f, secondaryBlue, Tolerance, "Blue should remain 1.0");
    }

    [TestMethod]
    public void PureBlack_Secondary_IsGray25Percent()
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(0f, 0f, 0f);

        Assert.AreEqual(0.25f, secondaryRed, Tolerance, "Red should be 0.25");
        Assert.AreEqual(0.25f, secondaryGreen, Tolerance, "Green should be 0.25");
        Assert.AreEqual(0.25f, secondaryBlue, Tolerance, "Blue should be 0.25");
    }

    [TestMethod]
    public void PureWhite_Secondary_IsStillWhite()
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(1f, 1f, 1f);

        Assert.AreEqual(1.0f, secondaryRed, Tolerance, "Red should be 1.0");
        Assert.AreEqual(1.0f, secondaryGreen, Tolerance, "Green should be 1.0");
        Assert.AreEqual(1.0f, secondaryBlue, Tolerance, "Blue should be 1.0");
    }

    [TestMethod]
    public void Yellow_Secondary_IsLighter()
    {
        var primary = Color.FromArgb("#FFB900");

        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(primary.Red, primary.Green, primary.Blue);

        Assert.AreEqual(1.0f, secondaryRed, Tolerance, "Red should be 1.0");
        Assert.IsTrue(secondaryGreen > primary.Green, "Green should increase");
        Assert.AreEqual(0.25f, secondaryBlue, Tolerance, "Blue should be 0.25");
    }

    // ── CalculateTertiary ────────────────────────────────────────────────────

    [TestMethod]
    public void PureWhite_Tertiary_IsDarkened()
    {
        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(1f, 1f, 1f);

        Assert.AreEqual(0.75f, tertiaryRed, Tolerance, "Red should be 0.75");
        Assert.AreEqual(0.75f, tertiaryGreen, Tolerance, "Green should be 0.75");
        Assert.AreEqual(0.75f, tertiaryBlue, Tolerance, "Blue should be 0.75");
    }

    [TestMethod]
    public void PureBlack_Tertiary_IsStillBlack()
    {
        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(0f, 0f, 0f);

        Assert.AreEqual(0.0f, tertiaryRed, Tolerance, "Red should be 0");
        Assert.AreEqual(0.0f, tertiaryGreen, Tolerance, "Green should be 0");
        Assert.AreEqual(0.0f, tertiaryBlue, Tolerance, "Blue should be 0");
    }

    [TestMethod]
    public void Blue_Tertiary_IsDarker()
    {
        var primary = Color.FromArgb("#0078D7");

        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(primary.Red, primary.Green, primary.Blue);

        Assert.IsTrue(tertiaryRed <= primary.Red + Tolerance, "Red should not increase");
        Assert.IsTrue(tertiaryGreen < primary.Green, "Green should decrease");
        Assert.IsTrue(tertiaryBlue < primary.Blue, "Blue should decrease");
    }

    // ── CalculateTintedGrays ─────────────────────────────────────────────────

    [TestMethod]
    public void TintedGrays_Returns11Keys()
    {
        var primary = Color.FromArgb("#0000FF");

        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);

        Assert.AreEqual(11, grays.Count, "Should have exactly 11 gray shades");
    }

    [TestMethod]
    public void TintedGrays_HasAllExpectedKeys()
    {
        var primary = Color.FromArgb("#0000FF");
        var expectedKeys = new[] { "000", "100", "200", "300", "400", "500", "600", "700", "800", "900", "950" };

        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);

        foreach (var key in expectedKeys)
            Assert.IsTrue(grays.ContainsKey(key), $"Key '{key}' should be present in tinted grays");
    }

    [TestMethod]
    public void TintedGrays_Gray000_IsLightest_Gray950_IsDarkest()
    {
        var primary = Color.FromArgb("#0078D7");

        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);

        var (r000, g000, b000) = grays["000"];
        var (r950, g950, b950) = grays["950"];
        Assert.IsTrue(Luminosity(r000, g000, b000) > Luminosity(r950, g950, b950), "Gray 000 should be lighter than Gray 950");
    }

    [TestMethod]
    public void TintedGrays_ReflectsPrimaryHue()
    {
        var primary = Color.FromArgb("#0078D7");

        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);
        var (r000, g000, b000) = grays["000"];

        var primaryHue = Hue(primary.Red, primary.Green, primary.Blue);
        var gray000Hue = Hue(r000, g000, b000);

        Assert.IsTrue(
            Math.Abs(gray000Hue - primaryHue) < 5f,
            $"Gray 000 hue ({gray000Hue:F2}) should be close to primary hue ({primaryHue:F2})");
    }

    [TestMethod]
    public void TintedGrays_BlackPrimary_AreNeutralGrays()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(0f, 0f, 0f);

        foreach (var (key, (red, green, blue)) in grays)
        {
            Assert.IsTrue(
                Saturation(red, green, blue) <= 0.01f,
                $"Gray '{key}' saturation should be near zero for black primary");
        }
    }

    // ── Secondary is lighter, Tertiary is darker ─────────────────────────────

    [TestMethod]
    public void Secondary_IsLighterThanPrimary()
    {
        var primary = Color.FromArgb("#0078D7");

        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(primary.Red, primary.Green, primary.Blue);

        Assert.IsTrue(
            Luminosity(secondaryRed, secondaryGreen, secondaryBlue) > Luminosity(primary.Red, primary.Green, primary.Blue),
            "Secondary should be lighter than primary");
    }

    [TestMethod]
    public void Tertiary_IsDarkerThanPrimary()
    {
        var primary = Color.FromArgb("#0078D7");

        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(primary.Red, primary.Green, primary.Blue);

        Assert.IsTrue(
            Luminosity(tertiaryRed, tertiaryGreen, tertiaryBlue) < Luminosity(primary.Red, primary.Green, primary.Blue),
            "Tertiary should be darker than primary");
    }

    [TestMethod]
    public void CalculateSecondary_IsDeterministic()
    {
        var primary = Color.FromArgb("#0078D7");

        var first = ThemeColorCalculator.CalculateSecondary(primary.Red, primary.Green, primary.Blue);
        var second = ThemeColorCalculator.CalculateSecondary(primary.Red, primary.Green, primary.Blue);

        Assert.AreEqual(first.Red, second.Red, Tolerance);
        Assert.AreEqual(first.Green, second.Green, Tolerance);
        Assert.AreEqual(first.Blue, second.Blue, Tolerance);
    }

    [TestMethod]
    public void CalculateTertiary_IsDeterministic()
    {
        var primary = Color.FromArgb("#0078D7");

        var first = ThemeColorCalculator.CalculateTertiary(primary.Red, primary.Green, primary.Blue);
        var second = ThemeColorCalculator.CalculateTertiary(primary.Red, primary.Green, primary.Blue);

        Assert.AreEqual(first.Red, second.Red, Tolerance);
        Assert.AreEqual(first.Green, second.Green, Tolerance);
        Assert.AreEqual(first.Blue, second.Blue, Tolerance);
    }

    [TestMethod]
    public void CalculateTintedGrays_IncludesAllKeys()
    {
        var primary = Color.FromArgb("#0078D7");

        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);

        Assert.IsNotNull(grays);
        Assert.AreEqual(11, grays.Count);
        Assert.IsTrue(grays.ContainsKey("000"));
        Assert.IsTrue(grays.ContainsKey("950"));
    }
}
