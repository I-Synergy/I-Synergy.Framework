using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Synchronization.Abstractions.Settings;

namespace ISynergy.Framework.Synchronization.Abstractions.Services;

public interface ISynchronizationService
{
    bool IsActive { get; }
    SyncAgent SynchronizationAgent { get; }
    Uri SynchronizationEndpoint { get; }
    public string SynchronizationFolder { get; }
    public string SnapshotsFolder { get; }
    public string BatchesFolder { get; }
    string OfflineDatabase { get; }
    ISynchronizationSettings SynchronizationOptions { get; }
    Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default);
}
