using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Watchers.Base;

namespace ISynergy.Framework.IO.Watchers;

/// <summary>
/// Class WatcherEx.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public class WatcherEx : BaseWatcher<FileSystemWatcherEx, WatcherExEventArgs>
{
    /// <summary>
    /// Occurs when [event path availability].
    /// </summary>
    public event WatcherEventHandler EventPathAvailability = delegate { };

    /// <summary>
    /// Initializes a new instance of the <see cref="WatcherEx" /> class.
    /// </summary>
    /// <param name="info">The information.</param>
    public WatcherEx(WatcherInfo info)
        : base(info)
    {
    }

    /// <summary>
    /// Handles the EventPathAvailability event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PathAvailablitiyEventArgs" /> instance containing the event data.</param>
    private void watcher_EventPathAvailability(object sender, PathAvailablitiyEventArgs e)
    {
        EventPathAvailability(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.PathAvailability));

        if (e.PathIsAvailable)
        {
            DisposeWatchers();
            Initialize();
        }
    }
}
