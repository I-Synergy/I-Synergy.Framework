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
internal class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>, IAsyncDisposable
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

    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

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

    private async Task EnsureConnectionAsync()
    {
        if (_channel is not null) return;

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_channel is not null) return;

            var exchangeName = _option is PublisherOptions publisherOptions
                ? publisherOptions.ExchangeName
                : string.Empty;

            var exchangeType = _option is PublisherOptions opts
                ? opts.ExchangeType
                : global::RabbitMQ.Client.ExchangeType.Direct;

            var factory = new ConnectionFactory { Uri = new Uri(_option.ConnectionString) };
            _connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
            _channel = await _connection.CreateChannelAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(exchangeName))
                await _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Sends a message to the RabbitMQ queue.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    /// <param name="sessionId">Used as CorrelationId when not <see cref="Guid.Empty"/>.</param>
    /// <returns>Task.</returns>
    public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId)
    {
        Argument.IsNotNull(queueMessage);

        await EnsureConnectionAsync().ConfigureAwait(false);

        var exchangeName = _option is PublisherOptions publisherOptions
            ? publisherOptions.ExchangeName
            : string.Empty;

        var body = JsonSerializer.SerializeToUtf8Bytes(queueMessage);

        var props = new BasicProperties
        {
            ContentType = queueMessage.ContentType,
            Persistent = true
        };

        if (!string.IsNullOrEmpty(queueMessage.Tag))
            props.Type = queueMessage.Tag;

        if (sessionId != Guid.Empty)
            props.CorrelationId = sessionId.ToString();

        await _channel!.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: _option.QueueName,
            mandatory: false,
            basicProperties: props,
            body: body).ConfigureAwait(false);

        _logger.LogInformation("Published message to queue {QueueName}", _option.QueueName);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync().ConfigureAwait(false);

        if (_connection is not null)
            await _connection.DisposeAsync().ConfigureAwait(false);

        _semaphore.Dispose();
    }
}
