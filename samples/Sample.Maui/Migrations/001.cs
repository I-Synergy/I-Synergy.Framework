using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.UI.Extensions;
using Sample.Models;
using System.Text.Json;

namespace Sample.Migrations;

public class _001 : IMigration
{
    public int MigrationVersion => 1;

    public void Up()
    {
        var localSettings = Preferences.Default.GetObject<LocalSettings>(nameof(LocalSettings), new LocalSettings());

        var settingsFolder = Path.Combine(FileSystem.AppDataDirectory, "Settings");
        var fileName = "settings.json";
        var file = Path.Combine(settingsFolder, fileName);

        if (Directory.Exists(settingsFolder) && File.Exists(file))
        {
            string json = File.ReadAllText(file);
            var oldSettings = JsonSerializer.Deserialize<LocalSettings>(json);

            localSettings.Color = oldSettings.Color;
            localSettings.DefaultUser = oldSettings.DefaultUser;
            localSettings.IsAdvanced = oldSettings.IsAdvanced;
            localSettings.IsAutoLogin = oldSettings.IsAutoLogin;
            localSettings.IsFullscreen = oldSettings.IsFullscreen;
            localSettings.Language = oldSettings.Language;
            localSettings.RefreshToken = oldSettings.RefreshToken;
            localSettings.Theme = oldSettings.Theme;
            localSettings.Wallpaper = oldSettings.Wallpaper;

            if (File.Exists(file))
                Directory.Delete(file, true);
        }

        Preferences.Default.SetObject(nameof(LocalSettings), localSettings);
    }

    public void Down()
    {
        var localSettings = Preferences.Default.GetObject<LocalSettings>(nameof(LocalSettings), new LocalSettings());

        var settingsFolder = Path.Combine(FileSystem.AppDataDirectory, "Settings");
        var fileName = "settings.json";
        var file = Path.Combine(settingsFolder, fileName);

        if (!Directory.Exists(settingsFolder))
            Directory.CreateDirectory(settingsFolder);

        var oldSettings = new LocalSettings
        {
            Color = localSettings.Color,
            DefaultUser = localSettings.DefaultUser,
            IsAdvanced = localSettings.IsAdvanced,
            IsAutoLogin = localSettings.IsAutoLogin,
            IsFullscreen = localSettings.IsFullscreen,
            Language = localSettings.Language,
            RefreshToken = localSettings.RefreshToken,
            Theme = localSettings.Theme,
            Wallpaper = localSettings.Wallpaper
        };

        string json = JsonSerializer.Serialize(oldSettings);
        File.WriteAllText(file, json);

        Preferences.Default.Remove(nameof(LocalSettings));
    }
}
