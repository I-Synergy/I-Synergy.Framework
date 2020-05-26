namespace ISynergy.Framework.Wopi.Models
{
    public class PostMessageProperties : IPostMessageProperties
    {
        public bool ClosePostMessage { get; set; }
        public bool EditModePostMessage { get; set; }
        public bool EditNotificationPostMessage { get; set; }
        public bool FileSharingPostMessage { get; set; }
        public string PostMessageOrigin { get; set; }
    }
}
