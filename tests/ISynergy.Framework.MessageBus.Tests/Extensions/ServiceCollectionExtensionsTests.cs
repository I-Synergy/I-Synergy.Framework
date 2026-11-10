using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Extensions;
using ISynergy.Framework.MessageBus.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.MessageBus.Tests.Extensions;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>. In particular the topic-flavoured
/// registration helpers, which simply forward to the queue-flavoured ones.
/// </summary>
[TestClass]
public class ServiceCollectionExtensionsTests
{
    /// <summary>Minimal publisher implementation used for the registration assertions.</summary>
    private sealed class TestPublisher : IPublisherServiceBus<TestMessage>
    {
        public Task SendMessageAsync(TestMessage queueMessage, Guid sessionId) => Task.CompletedTask;
    }

    [TestMethod]
    public void AddPublishingTopicMessageBus_RegistersPublisherAndImplementationAsSingleton()
    {
        var services = new ServiceCollection();

        var returned = services.AddPublishingTopicMessageBus<TestMessage, TestPublisher>();

        Assert.AreSame(services, returned, "The extension must return the same collection for chaining");
        Assert.IsTrue(
            services.Any(d => d.ServiceType == typeof(TestPublisher)),
            "The concrete implementation should be registered.");
        Assert.IsTrue(
            services.Any(d => d.ServiceType == typeof(IPublisherServiceBus<TestMessage>)),
            "The abstraction should be registered.");
        Assert.IsTrue(
            services.Single(d => d.ServiceType == typeof(TestPublisher)).Lifetime == ServiceLifetime.Singleton,
            "The registration should be a singleton.");
    }

    [TestMethod]
    public void AddPublishingTopicMessageBus_ResolvesAbstractionToImplementation()
    {
        var services = new ServiceCollection();
        services.AddPublishingTopicMessageBus<TestMessage, TestPublisher>();

        using var provider = services.BuildServiceProvider();
        var publisher = provider.GetRequiredService<IPublisherServiceBus<TestMessage>>();

        Assert.IsInstanceOfType<TestPublisher>(publisher);
        Assert.IsNotNull(provider.GetRequiredService<TestPublisher>());
    }
}
