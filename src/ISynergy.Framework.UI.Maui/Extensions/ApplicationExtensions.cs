using ISynergy.Framework.UI.Resources.Styles.Themes;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for MAUI Application to manage dynamic theme switching
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Sets the application color theme dynamically
    /// </summary>
    /// <param name="application">The MAUI application instance</param>
    /// <param name="color">The hex color string (e.g., "#FFB900")</param>
    /// <param name="logger">Optional logger for diagnostics</param>
    /// <returns>The application instance for method chaining</returns>
    public static Microsoft.Maui.Controls.Application SetApplicationColor(
        this Microsoft.Maui.Controls.Application application,
        string color,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);

        logger?.LogInformation("Setting application color to {Color}", color);

        // Remove existing theme dictionaries, but preserve other resource dictionaries
        var themesToRemove = application.Resources.MergedDictionaries
            .Where(dict => dict.GetType().Namespace == "ISynergy.Framework.UI.Resources.Styles.Themes")
            .ToList();

        foreach (var theme in themesToRemove)
        {
            application.Resources.MergedDictionaries.Remove(theme);
            logger?.LogTrace("Removed existing theme dictionary: {ThemeType}", theme.GetType().Name);
        }

        // Get the theme resource dictionary based on color
        var themeDict = GetThemeResourceDictionary(color);

        if (themeDict is not null)
        {
            if (!application.Resources.MergedDictionaries.Contains(themeDict))
            {
                application.Resources.MergedDictionaries.Add(themeDict);
                logger?.LogInformation("Applied theme for color {Color}", color);
            }
        }
        else
        {
            logger?.LogWarning("Theme not found for color {Color}, falling back to default", color);
            // Fallback to default theme
            var defaultTheme = new Themeffb900();
            if (!application.Resources.MergedDictionaries.Contains(defaultTheme))
            {
                application.Resources.MergedDictionaries.Add(defaultTheme);
            }
        }

        return application;
    }

    /// <summary>
    /// Gets the appropriate theme resource dictionary for the given color
    /// </summary>
    /// <param name="color">The hex color string</param>
    /// <returns>The theme resource dictionary or null if not found</returns>
    private static ResourceDictionary? GetThemeResourceDictionary(string color)
    {
        // Normalize the color string (remove # and convert to lowercase)
        var colorCode = color.TrimStart('#').ToLowerInvariant();

        return colorCode switch
        {
            "ffb900" => new Themeffb900(),
            "ff8c00" => new Themeff8c00(),
            "f7630c" => new Themef7630c(),
            "ca5010" => new Themeca5010(),
            "da3b01" => new Themeda3b01(),
            "ef6950" => new Themeef6950(),
            "d13438" => new Themed13438(),
            "ff4343" => new Themeff4343(),
            "e74856" => new Themee74856(),
            "e81123" => new Themee81123(),
            "ea005e" => new Themeea005e(),
            "c30052" => new Themec30052(),
            "e3008c" => new Themee3008c(),
            "bf0077" => new Themebf0077(),
            "c239b3" => new Themec239b3(),
            "9a0089" => new Theme9a0089(),
            "0078d7" => new Theme0078d7(),
            "0063b1" => new Theme0063b1(),
            "8e8cd8" => new Theme8e8cd8(),
            "6b69d6" => new Theme6b69d6(),
            "8764b8" => new Theme8764b8(),
            "744da9" => new Theme744da9(),
            "b146c2" => new Themeb146c2(),
            "881798" => new Theme881798(),
            "0099bc" => new Theme0099bc(),
            "2d7d9a" => new Theme2d7d9a(),
            "00b7c3" => new Theme00b7c3(),
            "038387" => new Theme038387(),
            "00b294" => new Theme00b294(),
            "018574" => new Theme018574(),
            "00cc6a" => new Theme00cc6a(),
            "10893e" => new Theme10893e(),
            "7a7574" => new Theme7a7574(),
            "5d5a58" => new Theme5d5a58(),
            "68768a" => new Theme68768a(),
            "515c6b" => new Theme515c6b(),
            "567c73" => new Theme567c73(),
            "486860" => new Theme486860(),
            "498205" => new Theme498205(),
            "107c10" => new Theme107c10(),
            "767676" => new Theme767676(),
            "4c4a48" => new Theme4c4a48(),
            "69797e" => new Theme69797e(),
            "4a5459" => new Theme4a5459(),
            "647c64" => new Theme647c64(),
            "525e54" => new Theme525e54(),
            "847545" => new Theme847545(),
            "7e735f" => new Theme7e735f(),
            _ => null
        };
    }
}
