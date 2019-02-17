using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ISynergy.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string SettingsKey = "RequestedTheme";

        public object Theme { get; set; }
        public bool IsLightThemeEnabled => (ElementTheme)Theme == ElementTheme.Light;

        public event EventHandler<object> OnThemeChanged = delegate { };

        public void Initialize()
        {
            Theme = LoadThemeFromSetting();
        }

        public void SwitchTheme()
        {
            if ((ElementTheme)Theme  == ElementTheme.Dark)
            {
                SetTheme(ElementTheme.Light);
            }
            else
            {
                SetTheme(ElementTheme.Dark);
            }
        }

        public void SetTheme(object theme)
        {
            Theme = (ElementTheme)theme;

            SetRequestedTheme();
            SaveThemeInSetting((ElementTheme)Theme);

            OnThemeChanged(null, (ElementTheme)Theme);
        }

        public void SetRequestedTheme()
        {
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

        private void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
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

        private object LoadThemeFromSetting()
        {
            ElementTheme cacheTheme = ElementTheme.Light;

            var themeName = ApplicationData.Current.LocalSettings.Values[SettingsKey];

            if (themeName is null)
            {
                cacheTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
            }
            else
            {
                Enum.TryParse<ElementTheme>(themeName.ToString(), out cacheTheme);
            }

            return cacheTheme;
        }

        private void SaveThemeInSetting(ElementTheme theme)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }
    }
}
