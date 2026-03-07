using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;

/// <summary>
/// Message bus subscriber implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
internal abstract class SubscriberServiceBus<TEntity, TOption> : ISubscriberServiceBus<TEntity>, IAsyncDisposable
    where TOption : class, IQueueOption, new()
{
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

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
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriberServiceBus{TEntity, TOption}"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
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
            : global::RabbitMQ.Client.ExchangeType.Direct;

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
            var data = JsonSerializer.Deserialize<TEntity>(body, s_jsonOptions);

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
