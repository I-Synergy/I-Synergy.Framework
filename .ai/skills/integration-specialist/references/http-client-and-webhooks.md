# Typed HTTP Clients & Webhooks

The typed-client interface/implementation/registration trio with the standard resilience handler, plus the webhook receiver endpoint and its signature-validating processor. Load this when integrating a third-party REST API or receiving webhooks.

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

    public bool ValidateSignature(ExternalServiceWebhookPayload payload, string signature)
    {
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        var secretBytes = Encoding.UTF8.GetBytes(_webhookSecret);

        using var hmac = new HMACSHA256(secretBytes);
        var hash = hmac.ComputeHash(bytes);
        var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();

        return signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
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
