using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using WinRT.Interop;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private readonly IApplicationSettingsService _applicationSettingsService;

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
    /// The window
    /// </summary>
    private Microsoft.UI.Xaml.Window _window;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => Style.Theme == Themes.Light;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="applicationSettingsService"></param>
    public ThemeService(IApplicationSettingsService applicationSettingsService)
    {
        _applicationSettingsService = applicationSettingsService;
        _applicationSettingsService.LoadSettings();
    }

    /// <summary>
    /// Ininitialize main window for service.
    /// </summary>
    /// <param name="mainWindow">The main window.</param>
    /// <exception cref="System.ArgumentException">MainWindow could not be set.</exception>
    public void InitializeMainWindow(object mainWindow)
    {
        if (mainWindow is Microsoft.UI.Xaml.Window window)
        {
            _window = window;
        }
        else
        {
            throw new ArgumentException("MainWindow could not be set.");
        }
    }

    /// <summary>
    /// Sets the theme.
    /// </summary>
    public void SetStyle()
    {
        if (_window.Content is FrameworkElement frameworkElement)
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

            if (Application.Current.Resources.ContainsKey("SystemAccentColor"))
                Application.Current.Resources["SystemAccentColor"] = Style.Color;

            if (Application.Current.Resources.ContainsKey("NavigationViewSelectionIndicatorForeground"))
                Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = Style.Color;
        }

        ReloadPageTheme(Style.Theme);

#if WINDOWS
        SetTitlebar();
#endif

        MessageService.Default.Send(new StyleChangedMessage(Style));
    }

    /// <summary>
    /// Reloads the page theme.
    /// </summary>
    /// <param name="theme">The theme.</param>
    private void ReloadPageTheme(Themes theme)
    {
        if (_window.Content is FrameworkElement frameworkElement)
        {
            switch (theme)
            {
                case Themes.Light:
                    //frameworkElement.RequestedTheme = ElementTheme.Dark;
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                    break;
                case Themes.Dark:
                    //frameworkElement.RequestedTheme = ElementTheme.Light;
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    //frameworkElement.RequestedTheme = ElementTheme.Light;
                    //frameworkElement.RequestedTheme = ElementTheme.Dark;
                    frameworkElement.RequestedTheme = ElementTheme.Default;
                    break;
            }
        }
    }

    /// <summary>
    /// Finds the hexColor palette resources for theme.
    /// </summary>
    /// <param name="theme">The theme.</param>
    /// <returns>ColorPaletteResources.</returns>
    private ColorPaletteResources FindColorPaletteResourcesForTheme(string theme)
    {
        foreach (var themeDictionary in Application.Current.Resources.ThemeDictionaries.EnsureNotNull())
        {
            if (themeDictionary.Key.ToString() == theme)
            {
                if (themeDictionary.Value is ColorPaletteResources)
                    return themeDictionary.Value as ColorPaletteResources;
                if (themeDictionary.Value is ResourceDictionary targetDictionary)
                    foreach (var mergedDictionary in targetDictionary.MergedDictionaries.EnsureNotNull())
                    {
                        if (mergedDictionary is ColorPaletteResources)
                            return mergedDictionary as ColorPaletteResources;
                    }
            }
        }
        return null;
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
#if WINDOWS
        var appWindow = GetAppWindowForCurrentWindow(_window);

        var iconPath = Path.Combine(Package.Current.InstalledLocation.Path, "icon.ico");

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

#if WINDOWS && !HAS_UNO
    private AppWindow GetAppWindowForCurrentWindow(Microsoft.UI.Xaml.Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
#endif
}
