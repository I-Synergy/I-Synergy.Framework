# Message Queue Integration (Azure Service Bus)

The publisher, the background-service consumer with dead-letter handling, and the DI registration. Load this when sending or receiving events over Azure Service Bus.

## Message Queue Integration (Azure Service Bus)

### Send Messages
```csharp
// File: {ApplicationName}.Infrastructure.Messaging/ServiceBusMessagePublisher.cs
namespace {ApplicationName}.Infrastructure.Messaging;

using Azure.Messaging.ServiceBus;
using System.Text.Json;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}

public class ServiceBusMessagePublisher(
    ServiceBusClient serviceBusClient,
    ILogger<ServiceBusMessagePublisher> logger
) : IMessagePublisher
{
    public async Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var queueName = GetQueueName<T>();
        var sender = serviceBusClient.CreateSender(queueName);

        try
        {
            var json = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                Subject = typeof(T).Name
            };

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            logger.LogInformation(
                "Published message {MessageType} to queue {QueueName}",
                typeof(T).Name, queueName);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to publish message {MessageType} to queue {QueueName}",
                typeof(T).Name, queueName);
            throw;
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }

    private static string GetQueueName<T>() => typeof(T).Name.ToLowerInvariant();
}
```

### Receive Messages (Background Service)
```csharp
// File: {ApplicationName}.Infrastructure.Messaging/ServiceBusMessageConsumer.cs
namespace {ApplicationName}.Infrastructure.Messaging;

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

public class ServiceBusMessageConsumer(
    ServiceBusClient serviceBusClient,
    IServiceProvider serviceProvider,
    ILogger<ServiceBusMessageConsumer> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processor = serviceBusClient.CreateProcessor(
            "budget-events",
            new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false
            });

        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);

        logger.LogInformation("Service Bus message consumer started");

        // Wait until cancellation
        await Task.Delay(Timeout.Infinite, stoppingToken);

        await processor.StopProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var messageBody = args.Message.Body.ToString();

        logger.LogDebug(
            "Processing message {MessageId} from queue",
            args.Message.MessageId);

        try
        {
            // Deserialize based on message subject
            var messageType = args.Message.Subject;

            await ProcessMessageByTypeAsync(messageType, messageBody, args.CancellationToken);

            // Complete the message
            await args.CompleteMessageAsync(args.Message);

            logger.LogInformation(
                "Successfully processed message {MessageId}",
                args.Message.MessageId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing message {MessageId}",
                args.Message.MessageId);

            // Dead letter the message after max retries
            if (args.Message.DeliveryCount >= 3)
            {
                await args.DeadLetterMessageAsync(
                    args.Message,
                    "Max retry count exceeded",
                    ex.Message);
            }
            else
            {
                // Abandon to retry
                await args.AbandonMessageAsync(args.Message);
            }
        }
    }

    private async Task ProcessMessageByTypeAsync(
        string messageType,
        string messageBody,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        switch (messageType)
        {
            case "BudgetCreatedEvent":
                var budgetEvent = JsonSerializer.Deserialize<BudgetCreatedEvent>(messageBody);
                var budgetHandler = scope.ServiceProvider.GetRequiredService<IBudgetEventHandler>();
                await budgetHandler.HandleAsync(budgetEvent!, cancellationToken);
                break;

            default:
                logger.LogWarning("Unknown message type: {MessageType}", messageType);
                break;
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Error in Service Bus processor: {ErrorSource}",
            args.ErrorSource);

        return Task.CompletedTask;
    }
}
```

### Register Service Bus
```csharp
// Program.cs
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["ServiceBus:ConnectionString"];
    return new ServiceBusClient(connectionString);
});

builder.Services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();
builder.Services.AddHostedService<ServiceBusMessageConsumer>();
```
