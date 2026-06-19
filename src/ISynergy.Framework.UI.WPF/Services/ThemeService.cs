using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Services;

public class ThemeService : IThemeService
{
    private readonly Themes _theme;
    private readonly string _color;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<ThemeService> _logger;

    public bool IsLightThemeEnabled => _theme == Themes.Light;

    public ThemeService(
        ISettingsService settingsService,
        IOptions<ThemeColorOptions> themeColorOptions,
        ILogger<ThemeService> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
        _theme = settingsService.LocalSettings.Theme;

        var opts = themeColorOptions.Value;
        if (opts.AvailableColors.Count == 1)
        {
            _color = opts.AvailableColors[0];
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
        else if (string.IsNullOrEmpty(settingsService.LocalSettings.Color))
        {
            _color = opts.AvailableColors.Count > 0 ? opts.AvailableColors[0] : ThemeColors.Default;
            settingsService.LocalSettings.Color = _color;
            settingsService.SaveLocalSettings();
        }
        else
        {
            _color = settingsService.LocalSettings.Color;
        }
    }

    public void ApplyTheme()
    {
        _logger.LogInformation("Applying WPF theme: color={Color} theme={Theme}", _color, _theme);

        if (System.Windows.Application.Current is not { } app)
        {
            _logger.LogWarning("Application.Current not available");
            return;
        }

        try
        {
            // Parse hex color string (e.g. "#0078D7" or "0078D7")
            var mediaColor = ParseColor(_color);
            app.SetApplicationColor(mediaColor, _logger);
            _logger.LogInformation("WPF theme applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply WPF theme");
        }
    }

    private static Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
        {
            var r = Convert.ToByte(hex.Substring(0, 2), 16);
            var g = Convert.ToByte(hex.Substring(2, 2), 16);
            var b = Convert.ToByte(hex.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
        return Colors.Transparent;
    }
}
