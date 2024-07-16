using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using ISynergy.Framework.UI.Styles;

#if WINDOWS
using Windows.UI;
#endif

#if WINDOWS && !HAS_UNO
using WinRT.Interop;
#endif

using Style = ISynergy.Framework.Core.Models.Style;


namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private readonly IApplicationSettingsService _applicationSettingsService;

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public Style Style
    {
        get => new(_applicationSettingsService.Settings.Color, _applicationSettingsService.Settings.Theme);
    }

    /// <summary>
    /// The window
    /// </summary>
    private Microsoft.UI.Xaml.Window _window;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => Style.Theme == Themes.Light;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="applicationSettingsService"></param>
    public ThemeService(IApplicationSettingsService applicationSettingsService)
    {
        _applicationSettingsService = applicationSettingsService;
        _applicationSettingsService.LoadSettings();
    }

    /// <summary>
    /// Ininitialize main window for service.
    /// </summary>
    /// <param name="mainWindow">The main window.</param>
    /// <exception cref="System.ArgumentException">MainWindow could not be set.</exception>
    public void InitializeMainWindow(object mainWindow)
    {
        if (mainWindow is Microsoft.UI.Xaml.Window window)
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
    public void SetStyle()
    {
        foreach (var item in Application.Current.Resources.MergedDictionaries.EnsureNotNull())
        {
            if (!item.Source.AbsoluteUri.EndsWith("themeresources.xaml") &&
                !item.Source.AbsoluteUri.EndsWith("Images.xaml") &&
                !item.Source.AbsoluteUri.EndsWith("Style.xaml") &&
                !item.Source.AbsoluteUri.EndsWith(Style.Color.Substring(1, 6)))
                Application.Current.Resources.MergedDictionaries.Remove(item);
        }

        switch (Style.Color)
        {
            case "#ff8c00":
                var style_ff8c00 = new _ff8c00();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ff8c00))
                    Application.Current.Resources.MergedDictionaries.Add(style_ff8c00);
                break;
            case "#f7630c":
                var style_f7630c = new _f7630c();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_f7630c))
                    Application.Current.Resources.MergedDictionaries.Add(style_f7630c);
                break;
            case "#ca5010":
                var style_ca5010 = new _ca5010();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ca5010))
                    Application.Current.Resources.MergedDictionaries.Add(style_ca5010);
                break;
            case "#da3b01":
                var style_da3b01 = new _da3b01();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_da3b01))
                    Application.Current.Resources.MergedDictionaries.Add(style_da3b01);
                break;
            case "#ef6950":
                var style_ef6950 = new _ef6950();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ef6950))
                    Application.Current.Resources.MergedDictionaries.Add(style_ef6950);
                break;
            case "#d13438":
                var style_d13438 = new _d13438();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_d13438))
                    Application.Current.Resources.MergedDictionaries.Add(style_d13438);
                break;
            case "#ff4343":
                var style_ff4343 = new _ff4343();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ff4343))
                    Application.Current.Resources.MergedDictionaries.Add(style_ff4343);
                break;
            case "#e74856":
                var style_e74856 = new _e74856();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_e74856))
                    Application.Current.Resources.MergedDictionaries.Add(style_e74856);
                break;
            case "#e81123":
                var style_e81123 = new _e81123();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_e81123))
                    Application.Current.Resources.MergedDictionaries.Add(style_e81123);
                break;
            case "#ea005e":
                var style_ea005e = new _ea005e();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ea005e))
                    Application.Current.Resources.MergedDictionaries.Add(style_ea005e);
                break;
            case "#c30052":
                var style_c30052 = new _c30052();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_c30052))
                    Application.Current.Resources.MergedDictionaries.Add(style_c30052);
                break;
            case "#e3008c":
                var style_e3008c = new _e3008c();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_e3008c))
                    Application.Current.Resources.MergedDictionaries.Add(style_e3008c);
                break;
            case "#bf0077":
                var style_bf0077 = new _bf0077();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_bf0077))
                    Application.Current.Resources.MergedDictionaries.Add(style_bf0077);
                break;
            case "#c239b3":
                var style_c239b3 = new _c239b3();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_c239b3))
                    Application.Current.Resources.MergedDictionaries.Add(style_c239b3);
                break;
            case "#9a0089":
                var style_9a0089 = new _9a0089();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_9a0089))
                    Application.Current.Resources.MergedDictionaries.Add(style_9a0089);
                break;
            case "#0078d7":
                var style_0078d7 = new _0078d7();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_0078d7))
                    Application.Current.Resources.MergedDictionaries.Add(style_0078d7);
                break;
            case "#0063b1":
                var style_0063b1 = new _0063b1();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_0063b1))
                    Application.Current.Resources.MergedDictionaries.Add(style_0063b1);
                break;
            case "#8e8cd8":
                var style_8e8cd80 = new _8e8cd8();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_8e8cd80))
                    Application.Current.Resources.MergedDictionaries.Add(style_8e8cd80);
                break;
            case "#6b69d6":
                var style_6b69d6 = new _6b69d6();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_6b69d6))
                    Application.Current.Resources.MergedDictionaries.Add(style_6b69d6);
                break;
            case "#8764b8":
                var style_8764b8 = new _8764b8();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_8764b8))
                    Application.Current.Resources.MergedDictionaries.Add(style_8764b8);
                break;
            case "#744da9":
                var style_744da9 = new _744da9();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_744da9))
                    Application.Current.Resources.MergedDictionaries.Add(style_744da9);
                break;
            case "#b146c2":
                var style_b146c2 = new _b146c2();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_b146c2))
                    Application.Current.Resources.MergedDictionaries.Add(style_b146c2);
                break;
            case "#881798":
                var style_881798 = new _881798();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_881798))
                    Application.Current.Resources.MergedDictionaries.Add(style_881798);
                break;
            case "#0099bc":
                var style_0099bc = new _0099bc();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_0099bc))
                    Application.Current.Resources.MergedDictionaries.Add(style_0099bc);
                break;
            case "#2d7d9a":
                var style_2d7d9a = new _2d7d9a();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_2d7d9a))
                    Application.Current.Resources.MergedDictionaries.Add(style_2d7d9a);
                break;
            case "#00b7c3":
                var style_00b7c3 = new _00b7c3();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_00b7c3))
                    Application.Current.Resources.MergedDictionaries.Add(style_00b7c3);
                break;
            case "#038387":
                var style_038387 = new _038387();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_038387))
                    Application.Current.Resources.MergedDictionaries.Add(style_038387);
                break;
            case "#00b294":
                var style_00b294 = new _00b294();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_00b294))
                    Application.Current.Resources.MergedDictionaries.Add(style_00b294);
                break;
            case "#018574":
                var style_018574 = new _018574();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_018574))
                    Application.Current.Resources.MergedDictionaries.Add(style_018574);
                break;
            case "#00cc6a":
                var style_00cc6a = new _00cc6a();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_00cc6a))
                    Application.Current.Resources.MergedDictionaries.Add(style_00cc6a);
                break;
            case "#10893e":
                var style_10893e = new _10893e();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_10893e))
                    Application.Current.Resources.MergedDictionaries.Add(style_10893e);
                break;
            case "#7a7574":
                var style_7a7574 = new _7a7574();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_7a7574))
                    Application.Current.Resources.MergedDictionaries.Add(style_7a7574);
                break;
            case "#5d5a58":
                var style_5d5a58 = new _5d5a58();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_5d5a58))
                    Application.Current.Resources.MergedDictionaries.Add(style_5d5a58);
                break;
            case "#68768a":
                var style_68768a = new _68768a();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_68768a))
                    Application.Current.Resources.MergedDictionaries.Add(style_68768a);
                break;
            case "#515c6b":
                var style_515c6b = new _515c6b();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_515c6b))
                    Application.Current.Resources.MergedDictionaries.Add(style_515c6b);
                break;
            case "#567c73":
                var style_567c73 = new _567c73();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_567c73))
                    Application.Current.Resources.MergedDictionaries.Add(style_567c73);
                break;
            case "#486860":
                var style_486860 = new _486860();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_486860))
                    Application.Current.Resources.MergedDictionaries.Add(style_486860);
                break;
            case "#498205":
                var style_498205 = new _498205();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_498205))
                    Application.Current.Resources.MergedDictionaries.Add(style_498205);
                break;
            case "#107c10":
                var style_107c10 = new _107c10();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_107c10))
                    Application.Current.Resources.MergedDictionaries.Add(style_107c10);
                break;
            case "#767676":
                var style_767676 = new _767676();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_767676))
                    Application.Current.Resources.MergedDictionaries.Add(style_767676);
                break;
            case "#4c4a48":
                var style_4c4a48 = new _4c4a48();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_4c4a48))
                    Application.Current.Resources.MergedDictionaries.Add(style_4c4a48);
                break;
            case "#69797e":
                var style_69797e = new _69797e();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_69797e))
                    Application.Current.Resources.MergedDictionaries.Add(style_69797e);
                break;
            case "#4a5459":
                var style_4a5459 = new _4a5459();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_4a5459))
                    Application.Current.Resources.MergedDictionaries.Add(style_4a5459);
                break;
            case "#647c64":
                var style_647c64 = new _647c64();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_647c64))
                    Application.Current.Resources.MergedDictionaries.Add(style_647c64);
                break;
            case "#525e54":
                var style_525e54 = new _525e54();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_525e54))
                    Application.Current.Resources.MergedDictionaries.Add(style_525e54);
                break;
            case "#847545":
                var style_847545 = new _847545();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_847545))
                    Application.Current.Resources.MergedDictionaries.Add(style_847545);
                break;
            case "#7e735f":
                var style_7e735f = new _7e735f();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_7e735f))
                    Application.Current.Resources.MergedDictionaries.Add(style_7e735f);
                break;
            case "#ffb900":
            default:
                var style_ffb900 = new _ffb900();
                if (!Application.Current.Resources.MergedDictionaries.Contains(style_ffb900))
                    Application.Current.Resources.MergedDictionaries.Add(style_ffb900);
                break;
        }

        if (_window.Content is FrameworkElement frameworkElement)
        {
            switch (Style.Theme)
            {
                case Themes.Light:
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                    break;
                case Themes.Dark:
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    frameworkElement.RequestedTheme = ElementTheme.Default;
                    break;
            }
        }

#if WINDOWS
        SetTitlebar();
#endif

        MessageService.Default.Send(new StyleChangedMessage(Style));
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
#if WINDOWS
        var appWindow = GetAppWindowForCurrentWindow(_window);

        var iconPath = Path.Combine(Package.Current.InstalledLocation.Path, "icon.ico");

        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            if (Style.Theme == Themes.Dark)
            {
                appWindow.TitleBar.ForegroundColor = Colors.White;
                appWindow.TitleBar.BackgroundColor = Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 51, 51, 51);
                appWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }
            else
            {
                appWindow.TitleBar.ForegroundColor = Colors.Black;
                appWindow.TitleBar.BackgroundColor = Colors.White;
                appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.InactiveBackgroundColor = Colors.White;

                appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonBackgroundColor = Colors.White;
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.White;

                appWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 245, 245, 245);
                appWindow.TitleBar.ButtonPressedForegroundColor = Colors.Black;
                appWindow.TitleBar.ButtonPressedBackgroundColor = Colors.White;
            }

            appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        }
#endif
    }

#if WINDOWS && !HAS_UNO
    private AppWindow GetAppWindowForCurrentWindow(Microsoft.UI.Xaml.Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }
#endif
}
