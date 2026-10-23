using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.ViewModels;

public class SettingsViewModel : ViewModelBlade<object>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;

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
    /// Gets or sets the selected theme.
    /// </summary>
    public Themes SelectedTheme
    {
        get => GetValue<Themes>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the available themes.
    /// </summary>
    public ObservableCollection<Themes> AvailableThemes
    {
        get => GetValue<ObservableCollection<Themes>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the selected language.
    /// </summary>
    public string SelectedLanguage
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the available languages.
    /// </summary>
    public ObservableCollection<string> AvailableLanguages
    {
        get => GetValue<ObservableCollection<string>>();
        set => SetValue(value);
    }

    public SettingsViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<SettingsViewModel> logger,
        IThemeService themeService)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;
        _themeService = themeService;

        LocalSettings = new LocalSettings();
        GlobalSettings = new GlobalSettings();

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

        // Initialize theme options
        AvailableThemes = new ObservableCollection<Themes>
        {
            Themes.Light,
            Themes.Dark
        };
        SelectedTheme = _themeService.Theme;

        // Initialize language options
        AvailableLanguages = new ObservableCollection<string>
        {
            "en-US",
            "nl-NL"
        };
        SelectedLanguage = CultureInfo.CurrentCulture.Name;
    }

    public override async Task InitializeAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            await base.InitializeAsync();

            if (!IsInitialized)
            {
                Application.Services.GetRequiredService<ISettingsService>().LoadLocalSettings();

                if (Application.Services.GetRequiredService<ISettingsService>().LocalSettings is LocalSettings localSetting)
                {
                    LocalSettings = localSetting;
                    // Reflect current theme and language from settings
                    SelectedTheme = localSetting.Theme;
                }

                await Application.Services.GetRequiredService<ISettingsService>().LoadGlobalSettingsAsync();

                if (Application.Services.GetRequiredService<ISettingsService>().GlobalSettings is GlobalSettings globalSetting)
                    GlobalSettings = globalSetting;

                IsInitialized = true;
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    public override async Task CancelAsync()
    {
        await base.CancelAsync();
        await _navigationService.NavigateModalAsync<IShellViewModel>();
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (Application.Services.GetRequiredService<ISettingsService>().LocalSettings is LocalSettings localSettings)
            {
                // Apply theme live via IThemeService
                if (localSettings.Theme != SelectedTheme)
                {
                    _themeService.SetTheme(SelectedTheme);
                    localSettings.Theme = SelectedTheme;
                }

                // Apply language live via ApplyCulture
                if (!SelectedLanguage.Equals(CultureInfo.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var culture = new CultureInfo(SelectedLanguage);
                    culture.ApplyCulture(); // This calls LocalizedStrings.Default.Refresh() automatically
                    localSettings.Language = SelectedLanguage;
                }

                Application.Services.GetRequiredService<ISettingsService>().SaveLocalSettings();
            }

            if (Application.Services.GetRequiredService<ISettingsService>().GlobalSettings is GlobalSettings globalSetting)
                await Application.Services.GetRequiredService<ISettingsService>().AddOrUpdateGlobalSettingsAsync(globalSetting);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }

        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}

