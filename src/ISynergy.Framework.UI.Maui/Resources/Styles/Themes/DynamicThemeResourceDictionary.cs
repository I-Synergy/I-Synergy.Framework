using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Utilities;

namespace ISynergy.Framework.UI.Styles;

public sealed partial class DynamicThemeResourceDictionary : ResourceDictionary, IThemeResourceDictionary
{
    public DynamicThemeResourceDictionary(Color primary)
    {
        var (secondaryRed, secondaryGreen, secondaryBlue) = ThemeColorCalculator.CalculateSecondary(primary.Red, primary.Green, primary.Blue);
        var (tertiaryRed, tertiaryGreen, tertiaryBlue) = ThemeColorCalculator.CalculateTertiary(primary.Red, primary.Green, primary.Blue);
        var grays = ThemeColorCalculator.CalculateTintedGrays(primary.Red, primary.Green, primary.Blue);

        var secondary = Color.FromRgba(secondaryRed, secondaryGreen, secondaryBlue, 1f);
        var tertiary  = Color.FromRgba(tertiaryRed, tertiaryGreen, tertiaryBlue, 1f);

        this["Primary"]        = primary;
        this["PrimaryBrush"]   = new SolidColorBrush(primary);
        this["Secondary"]      = secondary;
        this["SecondaryBrush"] = new SolidColorBrush(secondary);
        this["Tertiary"]       = tertiary;
        this["TertiaryBrush"]  = new SolidColorBrush(tertiary);

        // Foregrounds for text/icons drawn on the accent surfaces. A fixed white fails WCAG AA on
        // the lighter accent steps (Secondary is 25% white), so the palette decides per accent.
        this["PrimaryForeground"] = ToColor(ThemeColorCalculator.CalculateOnColor(primary.Red, primary.Green, primary.Blue));
        this["SecondaryForeground"] = ToColor(ThemeColorCalculator.CalculateOnColor(secondaryRed, secondaryGreen, secondaryBlue));
        this["TertiaryForeground"] = ToColor(ThemeColorCalculator.CalculateOnColor(tertiaryRed, tertiaryGreen, tertiaryBlue));

        foreach (var (key, (red, green, blue)) in grays)
        {
            var color = Color.FromRgba(red, green, blue, 1f);
            this[$"TintedGray{key}"]      = color;
            this[$"TintedGray{key}Brush"] = new SolidColorBrush(color);
        }

        Color GrayColor(string key) { var (r, g, b) = grays[key]; return Color.FromRgba(r, g, b, 1f); }
        static Color ToColor((float Red, float Green, float Blue) value) => Color.FromRgba(value.Red, value.Green, value.Blue, 1f);

        this["TintedHoverBrush"] = new SolidColorBrush(GrayColor("850"));
    }
}
