using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Enumerations;
using Sample.MessageBus.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.MessageBus.Publisher;

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

        Guid sessionId = Guid.NewGuid();
        CancellationTokenSource cancellationTokenSource = new();
        List<Task> publishTasks = [];

        for (int i = 0; i < 10; i++)
        {
            Guid id = Guid.NewGuid();

            Console.WriteLine($"Sending message with id: {id}");

            publishTasks.Add(_messageBus.SendMessageAsync(new TestDataModel(QueueMessageActions.Add, $"Message {i} with id: {i}"), sessionId));
        }

        Task allPublishes = Task.WhenAll(publishTasks);

        Console.WriteLine("Publishing messages...");
        Console.WriteLine("Press any key to exit application.");

        await Task.WhenAll(
            Task.WhenAny(
                Task.Run(Console.ReadKey),
                Task.Delay(TimeSpan.FromSeconds(10))
            ).ContinueWith((t) => cancellationTokenSource.Cancel()),
            allPublishes);
    }
}
