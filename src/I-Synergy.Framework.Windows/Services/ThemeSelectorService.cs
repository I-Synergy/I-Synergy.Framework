using ISynergy.Extensions;
using ISynergy.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ISynergy.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "RequestedTheme";

        public static ElementTheme Theme { get; set; }
        public static bool IsLightThemeEnabled => Theme == ElementTheme.Light;

        public static event EventHandler<ElementTheme> OnThemeChanged = delegate { };

        public static void Initialize()
        {
            Theme = LoadThemeFromSetting();
        }

        public static void SwitchTheme()
        {
            if (Theme == ElementTheme.Dark)
            {
                SetTheme(ElementTheme.Light);
            }
            else
            {
                SetTheme(ElementTheme.Dark);
            }
        }

        public static void SetTheme(ElementTheme theme)
        {
            Theme = theme;

            SetRequestedTheme();
            SaveThemeInSetting(Theme);

            OnThemeChanged(null, Theme);
        }

        public static void SetRequestedTheme()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                {
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                }

                frameworkElement.RequestedTheme = Theme;
            }

            SetupTitlebar();
        }

        private static void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;

                    if (Theme == ElementTheme.Dark)
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

        private static ElementTheme LoadThemeFromSetting()
        {
            ElementTheme cacheTheme = ElementTheme.Light;

            var themeName = ApplicationData.Current.LocalSettings.Values[SettingsKey];

            if (themeName == null)
            {
                cacheTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
            }
            else
            {
                Enum.TryParse<ElementTheme>(themeName.ToString(), out cacheTheme);
            }

            return cacheTheme;
        }

        private static void SaveThemeInSetting(ElementTheme theme)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }
    }
}
