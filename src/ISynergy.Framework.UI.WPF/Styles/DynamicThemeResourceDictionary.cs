using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Utilities;
using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Styles;

public sealed class DynamicThemeResourceDictionary : ResourceDictionary, IThemeResourceDictionary
{
    public DynamicThemeResourceDictionary(Color primary)
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

        // TintedGray colors and brushes
        foreach (var (key, (red, green, blue)) in grays)
        {
            this[$"TintedGray{key}"] = FromFloats(red, green, blue);
            this[$"TintedGray{key}Brush"] = new SolidColorBrush(FromFloats(red, green, blue));
        }

        // Semantic tinted brushes (assumes dark theme as default; apps toggle via LocalSettings)
        Color GrayColor(string key) { var (red, green, blue) = grays[key]; return FromFloats(red, green, blue); }
        this["TintedBackgroundBrush"] = new SolidColorBrush(GrayColor("950"));
        this["TintedSurfaceBrush"] = new SolidColorBrush(GrayColor("900"));
        this["TintedControlBrush"] = new SolidColorBrush(GrayColor("900"));
        this["TintedDisabledBrush"] = new SolidColorBrush(GrayColor("600"));
        this["TintedBorderBrush"] = new SolidColorBrush(GrayColor("600"));
        this["TintedSecondaryBrush"] = new SolidColorBrush(GrayColor("500"));
    }

    private static Color FromFloats(float red, float green, float blue) =>
        Color.FromArgb(255,
            (byte)MathF.Round(red * 255f),
            (byte)MathF.Round(green * 255f),
            (byte)MathF.Round(blue * 255f));

    private static Color FromFloats((float Red, float Green, float Blue) t) =>
        FromFloats(t.Red, t.Green, t.Blue);
}
