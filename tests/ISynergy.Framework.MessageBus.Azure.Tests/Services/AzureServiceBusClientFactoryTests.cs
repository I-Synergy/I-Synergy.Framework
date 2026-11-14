using Azure.Core;
using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.MessageBus.Azure.Services;

/// <summary>
/// Tests for the identity-first / connection-string-fallback Service Bus client selection.
/// </summary>
[TestClass]
public class AzureServiceBusClientFactoryTests
{
    private const string FallbackConnectionString =
        "Endpoint=sb://fallback.servicebus.windows.net/;SharedAccessKeyName=key;SharedAccessKey=dmFsdWU=";

    [TestMethod]
    public async Task Create_UsesNamespaceAndCredential_AndPrefersThemOverConnectionString()
    {
        var option = new AzurePublisherOptions
        {
            FullyQualifiedNamespace = "identity.servicebus.windows.net",
            ConnectionString = FallbackConnectionString
        };

        await using var client = AzureServiceBusClientFactory.Create(option, new FakeTokenCredential());

        Assert.AreEqual("identity.servicebus.windows.net", client.FullyQualifiedNamespace);
    }

    [TestMethod]
    public async Task Create_FallsBackToConnectionString_WhenNamespaceIsNotConfigured()
    {
        var option = new AzurePublisherOptions
        {
            ConnectionString = FallbackConnectionString
        };

        await using var client = AzureServiceBusClientFactory.Create(option, new FakeTokenCredential());

        Assert.AreEqual("fallback.servicebus.windows.net", client.FullyQualifiedNamespace);
    }

    [TestMethod]
    public void Create_Throws_WhenNeitherNamespaceNorConnectionStringIsConfigured()
    {
        var option = new AzureSubscriberOptions { QueueName = "queue" };

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => AzureServiceBusClientFactory.Create(option));

        Assert.Contains("FullyQualifiedNamespace", exception.Message);
        Assert.Contains("ConnectionString", exception.Message);
    }

    [TestMethod]
    public void IsConfigured_ReturnsTrue_ForNamespaceOnly()
    {
        var option = new AzurePublisherOptions { FullyQualifiedNamespace = "identity.servicebus.windows.net" };

        Assert.IsTrue(AzureServiceBusClientFactory.IsConfigured(option));
    }

    [TestMethod]
    public void IsConfigured_ReturnsTrue_ForConnectionStringOnly()
    {
        var option = new AzurePublisherOptions { ConnectionString = FallbackConnectionString };

        Assert.IsTrue(AzureServiceBusClientFactory.IsConfigured(option));
    }

    [TestMethod]
    public void IsConfigured_ReturnsFalse_WhenNothingIsConfigured()
    {
        var option = new AzurePublisherOptions();

        Assert.IsFalse(AzureServiceBusClientFactory.IsConfigured(option));
    }

    private sealed class FakeTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => new("fake-token", DateTimeOffset.MaxValue);

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(new AccessToken("fake-token", DateTimeOffset.MaxValue));
    }
}
