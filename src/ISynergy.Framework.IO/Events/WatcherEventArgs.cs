using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events.Base;

namespace ISynergy.Framework.IO.Events;

/// <summary>
/// Class WatcherEventArgs.
/// </summary>
public class WatcherEventArgs : BaseEventArgs<FileSystemWatcher>
{
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
        : base(watcher, arguments, argumentType, filter)
    {
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
        : base(watcher, arguments, argumentType)
    {
    }
}
