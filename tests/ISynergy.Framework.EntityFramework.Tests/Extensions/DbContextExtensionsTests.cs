﻿using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using ISynergy.Framework.EntityFramework.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

[TestClass]
public class DbContextExtensionsTests
{
    private TestDataContext _dbContext;

    public DbContextExtensionsTests()
    {
        var contextOptions = new DbContextOptionsBuilder<TestDataContext>()
           .UseInMemoryDatabase("DbContextExtensionsTests")
           .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;

        _dbContext = new TestDataContext(contextOptions);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _dbContext.AddRange(
            new TestEntity { Id = 1 },
            new TestEntity { Id = 2 },
            new TestEntity { Id = 3 }
        );
        _dbContext.SaveChanges();
    }

    [TestMethod]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        Expression<Func<TestEntity, bool>> predicate = e => e.Id == 1;

        // Act
        var result = await _dbContext.ExistsAsync(predicate, cancellationToken);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task GetItemByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _dbContext.GetItemByIdAsync<TestEntity, int>(1, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public async Task AddItemAsync_ShouldAddEntity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var testRecord = new TestModel
        {
            Id = 4,
            Memo = "Test",
            CreatedBy = "Test"
        };

        // Act
        var result = await _dbContext.AddItemAsync<TestEntity, TestModel>(testRecord, cancellationToken);

        // Assert
        Assert.AreEqual(1, result);
        Assert.AreEqual(4, _dbContext.TestEntities.Count());
    }

    [TestMethod]
    public async Task UpdateItemAsync_ShouldUpdateEntity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var testRecord = new TestModel { Id = 1 };

        // Act
        var result = await _dbContext.UpdateItemAsync<TestEntity, TestModel>(testRecord, cancellationToken);

        // Assert
        Assert.AreEqual(1, result);
        Assert.AreEqual(3, _dbContext.TestEntities.Count());
    }

    [TestMethod]
    public async Task RemoveItemAsync_ShouldRemoveEntity()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var testEntity = new TestEntity { Id = 1 };

        // Act
        var result = await _dbContext.RemoveItemAsync<TestEntity, int>(1, cancellationToken);

        // Assert
        Assert.AreEqual(1, result);
        Assert.AreEqual(2, _dbContext.TestEntities.Count());
    }
}
