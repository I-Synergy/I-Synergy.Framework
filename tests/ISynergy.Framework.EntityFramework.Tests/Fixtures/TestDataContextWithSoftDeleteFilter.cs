using ISynergy.Framework.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;

internal class TestDataContextWithSoftDeleteFilter : TestDataContext
{
    public TestDataContextWithSoftDeleteFilter(DbContextOptions<TestDataContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplySoftDeleteFilters();
    }
}
