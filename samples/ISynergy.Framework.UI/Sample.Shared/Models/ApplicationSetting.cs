using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sample.Models
{
    public class ApplicationSetting : IBaseApplicationSettings
    {
        public string Culture { get; set; } = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        public bool IsFullscreen { get; set; }
        public string DefaultUser { get; set; } = string.Empty;
        public string DefaultPassword { get; set; } = string.Empty;
        public List<string> Users { get; set; } = new List<string>();
        public string RefreshToken { get; set; } = string.Empty;
        public string Color { get; set; } = ThemeColors.Default;
        public Themes Theme { get; set; } = Themes.Dark;
        public bool IsAutoLogin { get; set; }
        public bool IsAdvanced { get; set; }
        public byte[] Wallpaper { get; set; } = Array.Empty<byte>();
    }
}
