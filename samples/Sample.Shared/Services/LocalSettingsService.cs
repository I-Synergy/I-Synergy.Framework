using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using Sample.Abstractions.Services;
using Sample.Models;
using System.Text.Json;

namespace Sample.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _fileName = "settings.json";
    private readonly string _settingsFolder;

    private LocalSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingsService"/> class.
    /// </summary>
    public LocalSettingsService()
    {
        _settings = new LocalSettings();
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

        LoadSettings();
    }

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

    public void SaveSettings()
    {
        if (!Directory.Exists(_settingsFolder))
            Directory.CreateDirectory(_settingsFolder);

        string file = Path.Combine(_settingsFolder, _fileName);
        string json = JsonSerializer.Serialize(_settings);
        File.WriteAllText(file, json);
    }

    public LocalSettings Settings => _settings;

    IApplicationSettings IApplicationSettingsService.Settings => _settings;
}
