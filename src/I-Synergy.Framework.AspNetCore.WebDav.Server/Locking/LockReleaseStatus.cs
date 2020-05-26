namespace ISynergy.Framework.AspNetCore.WebDav.Server.Locking
{
    /// <summary>
    /// Result of an UNLOCK
    /// </summary>
    public enum LockReleaseStatus
    {
        /// <summary>
        /// UNLOCK successful
        /// </summary>
        Success,

        /// <summary>
        /// No lock to release found
        /// </summary>
        NoLock,

        /// <summary>
        /// The given path doesn't match the scope of the lock
        /// </summary>
        InvalidLockRange,
    }
}
