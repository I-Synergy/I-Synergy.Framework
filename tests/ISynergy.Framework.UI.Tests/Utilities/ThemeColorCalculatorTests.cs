using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Utilities;

namespace ISynergy.Framework.UI.Utilities;

[TestClass]
public class ThemeColorCalculatorTests
{
    private const float Tolerance = 0.001f;

    // Windows blue #0078D7
    private const float WindowsBlueRed = 0f;
    private const float WindowsBlueGreen = 120f / 255f;
    private const float WindowsBlueBlue = 215f / 255f;

    private static float Luminosity(float red, float green, float blue) =>
        (MathF.Max(red, MathF.Max(green, blue)) + MathF.Min(red, MathF.Min(green, blue))) / 2f;

    // ── WhiteBlend ───────────────────────────────────────────────────────────

    [TestMethod]
    public void WhiteBlend_WithFactorZero_ReturnsOriginalColor()
    {
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(0.2f, 0.4f, 0.6f, 0f);

        Assert.AreEqual(0.2f, red, Tolerance);
        Assert.AreEqual(0.4f, green, Tolerance);
        Assert.AreEqual(0.6f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_WithFactorOne_ReturnsWhite()
    {
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(0.2f, 0.4f, 0.6f, 1f);

        Assert.AreEqual(1.0f, red, Tolerance);
        Assert.AreEqual(1.0f, green, Tolerance);
        Assert.AreEqual(1.0f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_BlackWithFactor025_ReturnsGray025()
    {
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(0f, 0f, 0f, 0.25f);

        Assert.AreEqual(0.25f, red, Tolerance);
        Assert.AreEqual(0.25f, green, Tolerance);
        Assert.AreEqual(0.25f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_WhiteWithAnyFactor_RemainsWhite()
    {
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(1f, 1f, 1f, 0.5f);

        Assert.AreEqual(1.0f, red, Tolerance);
        Assert.AreEqual(1.0f, green, Tolerance);
        Assert.AreEqual(1.0f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_ChannelsBlendIndependently()
    {
        // R=0, G=0.5, B=1 with factor=0.5 =>
        // R = 0 + (1-0)*0.5 = 0.5, G = 0.5 + (1-0.5)*0.5 = 0.75, B = 1 + (1-1)*0.5 = 1.0
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(0f, 0.5f, 1f, 0.5f);

        Assert.AreEqual(0.5f, red, Tolerance);
        Assert.AreEqual(0.75f, green, Tolerance);
        Assert.AreEqual(1.0f, blue, Tolerance);
    }

    [TestMethod]
    public void WhiteBlend_ProducesLighterColor()
    {
        var originalLum = Luminosity(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var (red, green, blue) = ThemeColorCalculator.WhiteBlend(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue, 0.25f);

        Assert.IsTrue(Luminosity(red, green, blue) > originalLum);
    }

    // ── BlackBlend ───────────────────────────────────────────────────────────

    [TestMethod]
    public void BlackBlend_WithFactorZero_ReturnsOriginalColor()
    {
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(0.2f, 0.4f, 0.6f, 0f);

        Assert.AreEqual(0.2f, red, Tolerance);
        Assert.AreEqual(0.4f, green, Tolerance);
        Assert.AreEqual(0.6f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_WithFactorOne_ReturnsBlack()
    {
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(0.2f, 0.4f, 0.6f, 1f);

        Assert.AreEqual(0.0f, red, Tolerance);
        Assert.AreEqual(0.0f, green, Tolerance);
        Assert.AreEqual(0.0f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_WhiteWithFactor025_ReturnsGray075()
    {
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(1f, 1f, 1f, 0.25f);

        Assert.AreEqual(0.75f, red, Tolerance);
        Assert.AreEqual(0.75f, green, Tolerance);
        Assert.AreEqual(0.75f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_BlackWithAnyFactor_RemainsBlack()
    {
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(0f, 0f, 0f, 0.5f);

        Assert.AreEqual(0.0f, red, Tolerance);
        Assert.AreEqual(0.0f, green, Tolerance);
        Assert.AreEqual(0.0f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_ChannelsScaleIndependently()
    {
        // R=0.8, G=0.4, B=0.2 with factor=0.5 => R=0.4, G=0.2, B=0.1
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(0.8f, 0.4f, 0.2f, 0.5f);

        Assert.AreEqual(0.4f, red, Tolerance);
        Assert.AreEqual(0.2f, green, Tolerance);
        Assert.AreEqual(0.1f, blue, Tolerance);
    }

    [TestMethod]
    public void BlackBlend_ProducesDarkerColor()
    {
        var originalLum = Luminosity(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var (red, green, blue) = ThemeColorCalculator.BlackBlend(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue, 0.25f);

        Assert.IsTrue(Luminosity(red, green, blue) < originalLum);
    }

    // ── CalculateSecondary ───────────────────────────────────────────────────

    [TestMethod]
    public void CalculateSecondary_IsEquivalentToWhiteBlend025()
    {
        var secondary = ThemeColorCalculator.CalculateSecondary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var whiteBlend = ThemeColorCalculator.WhiteBlend(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue, 0.25f);

        Assert.AreEqual(whiteBlend.Red, secondary.Red, Tolerance);
        Assert.AreEqual(whiteBlend.Green, secondary.Green, Tolerance);
        Assert.AreEqual(whiteBlend.Blue, secondary.Blue, Tolerance);
    }

    [TestMethod]
    public void CalculateSecondary_IsLighterThanPrimary()
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.IsTrue(Luminosity(secondaryRed, secondaryGreen, secondaryBlue) > Luminosity(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue));
    }

    [TestMethod]
    public void CalculateSecondary_Black_ReturnsGray025()
    {
        var (red, green, blue) = ThemeColorCalculator.CalculateSecondary(0f, 0f, 0f);

        Assert.AreEqual(0.25f, red, Tolerance);
        Assert.AreEqual(0.25f, green, Tolerance);
        Assert.AreEqual(0.25f, blue, Tolerance);
    }

    [TestMethod]
    public void CalculateSecondary_White_RemainsWhite()
    {
        var (red, green, blue) = ThemeColorCalculator.CalculateSecondary(1f, 1f, 1f);

        Assert.AreEqual(1.0f, red, Tolerance);
        Assert.AreEqual(1.0f, green, Tolerance);
        Assert.AreEqual(1.0f, blue, Tolerance);
    }

    [TestMethod]
    public void CalculateSecondary_IsDeterministic()
    {
        var first = ThemeColorCalculator.CalculateSecondary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var second = ThemeColorCalculator.CalculateSecondary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.AreEqual(first.Red, second.Red, Tolerance);
        Assert.AreEqual(first.Green, second.Green, Tolerance);
        Assert.AreEqual(first.Blue, second.Blue, Tolerance);
    }

    // ── CalculateTertiary ────────────────────────────────────────────────────

    [TestMethod]
    public void CalculateTertiary_IsEquivalentToBlackBlend025()
    {
        var tertiary = ThemeColorCalculator.CalculateTertiary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var blackBlend = ThemeColorCalculator.BlackBlend(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue, 0.25f);

        Assert.AreEqual(blackBlend.Red, tertiary.Red, Tolerance);
        Assert.AreEqual(blackBlend.Green, tertiary.Green, Tolerance);
        Assert.AreEqual(blackBlend.Blue, tertiary.Blue, Tolerance);
    }

    [TestMethod]
    public void CalculateTertiary_IsDarkerThanPrimary()
    {
        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.IsTrue(Luminosity(tertiaryRed, tertiaryGreen, tertiaryBlue) < Luminosity(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue));
    }

    [TestMethod]
    public void CalculateTertiary_White_ReturnsGray075()
    {
        var (red, green, blue) = ThemeColorCalculator.CalculateTertiary(1f, 1f, 1f);

        Assert.AreEqual(0.75f, red, Tolerance);
        Assert.AreEqual(0.75f, green, Tolerance);
        Assert.AreEqual(0.75f, blue, Tolerance);
    }

    [TestMethod]
    public void CalculateTertiary_Black_RemainsBlack()
    {
        var (red, green, blue) = ThemeColorCalculator.CalculateTertiary(0f, 0f, 0f);

        Assert.AreEqual(0.0f, red, Tolerance);
        Assert.AreEqual(0.0f, green, Tolerance);
        Assert.AreEqual(0.0f, blue, Tolerance);
    }

    [TestMethod]
    public void CalculateTertiary_IsDeterministic()
    {
        var first = ThemeColorCalculator.CalculateTertiary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var second = ThemeColorCalculator.CalculateTertiary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.AreEqual(first.Red, second.Red, Tolerance);
        Assert.AreEqual(first.Green, second.Green, Tolerance);
        Assert.AreEqual(first.Blue, second.Blue, Tolerance);
    }

    [TestMethod]
    public void Secondary_IsAlwaysLighterThanTertiary()
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.IsTrue(Luminosity(secondaryRed, secondaryGreen, secondaryBlue) > Luminosity(tertiaryRed, tertiaryGreen, tertiaryBlue));
    }

    // ── CalculateTintedGrays ─────────────────────────────────────────────────

    [TestMethod]
    public void CalculateTintedGrays_Returns11Entries()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        Assert.AreEqual(20, grays.Count);
    }

    [TestMethod]
    public void CalculateTintedGrays_ContainsAllExpectedKeys()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var expectedKeys = new[] { "000", "050", "100", "150", "200", "250", "300", "350", "400", "450", "500", "550", "600", "650", "700", "750", "800", "850", "900", "950" };

        foreach (var key in expectedKeys)
            Assert.IsTrue(grays.ContainsKey(key), $"Missing key: {key}");
    }

    [TestMethod]
    public void CalculateTintedGrays_Gray000IsLighterThanGray950()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var (r000, g000, b000) = grays["000"];
        var (r950, g950, b950) = grays["950"];

        Assert.IsTrue(Luminosity(r000, g000, b000) > Luminosity(r950, g950, b950));
    }

    [TestMethod]
    public void CalculateTintedGrays_ShadesAreMonotonicallyDarkening()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var keys = new[] { "000", "050", "100", "150", "200", "250", "300", "350", "400", "450", "500", "550", "600", "650", "700", "750", "800", "850", "900", "950" };
        float previousLuminosity = float.MaxValue;

        foreach (var key in keys)
        {
            var (red, green, blue) = grays[key];
            float luminosity = Luminosity(red, green, blue);
            Assert.IsTrue(luminosity <= previousLuminosity + Tolerance, $"Gray {key} ({luminosity:F4}) should not be lighter than previous ({previousLuminosity:F4})");
            previousLuminosity = luminosity;
        }
    }

    [TestMethod]
    public void CalculateTintedGrays_AllChannelsInValidRange()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        foreach (var (key, (red, green, blue)) in grays)
        {
            Assert.IsTrue(red is >= 0f and <= 1f, $"Key {key}: Red {red:F4} out of [0,1]");
            Assert.IsTrue(green is >= 0f and <= 1f, $"Key {key}: Green {green:F4} out of [0,1]");
            Assert.IsTrue(blue is >= 0f and <= 1f, $"Key {key}: Blue {blue:F4} out of [0,1]");
        }
    }

    [TestMethod]
    public void CalculateTintedGrays_BlackPrimary_ProducesNeutralGrays()
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(0f, 0f, 0f);

        foreach (var (key, (red, green, blue)) in grays)
        {
            float chroma = MathF.Max(red, MathF.Max(green, blue)) - MathF.Min(red, MathF.Min(green, blue));
            Assert.IsTrue(chroma < 0.01f, $"Key {key}: chroma ({chroma:F4}) should be near zero for black primary");
        }
    }

    [TestMethod]
    public void CalculateTintedGrays_DifferentPrimaryHues_ProduceDifferentTints()
    {
        var blueGrays = ThemeColorCalculator.CalculateTintedGrays(0f, 0f, 1f);
        var redGrays = ThemeColorCalculator.CalculateTintedGrays(1f, 0f, 0f);
        var (blueR, _, _) = blueGrays["500"];
        var (redR, _, _) = redGrays["500"];

        Assert.AreNotEqual(blueR, redR, Tolerance, "Blue and red primaries should produce different gray tints");
    }

    [TestMethod]
    public void CalculateTintedGrays_IsDeterministic()
    {
        var first = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);
        var second = ThemeColorCalculator.CalculateTintedGrays(WindowsBlueRed, WindowsBlueGreen, WindowsBlueBlue);

        foreach (var key in first.Keys)
        {
            var (r1, g1, b1) = first[key];
            var (r2, g2, b2) = second[key];
            Assert.AreEqual(r1, r2, Tolerance, $"Key {key}: Red mismatch");
            Assert.AreEqual(g1, g2, Tolerance, $"Key {key}: Green mismatch");
            Assert.AreEqual(b1, b2, Tolerance, $"Key {key}: Blue mismatch");
        }
    }

    // ── HTML Color Preview ────────────────────────────────────────────────────

    [TestMethod]
    public void GenerateColorPreviewHtml()
    {
        var palettes = ThemeColors.Colors
            .Select(hex =>
            {
                var (red, green, blue) = ParseHex(hex);
                return (Name: hex.ToUpperInvariant(), Red: red, Green: green, Blue: blue);
            })
            .ToList();

        var html = BuildPreviewHtml(palettes, navGroupSize: 7);
        var outputPath = Path.Combine(FindSolutionRoot(), "theme-color-preview.html");
        File.WriteAllText(outputPath, html);
        Console.WriteLine($"[ThemeColorPreview] {outputPath}");
        Assert.IsTrue(File.Exists(outputPath));
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null &&
               !dir.GetFiles("*.sln").Any() &&
               !dir.GetFiles("*.slnx").Any())
            dir = dir.Parent;
        return dir?.FullName ?? AppContext.BaseDirectory;
    }

    private static (float Red, float Green, float Blue) ParseHex(string hex)
    {
        var h = hex.TrimStart('#');
        return (
            Convert.ToByte(h[0..2], 16) / 255f,
            Convert.ToByte(h[2..4], 16) / 255f,
            Convert.ToByte(h[4..6], 16) / 255f
        );
    }

    private static string BuildPreviewHtml(
        IList<(string Name, float Red, float Green, float Blue)> palettes,
        int navGroupSize = 0)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(HtmlPageHeader());
        sb.Append(BuildNavBar(palettes, navGroupSize));
        foreach (var (name, red, green, blue) in palettes)
            sb.Append(BuildPaletteSection(name, red, green, blue));
        sb.Append("</main>\n</body>\n</html>");
        return sb.ToString();
    }

    private static string BuildNavBar(
        IList<(string Name, float Red, float Green, float Blue)> palettes,
        int groupSize)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("<nav id=\"color-nav\">\n<span class=\"nav-title\">Colors</span>\n");

        if (groupSize > 0)
        {
            for (int i = 0; i < palettes.Count; i += groupSize)
            {
                sb.Append("<div class=\"nav-group\">\n");
                int end = Math.Min(i + groupSize, palettes.Count);
                for (int j = i; j < end; j++)
                {
                    var (name, red, green, blue) = palettes[j];
                    var pid = name.TrimStart('#').ToLowerInvariant();
                    var hex = RgbToHex(red, green, blue);
                    sb.Append($"<div class=\"nav-dot\" style=\"background:{hex}\" onclick=\"go('{pid}')\" title=\"{name}\"></div>\n");
                }
                sb.Append("</div>\n");
            }
        }
        else
        {
            foreach (var (name, red, green, blue) in palettes)
            {
                var pid = name.TrimStart('#').ToLowerInvariant();
                var hex = RgbToHex(red, green, blue);
                sb.Append($"<div class=\"nav-dot\" style=\"background:{hex}\" onclick=\"go('{pid}')\" title=\"{name}\"></div>\n");
            }
        }

        sb.Append("</nav>\n");
        return sb.ToString();
    }

    private static string HtmlPageHeader() =>
        """
        <!DOCTYPE html>
        <html lang="en">
        <head>
        <meta charset="utf-8">
        <title>Theme Color Preview — I-Synergy Framework</title>
        <style>
          *{box-sizing:border-box;margin:0;padding:0;}
          body{font-family:-apple-system,BlinkMacSystemFont,"Segoe UI Variable","Segoe UI",sans-serif;background:#111;color:#fff;padding:32px;}
          h1{font-size:20px;font-weight:600;color:#e0e0e0;letter-spacing:-0.02em;margin:0;}
          h2{font-size:15px;font-weight:600;margin:0 0 18px;}
          h3{font-size:9px;font-weight:700;text-transform:uppercase;letter-spacing:.1em;color:#555;margin:0 0 7px;}
          #color-nav{
            position:sticky;top:0;z-index:100;
            background:rgba(17,17,17,.97);backdrop-filter:blur(10px);
            padding:12px 32px;margin:-32px -32px 32px;
            border-bottom:1px solid #242424;
            display:flex;flex-wrap:wrap;gap:8px;align-items:center;
          }
          .nav-title{font-size:11px;font-weight:700;color:#555;margin-right:4px;white-space:nowrap;}
          .nav-group{display:flex;gap:4px;}
          .nav-dot{
            width:22px;height:22px;border-radius:5px;cursor:pointer;
            transition:transform .12s,box-shadow .12s;
            border:1px solid rgba(255,255,255,.07);
          }
          .nav-dot:hover{transform:scale(1.35);box-shadow:0 3px 10px rgba(0,0,0,.5);}
          .palette{background:#1a1a1a;border-radius:14px;padding:26px;margin-bottom:24px;border:1px solid #242424;scroll-margin-top:62px;}
          .row{display:flex;gap:5px;margin-bottom:18px;flex-wrap:wrap;}
          .swatch{min-width:52px;flex:1;height:64px;border-radius:8px;display:flex;flex-direction:column;
                  justify-content:flex-end;padding:5px 7px;font-size:9px;font-weight:600;cursor:default;
                  transition:transform .1s;}
          .swatch:hover{transform:scale(1.06) translateY(-2px);}
          .swatch .name{opacity:.9;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}
          .swatch .hex{opacity:.55;font-size:8px;margin-top:2px;font-family:monospace;}
          .themes{display:grid;grid-template-columns:1fr 1fr;gap:10px;}
          .theme-panel{border-radius:10px;padding:18px;}
          .theme-panel .row{margin-bottom:0;}
          .theme-panel .swatch{height:52px;min-width:40px;}
        </style>
        <script>function go(id){document.getElementById(id).scrollIntoView({behavior:'smooth',block:'start'});}</script>
        </head>
        <body>
        <main>
        """;

    private static string BuildPaletteSection(string name, float red, float green, float blue)
    {
        var grays = ThemeColorCalculator.CalculateTintedGrays(red, green, blue);
        var secondary = ThemeColorCalculator.CalculateSecondary(red, green, blue);
        var tertiary = ThemeColorCalculator.CalculateTertiary(red, green, blue);
        var pid = name.TrimStart('#').ToLowerInvariant();
        var hex = RgbToHex(red, green, blue);

        var sb = new System.Text.StringBuilder();
        sb.Append($"<section class=\"palette\" id=\"{pid}\" data-palette=\"{pid}\">\n");
        sb.Append($"<h2 style=\"color:{hex}\">{name}</h2>\n");

        // Primary palette
        sb.Append("<h3>Primary Palette</h3>\n<div class=\"row\">\n");
        sb.Append(BuildSwatch(pid, "tertiary", "Tertiary", tertiary.Red, tertiary.Green, tertiary.Blue));
        sb.Append(BuildSwatch(pid, "primary", "Primary", red, green, blue));
        sb.Append(BuildSwatch(pid, "secondary", "Secondary", secondary.Red, secondary.Green, secondary.Blue));
        sb.Append("</div>\n");

        // White blend scale
        sb.Append("<h3>White Blend → +10% steps</h3>\n<div class=\"row\">\n");
        for (int i = 1; i <= 9; i++)
        {
            var (wr, wg, wb) = ThemeColorCalculator.WhiteBlend(red, green, blue, i / 10f);
            sb.Append(BuildSwatch(pid, $"white-blend-{i * 10}", $"+{i * 10}%", wr, wg, wb));
        }
        sb.Append("</div>\n");

        // Black blend scale
        sb.Append("<h3>Black Blend → -10% steps</h3>\n<div class=\"row\">\n");
        for (int i = 1; i <= 9; i++)
        {
            var (br, bg2, bb) = ThemeColorCalculator.BlackBlend(red, green, blue, i / 10f);
            sb.Append(BuildSwatch(pid, $"black-blend-{i * 10}", $"-{i * 10}%", br, bg2, bb));
        }
        sb.Append("</div>\n");

        // Tinted gray scale
        sb.Append("<h3>Tinted Gray Scale</h3>\n<div class=\"row\">\n");
        foreach (var (key, (gr, gg, gb)) in grays)
            sb.Append(BuildSwatch(pid, $"tinted-gray-{key}", key, gr, gg, gb));
        sb.Append("</div>\n");

        // Semantic swatches — light + dark side by side
        sb.Append("<h3>Semantic Brushes</h3>\n<div class=\"themes\">\n");
        sb.Append(BuildSemanticPanel(pid, "Light Theme", red, green, blue, grays, isDark: false));
        sb.Append(BuildSemanticPanel(pid, "Dark Theme", red, green, blue, grays, isDark: true));
        sb.Append("</div>\n");

        sb.Append("</section>\n");
        return sb.ToString();
    }

    private static string BuildSemanticPanel(
        string paletteId, string label,
        float primaryRed, float primaryGreen, float primaryBlue,
        IReadOnlyDictionary<string, (float Red, float Green, float Blue)> grays,
        bool isDark)
    {
        var (bgR, bgG, bgB) = grays[isDark ? "950" : "000"];
        var theme = isDark ? "dark" : "light";

        // Keys mirror DynamicThemeResourceDictionary semantics
        var semantics = new (string Key, string Label, (float Red, float Green, float Blue) Color)[]
        {
            ("tinted-background", "Background", grays[isDark ? "950" : "000"]),
            ("tinted-surface",    "Surface",    grays[isDark ? "900" : "100"]),
            ("tinted-control",    "Control",    grays[isDark ? "800" : "200"]),
            ("tinted-disabled",   "Disabled",   grays[isDark ? "600" : "400"]),
            ("tinted-border",     "Border",     grays[isDark ? "600" : "200"]),
            ("tinted-secondary",  "Secondary",  grays[isDark ? "500" : "300"]),
            ("primary-accent",    "Accent",     (primaryRed, primaryGreen, primaryBlue)),
        };

        var sb = new System.Text.StringBuilder();
        sb.Append($"<div class=\"theme-panel\" data-theme=\"{theme}\" style=\"background:{RgbToHex(bgR, bgG, bgB)}\">\n");
        sb.Append($"<h3>{label}</h3>\n<div class=\"row\">\n");
        foreach (var (key, lbl, (r, g, b)) in semantics)
            sb.Append(BuildSwatch(paletteId, key, lbl, r, g, b, theme));
        sb.Append("</div>\n</div>\n");
        return sb.ToString();
    }

    private static string BuildSwatch(
        string paletteId, string key, string label,
        float red, float green, float blue, string? theme = null)
    {
        var hex = RgbToHex(red, green, blue);
        var textColor = Luminosity(red, green, blue) > 0.45f ? "#000" : "#fff";
        var themeAttr = theme is null ? "" : $" data-theme=\"{theme}\"";
        return $"<div class=\"swatch\" data-palette=\"{paletteId}\" data-key=\"{key}\" data-hex=\"{hex}\"{themeAttr} style=\"background:{hex};color:{textColor}\" title=\"{key}\"><span class=\"name\">{label}</span><span class=\"hex\">{hex}</span></div>\n";
    }

    private static string RgbToHex(float red, float green, float blue)
    {
        var r = (byte)MathF.Round(Math.Clamp(red, 0f, 1f) * 255f);
        var g = (byte)MathF.Round(Math.Clamp(green, 0f, 1f) * 255f);
        var b = (byte)MathF.Round(Math.Clamp(blue, 0f, 1f) * 255f);
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
