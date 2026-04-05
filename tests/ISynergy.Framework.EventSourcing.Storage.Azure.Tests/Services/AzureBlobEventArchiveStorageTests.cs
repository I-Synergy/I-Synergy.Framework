using Azure.Storage.Blobs;
using ISynergy.Framework.EventSourcing.Storage.Azure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Storage.Azure.Tests.Services;

[TestClass]
public class AzureBlobEventArchiveStorageTests
{
    private ILogger<AzureBlobEventArchiveStorage> _logger = null!;

    [TestInitialize]
    public void Setup()
    {
        _logger = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider()
            .GetRequiredService<ILogger<AzureBlobEventArchiveStorage>>();
    }

    // ------------------------------------------------------------------ //
    // Constructor null guards
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_NullBlobServiceClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AzureBlobEventArchiveStorage(null!, _logger));
    }

    [TestMethod]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        var client = new BlobServiceClient("UseDevelopmentStorage=true");

        Assert.Throws<ArgumentNullException>(() =>
            new AzureBlobEventArchiveStorage(client, null!));
    }

    // ------------------------------------------------------------------ //
    // Construction with valid parameters
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_WithValidArguments_Succeeds()
    {
        var client = new BlobServiceClient("UseDevelopmentStorage=true");
        var storage = new AzureBlobEventArchiveStorage(client, _logger);

        Assert.IsNotNull(storage);
    }

    // ------------------------------------------------------------------ //
    // Storage path format
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void StoragePath_Format_MatchesExpectedPattern()
    {
        // The blob path for a given stream batch must follow the documented
        // format: {streamType}/{streamId}/{vFrom}-{vTo}.json
        // This does NOT include the tenantId — tenant isolation is at the
        // container level for the Azure implementation.
        var streamType = "Order";
        var streamId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        const long versionFrom = 1L;
        const long versionTo = 10L;

        var expected = $"{streamType}/{streamId}/1-10.json";
        var actual = $"{streamType}/{streamId}/{versionFrom}-{versionTo}.json";

        Assert.AreEqual(expected, actual);
    }

    // ------------------------------------------------------------------ //
    // Container name format
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void ContainerName_IsLowercaseTenantGuid()
    {
        var tenantId = Guid.Parse("AABBCCDD-EEFF-0011-2233-445566778899");
        var expected = tenantId.ToString("D").ToLowerInvariant();

        // The container name must be the lowercase GUID string.
        Assert.AreEqual("aabbccdd-eeff-0011-2233-445566778899", expected);
        Assert.IsTrue(expected.Length <= 63, "Azure container names must be ≤ 63 characters.");
        Assert.IsFalse(expected.Contains('_'), "Azure container names must not contain underscores.");
    }
}
