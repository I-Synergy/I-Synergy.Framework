namespace ISynergy.Framework.Wopi.Models
{
    public interface IUserPermissions
    {
        bool ReadOnly { get; set; }
        bool RestrictedWebViewOnly { get; set; }
        bool UserCanAttend { get; set; }
        bool UserCanNotWriteRelative { get; set; }
        bool UserCanPresent { get; set; }
        bool UserCanRename { get; set; }
        bool UserCanWrite { get; set; }
        bool WebEditingDisabled { get; set; }
        bool DisablePrint { get; set; }
        bool DisableTranslation { get; set; }
    }
}
