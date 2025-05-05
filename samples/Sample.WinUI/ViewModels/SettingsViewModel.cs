﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

public class SettingsViewModel : ViewModelNavigation<object>
{
    public override string Title { get => LanguageService.Default.GetString("Settings"); }

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
        ILogger<SettingsViewModel> logger)
        : base(commonServices, logger)
    {
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
                _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LoadLocalSettings();

                if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings is LocalSettings localSetting)
                    LocalSettings = localSetting;

                await _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LoadGlobalSettingsAsync();

                if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().GlobalSettings is GlobalSettings globalSetting)
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
        await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
    }

    public override async Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings is LocalSettings)
                _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();

            if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().GlobalSettings is GlobalSettings globalSetting)
                await _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().AddOrUpdateGlobalSettingsAsync(globalSetting);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }

        await base.SubmitAsync(e, validateUnderlayingProperties);
    }
}
