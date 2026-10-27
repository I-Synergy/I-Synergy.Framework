using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Sample.Services;

/// <summary>
/// Manages dynamic accent color tokens that derive from the chosen primary accent color.
/// Updates AppAccentLight, AppAccentDark, AppAccentSoftLight, and AppAccentSoftDark
/// whenever the theme or accent color changes.
/// </summary>
/// <remarks>
/// The service listens to IThemeService.ThemeChanged and recomputes accent tokens on each change.
/// It must be initialized after the theme is applied via ApplyTheme().
/// </remarks>
public class ShellAccentTokens : IDisposable
{
    private readonly IThemeService _themeService;
    private readonly ILogger<ShellAccentTokens> _logger;
    private bool _disposed;

    public ShellAccentTokens(
        IThemeService themeService,
        ILogger<ShellAccentTokens> logger)
    {
        _themeService = themeService;
        _logger = logger;

        // Subscribe to theme/accent changes
        _themeService.ThemeChanged += OnThemeChanged;

        _logger.LogTrace("ShellAccentTokens initialized");
    }

    /// <summary>
    /// Initializes and applies accent tokens at startup.
    /// Call this immediately after IThemeService.ApplyTheme().
    /// </summary>
    public void ApplyAccentTokens()
    {
        _logger.LogTrace("Applying accent tokens at startup");
        UpdateAccentTokens();
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        _logger.LogTrace("ThemeChanged event received, updating accent tokens");
        UpdateAccentTokens();
    }

    /// <summary>
    /// Computes light and dark variants of the primary accent color and updates the resource dictionary.
    /// </summary>
    private void UpdateAccentTokens()
    {
        var app = Application.Current;
        if (app is null)
        {
            _logger.LogWarning("Application.Current is not available");
            return;
        }

        try
        {
            // Get the current accent hex color from the theme service
            var accentHex = _themeService.AccentColor;
            if (string.IsNullOrWhiteSpace(accentHex))
            {
                _logger.LogWarning("AccentColor not available from IThemeService");
                return;
            }

            // Parse the hex color
            var primaryColor = Color.FromArgb(accentHex);

            _logger.LogTrace("Computing accent tokens from AccentColor: {Color}", accentHex);

            // Compute light variant: use the primary color as-is for light mode
            // (it should be dark enough for contrast on light backgrounds)
            var lightVariant = primaryColor;

            // Compute dark variant: lighten the primary by adding luminosity
            // This ensures good contrast on dark backgrounds
            var darkVariant = primaryColor.AddLuminosity(0.30f);

            // Compute soft variants with opacity for selected rows/badges
            var softLight = ComputeSoftAccent(lightVariant, isLightMode: true);
            var softDark = ComputeSoftAccent(darkVariant, isLightMode: false);

            _logger.LogTrace(
                "Accent tokens computed: Light={Light}, Dark={Dark}, SoftLight={SoftLight}, SoftDark={SoftDark}",
                lightVariant, darkVariant, softLight, softDark);

            // Update the resource dictionary using string keys
            UpdateResourceInOwningDictionary(app.Resources, "AppAccentLight", lightVariant);
            UpdateResourceInOwningDictionary(app.Resources, "AppAccentDark", darkVariant);
            UpdateResourceInOwningDictionary(app.Resources, "AppAccentSoftLight", softLight);
            UpdateResourceInOwningDictionary(app.Resources, "AppAccentSoftDark", softDark);

            _logger.LogInformation("Accent tokens updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating accent tokens");
        }
    }

    /// <summary>
    /// Computes a soft accent color (low opacity) by blending the accent color with a background.
    /// </summary>
    /// <param name="accentColor">The accent color to soften.</param>
    /// <param name="isLightMode">True for light mode, false for dark mode.</param>
    /// <returns>The blended color.</returns>
    private Color ComputeSoftAccent(Color accentColor, bool isLightMode)
    {
        // Background colors for blending
        const string lightBg = "#F4F6F3";
        const string darkBg = "#0D1614";

        var bgColor = Color.FromArgb(isLightMode ? lightBg : darkBg);

        // Opacity for soft fill: ~12% for light mode, ~16% for dark mode
        float opacity = isLightMode ? 0.12f : 0.16f;

        // Blend the colors: result = accent * opacity + bg * (1 - opacity)
        // Color channels are floats in the 0..1 range.
        var blended = new Color(
            accentColor.Red * opacity + bgColor.Red * (1 - opacity),
            accentColor.Green * opacity + bgColor.Green * (1 - opacity),
            accentColor.Blue * opacity + bgColor.Blue * (1 - opacity),
            1.0f);

        return blended;
    }

    /// <summary>
    /// Updates a resource in the dictionary that owns the key, ensuring DynamicResource bindings
    /// are notified of the change.
    /// </summary>
    private static void UpdateResourceInOwningDictionary(
        ResourceDictionary resources,
        string key,
        object value)
    {
        // Traverse merged dictionaries to find the one that owns the key
        foreach (var dictionary in FlattenResourceDictionaries(resources))
        {
            if (!dictionary.TryGetValue(key, out _))
                continue;

            dictionary[key] = value;
            return;
        }

        // If not found in any merged dict, update in the root dict
        resources[key] = value;
    }

    /// <summary>
    /// Flattens a resource dictionary hierarchy into a sequence of dictionaries.
    /// </summary>
    private static IEnumerable<ResourceDictionary> FlattenResourceDictionaries(
        ResourceDictionary dictionary)
    {
        foreach (var merged in dictionary.MergedDictionaries)
        {
            foreach (var nested in FlattenResourceDictionaries(merged))
                yield return nested;
        }

        yield return dictionary;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _themeService.ThemeChanged -= OnThemeChanged;
        _logger.LogTrace("ShellAccentTokens disposed");
    }
}
