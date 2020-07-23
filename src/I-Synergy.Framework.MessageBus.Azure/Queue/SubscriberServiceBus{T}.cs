using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ISynergy.Framework.MessageBus.Azure.Queue
{
    /// <summary>
    /// Message bus implementation on Azure Service Bus.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TOption">The type of the t option.</typeparam>
    public abstract class SubscriberServiceBus<TEntity, TOption> : ISubscriberServiceBus<TEntity>
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
        /// Constructor of service bus.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        protected SubscriberServiceBus(IOptions<TOption> options, ILogger<SubscriberServiceBus<TEntity, TOption>> logger)
        {
            _option = options.Value;

            Argument.IsNotNullOrEmpty(nameof(_option.ConnectionString), _option.ConnectionString);
            Argument.IsNotNullOrEmpty(nameof(_option.QueueName), _option.QueueName);

            _logger = logger;
        }

        /// <summary>
        /// Processes the data asynchronous.
        /// </summary>
        /// <param name="queueMessage">The queue message.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        protected abstract Task<bool> ProcessDataAsync(TEntity queueMessage);


        /// <summary>
        /// Validates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected abstract bool ValidateMessage(TEntity message);

        /// <summary>
        /// Subscriber to message queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public virtual async Task SubscribeToMessagesAsync(CancellationToken cancellationToken = default)
        {
            // First, we create a generic MessageReceiver for the queue. This class can 
            // also receive from a topic subscription when given the correct path. 
            // 
            // Note that the receiver is created the with the ReceiveMode.PeekLock receive mode. 
            // This mode will pass the message to the receiver while the broker maintains 
            // a lock on message and hold on to the message. If the message has not been 
            // completed, deferred, dead lettered, or abandoned during the lock timeout period, 
            // the message will again appear in the queue (or in the topic subscription) 
            // for retrieval.
            //
            // This is different from the ReceiveMode.ReceiveAndDelete alternative where the 
            // message has been deleted as it arrives at the receiver. Here, the message
            // is either completed or dead lettered as you will see further below.
            //var receiver = new MessageReceiver(_option.ConnectionString, _option.QueueName, ReceiveMode.PeekLock);
            var receiver = new QueueClient(_option.ConnectionString, _option.QueueName, ReceiveMode.PeekLock);

            var doneReceiving = new TaskCompletionSource<bool>();

            // If the cancellation token is triggered, we close the receiver, which will trigger 
            // the receive operation below to return null as the receiver closes.
            cancellationToken.Register(
                async () =>
                {
                    await receiver.CloseAsync().ConfigureAwait(false);
                    doneReceiving.SetResult(true);
                });

            // register the RegisterMessageHandler callback
            receiver.RegisterMessageHandler(
                async (message, token) =>
                {
                    var body = Encoding.UTF8.GetString(message.Body);
                    var data = JsonConvert.DeserializeObject<TEntity>(body);

                    if (ValidateMessage(data))
                    {
                        if (await ProcessDataAsync(data).ConfigureAwait(false))
                        {
                            // Now that we're done with "processing" the message, we tell the broker about that being the
                            // case. The MessageReceiver.CompleteAsync operation will settle the message transfer with 
                            // the broker and remove it from the broker.
                            await receiver.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                        }
                        else
                        {
                            // If the message does not meet our processing criteria, we will dead letter it, meaning
                            // it is put into a special queue for handling defective messages. The broker will automatically
                            // dead letter the message, if delivery has been attempted too many times. 
                            await receiver.DeadLetterAsync(message.SystemProperties.LockToken).ConfigureAwait(false); //, "ProcessingError", "Don't know what to do with this message");
                        }
                    }
                },
                new MessageHandlerOptions((e) => ExceptionReceivedHandler(e)) { AutoComplete = false, MaxConcurrentCalls = 1 });

            await doneReceiving.Task;
        }

        /// <summary>
        /// Exceptions the received handler.
        /// </summary>
        /// <param name="exceptionReceivedEventArgs">The <see cref="ExceptionReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns>Task.</returns>
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogDebug($"- Endpoint: {context.Endpoint}");
            _logger.LogDebug($"- Entity Path: {context.EntityPath}");
            _logger.LogDebug($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}
