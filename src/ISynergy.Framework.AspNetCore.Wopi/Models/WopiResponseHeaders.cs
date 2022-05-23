namespace ISynergy.Framework.Wopi.Models
{
    /// <summary>
    /// Contains valid WOPI response headers
    /// </summary>
    public class WopiResponseHeaders
    {
        //WOPI Header Consts
        public const string HOST_ENDPOINT = "X-WOPI-HostEndpoint";
        public const string INVALID_FILE_NAME_ERROR = "X-WOPI-InvalidFileNameError";
        public const string LOCK = "X-WOPI-Lock";
        public const string LOCK_FAILURE_REASON = "X-WOPI-LockFailureReason";
        public const string LOCKED_BY_OTHER_INTERFACE = "X-WOPI-LockedByOtherInterface";
        public const string MACHINE_NAME = "X-WOPI-MachineName";
        public const string PREF_TRACE = "X-WOPI-PerfTrace";
        public const string SERVER_ERROR = "X-WOPI-ServerError";
        public const string SERVER_VERSION = "X-WOPI-ServerVersion";
        public const string VALID_RELATIVE_TARGET = "X-WOPI-ValidRelativeTarget";
        public const string ITEM_VERSION = "X-WOPI-ItemVersion";
    }
}