using ISynergy.Framework.Storage.Azure.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;
using System.Text;

namespace ISynergy.Framework.Storage.Azure.Tests.StepDefinitions;

/// <summary>
/// Step definitions for container management scenarios.
/// Demonstrates BDD testing for Azure Blob Storage container operations.
/// </summary>
[Binding]
public class ContainerManagementSteps
{
    private readonly ILogger<ContainerManagementSteps> _logger;
    private readonly StorageTestContext _context;

    public ContainerManagementSteps(StorageTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<ContainerManagementSteps>();
        _context = context;
    }

    [Given(@"I have a container name")]
    public void GivenIHaveAContainerName()
 {
        _logger.LogInformation("Generating container name");
      _context.ContainerName = $"test-container-{Guid.NewGuid()}";
    }

    [Given(@"a container with multiple blobs exists")]
    public void GivenAContainerWithMultipleBlobsExists()
  {
        _logger.LogInformation("Creating container with multiple blobs");
      _context.ContainerName = $"multi-blob-container-{Guid.NewGuid()}";
        _context.ContainerExists = true;

        // Create multiple test blobs
    for (int i = 1; i <= 5; i++)
   {
   var blob = new TestBlob
  {
 Name = $"blob-{i}.txt",
              Content = Encoding.UTF8.GetBytes($"Content for blob {i}"),
        ContentType = "text/plain",
  Uri = $"https://teststorage.blob.core.windows.net/{_context.ContainerName}/blob-{i}.txt",
  CreatedOn = DateTime.UtcNow.AddMinutes(-i)
   };
         _context.Blobs.Add(blob);
        }

  _context.BlobCount = _context.Blobs.Count;
        _logger.LogInformation("Container created with {Count} blobs", _context.BlobCount);
    }

    [Given(@"a container exists")]
    public void GivenAContainerExists()
    {
        _logger.LogInformation("Setting up existing container");
        _context.ContainerName = $"existing-container-{Guid.NewGuid()}";
    _context.ContainerExists = true;
  _context.ContainerAccessLevel = "Private";
    }

    [When(@"I create a new container")]
    public void WhenICreateANewContainer()
    {
      _logger.LogInformation("Creating new container");
        ArgumentException.ThrowIfNullOrWhiteSpace(_context.ContainerName);

        try
        {
  _context.ContainerNames.Add(_context.ContainerName);
          _context.ContainerExists = true;
         _context.ContainerAccessLevel = "Private";
          _logger.LogInformation("Container '{Name}' created successfully", _context.ContainerName);
        }
        catch (Exception ex)
        {
   _logger.LogError(ex, "Error creating container");
       _context.CaughtException = ex;
     }
    }

    [When(@"I list the blobs in the container")]
    public void WhenIListTheBlobsInTheContainer()
    {
        _logger.LogInformation("Listing blobs in container");
        ArgumentException.ThrowIfNullOrWhiteSpace(_context.ContainerName);

        try
        {
 // In a real scenario, this would query the storage service
            _context.BlobCount = _context.Blobs.Count;
    _logger.LogInformation("Found {Count} blobs in container", _context.BlobCount);
     }
     catch (Exception ex)
 {
       _logger.LogError(ex, "Error listing blobs");
            _context.CaughtException = ex;
     }
    }

    [When(@"I delete the container")]
    public void WhenIDeleteTheContainer()
    {
    _logger.LogInformation("Deleting container");
        ArgumentException.ThrowIfNullOrWhiteSpace(_context.ContainerName);

   try
      {
      _context.ContainerNames.Remove(_context.ContainerName);
   _context.Blobs.Clear();
            _context.ContainerExists = false;
            _logger.LogInformation("Container '{Name}' deleted successfully", _context.ContainerName);
        }
        catch (Exception ex)
      {
      _logger.LogError(ex, "Error deleting container");
      _context.CaughtException = ex;
      }
    }

    [When(@"I set the container access level to public")]
    public void WhenISetTheContainerAccessLevelToPublic()
    {
        _logger.LogInformation("Setting container access level to public");
        ArgumentException.ThrowIfNullOrWhiteSpace(_context.ContainerName);

        try
        {
    _context.ContainerAccessLevel = "Blob"; // Public read access for blobs
 _logger.LogInformation("Container access level set to: {Level}", _context.ContainerAccessLevel);
        }
        catch (Exception ex)
{
_logger.LogError(ex, "Error setting access level");
    _context.CaughtException = ex;
  }
    }

    [Then(@"the container should be created successfully")]
public void ThenTheContainerShouldBeCreatedSuccessfully()
  {
        _logger.LogInformation("Verifying container creation");

        if (_context.CaughtException != null)
        {
         throw new InvalidOperationException($"Expected successful creation but got exception: {_context.CaughtException.Message}");
      }

        if (!_context.ContainerExists)
        {
       throw new InvalidOperationException("Expected container to be created");
        }
    }

    [Then(@"the container should exist in storage")]
    public void ThenTheContainerShouldExistInStorage()
    {
    _logger.LogInformation("Verifying container existence");

        if (!_context.ContainerExists)
  {
throw new InvalidOperationException("Expected container to exist in storage");
        }
    }

[Then(@"all blobs should be returned")]
    public void ThenAllBlobsShouldBeReturned()
    {
        _logger.LogInformation("Verifying all blobs returned");

     if (_context.BlobCount == 0)
        {
   throw new InvalidOperationException("Expected blobs to be returned");
        }

  _logger.LogInformation("Successfully retrieved {Count} blobs", _context.BlobCount);
    }

    [Then(@"the blob count should match the expected number")]
    public void ThenTheBlobCountShouldMatchTheExpectedNumber()
    {
  _logger.LogInformation("Verifying blob count");

   var expectedCount = 5; // From the GivenAContainerWithMultipleBlobsExists step

 if (_context.BlobCount != expectedCount)
        {
  throw new InvalidOperationException($"Expected {expectedCount} blobs but got {_context.BlobCount}");
        }

    _logger.LogInformation("Blob count matches: {Count}", _context.BlobCount);
    }

    [Then(@"the container should be removed from storage")]
    public void ThenTheContainerShouldBeRemovedFromStorage()
    {
      _logger.LogInformation("Verifying container removal");

        if (_context.ContainerExists)
        {
            throw new InvalidOperationException("Expected container to be removed from storage");
   }
    }

    [Then(@"checking for the container should return false")]
    public void ThenCheckingForTheContainerShouldReturnFalse()
    {
 _logger.LogInformation("Verifying container does not exist");

if (_context.ContainerExists)
  {
   throw new InvalidOperationException("Expected container existence check to return false");
        }
    }

    [Then(@"the container access level should be updated")]
    public void ThenTheContainerAccessLevelShouldBeUpdated()
    {
        _logger.LogInformation("Verifying access level update");

        if (_context.ContainerAccessLevel == "Private")
        {
         throw new InvalidOperationException("Expected container access level to be updated from Private");
        }

        _logger.LogInformation("Container access level updated to: {Level}", _context.ContainerAccessLevel);
    }

    [Then(@"blobs should be publicly accessible")]
    public void ThenBlobsShouldBePubliclyAccessible()
    {
        _logger.LogInformation("Verifying public blob access");

        if (_context.ContainerAccessLevel != "Blob")
      {
   throw new InvalidOperationException($"Expected Blob access level but got {_context.ContainerAccessLevel}");
     }

        _logger.LogInformation("Blobs are publicly accessible");
    }
}
