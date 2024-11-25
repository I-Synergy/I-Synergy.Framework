using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

[TestClass]
public class ModelBuilderExtensionsTests
{
    [TestMethod]
    public void ApplyDecimalPrecision_ShouldSetDecimalPrecision()
    {
        var options = new DbContextOptionsBuilder<DbContext>().Options;
        var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>();
        modelBuilder.ApplyDecimalPrecision();

        var entity = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var property = entity.FindProperty(nameof(TestEntity.TestDecimal));

        Assert.AreEqual("decimal(38, 10)", property.GetColumnType());
    }

    [TestMethod]
    public void ApplyTenantFilters_ShouldSetTenantFilter()
    {
        var options = new DbContextOptionsBuilder<DbContext>().Options;
        var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestTenantEntity>();

        var tenantId = Guid.NewGuid();
        modelBuilder.ApplyTenantFilters(() => tenantId);

        var entity = modelBuilder.Model.FindEntityType(typeof(TestTenantEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
    }

    [TestMethod]
    public void ApplyTenantFilters_ShouldNotSetTenantFilter()
    {
        var options = new DbContextOptionsBuilder<DbContext>().Options;
        var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>();

        var tenantId = Guid.NewGuid();
        modelBuilder.ApplyTenantFilters(() => tenantId);

        var entity = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNull(filter);
    }

    [TestMethod]
    public void ApplySoftDeleteFilters_ShouldSetSoftDeleteFilter()
    {
        var options = new DbContextOptionsBuilder<DbContext>().Options;
        var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>();
        modelBuilder.ApplySoftDeleteFilters();

        var entity = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
    }

    [TestMethod]
    public void ApplyVersionFilters_ShouldSetDefaultVersion()
    {
        var options = new DbContextOptionsBuilder<DbContext>().Options;
        var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>();
        modelBuilder.ApplyVersionFilters();

        var entity = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var property = entity.FindProperty(nameof(IClass.Version));

        Assert.AreEqual(1, property.GetDefaultValue());
    }
}
