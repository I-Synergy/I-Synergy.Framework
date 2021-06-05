using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Storage;

namespace ISynergy.Framework.UI.Services.Base
{
    /// <summary>
    /// Abstract class BaseSettingsService.
    /// </summary>
    public abstract class BaseSettingsService : IBaseSettingsService
    {
        /// <summary>
        /// The local settings
        /// </summary>
        protected readonly ApplicationDataContainer localSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSettingsService"/> class.
        /// </summary>
        protected BaseSettingsService()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color
        {
            get
            {
                var setting = localSettings.Values[nameof(Color)];

                if (setting is null)
                {
                    setting = ThemeColors.Default.ToString();
                    localSettings.Values[nameof(Color)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Color)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public string Culture
        {
            get
            {
                var setting = localSettings.Values[nameof(Culture)];

                if (setting is null)
                {
                    setting = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    localSettings.Values[nameof(Culture)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Culture)] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fullscreen.
        /// </summary>
        /// <value><c>true</c> if this instance is fullscreen; otherwise, <c>false</c>.</value>
        public bool IsFullscreen
        {
            get
            {
                var setting = localSettings.Values[nameof(IsFullscreen)];

                if (setting is null)
                {
                    setting = true;
                    localSettings.Values[nameof(IsFullscreen)] = setting;
                }

                return (bool)setting;
            }

            set
            {
                localSettings.Values[nameof(IsFullscreen)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the default user.
        /// </summary>
        /// <value>The default user.</value>
        public string DefaultUser
        {
            get
            {
                var setting = localSettings.Values[nameof(DefaultUser)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(DefaultUser)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(DefaultUser)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public string Users
        {
            get
            {
                var setting = localSettings.Values[nameof(Users)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(Users)] = setting;
                }

                return setting.ToString();
            }

            set
            {
                localSettings.Values[nameof(Users)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken
        {
            get
            {
                var setting = localSettings.Values[nameof(RefreshToken)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(RefreshToken)] = setting;
                }

                return (string)setting;
            }
            set
            {
                localSettings.Values[nameof(RefreshToken)] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic login.
        /// </summary>
        /// <value><c>true</c> if this instance is automatic login; otherwise, <c>false</c>.</value>
        public bool IsAutoLogin
        {
            get
            {
                var setting = localSettings.Values[nameof(IsAutoLogin)];

                if (setting is null)
                {
                    setting = false;
                    localSettings.Values[nameof(IsAutoLogin)] = setting;
                }

                return (bool)setting;
            }
            set
            {
                localSettings.Values[nameof(IsAutoLogin)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the wallpaper.
        /// </summary>
        /// <value>The wallpaper.</value>
        public byte[] Wallpaper
        {
            get
            {
                var setting = localSettings.Values[nameof(Wallpaper)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(Culture)] = setting;
                }

                return Convert.FromBase64String(setting.ToString());
            }

            set
            {
                localSettings.Values[nameof(Wallpaper)] = Convert.ToBase64String(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update.
        /// </summary>
        /// <value><c>true</c> if this instance is update; otherwise, <c>false</c>.</value>
        public bool IsUpdate
        {
            get
            {
                var setting = localSettings.Values[nameof(IsUpdate)];

                if (setting is null)
                {
                    setting = false;
                    localSettings.Values[nameof(IsUpdate)] = setting;
                }

                return (bool)setting;
            }
            set
            {
                localSettings.Values[nameof(IsUpdate)] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is advanced.
        /// </summary>
        /// <value><c>true</c> if this instance is advanced; otherwise, <c>false</c>.</value>
        public bool IsAdvanced
        {
            get
            {
                var setting = localSettings.Values[nameof(IsAdvanced)];

                if (setting is null)
                {
                    setting = true;
                    localSettings.Values[nameof(IsAdvanced)] = setting;
                }

                return (bool)setting;
            }
            set
            {
                localSettings.Values[nameof(IsAdvanced)] = value;
            }
        }
    }
}
