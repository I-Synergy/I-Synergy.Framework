using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Models;
using System.Text.Json;

namespace Sample.Services;

public class LocalSettingsService : IApplicationSettingsService
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
        _settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "I-Synergy Framework UI Sample",
            "Settings");
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

    /// <summary>
    /// Settings used globally.
    /// </summary>
    public IApplicationSettings Settings => _settings;
}
