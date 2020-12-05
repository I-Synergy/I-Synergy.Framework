using System.IO;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Class WatcherEventArgs.
    /// </summary>
    public class WatcherEventArgs
    {
        /// <summary>
        /// Gets the watcher.
        /// </summary>
        /// <value>The watcher.</value>
        public FileSystemWatcher Watcher { get; private set; }
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public object Arguments { get; private set; }
        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <value>The type of the argument.</value>
        public FileWatcherArgumentTypes ArgumentType { get; private set; }
        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public NotifyFilters Filter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEventArgs"/> class.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <param name="filter">The filter.</param>
        public WatcherEventArgs(FileSystemWatcher watcher,
                                object arguments,
                                FileWatcherArgumentTypes argumentType,
                                NotifyFilters filter)
        {
            Watcher = watcher;
            Arguments = arguments;
            ArgumentType = argumentType;
            Filter = filter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEventArgs"/> class.
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentType">Type of the argument.</param>
        public WatcherEventArgs(FileSystemWatcher watcher,
                                object arguments,
                                FileWatcherArgumentTypes argumentType)
        {
            Watcher = watcher;
            Arguments = arguments;
            ArgumentType = argumentType;
            Filter = NotifyFilters.Attributes;
        }
    }
}
