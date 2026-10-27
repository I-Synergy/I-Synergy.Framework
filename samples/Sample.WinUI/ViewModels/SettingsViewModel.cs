using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.ViewModels;

/// <summary>
/// Settings blade for language, theme and accent color.
/// Changes are applied live, matching the Sample.Maui settings drawer.
/// </summary>
public class SettingsViewModel : ViewModelBlade<object>
{
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private readonly ISupportedCulturesService _supportedCulturesService;
    private bool _isInitializing = true;

    /// <summary>
    /// Gets the title.
    /// </summary>
    public override string Title => _commonServices.LanguageService.GetString("Settings");

    /// <summary>
    /// Gets or sets the LocalSettings property value.
    /// </summary>
    public LocalSettings LocalSettings
    {
        get => GetValue<LocalSettings>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the GlobalSettings property value.
    /// </summary>
    public GlobalSettings GlobalSettings
    {
        get => GetValue<GlobalSettings>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the selected theme.
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
    /// Gets or sets the available themes.
    /// </summary>
    public ObservableCollection<Themes> AvailableThemes { get; }

    /// <summary>
    /// Gets or sets the selected language.
    /// </summary>
    public string SelectedLanguage
    {
        get => GetValue<string>();
        set
        {
            var changed = !string.Equals(GetValue<string>(), value, StringComparison.OrdinalIgnoreCase);
            SetValue(value);

            if (changed && !_isInitializing && !string.IsNullOrWhiteSpace(value))
            {
                var culture = new CultureInfo(value);
                culture.ApplyCulture();
                _settingsService.LocalSettings.Language = value;
                _settingsService.SaveLocalSettings();
            }
        }
    }

    /// <summary>
    /// Gets or sets the available languages.
    /// </summary>
    public ObservableCollection<string> AvailableLanguages { get; }

    /// <summary>
    /// Gets or sets the selected accent color.
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
    /// Gets the sample accent palette (same four colors as Sample.Maui).
    /// </summary>
    public ObservableCollection<string> AvailableAccentColors { get; } = new()
    {
        "#0B5D55",
        "#0066CC",
        "#10B981",
        "#FF8C00"
    };

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
        _supportedCulturesService = supportedCulturesService;

        LocalSettings = new LocalSettings();
        GlobalSettings = new GlobalSettings();

        AvailableThemes = new ObservableCollection<Themes>
        {
            Themes.Default,
            Themes.Light,
            Themes.Dark
        };

        AvailableLanguages = new ObservableCollection<string>(
            supportedCulturesService.SupportedCultures.Select(culture => culture.Name));
    }

    public override async Task InitializeAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            await base.InitializeAsync();

            if (!IsInitialized)
            {
                _settingsService.LoadLocalSettings();

                if (_settingsService.LocalSettings is LocalSettings localSetting)
                {
                    LocalSettings = localSetting;
                    SelectedTheme = localSetting.Theme;
                    SelectedAccentColor = string.IsNullOrWhiteSpace(localSetting.Color)
                        ? AvailableAccentColors[0]
                        : localSetting.Color;
                }

                await _settingsService.LoadGlobalSettingsAsync();

                if (_settingsService.GlobalSettings is GlobalSettings globalSetting)
                    GlobalSettings = globalSetting;

                var savedLanguage = string.IsNullOrWhiteSpace(_settingsService.LocalSettings.Language)
                    ? CultureInfo.CurrentCulture.Name
                    : _settingsService.LocalSettings.Language;

                SelectedLanguage = AvailableLanguages.Contains(savedLanguage)
                    ? savedLanguage
                    : AvailableLanguages.FirstOrDefault() ?? CultureInfo.CurrentCulture.Name;

                IsInitialized = true;
            }
        }
        finally
        {
            _isInitializing = false;
        }
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
