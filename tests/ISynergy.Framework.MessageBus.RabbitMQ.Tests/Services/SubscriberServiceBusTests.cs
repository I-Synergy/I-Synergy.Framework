using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;
using ISynergy.Framework.MessageBus.Services.Queue;
using Microsoft.Extensions.Logging.Abstractions;
using MsOptions = Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Services;

[TestClass]
public class SubscriberServiceBusTests
{
    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static TestSubscriberServiceBus CreateSubscriber(
        string connectionString = "amqp://localhost",
        string queueName = "test-queue")
    {
        var options = MsOptions.Options.Create(new SubscriberOptions
        {
            ConnectionString = connectionString,
            QueueName = queueName
        });
        var logger = NullLogger<SubscriberServiceBus<TestMessage, SubscriberOptions>>.Instance;
        return new TestSubscriberServiceBus(options, logger);
    }

    // ------------------------------------------------------------------ //
    // Constructor — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_EmptyConnectionString_ThrowsArgumentNullException()
    {
        var options = MsOptions.Options.Create(new SubscriberOptions
        {
            ConnectionString = "",
            QueueName = "test-queue"
        });
        var logger = NullLogger<SubscriberServiceBus<TestMessage, SubscriberOptions>>.Instance;

        Assert.Throws<ArgumentNullException>(() =>
            new TestSubscriberServiceBus(options, logger));
    }

    [TestMethod]
    public void Constructor_EmptyQueueName_ThrowsArgumentNullException()
    {
        var options = MsOptions.Options.Create(new SubscriberOptions
        {
            ConnectionString = "amqp://localhost",
            QueueName = ""
        });
        var logger = NullLogger<SubscriberServiceBus<TestMessage, SubscriberOptions>>.Instance;

        Assert.Throws<ArgumentNullException>(() =>
            new TestSubscriberServiceBus(options, logger));
    }

    [TestMethod]
    public void Constructor_ValidOptions_DoesNotThrow()
    {
        var subscriber = CreateSubscriber();
        Assert.IsNotNull(subscriber);
    }

    // ------------------------------------------------------------------ //
    // Abstract member implementations — test subclass behaviour
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task ProcessDataAsync_AnyMessage_ReturnsTrue()
    {
        var subscriber = CreateSubscriber();
        var message = new TestMessage { Content = "hello" };

        var result = await subscriber.ProcessDataAsync(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ValidateMessage_NonNullMessage_ReturnsTrue()
    {
        var subscriber = CreateSubscriber();
        var message = new TestMessage();

        Assert.IsTrue(subscriber.ValidateMessage(message));
    }

    [TestMethod]
    public void ValidateMessage_NullMessage_ReturnsFalse()
    {
        var subscriber = CreateSubscriber();

        Assert.IsFalse(subscriber.ValidateMessage(null!));
    }

    // ------------------------------------------------------------------ //
    // DisposeAsync — idempotent
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_DoesNotThrow()
    {
        var subscriber = CreateSubscriber();
        await subscriber.DisposeAsync();

        // Second dispose must be a no-op (_disposed guard in the service).
        await subscriber.DisposeAsync();
    }
}
