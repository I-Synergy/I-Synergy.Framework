using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Utilities;
using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Styles;

public sealed class DynamicThemeResourceDictionary : ResourceDictionary, IThemeResourceDictionary
{
    public DynamicThemeResourceDictionary(Color primary, Themes theme = Themes.Dark)
    {
        var (primaryRed, primaryGreen, primaryBlue) = (primary.R / 255f, primary.G / 255f, primary.B / 255f);

        var secondary = FromFloats(ThemeColorCalculator.CalculateSecondary(primaryRed, primaryGreen, primaryBlue));
        var tertiary  = FromFloats(ThemeColorCalculator.CalculateTertiary(primaryRed, primaryGreen, primaryBlue));
        var grays     = ThemeColorCalculator.CalculateTintedGrays(primaryRed, primaryGreen, primaryBlue);

        // Primary / Secondary / Tertiary colors and brushes
        this["Primary"] = primary;
        this["Secondary"] = secondary;
        this["Tertiary"] = tertiary;
        this["PrimaryBrush"] = new SolidColorBrush(primary);
        this["SecondaryBrush"] = new SolidColorBrush(secondary);
        this["TertiaryBrush"] = new SolidColorBrush(tertiary);

        // System-alias keys used by framework controls (spinner, splash) and by WinUI-compatible
        // resource dictionaries. Without them those DynamicResource lookups resolve to nothing and
        // the controls fall back to system/default colours.
        this["SystemAccentColor"] = primary;
        this["SystemAccentBrush"] = new SolidColorBrush(primary);

        // TintedGray colors and brushes
        foreach (var (key, (red, green, blue)) in grays)
        {
            this[$"TintedGray{key}"] = FromFloats(red, green, blue);
            this[$"TintedGray{key}Brush"] = new SolidColorBrush(FromFloats(red, green, blue));
        }

        // Semantic tinted brushes follow the selected theme. The dark palette uses the low
        // (dark) end of the gray ramp and the light palette the high (light) end, matching the
        // WinUI theme dictionaries so framework controls switch with the application theme.
        var isDark = theme != Themes.Light;
        Color GrayColor(string key) { var (red, green, blue) = grays[key]; return FromFloats(red, green, blue); }
        this["TintedBackgroundBrush"] = new SolidColorBrush(GrayColor(isDark ? "950" : "000"));
        this["TintedSurfaceBrush"] = new SolidColorBrush(GrayColor(isDark ? "900" : "100"));
        this["TintedControlBrush"] = new SolidColorBrush(GrayColor(isDark ? "900" : "100"));
        this["TintedDisabledBrush"] = new SolidColorBrush(GrayColor(isDark ? "600" : "200"));
        this["TintedBorderBrush"] = new SolidColorBrush(GrayColor(isDark ? "600" : "200"));
        this["TintedSecondaryBrush"] = new SolidColorBrush(GrayColor(isDark ? "500" : "300"));
    }

    private static Color FromFloats(float red, float green, float blue) =>
        Color.FromArgb(255,
            (byte)MathF.Round(red * 255f),
            (byte)MathF.Round(green * 255f),
            (byte)MathF.Round(blue * 255f));

    private static Color FromFloats((float Red, float Green, float Blue) t) =>
        FromFloats(t.Red, t.Green, t.Blue);
}
