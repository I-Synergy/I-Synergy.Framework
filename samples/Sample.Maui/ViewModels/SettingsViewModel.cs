using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.ViewModels;

/// <summary>
/// Settings blade for language, theme and accent color. Changes are applied live and persisted;
/// the blade is closed through <see cref="SaveCommand"/> (save) or the inherited cancel command.
/// </summary>
[Lifetime(Lifetimes.Scoped)]
public partial class SettingsViewModel : ViewModelBlade<object>
{
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private readonly ISupportedCulturesService _supportedCulturesService;
    private CultureInfo? _selectedLanguage;

    /// <summary>
    /// Gets the title.
    /// </summary>
    public override string Title { get => _commonServices.LanguageService.GetString("Settings"); }

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
    /// Gets or sets the COMPorts property value.
    /// </summary>
    public ObservableCollection<string> COMPorts
    {
        get => GetValue<ObservableCollection<string>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the LanguageOptions property value.
    /// </summary>
    /// <remarks>
    /// The supported cultures come from <see cref="ISupportedCulturesService"/> and are shown by their
    /// display name ("English"), as in the WPF blade; a list of culture codes rendered "en" in the picker
    /// and could offer a culture the application was not configured for.
    /// </remarks>
    public ObservableCollection<CultureInfo> LanguageOptions
    {
        get => GetValue<ObservableCollection<CultureInfo>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the ThemeOptions property value.
    /// </summary>
    public ObservableCollection<Themes> ThemeOptions
    {
        get => GetValue<ObservableCollection<Themes>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the selected language.
    /// </summary>
    /// <remarks>
    /// The view binds to this instead of <see cref="LocalSettings"/> directly: a two-way binding into the
    /// settings model would write the picker's transient empty selection back into persisted settings
    /// while the blade page is being re-parented. The value is the <see cref="CultureInfo"/> the picker
    /// shows; the persisted setting stays the culture name.
    /// </remarks>
    public CultureInfo? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (value is null || string.Equals(LocalSettings?.Language, value.Name, StringComparison.OrdinalIgnoreCase))
                return;

            if (LocalSettings is not null)
                LocalSettings.Language = value.Name;

            _selectedLanguage = value;
            RaisePropertyChanged(nameof(SelectedLanguage));

            _settingsService.SaveLocalSettings();

            value.ApplyCulture();
        }
    }

    /// <summary>
    /// Gets or sets the selected theme.
    /// </summary>
    /// <remarks>
    /// Bound two-way by the view so the theme is applied and persisted from the view model, matching
    /// the WPF and WinUI settings blades.
    /// </remarks>
    public Themes SelectedTheme
    {
        get => LocalSettings?.Theme ?? Themes.Default;
        set
        {
            if (LocalSettings is not null && LocalSettings.Theme == value)
                return;

            if (LocalSettings is not null)
                LocalSettings.Theme = value;

            SetValue(value);

            _themeService.SetTheme(value);
            _settingsService.SaveLocalSettings();
        }
    }

    /// <summary>
    /// Gets the command that persists the settings and closes the blade.
    /// </summary>
    public AsyncRelayCommand SaveCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        ISettingsService settingsService,
        IThemeService themeService,
        ISupportedCulturesService supportedCulturesService,
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _settingsService = settingsService;
        _themeService = themeService;
        _supportedCulturesService = supportedCulturesService;

        LocalSettings = new LocalSettings();
        GlobalSettings = new GlobalSettings();

        LanguageOptions = new ObservableCollection<CultureInfo>(supportedCulturesService.SupportedCultures);

        ThemeOptions = new ObservableCollection<Themes>
        {
            Themes.Default,
            Themes.Light,
            Themes.Dark
        };

        COMPorts =
        [
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8"
        ];

        SaveCommand = new AsyncRelayCommand(SaveAsync);

        // The theme can also be switched from the shell header while the blade is open; without
        // this the picker kept showing the theme the blade was opened with.
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, EventArgs e) =>
        RaisePropertyChanged(nameof(SelectedTheme));

    /// <summary>
    /// Releases the theme subscription.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _themeService.ThemeChanged -= OnThemeChanged;

        base.Dispose(disposing);
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
                    LocalSettings = localSetting;

                // The picker holds CultureInfo instances; match the persisted name (English when the
                // setting is empty or no longer supported).
                _selectedLanguage = LanguageOptions.FirstOrDefault(culture =>
                        string.Equals(culture.Name, LocalSettings?.Language, StringComparison.OrdinalIgnoreCase))
                    ?? _supportedCulturesService.DefaultCulture;

                RaisePropertyChanged(nameof(SelectedLanguage));

                await _settingsService.LoadGlobalSettingsAsync();

                if (_settingsService.GlobalSettings is GlobalSettings globalSetting)
                    GlobalSettings = globalSetting;

                SelectedItem = true;
                IsInitialized = true;
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    /// <summary>
    /// Persists the local and global settings and closes the blade.
    /// </summary>
    private async Task SaveAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_settingsService.LocalSettings is LocalSettings &&
                _settingsService.SaveLocalSettings() &&
                await _dialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningSettingsChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButtons.YesNo) == MessageBoxResults.Yes)
            {
                _commonServices.RestartApplication();
            }

            if (_settingsService.GlobalSettings is GlobalSettings globalSetting)
                await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSetting);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }

        await CloseAsync();
    }
}
