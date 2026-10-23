using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color in WinUI.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via the XAML framework and updates the title bar colors accordingly.
/// </remarks>
public class ThemeService : IThemeService
{
    private Themes _theme;
    private string _color;

    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => _theme == Themes.Light;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    public Themes Theme => _theme;

    /// <summary>
    /// Gets the current accent color as a hex string.
    /// </summary>
    public string AccentColor => _color;

    /// <summary>
    /// Occurs when the theme or accent color changes.
    /// </summary>
    public event EventHandler? ThemeChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="settingsService">An application settings service that provides persisted theme and color values.</param>
    /// <param name="themeColorOptions">Theme color options providing available colors and defaults.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    public ThemeService(
        ISettingsService settingsService,
        IOptions<ThemeColorOptions> themeColorOptions,
        ILogger<ThemeService> logger)
    {
        _settingsService = settingsService;
        _logger = logger;

        _theme = settingsService.LocalSettings.Theme;
        _color = settingsService.LocalSettings.Color;

        var opts = themeColorOptions.Value;
        if (opts.AvailableColors.Count == 1)
        {
            _color = opts.AvailableColors[0];
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
        else if (string.IsNullOrEmpty(_color))
        {
            _color = opts.AvailableColors.Count > 0 ? opts.AvailableColors[0] : ThemeColors.Default;
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
    }

    /// <summary>
    /// Applies the configured theme and accent color to the WinUI <see cref="Microsoft.UI.Xaml.Application"/> instance.
    /// </summary>
    /// <remarks>
    /// This method updates the XAML framework theme, sets the accent color, and customizes the title bar.
    /// It logs diagnostics if the Application.Current instance is not available.
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
            if (Application.Current.Resources.TryGetValue("SystemAccentColor", out var resource) && resource is Color primaryColor)
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
            UpdatePlatformColors();

            _logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set application style");
        }
    }

    /// <summary>
    /// Sets the application theme and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the theme, applies it live, persists the change, and raises the <see cref="ThemeChanged"/> event.
    /// It is safe to call on the UI thread.
    /// </remarks>
    /// <param name="theme">The theme to apply.</param>
    public void SetTheme(Themes theme)
    {
        if (_theme == theme)
            return;

        _theme = theme;
        _settingsService.LocalSettings.Theme = theme;
        _settingsService.SaveLocalSettings();

        ApplyTheme();
        ThemeChanged?.Invoke(this, EventArgs.Empty);

        _logger.LogInformation("Theme changed to {Theme}", theme);
    }

    /// <summary>
    /// Sets the accent color and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the accent color, applies it live, persists the change, and raises the <see cref="ThemeChanged"/> event.
    /// It is safe to call on the UI thread.
    /// </remarks>
    /// <param name="hexColor">The hex color string (e.g., "#0078D7" or "0078D7").</param>
    public void SetAccentColor(string hexColor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hexColor);

        if (string.Equals(_color, hexColor, StringComparison.OrdinalIgnoreCase))
            return;

        _color = hexColor;
        _settingsService.LocalSettings.Color = hexColor;
        _settingsService.SaveLocalSettings();

        ApplyTheme();
        ThemeChanged?.Invoke(this, EventArgs.Empty);

        _logger.LogInformation("Accent color changed to {Color}", hexColor);
    }

    /// <summary>
    /// Updates platform-specific colors and applies theme to the WinUI framework element.
    /// </summary>
    private void UpdatePlatformColors()
    {
        if (Application.MainWindow is null)
            return;

        if (Application.MainWindow.Content is FrameworkElement frameworkElement)
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

        SetTitlebar();
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
        var appWindow = WindowNativeExtensions.GetAppWindowForCurrentWindow(Application.MainWindow!);

        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");

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
    }
}
