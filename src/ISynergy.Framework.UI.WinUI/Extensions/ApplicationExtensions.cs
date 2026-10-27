using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Styles;
using Windows.UI;

namespace ISynergy.Framework.UI.Extensions;

public static class ApplicationExtensions
{
    public static Microsoft.UI.Xaml.Application SetApplicationColor(this Microsoft.UI.Xaml.Application application, Color color) // NOSONAR
    {
        // Remove existing theme dictionaries: either a dynamic IThemeResourceDictionary,
        // or a XAML-backed theme dict identified by its source URI.
        var toRemove = application.Resources.MergedDictionaries
            .Where(item =>
                item is IThemeResourceDictionary ||
                (item.Source is not null &&
                 !item.Source.AbsoluteUri.EndsWith("themeresources.xaml", StringComparison.OrdinalIgnoreCase) &&
                 !item.Source.AbsoluteUri.EndsWith("themeresources_v2.xaml", StringComparison.OrdinalIgnoreCase) &&
                 !item.Source.AbsoluteUri.EndsWith("Images.xaml", StringComparison.OrdinalIgnoreCase) &&
                 !item.Source.AbsoluteUri.EndsWith("Style.xaml", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var item in toRemove)
            application.Resources.MergedDictionaries.Remove(item);

        var themeDict = new DynamicThemeResourceDictionary(color);
        if (!application.Resources.MergedDictionaries.Contains(themeDict))
            application.Resources.MergedDictionaries.Add(themeDict);

        return application;
    }
}
