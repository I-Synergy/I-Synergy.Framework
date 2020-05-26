namespace ISynergy.Framework.Wopi.Models
{
    public class WopiHostProperties : IWopiHostProperties
    {
        public bool AllowExternalMarketplace { get; set; }
        public bool CloseButtonClosesWindow { get; set; }
        public int FileNameMaxLength { get; set; }
    }
}
