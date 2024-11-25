using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;

internal class TestClassDataContext : DbContext
{
    public TestClassDataContext(DbContextOptions<TestClassDataContext> options)
        :base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}
