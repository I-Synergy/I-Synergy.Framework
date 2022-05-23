namespace ISynergy.Framework.Wopi.Models
{
    public interface IWopiHostProperties
    {
        bool AllowExternalMarketplace { get; set; }
        bool CloseButtonClosesWindow { get; set; }
        int FileNameMaxLength { get; set; }
    }
}
