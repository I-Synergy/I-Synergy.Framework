using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.IO.Events.Base;

/// <summary>
/// Base EventArgs for watchers.
/// </summary>
/// <typeparam name="TWatcher"></typeparam>
public abstract class BaseEventArgs<TWatcher>
    where TWatcher : FileSystemWatcher, new()
{
    /// <summary>
    /// Gets the watcher.
    /// </summary>
    /// <value>The watcher.</value>
    public TWatcher Watcher { get; private set; }
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
    protected BaseEventArgs(TWatcher watcher,
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
    protected BaseEventArgs(TWatcher watcher,
                            object arguments,
                            FileWatcherArgumentTypes argumentType)
    {
        Watcher = watcher;
        Arguments = arguments;
        ArgumentType = argumentType;
        Filter = NotifyFilters.Attributes;
    }
}
