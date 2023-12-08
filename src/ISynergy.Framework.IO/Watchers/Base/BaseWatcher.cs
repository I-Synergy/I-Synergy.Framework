using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.IO.Events.Base;
using ISynergy.Framework.IO.Models;

namespace ISynergy.Framework.IO.Watchers.Base;

/// <summary>
/// Class BaseWatcher.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public abstract class BaseWatcher<TWatcher, TWatcherEventArgs> : IDisposable
    where TWatcher : FileSystemWatcher, new()
    where TWatcherEventArgs : BaseEventArgs<TWatcher>
{
    /// <summary>
    /// Delegate WatcherEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The instance containing the event data.</param>
    public delegate void WatcherEventHandler(object sender, TWatcherEventArgs e);

    /// <summary>
    /// Occurs when [event changed attribute].
    /// </summary>
    public event WatcherEventHandler EventChangedAttribute = delegate { };
    /// <summary>
    /// Occurs when [event changed creation time].
    /// </summary>
    public event WatcherEventHandler EventChangedCreationTime = delegate { };
    /// <summary>
    /// Occurs when [event changed directory name].
    /// </summary>
    public event WatcherEventHandler EventChangedDirectoryName = delegate { };
    /// <summary>
    /// Occurs when [event changed file name].
    /// </summary>
    public event WatcherEventHandler EventChangedFileName = delegate { };
    /// <summary>
    /// Occurs when [event changed last access].
    /// </summary>
    public event WatcherEventHandler EventChangedLastAccess = delegate { };
    /// <summary>
    /// Occurs when [event changed last write].
    /// </summary>
    public event WatcherEventHandler EventChangedLastWrite = delegate { };
    /// <summary>
    /// Occurs when [event changed security].
    /// </summary>
    public event WatcherEventHandler EventChangedSecurity = delegate { };
    /// <summary>
    /// Occurs when [event changed size].
    /// </summary>
    public event WatcherEventHandler EventChangedSize = delegate { };
    /// <summary>
    /// Occurs when [event created].
    /// </summary>
    public event WatcherEventHandler EventCreated = delegate { };
    /// <summary>
    /// Occurs when [event deleted].
    /// </summary>
    public event WatcherEventHandler EventDeleted = delegate { };
    /// <summary>
    /// Occurs when [event renamed].
    /// </summary>
    public event WatcherEventHandler EventRenamed = delegate { };
    /// <summary>
    /// Occurs when [event error].
    /// </summary>
    public event WatcherEventHandler EventError = delegate { };
    /// <summary>
    /// Occurs when [event disposed].
    /// </summary>
    public event WatcherEventHandler EventDisposed = delegate { };

    /// <summary>
    /// The watchers
    /// </summary>
    protected readonly List<TWatcher> _watchers = new List<TWatcher>();

    /// <summary>
    /// The watcher information
    /// </summary>
    protected WatcherInfo _watcherInfo = null;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="info">The information.</param>
    protected BaseWatcher(WatcherInfo info)
    {
        Argument.IsNotNull(info);
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
    protected void CreateWatcher(bool changedWatcher, NotifyFilters filter)
    {
        TWatcher watcher = null;
        int bufferSize = (int)_watcherInfo.BufferKBytes * 1024;

        // Each "Change" filter gets its own watcher so we can determine *what* 
        // actually changed. This will allow us to react only to the change events 
        // that we actually want.  The reason I do this is because some programs 
        // fire TWO events for  certain changes. For example, Notepad sends two 
        // events when a file is created. One for CreationTime, and one for 
        // Attributes.
        if (changedWatcher && HandleNotifyFilter(filter))
        {
            watcher = (TWatcher)Activator.CreateInstance(typeof(TWatcher), new object[] { _watcherInfo.WatchPath });
            watcher.IncludeSubdirectories = _watcherInfo.IncludeSubFolders;
            watcher.Filter = _watcherInfo.FileFilter;
            watcher.NotifyFilter = filter;
            watcher.InternalBufferSize = bufferSize;

            switch (filter)
            {
                case NotifyFilters.Attributes:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedAttribute);
                    break;
                case NotifyFilters.CreationTime:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedCreationTime);
                    break;
                case NotifyFilters.DirectoryName:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedDirectoryName);
                    break;
                case NotifyFilters.FileName:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedFileName);
                    break;
                case NotifyFilters.LastAccess:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedLastAccess);
                    break;
                case NotifyFilters.LastWrite:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedLastWrite);
                    break;
                case NotifyFilters.Security:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedSecurity);
                    break;
                case NotifyFilters.Size:
                    watcher.Changed += new FileSystemEventHandler(watcher_ChangedSize);
                    break;
            }
        }

        // All other FileSystemWatcher events are handled through a single "main" 
        // watcher.
        else if (HandleWatchesFilter(WatcherChangeTypes.Created) ||
                HandleWatchesFilter(WatcherChangeTypes.Deleted) ||
                HandleWatchesFilter(WatcherChangeTypes.Renamed) ||
                _watcherInfo.WatchForError ||
                _watcherInfo.WatchForDisposed)
        {
            watcher = (TWatcher)Activator.CreateInstance(typeof(TWatcher), new object[] { _watcherInfo.WatchPath });
            watcher.IncludeSubdirectories = _watcherInfo.IncludeSubFolders;
            watcher.Filter = _watcherInfo.FileFilter;
            watcher.InternalBufferSize = bufferSize;

            if (HandleWatchesFilter(WatcherChangeTypes.Created))
                watcher.Created += new FileSystemEventHandler(watcher_CreatedDeleted);

            if (HandleWatchesFilter(WatcherChangeTypes.Deleted))
                watcher.Deleted += new FileSystemEventHandler(watcher_CreatedDeleted);

            if (HandleWatchesFilter(WatcherChangeTypes.Renamed))
                watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
        }

        if (watcher is not null)
        {
            if (_watcherInfo.WatchForError)
                watcher.Error += new ErrorEventHandler(watcher_Error);

            if (_watcherInfo.WatchForDisposed)
                watcher.Disposed += new EventHandler(watcher_Disposed);

            _watchers.Add(watcher);
        }
    }

    /// <summary>
    /// Handles the ChangedAttribute event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedAttribute(object sender, FileSystemEventArgs e) =>
        EventChangedAttribute(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Attributes }));

    /// <summary>
    /// Handles the ChangedCreationTime event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedCreationTime(object sender, FileSystemEventArgs e) =>
        EventChangedCreationTime(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.CreationTime }));

    /// <summary>
    /// Handles the ChangedDirectoryName event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedDirectoryName(object sender, FileSystemEventArgs e) =>
        EventChangedDirectoryName(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.DirectoryName }));

    /// <summary>
    /// Handles the ChangedFileName event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedFileName(object sender, FileSystemEventArgs e) =>
        EventChangedFileName(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.FileName }));

    /// <summary>
    /// Handles the ChangedLastAccess event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedLastAccess(object sender, FileSystemEventArgs e) =>
        EventChangedLastAccess(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastAccess }));

    /// <summary>
    /// Handles the ChangedLastWrite event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedLastWrite(object sender, FileSystemEventArgs e) =>
        EventChangedLastWrite(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastWrite }));

    /// <summary>
    /// Handles the ChangedSecurity event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedSecurity(object sender, FileSystemEventArgs e) =>
        EventChangedSecurity(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Security }));

    /// <summary>
    /// Handles the ChangedSize event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_ChangedSize(object sender, FileSystemEventArgs e) =>
        EventChangedSize(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Size }));

    /// <summary>
    /// Handles the Disposed event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void watcher_Disposed(object sender, EventArgs e) =>
        EventDisposed(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.StandardEvent }));

    /// <summary>
    /// Handles the Error event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
    private void watcher_Error(object sender, ErrorEventArgs e) =>
        EventError(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.Error }));

    /// <summary>
    /// Handles the Renamed event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
    private void watcher_Renamed(object sender, RenamedEventArgs e) =>
        EventRenamed(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.Renamed }));

    /// <summary>
    /// Handles the CreatedDeleted event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_CreatedDeleted(object sender, FileSystemEventArgs e)
    {
        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Created:
                EventCreated(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem }));
                break;
            case WatcherChangeTypes.Deleted:
                EventDeleted(this, (TWatcherEventArgs)Activator.CreateInstance(typeof(TWatcherEventArgs), new object[] { sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem }));
                break;
        }
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    public void Start()
    {
        foreach (var watcher in _watchers)
            watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        foreach (var watcher in _watchers)
            watcher.EnableRaisingEvents = false;
    }

    /// <summary>
    /// Handles the notify filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool HandleNotifyFilter(NotifyFilters filter) =>
        (_watcherInfo.ChangesFilters & filter) == filter;

    /// <summary>
    /// Handles the watches filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool HandleWatchesFilter(WatcherChangeTypes filter) =>
        (_watcherInfo.WatchesFilters & filter) == filter;

    /// <summary>
    /// Disposes the watchers.
    /// </summary>
    public void DisposeWatchers()
    {
        foreach (var watcher in _watchers)
            watcher.Dispose();

        _watchers.Clear();
    }

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
    /// Finalizes an instance of the class.
    /// </summary>
    ~BaseWatcher()
    {
        Dispose(false);
    }
}
