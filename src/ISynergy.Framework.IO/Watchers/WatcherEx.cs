using System;
using System.Collections.Generic;
using System.IO;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Watchers.Base;

namespace ISynergy.Framework.IO.Watchers
{
    /// <summary>
    /// Class WatchersExList.
    /// Implements the <see cref="List{FileSystemWatcherEx}" />
    /// </summary>
    /// <seealso cref="List{FileSystemWatcherEx}" />
    public class WatchersExList : List<FileSystemWatcherEx> { }

    /// <summary>
    /// Delegate WatcherExEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="WatcherExEventArgs" /> instance containing the event data.</param>
    public delegate void WatcherExEventHandler(object sender, WatcherExEventArgs e);

    /// <summary>
    /// Class WatcherEx.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class WatcherEx : BaseWatcher
    {
        /// <summary>
        /// The watchers
        /// </summary>
        private readonly WatchersExList _watchers = new WatchersExList();

        /// <summary>
        /// Occurs when [event changed attribute].
        /// </summary>
        public event WatcherExEventHandler EventChangedAttribute = delegate { };
        /// <summary>
        /// Occurs when [event changed creation time].
        /// </summary>
        public event WatcherExEventHandler EventChangedCreationTime = delegate { };
        /// <summary>
        /// Occurs when [event changed directory name].
        /// </summary>
        public event WatcherExEventHandler EventChangedDirectoryName = delegate { };
        /// <summary>
        /// Occurs when [event changed file name].
        /// </summary>
        public event WatcherExEventHandler EventChangedFileName = delegate { };
        /// <summary>
        /// Occurs when [event changed last access].
        /// </summary>
        public event WatcherExEventHandler EventChangedLastAccess = delegate { };
        /// <summary>
        /// Occurs when [event changed last write].
        /// </summary>
        public event WatcherExEventHandler EventChangedLastWrite = delegate { };
        /// <summary>
        /// Occurs when [event changed security].
        /// </summary>
        public event WatcherExEventHandler EventChangedSecurity = delegate { };
        /// <summary>
        /// Occurs when [event changed size].
        /// </summary>
        public event WatcherExEventHandler EventChangedSize = delegate { };
        /// <summary>
        /// Occurs when [event created].
        /// </summary>
        public event WatcherExEventHandler EventCreated = delegate { };
        /// <summary>
        /// Occurs when [event deleted].
        /// </summary>
        public event WatcherExEventHandler EventDeleted = delegate { };
        /// <summary>
        /// Occurs when [event renamed].
        /// </summary>
        public event WatcherExEventHandler EventRenamed = delegate { };
        /// <summary>
        /// Occurs when [event error].
        /// </summary>
        public event WatcherExEventHandler EventError = delegate { };
        /// <summary>
        /// Occurs when [event disposed].
        /// </summary>
        public event WatcherExEventHandler EventDisposed = delegate { };
        /// <summary>
        /// Occurs when [event path availability].
        /// </summary>
        public event WatcherExEventHandler EventPathAvailability = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEx" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        public WatcherEx(WatcherInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Actually creates the necessary FileSystemWatcher objects, depending oin which
        /// notify filters and change types the user specified.
        /// </summary>
        /// <param name="changedWatcher">if set to <c>true</c> [changed watcher].</param>
        /// <param name="filter">The filter.</param>
        protected override void CreateWatcher(bool changedWatcher, NotifyFilters filter)
        {
            FileSystemWatcherEx watcher = null;
            int bufferSize = (int)_watcherInfo.BufferKBytes * 1024;
            // Each "Change" filter gets its own watcher so we can determine *what* 
            // actually changed. This will allow us to react only to the change events 
            // that we actually want.  The reason I do this is because some programs 
            // fire TWO events for  certain changes. For example, Notepad sends two 
            // events when a file is created. One for CreationTime, and one for 
            // Attributes.
            if (changedWatcher && HandleNotifyFilter(filter))
            {
                watcher = new FileSystemWatcherEx(_watcherInfo.WatchPath)
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
                watcher = new FileSystemWatcherEx(_watcherInfo.WatchPath, _watcherInfo.MonitorPathInterval)
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

                if (_watcherInfo.MonitorPathInterval > 0)
                    watcher.PathAvailabilityEvent += new PathAvailabilityHandler(watcher_EventPathAvailability);
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
        /// Fired when the watcher responsible for monitoring attribute changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedAttribute(object sender, FileSystemEventArgs e) =>
            EventChangedAttribute(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Attributes));

        /// <summary>
        /// Fired when the watcher responsible for monitoring creation time changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedCreationTime(object sender, FileSystemEventArgs e) =>
            EventChangedCreationTime(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.CreationTime));

        /// <summary>
        /// Fired when the watcher responsible for monitoring directory name changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedDirectoryName(object sender, FileSystemEventArgs e) =>
            EventChangedDirectoryName(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.DirectoryName));

        /// <summary>
        /// Fired when the watcher responsible for monitoring file name changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedFileName(object sender, FileSystemEventArgs e) =>
            EventChangedFileName(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.FileName));

        /// <summary>
        /// Fired when the watcher responsible for monitoring last access date/time
        /// changes is triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedLastAccess(object sender, FileSystemEventArgs e) =>
            EventChangedLastAccess(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastAccess));

        /// <summary>
        /// Fired when the watcher responsible for monitoring last write date/time
        /// changes is triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedLastWrite(object sender, FileSystemEventArgs e) =>
            EventChangedLastWrite(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.LastWrite));

        /// <summary>
        /// Fired when the watcher responsible for monitoring security changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedSecurity(object sender, FileSystemEventArgs e) =>
            EventChangedSecurity(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Security));

        /// <summary>
        /// Fired when the watcher responsible for monitoring size changes is
        /// triggered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_ChangedSize(object sender, FileSystemEventArgs e) =>
            EventChangedSize(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem, NotifyFilters.Size));

        /// <summary>
        /// Fired when an internal watcher is disposed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void watcher_Disposed(object sender, EventArgs e) =>
            EventDisposed(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.StandardEvent));

        /// <summary>
        /// Fired when the main watcher detects an error (the watcher that detected the
        /// error is part of the event's arguments object)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ErrorEventArgs" /> instance containing the event data.</param>
        private void watcher_Error(object sender, ErrorEventArgs e) =>
            EventError(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.Error));

        /// <summary>
        /// Fired when the main watcher detects a file rename.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenamedEventArgs" /> instance containing the event data.</param>
        private void watcher_Renamed(object sender, RenamedEventArgs e) =>
            EventRenamed(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.Renamed));

        /// <summary>
        /// Handles the CreatedDeleted event of the watcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs" /> instance containing the event data.</param>
        private void watcher_CreatedDeleted(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    EventCreated(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem));
                    break;
                case WatcherChangeTypes.Deleted:
                    EventDeleted(this, new WatcherExEventArgs(sender as FileSystemWatcherEx, e, FileWatcherArgumentTypes.FileSystem));
                    break;
            }
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
