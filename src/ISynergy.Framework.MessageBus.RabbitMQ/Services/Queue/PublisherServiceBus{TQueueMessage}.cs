using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.MessageBus.Services.Queue;

/// <summary>
/// Message bus publisher implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TQueueMessage">The type of the queue message.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
public class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>, IAsyncDisposable
    where TQueueMessage : class, IBaseMessage
    where TOption : class, IQueueOption, new()
{
    private readonly TOption _option;
    private readonly ILogger _logger;
    private readonly JsonTypeInfo<TQueueMessage>? _jsonTypeInfo;

    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance of <see cref="PublisherServiceBus{TQueueMessage, TOption}"/> using
    /// AOT-safe source-generated JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TQueueMessage"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>. Required for Native AOT publishing.
    /// </param>
    public PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger,
        JsonTypeInfo<TQueueMessage> jsonTypeInfo)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
        _jsonTypeInfo = jsonTypeInfo;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PublisherServiceBus{TQueueMessage, TOption}"/> using
    /// reflection-based JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="JsonSerializer"/> path and is not compatible with
    /// Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/> for AOT scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Serialization uses reflection. Pass a JsonTypeInfo<TQueueMessage> for AOT compatibility.")]
    [RequiresDynamicCode("Serialization uses dynamic code generation. Pass a JsonTypeInfo<TQueueMessage> for AOT compatibility.")]
    public PublisherServiceBus(
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
                : RabbitMQ.Client.ExchangeType.Direct;

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

        var body = _jsonTypeInfo is not null
            ? JsonSerializer.SerializeToUtf8Bytes(queueMessage, _jsonTypeInfo)
            : JsonSerializer.SerializeToUtf8Bytes(queueMessage);

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
