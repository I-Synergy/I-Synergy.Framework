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
public class ThemeService : IThemeService, IDisposable
{
    private Themes _theme;
    private string _color;
    private Themes _resolvedSystemTheme = Themes.Light;
    private readonly ISettingsService _settingsService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ThemeService> _logger;
    private DotNetObjectReference<ThemeService>? _dotNetReference;
    private bool _isWatchingSystemTheme;
    private bool _isInitialized;
    private bool _disposed;

    /// <summary>
    /// Gets a value that indicates whether the light theme is enabled.
    /// </summary>
    /// <value><see langword="true"/> if the resolved theme is light; otherwise, <see langword="false"/>.</value>
    public bool IsLightThemeEnabled => Theme == Themes.Light;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    /// <remarks>
    /// <see cref="Themes.Default"/> is resolved to the color scheme the browser reports, so
    /// consumers never have to interpret it themselves. The resolved value is refreshed when the
    /// operating system color scheme changes.
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

            // Apply theme via JS interop. "system" makes the document follow the browser/OS
            // color scheme; the module resolves it and returns the concrete theme it applied.
            var resolvedTheme = await module.InvokeAsync<string>("applyTheme",
                ToJsPreference(_theme),
                accentColor);

            var systemThemeChanged = _theme == Themes.Default
                && TryParseTheme(resolvedTheme, out var resolved)
                && resolved != _resolvedSystemTheme;

            if (TryParseTheme(resolvedTheme, out var concreteTheme))
                _resolvedSystemTheme = concreteTheme;

            await EnsureSystemThemeWatcherAsync(module);

            if (!_isInitialized)
                _isInitialized = true;

            _logger.LogInformation("Application style successfully applied");

            if (systemThemeChanged)
                ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("JSException") || ex.GetType().Name.Contains("JSDisabled"))
        {
            _logger.LogDebug(ex, "JavaScript interop error; theme application may not have succeeded");
        }
    }

    /// <summary>
    /// Maps a theme preference to the value understood by the interop module.
    /// </summary>
    private static string ToJsPreference(Themes theme) => theme switch
    {
        Themes.Light => "light",
        Themes.Dark => "dark",
        _ => "system"
    };

    /// <summary>
    /// Parses the concrete theme name returned by the interop module.
    /// </summary>
    private static bool TryParseTheme(string? themeName, out Themes theme) =>
        Enum.TryParse(themeName, ignoreCase: true, out theme);

    /// <summary>
    /// Registers the JavaScript color-scheme listener while the theme follows the system.
    /// </summary>
    private async Task EnsureSystemThemeWatcherAsync(IJSObjectReference module)
    {
        if (_theme != Themes.Default)
        {
            // The listener stays registered but ignores non-system preferences, so switching back
            // to system does not need a second registration.
            _isWatchingSystemTheme = false;
            return;
        }

        if (_isWatchingSystemTheme)
            return;

        _dotNetReference ??= DotNetObjectReference.Create(this);

        try
        {
            await module.InvokeVoidAsync("watchSystemTheme", _dotNetReference);
            _isWatchingSystemTheme = true;
        }
        catch (JSException ex)
        {
            _logger.LogDebug(ex, "The JavaScript color-scheme listener could not be registered");
        }
    }

    /// <summary>
    /// Called from the interop module when the operating system color scheme changes.
    /// </summary>
    /// <param name="isDark"><see langword="true"/> when the system switched to dark mode.</param>
    [JSInvokable]
    public Task OnSystemThemeChanged(bool isDark)
    {
        if (_theme != Themes.Default)
            return Task.CompletedTask;

        var resolvedTheme = isDark ? Themes.Dark : Themes.Light;

        if (resolvedTheme == _resolvedSystemTheme)
            return Task.CompletedTask;

        _resolvedSystemTheme = resolvedTheme;
        _logger.LogInformation("Operating system color scheme changed to {Theme}", resolvedTheme);

        ThemeChanged?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
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
    /// Releases the .NET reference shared with the JavaScript color-scheme listener.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the interop reference.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _dotNetReference?.Dispose();
            _dotNetReference = null;
            _isWatchingSystemTheme = false;
        }

        _disposed = true;
    }
}
