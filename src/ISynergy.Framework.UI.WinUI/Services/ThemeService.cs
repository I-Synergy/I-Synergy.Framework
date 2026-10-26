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
using System.Runtime.InteropServices;
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
public class ThemeService : IThemeService, IDisposable
{
    private Themes _theme;
    private string _color;
    private Themes _resolvedSystemTheme;

    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;
    private FrameworkElement? _themeWatchedElement;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    public bool IsLightThemeEnabled => Theme == Themes.Light;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    /// <remarks>
    /// <see cref="Themes.Default"/> is resolved to the theme the window root actually renders, so
    /// consumers never have to interpret it themselves. The resolved value is refreshed when the
    /// operating system theme changes.
    /// </remarks>
    public Themes Theme => _theme == Themes.Default ? _resolvedSystemTheme : _theme;

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
        _resolvedSystemTheme = ResolveSystemTheme();

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
            _color, Theme);

        Microsoft.UI.Xaml.Application? currentApplication;

        try
        {
            currentApplication = Microsoft.UI.Xaml.Application.Current;
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException)
        {
            // The WinUI runtime is not initialized (unit-test host, design-time, early shutdown).
            _logger.LogWarning(ex, "WinUI Application.Current is not available");
            return;
        }

        if (currentApplication is not Application application)
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
        // An explicit choice is persisted even when it matches the resolved system theme, so the
        // application stops following the OS once the user has picked light or dark.
        if (_theme == theme && theme != Themes.Default)
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

            // Remember the theme the root is actually rendering, so Theme reports the resolved
            // system theme before the first ActualThemeChanged event arrives.
            _resolvedSystemTheme = ToTheme(frameworkElement.ActualTheme);
            UpdateSystemThemeSubscription(frameworkElement);
        }

        SetTitlebar();
    }

    /// <summary>
    /// Subscribes to the window root's actual-theme changes while the application follows the
    /// system, and detaches when an explicit light or dark theme is selected.
    /// </summary>
    /// <param name="element">The current window root.</param>
    private void UpdateSystemThemeSubscription(FrameworkElement element)
    {
        if (_theme != Themes.Default)
        {
            DetachSystemThemeSubscription();
            return;
        }

        if (ReferenceEquals(_themeWatchedElement, element))
            return;

        DetachSystemThemeSubscription();
        element.ActualThemeChanged += OnActualThemeChanged;
        _themeWatchedElement = element;
        _logger.LogDebug("Watching the window root for operating system theme changes");
    }

    /// <summary>
    /// Re-applies the title bar colors when the operating system theme changes.
    /// </summary>
    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_theme != Themes.Default)
            return;

        var resolvedTheme = ToTheme(sender.ActualTheme);

        if (resolvedTheme == _resolvedSystemTheme)
            return;

        _resolvedSystemTheme = resolvedTheme;
        _logger.LogInformation("Operating system theme changed to {Theme}", resolvedTheme);

        SetTitlebar();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Detaches the actual-theme subscription from the watched root.
    /// </summary>
    private void DetachSystemThemeSubscription()
    {
        if (_themeWatchedElement is null)
            return;

        _themeWatchedElement.ActualThemeChanged -= OnActualThemeChanged;
        _themeWatchedElement = null;
    }

    /// <summary>
    /// Maps an <see cref="ElementTheme"/> to a concrete <see cref="Themes"/> value.
    /// </summary>
    private static Themes ToTheme(ElementTheme elementTheme) =>
        elementTheme == ElementTheme.Dark ? Themes.Dark : Themes.Light;

    /// <summary>
    /// Reads the theme Windows is currently using for applications.
    /// </summary>
    private static Themes ResolveSystemTheme()
    {
        try
        {
            return Application.Current?.RequestedTheme == ApplicationTheme.Dark
                ? Themes.Dark
                : Themes.Light;
        }
        catch (Exception)
        {
            return Themes.Light;
        }
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
        if (Application.MainWindow is null)
            return;

        var appWindow = WindowNativeExtensions.GetAppWindowForCurrentWindow(Application.MainWindow);

        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");

        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (!AppWindowTitleBar.IsCustomizationSupported())
            return;

        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

        // The resolved theme is used instead of the raw setting: Themes.Default follows the
        // operating system, and painting white caption buttons over a dark shell is exactly what
        // made the min/max/close buttons look light themed.
        var isDark = Theme == Themes.Dark;

        var foreground = isDark ? Colors.White : Colors.Black;
        var hoverBackground = isDark
            ? Windows.UI.Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF)
            : Windows.UI.Color.FromArgb(0x14, 0x00, 0x00, 0x00);
        var pressedBackground = isDark
            ? Windows.UI.Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)
            : Windows.UI.Color.FromArgb(0x24, 0x00, 0x00, 0x00);

        // Transparent backgrounds let the caption buttons blend with the application's own title bar
        // instead of drawing an opaque box that contradicts the shell.
        appWindow.TitleBar.BackgroundColor = Colors.Transparent;
        appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ForegroundColor = foreground;
        appWindow.TitleBar.InactiveForegroundColor = foreground;

        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonForegroundColor = foreground;
        appWindow.TitleBar.ButtonInactiveForegroundColor = foreground;
        appWindow.TitleBar.ButtonHoverBackgroundColor = hoverBackground;
        appWindow.TitleBar.ButtonHoverForegroundColor = foreground;
        appWindow.TitleBar.ButtonPressedBackgroundColor = pressedBackground;
        appWindow.TitleBar.ButtonPressedForegroundColor = foreground;

        appWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
    }

    /// <summary>
    /// Stops watching the window root and releases the subscription.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the actual-theme subscription.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            DetachSystemThemeSubscription();

        _disposed = true;
    }
}
