using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;
using WinRT.Interop;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
/// <summary>
/// Class ThemeSelectorService.
/// Implements the <see cref="IThemeService" />
/// </summary>
/// <seealso cref="IThemeService" />
public class ThemeService : IThemeService
{
    private const string Primary = nameof(Primary);
    private const string Secondary = nameof(Secondary);
    private const string Tertiary = nameof(Tertiary);

    private Themes _theme;
    private string _color;

    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;

    /// <summary>
    /// The window
    /// </summary>
    private Microsoft.UI.Xaml.Window? _window;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => _theme == Themes.Light;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="settingsService">An application settings service that provides persisted theme and color values.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    public ThemeService(
        ISettingsService settingsService,
        IMessengerService messengerService,
        ILogger<ThemeService> logger)
    {
        _settingsService = settingsService;
        _logger = logger;

        _theme = settingsService.LocalSettings.Theme;
        _color = settingsService.LocalSettings.Color;

        messengerService.Register<UserSignedInMessage>(this, OnUserSignedIn, keepTargetAlive: true);
    }

    private void OnUserSignedIn(UserSignedInMessage message)
    {
        _theme = _settingsService.LocalSettings.Theme;
        _color = _settingsService.LocalSettings.Color;
        ApplyTheme();
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
    /// Applies the configured theme and accent color to the MAUI <see cref="Application"/> instance.
    /// </summary>
    /// <remarks>
    /// The method safely handles a missing Application.Current instance and logs diagnostics.
    /// For Windows, it updates the title bar colors; for Android, it updates common color resources.
    /// </remarks>
    public void ApplyTheme()
    {
        _logger.LogInformation("Setting application style with color {Color} and theme {Theme}",
            _color, _theme);

        if (Application.Current is not Application application)
        {
            _logger.LogWarning("Application.Current is not available");
            return;
        }

        try
        {
            // Use the new ApplicationExtensions to dynamically load the theme
            application.SetApplicationColor(ColorHelper.GetColorFromHexString(_color));

            // Set the accent color from the loaded theme
            if (Application.Current.Resources.TryGetValue(Primary, out var resource) && resource is Color primaryColor)
            {
                Application.AccentColor = primaryColor;
                _logger.LogTrace("AccentColor set to {Color}", primaryColor);
            }
            else
            {
                // Fallback to parsing the color from settings
                Application.AccentColor = ColorHelper.HexStringToColor(_color);
                _logger.LogTrace("AccentColor set to {Color} from settings", _color);
            }

            // Update platform-specific color resources
            UpdatePlatformColors(application);

            _logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set application style");
        }
    }

    private void UpdatePlatformColors(Application application)
    {
        if (_window is null)
            return;

        if (_window.Content is FrameworkElement frameworkElement)
        {
            switch (_theme)
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
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
#if WINDOWS
        var appWindow = GetAppWindowForCurrentWindow(_window);

        var iconPath = Path.Combine(Environment.CurrentDirectory, "icon.ico");

        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            if (_theme == Themes.Dark)
            {
                appWindow.TitleBar.ForegroundColor = Colors.White;
                appWindow.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.InactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);

                appWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 51, 51, 51);
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
                appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 245, 245, 245);
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
