using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Styles;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Extensions;

public static class ApplicationExtensions
{
    public static System.Windows.Application SetApplicationColor(
        this System.Windows.Application application,
        Color color,
        ILogger? logger = null,
        Themes theme = Themes.Dark)
    {
        ArgumentNullException.ThrowIfNull(application);

        // Remove existing dynamic theme dicts
        var toRemove = application.Resources.MergedDictionaries
            .Where(d => d is IThemeResourceDictionary)
            .ToList();
        foreach (var d in toRemove)
            application.Resources.MergedDictionaries.Remove(d);

        var themeDict = new DynamicThemeResourceDictionary(color, theme);
        application.Resources.MergedDictionaries.Add(themeDict);

        logger?.LogInformation("Applied WPF dynamic theme for color {Color} and theme {Theme}", color, theme);
        return application;
    }
}
