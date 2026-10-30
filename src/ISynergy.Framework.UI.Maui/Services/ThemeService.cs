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
public class ThemeService : BaseThemeService
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
        : base(settingsService, themeColorOptions, logger)
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
#if ANDROID
            Logger.LogTrace("Updating Android-specific colors");

            UpdateOrAddResource(application, "colorPrimary", Application.AccentColor);
            UpdateOrAddResource(application, "colorAccent", Application.AccentColor);
            UpdateOrAddResource(application, "colorPrimaryDark", Application.AccentColor.AddLuminosity(-0.25f));
#endif

#if IOS
            Logger.LogTrace("Updating iOS-specific colors");

            UIKit.UIView.Appearance.TintColor = new UIKit.UIColor(
                (nfloat)Application.AccentColor.Red,
                (nfloat)Application.AccentColor.Green,
                (nfloat)Application.AccentColor.Blue,
                1.0f);
#endif

#if WINDOWS
            Logger.LogTrace("Updating Windows-specific colors");

            UpdateOrAddResource(application, "SystemAccentColor", Application.AccentColor);
            UpdateOrAddResource(application, "SystemColorControlAccentColor", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorDark1", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorDark2", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorDark3", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorLight1", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorLight2", Application.AccentColor);
            UpdateOrAddResource(application, "SystemAccentColorLight3", Application.AccentColor);

            // Register WinUI text control focus border overrides.
            ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions.ControlAccentStyler.Register();

            // Update Windows title bar colors
            UpdateWindowsTitleBar();
#endif
            Logger.LogTrace("Platform-specific colors updated");
        }
    }

#if WINDOWS
    /// <summary>
    /// Updates Windows title bar colors to match the current accent color.
    /// </summary>
    private void UpdateWindowsTitleBar()
    {
        try
        {
            var window = Microsoft.Maui.Controls.Application.Current?.Windows?.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

            if (window?.AppWindow?.TitleBar is null)
            {
                Logger.LogTrace("Windows title bar not available for customization");
                return;
            }

            // Use the page background color (TintedGray000 light / TintedGray950 dark)
            // so the title bar blends seamlessly with the page content.
            var backgroundResourceKey = IsLightThemeEnabled ? TintedGray050 : TintedGray900;
            var pageBackgroundColor = ResourceUtility.FindResource<Color>(backgroundResourceKey);

            if (pageBackgroundColor is not null)
            {
                // Convert MAUI Color to Windows.UI.Color (use global:: to avoid namespace conflict)
                var winColor = global::Windows.UI.Color.FromArgb(
                    (byte)(pageBackgroundColor.Alpha * 255),
                    (byte)(pageBackgroundColor.Red * 255),
                    (byte)(pageBackgroundColor.Green * 255),
                    (byte)(pageBackgroundColor.Blue * 255)
                 );

                var titleBar = window.AppWindow.TitleBar;

                // Set native title bar button colors to match page background
                titleBar.ButtonBackgroundColor = winColor;
                titleBar.ButtonInactiveBackgroundColor = winColor;
                titleBar.BackgroundColor = winColor;

                // Set text/foreground colors based on theme for contrast
                var foregroundColor = IsLightThemeEnabled
                    ? global::Windows.UI.Color.FromArgb(255, 0, 0, 0)       // Black text on light background
                    : global::Windows.UI.Color.FromArgb(255, 255, 255, 255); // White text on dark background

                titleBar.ForegroundColor = foregroundColor;
                titleBar.ButtonForegroundColor = foregroundColor;
                titleBar.ButtonHoverForegroundColor = foregroundColor;
                titleBar.ButtonPressedForegroundColor = foregroundColor;

                // Also set the MAUI TitleBar colors, which renders
                // the title bar area when Window.TitleBar is assigned.
                if (Microsoft.Maui.Controls.Application.Current?.Windows?.FirstOrDefault() is Microsoft.Maui.Controls.Window mauiWindow
                    && mauiWindow.TitleBar is Microsoft.Maui.Controls.TitleBar mauiTitleBar)
                {
                    mauiTitleBar.BackgroundColor = pageBackgroundColor;
                    mauiTitleBar.ForegroundColor = IsLightThemeEnabled ? Colors.Black : Colors.White;
                }

                Logger.LogTrace("Windows title bar colors updated to page background ({Theme})",
                    IsLightThemeEnabled ? "Light" : "Dark");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update Windows title bar colors");
        }
    }
#endif

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
    /// Re-applies the palette and raises <see cref="ThemeChanged"/> when the platform theme changes.
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
    protected override Themes ResolveSystemTheme()
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
