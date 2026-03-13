using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Synchronization.Abstractions.Settings;

namespace ISynergy.Framework.Synchronization.Abstractions.Services;

/// <summary>
/// Defines the contract for a synchronization service that manages offline-to-online
/// data synchronization using <c>Dotmim.Sync</c>.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Native AOT / Trimming:</strong> This interface and all implementations in
/// <c>ISynergy.Framework.Synchronization</c> are not compatible with Native AOT or
/// linker-trimmed builds. The underlying <c>Dotmim.Sync</c> framework uses extensive
/// internal reflection for dynamic table schema discovery, runtime type-to-SQL-column
/// mapping, and <c>SyncAgent</c> orchestration. Additionally,
/// <c>DefaultMessagePackSerializer</c> uses <c>ContractlessStandardResolver</c> which
/// requires runtime member reflection.
/// </para>
/// <para>
/// Native AOT support for this library depends on <c>Dotmim.Sync</c> gaining AOT
/// compatibility. Monitor the Dotmim.Sync GitHub repository for progress:
/// https://github.com/Mimetis/Dotmim.Sync
/// </para>
/// </remarks>
public interface ISynchronizationService
{
    bool IsActive { get; }
    SyncAgent? SynchronizationAgent { get; }
    Uri? SynchronizationEndpoint { get; }
    public string SynchronizationFolder { get; }
    public string SnapshotsFolder { get; }
    public string BatchesFolder { get; }
    string OfflineDatabase { get; }
    ISynchronizationSettings? SynchronizationOptions { get; }
    Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default);
}
