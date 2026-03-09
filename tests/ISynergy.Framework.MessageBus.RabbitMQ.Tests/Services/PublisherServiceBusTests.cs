using ISynergy.Framework.MessageBus.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Tests.Fixtures;
using ISynergy.Framework.MessageBus.Services.Queue;
using Microsoft.Extensions.Logging.Abstractions;
using MsOptions = Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Tests.Services;

[TestClass]
public class PublisherServiceBusTests
{
    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static TestPublisherServiceBus CreatePublisher(
        string connectionString = "amqp://localhost",
        string queueName = "test-queue")
    {
        var options = MsOptions.Options.Create(new PublisherOptions
        {
            ConnectionString = connectionString,
            QueueName = queueName
        });
        var logger = NullLogger<PublisherServiceBus<TestMessage, PublisherOptions>>.Instance;
        return new TestPublisherServiceBus(options, logger);
    }

    // ------------------------------------------------------------------ //
    // Constructor — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Constructor_EmptyConnectionString_ThrowsArgumentNullException()
    {
        var options = MsOptions.Options.Create(new PublisherOptions
        {
            ConnectionString = "",
            QueueName = "test-queue"
        });
        var logger = NullLogger<PublisherServiceBus<TestMessage, PublisherOptions>>.Instance;

        Assert.Throws<ArgumentNullException>(() =>
            new TestPublisherServiceBus(options, logger));
    }

    [TestMethod]
    public void Constructor_EmptyQueueName_ThrowsArgumentNullException()
    {
        var options = MsOptions.Options.Create(new PublisherOptions
        {
            ConnectionString = "amqp://localhost",
            QueueName = ""
        });
        var logger = NullLogger<PublisherServiceBus<TestMessage, PublisherOptions>>.Instance;

        Assert.Throws<ArgumentNullException>(() =>
            new TestPublisherServiceBus(options, logger));
    }

    [TestMethod]
    public void Constructor_ValidOptions_DoesNotThrow()
    {
        var publisher = CreatePublisher();
        Assert.IsNotNull(publisher);
    }

    // ------------------------------------------------------------------ //
    // SendMessageAsync — argument validation
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task SendMessageAsync_NullMessage_ThrowsArgumentNullException()
    {
        await using var publisher = CreatePublisher();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            publisher.SendMessageAsync(null!, Guid.Empty));
    }

    // ------------------------------------------------------------------ //
    // DisposeAsync — idempotent
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_DoesNotThrow()
    {
        var publisher = CreatePublisher();
        await publisher.DisposeAsync();
        await publisher.DisposeAsync();
    }
}
