using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.EntityFramework.Tests.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

[TestClass]
public class ModelBuilderExtensionsTests
{
    private DbContextOptions<DbContext> _options;
    private DbContext _context;
    private ModelBuilder _modelBuilder;
    private readonly Guid _testTenantId = Guid.Parse("12345678-1234-1234-1234-123456789012");

    [TestInitialize]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        _context = new DbContext(_options);
        _modelBuilder = new ModelBuilder();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void ApplyDecimalPrecision_WithDefaultPrecision_ShouldSetCorrectPrecision()
    {
        // Arrange
        _modelBuilder.Entity<TestEntity>();

        // Act
        _modelBuilder.ApplyDecimalPrecision();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var property = entity.FindProperty(nameof(TestEntity.TestDecimal));
        Assert.AreEqual("decimal(38, 10)", property.GetColumnType());
    }

    [TestMethod]
    public void ApplyDecimalPrecision_WithCustomPrecision_ShouldSetCustomPrecision()
    {
        // Arrange
        _modelBuilder.Entity<TestEntity>();
        var customPrecision = "decimal(20, 4)";

        // Act
        _modelBuilder.ApplyDecimalPrecision(customPrecision);

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var property = entity.FindProperty(nameof(TestEntity.TestDecimal));
        Assert.AreEqual(customPrecision, property.GetColumnType());
    }

    [TestMethod]
    public void ApplyTenantFilters_WhenEntityImplementsITenantEntity_ShouldSetFilter()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntity>();

        // Act
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        Assert.IsInstanceOfType(filter, typeof(LambdaExpression));

        // Validate filter structure
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Parameters.Count == 1);
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.Equal);
    }

    [TestMethod]
    public void ApplyTenantFilters_WhenEntityDoesNotImplementITenantEntity_ShouldNotSetFilter()
    {
        // Arrange
        _modelBuilder.Entity<TestEntity>();

        // Act
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var filter = entity.GetQueryFilter();
        Assert.IsNull(filter);
    }

    [TestMethod]
    public void ApplySoftDeleteFilters_WhenEntityImplementsIEntity_ShouldSetFilter()
    {
        // Arrange
        _modelBuilder.Entity<TestEntity>();

        // Act
        _modelBuilder.ApplySoftDeleteFilters();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        Assert.IsInstanceOfType(filter, typeof(LambdaExpression));

        // Validate filter structure
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Parameters.Count == 1);
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.Not);
    }

    [TestMethod]
    public void ApplyVersioning_WhenEntityImplementsIClass_ShouldSetDefaultVersion()
    {
        // Arrange
        _modelBuilder.Entity<TestEntity>();

        // Act
        _modelBuilder.ApplyVersioning();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestEntity));
        var property = entity.FindProperty(nameof(IClass.Version));

        Assert.IsNotNull(property);
        Assert.AreEqual(1, property.GetDefaultValue());
    }

    [TestMethod]
    public void ApplyTenantAndSoftDeleteFilters_WhenBothApplied_ShouldCombineFilters()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntity>();

        // Act
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);
        _modelBuilder.ApplySoftDeleteFilters();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        Assert.IsInstanceOfType(filter, typeof(LambdaExpression));

        // Validate combined filter structure
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Parameters.Count == 1);
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.AndAlso);
    }

    [TestMethod]
    public void CombineQueryFilters_WithNoExpressions_ShouldReturnTrueExpression()
    {
        // Arrange
        var entityType = typeof(TestEntity);
        var expressions = Array.Empty<LambdaExpression>();

        // Act
        var result = ModelBuilderExtensions.CombineQueryFilters(entityType, expressions);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Body.NodeType == ExpressionType.Constant);
        Assert.AreEqual(true, ((ConstantExpression)result.Body).Value);
    }

    [TestMethod]
    public void CombineQueryFilters_WithSingleExpression_ShouldReturnSameExpression()
    {
        // Arrange
        var entityType = typeof(TestEntity);
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(IEntity.IsDeleted));
        var expression = Expression.Lambda(Expression.Not(property), parameter);

        // Act
        var result = ModelBuilderExtensions.CombineQueryFilters(entityType, new[] { expression });

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Body.NodeType == ExpressionType.Not);
    }

    [TestMethod]
    public void ApplyTenantAndSoftDeleteFilters_WhenAppliedInOrder_ShouldCombineFiltersCorrectly()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntity>();

        // Act - Apply tenant filter first, then soft delete
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);
        _modelBuilder.ApplySoftDeleteFilters();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        Assert.IsInstanceOfType(filter, typeof(LambdaExpression));

        // Validate combined filter structure
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Parameters.Count == 1);
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.AndAlso);

        // Check both parts of the combined filter
        var andAlso = (BinaryExpression)lambda.Body;
        Assert.IsTrue(andAlso.Left.NodeType == ExpressionType.Equal); // Tenant filter
        Assert.IsTrue(andAlso.Right.NodeType == ExpressionType.Not);  // Soft delete filter
    }

    [TestMethod]
    public void ApplySoftDeleteAndTenantFilters_WhenAppliedInReverseOrder_ShouldCombineFiltersCorrectly()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntity>();

        // Act - Apply soft delete first, then tenant filter
        _modelBuilder.ApplySoftDeleteFilters();
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntity));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        Assert.IsInstanceOfType(filter, typeof(LambdaExpression));

        // Validate combined filter structure
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Parameters.Count == 1);
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.AndAlso);

        // Check both parts of the combined filter
        var andAlso = (BinaryExpression)lambda.Body;
        Assert.IsTrue(andAlso.Left.NodeType == ExpressionType.Not);    // Soft delete filter
        Assert.IsTrue(andAlso.Right.NodeType == ExpressionType.Equal); // Tenant filter
    }

    [TestMethod]
    public void ApplySoftDeleteFilters_WhenEntityHasIgnoreAttribute_ShouldNotSetFilter()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntityWithIgnoreSoftDelete>();

        // Act
        _modelBuilder.ApplySoftDeleteFilters();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntityWithIgnoreSoftDelete));
        var filter = entity.GetQueryFilter();
        Assert.IsNull(filter);
    }


    [TestMethod]
    public void ApplyBothFilters_WhenEntityHasIgnoreSoftDelete_ShouldOnlySetTenantFilter()
    {
        // Arrange
        _modelBuilder.Entity<TestTenantEntityWithIgnoreSoftDelete>();

        // Act
        _modelBuilder.ApplyTenantFilters(() => _testTenantId);
        _modelBuilder.ApplySoftDeleteFilters();

        // Assert
        var entity = _modelBuilder.Model.FindEntityType(typeof(TestTenantEntityWithIgnoreSoftDelete));
        var filter = entity.GetQueryFilter();

        Assert.IsNotNull(filter);
        var lambda = (LambdaExpression)filter;
        Assert.IsTrue(lambda.Body.NodeType == ExpressionType.Equal); // Only tenant filter
    }
}
