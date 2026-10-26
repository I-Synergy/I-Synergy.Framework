using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ISynergy.Framework.UI.Styles;

public sealed class DynamicThemeResourceDictionary : ResourceDictionary, IThemeResourceDictionary
{
    public DynamicThemeResourceDictionary(Color primary)
    {
        var (primaryRed, primaryGreen, primaryBlue) = ToFloats(primary);

        var secondary = FromFloats(ThemeColorCalculator.CalculateSecondary(primaryRed, primaryGreen, primaryBlue));
        var tertiary = FromFloats(ThemeColorCalculator.CalculateTertiary(primaryRed, primaryGreen, primaryBlue));
        var grays = ThemeColorCalculator.CalculateTintedGrays(primaryRed, primaryGreen, primaryBlue);
        var accentDark1 = FromFloats(ThemeColorCalculator.WhiteBlend(primaryRed, primaryGreen, primaryBlue, 0.12f));
        var accentDark2 = FromFloats(ThemeColorCalculator.WhiteBlend(primaryRed, primaryGreen, primaryBlue, 0.24f));
        var accentDark3 = FromFloats(ThemeColorCalculator.WhiteBlend(primaryRed, primaryGreen, primaryBlue, 0.36f));
        var accentLight1 = FromFloats(ThemeColorCalculator.BlackBlend(primaryRed, primaryGreen, primaryBlue, 0.08f));
        var accentLight2 = FromFloats(ThemeColorCalculator.BlackBlend(primaryRed, primaryGreen, primaryBlue, 0.16f));
        var accentLight3 = FromFloats(ThemeColorCalculator.BlackBlend(primaryRed, primaryGreen, primaryBlue, 0.25f));

        // Root-level TintedGray colors and brushes (theme-invariant)
        foreach (var (key, (red, green, blue)) in grays)
        {
            this[$"TintedGray{key}"] = FromFloats(red, green, blue);
            this[$"TintedGray{key}Brush"] = new SolidColorBrush(FromFloats(red, green, blue));
        }

        // Root-level Primary/Secondary/Tertiary
        this["Primary"] = primary;
        this["Secondary"] = secondary;
        this["Tertiary"] = tertiary;
        this["PrimaryBrush"] = new SolidColorBrush(primary);
        this["SecondaryBrush"] = new SolidColorBrush(secondary);
        this["TertiaryBrush"] = new SolidColorBrush(tertiary);
        this["SystemAccentBrush"] = new SolidColorBrush(primary);

        // ThemeDictionaries
        ThemeDictionaries["Default"] = BuildDefaultTheme(primary, accentDark1, accentDark2, accentDark3, accentLight1, accentLight2, accentLight3, grays, isDark: true);
        ThemeDictionaries["Light"] = BuildDefaultTheme(primary, accentLight1, accentLight2, accentLight3, accentDark1, accentDark2, accentDark3, grays, isDark: false);
        ThemeDictionaries["HighContrast"] = new ResourceDictionary();
    }

    private static Color FromFloats(float red, float green, float blue) =>
        Color.FromArgb(255,
            (byte)MathF.Round(red * 255f),
            (byte)MathF.Round(green * 255f),
            (byte)MathF.Round(blue * 255f));

    private static Color FromFloats((float Red, float Green, float Blue) t) =>
        FromFloats(t.Red, t.Green, t.Blue);

    private static (float Red, float Green, float Blue) ToFloats(Color color) =>
        (color.R / 255f, color.G / 255f, color.B / 255f);

    private static ResourceDictionary BuildDefaultTheme(
        Color accent,
        Color variant1, Color variant2, Color variant3,
        Color altVariant1, Color altVariant2, Color altVariant3,
        IReadOnlyDictionary<string, (float Red, float Green, float Blue)> grays,
        bool isDark)
    {
        Color GrayColor(string key) { var (red, green, blue) = grays[key]; return FromFloats(red, green, blue); }

        // ColorPaletteResources for WinUI native controls
        var palette = isDark
            ? CreateDarkPalette(accent)
            : CreateLightPalette(accent);

        // Inner ResourceDictionary with system accent color overrides and semantic brushes
        var inner = new ResourceDictionary();
        inner["SystemAccentColor"] = accent;
        inner["SystemAccentColorDark1"] = isDark ? variant1 : altVariant1;
        inner["SystemAccentColorDark2"] = isDark ? variant2 : altVariant2;
        inner["SystemAccentColorDark3"] = isDark ? variant3 : altVariant3;
        inner["SystemAccentColorLight1"] = isDark ? altVariant1 : variant1;
        inner["SystemAccentColorLight2"] = isDark ? altVariant2 : variant2;
        inner["SystemAccentColorLight3"] = isDark ? altVariant3 : variant3;
        inner["RegionColor"] = isDark ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
        inner["RegionBrush"] = new SolidColorBrush(isDark ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255));
        // Semantic tinted brushes (dark: uses dark grays, light: uses light grays)
        inner["TintedBackgroundBrush"] = new SolidColorBrush(isDark ? GrayColor("950") : GrayColor("000"));
        inner["TintedSurfaceBrush"] = new SolidColorBrush(isDark ? GrayColor("900") : GrayColor("100"));
        inner["TintedControlBrush"] = new SolidColorBrush(isDark ? GrayColor("900") : GrayColor("100"));
        inner["TintedHoverBrush"] = new SolidColorBrush(isDark ? GrayColor("850") : GrayColor("150"));
        inner["TintedDisabledBrush"] = new SolidColorBrush(isDark ? GrayColor("600") : GrayColor("200"));
        inner["TintedBorderBrush"] = new SolidColorBrush(isDark ? GrayColor("600") : GrayColor("200"));
        inner["TintedSecondaryBrush"] = new SolidColorBrush(isDark ? GrayColor("500") : GrayColor("300"));

        // Shared sample design tokens (match Sample.Maui).
        inner["AppBackgroundBrush"] = inner["TintedBackgroundBrush"];
        inner["AppSurfaceBrush"] = inner["TintedSurfaceBrush"];
        inner["AppSurface2Brush"] = new SolidColorBrush(isDark
            ? Color.FromArgb(255, 0x1C, 0x2A, 0x25)
            : Color.FromArgb(255, 0xE8, 0xED, 0xE9));
        inner["AppControlBrush"] = inner["TintedControlBrush"];
        inner["AppBorderBrush"] = inner["TintedBorderBrush"];
        inner["AppTextBrush"] = new SolidColorBrush(isDark
            ? Color.FromArgb(255, 0xEE, 0xF1, 0xEA)
            : Color.FromArgb(255, 0x14, 0x21, 0x1E));
        inner["AppMutedBrush"] = new SolidColorBrush(isDark
            ? Color.FromArgb(255, 0x9C, 0xB0, 0xA8)
            : Color.FromArgb(255, 0x55, 0x66, 0x5F));
        inner["AppAccentBrush"] = new SolidColorBrush(accent);
        inner["AppInkBrush"] = new SolidColorBrush(isDark
            ? Color.FromArgb(255, 0x04, 0x23, 0x1A)
            : Color.FromArgb(255, 0xFF, 0xFF, 0xFF));

        var theme = new ResourceDictionary();
        theme.MergedDictionaries.Add(palette);
        theme.MergedDictionaries.Add(inner);
        return theme;
    }

    private static ColorPaletteResources CreateDarkPalette(Color accent) => new()
    {
        Accent = accent,
        AltHigh = Color.FromArgb(255, 0, 0, 0),
        AltLow = Color.FromArgb(255, 0, 0, 0),
        AltMedium = Color.FromArgb(255, 0, 0, 0),
        AltMediumHigh = Color.FromArgb(255, 0, 0, 0),
        AltMediumLow = Color.FromArgb(255, 0, 0, 0),
        BaseHigh = Color.FromArgb(255, 255, 255, 255),
        BaseLow = Color.FromArgb(255, 51, 51, 51),
        BaseMedium = Color.FromArgb(255, 154, 154, 154),
        BaseMediumHigh = Color.FromArgb(255, 180, 180, 180),
        BaseMediumLow = Color.FromArgb(255, 103, 103, 103),
        ChromeAltLow = Color.FromArgb(255, 180, 180, 180),
        ChromeBlackHigh = Color.FromArgb(255, 0, 0, 0),
        ChromeBlackLow = Color.FromArgb(255, 180, 180, 180),
        ChromeBlackMedium = Color.FromArgb(255, 0, 0, 0),
        ChromeBlackMediumLow = Color.FromArgb(255, 0, 0, 0),
        ChromeDisabledHigh = Color.FromArgb(255, 51, 51, 51),
        ChromeDisabledLow = Color.FromArgb(255, 154, 154, 154),
        ChromeGray = Color.FromArgb(255, 128, 128, 128),
        ChromeHigh = Color.FromArgb(255, 128, 128, 128),
        ChromeLow = Color.FromArgb(255, 21, 21, 21),
        ChromeMedium = Color.FromArgb(255, 29, 29, 29),
        ChromeMediumLow = Color.FromArgb(255, 44, 44, 44),
        ChromeWhite = Color.FromArgb(255, 255, 255, 255),
        ListLow = Color.FromArgb(255, 29, 29, 29),
        ListMedium = Color.FromArgb(255, 51, 51, 51),
    };

    private static ColorPaletteResources CreateLightPalette(Color accent) => new()
    {
        Accent = accent,
        AltHigh = Color.FromArgb(255, 255, 255, 255),
        AltLow = Color.FromArgb(255, 255, 255, 255),
        AltMedium = Color.FromArgb(255, 255, 255, 255),
        AltMediumHigh = Color.FromArgb(255, 255, 255, 255),
        AltMediumLow = Color.FromArgb(255, 255, 255, 255),
        BaseHigh = Color.FromArgb(255, 0, 0, 0),
        BaseLow = Color.FromArgb(255, 204, 204, 204),
        BaseMedium = Color.FromArgb(255, 137, 137, 137),
        BaseMediumHigh = Color.FromArgb(255, 93, 93, 93),
        BaseMediumLow = Color.FromArgb(255, 115, 115, 115),
        ChromeAltLow = Color.FromArgb(255, 93, 93, 93),
        ChromeBlackHigh = Color.FromArgb(255, 0, 0, 0),
        ChromeBlackLow = Color.FromArgb(255, 204, 204, 204),
        ChromeBlackMedium = Color.FromArgb(255, 93, 93, 93),
        ChromeBlackMediumLow = Color.FromArgb(255, 137, 137, 137),
        ChromeDisabledHigh = Color.FromArgb(255, 204, 204, 204),
        ChromeDisabledLow = Color.FromArgb(255, 137, 137, 137),
        ChromeGray = Color.FromArgb(255, 115, 115, 115),
        ChromeHigh = Color.FromArgb(255, 204, 204, 204),
        ChromeLow = Color.FromArgb(255, 236, 236, 236),
        ChromeMedium = Color.FromArgb(255, 230, 230, 230),
        ChromeMediumLow = Color.FromArgb(255, 236, 236, 236),
        ChromeWhite = Color.FromArgb(255, 255, 255, 255),
        ListLow = Color.FromArgb(255, 230, 230, 230),
        ListMedium = Color.FromArgb(255, 204, 204, 204),
    };
}
