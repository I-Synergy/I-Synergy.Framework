using ISynergy.Framework.UI.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Windows-specific half of <see cref="ThemeService"/>: WinUI accent resources and the native title
/// bar colours. Lives in Platforms/Windows, which the MAUI SDK only compiles for Windows targets, so
/// the shared file needs no conditional compilation.
/// </summary>
public partial class ThemeService
{
    /// <inheritdoc />
    partial void ApplyPlatformColors(Application application)
    {
        Logger.LogTrace("Updating Windows-specific colors");

        var accent = Application.AccentColor;

        if (accent is null)
            return;

        UpdateOrAddResource(application, "SystemAccentColor", accent);
        UpdateOrAddResource(application, "SystemColorControlAccentColor", accent);
        UpdateOrAddResource(application, "SystemAccentColorDark1", accent);
        UpdateOrAddResource(application, "SystemAccentColorDark2", accent);
        UpdateOrAddResource(application, "SystemAccentColorDark3", accent);
        UpdateOrAddResource(application, "SystemAccentColorLight1", accent);
        UpdateOrAddResource(application, "SystemAccentColorLight2", accent);
        UpdateOrAddResource(application, "SystemAccentColorLight3", accent);

        // Register WinUI text control focus border overrides.
        ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions.ControlAccentStyler.Register();

        UpdateWindowsTitleBar();
    }

    /// <summary>
    /// Updates Windows title bar colors to match the current accent color.
    /// </summary>
    private void UpdateWindowsTitleBar()
    {
        try
        {
            var window = (Microsoft.Maui.Controls.Application.Current?.Windows is { Count: > 0 } windows ? windows[0].Handler?.PlatformView : null) as Microsoft.UI.Xaml.Window;

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
                if (Microsoft.Maui.Controls.Application.Current?.Windows is { Count: > 0 } titleBarWindows && titleBarWindows[0] is Microsoft.Maui.Controls.Window mauiWindow
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
}
