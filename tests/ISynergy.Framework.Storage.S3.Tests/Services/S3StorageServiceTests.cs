using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Storage.S3.Tests.Services;

[TestClass]
public class S3StorageServiceTests
{
    private IStorageService _service = null!;

    // ------------------------------------------------------------------ //
    // Setup
    // ------------------------------------------------------------------ //

    [TestInitialize]
    public void Setup()
    {
        // Provide non-empty credentials so the constructor's guard clauses pass.
        // The AmazonS3Client SDK validates credentials only on the first API call,
        // so construction succeeds with fake values and pointing to a local URL.
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["S3StorageOptions:AccessKey"] = "fake-access-key",
                ["S3StorageOptions:SecretKey"] = "fake-secret-key",
                ["S3StorageOptions:ServiceUrl"] = "http://localhost:9999",
                ["S3StorageOptions:ForcePathStyle"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddS3StorageIntegration(config);

        _service = services.BuildServiceProvider().GetRequiredService<IStorageService>();
    }

    // ------------------------------------------------------------------ //
    // Constructor validation (via DI with invalid options)
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void GetService_WithEmptyAccessKey_ThrowsOnResolution()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["S3StorageOptions:AccessKey"] = "",
                ["S3StorageOptions:SecretKey"] = "secret"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddS3StorageIntegration(config);

        var provider = services.BuildServiceProvider();

        // DI wraps constructor exceptions in InvalidOperationException.
        Assert.Throws<ArgumentNullException>(() =>
            provider.GetRequiredService<IStorageService>());
    }

    [TestMethod]
    public void GetService_WithEmptySecretKey_ThrowsOnResolution()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["S3StorageOptions:AccessKey"] = "key",
                ["S3StorageOptions:SecretKey"] = ""
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddS3StorageIntegration(config);

        var provider = services.BuildServiceProvider();

        Assert.Throws<ArgumentNullException>(() =>
            provider.GetRequiredService<IStorageService>());
    }

    // ------------------------------------------------------------------ //
    // UploadFileAsync — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task UploadFileAsync_EmptyContainerName_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadFileAsync("conn", "", new byte[1], "text/plain", "file.txt", ""));
    }

    [TestMethod]
    public async Task UploadFileAsync_EmptyFilename_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadFileAsync("conn", "bucket", new byte[1], "text/plain", "", ""));
    }

    [TestMethod]
    public async Task UploadFileAsync_NullFileBytes_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadFileAsync("conn", "bucket", null!, "text/plain", "file.txt", ""));
    }

    // ------------------------------------------------------------------ //
    // DownloadFileAsync — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task DownloadFileAsync_EmptyContainerName_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.DownloadFileAsync("conn", "", "file.txt", ""));
    }

    [TestMethod]
    public async Task DownloadFileAsync_EmptyFilename_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.DownloadFileAsync("conn", "bucket", "", ""));
    }

    // ------------------------------------------------------------------ //
    // UpdateFileAsync — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task UpdateFileAsync_EmptyContainerName_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateFileAsync("conn", "", new byte[1], "text/plain", "file.txt", ""));
    }

    [TestMethod]
    public async Task UpdateFileAsync_EmptyFilename_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateFileAsync("conn", "bucket", new byte[1], "text/plain", "", ""));
    }

    [TestMethod]
    public async Task UpdateFileAsync_NullFileBytes_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateFileAsync("conn", "bucket", null!, "text/plain", "file.txt", ""));
    }

    // ------------------------------------------------------------------ //
    // RemoveFileAsync — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task RemoveFileAsync_EmptyContainerName_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.RemoveFileAsync("conn", "", "file.txt", ""));
    }

    [TestMethod]
    public async Task RemoveFileAsync_EmptyFilename_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.RemoveFileAsync("conn", "bucket", "", ""));
    }

    // ------------------------------------------------------------------ //
    // Dispose
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var disposable = (IDisposable)_service;
        disposable.Dispose();

        // Second dispose must be a no-op (idempotent guard in the service).
        disposable.Dispose();
    }
}
