using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Enumerations;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.ViewManagement;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI;
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
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
        /// Gets or sets a value indicating whether this <see cref="IThemeSelectorService" /> is material.
        /// </summary>
        /// <value><c>true</c> if material; otherwise, <c>false</c>.</value>
        public bool Material { get; set; }

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
        /// Sets the requested theme.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetThemeColor(ThemeColors color)
        {
            // Default I-Synergy color.
            var accentColor = "#3399ff";
            var lightColor = "#79c9ff";
            var darkColor = "#006ccb";

            switch (color)
            {
                case ThemeColors.Gold:
                    accentColor = "#f3b200";
                    lightColor = "#ffe44b";
                    darkColor = "#bb8300";
                    break;
                case ThemeColors.Lime:
                    accentColor = "#77b900";
                    lightColor = "#abec49";
                    darkColor = "#438900";
                    break;
                case ThemeColors.Magenta:
                    accentColor = "#ff00ff";
                    lightColor = "#ff63ff";
                    darkColor = "#c700cb";
                    break;
                case ThemeColors.Maroon:
                    accentColor = "#ac193d";
                    lightColor = "#e35267";
                    darkColor = "#760018";
                    break;
                case ThemeColors.OrangeRed:
                    accentColor = "#d24726";
                    lightColor = "#ff7851";
                    darkColor = "#9a0b00";
                    break;
                case ThemeColors.RoyalBlue:
                    accentColor = "#0073cf";
                    lightColor = "#5da1ff";
                    darkColor = "#00489d";
                    break;
            }

            if(Material)
            {
                //// Set a default palette to make sure all colors used by MaterialResources exist
                //Application.Current.Resources.MergedDictionaries.Add(new MaterialColorPalette());

                //// Add all the material resources. Those resources depend on the colors above, which is why this one must be added last.
                //Application.Current.Resources.MergedDictionaries.Add(new MaterialResources());


                //Application.Current.Resources["MaterialPrimaryColor"] = accentColor;
                //Application.Current.Resources["MaterialPrimaryVariantLightColor"] = lightColor;
                //Application.Current.Resources["MaterialPrimaryVariantDarkColor"] = darkColor;
            }

            Application.Current.Resources["SystemAccentColor"] = accentColor;
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

#if NETFX_CORE || (NET5_0 && WINDOWS)
            SetupTitlebar();
#endif

            OnThemeChanged(null, (ElementTheme)Theme);
        }

#if NETFX_CORE || (NET5_0 && WINDOWS)
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
