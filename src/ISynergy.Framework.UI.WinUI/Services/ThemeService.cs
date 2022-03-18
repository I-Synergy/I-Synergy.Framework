using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ThemeSelectorService.
    /// Implements the <see cref="IThemeService" />
    /// </summary>
    /// <seealso cref="IThemeService" />
    public partial class ThemeService
    {
        private const string _icon = "icon.ico";
        private readonly IInfoService _infoService;
        private Window _window;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="infoService"></param>
        /// <param name="applicationSettingsService"></param>
        /// <param name="options"></param>
        public ThemeService(
            IInfoService infoService,
            IBaseApplicationSettingsService applicationSettingsService,
            IOptions<ConfigurationOptions> options)
            : this(applicationSettingsService, options)
        {
            _infoService = infoService;
        }

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
        /// Sets the theme.
        /// </summary>
        /// <param name="style">The theme.</param>
        public void SetStyle(Style style)
        {
            //Argument.IsNotNull(style);

            //_style = style;

            //if (Window.Current.Content is FrameworkElement frameworkElement && !new AccessibilitySettings().HighContrast)
            //{
            //    var palette = FindColorPaletteResourcesForTheme(_style.Theme.ToString());

            //    if (palette is not null)
            //    {
            //        palette.Accent = ColorHelper.HexStringToColor(style.Color);
            //    }
            //    else
            //    {
            //        palette = new ColorPaletteResources();
            //        palette.Accent = ColorHelper.HexStringToColor(style.Color);
            //        Application.Current.Resources.MergedDictionaries.Add(palette);
            //    }

            //    Application.Current.Resources["SystemAccentColor"] = style.Color;
            //    Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = style.Color;
            //}

            //SetupTitlebar();
            //ReloadPageTheme(style.Theme);
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
                var appWindow = _window.GetAppWindow();
                
                appWindow.Title = _infoService.ProductName;

                if(File.Exists(_icon)) appWindow.SetIcon(_icon);

                var titleBar = appWindow.TitleBar;

                if (titleBar is not null)
                {
                    titleBar.ExtendsContentIntoTitleBar = true;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
                }
            }
        }
    }
}
