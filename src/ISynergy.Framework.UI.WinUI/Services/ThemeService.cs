using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private readonly IBaseApplicationSettingsService _applicationSettingsService;

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public Style Style
    {
        get => new()
        {
            Theme = _applicationSettingsService.Settings.Theme,
            Color = _applicationSettingsService.Settings.Color
        };
    }

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => Style.Theme == Themes.Light;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="applicationSettingsService"></param>
    public ThemeService(IBaseApplicationSettingsService applicationSettingsService)
    {
        _applicationSettingsService = applicationSettingsService;
        _applicationSettingsService.LoadSettings();
    }

    /// <summary>
    /// Sets the theme.
    /// </summary>
    public void SetStyle()
    {
        var palette = FindColorPaletteResourcesForTheme(Style.Theme.ToString());

        if (palette is not null)
        {
            palette.Accent = ISynergy.Framework.UI.Helpers.ColorHelper.HexStringToColor(Style.Color);
        }
        else
        {
            palette = new ColorPaletteResources();
            palette.Accent = ISynergy.Framework.UI.Helpers.ColorHelper.HexStringToColor(Style.Color);
            Application.Current.Resources.MergedDictionaries.Add(palette);
        }

        Application.Current.Resources["SystemAccentColor"] = Style.Color;
        Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = Style.Color;

        switch (Style.Theme)
        {
            case Themes.Light:
                if (RootTheme != ElementTheme.Light)
                    RootTheme = ElementTheme.Light;
                break;
            case Themes.Dark:
                if (RootTheme != ElementTheme.Dark)
                    RootTheme = ElementTheme.Dark;
                break;
            default:
                if (RootTheme != ElementTheme.Default)
                    RootTheme = ElementTheme.Default;
                break;
        }

        foreach (var window in WindowHelper.ActiveWindows)
        {
            if (window.Content is FrameworkElement rootElement)
                SetTitlebar(window);
        }
    }

    /// <summary>
    /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
    /// </summary>
    private ElementTheme RootTheme
    {
        get
        {
            foreach (var window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                    return rootElement.ActualTheme;
            }

            return ElementTheme.Default;
        }
        set
        {
            foreach (var window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                    rootElement.RequestedTheme = value;
            }
        }
    }

    /// <summary>
    /// Finds the color palette resources for theme.
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns>ColorPaletteResources.</returns>
    private ColorPaletteResources FindColorPaletteResourcesForTheme(string theme)
    {
        foreach (var themeDictionary in Application.Current.Resources.ThemeDictionaries)
        {
            if (themeDictionary.Key.ToString() == theme)
            {
                if (themeDictionary.Value is ColorPaletteResources)
                    return themeDictionary.Value as ColorPaletteResources;
                else if (themeDictionary.Value is ResourceDictionary targetDictionary)
                    foreach (var mergedDictionary in targetDictionary.MergedDictionaries)
                    {
                        if (mergedDictionary is ColorPaletteResources)
                            return mergedDictionary as ColorPaletteResources;
                    }
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the current actual theme of the app based on the requested theme of the
    /// root element, or if that value is Default, the requested theme of the Application.
    /// </summary>
    private ElementTheme ActualTheme
    {
        get
        {
            foreach (var window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement && rootElement.RequestedTheme != ElementTheme.Default)
                {
                    return rootElement.RequestedTheme;
                }
            }

            switch (Application.Current.RequestedTheme)
            {
                case ApplicationTheme.Light:
                    return ElementTheme.Light;
                case ApplicationTheme.Dark:
                    return ElementTheme.Dark;
                default:
                    return ElementTheme.Default;
            }
        }
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    public void SetTitlebar(Microsoft.UI.Xaml.Window window)
    {
#if WINDOWS
        var appWindow = WindowHelper.GetAppWindow(window);

        var iconPath = Path.Combine(System.AppContext.BaseDirectory, "icon.ico");
        
        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            if (Style.Theme == Themes.Dark)
            {
                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                appWindow.TitleBar.ForegroundColor = Colors.White;
            }
            else
            {
                appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                appWindow.TitleBar.ForegroundColor = Colors.Black;
            }

            appWindow.TitleBar.BackgroundColor = Colors.Black;

            appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.LightGray;
        }
#endif
    }
}
