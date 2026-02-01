using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public abstract class SimpleFsTests<T> : IClassFixture<T>, IDisposable
        where T : class, IFileSystemServices
    {
        private readonly IServiceScope _serviceScope;

        protected SimpleFsTests(T fsServices)
        {
            var serviceScopeFactory = fsServices.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
            _serviceScope = serviceScopeFactory.CreateScope();
            FileSystem = _serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();
        }

        public IFileSystem FileSystem { get; }

        [Fact]
        public async Task Empty()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var rootChildren = await root.GetChildrenAsync(ct);
            Assert.Equal(0, rootChildren.Count);
        }

        [Fact]
        public async Task SingleEmptyDirectory()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            var rootChildren = await root.GetChildrenAsync(ct);
            Assert.Collection(
                rootChildren,
                child =>
                {
                    Assert.NotNull(child);
                    var coll = Assert.IsAssignableFrom<ICollection>(child);
                    Assert.Equal(test1.Path, coll.Path);
                    Assert.Equal("test1", coll.Name);
                    Assert.NotNull(coll.Parent);
                    Assert.Equal(root.Path, coll.Parent.Path);
                });
        }

        [Fact]
        public async Task TwoEmptyDirectories()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            var test2 = await root.CreateCollectionAsync("test2", ct);
            var rootChildren = await root.GetChildrenAsync(ct);
            Assert.Collection(
                rootChildren.OrderBy(n => n.Name),
                child =>
                {
                    Assert.NotNull(child);
                    var coll = Assert.IsAssignableFrom<ICollection>(child);
                    Assert.Equal(test1.Path, coll.Path);
                    Assert.Equal("test1", coll.Name);
                    Assert.NotNull(coll.Parent);
                    Assert.Equal(root.Path, coll.Parent.Path);
                },
                child =>
                {
                    Assert.NotNull(child);
                    var coll = Assert.IsAssignableFrom<ICollection>(child);
                    Assert.Equal(test2.Path, coll.Path);
                    Assert.Equal("test2", coll.Name);
                    Assert.NotNull(coll.Parent);
                    Assert.Equal(root.Path, coll.Parent.Path);
                });
        }

        [Fact]
        public async Task CannotAddTwoDirectoriesWithSameName()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            await root.CreateCollectionAsync("test1", ct);
            await Assert.ThrowsAnyAsync<IOException>(async () => await root.CreateCollectionAsync("test1", ct));
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
