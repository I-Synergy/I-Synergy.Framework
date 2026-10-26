using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.ViewModels;

/// <summary>
/// Settings surface for language, theme and accent color.
/// Changes are applied live and persisted, matching the Sample.Maui settings drawer.
/// </summary>
public class SettingsViewModel : ViewModelBlade<object>
{
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private bool _isInitializing = true;

    /// <summary>
    /// Gets the title.
    /// </summary>
    public override string Title => _commonServices.LanguageService.GetString("Settings");

    /// <summary>
    /// Gets the supported languages.
    /// </summary>
    public ObservableCollection<CultureInfo> Languages { get; }

    /// <summary>
    /// Gets the available themes.
    /// </summary>
    public ObservableCollection<Themes> ThemeOptions { get; }

    /// <summary>
    /// Gets the sample accent palette (same four colors as Sample.Maui).
    /// </summary>
    public ObservableCollection<string> AccentColors { get; } = new()
    {
        "#0B5D55",
        "#0066CC",
        "#10B981",
        "#FF8C00"
    };

    /// <summary>
    /// Gets or sets the selected language. Applying a language happens immediately.
    /// </summary>
    public CultureInfo SelectedLanguage
    {
        get => GetValue<CultureInfo>();
        set
        {
            var changed = !Equals(GetValue<CultureInfo>(), value);
            SetValue(value);

            if (changed && !_isInitializing && value is not null)
            {
                value.ApplyCulture();
                _settingsService.LocalSettings.Language = value.Name;
                _settingsService.SaveLocalSettings();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected theme. Applying a theme happens immediately.
    /// </summary>
    public Themes SelectedTheme
    {
        get => GetValue<Themes>();
        set
        {
            var changed = !Equals(GetValue<Themes>(), value);
            SetValue(value);

            if (changed && !_isInitializing && _themeService.Theme != value)
                _themeService.SetTheme(value);
        }
    }

    /// <summary>
    /// Gets or sets the selected accent color. Applying a color happens immediately.
    /// </summary>
    public string SelectedAccentColor
    {
        get => GetValue<string>();
        set
        {
            var changed = !string.Equals(GetValue<string>(), value, StringComparison.OrdinalIgnoreCase);
            SetValue(value);

            if (changed && !_isInitializing && !string.IsNullOrWhiteSpace(value))
                _themeService.SetAccentColor(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel(
        ICommonServices commonServices,
        ISettingsService settingsService,
        IThemeService themeService,
        ISupportedCulturesService supportedCulturesService,
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        _settingsService = settingsService;
        _themeService = themeService;

        _settingsService.LoadLocalSettings();

        Languages = new ObservableCollection<CultureInfo>(supportedCulturesService.SupportedCultures);
        ThemeOptions = new ObservableCollection<Themes> { Themes.Light, Themes.Dark };

        SelectedLanguage = Languages.FirstOrDefault(culture =>
            string.Equals(culture.Name, _settingsService.LocalSettings.Language, StringComparison.OrdinalIgnoreCase))
            ?? supportedCulturesService.DefaultCulture;
        SelectedTheme = _settingsService.LocalSettings.Theme;
        SelectedAccentColor = string.IsNullOrWhiteSpace(_settingsService.LocalSettings.Color)
            ? AccentColors[0]
            : _settingsService.LocalSettings.Color;

        _isInitializing = false;
    }
}
