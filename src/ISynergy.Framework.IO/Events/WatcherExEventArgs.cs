using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events.Base;
using ISynergy.Framework.IO.Watchers;

namespace ISynergy.Framework.IO.Events
{
    /// <summary>
    /// Class WatcherExEventArgs.
    /// </summary>
    public class WatcherExEventArgs : BaseEventArgs<FileSystemWatcherEx>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherExEventArgs"/> class.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <param name="filter">The filter.</param>
        public WatcherExEventArgs(FileSystemWatcherEx watcher,
                                  object arguments,
                                  FileWatcherArgumentTypes argumentType,
                                  NotifyFilters filter)
            : base(watcher, arguments, argumentType, filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherExEventArgs"/> class.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentType">Type of the argument.</param>
        public WatcherExEventArgs(FileSystemWatcherEx watcher,
                                  object arguments,
                                  FileWatcherArgumentTypes argumentType)
            : base(watcher, arguments, argumentType)
        {
        }
    }
}
