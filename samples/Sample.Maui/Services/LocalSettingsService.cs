using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Synchronization.Options;
using ISynergy.Framework.UI.Extensions;
using Sample.Abstractions;
using Sample.Models;

namespace Sample.Services;

internal class LocalSettingsService : ILocalSettingsService
{
    private readonly IPreferences _preferences;
    
    private LocalSettings _localSettings;
    private SynchronizationSettings _synchronizationSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingsService"/> class.
    /// </summary>
    public LocalSettingsService(IPreferences preferences)
    {
        _preferences = preferences;

        LoadSettings();
    }

    /// <summary>
    /// Loads the settings json file.
    /// </summary>
    public void LoadSettings()
    {
        _localSettings = _preferences.GetObject<LocalSettings>(nameof(LocalSettings), new LocalSettings());
        _synchronizationSettings = _preferences.GetObject<SynchronizationSettings>(nameof(SynchronizationSettings), new SynchronizationSettings());
    }

    /// <summary>
    /// Saves all changes to the json file.
    /// </summary>
    public void SaveSettings()
    {
        _preferences.SetObject(nameof(LocalSettings), _localSettings);
        _preferences.GetObject(nameof(SynchronizationSettings), _synchronizationSettings);
    }

    public LocalSettings Settings => _localSettings;
    public SynchronizationSettings SynchronizationSettings => _synchronizationSettings;

    IApplicationSettings IApplicationSettingsService.Settings => _localSettings;
}
