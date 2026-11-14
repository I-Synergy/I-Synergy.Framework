using ISynergy.Framework.MessageBus.Azure.Options.Queue;
using ISynergy.Framework.MessageBus.Azure.Services.Queue;
using ISynergy.Framework.MessageBus.Azure.Tests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionsX = Microsoft.Extensions.Options.Options;

namespace ISynergy.Framework.MessageBus.Azure.Services;

/// <summary>
/// Tests for the identity/connection-string validation in the message bus constructors.
/// </summary>
[TestClass]
public class ServiceBusConstructorTests
{
    [TestMethod]
    public void Publisher_AllowsNamespaceOnlyConfiguration()
    {
        var options = new AzurePublisherOptions
        {
            QueueName = "queue",
            FullyQualifiedNamespace = "identity.servicebus.windows.net"
        };

        var publisher = CreatePublisher(options);

        Assert.IsNotNull(publisher);
    }

    [TestMethod]
    public void Publisher_AllowsConnectionStringOnlyConfiguration()
    {
        var options = new AzurePublisherOptions
        {
            QueueName = "queue",
            ConnectionString = "Endpoint=sb://fallback.servicebus.windows.net/;SharedAccessKeyName=key;SharedAccessKey=dmFsdWU="
        };

        var publisher = CreatePublisher(options);

        Assert.IsNotNull(publisher);
    }

    [TestMethod]
    public void Publisher_Throws_WhenNoAuthenticationIsConfigured()
    {
        var options = new AzurePublisherOptions { QueueName = "queue" };

        Assert.ThrowsExactly<InvalidOperationException>(() => CreatePublisher(options));
    }

    [TestMethod]
    public void Subscriber_AllowsNamespaceOnlyConfiguration()
    {
        var options = new AzureSubscriberOptions
        {
            QueueName = "queue",
            FullyQualifiedNamespace = "identity.servicebus.windows.net"
        };

        var subscriber = new TestSubscriberServiceBus(
            OptionsX.Create(options),
            NullLogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>>.Instance);

        Assert.IsNotNull(subscriber);
    }

    [TestMethod]
    public void Subscriber_AllowsConnectionStringOnlyConfiguration()
    {
        var options = new AzureSubscriberOptions
        {
            QueueName = "queue",
            ConnectionString = "Endpoint=sb://fallback.servicebus.windows.net/;SharedAccessKeyName=key;SharedAccessKey=dmFsdWU="
        };

        var subscriber = new TestSubscriberServiceBus(
            OptionsX.Create(options),
            NullLogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>>.Instance);

        Assert.IsNotNull(subscriber);
    }

    [TestMethod]
    public void Subscriber_Throws_WhenNoAuthenticationIsConfigured()
    {
        var options = new AzureSubscriberOptions { QueueName = "queue" };

        Assert.ThrowsExactly<InvalidOperationException>(() => new TestSubscriberServiceBus(
            OptionsX.Create(options),
            NullLogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>>.Instance));
    }

    [TestMethod]
    public void Publisher_Throws_WhenQueueNameIsMissing()
    {
        var options = new AzurePublisherOptions
        {
            FullyQualifiedNamespace = "identity.servicebus.windows.net"
        };

        Assert.ThrowsExactly<ArgumentNullException>(() => CreatePublisher(options));
    }

    [TestMethod]
    public void Publisher_AotConstructor_AllowsIdentityConfiguration()
    {
        var options = new AzurePublisherOptions
        {
            QueueName = "queue",
            FullyQualifiedNamespace = "identity.servicebus.windows.net"
        };

        var publisher = new PublisherServiceBus<TestMessage, AzurePublisherOptions>(
            OptionsX.Create(options),
            TestJsonContext.Default.TestMessage);

        Assert.IsNotNull(publisher);
    }

    [TestMethod]
    public void Subscriber_AotConstructor_AllowsIdentityConfiguration()
    {
        var options = new AzureSubscriberOptions
        {
            QueueName = "queue",
            FullyQualifiedNamespace = "identity.servicebus.windows.net"
        };

        var subscriber = new TestSubscriberServiceBusAot(
            OptionsX.Create(options),
            NullLogger<SubscriberServiceBus<TestMessage, AzureSubscriberOptions>>.Instance,
            TestJsonContext.Default.TestMessage);

        Assert.IsNotNull(subscriber);
    }

    private static PublisherServiceBus<TestMessage, AzurePublisherOptions> CreatePublisher(AzurePublisherOptions options)
#pragma warning disable IL2026, IL3050 // Reflection-based serialization path used in tests; AOT is not a test concern.
        => new(
            OptionsX.Create(options),
            NullLogger<PublisherServiceBus<TestMessage, AzurePublisherOptions>>.Instance);
#pragma warning restore IL2026, IL3050
}
