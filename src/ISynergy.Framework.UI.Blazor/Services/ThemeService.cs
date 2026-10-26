using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color in Blazor.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via the document element's data-theme attribute and CSS custom properties via JavaScript interop.
/// The service tolerates prerender scenarios (when JS interop is not available) and applies the theme on first interactive render.
/// </remarks>
public class ThemeService : IThemeService
{
    private Themes _theme;
    private string _color;
    private readonly ISettingsService _settingsService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ThemeService> _logger;
    private bool _isInitialized;

    /// <summary>
    /// Gets a value that indicates whether the light theme is enabled.
    /// </summary>
    /// <value><see langword="true"/> if the current theme equals <see cref="Themes.Light"/>; otherwise, <see langword="false"/>.</value>
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
    /// <param name="jsRuntime">A JavaScript interop runtime used to apply theme to the document.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    public ThemeService(
        ISettingsService settingsService,
        IJSRuntime jsRuntime,
        ILogger<ThemeService> logger)
    {
        _settingsService = settingsService;
        _jsRuntime = jsRuntime;
        _logger = logger;

        _theme = settingsService.LocalSettings.Theme;
        _color = settingsService.LocalSettings.Color;

        // Initialize color to a valid default if empty
        if (string.IsNullOrEmpty(_color))
        {
            _color = "#0078D7"; // Default Windows 11 blue
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
    }

    /// <summary>
    /// Applies the configured theme and accent color to the Blazor document.
    /// </summary>
    /// <remarks>
    /// This method is safe to call during prerender (when JS interop is unavailable) — it will
    /// not throw an exception and will instead apply the theme on the first interactive render.
    /// </remarks>
    public void ApplyTheme()
    {
        _logger.LogInformation("Setting application style with color {Color} and theme {Theme}",
            _color, _theme);

        try
        {
            _ = ApplyThemeAsync();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript"))
        {
            // Expected during prerender when JS interop is unavailable
            _logger.LogDebug(ex, "JavaScript interop not available during prerender; theme will be applied on first interactive render");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set application style");
        }
    }

    /// <summary>
    /// Applies the configured theme and accent color to the Blazor document asynchronously.
    /// </summary>
    private async Task ApplyThemeAsync()
    {
        try
        {
            // Ensure the JS module is loaded
            var module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/I-Synergy.Framework.UI.Blazor/js/theme_service_interop.js");

            // Normalize the color to ensure it has a # prefix
            var accentColor = _color.StartsWith("#") ? _color : "#" + _color;

            // Apply theme via JS interop
            await module.InvokeVoidAsync("applyTheme",
                IsLightThemeEnabled ? "light" : "dark",
                accentColor);

            if (!_isInitialized)
                _isInitialized = true;

            _logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("JSException") || ex.GetType().Name.Contains("JSDisabled"))
        {
            _logger.LogDebug(ex, "JavaScript interop error; theme application may not have succeeded");
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
}
