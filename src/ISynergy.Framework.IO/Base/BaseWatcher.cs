namespace ISynergy.Framework.IO.Base
{
    /// <summary>
    /// Class BaseWatcher.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    public abstract class BaseWatcher : IDisposable
    {
        /// <summary>
        /// The watcher information
        /// </summary>
        protected WatcherInfo _watcherInfo = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWatcher" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        protected BaseWatcher(WatcherInfo info)
        {
            Argument.IsNotNull(nameof(info), info);
            _watcherInfo = info;
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected void Initialize()
        {
            // the buffer can be from 4 to 64 kbytes.  Default is 8
            _watcherInfo.BufferKBytes = Math.Max(4, Math.Min(_watcherInfo.BufferKBytes, 64));

            // create the main watcher (handles create/delete, rename, error, and dispose)
            // can't pass a null enum type, so we just pass ta dummy one on the first call
            CreateWatcher(false, _watcherInfo.ChangesFilters);

            // create a change watcher for each NotifyFilter item
            CreateWatcher(true, NotifyFilters.Attributes);
            CreateWatcher(true, NotifyFilters.CreationTime);
            CreateWatcher(true, NotifyFilters.DirectoryName);
            CreateWatcher(true, NotifyFilters.FileName);
            CreateWatcher(true, NotifyFilters.LastAccess);
            CreateWatcher(true, NotifyFilters.LastWrite);
            CreateWatcher(true, NotifyFilters.Security);
            CreateWatcher(true, NotifyFilters.Size);
        }

        /// <summary>
        /// Creates the watcher.
        /// </summary>
        /// <param name="changedWatcher">if set to <c>true</c> [changed watcher].</param>
        /// <param name="filter">The filter.</param>
        protected abstract void CreateWatcher(bool changedWatcher, NotifyFilters filter);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            DisposeWatchers();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BaseWatcher"/> class.
        /// </summary>
        ~BaseWatcher()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the watchers.
        /// </summary>
        public abstract void DisposeWatchers();
    }
}
