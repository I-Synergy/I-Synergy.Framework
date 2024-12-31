using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Sample.Services;

public class SettingsService<TLocalSettings, TRoamingSettings, TGlobalSettings> : ISettingsService
    where TLocalSettings : class, ILocalSettings, new()
    where TRoamingSettings : class, IRoamingSettings, new()
    where TGlobalSettings : class, IGlobalSettings, new()
{
    private const string _fileName = "settings.json";
    private readonly string _settingsFolder;
    private readonly ILogger _logger;

    private TLocalSettings _localSettings;
    private TRoamingSettings _roamingSettings;
    private TGlobalSettings _globalSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService{TLocalSettings, TRoamingSettings, TGlobalSettings}"/> class.
    /// </summary>
    public SettingsService(ILogger<SettingsService<TLocalSettings, TRoamingSettings, TGlobalSettings>> logger)
    {
        _logger = logger;
        _logger.LogDebug($"SettingsService instance created with ID: {Guid.NewGuid()}");

        _localSettings = new TLocalSettings();
        _roamingSettings = new TRoamingSettings();
        _globalSettings = new TGlobalSettings();

        _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Settings");

        if (!Directory.Exists(_settingsFolder))
            Directory.CreateDirectory(_settingsFolder);

        var oldPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "I-Synergy", "I-Synergy Framework Sample");
        var oldSettings = Path.Combine(oldPath, "Settings", _fileName);

        if (File.Exists(oldSettings))
        {
            File.Move(oldSettings, Path.Combine(_settingsFolder, _fileName), true);
            Directory.Delete(oldPath, true);
        }

        LoadLocalSettings();
    }

    public void LoadLocalSettings()
    {
        try
        {
            string file = Path.Combine(_settingsFolder, _fileName);

            if (!File.Exists(file))
                SaveLocalSettings();

            string json = File.ReadAllText(file);
            _localSettings = JsonSerializer.Deserialize<TLocalSettings>(json);
        }
        catch (JsonException)
        {
            SaveLocalSettings();
        }
        catch (FileNotFoundException)
        {
            _localSettings = new TLocalSettings();
        }
    }

    public bool SaveLocalSettings()
    {
        try
        {
            if (!Directory.Exists(_settingsFolder))
                Directory.CreateDirectory(_settingsFolder);

            string file = Path.Combine(_settingsFolder, _fileName);
            string json = JsonSerializer.Serialize(_localSettings);
            File.WriteAllText(file, json);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task LoadRoamingSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveRoamingSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> AddOrUpdateGlobalSettingsAsync(IGlobalSettings e, CancellationToken cancellationToken = default)
    {
        // Update settings
        return Task.FromResult(1);
    }

    public Task LoadGlobalSettingsAsync(CancellationToken cancellationToken = default)
    {
        // Load settings from database
        return Task.CompletedTask;
    }

    public T GetGlobalSetting<T>(string name, T defaultvalue) where T : IComparable<T> =>
        _globalSettings.GetPropertyValue(name, defaultvalue);


    public void ClearSettings()
    {
        _globalSettings = null;
        _roamingSettings = null;
    }

    public TLocalSettings LocalSettings => _localSettings;
    public TRoamingSettings RoamingSettings => _roamingSettings;
    public TGlobalSettings GlobalSettings => _globalSettings;

    ILocalSettings ISettingsService.LocalSettings => _localSettings;
    IRoamingSettings ISettingsService.RoamingSettings => _roamingSettings;
    IGlobalSettings ISettingsService.GlobalSettings => _globalSettings;
}
