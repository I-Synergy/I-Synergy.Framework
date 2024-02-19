using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Models.Tests;
using ISynergy.Framework.IO.Watchers;

namespace ISynergy.Framework.IO.Tests.Fixtures;

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
            ObservedFiles = [];

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

        throw new FileNotFoundException("The folder (or file) specified does not exist.", fileOrFolderToWatch);
    }

    /// <summary>
    /// Adds the event handlers.
    /// </summary>
    private void AddEventHandlers()
    {
        Argument.IsNotNull(FileWatcher);

        FileWatcher.EventChangedAttribute += fileWatcher_EventChanged;
        FileWatcher.EventChangedCreationTime += fileWatcher_EventChanged;
        FileWatcher.EventChangedDirectoryName += fileWatcher_EventChanged;
        FileWatcher.EventChangedFileName += fileWatcher_EventChanged;
        FileWatcher.EventChangedLastAccess += fileWatcher_EventChanged;
        FileWatcher.EventChangedLastWrite += fileWatcher_EventChanged;
        FileWatcher.EventChangedSecurity += fileWatcher_EventChanged;
        FileWatcher.EventChangedSize += fileWatcher_EventChanged;
        FileWatcher.EventCreated += fileWatcher_EventCreated;
        FileWatcher.EventDeleted += fileWatcher_EventDeleted;
        FileWatcher.EventDisposed += fileWatcher_EventDisposed;
        FileWatcher.EventError += fileWatcher_EventError;
        FileWatcher.EventRenamed += fileWatcher_EventRenamed;
    }

    /// <summary>
    /// Removes the event handlers.
    /// </summary>
    public void RemoveEventHandlers()
    {
        Argument.IsNotNull(FileWatcher);

        FileWatcher.EventChangedAttribute -= fileWatcher_EventChanged;
        FileWatcher.EventChangedCreationTime -= fileWatcher_EventChanged;
        FileWatcher.EventChangedDirectoryName -= fileWatcher_EventChanged;
        FileWatcher.EventChangedFileName -= fileWatcher_EventChanged;
        FileWatcher.EventChangedLastAccess -= fileWatcher_EventChanged;
        FileWatcher.EventChangedLastWrite -= fileWatcher_EventChanged;
        FileWatcher.EventChangedSecurity -= fileWatcher_EventChanged;
        FileWatcher.EventChangedSize -= fileWatcher_EventChanged;
        FileWatcher.EventCreated -= fileWatcher_EventCreated;
        FileWatcher.EventDeleted -= fileWatcher_EventDeleted;
        FileWatcher.EventDisposed -= fileWatcher_EventDisposed;
        FileWatcher.EventError -= fileWatcher_EventError;
        FileWatcher.EventRenamed -= fileWatcher_EventRenamed;
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
            ObservedFiles.Add(new ObservedFile("Renamed", "N/A", (e.Arguments is null) ? "Null argument object" : ((RenamedEventArgs)(e.Arguments)).FullPath));
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
            ObservedFiles.Add(new ObservedFile("Error", "N/A", (e.Arguments is null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
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
            ObservedFiles.Add(new ObservedFile("Disposed", "N/A", (e.Arguments is null) ? "Null argument object" : ((EventArgs)(e.Arguments)).ToString()));
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
            ObservedFiles.Add(new ObservedFile("Deleted", "N/A", (e.Arguments is null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
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
            ObservedFiles.Add(new ObservedFile("Created", "N/A", (e.Arguments is null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
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
            ObservedFiles.Add(new ObservedFile("Change", e.Filter.ToString(), (e.Arguments is null) ? "Null argument object" : ((FileSystemEventArgs)(e.Arguments)).FullPath));
        }
    }
}
