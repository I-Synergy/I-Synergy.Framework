using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Storage.Abstractions;
using ISynergy.Framework.Storage.Azure.Abstractions;
using Moq;
using Xunit;

namespace ISynergy.Framework.Storage.Azure.Tests
{
    /// <summary>
    /// Class AzureBlobTests.
    /// Implements the <see cref="TestFixture" />
    /// </summary>
    /// <seealso cref="TestFixture" />
    public class StorageServiceTests
    {
        private readonly ITenantService _tenantService;
        private readonly IAzureStorageBlobOptions _storageBlobOptions;
        private readonly IStorageService _storageService;

        public StorageServiceTests()
        {
            var tenantMock = new Mock<ITenantService>();
            _tenantService = tenantMock.Object;
            _tenantService.SetTenant(Guid.Parse("52F702C1-7304-466C-9DE6-B02FC14B4C3E"), "TestUser");

            var configMock = new Mock<IAzureStorageBlobOptions>();
            _storageBlobOptions = configMock.Object;
            _storageBlobOptions.ConnectionString = "https://aka.ms/bloburl";

            var storageMock = new Mock<IStorageService>();
            storageMock.Setup(x => x.UploadFileAsync(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

            storageMock.Setup(x => x.UpdateFileAsync(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

            storageMock.Setup(x => x.DownloadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            storageMock.Setup(x => x.RemoveFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _storageService = storageMock.Object;
        }

        [Fact]
        public async Task UploadBlobAsyncTest()
        {
            var ms = new MemoryStream();
            var uri = await _storageService.UploadFileAsync(ms, "contentType", "filename", "folder");
            Assert.NotNull(uri);
        }
     
        [Fact]
        public async Task DownloadFileAsyncTest()
        {
            var stream = await _storageService.DownloadFileAsync("filename", "folder");
            Assert.NotNull(stream);
        }

        [Fact]
        public async Task UpdateBlobAsyncTest()
        {
            var ms = new MemoryStream();
            var uri = await _storageService.UpdateFileAsync(ms, "contentType", "filename", "folder");
            Assert.NotNull(uri);
        }

        [Fact]
        public async Task RemoveFileAsyncTest()
        {
            var result = await _storageService.RemoveFileAsync("filename", "folder");
            Assert.True(result);
        }
    }
}
