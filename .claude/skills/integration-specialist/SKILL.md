---
name: integration-specialist
description: External API integration specialist. Use when integrating with third-party APIs, implementing webhooks, message queues, or handling external service communication.
---

# External Integration Specialist Skill

Specialized agent for integrating with external APIs, message queues, webhooks, and third-party services.

## Role

You are an Integration Specialist responsible for connecting the application with external services, implementing API clients, handling webhooks, managing message queues, and ensuring reliable communication with third-party systems.

## Expertise Areas

- REST API client implementation
- HttpClient best practices and resilience
- Webhook handling and validation
- Message queues (Azure Service Bus, RabbitMQ)
- Event-driven architecture
- API versioning strategies
- Retry and circuit breaker patterns
- Third-party SDK integration
- API rate limit handling
- OAuth2 client flows

## Responsibilities

1. **API Client Implementation**
   - Create typed HTTP clients
   - Implement authentication
   - Handle errors and retries
   - Serialize/deserialize requests/responses
   - Mock external services for testing

2. **Webhook Management**
   - Receive and validate webhooks
   - Implement signature verification
   - Handle idempotency
   - Queue webhook processing
   - Retry failed webhooks

3. **Message Queue Integration**
   - Send and receive messages
   - Handle dead letter queues
   - Implement message retry logic
   - Monitor queue health
   - Ensure message ordering where needed

4. **Resilience Patterns**
   - Implement retry with exponential backoff
   - Configure circuit breakers
   - Handle timeout scenarios
   - Implement fallback strategies
   - Monitor integration health

## Load Additional Patterns

- [`api-patterns.md`](../../patterns/api-patterns.md)

## Critical Rules

### HTTP Client Best Practices
- NEVER create HttpClient directly (use IHttpClientFactory)
- ALWAYS implement resilience patterns (retry, circuit breaker)
- ALWAYS validate external responses
- ALWAYS handle rate limiting
- Set appropriate timeouts
- Use typed clients for clean separation
- Mock external services in tests

### Webhook Security
- ALWAYS verify webhook signatures
- ALWAYS implement idempotency
- Process webhooks asynchronously
- Respond quickly (< 3 seconds)
- Validate webhook payload schema
- Log all webhook events

### Message Queue Patterns
- Use transactions where applicable
- Handle poison messages
- Implement dead letter queue processing
- Monitor queue depth
- Use message TTL appropriately
- Ensure at-least-once delivery handling

## Typed HTTP Client Pattern

### Define Client Interface
```csharp
// File: {ApplicationName}.Contracts.ExternalServices/IExternalApiClient.cs
namespace {ApplicationName}.Contracts.ExternalServices;

public interface IExternalApiClient
{
    Task<ExternalUser> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<ExternalData> GetDataAsync(string dataId, CancellationToken cancellationToken = default);
    Task<CreateExternalResourceResponse> CreateResourceAsync(
        CreateExternalResourceRequest request,
        CancellationToken cancellationToken = default);
}
```

### Implement Typed Client
```csharp
// File: {ApplicationName}.Infrastructure.ExternalServices/ExternalApiClient.cs
namespace {ApplicationName}.Infrastructure.ExternalServices;

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using {ApplicationName}.Contracts.ExternalServices;

public class ExternalApiClient(
    HttpClient httpClient,
    ILogger<ExternalApiClient> logger
) : IExternalApiClient
{
    public async Task<ExternalUser> GetUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        logger.LogDebug("Fetching user {UserId} from external API", userId);

        try
        {
            var response = await httpClient.GetAsync(
                $"/users/{userId}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<ExternalUser>(
                cancellationToken: cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Failed to deserialize user response");

            logger.LogInformation("Successfully fetched user {UserId}", userId);

            return user;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "HTTP request failed while fetching user {UserId}",
                userId);
            throw new ExternalApiException("Failed to fetch user from external API", ex);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(
                ex,
                "Request timed out while fetching user {UserId}",
                userId);
            throw new ExternalApiException("External API request timed out", ex);
        }
    }

    public async Task<CreateExternalResourceResponse> CreateResourceAsync(
        CreateExternalResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        logger.LogDebug("Creating external resource: {ResourceName}", request.Name);

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "/resources",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreateExternalResourceResponse>(
                cancellationToken: cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Failed to deserialize response");

            logger.LogInformation(
                "Successfully created external resource with ID {ResourceId}",
                result.ResourceId);

            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create external resource");
            throw new ExternalApiException("Failed to create resource in external API", ex);
        }
    }
}
```

### Register Typed Client with Resilience
```csharp
// Program.cs
builder.Services.AddHttpClient<IExternalApiClient, ExternalApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "{ApplicationName}/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddStandardResilienceHandler(options =>
{
    // Retry configuration
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
    options.Retry.UseJitter = true;

    // Circuit breaker configuration
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.MinimumThroughput = 10;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    // Timeout configuration
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
});
```

## Webhook Implementation

### Webhook Receiver Endpoint
```csharp
// File: {ApplicationName}.Services.API/Endpoints/WebhookEndpoints.cs
namespace {ApplicationName}.Services.API.Endpoints;

using Microsoft.AspNetCore.Mvc;

public static class WebhookEndpoints
{
    public static void MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/webhooks")
            .WithTags("Webhooks");

        group.MapPost("/external-service", async (
            [FromBody] ExternalServiceWebhookPayload payload,
            [FromHeader(Name = "X-Signature")] string signature,
            IWebhookProcessor processor,
            ILogger<Program> logger) =>
        {
            // Validate signature
            if (!processor.ValidateSignature(payload, signature))
            {
                logger.LogWarning("Invalid webhook signature received");
                return Results.Unauthorized();
            }

            // Quick acknowledgment (< 3 seconds)
            _ = Task.Run(async () =>
            {
                try
                {
                    await processor.ProcessWebhookAsync(payload);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing webhook");
                }
            });

            return Results.Ok(new { received = true });
        })
        .AllowAnonymous()
        .WithName("ReceiveExternalServiceWebhook")
        .WithOpenApi();
    }
}
```

### Webhook Processor
```csharp
// File: {ApplicationName}.Infrastructure.Webhooks/WebhookProcessor.cs
namespace {ApplicationName}.Infrastructure.Webhooks;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public interface IWebhookProcessor
{
    bool ValidateSignature(ExternalServiceWebhookPayload payload, string signature);
    Task ProcessWebhookAsync(ExternalServiceWebhookPayload payload);
}

public class WebhookProcessor(
    IConfiguration configuration,
    DataContext dataContext,
    ILogger<WebhookProcessor> logger
) : IWebhookProcessor
{
    private readonly string _webhookSecret = configuration["ExternalService:WebhookSecret"]
        ?? throw new InvalidOperationException("Webhook secret not configured");

    public bool ValidateSignature(byte[] requestBody, string signature)
    {
        if (requestBody is null)
        {
            throw new ArgumentNullException(nameof(requestBody));
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var secretBytes = Encoding.UTF8.GetBytes(_webhookSecret);

        using var hmac = new HMACSHA256(secretBytes);
        var computedHash = hmac.ComputeHash(requestBody);

        if (!TryDecodeHexString(signature, out var expectedHash))
        {
            return false;
        }

        if (expectedHash.Length != computedHash.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
    }

    private static bool TryDecodeHexString(string hex, out byte[] bytes)
    {
        if (hex is null)
        {
            bytes = Array.Empty<byte>();
            return false;
        }

        if ((hex.Length & 1) != 0)
        {
            bytes = Array.Empty<byte>();
            return false;
        }

        var result = new byte[hex.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            var high = Convert.ToByte(hex[2 * i].ToString(), 16);
            var low = Convert.ToByte(hex[2 * i + 1].ToString(), 16);
            result[i] = (byte)((high << 4) | low);
        }

        bytes = result;
        return true;
    }
    public async Task ProcessWebhookAsync(ExternalServiceWebhookPayload payload)
    {
        logger.LogInformation(
            "Processing webhook event {EventType} with ID {EventId}",
            payload.EventType, payload.EventId);

        // Check idempotency
        var existing = await dataContext.ProcessedWebhooks
            .FirstOrDefaultAsync(w => w.EventId == payload.EventId);

        if (existing is not null)
        {
            logger.LogWarning(
                "Webhook event {EventId} already processed, skipping",
                payload.EventId);
            return;
        }

        // Process based on event type
        switch (payload.EventType)
        {
            case "user.created":
                await HandleUserCreatedAsync(payload);
                break;

            case "user.updated":
                await HandleUserUpdatedAsync(payload);
                break;

            case "user.deleted":
                await HandleUserDeletedAsync(payload);
                break;

            default:
                logger.LogWarning(
                    "Unknown webhook event type: {EventType}",
                    payload.EventType);
                break;
        }

        // Record as processed
        await dataContext.ProcessedWebhooks.AddAsync(new ProcessedWebhook
        {
            EventId = payload.EventId,
            EventType = payload.EventType,
            ProcessedAt = DateTimeOffset.UtcNow
        });

        await dataContext.SaveChangesAsync();

        logger.LogInformation(
            "Successfully processed webhook event {EventId}",
            payload.EventId);
    }

    private async Task HandleUserCreatedAsync(ExternalServiceWebhookPayload payload)
    {
        // Handle user created event
        logger.LogDebug("Handling user.created event");
        // Implementation...
    }

    private async Task HandleUserUpdatedAsync(ExternalServiceWebhookPayload payload)
    {
        // Handle user updated event
        logger.LogDebug("Handling user.updated event");
        // Implementation...
    }

    private async Task HandleUserDeletedAsync(ExternalServiceWebhookPayload payload)
    {
        // Handle user deleted event
        logger.LogDebug("Handling user.deleted event");
        // Implementation...
    }
}
```

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

## OAuth2 Client Flow

```csharp
// File: {ApplicationName}.Infrastructure.ExternalServices/OAuth2TokenService.cs
namespace {ApplicationName}.Infrastructure.ExternalServices;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public interface IOAuth2TokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public class OAuth2TokenService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<OAuth2TokenService> logger
) : IOAuth2TokenService
{
    private string? _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private readonly System.Threading.SemaphoreSlim _tokenSemaphore = new(1, 1);

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Fast-path: return cached token if valid
        if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _tokenExpiry)
        {
            logger.LogDebug("Using cached OAuth2 token");
            return _cachedToken;
        }

        await _tokenSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Double-check after acquiring the lock to avoid redundant refreshes
            if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _tokenExpiry)
            {
                logger.LogDebug("Using cached OAuth2 token after synchronization");
                return _cachedToken;
            }

            logger.LogDebug("Requesting new OAuth2 token");

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = configuration["OAuth:ClientId"]!,
                ["client_secret"] = configuration["OAuth:ClientSecret"]!,
                ["scope"] = configuration["OAuth:Scope"]!
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token")
            {
                Content = new FormUrlEncodedContent(tokenRequest)
            };

            var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                throw new InvalidOperationException("Failed to obtain OAuth2 token");

            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 minute early

            logger.LogInformation("Successfully obtained OAuth2 token, expires in {ExpiresIn}s", tokenResponse.ExpiresIn);

            return _cachedToken;
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}

// Add authentication to API client
builder.Services.AddHttpClient<IExternalApiClient, ExternalApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!);
})
.AddHttpMessageHandler<OAuth2DelegatingHandler>();

// OAuth2 delegating handler
public class OAuth2DelegatingHandler(
    IOAuth2TokenService tokenService
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
```

## Rate Limit Handling

```csharp
public class RateLimitedApiClient(
    HttpClient httpClient,
    ILogger<RateLimitedApiClient> logger
) : IExternalApiClient
{
    public async Task<ExternalData> GetDataAsync(
        string dataId,
        CancellationToken cancellationToken = default)
    {
        var maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var response = await httpClient.GetAsync($"/data/{dataId}", cancellationToken);

                // Handle rate limit (429 Too Many Requests)
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);

                    logger.LogWarning(
                        "Rate limited by external API. Retrying after {RetryAfter}",
                        retryAfter);

                    await Task.Delay(retryAfter, cancellationToken);
                    retryCount++;
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<ExternalData>(
                    cancellationToken: cancellationToken);

                return data ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (HttpRequestException ex) when (retryCount < maxRetries - 1)
            {
                logger.LogWarning(
                    ex,
                    "Request failed, retrying ({RetryCount}/{MaxRetries})",
                    retryCount + 1, maxRetries);

                retryCount++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
            }
        }

        throw new ExternalApiException("Failed to fetch data after max retries");
    }
}
```

## Common Integration Pitfalls

### ❌ Avoid These Mistakes

1. **Creating HttpClient Directly**
   - ❌ `new HttpClient()`
   - ✅ Use IHttpClientFactory

2. **No Resilience Patterns**
   - ❌ No retries or circuit breakers
   - ✅ Add standard resilience handler

3. **Blocking Webhook Processing**
   - ❌ Processing webhook synchronously in endpoint
   - ✅ Queue and process asynchronously

4. **Not Validating Webhook Signatures**
   - ❌ Trusting all incoming webhooks
   - ✅ Verify HMAC signature

5. **Ignoring Idempotency**
   - ❌ Processing same webhook multiple times
   - ✅ Check if already processed

6. **No Timeout Configuration**
   - ❌ Default infinite timeout
   - ✅ Set appropriate timeouts

## Integration Checklist

### API Client
- [ ] Typed HTTP client implemented
- [ ] IHttpClientFactory used
- [ ] Resilience patterns configured
- [ ] Authentication handled
- [ ] Rate limiting handled
- [ ] Errors logged appropriately
- [ ] Timeouts configured

### Webhooks
- [ ] Signature validation implemented
- [ ] Idempotency check in place
- [ ] Async processing configured
- [ ] Quick response (< 3s)
- [ ] Webhook events logged
- [ ] Retry logic for processing failures

### Message Queue
- [ ] Publisher implemented
- [ ] Consumer background service created
- [ ] Dead letter queue handling
- [ ] Message retry logic
- [ ] Idempotent message handling
- [ ] Queue health monitoring

### Testing
- [ ] Mock external services in tests
- [ ] Test retry scenarios
- [ ] Test timeout scenarios
- [ ] Test rate limiting
- [ ] Test webhook signature validation

## Checklist Before Completion

- [ ] All external services have typed clients
- [ ] Resilience patterns implemented
- [ ] Webhook validation functional
- [ ] Message queue integration working
- [ ] Rate limiting handled
- [ ] OAuth2 token refresh working
- [ ] Error handling comprehensive
- [ ] Logging includes correlation IDs
- [ ] Integration tests passing
- [ ] Documentation complete
