# Data Synchronization (Dotmim.Sync)
Offline-to-server sync with Dotmim.Sync: setup, conflict handling and sync triggers. Load this when implementing or debugging data synchronization.

### Sync Service
```csharp
// File: Services/SyncService.cs
using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;

namespace {ApplicationName}.Mobile.Services;

public interface ISyncService
{
    Task<bool> SyncDataAsync();
    bool IsOnline { get; }
}

public class SyncService(
    IConnectivity connectivity,
    ILogger<SyncService> logger
) : ISyncService
{
    public bool IsOnline => connectivity.NetworkAccess == NetworkAccess.Internet;

    public async Task<bool> SyncDataAsync()
    {
        if (!IsOnline)
        {
            logger.LogWarning("Cannot sync: offline");
            return false;
        }

        try
        {
            logger.LogInformation("Starting data synchronization");

            var serverOrchestrator = new WebRemoteOrchestrator("https://your-api.com/sync");

            var clientProvider = new SqliteSyncProvider(
                Path.Combine(FileSystem.AppDataDirectory, "{ApplicationName}.db"));

            var agent = new SyncAgent(clientProvider, serverOrchestrator);

            var tables = new string[] { "budgets", "goals", "debts" };
            var setup = new SyncSetup(tables);

            var result = await agent.SynchronizeAsync(setup);

            logger.LogInformation(
                "Sync completed: {TotalChangesDownloaded} downloaded, {TotalChangesUploaded} uploaded",
                result.TotalChangesDownloaded, result.TotalChangesUploaded);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during synchronization");
            return false;
        }
    }
}
```

