using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Abstractions.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public class ThemeService : IThemeService
    {
        private Style _style;
        private Window _window;

        /// <summary>
        /// Ininitialize main window for service.
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <exception cref="ArgumentException"></exception>
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
        /// <param name="applicationSettingsService"></param>
        public ThemeService(IBaseApplicationSettingsService applicationSettingsService)
        {
            _style = new Style
            {
                Theme = applicationSettingsService.Settings.Theme,
                Color = applicationSettingsService.Settings.Color
            };

            //SetStyle(_style);
        }

        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="style">The theme.</param>
        public void SetStyle(Style style)
        {
            Argument.IsNotNull(style);

            _style = style;

            if (Window.Current.Content is FrameworkElement frameworkElement && !new AccessibilitySettings().HighContrast)
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

            SetupTitlebar();
            ReloadPageTheme(style.Theme);
        }

        private void ReloadPageTheme(Themes theme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
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
        private void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var coreTitleBra = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBra.ExtendViewIntoTitleBar = true;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (titleBar is not null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;

                    if (_style.Theme == Themes.Dark)
                    {
                        titleBar.ButtonForegroundColor = Colors.White;
                        titleBar.ForegroundColor = Colors.White;
                    }
                    else
                    {
                        titleBar.ButtonForegroundColor = Colors.Black;
                        titleBar.ForegroundColor = Colors.Black;
                    }

                    titleBar.BackgroundColor = Colors.Black;

                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveForegroundColor = Colors.LightGray;
                }
            }
        }
    }
}
