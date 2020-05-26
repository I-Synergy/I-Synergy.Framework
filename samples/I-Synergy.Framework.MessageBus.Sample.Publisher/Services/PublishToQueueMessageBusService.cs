using ISynergy.Framework.MessageBus.Azure.Queue;
using ISynergy.Framework.MessageBus.Sample.Models;
using ISynergy.Framework.MessageBus.Sample.Publisher.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.MessageBus.Sample.Publisher.Services
{
    /// <summary>
    /// Class TestQueueMessageBusService.
    /// Implements the <see cref="PublisherServiceBus{TestDataModel, TestQueueOptions}" />
    /// </summary>
    /// <seealso cref="PublisherServiceBus{TestDataModel, TestQueueOptions}" />
    public class PublishToQueueMessageBusService : PublisherServiceBus<TestDataModel, TestQueueOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishToQueueMessageBusService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public PublishToQueueMessageBusService(IOptions<TestQueueOptions> options, ILogger<PublishToQueueMessageBusService> logger)
            : base(options, logger)
        {
        }
    }
}
