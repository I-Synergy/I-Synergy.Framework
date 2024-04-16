using Dotmim.Sync;
using Dotmim.Sync.Enumerations;

namespace ISynergy.Framework.Synchronization.Abstractions;

public interface ISynchronizationService
{
    string SynchronizationFolder { get; }
    string SnapshotFolder { get; }
    string BatchFolder { get; }
    string SynchronizationDatabase { get; }

    Task StartServiceAsync(SyncParameters parameters, CancellationToken cancellationToken = default);
    void StopService();
    Task SynchronizeAsync(SyncType syncType, SyncParameters parameters, CancellationToken cancellationToken = default);
}
