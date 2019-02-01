using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract class BaseSettingsService : IBaseSettingsService
    {
        Windows.Storage.ApplicationDataContainer localSettings;

        public BaseSettingsService()
        {
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        public string Application_Color
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_Color)];

                if (setting is null)
                {
                    setting = "#FF3399FF";
                    localSettings.Values[nameof(Application_Color)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Application_Color)] = value;
            }
        }

        public string Application_Culture
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_Culture)];

                if (setting is null)
                {
                    setting = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    localSettings.Values[nameof(Application_Culture)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Application_Culture)] = value;
            }
        }

        public bool Application_Fullscreen
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_Fullscreen)];

                if (setting is null)
                {
                    setting = true;
                    localSettings.Values[nameof(Application_Fullscreen)] = setting;
                }

                return (bool)setting;
            }

            set
            {
                localSettings.Values[nameof(Application_Fullscreen)] = value;
            }
        }

        public string Application_User
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_User)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(Application_User)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Application_User)] = value;
            }
        }

        public string Application_Users
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_Users)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(Application_Users)] = setting;
                }

                return (string)setting;
            }

            set
            {
                localSettings.Values[nameof(Application_Users)] = value;
            }
        }

        public string User_RefreshToken
        {
            get
            {
                var setting = localSettings.Values[nameof(User_RefreshToken)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(User_RefreshToken)] = setting;
                }

                return (string)setting;
            }
            set
            {
                localSettings.Values[nameof(User_RefreshToken)] = value;
            }
        }

        public bool User_AutoLogin
        {
            get
            {
                var setting = localSettings.Values[nameof(User_AutoLogin)];

                if (setting is null)
                {
                    setting = false;
                    localSettings.Values[nameof(User_AutoLogin)] = setting;
                }

                return (bool)setting;
            }
            set
            {
                localSettings.Values[nameof(User_AutoLogin)] = value;
            }
        }

        public byte[] Application_Wallpaper
        {
            get
            {
                var setting = localSettings.Values[nameof(Application_Wallpaper)];

                if (setting is null)
                {
                    setting = string.Empty;
                    localSettings.Values[nameof(Application_Culture)] = setting;
                }

                return Convert.FromBase64String(setting.ToString());
            }

            set
            {
                localSettings.Values[nameof(Application_Wallpaper)] = Convert.ToBase64String(value);
            }
        }

        public abstract Task LoadSettingsAsync();
        public abstract T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
        public abstract void CheckForUpgrade();
        public abstract int DefaultCurrencyId { get; }
        public abstract string ApplicationInsights_InstrumentationKey { get; }
        public abstract string AppCenter_InstrumentationKey { get; }
        public abstract bool Application_Update { get; set; }
        public abstract bool Application_Advanced { get; set; }
        public abstract bool Application_IsFirstRun { get; }
    }
}
