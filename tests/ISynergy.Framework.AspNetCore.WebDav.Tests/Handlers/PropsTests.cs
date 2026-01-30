using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DecaTec.WebDav.WebDavArtifacts;
using ISynergy.Framework.AspNetCore.WebDav.FileSystem.InMemory;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Live;
using ISynergy.Framework.AspNetCore.WebDav.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Handlers
{
    public class PropsTests : ServerTestsBase
    {
        private readonly XName[] _propsToIgnoreDocument;
        private readonly XName[] _propsToIgnoreCollection;

        public PropsTests()
            : base(RecursiveProcessingMode.PreferFastest)
        {
            _propsToIgnoreDocument = new[] { LockDiscoveryProperty.PropertyName, DisplayNameProperty.PropertyName, GetETagProperty.PropertyName };
            _propsToIgnoreCollection = new[] { LockDiscoveryProperty.PropertyName, DisplayNameProperty.PropertyName, GetETagProperty.PropertyName };
            Dispatcher = ServiceProvider.GetRequiredService<IWebDavDispatcher>();
        }

        private IWebDavDispatcher Dispatcher { get; }

        [Fact]
        public async Task SetNewProp()
        {
            var ct = CancellationToken.None;
            var root = await FileSystem.Root;
            const string resourceName = "text1.txt";

            var doc1 = await root.CreateDocumentAsync(resourceName, ct);
            await doc1.FillWithAsync("Dokument 1", ct);

            var propsBefore = await doc1.GetPropertyElementsAsync(Dispatcher, ct);

            var requestUri = new Uri(Client.BaseAddress, new Uri(resourceName, UriKind.Relative));
            var propertyValue = "<testProp>someValue</testProp>";
            var response = await Client
                .PropPatchAsync(
                    requestUri,
                    new PropertyUpdate
                    {
                        Items = new[]
                        {
                            new Set
                            {
                                Prop = new Prop
                                {
                                    AdditionalProperties = new[]
                                    {
                                        XElement.Parse(propertyValue),
                                    },
                                },
                            },
                        },
                    },
                    ct)
                ;

            Assert.True(response.IsSuccessStatusCode);

            var child = await root.GetChildAsync(resourceName, ct);
            var doc2 = Assert.IsType<InMemoryFile>(child);
            var props2 = await doc2.GetPropertyElementsAsync(Dispatcher, ct);
            var changes = PropertyComparer.FindChanges(propsBefore, props2, _propsToIgnoreDocument);
            var addedProperty = Assert.Single(changes);

            Assert.NotNull(addedProperty);

            var expectedAddedChangeItem = PropertyChangeItem.Added(XElement.Parse(propertyValue));

            Assert.Equal(expectedAddedChangeItem.Name, addedProperty.Name);
            Assert.Equal(expectedAddedChangeItem.Change, addedProperty.Change);
            Assert.Equal(expectedAddedChangeItem.Left, addedProperty.Left);
            Assert.True(XNode.DeepEquals(expectedAddedChangeItem.Right, addedProperty.Right));
        }
    }
}
