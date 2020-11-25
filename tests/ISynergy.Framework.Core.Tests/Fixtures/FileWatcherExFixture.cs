using System;
using System.Collections.Generic;
using System.IO;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.IO;
using ISynergy.Framework.Core.Models.Tests;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Tests.Fixtures
{
    public class FileWatcherExFixture
    {
        public WatcherEx FileWatcher = null;
        
        public List<ObservedFile> ObservedFiles { get; private set; }

        public FileWatcherExFixture()
        {
            ObservedFiles = new List<ObservedFile>();
        }

        public bool InitializeWatcher(string fileOrFolderToWatch, bool includeSubFolder)
        {
            if (Directory.Exists(fileOrFolderToWatch) || File.Exists(fileOrFolderToWatch))
            {
                FileWatcher = new WatcherEx(new WatcherInfo
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
                    WatchForError = false,
                    WatchPath = fileOrFolderToWatch,
                    BufferKBytes = 8,
                    MonitorPathInterval = 250
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

            FileWatcher.EventChangedAttribute += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedCreationTime += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedDirectoryName += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedFileName += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastAccess += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedLastWrite += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSecurity += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventChangedSize += new WatcherExEventHandler(fileWatcher_EventChanged);
            FileWatcher.EventCreated += new WatcherExEventHandler(fileWatcher_EventCreated);
            FileWatcher.EventDeleted += new WatcherExEventHandler(fileWatcher_EventDeleted);
            FileWatcher.EventDisposed += new WatcherExEventHandler(fileWatcher_EventDisposed);
            FileWatcher.EventError += new WatcherExEventHandler(fileWatcher_EventError);
            FileWatcher.EventRenamed += new WatcherExEventHandler(fileWatcher_EventRenamed);
            FileWatcher.EventPathAvailability += new WatcherExEventHandler(fileWatcher_EventPathAvailability);
        }

        public void RemoveEventHandlers()
        {
            if(FileWatcher != null)
            {
                FileWatcher.EventChangedAttribute -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedCreationTime -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedDirectoryName -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedFileName -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedLastAccess -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedLastWrite -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedSecurity -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventChangedSize -= new WatcherExEventHandler(fileWatcher_EventChanged);
                FileWatcher.EventCreated -= new WatcherExEventHandler(fileWatcher_EventCreated);
                FileWatcher.EventDeleted -= new WatcherExEventHandler(fileWatcher_EventDeleted);
                FileWatcher.EventDisposed -= new WatcherExEventHandler(fileWatcher_EventDisposed);
                FileWatcher.EventError -= new WatcherExEventHandler(fileWatcher_EventError);
                FileWatcher.EventRenamed -= new WatcherExEventHandler(fileWatcher_EventRenamed);
                FileWatcher.EventPathAvailability -= new WatcherExEventHandler(fileWatcher_EventPathAvailability);
            }
        }

        private void fileWatcher_EventRenamed(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Renamed", "N/A", (e.Arguments == null) ? "Null argument object" : ((RenamedEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventError(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Error", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        private void fileWatcher_EventDisposed(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Disposed", "N/A", (e.Arguments == null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
            }
        }

        private void fileWatcher_EventDeleted(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Deleted", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventCreated(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Created", "N/A", (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventChanged(object sender, WatcherExEventArgs e)
        {
            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile("Change", e.Filter.ToString(), (e.Arguments == null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
            }
        }

        private void fileWatcher_EventPathAvailability(object sender, WatcherExEventArgs e)
        {
            var eventName = "Availability";
            var filterName = "N/A";
            string status;

            if (e.Arguments == null)
            {
                status = "Null argument object";
            }
            else
            {
                status = (((PathAvailablitiyEventArgs)(e.Arguments)).PathIsAvailable) ? "Connected"
                                                                                      : "Disconnected";
            }

            lock (ObservedFiles)
            {
                ObservedFiles.Add(new ObservedFile(eventName, filterName, status));
            }
        }
    }
}
