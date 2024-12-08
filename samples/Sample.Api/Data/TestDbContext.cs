using Microsoft.EntityFrameworkCore;
using Sample.Api.Entities;

namespace Sample.Api.Data;
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}
