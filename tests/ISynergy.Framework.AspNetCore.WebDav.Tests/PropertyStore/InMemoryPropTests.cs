using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.PropertyStore
{
    public class InMemoryPropTests : IClassFixture<InMemoryFileSystemServices>, IDisposable
    {
        private readonly IServiceScope _serviceScope;

        public InMemoryPropTests(InMemoryFileSystemServices fsServices)
        {
            var serviceScopeFactory = fsServices.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
            _serviceScope = serviceScopeFactory.CreateScope();
            FileSystem = _serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();
            Dispatcher = _serviceScope.ServiceProvider.GetRequiredService<IWebDavDispatcher>();
        }

        public IWebDavDispatcher Dispatcher { get; }

        public IFileSystem FileSystem { get; }

        public IPropertyStore PropertyStore
        {
            get
            {
                Assert.NotNull(FileSystem.PropertyStore);
                return FileSystem.PropertyStore;
            }
        }

        [Fact]
        public async Task Empty()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var displayNameProperty = await GetDisplayNamePropertyAsync(root, ct);
            Assert.Equal(string.Empty, await displayNameProperty.GetValueAsync(ct));
        }

        [Fact]
        public async Task DocumentWithExtension()
        {
            var ct = CancellationToken.None;

            var root = await FileSystem.Root;
            var doc = await root.CreateDocumentAsync("test1.txt", ct);

            var displayNameProperty = await GetDisplayNamePropertyAsync(doc, ct);
            Assert.Equal("test1.txt", await displayNameProperty.GetValueAsync(ct));
        }

        [Fact]
        public async Task SameNameDocumentsInDifferentCollections()
        {
            var ct = CancellationToken.None;

            var root = await FileSystem.Root;
            var coll1 = await root.CreateCollectionAsync("coll1", ct);
            var docRoot = await root.CreateDocumentAsync("test1.txt", ct);
            var docColl1 = await coll1.CreateDocumentAsync("test1.txt", ct);
            var eTagDocRoot = await PropertyStore.GetETagAsync(docRoot, ct);
            var eTagDocColl1 = await PropertyStore.GetETagAsync(docColl1, ct);
            Assert.NotEqual(eTagDocRoot, eTagDocColl1);
        }

        [Fact]
        public async Task DisplayNameChangeable()
        {
            var ct = CancellationToken.None;

            var root = await FileSystem.Root;
            var doc = await root.CreateDocumentAsync("test1.txt", ct);
            var displayNameProperty = await GetDisplayNamePropertyAsync(doc, ct);

            await displayNameProperty.SetValueAsync("test1-Dokument", ct);
            Assert.Equal("test1-Dokument", await displayNameProperty.GetValueAsync(ct));

            displayNameProperty = await GetDisplayNamePropertyAsync(doc, ct);
            Assert.Equal("test1-Dokument", await displayNameProperty.GetValueAsync(ct));
        }

        [Theory]
        [InlineData("test1.txt", "text/plain")]
        [InlineData("test1.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [InlineData("test1.png", "image/png")]
        public async Task ContentTypeDetected(string fileName, string contentType)
        {
            var ct = CancellationToken.None;

            var root = await FileSystem.Root;
            var doc = await root.CreateDocumentAsync(fileName, ct);
            var contentTypeProperty = await GetContentTypePropertyAsync(doc, ct);

            Assert.Equal(contentType, await contentTypeProperty.GetValueAsync(ct));
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        private async Task<DisplayNameProperty> GetDisplayNamePropertyAsync(IEntry entry, CancellationToken ct)
        {
            var untypedDisplayNameProperty = await entry.GetProperties(Dispatcher).Single(x => x.Name == DisplayNameProperty.PropertyName, ct);
            Assert.NotNull(untypedDisplayNameProperty);
            var displayNameProperty = Assert.IsType<DisplayNameProperty>(untypedDisplayNameProperty);
            return displayNameProperty;
        }

        private async Task<GetContentTypeProperty> GetContentTypePropertyAsync(IEntry entry, CancellationToken ct)
        {
            var untypedContentTypeProperty = await entry.GetProperties(Dispatcher).Single(x => x.Name == GetContentTypeProperty.PropertyName, ct);
            Assert.NotNull(untypedContentTypeProperty);
            var contentTypeProperty = Assert.IsType<GetContentTypeProperty>(untypedContentTypeProperty);
            return contentTypeProperty;
        }
    }
}
