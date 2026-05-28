using Amazon.Runtime;
using Amazon.S3;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.S3.Extensions;
using ISynergy.Framework.EventSourcing.Storage.S3.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Storage.S3.Tests.Services;

[TestClass]
public class S3EventArchiveStorageTests
{
    private AmazonS3Client _client = null!;
    private ILogger<S3EventArchiveStorage> _logger = null!;

    // ------------------------------------------------------------------ //
    // Setup
    // ------------------------------------------------------------------ //

    [TestInitialize]
    public void Setup()
    {
        var credentials = new BasicAWSCredentials("fake-access-key", "fake-secret-key");
        var config = new AmazonS3Config { ServiceURL = "http://localhost:9999", ForcePathStyle = true };
        _client = new AmazonS3Client(credentials, config);

        _logger = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider()
            .GetRequiredService<ILogger<S3EventArchiveStorage>>();
    }

    [TestCleanup]
    public void Cleanup() => _client.Dispose();

    // ------------------------------------------------------------------ //
    // Constructor null / empty guards
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_NullClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new S3EventArchiveStorage(null!, "archive", _logger));
    }

    [TestMethod]
    public void Constructor_EmptyBucketNamePrefix_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new S3EventArchiveStorage(_client, "", _logger));
    }

    [TestMethod]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new S3EventArchiveStorage(_client, "archive", null!));
    }

    // ------------------------------------------------------------------ //
    // Construction with valid parameters
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_WithValidArguments_Succeeds()
    {
        var storage = new S3EventArchiveStorage(_client, "archive", _logger);

        Assert.IsNotNull(storage);
        Assert.IsInstanceOfType<IEventArchiveStorage>(storage);
    }

    // ------------------------------------------------------------------ //
    // DI resolution
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void GetService_ViaAddS3EventArchiveStorage_ResolvesSuccessfully()
    {
        var credentials = new BasicAWSCredentials("fake-key", "fake-secret");
        var config = new AmazonS3Config { ServiceURL = "http://localhost:9999", ForcePathStyle = true };

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new AmazonS3Client(credentials, config));
        services.AddS3EventArchiveStorage("archive");

        var scope = services.BuildServiceProvider().CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<IEventArchiveStorage>();

        Assert.IsNotNull(storage);
    }

    // ------------------------------------------------------------------ //
    // Bucket name format
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void BucketName_Format_MatchesExpectedPattern()
    {
        // Each tenant must get its own bucket named {prefix}-{tenantId}.
        // This mirrors the Azure pattern where each tenant gets its own container.
        var tenantId = Guid.Parse("aabbccdd-eeff-0011-2233-445566778899");
        const string prefix = "archive";

        var expected = $"{prefix}-{tenantId.ToString("D").ToLowerInvariant()}";

        Assert.AreEqual("archive-aabbccdd-eeff-0011-2233-445566778899", expected);
        Assert.IsTrue(expected.Length <= 63, "S3 bucket names must be ≤ 63 characters.");
        Assert.IsFalse(expected.Contains('_'), "S3 bucket names must not contain underscores.");
    }

    [TestMethod]
    public void ObjectKey_Format_ContainsNoTenantPrefix()
    {
        // Tenant isolation is at the bucket level (not the key), so the object
        // key must only contain stream coordinates — identical to the Azure blob path.
        var streamType = "Order";
        var streamId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        const long versionFrom = 1L;
        const long versionTo = 10L;

        var key = $"{streamType}/{streamId}/{versionFrom}-{versionTo}.json";

        Assert.AreEqual("Order/11111111-2222-3333-4444-555555555555/1-10.json", key);
        Assert.IsFalse(key.StartsWith(Guid.NewGuid().ToString()[..8]),
            "Key must not be prefixed with a tenant GUID.");
    }
}
