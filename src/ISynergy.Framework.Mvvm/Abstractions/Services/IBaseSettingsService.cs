using ISynergy.Framework.Mvvm.Enumerations;
using System.Collections.Generic;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Base class for application settings.
    /// </summary>
    public interface IBaseSettingsService
    {
        /// <summary>
        /// Setting for maintaining the default culture used.
        /// </summary>
        string Culture { get; set; }
        /// <summary>
        /// Setting for maintaining if application is in fullscreen mode or not.
        /// </summary>
        bool IsFullscreen { get; set; }
        /// <summary>
        /// Setting for maintaining last succesfull username logged in.
        /// </summary>
        string DefaultUser { get; set; }
        /// <summary>
        /// Setting for maintaining all successfull usernames logged in.
        /// </summary>
        string Users { get; set; }
        /// <summary>
        /// Setting for storing the refresh token received on authentication.
        /// </summary>
        string RefreshToken { get; set; }
        /// <summary>
        /// Setting for storing the theme color.
        /// </summary>
        string Color { get; set; }
        /// <summary>
        /// If RefreshToken is available and setting is true, the application can login automatically. 
        /// </summary>
        bool IsAutoLogin { get; set; }
        /// <summary>
        /// Setting for maintaining the advanced mode of the application.
        /// </summary>
        bool IsAdvanced { get; set; }
        /// <summary>
        /// Wallpaper of the application.
        /// </summary>
        byte[] Wallpaper { get; set; }
    }
}