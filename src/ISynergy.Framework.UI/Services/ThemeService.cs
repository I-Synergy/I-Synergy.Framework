using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Style = ISynergy.Framework.Core.Models.Style;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using System.IO;
using ISynergy.Framework.Core.Abstractions.Services;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI;
using Windows.UI.Xaml;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
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
#if WINDOWS
        private const string _icon = "icon.ico";
        private readonly IInfoService _infoService;
#endif

        private readonly IBaseSettingsService _baseSettingsService;
        

        private Style _style;
        private Window _window;
        private ConfigurationOptions _options;

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
        /// <param name="baseSettingsService"></param>
        /// <param name="options"></param>
        public ThemeService(
            IBaseSettingsService baseSettingsService,
            IOptions<ConfigurationOptions> options)
        {
            _baseSettingsService = baseSettingsService;
            _options = options.Value;

            _style = new Style
            {
                Theme = _baseSettingsService.Theme,
                Color = _baseSettingsService.Color
            };

            SetStyle(_style);
        }

#if WINDOWS
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="infoService"></param>
        /// <param name="baseSettingsService"></param>
        /// <param name="options"></param>
        public ThemeService(
            IInfoService infoService,
            IBaseSettingsService baseSettingsService,
            IOptions<ConfigurationOptions> options)
            : this(baseSettingsService, options)
        {
            _infoService = infoService;
        }
#endif

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
            Argument.IsNotNull(style);

            _style = style;

            _baseSettingsService.Theme = _style.Theme;
            _baseSettingsService.Color = _style.Color;

            if (Window.Current.Content is FrameworkElement frameworkElement && !new AccessibilitySettings().HighContrast)
            {
                var palette = FindColorPaletteResourcesForTheme(_style.Theme.ToString());

                if (palette is not null)
                {
                    palette.Accent = ColorHelper.HexStringToColor(style.Color);
                }
                else
                {
                    palette = new ColorPaletteResources();
                    palette.Accent = ColorHelper.HexStringToColor(style.Color);
                    Application.Current.Resources.MergedDictionaries.Add(palette);
                }

                Application.Current.Resources["SystemAccentColor"] = style.Color;
                Application.Current.Resources["NavigationViewSelectionIndicatorForeground"] = style.Color;
            }

#if WINDOWS_UWP || WINDOWS
            SetupTitlebar();
#endif
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

#if WINDOWS_UWP || WINDOWS
        /// <summary>
        /// Setups the titlebar.
        /// </summary>
        private void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
#if WINDOWS
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
#elif WINDOWS_UWP
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
#endif
            }
        }
#endif
    }
}
