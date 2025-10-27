using ISynergy.Framework.Storage.Azure.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;
using System.Text;

namespace ISynergy.Framework.Storage.Azure.Tests.StepDefinitions;

/// <summary>
/// Step definitions for blob operations scenarios.
/// Demonstrates BDD testing for Azure Blob Storage operations.
/// </summary>
[Binding]
public class BlobOperationsSteps
{
    private readonly ILogger<BlobOperationsSteps> _logger;
    private readonly StorageTestContext _context;

    public BlobOperationsSteps(StorageTestContext context)
    {
     var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<BlobOperationsSteps>();
        _context = context;
    }

    [Given(@"the blob storage service is configured")]
    public void GivenTheBlobStorageServiceIsConfigured()
{
        _logger.LogInformation("Configuring blob storage service");
    // In-memory blob storage simulation - no actual Azure dependencies needed for BDD
    }

    [Given(@"I have a test container")]
    public void GivenIHaveATestContainer()
    {
        _logger.LogInformation("Creating test container");
 _context.ContainerName = $"test-container-{Guid.NewGuid()}";
        _context.ContainerExists = true;
    }

    [Given(@"I have a file to upload")]
    public void GivenIHaveAFileToUpload()
    {
      _logger.LogInformation("Preparing file for upload");
        _context.CurrentBlob = new TestBlob
        {
          Name = "test-file.txt",
 Content = Encoding.UTF8.GetBytes("Test file content for BDD scenario"),
         ContentType = "text/plain",
    Metadata = new Dictionary<string, string>
       {
        { "author", "BDD Test" },
     { "purpose", "testing" }
    }
        };
    }

  [Given(@"a blob exists in the container")]
public void GivenABlobExistsInTheContainer()
    {
        _logger.LogInformation("Setting up existing blob");
        var blob = new TestBlob
        {
            Name = "existing-blob.txt",
            Content = Encoding.UTF8.GetBytes("Existing blob content"),
    ContentType = "text/plain",
            Uri = $"https://teststorage.blob.core.windows.net/{_context.ContainerName}/existing-blob.txt",
            CreatedOn = DateTime.UtcNow.AddHours(-1)
 };

        _context.Blobs.Add(blob);
        _context.CurrentBlob = blob;
 _context.BlobExists = true;
        _logger.LogInformation("Blob '{Name}' created in container", blob.Name);
    }

    [Given(@"a blob with metadata exists")]
    public void GivenABlobWithMetadataExists()
    {
        _logger.LogInformation("Setting up blob with metadata");
        var blob = new TestBlob
        {
            Name = "metadata-blob.txt",
          Content = Encoding.UTF8.GetBytes("Blob with metadata"),
  ContentType = "text/plain",
            Metadata = new Dictionary<string, string>
            {
          { "department", "Engineering" },
  { "project", "I-Synergy Framework" },
  { "version", "1.0" }
          },
   Uri = $"https://teststorage.blob.core.windows.net/{_context.ContainerName}/metadata-blob.txt"
        };

        _context.Blobs.Add(blob);
        _context.CurrentBlob = blob;
    }

    [When(@"I upload the file to blob storage")]
    public void WhenIUploadTheFileToBlobStorage()
    {
        _logger.LogInformation("Uploading file to blob storage");
        ArgumentNullException.ThrowIfNull(_context.CurrentBlob);
        ArgumentNullException.ThrowIfNull(_context.ContainerName);

    try
        {
  _context.CurrentBlob.Uri = $"https://teststorage.blob.core.windows.net/{_context.ContainerName}/{_context.CurrentBlob.Name}";
        _context.Blobs.Add(_context.CurrentBlob);
  _context.BlobExists = true;
            _logger.LogInformation("Blob '{Name}' uploaded successfully", _context.CurrentBlob.Name);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error uploading blob");
 _context.CaughtException = ex;
        }
    }

    [When(@"I download the blob")]
    public void WhenIDownloadTheBlob()
    {
        _logger.LogInformation("Downloading blob");
        ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

        try
        {
            _context.DownloadedContent = _context.CurrentBlob.Content;
     _logger.LogInformation("Blob downloaded, size: {Size} bytes", _context.DownloadedContent.Length);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error downloading blob");
            _context.CaughtException = ex;
        }
    }

    [When(@"I delete the blob")]
    public void WhenIDeleteTheBlob()
    {
        _logger.LogInformation("Deleting blob");
        ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

        try
        {
            _context.Blobs.Remove(_context.CurrentBlob);
      _context.BlobExists = false;
      _logger.LogInformation("Blob '{Name}' deleted successfully", _context.CurrentBlob.Name);
    }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob");
       _context.CaughtException = ex;
        }
    }

    [When(@"I check if the blob exists")]
    public void WhenICheckIfTheBlobExists()
    {
        _logger.LogInformation("Checking blob existence");
        ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

        try
   {
        _context.BlobExists = _context.Blobs.Any(b => b.Name == _context.CurrentBlob.Name);
            _logger.LogInformation("Blob exists: {Exists}", _context.BlobExists);
        }
 catch (Exception ex)
        {
       _logger.LogError(ex, "Error checking blob existence");
            _context.CaughtException = ex;
        }
  }

    [When(@"I retrieve the blob metadata")]
    public void WhenIRetrieveTheBlobMetadata()
    {
        _logger.LogInformation("Retrieving blob metadata");
    ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

        try
     {
       _context.RetrievedMetadata = _context.CurrentBlob.Metadata;
            _logger.LogInformation("Retrieved {Count} metadata properties", _context.RetrievedMetadata.Count);
        }
    catch (Exception ex)
        {
   _logger.LogError(ex, "Error retrieving metadata");
   _context.CaughtException = ex;
    }
    }

    [When(@"I generate a SAS token for the blob")]
    public void WhenIGenerateASasTokenForTheBlob()
    {
        _logger.LogInformation("Generating SAS token");
        ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

     try
        {
  // Simulate SAS token generation
            var sasToken = $"?sv=2023-01-01&sr=b&sig={Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..10]}&st={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}&se={DateTime.UtcNow.AddHours(1):yyyy-MM-ddTHH:mm:ssZ}&sp=r";
     _context.GeneratedSasUri = $"{_context.CurrentBlob.Uri}{sasToken}";
            _context.CurrentBlob.SasToken = sasToken;
     _logger.LogInformation("SAS token generated successfully");
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Error generating SAS token");
      _context.CaughtException = ex;
        }
    }

    [Then(@"the file should be uploaded successfully")]
    public void ThenTheFileShouldBeUploadedSuccessfully()
    {
        _logger.LogInformation("Verifying upload success");

   if (_context.CaughtException != null)
        {
  throw new InvalidOperationException($"Expected successful upload but got exception: {_context.CaughtException.Message}");
        }

   if (!_context.BlobExists)
        {
       throw new InvalidOperationException("Expected blob to exist after upload");
        }
    }

    [Then(@"the blob should exist in the container")]
    public void ThenTheBlobShouldExistInTheContainer()
    {
        _logger.LogInformation("Verifying blob existence");

        if (!_context.BlobExists)
        {
  throw new InvalidOperationException("Expected blob to exist in container");
        }
    }

    [Then(@"the blob content should be retrieved successfully")]
    public void ThenTheBlobContentShouldBeRetrievedSuccessfully()
    {
    _logger.LogInformation("Verifying downloaded content");

        if (_context.DownloadedContent == null || _context.DownloadedContent.Length == 0)
        {
   throw new InvalidOperationException("Expected blob content to be downloaded");
        }
    }

    [Then(@"the content should match the uploaded file")]
    public void ThenTheContentShouldMatchTheUploadedFile()
    {
    _logger.LogInformation("Verifying content match");
    ArgumentNullException.ThrowIfNull(_context.CurrentBlob);
        ArgumentNullException.ThrowIfNull(_context.DownloadedContent);

        if (!_context.CurrentBlob.Content.SequenceEqual(_context.DownloadedContent))
    {
            throw new InvalidOperationException("Downloaded content does not match uploaded content");
        }

        _logger.LogInformation("Content verified: {Size} bytes match", _context.DownloadedContent.Length);
    }

 [Then(@"the blob should be removed from storage")]
    public void ThenTheBlobShouldBeRemovedFromStorage()
    {
        _logger.LogInformation("Verifying blob removal");

        if (_context.BlobExists)
        {
            throw new InvalidOperationException("Expected blob to be removed from storage");
     }
    }

    [Then(@"checking for the blob should return false")]
    public void ThenCheckingForTheBlobShouldReturnFalse()
    {
        _logger.LogInformation("Verifying blob does not exist");

  if (_context.BlobExists)
        {
            throw new InvalidOperationException("Expected blob existence check to return false");
        }
    }

    [Then(@"the existence check should return true")]
    public void ThenTheExistenceCheckShouldReturnTrue()
    {
        _logger.LogInformation("Verifying existence check returned true");

 if (!_context.BlobExists)
     {
        throw new InvalidOperationException("Expected existence check to return true");
        }
    }

    [Then(@"checking for a non-existent blob should return false")]
    public void ThenCheckingForANonExistentBlobShouldReturnFalse()
    {
        _logger.LogInformation("Checking for non-existent blob");

        var exists = _context.Blobs.Any(b => b.Name == "non-existent-blob.txt");

        if (exists)
        {
    throw new InvalidOperationException("Expected non-existent blob check to return false");
        }
    }

    [Then(@"the metadata should be returned successfully")]
    public void ThenTheMetadataShouldBeReturnedSuccessfully()
    {
      _logger.LogInformation("Verifying metadata retrieval");

        if (_context.RetrievedMetadata == null || _context.RetrievedMetadata.Count == 0)
        {
            throw new InvalidOperationException("Expected metadata to be retrieved");
        }
    }

    [Then(@"the metadata properties should be accessible")]
    public void ThenTheMetadataPropertiesShouldBeAccessible()
    {
  _logger.LogInformation("Verifying metadata accessibility");
        ArgumentNullException.ThrowIfNull(_context.RetrievedMetadata);

   foreach (var kvp in _context.RetrievedMetadata)
        {
          if (string.IsNullOrEmpty(kvp.Key) || string.IsNullOrEmpty(kvp.Value))
    {
             throw new InvalidOperationException("Metadata contains invalid key-value pair");
      }
        }

        _logger.LogInformation("All {Count} metadata properties are accessible", _context.RetrievedMetadata.Count);
    }

    [Then(@"a valid SAS URI should be returned")]
 public void ThenAValidSasUriShouldBeReturned()
    {
        _logger.LogInformation("Verifying SAS URI");

   if (string.IsNullOrEmpty(_context.GeneratedSasUri))
  {
            throw new InvalidOperationException("Expected SAS URI to be generated");
        }

        if (!_context.GeneratedSasUri.Contains("?"))
        {
            throw new InvalidOperationException("SAS URI should contain query parameters");
     }

        _logger.LogInformation("Valid SAS URI generated");
    }

    [Then(@"the SAS token should have appropriate permissions")]
  public void ThenTheSasTokenShouldHaveAppropriatePermissions()
    {
   _logger.LogInformation("Verifying SAS token permissions");
     ArgumentNullException.ThrowIfNull(_context.CurrentBlob);

if (string.IsNullOrEmpty(_context.CurrentBlob.SasToken))
        {
  throw new InvalidOperationException("Expected SAS token to be present");
        }

        // Verify SAS token contains required parameters
        if (!_context.CurrentBlob.SasToken.Contains("sp="))
  {
throw new InvalidOperationException("SAS token should contain permissions");
        }

        _logger.LogInformation("SAS token has appropriate permissions");
    }
}
