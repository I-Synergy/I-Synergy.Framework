using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Enumerations;
using Sample.MessageBus.Models;

namespace Sample.MessageBus.Publisher;

/// <summary>
/// Class ApplicationAzure.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationAzure"/> class.
/// </remarks>
/// <param name="messageBus">The message bus.</param>
public class ApplicationAzure(IPublisherServiceBus<TestDataModel> messageBus)
{
    private readonly IPublisherServiceBus<TestDataModel> _messageBus = messageBus;

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
