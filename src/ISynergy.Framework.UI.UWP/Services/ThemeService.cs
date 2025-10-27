using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public class ThemeService : IThemeService
    {
        private readonly Themes _theme;
        private readonly string _color;

        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        public bool IsLightThemeEnabled => _theme == Themes.Light;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="settingsService"></param>
        public ThemeService(ISettingsService settingsService)
        {
            _theme = settingsService.LocalSettings.Theme;
            _color = settingsService.LocalSettings.Color;

            ApplyTheme();
        }

        public void ApplyTheme()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement && !new AccessibilitySettings().HighContrast)
            {
                var palette = FindColorPaletteResourcesForTheme(_theme.ToString());

                if (palette is not null)
                {
                    palette.Accent = ColorHelper.HexStringToColor(_color);
                }
                else
                {
                    palette = new ColorPaletteResources();
                    palette.Accent = ColorHelper.HexStringToColor(_color);
                    Application.Current.Resources.MergedDictionaries.Add(palette);
                }

                Application.Current.Resources["SystemAccentColor"] = _color;
                Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = _color;
            }

            SetupTitlebar();
            ReloadPageTheme(_theme);
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

                    if (_theme == Themes.Dark)
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
