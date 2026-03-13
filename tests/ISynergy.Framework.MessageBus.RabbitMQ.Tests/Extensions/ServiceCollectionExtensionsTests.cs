using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.RabbitMQ.Extensions;
using ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                ["RabbitMQPublisherOptions:ConnectionString"] = "amqp://localhost",
                ["RabbitMQPublisherOptions:QueueName"] = "test-queue"
            })
            .Build();

    private static IConfiguration CreateSubscriberConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMQSubscriberOptions:ConnectionString"] = "amqp://localhost",
                ["RabbitMQSubscriberOptions:QueueName"] = "test-queue"
            })
            .Build();

    // ------------------------------------------------------------------ //
    // AddMessageBusRabbitMQPublishIntegration
    // ------------------------------------------------------------------ //

    [TestMethod]
#pragma warning disable IL2026, IL3050 // Reflection-based overload used in tests; AOT not a test concern.
    public void AddMessageBusRabbitMQPublishIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddRabbitMQMessageBusPublishIntegration<TestMessage>(CreatePublisherConfig());

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_RegistersIPublisherServiceBus()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusPublishIntegration<TestMessage>(CreatePublisherConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusPublishIntegration<TestMessage>(CreatePublisherConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQPublishIntegration_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusPublishIntegration<TestMessage>(CreatePublisherConfig());
        services.AddRabbitMQMessageBusPublishIntegration<TestMessage>(CreatePublisherConfig());

        var count = services.Count(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>));

        // TryAddSingleton: only one registration survives.
        Assert.AreEqual(1, count);
    }
#pragma warning restore IL2026, IL3050

    // ------------------------------------------------------------------ //
    // AddMessageBusRabbitMQSubscribeIntegration
    // ------------------------------------------------------------------ //

    [TestMethod]
#pragma warning disable IL2026, IL3050 // Reflection-based overload used in tests; AOT not a test concern.
    public void AddMessageBusRabbitMQSubscribeIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddRabbitMQMessageBusSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_RegistersISubscriberServiceBus()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [TestMethod]
    public void AddMessageBusRabbitMQSubscribeIntegration_CalledTwice_DoesNotDuplicate()
    {
        var services = new ServiceCollection();
        services.AddRabbitMQMessageBusSubscribeIntegration<TestMessage>(CreateSubscriberConfig());
        services.AddRabbitMQMessageBusSubscribeIntegration<TestMessage>(CreateSubscriberConfig());

        var count = services.Count(d => d.ServiceType == typeof(ISubscriberServiceBus<TestMessage>));

        Assert.AreEqual(1, count);
    }
#pragma warning restore IL2026, IL3050
}
