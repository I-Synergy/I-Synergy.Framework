﻿using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Enumerations;
using System.Globalization;

namespace Sample.Models;
public class LocalSettings : IApplicationSettings
{
    public Languages Language { get; set; } = Languages.English;
    public bool IsFullscreen { get; set; }
    public string DefaultUser { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Color { get; set; } = ThemeColors.Default;
    public Themes Theme { get; set; } = Themes.Dark;
    public bool IsAutoLogin { get; set; }
    public bool IsAdvanced { get; set; }
    public int MigrationVersion { get; set; }
    public bool IsSynchronizationEnabled { get; set; }

    public LocalSettings()
    {
        Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "nl" => Languages.Dutch,
            "de" => Languages.German,
            "fr" => Languages.French,
            _ => Languages.English,
        };
    }
}
