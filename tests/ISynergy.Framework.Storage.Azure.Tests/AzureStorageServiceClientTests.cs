using Azure.Core;
using Azure.Storage.Blobs;
using ISynergy.Framework.Storage.Azure.Options;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionsX = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.Storage.Azure;

/// <summary>
/// Tests for the authentication precedence in <see cref="AzureStorageService"/>:
/// injected client, identity endpoint, then connection string.
/// </summary>
[TestClass]
public class AzureStorageServiceClientTests
{
    [TestMethod]
    public void CreateBlobContainerClient_UsesInjectedBlobServiceClient()
    {
        var blobServiceClient = new BlobServiceClient(new Uri("https://injected.blob.core.windows.net"));
        var service = new AzureStorageService(
            OptionsX.Create(new AzureStorageOptions
            {
                ServiceEndpoint = "https://identity.blob.core.windows.net",
                ConnectionString = "UseDevelopmentStorage=true"
            }),
            new FakeTokenCredential(),
            blobServiceClient);

        var container = service.CreateBlobContainerClient("mycontainer");

        Assert.AreEqual("https://injected.blob.core.windows.net/mycontainer", container.Uri.AbsoluteUri.TrimEnd('/'));
    }

    [TestMethod]
    public void CreateBlobContainerClient_UsesIdentityEndpoint_AndPrefersItOverConnectionString()
    {
        var service = new AzureStorageService(
            OptionsX.Create(new AzureStorageOptions
            {
                ServiceEndpoint = "https://identity.blob.core.windows.net/",
                ConnectionString = "UseDevelopmentStorage=true"
            }),
            new FakeTokenCredential());

        var container = service.CreateBlobContainerClient("mycontainer");

        Assert.AreEqual("https://identity.blob.core.windows.net/mycontainer", container.Uri.AbsoluteUri.TrimEnd('/'));
        Assert.AreEqual("identity", container.AccountName);
    }

    [TestMethod]
    public void CreateBlobContainerClient_FallsBackToConnectionString_WhenNoIdentityEndpointIsConfigured()
    {
        var service = new AzureStorageService(
            OptionsX.Create(new AzureStorageOptions
            {
                ConnectionString = "UseDevelopmentStorage=true"
            }),
            new FakeTokenCredential());

        var container = service.CreateBlobContainerClient("mycontainer");

        Assert.AreEqual("mycontainer", container.Name);
        Assert.AreEqual("devstoreaccount1", container.AccountName);
    }

    [TestMethod]
    public void CreateBlobContainerClient_Throws_WhenNothingIsConfigured()
    {
        var service = new AzureStorageService(OptionsX.Create(new AzureStorageOptions()));

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => service.CreateBlobContainerClient("mycontainer"));

        Assert.Contains(nameof(AzureStorageOptions.ServiceEndpoint), exception.Message);
        Assert.Contains(nameof(AzureStorageOptions.ConnectionString), exception.Message);
    }

    [TestMethod]
    public void CreateBlobContainerClient_CreatesDefaultCredential_WhenIdentityIsConfiguredWithoutClientId()
    {
        var service = new AzureStorageService(
            OptionsX.Create(new AzureStorageOptions
            {
                ServiceEndpoint = "https://identity.blob.core.windows.net"
            }));

        var container = service.CreateBlobContainerClient("mycontainer");

        Assert.AreEqual("https://identity.blob.core.windows.net/mycontainer", container.Uri.AbsoluteUri.TrimEnd('/'));
    }

    [TestMethod]
    public void CreateBlobContainerClient_CreatesManagedIdentityCredential_WhenClientIdIsConfigured()
    {
        var service = new AzureStorageService(
            OptionsX.Create(new AzureStorageOptions
            {
                ServiceEndpoint = "https://identity.blob.core.windows.net",
                ManagedIdentityClientId = Guid.NewGuid().ToString()
            }));

        var container = service.CreateBlobContainerClient("mycontainer");

        Assert.AreEqual("https://identity.blob.core.windows.net/mycontainer", container.Uri.AbsoluteUri.TrimEnd('/'));
    }

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
