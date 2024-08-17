using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Extensions;
using Sample.Abstractions.Services;
using Sample.Models;
using System.Text.Json;

namespace Sample.Services;

public class SettingsService : ISettingsService
{
    private const string _fileName = "settings.json";
    private readonly string _settingsFolder;

    private LocalSettings _localSettings;
    private RoamingSettings _roamingSettings;
    private GlobalSettings _globalSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService()
    {
        _localSettings = new LocalSettings();
        _roamingSettings = new RoamingSettings();
        _globalSettings = new GlobalSettings();

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
            _localSettings = JsonSerializer.Deserialize<LocalSettings>(json);
        }
        catch (JsonException)
        {
            SaveLocalSettings();
        }
        catch (FileNotFoundException)
        {
            _localSettings = new LocalSettings();
        }
    }

    public void SaveLocalSettings()
    {
        if (!Directory.Exists(_settingsFolder))
            Directory.CreateDirectory(_settingsFolder);

        string file = Path.Combine(_settingsFolder, _fileName);
        string json = JsonSerializer.Serialize(_localSettings);
        File.WriteAllText(file, json);
    }

    public Task LoadRoamingSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task SaveRoamingSettingsAsync()
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

    public LocalSettings LocalSettings => _localSettings;
    public RoamingSettings RoamingSettings => _roamingSettings;
    public GlobalSettings GlobalSettings => _globalSettings;

    ILocalSettings IBaseSettingsService.LocalSettings => _localSettings;
    IRoamingSettings IBaseSettingsService.RoamingSettings => _roamingSettings;
    IGlobalSettings IBaseSettingsService.GlobalSettings => _globalSettings;
}
