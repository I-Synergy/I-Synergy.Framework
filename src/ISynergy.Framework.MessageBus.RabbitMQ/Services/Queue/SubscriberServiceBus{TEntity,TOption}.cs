using ISynergy.Framework.Core.Serializers;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.MessageBus.Services.Queue;

/// <summary>
/// Message bus subscriber implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
public abstract class SubscriberServiceBus<TEntity, TOption> : ISubscriberServiceBus<TEntity>, IAsyncDisposable
    where TOption : class, IQueueOption, new()
{
    /// <summary>
    /// The queue option.
    /// </summary>
    protected readonly TOption _option;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// The JSON type info used for AOT-safe deserialization.
    /// </summary>
    protected readonly JsonTypeInfo<TEntity>? _jsonTypeInfo;

    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriberServiceBus{TEntity, TOption}"/> using
    /// AOT-safe source-generated JSON deserialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TEntity"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>. Required for Native AOT publishing.
    /// </param>
    protected SubscriberServiceBus(
        IOptions<TOption> options,
        ILogger<SubscriberServiceBus<TEntity, TOption>> logger,
        JsonTypeInfo<TEntity> jsonTypeInfo)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
        _jsonTypeInfo = jsonTypeInfo;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriberServiceBus{TEntity, TOption}"/> using
    /// reflection-based JSON deserialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="JsonSerializer"/> path and is not compatible with
    /// Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/> for AOT scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Deserialization uses reflection. Pass a JsonTypeInfo<TEntity> for AOT compatibility.")]
    [RequiresDynamicCode("Deserialization uses dynamic code generation. Pass a JsonTypeInfo<TEntity> for AOT compatibility.")]
    protected SubscriberServiceBus(
        IOptions<TOption> options,
        ILogger<SubscriberServiceBus<TEntity, TOption>> logger)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
    }

    /// <inheritdoc />
    public abstract Task<bool> ProcessDataAsync(TEntity queueMessage);

    /// <inheritdoc />
    public abstract bool ValidateMessage(TEntity message);

    /// <inheritdoc />
    public virtual async Task SubscribeToMessageBusAsync(CancellationToken cancellationToken = default)
    {
        var exchangeName = _option is SubscriberOptions subscriberOptions
            ? subscriberOptions.ExchangeName
            : string.Empty;

        var exchangeType = _option is SubscriberOptions subscriberOpts
            ? subscriberOpts.ExchangeType
            : RabbitMQ.Client.ExchangeType.Direct;

        var factory = new ConnectionFactory { Uri = new Uri(_option.ConnectionString) };

        _connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.QueueDeclareAsync(
            queue: _option.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await _channel.QueueBindAsync(_option.QueueName, exchangeName, _option.QueueName, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken).ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _option.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Subscribed to RabbitMQ queue {QueueName}", _option.QueueName);
    }

    /// <inheritdoc />
    public virtual async Task UnSubscribeFromMessageBusAsync(CancellationToken cancellationToken = default)
    {
        await DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            if (_channel is not null)
                await _channel.CloseAsync().ConfigureAwait(false);

            if (_connection is not null)
                await _connection.CloseAsync().ConfigureAwait(false);
        }
        finally
        {
            if (_channel is not null)
                await _channel.DisposeAsync().ConfigureAwait(false);

            if (_connection is not null)
                await _connection.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            var body = args.Body.ToArray();

            var data = _jsonTypeInfo is not null
                ? JsonSerializer.Deserialize(body, _jsonTypeInfo)
                : JsonSerializer.Deserialize<TEntity>(body, DefaultJsonSerializers.Web);

            if (data is not null && ValidateMessage(data))
            {
                if (await ProcessDataAsync(data).ConfigureAwait(false))
                {
                    await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false).ConfigureAwait(false);
                }
                else
                {
                    await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
                }
            }
            else
            {
                _logger.LogWarning("Message validation failed, dead-lettering message");
                await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RabbitMQ message from queue {QueueName}", _option.QueueName);
            await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
        }
    }
}
