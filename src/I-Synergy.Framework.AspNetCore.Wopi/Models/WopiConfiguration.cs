namespace ISynergy.Framework.Wopi.Models
{
    public class WopiConfiguration
    {
        static WopiConfiguration()
        {
            WopiHostCapabilities = new WopiHostCapabilities()
            {
                SupportsCobalt = false,
                SupportsContainers = false,
                SupportsDeleteFile = true,
                SupportsEcosystem = false,
                SupportsExtendedLockLength = true,
                SupportsFolders = false,
                SupportsGetLock = true,
                SupportsLocks = true,
                SupportsRename = true,
                SupportsUpdate = true,
                SupportsUserInfo = true
            };

            WopiHostProperties = new WopiHostProperties()
            {
                AllowExternalMarketplace = true,
                CloseButtonClosesWindow = true
            };
            PostMessageProperties = new PostMessageProperties();

            
        }

        public static string HostEndpoint { get; set; }
        public static string MachineName { get; set; }
        public static string ServerVersion { get; set; }

        public static WopiHostCapabilities WopiHostCapabilities { get; private set; }
        public static WopiHostProperties WopiHostProperties { get; private set; }
        public static PostMessageProperties PostMessageProperties { get; private set; }
        public static BreadcrumbProperties BreadcrumbProperties { get; private set; }
        //public static WopiFileServiceEvents WopiFileServiceEvents { get; private set; }
    } 
}
