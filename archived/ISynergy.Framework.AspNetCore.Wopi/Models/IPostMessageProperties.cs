namespace ISynergy.Framework.Wopi.Models
{
    internal interface IPostMessageProperties
    {
        bool ClosePostMessage { get; set; }
        bool EditModePostMessage { get; set; }
        bool EditNotificationPostMessage { get; set; }
        bool FileSharingPostMessage { get; set; }
        string PostMessageOrigin { get; set; }

    }
}
