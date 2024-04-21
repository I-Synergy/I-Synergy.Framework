using Dotmim.Sync;
using Dotmim.Sync.Enumerations;

namespace ISynergy.Framework.Synchronization.Abstractions;

public interface ISynchronizationService
{
    Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default);
}
