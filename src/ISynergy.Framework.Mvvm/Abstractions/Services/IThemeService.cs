using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Provides functionality to manage application theme and accent color settings.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets a value indicating whether the light theme is currently enabled.
    /// </summary>
    bool IsLightThemeEnabled { get; }

    /// <summary>
    /// Gets the current application theme.
    /// </summary>
    Themes Theme { get; }

    /// <summary>
    /// Gets the current accent color as a hex string.
    /// </summary>
    string AccentColor { get; }

    /// <summary>
    /// Occurs when the theme or accent color changes.
    /// </summary>
    event EventHandler? ThemeChanged;

    /// <summary>
    /// Applies the persisted theme and accent color configuration to the application.
    /// </summary>
    /// <remarks>
    /// This method is typically called once during application startup to load the saved theme settings.
    /// </remarks>
    void ApplyTheme();

    /// <summary>
    /// Sets the application theme and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the theme, applies it live, persists the change via <see cref="ISynergy.Framework.Core.Abstractions.Services.ISettingsService"/>,
    /// and raises the <see cref="ThemeChanged"/> event.
    /// </remarks>
    /// <param name="theme">The theme to apply.</param>
    void SetTheme(Themes theme);

    /// <summary>
    /// Sets the accent color and applies it immediately.
    /// </summary>
    /// <remarks>
    /// This method updates the accent color, applies it live, persists the change via <see cref="ISynergy.Framework.Core.Abstractions.Services.ISettingsService"/>,
    /// and raises the <see cref="ThemeChanged"/> event.
    /// </remarks>
    /// <param name="hexColor">The hex color string (e.g., "#0078D7" or "0078D7").</param>
    void SetAccentColor(string hexColor);
}
