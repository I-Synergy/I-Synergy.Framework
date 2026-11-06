using ISynergy.Framework.EntityFramework.Entities;
using ISynergy.Framework.EntityFramework.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EntityFramework.Extensions;

[TestClass]
public class ModelBuilderExtensionsWithSoftDeleteTests
{
    private TestDataContextWithSoftDeleteFilter _context;
    private readonly Guid _testTenantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
    private readonly Guid _otherTenantId = Guid.Parse("87654321-4321-4321-4321-987654321098");

    public ModelBuilderExtensionsWithSoftDeleteTests()
    {
        var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDataContextWithSoftDeleteFilter(options);
        SeedDatabase();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        // Add tenant entities
        _context.TestTenantEntities.AddRange(
            new TestTenantEntity { Id = Guid.NewGuid(), TenantId = _testTenantId, IsDeleted = false },
            new TestTenantEntity { Id = Guid.NewGuid(), TenantId = _testTenantId, IsDeleted = true },
            new TestTenantEntity { Id = Guid.NewGuid(), TenantId = _otherTenantId, IsDeleted = false }
        );

        // Add ignored tenant entities
        _context.TestEntitiesWithIgnoreTenant.AddRange(
            new TestTenantEntityWithIgnoreSoftDelete { Id = Guid.NewGuid(), TenantId = _testTenantId, IsDeleted = false },
            new TestTenantEntityWithIgnoreSoftDelete { Id = Guid.NewGuid(), TenantId = _otherTenantId, IsDeleted = false }
        );

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task SoftDeleteFilter_ShouldOnlyReturnNonDeletedEntities()
    {
        // Act
        var entities = await _context.TestTenantEntities.ToListAsync(TestContext.CancellationToken);

        // Assert
        Assert.HasCount(2, entities);
        Assert.DoesNotContain(e => e.IsDeleted, entities);
    }

    [TestMethod]
    public async Task IgnoreQueryFilters_ShouldBypassAllFilters()
    {
        // Act
        var entities = await _context.TestTenantEntities
            .IgnoreQueryFilters()
            .ToListAsync(TestContext.CancellationToken);

        // Assert
        Assert.HasCount(3, entities);
        Assert.Contains(e => e.TenantId != _testTenantId, entities);
        Assert.Contains(e => e.IsDeleted, entities);
    }

    public TestContext TestContext { get; set; }
}
