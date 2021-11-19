using System;
using System.Collections.Generic;
using System.IO;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events;

/* Unmerged change from project 'ISynergy.Framework.IO (net5.0)'
Before:
using ISynergy.Framework.IO.Base;
After:
using ISynergy.Framework.IO.Base;
using ISynergy.Framework.IO.Watchers;
using ISynergy;
using ISynergy.Framework;
using ISynergy.Framework.IO;
*/

/* Unmerged change from project 'ISynergy.Framework.IO (net6.0)'
Before:
using ISynergy.Framework.IO.Base;
After:
using ISynergy.Framework.IO.Base;
using ISynergy.Framework.IO.Watchers;
using ISynergy;
using ISynergy.Framework;
using ISynergy.Framework.IO;
*/
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Watchers.Base;

namespace ISynergy.Framework.IO.Watchers
{
    /// <summary>
    /// Class WatchersList.
    /// Implements the <see cref="List{FileSystemWatcher}" />
    /// </summary>
    /// <seealso cref="List{FileSystemWatcher}" />
    public class WatchersList : List<FileSystemWatcher> { }

    /// <summary>
    /// Delegate WatcherEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
    public delegate void WatcherEventHandler(object sender, WatcherEventArgs e);

    /// <summary>
    /// Class Watcher.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class Watcher : BaseWatcher
    {
        /// <summary>
        /// The watchers
        /// </summary>
        private readonly WatchersList _watchers = new WatchersList();

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
        /// Initializes a new instance of the <see cref="Watcher"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        public Watcher(WatcherInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Creates the watcher.
        /// </summary>
        /// <param name="changedWatcher">if set to <c>true</c> [changed watcher].</param>
        /// <param name="filter">The filter.</param>
        protected override void CreateWatcher(bool changedWatcher, NotifyFilters filter)
        {
            FileSystemWatcher watcher = null;
            int bufferSize = (int)_watcherInfo.BufferKBytes * 1024;

            // Each "Change" filter gets its own watcher so we can determine *what* 
            // actually changed. This will allow us to react only to the change events 
            // that we actually want.  The reason I do this is because some programs 
            // fire TWO events for  certain changes. For example, Notepad sends two 
            // events when a file is created. One for CreationTime, and one for 
            // Attributes.
            if (changedWatcher && HandleNotifyFilter(filter))
            {
                watcher = new FileSystemWatcher(_watcherInfo.WatchPath)
                {
                    IncludeSubdirectories = _watcherInfo.IncludeSubFolders,
                    Filter = _watcherInfo.FileFilter,
                    NotifyFilter = filter,
                    InternalBufferSize = bufferSize
                };

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
                watcher = new FileSystemWatcher(_watcherInfo.WatchPath)
                {
                    IncludeSubdirectories = _watcherInfo.IncludeSubFolders,
                    Filter = _watcherInfo.FileFilter,
                    InternalBufferSize = bufferSize
                };

                if (HandleWatchesFilter(WatcherChangeTypes.Created))
                    watcher.Created += new FileSystemEventHandler(watcher_CreatedDeleted);

                if (HandleWatchesFilter(WatcherChangeTypes.Deleted))
                    watcher.Deleted += new FileSystemEventHandler(watcher_CreatedDeleted);

                if (HandleWatchesFilter(WatcherChangeTypes.Renamed))
                    watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            }

            if (watcher != null)
            {
                if (_watcherInfo.WatchForError)
                    watcher.Error += new ErrorEventHandler(watcher_Error);

                if (_watcherInfo.WatchForDisposed)
                    watcher.Disposed += new EventHandler(watcher_Disposed);

                _watchers.Add(watcher);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            foreach (var watcher in _watchers)
                watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public override void Stop()
        {
            foreach (var watcher in _watchers)
                watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Handles the ChangedAttribute event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedAttribute(object sender, FileSystemEventArgs e) =>
            EventChangedAttribute(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Attributes));

        /// <summary>
        /// Handles the ChangedCreationTime event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedCreationTime(object sender, FileSystemEventArgs e) =>
            EventChangedCreationTime(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.CreationTime));

        /// <summary>
        /// Handles the ChangedDirectoryName event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedDirectoryName(object sender, FileSystemEventArgs e) =>
            EventChangedDirectoryName(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.DirectoryName));

        /// <summary>
        /// Handles the ChangedFileName event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedFileName(object sender, FileSystemEventArgs e) =>
            EventChangedFileName(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.FileName));

        /// <summary>
        /// Handles the ChangedLastAccess event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedLastAccess(object sender, FileSystemEventArgs e) =>
            EventChangedLastAccess(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastAccess));

        /// <summary>
        /// Handles the ChangedLastWrite event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedLastWrite(object sender, FileSystemEventArgs e) =>
            EventChangedLastWrite(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastWrite));

        /// <summary>
        /// Handles the ChangedSecurity event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedSecurity(object sender, FileSystemEventArgs e) =>
            EventChangedSecurity(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Security));

        /// <summary>
        /// Handles the ChangedSize event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void watcher_ChangedSize(object sender, FileSystemEventArgs e) =>
            EventChangedSize(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Size));

        /// <summary>
        /// Handles the Disposed event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void watcher_Disposed(object sender, EventArgs e) =>
            EventDisposed(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.StandardEvent));

        /// <summary>
        /// Handles the Error event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void watcher_Error(object sender, ErrorEventArgs e) =>
            EventError(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.Error));

        /// <summary>
        /// Handles the Renamed event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void watcher_Renamed(object sender, RenamedEventArgs e) =>
            EventRenamed(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.Renamed));

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
                    EventCreated(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem));
                    break;
                case WatcherChangeTypes.Deleted:
                    EventDeleted(this, new WatcherEventArgs(sender as FileSystemWatcher, e, FileWatcherArgumentTypes.FileSystem));
                    break;
            }
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
        public override void DisposeWatchers()
        {
            foreach (var watcher in _watchers)
                watcher.Dispose();

            _watchers.Clear();
        }
    }
}
