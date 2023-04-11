using ISynergy.Framework.MessageBus.Abstractions;
using Sample.MessageBus.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.MessageBus.Subscriber
{
    /// <summary>
    /// Class ApplicationAzure.
    /// </summary>
    public class Startup
    {
        private readonly ISubscriberServiceBus<TestDataModel> _messageBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public Startup(ISubscriberServiceBus<TestDataModel> messageBus)
        {
            _messageBus = messageBus;
        }

        /// <summary>
        /// run as an asynchronous operation.
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine("Azure implementation started...");

            CancellationTokenSource cancellationTokenSource = new();

            Task allReceives = Task.WhenAll(
                _messageBus.SubscribeToMessageBusAsync(cancellationTokenSource.Token));

            Console.WriteLine("Receiving messages...");
            Console.WriteLine("Wait for a minute or press any key to end the processing");

            await Task.WhenAll(
                Task.WhenAny(
                    Task.Run(Console.ReadKey),
                    Task.Delay(TimeSpan.FromSeconds(60))
                ).ContinueWith(async (t) =>
                {
                    await _messageBus.UnSubscribeFromMessageBusAsync();
                    cancellationTokenSource.Cancel();
                }),
                allReceives);
        }
    }
}
