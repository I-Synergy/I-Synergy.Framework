using ISynergy.Framework.EntityFramework.Tests.Entities;
using ISynergy.Framework.EntityFramework.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.EntityFramework.Tests.StepDefinitions;

/// <summary>
/// Step definitions for repository operations scenarios.
/// Demonstrates BDD testing for Entity Framework CRUD operations.
/// </summary>
[Binding]
public class RepositoryOperationsSteps
{
    private readonly ILogger<RepositoryOperationsSteps> _logger;
    private readonly EntityFrameworkTestContext _context;

    public RepositoryOperationsSteps(EntityFrameworkTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<RepositoryOperationsSteps>();
      _context = context;
    }

    [Given(@"the repository is initialized")]
    public void GivenTheRepositoryIsInitialized()
    {
        _logger.LogInformation("Initializing repository");
        // Repository initialization handled in database context setup
    }

    [Given(@"the database context is configured")]
    public void GivenTheDatabaseContextIsConfigured()
    {
        _logger.LogInformation("Configuring in-memory database context");

        var options = new DbContextOptionsBuilder<TestDataContext>()
       .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
        .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
         .Options;

     _context.DbContext = new TestDataContext(options);
        _logger.LogInformation("Database context configured successfully (in-memory, transactions not supported)");
    }

    [Given(@"I have a new test entity")]
  public void GivenIHaveANewTestEntity()
    {
        _logger.LogInformation("Creating new test entity");
        _context.CurrentEntity = new TestEntity
{
            Description = "Test Entity",
      Version = 1,
     IsActive = true,
     CreatedDate = DateTime.UtcNow
        };
    }

    [Given(@"an entity exists in the database")]
  public async Task GivenAnEntityExistsInTheDatabase()
    {
        _logger.LogInformation("Adding entity to database");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        var entity = new TestEntity
        {
      Description = "Existing Entity",
            Version = 1,
            IsActive = true,
          CreatedDate = DateTime.UtcNow
        };

        _context.DbContext.TestEntities!.Add(entity);
        await _context.DbContext.SaveChangesAsync();

        _context.CurrentEntity = entity;
        _logger.LogInformation("Entity added with ID: {Id}", entity.Id);
    }

    [Given(@"multiple entities exist in the database")]
    public async Task GivenMultipleEntitiesExistInTheDatabase()
    {
 _logger.LogInformation("Adding multiple entities to database");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

      var entities = new List<TestEntity>
        {
   new() { Description = "Entity 1", Version = 1, IsActive = true, CreatedDate = DateTime.UtcNow },
      new() { Description = "Entity 2", Version = 1, IsActive = false, CreatedDate = DateTime.UtcNow },
            new() { Description = "Entity 3", Version = 1, IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) }
        };

        _context.DbContext.TestEntities!.AddRange(entities);
        await _context.DbContext.SaveChangesAsync();

        _context.Entities.AddRange(entities);
      _logger.LogInformation("Added {Count} entities", entities.Count);
    }

    [When(@"I add the entity to the repository")]
    public async Task WhenIAddTheEntityToTheRepository()
    {
        _logger.LogInformation("Adding entity to repository");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
    ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

  try
        {
            _context.DbContext.TestEntities!.Add(_context.CurrentEntity);
            await _context.DbContext.SaveChangesAsync();
     _context.SavedEntityCount = 1;
        }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Error adding entity");
         _context.CaughtException = ex;
   }
  }

    [When(@"I query the entity by its ID")]
    public async Task WhenIQueryTheEntityByItsID()
    {
        _logger.LogInformation("Querying entity by ID");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

      try
        {
   var entity = await _context.DbContext.TestEntities!.FindAsync(_context.CurrentEntity.Id);
            _context.CurrentEntity = entity;
        }
     catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying entity");
            _context.CaughtException = ex;
        }
    }

    [When(@"I modify the entity properties")]
    public void WhenIModifyTheEntityProperties()
 {
      _logger.LogInformation("Modifying entity properties");
 ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

      _context.CurrentEntity.Description = "Modified Description";
      _context.CurrentEntity.Version = 2;
        _context.CurrentEntity.ModifiedDate = DateTime.UtcNow;
    }

    [When(@"I save the changes")]
    public async Task WhenISaveTheChanges()
    {
        _logger.LogInformation("Saving changes");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

        try
  {
  await _context.DbContext.SaveChangesAsync();
        }
     catch (Exception ex)
        {
   _logger.LogError(ex, "Error saving changes");
            _context.CaughtException = ex;
        }
    }

    [When(@"I delete the entity from the repository")]
    public async Task WhenIDeleteTheEntityFromTheRepository()
    {
  _logger.LogInformation("Deleting entity");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

        try
        {
    _context.DbContext.TestEntities!.Remove(_context.CurrentEntity);
            await _context.DbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity");
_context.CaughtException = ex;
    }
    }

    [When(@"I query entities with a filter predicate")]
    public async Task WhenIQueryEntitiesWithAFilterPredicate()
    {
   _logger.LogInformation("Querying entities with filter");
        ArgumentNullException.ThrowIfNull(_context.DbContext);

   try
        {
            var results = await _context.DbContext.TestEntities!
    .Where(e => e.IsActive == true)
              .ToListAsync();

            _context.QueryResults = results;
        }
        catch (Exception ex)
      {
            _logger.LogError(ex, "Error querying entities");
   _context.CaughtException = ex;
        }
    }

    [When(@"I check if the entity exists by ID")]
    public async Task WhenICheckIfTheEntityExistsByID()
    {
        _logger.LogInformation("Checking entity existence");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

      try
        {
        var exists = await _context.DbContext.TestEntities!.AnyAsync(e => e.Id == _context.CurrentEntity.Id);
            _logger.LogInformation("Entity exists: {Exists}", exists);
        }
     catch (Exception ex)
        {
         _logger.LogError(ex, "Error checking existence");
            _context.CaughtException = ex;
        }
    }

    [Then(@"the entity should be saved successfully")]
    public void ThenTheEntityShouldBeSavedSuccessfully()
    {
        _logger.LogInformation("Verifying entity was saved");

        if (_context.CaughtException != null)
        {
throw new InvalidOperationException($"Expected successful save but got exception: {_context.CaughtException.Message}");
        }

        if (_context.SavedEntityCount == 0)
        {
    throw new InvalidOperationException("Expected entity to be saved");
        }
    }

  [Then(@"the entity should have a database ID assigned")]
    public void ThenTheEntityShouldHaveADatabaseIDAssigned()
    {
        _logger.LogInformation("Verifying ID assignment");
      ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

        if (_context.CurrentEntity.Id == 0)
 {
            throw new InvalidOperationException("Expected entity to have a database ID");
 }

        _logger.LogInformation("Entity has ID: {Id}", _context.CurrentEntity.Id);
    }

    [Then(@"the entity should be retrieved successfully")]
    public void ThenTheEntityShouldBeRetrievedSuccessfully()
    {
      _logger.LogInformation("Verifying entity retrieval");

        if (_context.CurrentEntity == null)
        {
         throw new InvalidOperationException("Expected entity to be retrieved");
        }
    }

    [Then(@"the entity properties should match the saved values")]
    public void ThenTheEntityPropertiesShouldMatchTheSavedValues()
    {
_logger.LogInformation("Verifying entity properties");
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

        if (string.IsNullOrEmpty(_context.CurrentEntity.Description))
        {
throw new InvalidOperationException("Expected entity to have description");
   }
    }

    [Then(@"the entity should be updated successfully")]
    public void ThenTheEntityShouldBeUpdatedSuccessfully()
    {
        _logger.LogInformation("Verifying update");

        if (_context.CaughtException != null)
  {
            throw new InvalidOperationException($"Expected successful update but got exception: {_context.CaughtException.Message}");
 }
    }

    [Then(@"the modified properties should be persisted")]
    public void ThenTheModifiedPropertiesShouldBePersisted()
    {
 _logger.LogInformation("Verifying persisted changes");
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

        if (_context.CurrentEntity.Description != "Modified Description")
        {
          throw new InvalidOperationException("Expected modified description to be persisted");
        }

        if (_context.CurrentEntity.Version != 2)
        {
          throw new InvalidOperationException("Expected modified version to be persisted");
     }
    }

    [Then(@"the entity should be removed from the database")]
    public void ThenTheEntityShouldBeRemovedFromTheDatabase()
    {
        _logger.LogInformation("Verifying entity removal");

        if (_context.CaughtException != null)
        {
throw new InvalidOperationException($"Expected successful deletion but got exception: {_context.CaughtException.Message}");
        }
    }

    [Then(@"querying for the entity should return null")]
    public async Task ThenQueryingForTheEntityShouldReturnNull()
    {
   _logger.LogInformation("Verifying entity no longer exists");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

     var entity = await _context.DbContext.TestEntities!.FindAsync(_context.CurrentEntity.Id);

        if (entity != null)
     {
            throw new InvalidOperationException("Expected entity to be deleted");
        }
    }

    [Then(@"only matching entities should be returned")]
    public void ThenOnlyMatchingEntitiesShouldBeReturned()
    {
    _logger.LogInformation("Verifying filtered results");

    if (_context.QueryResults.Any(e => !e.IsActive))
  {
   throw new InvalidOperationException("Expected only active entities");
      }
    }

    [Then(@"the result count should match the filter criteria")]
    public void ThenTheResultCountShouldMatchTheFilterCriteria()
 {
        _logger.LogInformation("Verifying result count: {Count}", _context.QueryResults.Count);

    if (_context.QueryResults.Count == 0)
        {
            throw new InvalidOperationException("Expected at least one matching entity");
   }
    }

    [Then(@"the existence check should return true")]
    public async Task ThenTheExistenceCheckShouldReturnTrue()
    {
      _logger.LogInformation("Verifying existence check");
        ArgumentNullException.ThrowIfNull(_context.DbContext);
        ArgumentNullException.ThrowIfNull(_context.CurrentEntity);

        var exists = await _context.DbContext.TestEntities!.AnyAsync(e => e.Id == _context.CurrentEntity.Id);

        if (!exists)
    {
            throw new InvalidOperationException("Expected entity to exist");
        }
    }

    [Then(@"checking for a non-existent ID should return false")]
    public async Task ThenCheckingForANonExistentIDShouldReturnFalse()
    {
        _logger.LogInformation("Verifying non-existent check");
      ArgumentNullException.ThrowIfNull(_context.DbContext);

        var exists = await _context.DbContext.TestEntities!.AnyAsync(e => e.Id == 99999);

    if (exists)
    {
  throw new InvalidOperationException("Expected non-existent ID to return false");
 }
    }
}
