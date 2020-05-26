namespace ISynergy.Framework.Wopi.Models
{
    public class WopiHostCapabilities : IWopiHostCapabilities
    {
        public bool SupportsCobalt { get; set; }
        public bool SupportsContainers { get; set; }
        public bool SupportsDeleteFile { get; set; }
        public bool SupportsEcosystem { get; set; }
        public bool SupportsExtendedLockLength { get; set; }
        public bool SupportsFolders { get; set; }
        public bool SupportsGetLock { get; set; }
        public bool SupportsLocks { get; set; }
        public bool SupportsRename { get; set; }
        public bool SupportsUpdate { get; set; }
        public bool SupportsUserInfo { get; set; }
    }
}
