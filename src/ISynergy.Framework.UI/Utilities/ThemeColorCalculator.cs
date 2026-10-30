namespace ISynergy.Framework.UI.Utilities;

public static class ThemeColorCalculator
{
    private static readonly string[] GrayKeys =
    [
        "000", "050", "100", "150", "200", "250", "300", "350",
        "400", "450", "500", "550", "600", "650", "700", "750",
        "800", "850", "900", "950"
    ];

    private const float TintedSaturationFactor = 0.10f;
    private const float LightestL = 245f / 255f; // "000"
    private const float DarkestL  =  10f / 255f; // "950"

    public static (float Red, float Green, float Blue) WhiteBlend(float red, float green, float blue, float factor) =>
        (red + (1f - red) * factor, green + (1f - green) * factor, blue + (1f - blue) * factor);

    public static (float Red, float Green, float Blue) BlackBlend(float red, float green, float blue, float factor) =>
        (red * (1f - factor), green * (1f - factor), blue * (1f - factor));

    public static (float Red, float Green, float Blue) CalculateSecondary(float red, float green, float blue) =>
        WhiteBlend(red, green, blue, 0.25f);

    public static (float Red, float Green, float Blue) CalculateTertiary(float red, float green, float blue) =>
        BlackBlend(red, green, blue, 0.25f);

    public static IReadOnlyDictionary<string, (float Red, float Green, float Blue)> CalculateTintedGrays(float red, float green, float blue)
    {
        var (hue, saturation, _) = RgbToHsl(red, green, blue);
        float tintedSaturation = saturation * TintedSaturationFactor;
        float stepL = (LightestL - DarkestL) / (GrayKeys.Length - 1);

        var result = new Dictionary<string, (float Red, float Green, float Blue)>(GrayKeys.Length);
        for (int i = 0; i < GrayKeys.Length; i++)
            result[GrayKeys[i]] = HslToRgb(hue, tintedSaturation, LightestL - i * stepL);
        return result;
    }

    private static (float Hue, float Saturation, float Lightness) RgbToHsl(float red, float green, float blue)
    {
        float max = MathF.Max(red, MathF.Max(green, blue));
        float min = MathF.Min(red, MathF.Min(green, blue));
        float delta = max - min;
        float lightness = (max + min) / 2f;
        float saturation = delta == 0f ? 0f : delta / (1f - MathF.Abs(2f * lightness - 1f));
        float hue;
        if (delta == 0f) hue = 0f;
        else if (red >= green && red >= blue) hue = 60f * (((green - blue) / delta) % 6f);
        else if (green >= blue) hue = 60f * ((blue - red) / delta + 2f);
        else hue = 60f * ((red - green) / delta + 4f);
        if (hue < 0f) hue += 360f;
        return (hue, saturation, lightness);
    }

    private static (float Red, float Green, float Blue) HslToRgb(float hue, float saturation, float lightness)
    {
        float chroma = (1f - MathF.Abs(2f * lightness - 1f)) * saturation;
        float intermediateChroma = chroma * (1f - MathF.Abs((hue / 60f) % 2f - 1f));
        float match = lightness - chroma / 2f;
        float redComponent, greenComponent, blueComponent;
        if (hue < 60f)       { redComponent = chroma;             greenComponent = intermediateChroma; blueComponent = 0f; }
        else if (hue < 120f) { redComponent = intermediateChroma; greenComponent = chroma;             blueComponent = 0f; }
        else if (hue < 180f) { redComponent = 0f;                 greenComponent = chroma;             blueComponent = intermediateChroma; }
        else if (hue < 240f) { redComponent = 0f;                 greenComponent = intermediateChroma; blueComponent = chroma; }
        else if (hue < 300f) { redComponent = intermediateChroma; greenComponent = 0f;                 blueComponent = chroma; }
        else                 { redComponent = chroma;             greenComponent = 0f;                 blueComponent = intermediateChroma; }
        return (redComponent + match, greenComponent + match, blueComponent + match);
    }
}
