using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.UI.Services.Base;

/// <summary>
/// Shared theme state and persistence for the platform theme services. Platform subclasses provide
/// <see cref="ApplyTheme"/>, a system theme resolver and any native resource updates.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/>
/// during construction, then keeps them in sync whenever the user changes the theme or accent color.
/// </remarks>
public abstract class BaseThemeService : IThemeService, IDisposable
{
    private Themes _theme;
    private string _color;

    private readonly ISettingsService _settingsService;
    private readonly ILogger _logger;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    public bool IsLightThemeEnabled => Theme == Themes.Light;

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    /// <remarks>
    /// <see cref="Themes.Default"/> is resolved to the platform theme, so consumers never have to
    /// interpret it themselves. The resolved value is refreshed when the operating system theme changes.
    /// </remarks>
    public Themes Theme => _theme == Themes.Default ? ResolvedSystemTheme : _theme;

    /// <summary>
    /// Gets the current accent color as a hex string.
    /// </summary>
    public string AccentColor => _color;

    /// <summary>
    /// Occurs when the theme or accent color changes.
    /// </summary>
    public event EventHandler? ThemeChanged;

    /// <summary>
    /// Gets the settings service that persists the theme and color.
    /// </summary>
    protected ISettingsService SettingsService => _settingsService;

    /// <summary>
    /// Gets the logger used by the platform implementation.
    /// </summary>
    protected ILogger Logger => _logger;

    /// <summary>
    /// Gets the theme exactly as selected by the user (which may be <see cref="Themes.Default"/>).
    /// </summary>
    protected Themes SelectedTheme => _theme;

    /// <summary>
    /// Gets the accent color exactly as selected by the user.
    /// </summary>
    protected string SelectedColor => _color;

    /// <summary>
    /// Gets or sets the theme the platform currently resolves for <see cref="Themes.Default"/>.
    /// </summary>
    protected Themes ResolvedSystemTheme { get; set; }

    /// <summary>
    /// Gets a value indicating whether the service has been disposed.
    /// </summary>
    protected bool IsDisposed => _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseThemeService"/> class.
    /// </summary>
    /// <param name="settingsService">An application settings service that provides persisted theme and color values.</param>
    /// <param name="themeColorOptions">Theme color options providing available colors and defaults.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    /// <param name="resolveSystemTheme">
    /// Platform resolver for the operating system theme. Passed as a delegate instead of calling an
    /// overridable member from the constructor.
    /// </param>
    protected BaseThemeService(
        ISettingsService settingsService,
        IOptions<ThemeColorOptions> themeColorOptions,
        ILogger logger,
        Func<Themes> resolveSystemTheme)
    {
        _settingsService = settingsService;
        _logger = logger;

        _theme = settingsService.LocalSettings.Theme;
        _color = settingsService.LocalSettings.Color;
        ResolvedSystemTheme = resolveSystemTheme();

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
    /// Applies the configured theme and accent color to the platform application.
    /// </summary>
    public abstract void ApplyTheme();

    /// <summary>
    /// Sets the application theme and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the theme, applies it live, persists the change, and raises the
    /// <see cref="ThemeChanged"/> event. It is safe to call on the UI thread.
    /// </remarks>
    /// <param name="theme">The theme to apply.</param>
    public void SetTheme(Themes theme)
    {
        // An explicit choice is persisted even when it matches the resolved platform theme, so the
        // application stops following the OS once the user has picked light or dark.
        if (_theme == theme && theme != Themes.Default)
            return;

        _theme = theme;
        _settingsService.LocalSettings.Theme = theme;
        _settingsService.SaveLocalSettings();

        ApplyTheme();
        RaiseThemeChanged();

        _logger.LogInformation("Theme changed to {Theme}", theme);
    }

    /// <summary>
    /// Sets the accent color and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the accent color, applies it live, persists the change, and raises the
    /// <see cref="ThemeChanged"/> event. It is safe to call on the UI thread.
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
        OnAccentColorApplied(hexColor);
        RaiseThemeChanged();

        _logger.LogInformation("Accent color changed to {Color}", hexColor);
    }

    /// <summary>
    /// Updates platform resource values for a new accent color.
    /// </summary>
    /// <param name="hexColor">The new accent color.</param>
    protected virtual void OnAccentColorApplied(string hexColor)
    {
    }

    /// <summary>
    /// Raises the <see cref="ThemeChanged"/> event.
    /// </summary>
    protected void RaiseThemeChanged() => ThemeChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Releases the platform resources held by the service.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the platform resources held by the service.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            OnDispose();

        _disposed = true;
    }

    /// <summary>
    /// Detaches platform subscriptions when the service is disposed.
    /// </summary>
    protected virtual void OnDispose()
    {
    }
}
