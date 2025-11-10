using Microsoft.EntityFrameworkCore;
using Sample.Api.Entities;

namespace Sample.Api.Data;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}
