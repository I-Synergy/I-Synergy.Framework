using System;
using System.Globalization;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Windows.Abstractions.Services;
using Windows.Storage;

namespace ISynergy.Framework.Windows.Services
{
    public sealed class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly ApplicationDataContainer localSettings;

        public ApplicationSettingsService()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        public string Color
        {
            get
            {
                var result = ApplicationColors.Default;
                var setting = localSettings.Values[nameof(Color)];

                if (setting is null)
                {
                    localSettings.Values[nameof(Color)] = result.ToString();
                    setting = localSettings.Values[nameof(Color)];
                }

                if (Enum.TryParse(setting.ToString(), out ApplicationColors color))
                {
                    result = color;
                }
                else
                {
                    localSettings.Values[nameof(Color)] = result.ToString();
                    ServiceLocator.Default.GetInstance<IThemeSelectorService>().SetRequestedTheme();
                }

                return result.ToString();
            }

            set
            {
                localSettings.Values[nameof(Color)] = value.ToString();
                ServiceLocator.Default.GetInstance<IThemeSelectorService>().SetRequestedTheme();
            }
        }

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

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Users)] = value;
            }
        }

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
