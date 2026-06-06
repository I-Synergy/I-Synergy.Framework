using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace Sample.Models;

public class LocalSettings : ObservableValidatedClass, ILocalSettings
{
    public string Language
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public bool IsFullscreen
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public List<string> Users
    {
        get => GetValue<List<string>>();
        set => SetValue(value);
    }

    public string DefaultUser
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string? RefreshToken
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string Color
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public Themes Theme
    {
        get => GetValue<Themes>();
        set => SetValue(value);
    }

    public bool IsLightThemeEnabled => Theme == Themes.Light;

    public bool IsAutoLogin
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public bool IsAdvanced
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public int MigrationVersion
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    public bool ShowConsole
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public LocalSettings()
    {
        Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Users = new List<string>();
        Color = ThemeColors.Default;
        Theme = Themes.Dark;
        DefaultUser = string.Empty;
        RefreshToken = string.Empty;
        MigrationVersion = 0;
    }
}
