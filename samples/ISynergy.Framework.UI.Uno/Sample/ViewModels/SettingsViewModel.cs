using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Sample.Abstractions.Services;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

public class SettingsViewModel : ViewModelNavigation<object>
{
    public override string Title { get => BaseCommonServices.LanguageService.GetString("Settings"); }

    private readonly ICommonServices _commonServices;
    private readonly IBaseApplicationSettingsService _localSettingsService;
    private readonly IGlobalSettingsService _globalSettingsService;

    /// <summary>
    /// Gets or sets the LocalSetting property value.
    /// </summary>
    public LocalSettings LocalSetting
    {
        get => GetValue<LocalSettings>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the GlobalSetting property value.
    /// </summary>
    public GlobalSettings GlobalSetting
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
        IBaseApplicationSettingsService localSettingsService,
        IGlobalSettingsService globalSettingsService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _commonServices = commonServices;
        _localSettingsService = localSettingsService;
        _globalSettingsService = globalSettingsService;

        LocalSetting = new LocalSettings();
        GlobalSetting = new GlobalSettings();

        COMPorts = new ObservableCollection<string>()
        {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8"
        };
    }

    public override async Task InitializeAsync()
    {
        _commonServices.BusyService.StartBusy();

        if (!IsInitialized)
        {
            await base.InitializeAsync();

            _localSettingsService.LoadSettings();

            await _globalSettingsService.LoadSettingsAsync();

            if (_localSettingsService.Settings is LocalSettings localSetting)
                LocalSetting = localSetting;

            if (_globalSettingsService.Settings is GlobalSettings globalSetting)
                GlobalSetting = globalSetting;

            IsInitialized = true;
        }

        _commonServices.BusyService.EndBusy();
    }

    public override async Task CancelAsync()
    {
        await base.CancelAsync();
        await _commonServices.NavigationService.NavigateModalAsync<ShellViewModel>();
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_localSettingsService.Settings is LocalSettings localSetting)
            {
                _localSettingsService.SaveSettings();
            }

            if (_globalSettingsService.Settings is GlobalSettings globalSetting)
            {

                await _globalSettingsService.AddOrUpdateSettingsAsync(globalSetting);
            }
        }
        finally
        {
            _commonServices.BusyService.EndBusy();
        }

        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}
