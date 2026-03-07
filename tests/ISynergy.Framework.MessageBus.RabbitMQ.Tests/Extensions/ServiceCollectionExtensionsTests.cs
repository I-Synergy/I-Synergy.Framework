using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.RabbitMQ.Extensions;
using ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static IConfiguration CreatePublisherConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["PublisherOptions:ConnectionString"] = "amqp://localhost",
                ["PublisherOptions:QueueName"] = "test-queue"
            })
            .Build();

    private static IConfiguration CreateSubscriberConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SubscriberOptions:ConnectionString"] = "amqp://localhost",
                ["SubscriberOptions:QueueName"] = "test-queue"
            })
            .Build();

    // ------------------------------------------------------------------ //
    // AddMessageBusRabbitMQPublishIntegration
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddMessageBusRabbitMQPublishIntegration<TestMessage>(CreatePublisherConfig());

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_RegistersIPublisherServiceBus()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQPublishIntegration<TestMessage>(CreatePublisherConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQPublishIntegration<TestMessage>(CreatePublisherConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQPublishIntegration<TestMessage>(CreatePublisherConfig());
        services.AddMessageBusRabbitMQPublishIntegration<TestMessage>(CreatePublisherConfig());

        var count = services.Count(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        // TryAddSingleton: only one registration survives.
        Assert.AreEqual(1, count);
    }

    // ------------------------------------------------------------------ //
    // AddMessageBusRabbitMQSubscribeIntegration
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddMessageBusRabbitMQSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_RegistersISubscriberServiceBus()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();
        services.AddMessageBusRabbitMQSubscribeIntegration<TestMessage>(CreateSubscriberConfig());
        services.AddMessageBusRabbitMQSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var count = services.Count(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.AreEqual(1, count);
    }
}
