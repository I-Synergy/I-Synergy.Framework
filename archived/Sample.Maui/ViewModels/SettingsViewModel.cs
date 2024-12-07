using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class SettingsViewModel : ViewModelNavigation<object>
{
    public override string Title { get => BaseCommonServices.LanguageService.GetString("Settings"); }

    private readonly ICommonServices _commonServices;
    private readonly ISettingsService _settingsService;

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
        IContext context,
        ICommonServices commonServices,
        ISettingsService settingsService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _commonServices = commonServices;
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

                IsInitialized = true;
            }
        }
        finally
        {
            _commonServices.BusyService.EndBusy();
        }
    }

    public override async Task CancelAsync()
    {
        await base.CancelAsync();
        await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_settingsService.LocalSettings is LocalSettings)
                _settingsService.SaveLocalSettings();

            if (_settingsService.GlobalSettings is GlobalSettings globalSetting)
                await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSetting);
        }
        finally
        {
            _commonServices.BusyService.EndBusy();
        }

        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}
