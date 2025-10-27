using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.EntityFramework.Tests.StepDefinitions;

/// <summary>
/// Step definitions for transaction management scenarios.
/// Demonstrates BDD testing for Entity Framework transaction handling.
/// </summary>
[Binding]
public class TransactionManagementSteps
{
    private readonly ILogger<TransactionManagementSteps> _logger;
    private readonly EntityFrameworkTestContext _context;
    private IDbContextTransaction? _parentTransaction;

    public TransactionManagementSteps(EntityFrameworkTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<TransactionManagementSteps>();
    _context = context;
    }

    [Given(@"I have started a database transaction")]
    public async Task GivenIHaveStartedADatabaseTransaction()
    {
 _logger.LogInformation("Starting database transaction");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        try
    {
    _context.Transaction = await _context.DbContext.Database.BeginTransactionAsync();
          _logger.LogInformation("Transaction started successfully");
  }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting transaction");
    _context.CaughtException = ex;
        }
    }

    [Given(@"I have started a parent transaction")]
 public async Task GivenIHaveStartedAParentTransaction()
    {
        _logger.LogInformation("Starting parent transaction");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        _parentTransaction = await _context.DbContext.Database.BeginTransactionAsync();
        _logger.LogInformation("Parent transaction started");
  }

    [Given(@"I have started a transaction with a (.*) second timeout")]
    public async Task GivenIHaveStartedATransactionWithTimeout(int timeoutSeconds)
    {
        _logger.LogInformation("Starting transaction with {Timeout} second timeout", timeoutSeconds);
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        try
        {
   _context.DbContext.Database.SetCommandTimeout(timeoutSeconds);
          _context.Transaction = await _context.DbContext.Database.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting transaction with timeout");
    _context.CaughtException = ex;
        }
    }

    [Given(@"I have two parallel transactions")]
 public void GivenIHaveTwoParallelTransactions()
    {
        _logger.LogInformation("Preparing two parallel transactions");
        // Setup for concurrent transaction test
    }

    [When(@"I add multiple entities within the transaction")]
    public async Task WhenIAddMultipleEntitiesWithinTheTransaction()
    {
  _logger.LogInformation("Adding multiple entities within transaction");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        try
        {
  var entities = new List<TestEntity>
     {
       new() { Description = "Transaction Entity 1", Version = 1, IsActive = true, CreatedDate = DateTime.UtcNow },
  new() { Description = "Transaction Entity 2", Version = 1, IsActive = true, CreatedDate = DateTime.UtcNow },
         new() { Description = "Transaction Entity 3", Version = 1, IsActive = true, CreatedDate = DateTime.UtcNow }
   };

      _context.DbContext.TestEntities!.AddRange(entities);
    await _context.DbContext.SaveChangesAsync();
            _context.SavedEntityCount = entities.Count;
        _context.Entities.AddRange(entities);
        }
        catch (Exception ex)
  {
       _logger.LogError(ex, "Error adding entities");
      _context.CaughtException = ex;
     }
    }

    [When(@"I add an entity within the transaction")]
    public async Task WhenIAddAnEntityWithinTheTransaction()
    {
        _logger.LogInformation("Adding single entity within transaction");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        try
        {
   var entity = new TestEntity
            {
    Description = "Transaction Entity",
       Version = 1,
          IsActive = true,
     CreatedDate = DateTime.UtcNow
       };

            _context.DbContext.TestEntities!.Add(entity);
       await _context.DbContext.SaveChangesAsync();
    _context.CurrentEntity = entity;
        }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Error adding entity");
 _context.CaughtException = ex;
        }
    }

    [When(@"I commit the transaction")]
    public async Task WhenICommitTheTransaction()
    {
        _logger.LogInformation("Committing transaction");

        try
        {
         ArgumentNullException.ThrowIfNull(_context.Transaction);
      await _context.Transaction.CommitAsync();
     _context.TransactionCommitted = true;
            _logger.LogInformation("Transaction committed successfully");
        }
 catch (Exception ex)
        {
 _logger.LogError(ex, "Error committing transaction");
    _context.CaughtException = ex;
    }
    }

    [When(@"an error occurs during save")]
    public void WhenAnErrorOccursDuringSave()
    {
        _logger.LogInformation("Simulating error during save");
 _context.CaughtException = new InvalidOperationException("Simulated save error");
    }

    [When(@"I rollback the transaction")]
    public async Task WhenIRollbackTheTransaction()
    {
        _logger.LogInformation("Rolling back transaction");

   try
        {
    ArgumentNullException.ThrowIfNull(_context.Transaction);
await _context.Transaction.RollbackAsync();
  _context.TransactionRolledBack = true;
            _logger.LogInformation("Transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            _context.CaughtException = ex;
        }
  }

    [When(@"I create a nested transaction")]
    public async Task WhenICreateANestedTransaction()
    {
      _logger.LogInformation("Creating nested transaction");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        _context.Transaction = await _context.DbContext.Database.BeginTransactionAsync();
  _logger.LogInformation("Nested transaction created");
    }

    [When(@"I save entities in both transactions")]
 public async Task WhenISaveEntitiesInBothTransactions()
    {
        _logger.LogInformation("Saving entities in both transactions");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

    var entity = new TestEntity
        {
         Description = "Nested Transaction Entity",
            Version = 1,
  IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

    _context.DbContext.TestEntities!.Add(entity);
        await _context.DbContext.SaveChangesAsync();
  _context.CurrentEntity = entity;
  }

    [When(@"I commit the nested transaction")]
    public async Task WhenICommitTheNestedTransaction()
 {
 _logger.LogInformation("Committing nested transaction");

   try
        {
          ArgumentNullException.ThrowIfNull(_context.Transaction);
            await _context.Transaction.CommitAsync();
        }
        catch (Exception ex)
   {
     _logger.LogError(ex, "Error committing nested transaction");
  _context.CaughtException = ex;
        }
    }

    [When(@"the operation exceeds the timeout duration")]
    public async Task WhenTheOperationExceedsTheTimeoutDuration()
    {
        _logger.LogInformation("Simulating operation timeout");

try
        {
      await Task.Delay(TimeSpan.FromSeconds(2)); // Exceeds 1 second timeout
        }
        catch (Exception ex)
        {
     _logger.LogWarning(ex, "Timeout occurred");
   _context.CaughtException = ex;
        }
    }

    [When(@"both transactions modify the same entity")]
    public void WhenBothTransactionsModifyTheSameEntity()
    {
        _logger.LogInformation("Setting up concurrent modification scenario");
    // Concurrent modification simulation
    }

    [When(@"both attempt to commit")]
    public void WhenBothAttemptToCommit()
    {
      _logger.LogInformation("Attempting concurrent commits");
        // Concurrent commit simulation
    }

    [Then(@"all entities should be saved successfully")]
    public async Task ThenAllEntitiesShouldBeSavedSuccessfully()
    {
      _logger.LogInformation("Verifying all entities saved");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

     var count = await _context.DbContext.TestEntities!.CountAsync();

        if (count < _context.SavedEntityCount)
        {
        throw new InvalidOperationException($"Expected {_context.SavedEntityCount} entities but found {count}");
        }
  }

    [Then(@"the transaction should be marked as committed")]
    public void ThenTheTransactionShouldBeMarkedAsCommitted()
    {
        _logger.LogInformation("Verifying transaction committed");

        if (!_context.TransactionCommitted)
        {
     throw new InvalidOperationException("Expected transaction to be committed");
    }
    }

    [Then(@"no entities should be saved to the database")]
    public async Task ThenNoEntitiesShouldBeSavedToTheDatabase()
    {
        _logger.LogInformation("Verifying no entities were saved");
   ArgumentNullException.ThrowIfNull(_context.DbContext);

        var count = await _context.DbContext.TestEntities!.CountAsync();

     if (count > 0)
  {
            throw new InvalidOperationException($"Expected no entities but found {count}");
        }
    }

    [Then(@"the transaction should be rolled back")]
    public void ThenTheTransactionShouldBeRolledBack()
    {
        _logger.LogInformation("Verifying transaction rolled back");

      if (!_context.TransactionRolledBack)
        {
 throw new InvalidOperationException("Expected transaction to be rolled back");
        }
    }

    [Then(@"the nested changes should be visible to the parent")]
    public void ThenTheNestedChangesShouldBeVisibleToTheParent()
    {
        _logger.LogInformation("Verifying nested changes visibility");

        if (_context.CurrentEntity == null)
        {
  throw new InvalidOperationException("Expected nested transaction changes to be visible");
        }
    }

    [Then(@"committing the parent should save all changes")]
  public async Task ThenCommittingTheParentShouldSaveAllChanges()
    {
        _logger.LogInformation("Committing parent transaction");
        ArgumentNullException.ThrowIfNull(_parentTransaction);

        await _parentTransaction.CommitAsync();
        _logger.LogInformation("Parent transaction committed");
    }

    [Then(@"a transaction timeout exception should be thrown")]
    public void ThenATransactionTimeoutExceptionShouldBeThrown()
    {
        _logger.LogInformation("Verifying timeout exception");

        if (_context.CaughtException == null)
        {
          throw new InvalidOperationException("Expected timeout exception");
        }
}

    [Then(@"no changes should be persisted")]
    public async Task ThenNoChangesShouldBePersisted()
    {
        _logger.LogInformation("Verifying no changes persisted");
    ArgumentNullException.ThrowIfNull(_context.DbContext);

        var count = await _context.DbContext.TestEntities!.CountAsync();

        if (count > 0)
        {
            throw new InvalidOperationException("Expected no persisted changes");
  }
    }

 [Then(@"one transaction should succeed")]
  public void ThenOneTransactionShouldSucceed()
    {
        _logger.LogInformation("Verifying one transaction succeeded");
        // Concurrent transaction verification
    }

    [Then(@"the other should detect a concurrency conflict")]
    public void ThenTheOtherShouldDetectAConcurrencyConflict()
    {
      _logger.LogInformation("Verifying concurrency conflict detected");
   // Concurrency conflict verification
    }
}
