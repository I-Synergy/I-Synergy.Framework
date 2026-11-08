using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Services.Base;
using ISynergy.Framework.UI.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color across platforms.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via Application.UserAppTheme and sets the global Application.AccentColor.
/// Platform-specific resources (Android, Windows) are updated to keep native UI elements in sync.
/// </remarks>
public partial class ThemeService : BaseThemeService
{
    private const string Primary = nameof(Primary);
    private const string Secondary = nameof(Secondary);
    private const string Tertiary = nameof(Tertiary);
    private const string TintedGray050 = nameof(TintedGray050);
    private const string TintedGray900 = nameof(TintedGray900);

    private bool _isListeningToSystemTheme;

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
        : base(settingsService, themeColorOptions, logger, ResolveSystemTheme)
    {
    }

    /// <summary>
    /// Applies the configured theme and accent color to the MAUI <see cref="Application"/> instance.
    /// </summary>
    /// <remarks>
    /// The method safely handles a missing Application.Current instance and logs diagnostics.
    /// For Windows, it updates the title bar colors; for Android, it updates common color resources.
    /// </remarks>
    public override void ApplyTheme()
    {
        Logger.LogInformation("Setting application style with color {Color} and theme {Theme}",
            SelectedColor, SelectedTheme);

        if (Application.Current is not Application application)
        {
            Logger.LogWarning("Application.Current is not available");
            return;
        }

        try
        {
            // Themes.Default means "follow the platform": leaving UserAppTheme unspecified is what
            // lets MAUI track the operating system, including runtime changes.
            Application.Current.UserAppTheme = SelectedTheme switch
            {
                Themes.Light => AppTheme.Light,
                Themes.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            ResolvedSystemTheme = ResolveSystemTheme();
            UpdateSystemThemeSubscription();

            // Use the new ApplicationExtensions to dynamically load the theme
            application.SetApplicationColor(SelectedColor, Logger);

            // Set the accent color from the loaded theme
            var primaryColor = ResourceUtility.FindResource<Color>(Primary);
            Application.AccentColor = primaryColor;
            Logger.LogTrace("AccentColor set to {Color}", primaryColor);

            // Update platform-specific color resources
            UpdatePlatformColors(application);

            Logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set application style");
        }
    }

    /// <inheritdoc />
    protected override void OnAccentColorApplied(string hexColor)
    {
        // Ensure live updates for DynamicResource bindings by directly updating resource values
        // in the owning dictionary (following the AccentPalette pattern).
        if (Application.Current is { } application)
        {
            var newColor = Color.FromArgb(hexColor);
            UpdateResourceInOwningDictionary(application.Resources, Primary, newColor);
            UpdateResourceInOwningDictionary(application.Resources, Secondary, newColor);
            UpdateResourceInOwningDictionary(application.Resources, Tertiary, newColor);
            UpdateResourceInOwningDictionary(application.Resources, "AccentColor", newColor);
        }
    }


    /// <summary>
    /// Updates platform-specific color resources.
    /// </summary>
    /// <param name="application">An application instance whose resources are updated.</param>
    private void UpdatePlatformColors(Application application) // NOSONAR
    {
        if (Application.AccentColor is not null)
        {
            // Each head implements this in Platforms/<head>/Services/ThemeService.<head>.cs.
            ApplyPlatformColors(application);

            Logger.LogTrace("Platform-specific colors updated");
        }
    }

    /// <summary>
    /// Applies the accent colour to the native resources of the current head.
    /// </summary>
    /// <param name="application">The MAUI application whose resources are updated.</param>
    partial void ApplyPlatformColors(Application application);


    /// <summary>
    /// Updates an existing resource or adds it to the application resources when absent.
    /// </summary>
    /// <param name="application">An application instance whose resource dictionary is modified.</param>
    /// <param name="key">A resource key.</param>
    /// <param name="value">A resource value.</param>
    private static void UpdateOrAddResource(Application application, string key, object value) // NOSONAR
    {
        if (application.Resources.ContainsKey(key))
            application.Resources[key] = value;
        else
            application.Resources.Add(key, value);
    }

    /// <summary>
    /// Updates a resource value in the dictionary that owns the key, ensuring live updates
    /// for DynamicResource bindings.
    /// </summary>
    /// <remarks>
    /// This method traverses the resource dictionary hierarchy (including merged dictionaries)
    /// and updates the value in the dictionary that owns the given key. This pattern ensures
    /// that DynamicResource bindings are properly notified of the change.
    /// </remarks>
    /// <param name="resources">The application resource dictionary to search.</param>
    /// <param name="key">The resource key to update.</param>
    /// <param name="value">The new resource value.</param>
    private static void UpdateResourceInOwningDictionary(ResourceDictionary resources, string key, object value)
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
    /// <remarks>
    /// Merged dictionaries are traversed recursively, yielding nested dictionaries before
    /// the parent dictionary.
    /// </remarks>
    /// <param name="dictionary">The root resource dictionary to flatten.</param>
    /// <returns>A sequence of resource dictionaries from the hierarchy.</returns>
    private static IEnumerable<ResourceDictionary> FlattenResourceDictionaries(ResourceDictionary dictionary)
    {
        foreach (var merged in dictionary.MergedDictionaries)
        {
            foreach (var nested in FlattenResourceDictionaries(merged))
                yield return nested;
        }

        yield return dictionary;
    }

    /// <summary>
    /// Subscribes to the platform theme changes while the application follows the system, and
    /// detaches once an explicit light or dark theme is selected.
    /// </summary>
    private void UpdateSystemThemeSubscription()
    {
        if (Application.Current is not { } application)
            return;

        var shouldListen = SelectedTheme == Themes.Default;

        if (shouldListen == _isListeningToSystemTheme)
            return;

        if (shouldListen)
        {
            application.RequestedThemeChanged += OnRequestedThemeChanged;
            Logger.LogDebug("Watching platform theme changes");
        }
        else
        {
            application.RequestedThemeChanged -= OnRequestedThemeChanged;
        }

        _isListeningToSystemTheme = shouldListen;
    }

    /// <summary>
    /// Re-applies the palette and raises <see cref="BaseThemeService.ThemeChanged"/> when the platform theme changes.
    /// </summary>
    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (SelectedTheme != Themes.Default)
            return;

        var resolvedTheme = e.RequestedTheme == AppTheme.Dark ? Themes.Dark : Themes.Light;

        if (resolvedTheme == ResolvedSystemTheme)
            return;

        ResolvedSystemTheme = resolvedTheme;
        Logger.LogInformation("Platform theme changed to {Theme}", resolvedTheme);

        ApplyTheme();
        RaiseThemeChanged();
    }

    /// <summary>
    /// Reads the theme the operating system is using, independently of the application's user theme.
    /// </summary>
    private static Themes ResolveSystemTheme()
    {
        try
        {
            return Application.Current?.PlatformAppTheme == AppTheme.Dark
                ? Themes.Dark
                : Themes.Light;
        }
        catch (Exception)
        {
            return Themes.Light;
        }
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        if (_isListeningToSystemTheme && Application.Current is { } application)
        {
            application.RequestedThemeChanged -= OnRequestedThemeChanged;
            _isListeningToSystemTheme = false;
        }
    }
}
