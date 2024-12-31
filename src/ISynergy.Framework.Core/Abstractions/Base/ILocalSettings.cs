﻿using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Base class for application settings.
/// </summary>
public interface ILocalSettings
{
    /// <summary>
    /// Setting for maintaining the default culture used.
    /// </summary>
    Languages Language { get; set; }
    /// <summary>
    /// Setting for maintaining if application is in fullscreen mode or not.
    /// </summary>
    bool IsFullscreen { get; set; }
    /// <summary>
    /// Setting for maintaining a list of users used.
    /// </summary>
    List<string> Users { get; set; }
    /// <summary>
    /// Setting for maintaining last succesfull username logged in.
    /// </summary>
    string DefaultUser { get; set; }
    /// <summary>
    /// Setting for storing the refresh token received on authentication.
    /// </summary>
    string RefreshToken { get; set; }
    /// <summary>
    /// Setting for storing the theme color.
    /// </summary>
    string Color { get; set; }
    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    Themes Theme { get; set; }
    /// <summary>
    /// Gets a value indicating whether this instance is light theme enabled.
    /// </summary>
    /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
    bool IsLightThemeEnabled { get; }
    /// <summary>
    /// If RefreshToken is available or user is available in Windows Credentials locker and setting is true, the application can login automatically. 
    /// </summary>
    bool IsAutoLogin { get; set; }
    /// <summary>
    /// Setting for maintaining the advanced mode of the application.
    /// </summary>
    bool IsAdvanced { get; set; }
    /// <summary>
    /// Setting for maintaining the last used migration version of the application.
    /// </summary>
    int MigrationVersion { get; set; }
}