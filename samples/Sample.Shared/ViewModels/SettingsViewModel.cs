using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

[Scoped(true)]
public class SettingsViewModel : ViewModelNavigation<object>
{
    public override string Title { get => BaseCommonServices.LanguageService.GetString("Settings"); }

    private readonly ICommonServices _commonServices;
    private readonly IBaseApplicationSettingsService _localSettingsService;
    private readonly ISettingsService<GlobalSettings> _globalSettingsService;

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
        IBaseApplicationSettingsService localSettingsService,
        ISettingsService<GlobalSettings> globalSettingsService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _commonServices = commonServices;
        _localSettingsService = localSettingsService;
        _globalSettingsService = globalSettingsService;

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
        _commonServices.BusyService.StartBusy();

        if (!IsInitialized)
        {
            await base.InitializeAsync();

            _localSettingsService.LoadSettings();

            await _globalSettingsService.LoadSettingsAsync();

            if (_localSettingsService.Settings is LocalSettings localSetting)
                LocalSettings = localSetting;

            if (_globalSettingsService.Settings is { } globalSetting)
                GlobalSettings = globalSetting;

            IsInitialized = true;
        }

        _commonServices.BusyService.EndBusy();
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

            if (_localSettingsService.Settings is LocalSettings)
            {
                _localSettingsService.SaveSettings();
            }

            if (_globalSettingsService.Settings is { } globalSetting)
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
