using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Constants;
using System.Globalization;

namespace Sample.Models;

public class LocalSettings : ObservableValidatedClass, ILocalSettings
{
    public string Language { get; set; } = GlobalizationConstants.DefaultCultureInfoName;
    public bool IsFullscreen { get; set; }
    public List<string> Users { get; set; } = [];
    public string DefaultUser { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Themes Theme { get; set; } = Themes.Dark;
    public bool IsLightThemeEnabled => Theme == Themes.Light;
    public bool IsAutoLogin { get; set; }
    public bool IsAdvanced { get; set; }
    public int MigrationVersion { get; set; }
    public bool ShowConsole { get; set; }

    public LocalSettings()
    {
        Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        MigrationVersion = 0;
    }
}
