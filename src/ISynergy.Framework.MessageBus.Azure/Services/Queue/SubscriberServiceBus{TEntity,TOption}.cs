using Azure.Messaging.ServiceBus;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.Azure.Services.Queue;

/// <summary>
/// Message bus implementation on Azure Service Bus.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <typeparam name="TOption">The type of the t option.</typeparam>
internal abstract class SubscriberServiceBus<TEntity, TOption> : ISubscriberServiceBus<TEntity>
    where TOption : class, IQueueOption, new()
{
    /// <summary>
    /// The option
    /// </summary>
    protected readonly TOption _option;
    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// ServiceBus client.
    /// </summary>
    private ServiceBusClient? _serviceBusClient;

    /// <summary>
    /// ServiceBus processor.
    /// </summary>
    private ServiceBusProcessor? _serviceBusProcessor;

    /// <summary>
    /// Constructor of service bus.
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

    /// <summary>
    /// Processes the data asynchronous.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public abstract Task<bool> ProcessDataAsync(TEntity queueMessage);

    /// <summary>
    /// Validates the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public abstract bool ValidateMessage(TEntity message);

    /// <summary>
    /// Subscriber to message queue.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public virtual Task SubscribeToMessageBusAsync(CancellationToken cancellationToken = default)
    {
        _serviceBusClient = new ServiceBusClient(_option.ConnectionString);

        // create the options to use for configuring the processor
        var options = new ServiceBusProcessorOptions
        {
            // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
            // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
            // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
            AutoCompleteMessages = false,
            // I can also allow for multi-threading
            MaxConcurrentCalls = 2,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        };

        // create a processor that we can use to process the messages
        _serviceBusProcessor = _serviceBusClient.CreateProcessor(_option.QueueName, options);

        // configure the message and error handler to use
        _serviceBusProcessor.ProcessMessageAsync += MessageHandlerAsync;
        _serviceBusProcessor.ProcessErrorAsync += ErrorHandlerAsync;

        // start processing
        return _serviceBusProcessor.StartProcessingAsync(cancellationToken);
    }

    /// <summary>
    /// Unsubscribe from messagebus and closes connection.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task UnSubscribeFromMessageBusAsync(CancellationToken cancellationToken = default)
    {
        if (_serviceBusProcessor is not null)
            await _serviceBusProcessor.DisposeAsync();

        if (_serviceBusClient is not null)
            await _serviceBusClient.DisposeAsync();
    }

    private async Task ErrorHandlerAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Message handler encountered an exception");
        _logger.LogDebug($"- Error Source: {arg.ErrorSource}");
        _logger.LogDebug($"- Fully Qualified Namespace: {arg.FullyQualifiedNamespace}");
        _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
        _logger.LogDebug($"- Exception: {arg.Exception.ToString()}");

        if (_serviceBusProcessor is not null)
            await _serviceBusProcessor.CloseAsync();
    }

    private async Task MessageHandlerAsync(ProcessMessageEventArgs arg)
    {
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body.ToArray());
        var data = JsonSerializer.Deserialize<TEntity>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (data is not null && ValidateMessage(data))
        {
            if (await ProcessDataAsync(data).ConfigureAwait(false))
            {
                // Now that we're done with "processing" the message, we tell the broker about that being the
                // case. The MessageReceiver.CompleteAsync operation will settle the message transfer with 
                // the broker and remove it from the broker.
                await arg.CompleteMessageAsync(message)
                    .ConfigureAwait(false);
            }
            else
            {
                // If the message does not meet our processing criteria, we will dead letter it, meaning
                // it is put into a special queue for handling defective messages. The broker will automatically
                // dead letter the message, if delivery has been attempted too many times. 
                await arg.DeadLetterMessageAsync(message)
                    .ConfigureAwait(false);
            }
        }
    }
}
