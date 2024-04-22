using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Synchronization.Abstractions;
using Sample.Abstractions;
using Sample.Models;
using System.Text.Json;

namespace Sample.Services;

internal class LocalSettingsService : ILocalSettingsService
{
    private const string _fileName = "settings.json";

    /// <summary>
    /// The global settings folder.
    /// </summary>
    private readonly string _settingsFolder;

    /// <summary>
    /// Settings used globally.
    /// </summary>
    private LocalSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingsService"/> class.
    /// </summary>
    public LocalSettingsService()
    {
        _settings = new LocalSettings();
        _settingsFolder = Path.Combine(FileSystem.AppDataDirectory, "Settings");

        if (!Directory.Exists(_settingsFolder))
            Directory.CreateDirectory(_settingsFolder);

        var oldPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "I-Synergy Framework UI Sample");
        var oldSettings = Path.Combine(oldPath, "Settings", _fileName);

        if (File.Exists(oldSettings))
        {
            File.Move(oldSettings, Path.Combine(_settingsFolder, _fileName), true);
            Directory.Delete(oldPath, true);
        }
    }

    /// <summary>
    /// Loads the settings json file.
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            string file = Path.Combine(_settingsFolder, _fileName);

            if (!File.Exists(file))
                SaveSettings();

            string json = File.ReadAllText(file);
            _settings = JsonSerializer.Deserialize<LocalSettings>(json);
        }
        catch (JsonException)
        {
            SaveSettings();
        }
        catch (FileNotFoundException)
        {
            _settings = new LocalSettings();
        }
    }

    /// <summary>
    /// Saves all changes to the json file.
    /// </summary>
    public void SaveSettings()
    {
        if (!Directory.Exists(_settingsFolder))
            Directory.CreateDirectory(_settingsFolder);

        string file = Path.Combine(_settingsFolder, _fileName);
        string json = JsonSerializer.Serialize(_settings);
        File.WriteAllText(file, json);
    }

    IBaseApplicationSettings IBaseApplicationSettingsService.Settings => _settings;
    ISynchronizationApplicationSettings ISynchronizationSettingsService.Settings => _settings;

    public LocalSettings Settings => _settings;
}
