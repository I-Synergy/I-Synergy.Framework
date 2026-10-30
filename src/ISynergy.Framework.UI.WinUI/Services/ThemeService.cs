using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.UI;
using ColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Provides functionality to apply the application's theme and accent color in WinUI.
/// </summary>
/// <remarks>
/// The service reads the persisted <see cref="Themes"/> and color value from <see cref="ISettingsService"/> during construction.
/// It then applies the theme via the XAML framework and updates the title bar colors accordingly.
/// </remarks>
public class ThemeService : BaseThemeService
{
    private FrameworkElement? _themeWatchedElement;

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
    /// Applies the configured theme and accent color to the WinUI <see cref="Microsoft.UI.Xaml.Application"/> instance.
    /// </summary>
    /// <remarks>
    /// This method updates the XAML framework theme, sets the accent color, and customizes the title bar.
    /// It logs diagnostics if the Application.Current instance is not available.
    /// </remarks>
    public override void ApplyTheme()
    {
        Logger.LogInformation("Setting application style with color {Color} and theme {Theme}",
            SelectedColor, Theme);

        Microsoft.UI.Xaml.Application? currentApplication;

        try
        {
            currentApplication = Microsoft.UI.Xaml.Application.Current;
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException)
        {
            // The WinUI runtime is not initialized (unit-test host, design-time, early shutdown).
            Logger.LogWarning(ex, "WinUI Application.Current is not available");
            return;
        }

        if (currentApplication is not Application application)
        {
            Logger.LogWarning("Application.Current is not available");
            return;
        }

        try
        {
            // Use the new ApplicationExtensions to dynamically load the theme
            application.SetApplicationColor(ColorHelper.GetColorFromHexString(SelectedColor));

            // Set the accent color from the loaded theme
            if (Application.Current.Resources.TryGetValue("SystemAccentColor", out var resource) && resource is Color primaryColor)
            {
                Application.AccentColor = primaryColor;
                Logger.LogTrace("AccentColor set to {Color}", primaryColor);
            }
            else
            {
                // Fallback to parsing the color from settings
                Application.AccentColor = ColorHelper.HexStringToColor(SelectedColor);
                Logger.LogTrace("AccentColor set to {Color} from settings", SelectedColor);
            }

            // Update platform-specific color resources
            UpdatePlatformColors();

            Logger.LogInformation("Application style successfully applied");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set application style");
        }
    }


    /// <summary>
    /// Updates platform-specific colors and applies theme to the WinUI framework element.
    /// </summary>
    private void UpdatePlatformColors()
    {
        if (Application.MainWindow is null)
            return;

        if (Application.MainWindow.Content is FrameworkElement frameworkElement)
        {
            switch (SelectedTheme)
            {
                case Themes.Light:
                    frameworkElement.RequestedTheme = ElementTheme.Light;
                    break;
                case Themes.Dark:
                    frameworkElement.RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    frameworkElement.RequestedTheme = ElementTheme.Default;
                    break;
            }

            // Remember the theme the root is actually rendering, so Theme reports the resolved
            // system theme before the first ActualThemeChanged event arrives.
            ResolvedSystemTheme = ToTheme(frameworkElement.ActualTheme);
            UpdateSystemThemeSubscription(frameworkElement);
        }

        SetTitlebar();
    }

    /// <summary>
    /// Subscribes to the window root's actual-theme changes while the application follows the
    /// system, and detaches when an explicit light or dark theme is selected.
    /// </summary>
    /// <param name="element">The current window root.</param>
    private void UpdateSystemThemeSubscription(FrameworkElement element)
    {
        if (SelectedTheme != Themes.Default)
        {
            DetachSystemThemeSubscription();
            return;
        }

        if (ReferenceEquals(_themeWatchedElement, element))
            return;

        DetachSystemThemeSubscription();
        element.ActualThemeChanged += OnActualThemeChanged;
        _themeWatchedElement = element;
        Logger.LogDebug("Watching the window root for operating system theme changes");
    }

    /// <summary>
    /// Re-applies the title bar colors when the operating system theme changes.
    /// </summary>
    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (SelectedTheme != Themes.Default)
            return;

        var resolvedTheme = ToTheme(sender.ActualTheme);

        if (resolvedTheme == ResolvedSystemTheme)
            return;

        ResolvedSystemTheme = resolvedTheme;
        Logger.LogInformation("Operating system theme changed to {Theme}", resolvedTheme);

        SetTitlebar();
        RaiseThemeChanged();
    }

    /// <summary>
    /// Detaches the actual-theme subscription from the watched root.
    /// </summary>
    private void DetachSystemThemeSubscription()
    {
        if (_themeWatchedElement is null)
            return;

        _themeWatchedElement.ActualThemeChanged -= OnActualThemeChanged;
        _themeWatchedElement = null;
    }

    /// <summary>
    /// Maps an <see cref="ElementTheme"/> to a concrete <see cref="Themes"/> value.
    /// </summary>
    private static Themes ToTheme(ElementTheme elementTheme) =>
        elementTheme == ElementTheme.Dark ? Themes.Dark : Themes.Light;

    /// <summary>
    /// Reads the theme Windows is currently using for applications.
    /// </summary>
    protected override Themes ResolveSystemTheme()
    {
        try
        {
            return Application.Current?.RequestedTheme == ApplicationTheme.Dark
                ? Themes.Dark
                : Themes.Light;
        }
        catch (Exception)
        {
            return Themes.Light;
        }
    }

    /// <summary>
    /// Setups the titlebar.
    /// </summary>
    private void SetTitlebar()
    {
        if (Application.MainWindow is null)
            return;

        var appWindow = WindowNativeExtensions.GetAppWindowForCurrentWindow(Application.MainWindow);

        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");

        if (File.Exists(iconPath))
            appWindow.SetIcon(iconPath);

        if (!AppWindowTitleBar.IsCustomizationSupported())
            return;

        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

        // The resolved theme is used instead of the raw setting: Themes.Default follows the
        // operating system, and painting white caption buttons over a dark shell is exactly what
        // made the min/max/close buttons look light themed.
        var isDark = Theme == Themes.Dark;

        var foreground = isDark ? Colors.White : Colors.Black;
        var hoverBackground = isDark
            ? Windows.UI.Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF)
            : Windows.UI.Color.FromArgb(0x14, 0x00, 0x00, 0x00);
        var pressedBackground = isDark
            ? Windows.UI.Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)
            : Windows.UI.Color.FromArgb(0x24, 0x00, 0x00, 0x00);

        // Transparent backgrounds let the caption buttons blend with the application's own title bar
        // instead of drawing an opaque box that contradicts the shell.
        appWindow.TitleBar.BackgroundColor = Colors.Transparent;
        appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ForegroundColor = foreground;
        appWindow.TitleBar.InactiveForegroundColor = foreground;

        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonForegroundColor = foreground;
        appWindow.TitleBar.ButtonInactiveForegroundColor = foreground;
        appWindow.TitleBar.ButtonHoverBackgroundColor = hoverBackground;
        appWindow.TitleBar.ButtonHoverForegroundColor = foreground;
        appWindow.TitleBar.ButtonPressedBackgroundColor = pressedBackground;
        appWindow.TitleBar.ButtonPressedForegroundColor = foreground;

        appWindow.TitleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
    }

    /// <inheritdoc />
    protected override void OnDispose() => DetachSystemThemeSubscription();
}
