using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IBaseSettingsService
    {
        #region "User Settings"
        string ApplicationInsights_InstrumentationKey { get; }
        string AppCenter_InstrumentationKey { get; }
        string Application_User { get; set; }
        string Application_Users { get; set; }
        string User_RefreshToken { get; set; }
        bool User_AutoLogin { get; set; }
        byte[] Application_Wallpaper { get; set; }
        string Application_Color { get; set; }
        bool Application_Fullscreen { get; set; }
        string Application_Culture { get; set; }
        bool Application_Update { get; set; }
        bool Application_Advanced { get; set; }
        void CheckForUpgrade();
        #endregion

        #region "Application Settings"
        bool Application_IsFirstRun { get; }
        int DefaultCurrencyId { get; }

        Task LoadSettingsAsync();
        T GetSetting<T>(string name, T defaultvalue) where T : IComparable<T>;
        #endregion
    }
}
