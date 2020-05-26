namespace ISynergy.Framework.AspNetCore.WebDav.Server.Locking
{
    /// <summary>
    /// An interface for a background task that removes expired locks
    /// </summary>
    public interface ILockCleanupTask
    {
        /// <summary>
        /// Adds a lock to be tracked by this cleanup task.
        /// </summary>
        /// <param name="lockManager">The lock manager that created this active lock.</param>
        /// <param name="activeLock">The active lock to track</param>
        void Add(ILockManager lockManager, IActiveLock activeLock);

        /// <summary>
        /// Removes the active lock so that it isn't tracked any more by this cleanup task.
        /// </summary>
        /// <param name="activeLock">The active lock to remove</param>
        void Remove(IActiveLock activeLock);
    }
}
