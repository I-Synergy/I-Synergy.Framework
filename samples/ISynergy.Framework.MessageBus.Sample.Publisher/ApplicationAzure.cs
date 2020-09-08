using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Sample.Models;

namespace ISynergy.Framework.MessageBus.Sample
{
    /// <summary>
    /// Class ApplicationAzure.
    /// </summary>
    public class ApplicationAzure
    {
        private readonly IPublisherServiceBus<TestDataModel> _messageBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAzure"/> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public ApplicationAzure(IPublisherServiceBus<TestDataModel> messageBus)
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
            var publishTasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();

                Console.WriteLine($"Sending message with id: {id}");

                publishTasks.Add(_messageBus.SendMessageAsync(new TestDataModel(Enumerations.QueueMessageActions.Add, $"Message {i} with id: {i}"), sessionId));
            }

            var allPublishes = Task.WhenAll(publishTasks);

            Console.WriteLine("Publishing messages...");
            Console.WriteLine("Press any key to exit application.");

            await Task.WhenAll(
                Task.WhenAny(
                    Task.Run(() => Console.ReadKey()),
                    Task.Delay(TimeSpan.FromSeconds(10))
                ).ContinueWith((t) => cancellationTokenSource.Cancel()),
                allPublishes);
        }
    }
}
