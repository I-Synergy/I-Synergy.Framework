using ISynergy.Framework.EntityFramework.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class TestDataContext : DbContext
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
