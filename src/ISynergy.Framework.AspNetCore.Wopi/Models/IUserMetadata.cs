namespace ISynergy.Framework.Wopi.Models
{
    public interface IUserMetadata
    {
        bool IsEduUser { get; set; }
        bool LicenseCheckForEditIsEnabled { get; set; }
        string UserFriendlyName { get; set; }
        string UserInfo { get; set; }
    }
}
