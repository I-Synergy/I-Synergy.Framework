using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Synchronization.Abstractions;
using System.Globalization;

namespace Sample.Models;
public class LocalSettings : ISynchronizationApplicationSettings
{
    public Languages Language { get; set; } = Languages.English;
    public bool IsFullscreen { get; set; }
    public string DefaultUser { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Color { get; set; } = ThemeColors.Default;
    public Themes Theme { get; set; } = Themes.Dark;
    public bool IsAutoLogin { get; set; }
    public bool IsAdvanced { get; set; }
    public byte[] Wallpaper { get; set; } = Array.Empty<byte>();
    public bool IsSynchronizationEnabled { get; set; }
    public int SynchronizationInterval { get; set; }
    public int BatchSize { get; set; }
    public string SynchronizationFolder { get; set; }
    public string SnapshotFolder { get; set; }
    public string BatchesFolder { get; set; }
    public bool CleanSynchronizationFolder { get; set; }
    public bool CleanSynchronizationMetadatas { get; set; }

    public LocalSettings()
    {
        Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "nl" => Languages.Dutch,
            "de" => Languages.German,
            "fr" => Languages.French,
            _ => Languages.English,
        };

        IsSynchronizationEnabled = false;
        SynchronizationInterval = 30;
        BatchSize = 1000;
        SynchronizationFolder = Path.Combine(FileSystem.AppDataDirectory, "Synchronization");
        SnapshotFolder = Path.Combine(FileSystem.AppDataDirectory, "Snapshots");
        BatchesFolder = Path.Combine(FileSystem.AppDataDirectory, "Batches");
        CleanSynchronizationFolder = true;
        CleanSynchronizationMetadatas = true;
    }
}
