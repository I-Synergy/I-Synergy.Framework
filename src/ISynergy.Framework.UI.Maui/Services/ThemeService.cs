using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Utilities;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color across platforms.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via Application.UserAppTheme and sets the global Application.AccentColor.
/// Platform-specific resources (Android, Windows) are updated to keep native UI elements in sync.
/// </remarks>
public class ThemeService : IThemeService
{
    private const string Primary = nameof(Primary);
    private const string Secondary = nameof(Secondary);
    private const string Tertiary = nameof(Tertiary);
    private const string TintedGray000 = nameof(TintedGray000);
    private const string TintedGray950 = nameof(TintedGray950);

    private Themes _theme;
    private string _color;
    private readonly ILocalSettings _localSettings;
    private readonly ILogger<ThemeService> _logger;

    /// <summary>
    /// Gets a value that indicates whether the light theme is enabled.
    /// </summary>
    /// <value><see langword="true"/> if the current theme equals <see cref="Themes.Light"/>; otherwise, <see langword="false"/>.</value>
    public bool IsLightThemeEnabled => _theme == Themes.Light;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="localSettings">An application settings service that provides persisted theme and color values.</param>
    /// <param name="logger">A logger used for diagnostics and structured logging.</param>
    public ThemeService(
        ILocalSettings localSettings,
        ILogger<ThemeService> logger)
    {
        _localSettings = localSettings;
        _logger = logger;

        _theme = localSettings.Theme;
        _color = localSettings.Color;
    }

    /// <summary>
    /// Applies the configured theme and accent color to the MAUI <see cref="Application"/> instance.
    /// </summary>
    /// <remarks>
    /// The method safely handles a missing Application.Current instance and logs diagnostics.
    /// For Windows, it updates the title bar colors; for Android, it updates common color resources.
    /// </remarks>
    public void ApplyTheme()
    {
        _logger.LogInformation("Setting application style with color {Color} and theme {Theme}",
            _color, _theme);

        if (Application.Current is not Application application)
        {
            _logger.LogWarning("Application.Current is not available");
            return;
        }

        try
        {
            if (IsLightThemeEnabled)
                Application.Current.UserAppTheme = AppTheme.Light;
            else
                Application.Current.UserAppTheme = AppTheme.Dark;

            // Use the new ApplicationExtensions to dynamically load the theme
            application.SetApplicationColor(_color, _logger);

            // Set the accent color from the loaded theme
            if (ResourceUtility.FindResource<Color>(Primary) is Color primaryColor)
            {
                Application.AccentColor = primaryColor;
                _logger.LogTrace("AccentColor set to {Color}", primaryColor);
            }
            else
            {
                // Fallback to parsing the color from settings
                Application.AccentColor = Color.FromArgb(_color);
                _logger.LogTrace("AccentColor set to {Color} from settings", _color);
            }

            // Update platform-specific color resources
            UpdatePlatformColors(application);

            _logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set application style");
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
            _logger.LogTrace("Updating Android-specific colors");

            UpdateOrAddResource(application, "colorPrimary", Application.AccentColor);
            UpdateOrAddResource(application, "colorAccent", Application.AccentColor);
            UpdateOrAddResource(application, "colorPrimaryDark", Application.AccentColor.AddLuminosity(-0.25f));
#endif

#if WINDOWS
            _logger.LogTrace("Updating Windows-specific colors");

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
            _logger.LogTrace("Platform-specific colors updated");
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
                _logger.LogTrace("Windows title bar not available for customization");
                return;
            }

            // Use the page background color (TintedGray000 light / TintedGray950 dark)
            // so the title bar blends seamlessly with the page content.
            var backgroundResourceKey = IsLightThemeEnabled ? TintedGray000 : TintedGray950;
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

                _logger.LogTrace("Windows title bar colors updated to page background ({Theme})",
                    IsLightThemeEnabled ? "Light" : "Dark");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update Windows title bar colors");
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
}
