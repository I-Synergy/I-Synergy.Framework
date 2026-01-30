using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Storage.Abstractions.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Storage.Azure.Tests;

/// <summary>
/// Class AzureBlobTests.
/// </summary>
[TestClass]
public class StorageServiceTests
{
    private readonly Guid tenantId = Guid.Parse("52F702C1-7304-466C-9DE6-B02FC14B4C3E");
    private readonly string connectionString = "https://aka.ms/bloburl";
    /// <summary>
    /// The tenant service
    /// </summary>
    private readonly ITenantService _tenantService;
    /// <summary>
    /// The storage service
    /// </summary>
    private readonly IStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageServiceTests"/> class.
    /// </summary>
    public StorageServiceTests()
    {
        Mock<ITenantService> tenantMock = new();
        _tenantService = tenantMock.Object;
        _tenantService.SetTenant(tenantId, "TestUser");

        Mock<IStorageService> storageMock = new();

        storageMock.Setup(x => x.UploadFileAsync(
                connectionString,
                tenantId.ToString(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

        storageMock.Setup(x => x.UpdateFileAsync(
                connectionString,
                tenantId.ToString(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

        storageMock.Setup(x => x.DownloadFileAsync(
                connectionString,
                tenantId.ToString(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<byte>());

        storageMock.Setup(x => x.RemoveFileAsync(
                connectionString,
                tenantId.ToString(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _storageService = storageMock.Object;
    }

    /// <summary>
    /// Defines the test method UploadBlobAsyncTest.
    /// </summary>
    [TestMethod]
    public async Task UploadBlobAsyncTest()
    {
        Uri uri = await _storageService.UploadFileAsync(connectionString, tenantId.ToString(), Array.Empty<byte>(), "contentType", "filename", "folder");
        Assert.IsNotNull(uri);
    }

    /// <summary>
    /// Defines the test method DownloadFileAsyncTest.
    /// </summary>
    [TestMethod]
    public async Task DownloadFileAsyncTest()
    {
        byte[] bytes = await _storageService.DownloadFileAsync(connectionString, tenantId.ToString(), "filename", "folder");
        Assert.IsNotNull(bytes);
    }

    /// <summary>
    /// Defines the test method UpdateBlobAsyncTest.
    /// </summary>
    [TestMethod]
    public async Task UpdateBlobAsyncTest()
    {
        Uri uri = await _storageService.UpdateFileAsync(connectionString, tenantId.ToString(), Array.Empty<byte>(), "contentType", "filename", "folder");
        Assert.IsNotNull(uri);
    }

    /// <summary>
    /// Defines the test method RemoveFileAsyncTest.
    /// </summary>
    [TestMethod]
    public async Task RemoveFileAsyncTest()
    {
        bool result = await _storageService.RemoveFileAsync(connectionString, tenantId.ToString(), "filename", "folder");
        Assert.IsTrue(result);
    }
}
