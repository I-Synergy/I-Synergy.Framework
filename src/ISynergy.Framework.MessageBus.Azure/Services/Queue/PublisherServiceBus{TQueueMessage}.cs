using Azure.Messaging.ServiceBus;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.MessageBus.Azure.Services.Queue
{
    /// <summary>
    /// Message bus implementation on Azure Service Bus.
    /// </summary>
    /// <typeparam name="TQueueMessage">The type of the t queue message.</typeparam>
    /// <typeparam name="TOption">The type of the t option.</typeparam>
    internal class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>
        where TQueueMessage : class, IBaseMessage
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
        protected PublisherServiceBus(IOptions<TOption> options, ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger)
        {
            _option = options.Value;

            Argument.IsNotNullOrEmpty(_option.ConnectionString);
            Argument.IsNotNullOrEmpty(_option.QueueName);

            _logger = logger;
        }

        /// <summary>
        /// Send message to queue.
        /// If sessionId is provided, messages retrieval is based on FIFO.
        /// If sessionId is Guid.Empty, sessionId is not added and retrieval is random.
        /// </summary>
        /// <param name="queueMessage">The queue message.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentException">Entity should be type of IQueueMessage{TModel} instead of {queueMessage.GetType().FullName}</exception>
        public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId)
        {
            if (queueMessage is TQueueMessage model)
            {
                await using var client = new ServiceBusClient(_option.ConnectionString);
                var sender = client.CreateSender(_option.QueueName);

                var body = JsonSerializer.Serialize(queueMessage);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(body));

                //If a session id is provided, add message to session.
                if (sessionId != Guid.Empty)
                    message.SessionId = sessionId.ToString();

                //If a ContentType is provided, add ContentType to session.
                if (!string.IsNullOrEmpty(model.ContentType))
                    message.ContentType = model.ContentType;

                //If a Tag is provided, add label to session.
                if (!string.IsNullOrEmpty(model.Tag))
                    message.Subject = model.Tag;

                await sender.SendMessageAsync(message);
            }
            else
            {
                throw new ArgumentException("Entity should be type of IQueueMessage<TModel>");
            }
        }
    }
}
