using System;

namespace ISynergy.Framework.Wopi.Models
{
    internal interface IFileUrlProperties
    {
        Uri CloseUrl { get; set; }
        Uri DownloadUrl { get; set; }
        Uri FileSharingUrl { get; set; }
        Uri HostEditUrl { get; set; }
        Uri HostViewUrl { get; set; }
        Uri SignoutUrl { get; set; }
    }
}
