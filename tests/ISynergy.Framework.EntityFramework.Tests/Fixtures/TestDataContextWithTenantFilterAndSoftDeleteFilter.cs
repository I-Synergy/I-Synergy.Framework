using ISynergy.Framework.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;
internal class TestDataContextWithTenantFilterAndSoftDeleteFilter : TestDataContext
{
    private readonly Func<Guid> _tenantId;

    public TestDataContextWithTenantFilterAndSoftDeleteFilter(DbContextOptions<TestDataContext> options, Func<Guid> tenantId)
        : base(options)
    {
        _tenantId = tenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyTenantFilters(_tenantId);
        modelBuilder.ApplySoftDeleteFilters();
    }
}