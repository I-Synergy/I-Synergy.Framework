using System;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Azure.Queue;
using ISynergy.Framework.MessageBus.Sample.Models;
using ISynergy.Framework.MessageBus.Sample.Subscriber.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.Sample.Subscriber.Services
{
    /// <summary>
    /// Class TestQueueMessageBusService.
    /// Implements the <see cref="SubscriberServiceBus{TestDataModel, TestQueueOptions}" />
    /// </summary>
    /// <seealso cref="SubscriberServiceBus{TestDataModel, TestQueueOptions}" />
    public class SubscribeToQueueMessageBusService : SubscriberServiceBus<TestDataModel, TestQueueOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeToQueueMessageBusService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public SubscribeToQueueMessageBusService(IOptions<TestQueueOptions> options, ILogger<SubscribeToQueueMessageBusService> logger)
            : base(options, logger)
        {
        }

        /// <summary>
        /// Processes the data asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        protected override Task<bool> ProcessDataAsync(TestDataModel data)
        {
            Console.WriteLine($"Message received with id: {data.Action}");
            Console.WriteLine(data.Data);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Validates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool ValidateMessage(TestDataModel message)
        {
            Argument.IsNotNull(nameof(message), message);

            if(message.Action == Enumerations.QueueMessageActions.Add)
            {
                return true;
            }

            return false;
        }
    }
}
