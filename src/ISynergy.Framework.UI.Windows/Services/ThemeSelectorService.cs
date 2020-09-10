using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
        /// Sets the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        public void SetTheme(object theme)
        {
            Theme = (ElementTheme)theme;

            SetRequestedTheme();
            SaveThemeInSetting((ElementTheme)Theme);

            OnThemeChanged(null, (ElementTheme)Theme);
        }

        /// <summary>
        /// Sets the requested theme.
        /// </summary>
        public void SetRequestedTheme()
        {
            var color = ServiceLocator.Default.GetInstance<IApplicationSettingsService>().Color;

            Application.Current.Resources.ThemeDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"ms-appx:///ISynergy.Framework.UI/Themes/Theme.{color}.xaml", UriKind.RelativeOrAbsolute) });

            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                {
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                }

                frameworkElement.RequestedTheme = (ElementTheme)Theme;
            }

            SetupTitlebar();
        }

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

        /// <summary>
        /// Loads the theme from setting.
        /// </summary>
        /// <returns>System.Object.</returns>
        private static object LoadThemeFromSetting()
        {
            var cacheTheme = ElementTheme.Light;
            var themeName = ApplicationData.Current.LocalSettings.Values[SettingsKey];

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
