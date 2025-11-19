using ISynergy.Framework.Storage.Azure.Tests.Fixtures;

namespace ISynergy.Framework.Storage.Azure.Tests.StepDefinitions;

/// <summary>
/// Shared context for Azure Storage test scenarios.
/// Allows state sharing between different step definition classes.
/// </summary>
public class StorageTestContext
{
  public List<TestBlob> Blobs { get; set; } = new();
    public TestBlob? CurrentBlob { get; set; }
    public string? ContainerName { get; set; }
    public List<string> ContainerNames { get; set; } = new();
    public byte[]? DownloadedContent { get; set; }
    public Dictionary<string, string>? RetrievedMetadata { get; set; }
    public string? GeneratedSasUri { get; set; }
    public bool BlobExists { get; set; }
    public bool ContainerExists { get; set; }
    public string? ContainerAccessLevel { get; set; }
    public int BlobCount { get; set; }
    public Exception? CaughtException { get; set; }
}
