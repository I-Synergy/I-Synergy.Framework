using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Utilities;

namespace ISynergy.Framework.UI.Styles;

public sealed class DynamicThemeResourceDictionary : ResourceDictionary, IThemeResourceDictionary
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

        foreach (var (key, (red, green, blue)) in grays)
        {
            var color = Color.FromRgba(red, green, blue, 1f);
            this[$"TintedGray{key}"]      = color;
            this[$"TintedGray{key}Brush"] = new SolidColorBrush(color);
        }
    }
}
