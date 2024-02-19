using ISynergy.Framework.MessageBus.Abstractions;
using Sample.MessageBus.Models;

namespace Sample.MessageBus.Subscriber;

/// <summary>
/// Class ApplicationAzure.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Startup"/> class.
/// </remarks>
/// <param name="messageBus">The message bus.</param>
public class Startup(ISubscriberServiceBus<TestDataModel> messageBus)
{
    /// <summary>
    /// run as an asynchronous operation.
    /// </summary>
    public async Task RunAsync()
    {
        Console.WriteLine("Azure implementation started...");

        using CancellationTokenSource cancellationTokenSource = new();

        Task allReceives = Task.WhenAll(
            messageBus.SubscribeToMessageBusAsync(cancellationTokenSource.Token));

        Console.WriteLine("Receiving messages...");
        Console.WriteLine("Wait for a minute or press any key to end the processing");

        await Task.WhenAll(
            Task.WhenAny(
                Task.Run(Console.ReadKey),
                Task.Delay(TimeSpan.FromSeconds(60))
            ).ContinueWith(async (t) =>
            {
                await messageBus.UnSubscribeFromMessageBusAsync();
                cancellationTokenSource.Cancel();
            }),
            allReceives);
    }
}
