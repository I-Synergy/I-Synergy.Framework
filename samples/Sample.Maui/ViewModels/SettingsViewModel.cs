using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class SettingsViewModel : ViewModelNavigation<bool>
{
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ISettingsService _settingsService;

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

    public SettingsViewModel(
        ICommonServices commonServices,
        INavigationService navigationService,
        IDialogService dialogService,
        ISettingsService settingsService,
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;
        _settingsService = settingsService;

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

    public override async Task CancelAsync()
    {
        await base.CancelAsync();
        await _navigationService.GoBackAsync();
    }

    public override async Task SubmitAsync(bool e, bool validateUnderlayingProperties = true)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_settingsService.LocalSettings is LocalSettings)
            {
                if (_settingsService.SaveLocalSettings() &&
                    await _dialogService.ShowMessageAsync(
                        _commonServices.LanguageService.GetString("WarningSettingsChange") +
                        Environment.NewLine +
                        _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                        _commonServices.LanguageService.GetString("TitleQuestion"),
                        MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                {
                    _commonServices.RestartApplication();
                }
            }

            if (_settingsService.GlobalSettings is GlobalSettings globalSetting)
                await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSetting);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }

        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}
