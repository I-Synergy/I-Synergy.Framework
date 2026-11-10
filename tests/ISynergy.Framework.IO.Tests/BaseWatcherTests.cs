using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.IO.Events;
using ISynergy.Framework.IO.Models;
using ISynergy.Framework.IO.Watchers.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ISynergy.Framework.IO;

/// <summary>
/// Tests for the extracted watcher-creation logic in <see cref="BaseWatcher{TWatcher, TWatcherEventArgs}"/>.
/// </summary>
[TestClass]
public class BaseWatcherTests
{
    private static readonly NotifyFilters[] AllChangeFilters =
    [
        NotifyFilters.Attributes,
        NotifyFilters.CreationTime,
        NotifyFilters.DirectoryName,
        NotifyFilters.FileName,
        NotifyFilters.LastAccess,
        NotifyFilters.LastWrite,
        NotifyFilters.Security,
        NotifyFilters.Size
    ];

    private string _tempDirectory = string.Empty;

    [TestInitialize]
    public void Initialize()
    {
        _tempDirectory = Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), $"BaseWatcherTests_{Guid.NewGuid():N}")).FullName;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, recursive: true);
    }

    /// <summary>
    /// A concrete, inspectable watcher that exposes the protected members of <see cref="BaseWatcher{TWatcher, TWatcherEventArgs}"/>.
    /// </summary>
    private sealed class TestableWatcher : BaseWatcher<FileSystemWatcher, WatcherEventArgs>
    {
        public TestableWatcher(WatcherInfo info)
            : base(info)
        {
        }

        public IReadOnlyList<FileSystemWatcher> Watchers => _watchers;

        public bool HandleNotify(NotifyFilters filter) => HandleNotifyFilter(filter);

        public bool HandleWatches(WatcherChangeTypes filter) => HandleWatchesFilter(filter);
    }

    private WatcherInfo CreateInfo(
        NotifyFilters changesFilters,
        WatcherChangeTypes watchesFilters = 0,
        bool watchForError = false,
        bool watchForDisposed = false,
        bool includeSubFolders = false,
        string fileFilter = "",
        uint bufferKBytes = 8) =>
        new()
        {
            WatchPath = _tempDirectory,
            ChangesFilters = changesFilters,
            WatchesFilters = watchesFilters,
            WatchForError = watchForError,
            WatchForDisposed = watchForDisposed,
            IncludeSubFolders = includeSubFolders,
            FileFilter = fileFilter,
            BufferKBytes = bufferKBytes
        };

    [TestMethod]
    public void Constructor_WithNullInfo_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TestableWatcher(null!));
    }

    [TestMethod]
    public void Initialize_WatchesFilterRequested_CreatesMainWatchersForEverySlot()
    {
        // Whenever any Created/Deleted/Renamed/error/disposed watcher is requested, the first
        // ("main") watcher is created and each of the eight change-filter slots that is not an
        // enabled change filter falls through to the main-watcher branch. The resulting collection
        // is therefore deterministic: 1 initial main watcher + 8 slots.
        using var watcher = new TestableWatcher(CreateInfo(0, WatcherChangeTypes.Created));

        Assert.HasCount(1 + AllChangeFilters.Length, watcher.Watchers);
        // The main watcher must not have a NotifyFilter applied (filter is null on that path).
        Assert.AreEqual(new FileSystemWatcher().NotifyFilter, watcher.Watchers[0].NotifyFilter);
    }

    [TestMethod]
    public void Initialize_WhenNothingRequested_CreatesNoWatchers()
    {
        // No change filters, no watches filters, no error/disposed -> no watchers at all.
        using var watcher = new TestableWatcher(CreateInfo(0));

        Assert.IsEmpty(watcher.Watchers);
    }

    [TestMethod]
    public void Initialize_DoesNotCreateMainWatcherWhenNoEventsRequested()
    {
        // One change filter, but no watches filters and no error/disposed -> only the change watcher.
        using var watcher = new TestableWatcher(CreateInfo(NotifyFilters.Attributes));

        Assert.HasCount(1, watcher.Watchers);
        Assert.AreEqual(NotifyFilters.Attributes, watcher.Watchers[0].NotifyFilter);
    }

    [TestMethod]
    public void Initialize_CreatesOneWatcherPerChangeFilter()
    {
        // All change filters, no watches filters -> exactly one watcher per change filter.
        using var watcher = new TestableWatcher(CreateInfo(
            NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
            NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
            NotifyFilters.Security | NotifyFilters.Size));

        Assert.HasCount(AllChangeFilters.Length, watcher.Watchers);

        for (var i = 0; i < AllChangeFilters.Length; i++)
        {
            Assert.AreEqual(AllChangeFilters[i], watcher.Watchers[i].NotifyFilter);
        }
    }

    [TestMethod]
    public void Initialize_CreatesMainWatcherPlusEveryChangeWatcher()
    {
        using var watcher = new TestableWatcher(CreateInfo(
            NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
            NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
            NotifyFilters.Security | NotifyFilters.Size,
            WatcherChangeTypes.All));

        // 1 main watcher + 8 change watchers.
        Assert.HasCount(1 + AllChangeFilters.Length, watcher.Watchers);
    }

    [TestMethod]
    public void CreateWatcherInstance_AppliesCommonSettings()
    {
        using var watcher = new TestableWatcher(CreateInfo(
            NotifyFilters.FileName,
            includeSubFolders: true,
            fileFilter: "*.txt",
            bufferKBytes: 16));

        var changeWatcher = watcher.Watchers[0];

        Assert.IsTrue(changeWatcher.IncludeSubdirectories);
        Assert.AreEqual("*.txt", changeWatcher.Filter);
        Assert.AreEqual(16 * 1024, changeWatcher.InternalBufferSize);
    }

    [TestMethod]
    public void Initialize_ClampsBufferToMinimumFourKBytes()
    {
        using var watcher = new TestableWatcher(CreateInfo(NotifyFilters.Attributes, bufferKBytes: 1));

        Assert.AreEqual(4 * 1024, watcher.Watchers[0].InternalBufferSize);
    }

    [TestMethod]
    public void Initialize_ClampsBufferToMaximumSixtyFourKBytes()
    {
        using var watcher = new TestableWatcher(CreateInfo(NotifyFilters.Attributes, bufferKBytes: 200));

        Assert.AreEqual(64 * 1024, watcher.Watchers[0].InternalBufferSize);
    }

    [TestMethod]
    public void StartAndStop_ToggleEnableRaisingEvents()
    {
        using var watcher = new TestableWatcher(CreateInfo(
            NotifyFilters.Attributes,
            WatcherChangeTypes.Created));

        watcher.Start();
        Assert.IsTrue(watcher.Watchers.All(w => w.EnableRaisingEvents));

        watcher.Stop();
        Assert.IsTrue(watcher.Watchers.All(w => !w.EnableRaisingEvents));
    }

    [TestMethod]
    public void HandleNotifyFilter_ReturnsTrueOnlyWhenFilterEnabled()
    {
        using var watcher = new TestableWatcher(CreateInfo(
            NotifyFilters.Attributes | NotifyFilters.Size));

        Assert.IsTrue(watcher.HandleNotify(NotifyFilters.Attributes));
        Assert.IsTrue(watcher.HandleNotify(NotifyFilters.Attributes | NotifyFilters.Size));
        Assert.IsFalse(watcher.HandleNotify(NotifyFilters.FileName));
    }

    [TestMethod]
    public void HandleWatchesFilter_ReturnsTrueOnlyWhenFilterEnabled()
    {
        using var watcher = new TestableWatcher(CreateInfo(
            0,
            WatcherChangeTypes.Created | WatcherChangeTypes.Deleted));

        Assert.IsTrue(watcher.HandleWatches(WatcherChangeTypes.Created));
        Assert.IsTrue(watcher.HandleWatches(WatcherChangeTypes.Deleted));
        Assert.IsFalse(watcher.HandleWatches(WatcherChangeTypes.Renamed));
    }

    [TestMethod]
    public void DisposeWatchers_ClearsWatcherCollection()
    {
        var watcher = new TestableWatcher(CreateInfo(NotifyFilters.Attributes));
        Assert.IsNotEmpty(watcher.Watchers);

        watcher.DisposeWatchers();

        Assert.IsEmpty(watcher.Watchers);
        watcher.Dispose();
    }

    [TestMethod]
    public void EventCreated_IsRaisedWhenFileIsCreated()
    {
        using var watcher = new TestableWatcher(CreateInfo(0, WatcherChangeTypes.Created));

        using var raised = new ManualResetEventSlim();
        WatcherEventArgs? captured = null;
        watcher.EventCreated += (_, e) =>
        {
            captured = e;
            raised.Set();
        };

        watcher.Start();

        var filePath = Path.Combine(_tempDirectory, "created.txt");
        File.WriteAllText(filePath, "content");

        Assert.IsTrue(raised.Wait(TimeSpan.FromSeconds(5)), "EventCreated was not raised.");
        Assert.IsNotNull(captured);
        Assert.AreEqual(FileWatcherArgumentTypes.FileSystem, captured!.ArgumentType);
    }

    [TestMethod]
    public void EventDeleted_IsRaisedWhenFileIsDeleted()
    {
        var filePath = Path.Combine(_tempDirectory, "deleted.txt");
        File.WriteAllText(filePath, "content");

        using var watcher = new TestableWatcher(CreateInfo(0, WatcherChangeTypes.Deleted));

        using var raised = new ManualResetEventSlim();
        watcher.EventDeleted += (_, _) => raised.Set();

        watcher.Start();

        File.Delete(filePath);

        Assert.IsTrue(raised.Wait(TimeSpan.FromSeconds(5)), "EventDeleted was not raised.");
    }

    [TestMethod]
    public void EventRenamed_IsRaisedWhenFileIsRenamed()
    {
        var sourcePath = Path.Combine(_tempDirectory, "source.txt");
        File.WriteAllText(sourcePath, "content");

        using var watcher = new TestableWatcher(CreateInfo(0, WatcherChangeTypes.Renamed));

        using var raised = new ManualResetEventSlim();
        watcher.EventRenamed += (_, e) =>
        {
            Assert.AreEqual(FileWatcherArgumentTypes.Renamed, e.ArgumentType);
            raised.Set();
        };

        watcher.Start();

        File.Move(sourcePath, Path.Combine(_tempDirectory, "renamed.txt"));

        Assert.IsTrue(raised.Wait(TimeSpan.FromSeconds(5)), "EventRenamed was not raised.");
    }

    [TestMethod]
    public void EventChangedLastWrite_IsRaisedWhenFileIsModified()
    {
        var filePath = Path.Combine(_tempDirectory, "modified.txt");
        File.WriteAllText(filePath, "content");

        // Only the LastWrite change handler should be attached for this configuration.
        using var watcher = new TestableWatcher(CreateInfo(NotifyFilters.LastWrite));

        using var raised = new ManualResetEventSlim();
        watcher.EventChangedLastWrite += (_, e) =>
        {
            Assert.AreEqual(NotifyFilters.LastWrite, e.Filter);
            raised.Set();
        };

        watcher.Start();

        // File system change notifications can be coalesced/delayed; retry a few times within the window.
        var stopwatch = Stopwatch.StartNew();
        var attempt = 0;
        while (!raised.IsSet && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
        {
            File.AppendAllText(filePath, $"edit-{attempt++}");
            raised.Wait(TimeSpan.FromMilliseconds(200));
        }

        Assert.IsTrue(raised.IsSet, "EventChangedLastWrite was not raised.");
    }
}
