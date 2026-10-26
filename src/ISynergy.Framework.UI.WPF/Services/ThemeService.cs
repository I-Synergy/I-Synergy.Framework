using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color in WPF.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via resource dictionaries and sets the global application accent color.
/// <see cref="Themes.Default"/> follows the operating system's application theme.
/// </remarks>
public class ThemeService : IThemeService, IDisposable
{
    private const string _personalizeKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string _appsUseLightThemeValue = "AppsUseLightTheme";

    private Themes _theme;
    private string _color;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;
    private Themes _lastEffectiveTheme;
    private bool _isListeningToSystemTheme;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether the light theme is enabled.
    /// </summary>
    public bool IsLightThemeEnabled => Theme == Themes.Light;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    /// <remarks>
    /// <see cref="Themes.Default"/> is resolved to the theme the operating system reports, so
    /// consumers never have to interpret it themselves. Treating it as light in one place and as
    /// dark in another is what made the shell start with light surfaces while its toggle believed
    /// the dark theme was active.
    /// </remarks>
    public Themes Theme => _theme == Themes.Default ? GetSystemTheme() : _theme;

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

        var opts = themeColorOptions.Value;
        if (opts.AvailableColors.Count == 1)
        {
            _color = opts.AvailableColors[0];
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
        else if (string.IsNullOrEmpty(settingsService.LocalSettings.Color))
        {
            _color = opts.AvailableColors.Count > 0 ? opts.AvailableColors[0] : ThemeColors.Default;
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
        else
        {
            _color = settingsService.LocalSettings.Color;
        }

        _lastEffectiveTheme = Theme;
        UpdateSystemThemeSubscription();
    }

    /// <summary>
    /// Applies the configured theme and accent color to the WPF <see cref="System.Windows.Application"/> instance.
    /// </summary>
    /// <remarks>
    /// This method is called during application startup and when theme or accent color is changed.
    /// It updates resource dictionaries and logs any errors that occur during application.
    /// </remarks>
    public void ApplyTheme()
    {
        _logger.LogInformation("Applying WPF theme: color={Color} theme={Theme}", _color, _theme);

        if (System.Windows.Application.Current is not { } app)
        {
            _logger.LogWarning("Application.Current not available");
            return;
        }

        try
        {
            // Parse hex color string (e.g. "#0078D7" or "0078D7")
            var mediaColor = ParseColor(_color);
            app.SetApplicationColor(mediaColor, _logger, Theme);
            _logger.LogInformation("WPF theme applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply WPF theme");
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

        UpdateSystemThemeSubscription();
        _lastEffectiveTheme = Theme;
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
    /// Subscribes to operating system preference changes while the theme follows the system, so an
    /// OS theme switch is reflected without restarting the application.
    /// </summary>
    private void UpdateSystemThemeSubscription()
    {
        var shouldListen = _theme == Themes.Default;

        if (shouldListen == _isListeningToSystemTheme)
            return;

        try
        {
            if (shouldListen)
            {
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                _isListeningToSystemTheme = true;
                _logger.LogDebug("Watching operating system theme changes");
            }
            else
            {
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
                _isListeningToSystemTheme = false;
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or ExternalException)
        {
            _logger.LogWarning(ex, "The operating system theme cannot be watched in this host");
        }
    }

    /// <summary>
    /// Re-applies the theme when the operating system's application theme changes.
    /// </summary>
    /// <remarks>
    /// <see cref="SystemEvents"/> raises this event from its own thread, while resource dictionaries
    /// and the palette tokens must be touched on the UI thread.
    /// </remarks>
    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category != UserPreferenceCategory.General)
            return;

        var dispatcher = System.Windows.Application.Current?.Dispatcher;

        if (dispatcher is null || dispatcher.HasShutdownStarted)
            return;

        dispatcher.Invoke(() =>
        {
            var effectiveTheme = Theme;

            if (effectiveTheme == _lastEffectiveTheme)
                return;

            _lastEffectiveTheme = effectiveTheme;
            _logger.LogInformation("Operating system theme changed to {Theme}", effectiveTheme);

            ApplyTheme();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        });
    }

    /// <summary>
    /// Resolves the operating system's application theme.
    /// </summary>
    /// <remarks>
    /// WPF has no API that reports the resolved OS app theme, so the personalization value Windows
    /// exposes is read instead. Light is used when the value is unavailable (older Windows builds,
    /// policy-locked or non-Windows hosts), matching the Windows default.
    /// </remarks>
    /// <returns><see cref="Themes.Dark"/> when the OS uses dark app mode; otherwise <see cref="Themes.Light"/>.</returns>
    private static Themes GetSystemTheme()
    {
        try
        {
            var value = Registry.GetValue(_personalizeKey, _appsUseLightThemeValue, 1);
            return value switch
            {
                0 or 0L => Themes.Dark,
                _ => Themes.Light
            };
        }
        catch (Exception ex) when (ex is IOException or SecurityException or UnauthorizedAccessException)
        {
            return Themes.Light;
        }
    }

    /// <summary>
    /// Parses a hex color string and returns a WPF Color object.
    /// </summary>
    /// <param name="hex">The hex color string (e.g., "#0078D7" or "0078D7").</param>
    /// <returns>A <see cref="Color"/> object, or <see cref="Colors.Transparent"/> if parsing fails.</returns>
    private static Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
        {
            var r = Convert.ToByte(hex.Substring(0, 2), 16);
            var g = Convert.ToByte(hex.Substring(2, 2), 16);
            var b = Convert.ToByte(hex.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
        return Colors.Transparent;
    }

    /// <summary>
    /// Stops watching the operating system theme and releases the subscription.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the operating system theme subscription.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing && _isListeningToSystemTheme)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            _isListeningToSystemTheme = false;
        }

        _disposed = true;
    }
}
