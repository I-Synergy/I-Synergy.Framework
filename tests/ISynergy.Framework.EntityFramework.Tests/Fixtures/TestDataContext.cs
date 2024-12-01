using ISynergy.Framework.EntityFramework.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;

internal class TestDataContext : DbContext
{
    public TestDataContext(DbContextOptions<TestDataContext> options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestTenantEntity> TestTenantEntities { get; set; }
    public DbSet<TestTenantEntityWithIgnoreSoftDelete> TestEntitiesWithIgnoreTenant { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
