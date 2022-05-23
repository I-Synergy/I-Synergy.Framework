using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Extensions;
using ISynergy.Framework.AspNetCore.WebDav.FileSystem.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Locking.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using ISynergy.Framework.AspNetCore.WebDav.Storage.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public class MountTests : IClassFixture<MountTests.FileSystemServices>, IDisposable
    {
        private readonly IServiceScope _serviceScope;

        public MountTests(FileSystemServices fsServices)
        {
            var serviceScopeFactory = fsServices.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
            _serviceScope = serviceScopeFactory.CreateScope();
            FileSystem = _serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();
        }

        public IFileSystem FileSystem { get; }

        [Fact]
        public async Task CannotCreateDocument()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await root.CreateDocumentAsync("test1", ct));
        }

        [Fact]
        public async Task CannotCreateCollection()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await root.CreateCollectionAsync("test1", ct));
        }

        [Fact]
        public async Task CannotModifyReadOnlyEntry()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test = await root.GetChildAsync("test", ct);
            Assert.NotNull(test);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await test.DeleteAsync(ct));
        }

        [Fact]
        public async Task DocumentInMountPoint()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test = await root.GetChildAsync("test", ct) as ICollection;
            Assert.NotNull(test);
            var testText = await test.GetChildAsync("test.txt", ct) as IDocument;
            Assert.NotNull(testText);
            Assert.Equal("Hello!", await testText.ReadAllAsync(ct));
        }

        [Fact]
        public async Task CanRemoveDocumentInMountPoint()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test = await root.GetChildAsync("test", ct) as ICollection;
            Assert.NotNull(test);
            var testText = await test.GetChildAsync("test.txt", ct) as IDocument;
            Assert.NotNull(testText);
            await testText.DeleteAsync(ct);
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        public class FileSystemServices : IDisposable
        {
            private readonly ServiceProvider _rootServiceProvider;
            private readonly IServiceScope _scope;

            public FileSystemServices()
            {
                IPropertyStoreFactory propertyStoreFactory = null;

                var serviceCollection = new ServiceCollection()
                    .AddOptions()
                    .AddLogging()
                    .Configure<InMemoryLockManagerOptions>(
                        opt =>
                        {
                            opt.Rounding = new DefaultLockTimeRounding(DefaultLockTimeRoundingMode.OneHundredMilliseconds);
                        })
                    .AddScoped<ILockManager, InMemoryLockManager>()
                    .AddScoped<IWebDavContext>(sp => new TestHost(sp, new Uri("http://localhost/")))
                    .AddScoped<InMemoryFileSystemFactory>()
                    .AddScoped<IFileSystemFactory, MyVirtualRootFileSystemFactory>()
                    .AddScoped(sp => propertyStoreFactory ?? (propertyStoreFactory = ActivatorUtilities.CreateInstance<InMemoryPropertyStoreFactory>(sp)))
                    .AddWebDav();

                _rootServiceProvider = serviceCollection.BuildServiceProvider(true);
                _scope = _rootServiceProvider.CreateScope();

                var loggerFactory = _rootServiceProvider.GetRequiredService<ILoggerFactory>();
                loggerFactory.AddDebug(LogLevel.Trace);
            }

            public IServiceProvider ServiceProvider => _scope.ServiceProvider;

            public void Dispose()
            {
                _scope.Dispose();
                _rootServiceProvider.Dispose();
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class MyVirtualRootFileSystemFactory : InMemoryFileSystemFactory
        {
            private readonly IServiceProvider _serviceProvider;

            public MyVirtualRootFileSystemFactory(
                IServiceProvider serviceProvider,
                IPathTraversalEngine pathTraversalEngine,
                ISystemClock systemClock,
                ILockManager lockManager = null,
                IPropertyStoreFactory propertyStoreFactory = null)
                : base(pathTraversalEngine, systemClock, lockManager, propertyStoreFactory)
            {
                _serviceProvider = serviceProvider;
            }

            protected override void InitializeFileSystem(ICollection mountPoint, IPrincipal principal, InMemoryFileSystem fileSystem)
            {
                // Create the mount point
                var testMountPoint = fileSystem.RootCollection.CreateCollection("test");

                // Create the mount point file system
                var testMountPointFileSystemFactory = _serviceProvider.GetRequiredService<InMemoryFileSystemFactory>();
                var testMountPointFileSystem = Assert.IsType<InMemoryFileSystem>(testMountPointFileSystemFactory.CreateFileSystem(testMountPoint, principal));

                // Populate content of mount point file system
                testMountPointFileSystem.RootCollection.CreateDocument("test.txt").Data = new MemoryStream(Encoding.UTF8.GetBytes("Hello!"));

                // Add mount point
                fileSystem.Mount(testMountPoint.Path, testMountPointFileSystem);

                // Make the root file system read-only
                fileSystem.IsReadOnly = true;
            }
        }
    }
}
