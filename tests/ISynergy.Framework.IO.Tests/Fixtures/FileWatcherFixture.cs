﻿using System;
using System.Collections.Generic;
using System.IO;
using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models.Tests;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.IO.Tests.Fixtures
{
    /// <summary>
    /// Class FileWatcherFixture.
    /// </summary>
    public class FileWatcherFixture
    {
        /// <summary>
        /// The file watcher
        /// </summary>
        public Watcher FileWatcher = null;

        /// <summary>
        /// Gets the observed files.
        /// </summary>
        /// <value>The observed files.</value>
        public List<ObservedFile> ObservedFiles { get; private set; }

        /// <summary>
        /// Initializes the watcher.
        /// </summary>
        /// <param name="fileOrFolderToWatch">The file or folder to watch.</param>
        /// <param name="includeSubFolder">if set to <c>true</c> [include sub folder].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="FileNotFoundException">The folder (or file) specified does not exist.</exception>
        public bool InitializeWatcher(string fileOrFolderToWatch, bool includeSubFolder)
        {
            if (Directory.Exists(fileOrFolderToWatch) || File.Exists(fileOrFolderToWatch))
            {
                ObservedFiles = new List<ObservedFile>();

                FileWatcher = new Watcher(new WatcherInfo
                {
                    ChangesFilters = NotifyFilters.Attributes |
                                      NotifyFilters.CreationTime |
                                      NotifyFilters.DirectoryName |
                                      NotifyFilters.FileName |
                                      NotifyFilters.LastAccess |
                                      NotifyFilters.LastWrite |
                                      NotifyFilters.Security |
                                      NotifyFilters.Size,

                    IncludeSubFolders = includeSubFolder,
                    WatchesFilters = WatcherChangeTypes.All,
                    WatchForDisposed = true,
                    WatchForError = true,
                    WatchPath = fileOrFolderToWatch,
                    BufferKBytes = 8
                });

                AddEventHandlers();

                return true;
            }
            else
            {
                throw new FileNotFoundException("The folder (or file) specified does not exist.", fileOrFolderToWatch);
            }
        }

        /// <summary>
        /// Adds the event handlers.
        /// </summary>
        private void AddEventHandlers()
        {
            Argument.IsNotNull(nameof(FileWatcher), FileWatcher);

            FileWatcher.EventChangedAttribute += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedCreationTime += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedDirectoryName += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedFileName += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastAccess += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastWrite += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSecurity += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSize += new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventCreated += new WatcherEventHandler(fileWatcher_EventCreated);
            FileWatcher.EventDeleted += new WatcherEventHandler(fileWatcher_EventDeleted);
            FileWatcher.EventDisposed += new WatcherEventHandler(fileWatcher_EventDisposed);
            FileWatcher.EventError += new WatcherEventHandler(fileWatcher_EventError);
            FileWatcher.EventRenamed += new WatcherEventHandler(fileWatcher_EventRenamed);
        }

        /// <summary>
        /// Removes the event handlers.
        /// </summary>
        public void RemoveEventHandlers()
        {
            Argument.IsNotNull(nameof(FileWatcher), FileWatcher);

            FileWatcher.EventChangedAttribute -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedCreationTime -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedDirectoryName -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedFileName -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastAccess -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastWrite -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSecurity -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSize -= new WatcherEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventCreated -= new WatcherEventHandler(fileWatcher_EventCreated);
            FileWatcher.EventDeleted -= new WatcherEventHandler(fileWatcher_EventDeleted);
            FileWatcher.EventDisposed -= new WatcherEventHandler(fileWatcher_EventDisposed);
            FileWatcher.EventError -= new WatcherEventHandler(fileWatcher_EventError);
            FileWatcher.EventRenamed -= new WatcherEventHandler(fileWatcher_EventRenamed);
        }

        /// <summary>
        /// Handles the EventRenamed event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventRenamed(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Renamed", "N/A", (e.Arguments == null) ? "Null argument object" : ((RenamedEventArgs)(e.Arguments)).FullPath));
            }
        }

        /// <summary>
        /// Handles the EventError event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventError(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Error", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        /// <summary>
        /// Handles the EventDisposed event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventDisposed(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Disposed", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        /// <summary>
        /// Handles the EventDeleted event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventDeleted(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Deleted", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        /// <summary>
        /// Handles the EventCreated event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventCreated(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Created", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        /// <summary>
        /// Handles the EventChanged event of the fileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        private void fileWatcher_EventChanged(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Change", e.Filter.ToString(), (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }
    }
}
