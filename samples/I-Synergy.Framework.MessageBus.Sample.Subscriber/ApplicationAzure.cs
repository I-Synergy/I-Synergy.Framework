using System;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Sample.Models;

namespace ISynergy.Framework.MessageBus.Sample.Subscriber
{
    /// <summary>
    /// Class ApplicationAzure.
    /// </summary>
    public class ApplicationAzure
    {
        private readonly ISubscriberServiceBus<TestDataModel> _messageBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAzure"/> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public ApplicationAzure(ISubscriberServiceBus<TestDataModel> messageBus)
        {
            _messageBus = messageBus;
        }

        /// <summary>
        /// run as an asynchronous operation.
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine("Azure implementation started...");

            var sessionId = Guid.NewGuid();
            var cancellationTokenSource = new CancellationTokenSource();

            var allReceives = Task.WhenAll(
                _messageBus.SubscribeToMessagesAsync(cancellationTokenSource.Token));

            Console.WriteLine("Receiving messages...");

            await Task.WhenAll(
                Task.WhenAny(
                    Task.Run(() => Console.ReadKey()),
                    Task.Delay(TimeSpan.FromSeconds(60))
                ).ContinueWith((t) => cancellationTokenSource.Cancel()),
                allReceives);
        }
    }
}
