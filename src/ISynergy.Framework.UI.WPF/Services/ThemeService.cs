using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color in WPF.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via resource dictionaries and sets the global application accent color.
/// </remarks>
public class ThemeService : IThemeService
{
    private Themes _theme;
    private string _color;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;

    /// <summary>
    /// Gets a value indicating whether the light theme is enabled.
    /// </summary>
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
            app.SetApplicationColor(mediaColor, _logger);
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
}
