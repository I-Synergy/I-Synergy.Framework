using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

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
        var entities = await _context.TestTenantEntities.ToListAsync();

        // Assert
        Assert.AreEqual(2, entities.Count);
        Assert.IsFalse(entities.Any(e => e.IsDeleted));
    }

    [TestMethod]
    public async Task IgnoreQueryFilters_ShouldBypassAllFilters()
    {
        // Act
        var entities = await _context.TestTenantEntities
            .IgnoreQueryFilters()
            .ToListAsync();

        // Assert
        Assert.AreEqual(3, entities.Count);
        Assert.IsTrue(entities.Any(e => e.TenantId != _testTenantId));
        Assert.IsTrue(entities.Any(e => e.IsDeleted));
    }
}
