using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using System;
using Windows.Storage;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI;
using Windows.UI.Xaml;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeSelectorService" />
    /// </summary>
    /// <seealso cref="IThemeSelectorService" />
    public class ThemeSelectorService : IThemeSelectorService
    {
        /// <summary>
        /// The settings key
        /// </summary>
        private const string SettingsKey = "RequestedTheme";

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public object Theme { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        public bool IsLightThemeEnabled => (ElementTheme)Theme == ElementTheme.Light;

        /// <summary>
        /// Occurs when [on theme changed].
        /// </summary>
        public event EventHandler<object> OnThemeChanged = delegate { };

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            Theme = LoadThemeFromSetting();
        }

        /// <summary>
        /// Switches the theme.
        /// </summary>
        public void SwitchTheme()
        {
            if ((ElementTheme)Theme == ElementTheme.Dark)
            {
                SetTheme(ElementTheme.Light);
            }
            else
            {
                SetTheme(ElementTheme.Dark);
            }
        }

        /// <summary>
        /// AccentColor property.
        /// </summary>
        public string AccentColor { get; private set; }

        /// <summary>
        /// Light theme AccentColor property.
        /// </summary>
        public string LightAccentColor { get; private set; }

        /// <summary>
        /// Dark theme AccentColor property.
        /// </summary>
        public string DarkAccentColor { get; private set; }


        /// <summary>
        /// Sets the requested theme.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetThemeColor(ThemeColors color)
        {
            // Default I-Synergy color.
            AccentColor = "#3399ff";
            LightAccentColor = "#79c9ff";
            DarkAccentColor = "#006ccb";

            switch (color)
            {
                case ThemeColors.Gold:
                    AccentColor = "#f3b200";
                    LightAccentColor = "#ffe44b";
                    DarkAccentColor = "#bb8300";
                    break;
                case ThemeColors.Lime:
                    AccentColor = "#77b900";
                    LightAccentColor = "#abec49";
                    DarkAccentColor = "#438900";
                    break;
                case ThemeColors.Magenta:
                    AccentColor = "#ff00ff";
                    LightAccentColor = "#ff63ff";
                    DarkAccentColor = "#c700cb";
                    break;
                case ThemeColors.Maroon:
                    AccentColor = "#ac193d";
                    LightAccentColor = "#e35267";
                    DarkAccentColor = "#760018";
                    break;
                case ThemeColors.OrangeRed:
                    AccentColor = "#d24726";
                    LightAccentColor = "#ff7851";
                    DarkAccentColor = "#9a0b00";
                    break;
                case ThemeColors.RoyalBlue:
                    AccentColor = "#0073cf";
                    LightAccentColor = "#5da1ff";
                    DarkAccentColor = "#00489d";
                    break;
            }
        }

        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        public void SetTheme(object theme)
        {
            Theme = (ElementTheme)theme;
            SaveThemeInSetting((ElementTheme)Theme);

            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                {
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                }

                frameworkElement.RequestedTheme = (ElementTheme)Theme;
            }

#if WINDOWS_UWP || WINDOWS
            SetupTitlebar();
#endif

            OnThemeChanged(null, (ElementTheme)Theme);
        }

#if WINDOWS_UWP || WINDOWS
        /// <summary>
        /// Setups the titlebar.
        /// </summary>
        private void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;

                    if ((ElementTheme)Theme == ElementTheme.Dark)
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
#endif

        /// <summary>
        /// Loads the theme from setting.
        /// </summary>
        /// <returns>System.Object.</returns>
        private static object LoadThemeFromSetting()
        {
            var themeName = ApplicationData.Current.LocalSettings.Values[SettingsKey];

            ElementTheme cacheTheme;

            if (themeName is null)
            {
                cacheTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
            }
            else
            {
                Enum.TryParse(themeName.ToString(), out cacheTheme);
            }

            return cacheTheme;
        }

        /// <summary>
        /// Saves the theme in setting.
        /// </summary>
        /// <param name="theme">The theme.</param>
        private static void SaveThemeInSetting(ElementTheme theme)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }
    }
}
