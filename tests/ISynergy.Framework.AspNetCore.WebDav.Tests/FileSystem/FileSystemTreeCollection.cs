using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support.ServiceBuilders;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.FileSystem
{
    public abstract class FileSystemTreeCollection<T> : IClassFixture<T>, IDisposable
        where T : class, IFileSystemServices
    {
        private readonly IServiceScope _serviceScope;

        protected FileSystemTreeCollection(T fsServices)
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
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Equal(0, rootNode.Nodes.Count);
        }

        [Fact]
        public async Task SingleEmptyDirectory()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            await root.CreateCollectionAsync("test1", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes,
                node =>
                {
                    Assert.NotNull(node.Collection);
                    Assert.Equal("test1", node.Collection.Name);
                    Assert.Same(rootNode.Collection, node.Collection.Parent);
                    Assert.Equal(0, node.Documents.Count);
                    Assert.Equal(0, node.Nodes.Count);
                });
        }

        [Fact]
        public async Task TwoNestedEmptyDirectories()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            await test1.CreateCollectionAsync("test1.1", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes,
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test1", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Documents.Count);
                    Assert.Collection(
                        node1.Nodes,
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test1.1", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                });
        }

        [Fact]
        public async Task TwoEmptyDirectories()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            await root.CreateCollectionAsync("test1", ct);
            await root.CreateCollectionAsync("test2", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes.OrderBy(n => n.Name),
                node =>
                {
                    Assert.NotNull(node.Collection);
                    Assert.Equal("test1", node.Collection.Name);
                    Assert.Same(rootNode.Collection, node.Collection.Parent);
                    Assert.Equal(0, node.Documents.Count);
                    Assert.Equal(0, node.Nodes.Count);
                },
                node =>
                {
                    Assert.NotNull(node.Collection);
                    Assert.Equal("test2", node.Collection.Name);
                    Assert.Same(rootNode.Collection, node.Collection.Parent);
                    Assert.Equal(0, node.Documents.Count);
                    Assert.Equal(0, node.Nodes.Count);
                });
        }

        [Fact]
        public async Task TwoDirectoriesWithOneEmptyChildDirectory()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            await test1.CreateCollectionAsync("test1.1", ct);
            var test2 = await root.CreateCollectionAsync("test2", ct);
            await test2.CreateCollectionAsync("test2.1", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes.OrderBy(n => n.Name),
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test1", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Documents.Count);
                    Assert.Collection(
                        node1.Nodes,
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test1.1", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                },
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test2", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Documents.Count);
                    Assert.Collection(
                        node1.Nodes,
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test2.1", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                });
        }

        [Fact]
        public async Task TwoDirectoriesWithTwoEmptyChildDirectories()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            await test1.CreateCollectionAsync("test1.1", ct);
            await test1.CreateCollectionAsync("test1.2", ct);
            var test2 = await root.CreateCollectionAsync("test2", ct);
            await test2.CreateCollectionAsync("test2.1", ct);
            await test2.CreateCollectionAsync("test2.2", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes.OrderBy(n => n.Name),
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test1", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Documents.Count);
                    Assert.Collection(
                        node1.Nodes.OrderBy(n => n.Name),
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test1.1", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        },
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test1.2", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                },
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test2", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Documents.Count);
                    Assert.Collection(
                        node1.Nodes.OrderBy(n => n.Name),
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test2.1", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        },
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test2.2", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                });
        }

        [Fact]
        public async Task TwoDirectoriesWithTwoEmptyFiles()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            await test1.CreateDocumentAsync("test1.1", ct);
            await test1.CreateDocumentAsync("test1.2", ct);
            var test2 = await root.CreateCollectionAsync("test2", ct);
            await test2.CreateDocumentAsync("test2.1", ct);
            await test2.CreateDocumentAsync("test2.2", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes.OrderBy(n => n.Name),
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test1", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Nodes.Count);
                    Assert.Collection(
                        node1.Documents.OrderBy(n => n.Name),
                        document =>
                        {
                            Assert.Equal("test1.1", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        },
                        document =>
                        {
                            Assert.Equal("test1.2", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        });
                },
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test2", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Equal(0, node1.Nodes.Count);
                    Assert.Collection(
                        node1.Documents.OrderBy(n => n.Name),
                        document =>
                        {
                            Assert.Equal("test2.1", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        },
                        document =>
                        {
                            Assert.Equal("test2.2", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        });
                });
        }

        [Fact]
        public async Task TwoDirectoriesWithTwoEmptyFilesAndEmptyDirectory()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            var test1 = await root.CreateCollectionAsync("test1", ct);
            await test1.CreateDocumentAsync("test1.1", ct);
            await test1.CreateCollectionAsync("test1.2", ct);
            await test1.CreateDocumentAsync("test1.3", ct);
            var test2 = await root.CreateCollectionAsync("test2", ct);
            await test2.CreateDocumentAsync("test2.1", ct);
            await test2.CreateCollectionAsync("test2.2", ct);
            await test2.CreateDocumentAsync("test2.3", ct);
            var rootNode = await root.GetNodeAsync(int.MaxValue, ct);
            Assert.Same(root, rootNode.Collection);
            Assert.Equal(0, rootNode.Documents.Count);
            Assert.Collection(
                rootNode.Nodes.OrderBy(n => n.Name),
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test1", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Collection(
                        node1.Nodes.OrderBy(n => n.Name),
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test1.2", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                    Assert.Collection(
                        node1.Documents.OrderBy(n => n.Name),
                        document =>
                        {
                            Assert.Equal("test1.1", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        },
                        document =>
                        {
                            Assert.Equal("test1.3", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        });
                },
                node1 =>
                {
                    Assert.NotNull(node1.Collection);
                    Assert.Equal("test2", node1.Collection.Name);
                    Assert.Same(rootNode.Collection, node1.Collection.Parent);
                    Assert.Collection(
                        node1.Nodes.OrderBy(n => n.Name),
                        node2 =>
                        {
                            Assert.NotNull(node2.Collection);
                            Assert.Equal("test2.2", node2.Collection.Name);
                            Assert.Same(node1.Collection, node2.Collection.Parent);
                            Assert.Equal(0, node2.Documents.Count);
                            Assert.Equal(0, node2.Nodes.Count);
                        });
                    Assert.Collection(
                        node1.Documents.OrderBy(n => n.Name),
                        document =>
                        {
                            Assert.Equal("test2.1", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        },
                        document =>
                        {
                            Assert.Equal("test2.3", document.Name);
                            Assert.Same(node1.Collection, document.Parent);
                        });
                });
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
