using System;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Storage.Abstractions;
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
        private readonly AzureBlobOptions _storageBlobOptions;
        private readonly IStorageService<AzureBlobOptions> _storageService;

        public StorageServiceTests()
        {
            var tenantMock = new Mock<ITenantService>();
            _tenantService = tenantMock.Object;
            _tenantService.SetTenant(Guid.Parse("52F702C1-7304-466C-9DE6-B02FC14B4C3E"), "TestUser");

            var configMock = new Mock<AzureBlobOptions>();
            _storageBlobOptions = configMock.Object;
            _storageBlobOptions.ConnectionString = "https://aka.ms/bloburl";

            var storageMock = new Mock<IStorageService<AzureBlobOptions>>();
            storageMock.Setup(x => x.UploadFileAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

            storageMock.Setup(x => x.UpdateFileAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Uri(@"https://aka.ms/bloburl"));

            storageMock.Setup(x => x.DownloadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<byte>());

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
            var uri = await _storageService.UploadFileAsync(Array.Empty<byte>(), "contentType", "filename", "folder");
            Assert.NotNull(uri);
        }
     
        [Fact]
        public async Task DownloadFileAsyncTest()
        {
            var bytes = await _storageService.DownloadFileAsync("filename", "folder");
            Assert.NotNull(bytes);
        }

        [Fact]
        public async Task UpdateBlobAsyncTest()
        {
            var uri = await _storageService.UpdateFileAsync(Array.Empty<byte>(), "contentType", "filename", "folder");
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
