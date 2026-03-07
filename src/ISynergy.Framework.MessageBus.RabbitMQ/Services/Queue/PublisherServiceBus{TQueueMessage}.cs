using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;

/// <summary>
/// Message bus publisher implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TQueueMessage">The type of the queue message.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
internal class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>
    where TQueueMessage : class, IBaseMessage
    where TOption : class, IQueueOption, new()
{
    /// <summary>
    /// The option.
    /// </summary>
    protected readonly TOption _option;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="PublisherServiceBus{TQueueMessage, TOption}"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    protected PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
    }

    /// <summary>
    /// Sends a message to the RabbitMQ queue.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    /// <param name="sessionId">Used as CorrelationId when not <see cref="Guid.Empty"/>.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Thrown when queueMessage is not a valid IBaseMessage.</exception>
    public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId)
    {
        if (queueMessage is not { } model)
            throw new ArgumentException("Entity should be type of IQueueMessage<TModel>");

        var exchangeName = _option is PublisherOptions publisherOptions
            ? publisherOptions.ExchangeName
            : string.Empty;

        var factory = new ConnectionFactory { Uri = new Uri(_option.ConnectionString) };

        await using var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
        await using var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true).ConfigureAwait(false);

        var body = JsonSerializer.SerializeToUtf8Bytes(queueMessage);

        var props = new BasicProperties
        {
            ContentType = model.ContentType,
            Persistent = true
        };

        if (!string.IsNullOrEmpty(model.Tag))
            props.Type = model.Tag;

        if (sessionId != Guid.Empty)
            props.CorrelationId = sessionId.ToString();

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: _option.QueueName,
            mandatory: false,
            basicProperties: props,
            body: body).ConfigureAwait(false);

        _logger.LogInformation("Published message to queue {QueueName}", _option.QueueName);
    }
}
