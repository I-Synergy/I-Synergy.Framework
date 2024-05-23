using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Synchronization.Options;

namespace ISynergy.Framework.Synchronization.Abstractions;

public interface ISynchronizationService
{
    SyncAgent SynchronizationAgent { get; }
    Uri SynchronizationEndpoint { get; }
    public string SynchronizationFolder { get; }
    public string SnapshotsFolder { get; }
    public string BatchesFolder { get; }
    string OfflineDatabase { get; }
    SynchronizationSettings SynchronizationOptions { get; }
    Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default);
}
