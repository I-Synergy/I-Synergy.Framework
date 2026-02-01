using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

[TestClass]
public class ModelBuilderExtensionsWithTenantTests
{
    private TestDataContextWithTenantFilter _context;
    private readonly Guid _testTenantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
    private readonly Guid _otherTenantId = Guid.Parse("87654321-4321-4321-4321-987654321098");

    public ModelBuilderExtensionsWithTenantTests()
    {
        var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDataContextWithTenantFilter(options, () => _testTenantId);
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
    public async Task TenantFilter_ShouldOnlyReturnCurrentTenantEntities()
    {
        // Act
        var entities = await _context.TestTenantEntities.ToListAsync();

        // Assert
        Assert.AreEqual(2, entities.Count);
        Assert.IsTrue(entities.All(e => e.TenantId == _testTenantId));
    }
}
