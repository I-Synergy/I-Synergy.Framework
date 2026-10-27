# Local Database (SQLite)
Offline SQLite storage in a MAUI app: entity setup, migrations and repository access. Load this when adding or changing local persistence.


### Database Context
```csharp
// File: Data/LocalDatabase.cs
using SQLite;
using {ApplicationName}.Mobile.Models;

namespace {ApplicationName}.Mobile.Data;

public class LocalDatabase
{
    private SQLiteAsyncConnection? _database;

    public async Task InitializeAsync()
    {
        if (_database is not null)
            return;

        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "{ApplicationName}.db");

        _database = new SQLiteAsyncConnection(databasePath);

        await _database.CreateTableAsync<BudgetLocal>();
        await _database.CreateTableAsync<GoalLocal>();
        await _database.CreateTableAsync<SyncQueue>();
    }

    // Budget operations
    public async Task<List<BudgetLocal>> GetBudgetsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<BudgetLocal>().ToListAsync();
    }

    public async Task<BudgetLocal?> GetBudgetByIdAsync(Guid budgetId)
    {
        await InitializeAsync();
        return await _database!.Table<BudgetLocal>()
            .Where(b => b.BudgetId == budgetId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveBudgetAsync(BudgetLocal budget)
    {
        await InitializeAsync();

        if (budget.Id == 0)
        {
            budget.CreatedDate = DateTimeOffset.UtcNow;
            return await _database!.InsertAsync(budget);
        }
        else
        {
            budget.ChangedDate = DateTimeOffset.UtcNow;
            return await _database!.UpdateAsync(budget);
        }
    }

    public async Task<int> DeleteBudgetAsync(BudgetLocal budget)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(budget);
    }

    // Sync queue operations
    public async Task QueueOperationAsync(string operation, string entityType, Guid entityId)
    {
        await InitializeAsync();

        var queueItem = new SyncQueue
        {
            Operation = operation,
            EntityType = entityType,
            EntityId = entityId,
            QueuedAt = DateTimeOffset.UtcNow
        };

        await _database!.InsertAsync(queueItem);
    }

    public async Task<List<SyncQueue>> GetPendingOperationsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<SyncQueue>()
            .OrderBy(s => s.QueuedAt)
            .ToListAsync();
    }

    public async Task ClearQueueAsync()
    {
        await InitializeAsync();
        await _database!.DeleteAllAsync<SyncQueue>();
    }
}
```

### Local Entity Model
```csharp
// File: Models/BudgetLocal.cs
using SQLite;

namespace {ApplicationName}.Mobile.Models;

[Table("budgets")]
public class BudgetLocal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public Guid BudgetId { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? ChangedDate { get; set; }

    public bool IsSynced { get; set; }
}

[Table("sync_queue")]
public class SyncQueue
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Operation { get; set; } = string.Empty; // Create, Update, Delete

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public DateTimeOffset QueuedAt { get; set; }
}
```

