using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Abstractions.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.UI.ViewManagement;
using Style = ISynergy.Framework.Core.Models.Style;

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
using WinRT.Interop;
#endif

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public class ThemeService : IThemeService
    {
        /// <summary>
        /// The style
        /// </summary>
        private Style _style;
        /// <summary>
        /// The window
        /// </summary>
        private Window _window;

        /// <summary>
        /// Ininitialize main window for service.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <exception cref="System.ArgumentException">MainWindow could not be set.</exception>
        public void InitializeMainWindow(object mainWindow)
        {
            if (mainWindow is Window window)
            {
                _window = window;
            }
            else
            {
                throw new ArgumentException("MainWindow could not be set.");
            }
        }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public Style Style { get => _style; }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        public bool IsLightThemeEnabled => _style.Theme == Themes.Light;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="applicationSettingsService">The application settings service.</param>
        public ThemeService(IBaseApplicationSettingsService applicationSettingsService)
        {
            _style = new Style(
                applicationSettingsService.Settings.Color, 
                applicationSettingsService.Settings.Theme);
        }

        /// <summary>
        /// Sets the style.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="theme">The theme.</param>
        public void SetStyle(string color, Themes theme) =>
            SetStyle(new Style(color, theme));

        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="style">The theme.</param>
        public void SetStyle(Style style)
        {
            Argument.IsNotNull(style);

            _style = style;

            if (_window.Content is FrameworkElement frameworkElement && !new AccessibilitySettings().HighContrast)
            {
                var palette = FindColorPaletteResourcesForTheme(_style.Theme.ToString());

                if (palette is not null)
                {
                    palette.Accent = ISynergy.Framework.UI.Helpers.ColorHelper.HexStringToColor(style.Color);
                }
                else
                {
                    palette = new ColorPaletteResources();
                    palette.Accent = ISynergy.Framework.UI.Helpers.ColorHelper.HexStringToColor(style.Color);
                    Application.Current.Resources.MergedDictionaries.Add(palette);
                }

                Application.Current.Resources["SystemAccentColor"] = style.Color;
                Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = style.Color;
            }

            ReloadPageTheme(style.Theme);
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
                        frameworkElement.RequestedTheme = ElementTheme.Dark;
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                        break;
                    case Themes.Dark:
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                        frameworkElement.RequestedTheme = ElementTheme.Dark;
                        break;
                    default:
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                        frameworkElement.RequestedTheme = ElementTheme.Dark;
                        frameworkElement.RequestedTheme = ElementTheme.Default;
                        break;
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
        /// Setups the titlebar.
        /// </summary>
        public void SetTitlebar(Window window)
        {
#if WINDOWS10_0_18362_0_OR_GREATER
            var appWindow = GetAppWindowForCurrentWindow(window);

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

                appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

                if (_style.Theme == Themes.Dark)
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

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
        protected virtual AppWindow GetAppWindowForCurrentWindow(Window window)
        {
            var hWnd = WindowNative.GetWindowHandle(window);
            var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
#endif
    }
}
