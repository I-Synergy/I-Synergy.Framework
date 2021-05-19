namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IApplicationSettingsService
    /// https://stackoverflow.com/questions/41653688/asp-net-core-appsettings-json-update-in-code
    /// </summary>
    public interface IApplicationSettingsService
    {
        /// <summary>
        /// Gets or sets the default user.
        /// </summary>
        /// <value>The default user.</value>
        string DefaultUser { get; set; }
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        string Users { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        string RefreshToken { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic login.
        /// </summary>
        /// <value><c>true</c> if this instance is automatic login; otherwise, <c>false</c>.</value>
        bool IsAutoLogin { get; set; }
        /// <summary>
        /// Gets or sets the wallpaper.
        /// </summary>
        /// <value>The wallpaper.</value>
        byte[] Wallpaper { get; set; }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        string Color { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is fullscreen.
        /// </summary>
        /// <value><c>true</c> if this instance is fullscreen; otherwise, <c>false</c>.</value>
        bool IsFullscreen { get; set; }
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        string Culture { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is update.
        /// </summary>
        /// <value><c>true</c> if this instance is update; otherwise, <c>false</c>.</value>
        bool IsUpdate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is advanced.
        /// </summary>
        /// <value><c>true</c> if this instance is advanced; otherwise, <c>false</c>.</value>
        bool IsAdvanced { get; set; }
    }
}
