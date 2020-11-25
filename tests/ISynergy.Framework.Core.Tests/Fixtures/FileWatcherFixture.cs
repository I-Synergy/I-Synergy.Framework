using System;
using System.Collections.Generic;
using System.IO;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.IO;
using ISynergy.Framework.Core.Models.Tests;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Tests.Fixtures
{
    public class FileWatcherFixture
    {
        public Watcher FileWatcher = null;

        public List<ObservedFile> ObservedFiles { get; private set; }

        public FileWatcherFixture()
        {
            ObservedFiles = new List<ObservedFile>();
        }

        public bool InitializeWatcher(string fileOrFolderToWatch, bool includeSubFolder)
        {
            if (Directory.Exists(fileOrFolderToWatch) || File.Exists(fileOrFolderToWatch))
            {
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

        private void fileWatcher_EventRenamed(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Renamed", "N/A", (e.Arguments == null) ? "Null argument object" : ((RenamedEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventError(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Error", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        private void fileWatcher_EventDisposed(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Disposed", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        private void fileWatcher_EventDeleted(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Deleted", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventCreated(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Created", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventChanged(object sender, WatcherEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Change", e.Filter.ToString(), (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }
    }
}
