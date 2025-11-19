using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore.Storage;

namespace ISynergy.Framework.EntityFramework.Tests.StepDefinitions;

/// <summary>
/// Shared context for EntityFramework test scenarios.
/// </summary>
public class EntityFrameworkTestContext
{
    public TestDataContext? DbContext { get; set; }
    public List<TestEntity> Entities { get; set; } = new();
    public TestEntity? CurrentEntity { get; set; }
    public List<TestEntity> QueryResults { get; set; } = new();
    public Exception? CaughtException { get; set; }
    public IDbContextTransaction? Transaction { get; set; }
    public bool TransactionCommitted { get; set; }
    public bool TransactionRolledBack { get; set; }
    public int SavedEntityCount { get; set; }
}
