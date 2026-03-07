# RabbitMQ MessageBus + S3 Storage Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add `ISynergy.Framework.MessageBus.RabbitMQ` and `ISynergy.Framework.Storage.S3` as provider implementations mirroring the existing Azure equivalents.

**Architecture:** Each new project is a thin provider implementation that depends on the existing abstraction project (`ISynergy.Framework.MessageBus` / `ISynergy.Framework.Storage`) and implements its interfaces. No changes to existing projects. The S3 implementation supports AWS S3, MinIO, and any other S3-compatible store via configurable `ServiceUrl` and `ForcePathStyle`.

**Tech Stack:** RabbitMQ.Client v7.x (async API), AWSSDK.S3 v3.7.x, System.Text.Json, Microsoft.Extensions.Options, .NET 10

---

## Reference: Existing Patterns

Before implementing, read these files to understand exact patterns to mirror:
- `src/ISynergy.Framework.MessageBus.Azure/Services/Queue/PublisherServiceBus{TQueueMessage}.cs`
- `src/ISynergy.Framework.MessageBus.Azure/Services/Queue/SubscriberServiceBus{TEntity,TOption}.cs`
- `src/ISynergy.Framework.MessageBus.Azure/Extensions/ServiceCollectionExtensions.cs`
- `src/ISynergy.Framework.MessageBus.Azure/Options/Queue/PublisherOptions.cs`
- `src/ISynergy.Framework.Storage.Azure/Services/AzureStorageService.cs`
- `src/ISynergy.Framework.Storage.Azure/Extensions/ServiceCollectionExtensions.cs`
- `src/ISynergy.Framework.MessageBus/Options/Base/BaseQueueOption.cs`
- `src/ISynergy.Framework.MessageBus/Abstractions/IPublisherServiceBus{TQueueMessage}.cs`
- `src/ISynergy.Framework.MessageBus/Abstractions/ISubscriberServiceBus{TEntity}.cs`
- `src/ISynergy.Framework.Storage/Abstractions/Services/IStorageService.cs`

---

## Task 1: Add NuGet package versions to Directory.Packages.props

**Files:**
- Modify: `Directory.Packages.props`

**Step 1: Verify package versions on nuget.org**

Check these exact package IDs:
- `RabbitMQ.Client` — use latest stable v7.x (targets .NET 8+, has async API)
- `AWSSDK.S3` — use latest stable v3.7.x

**Step 2: Add PackageVersion entries**

In `Directory.Packages.props`, inside the `<ItemGroup>`, add (in alphabetical order with existing entries):

```xml
<PackageVersion Include="AWSSDK.S3" Version="3.7.430.2" />
<PackageVersion Include="RabbitMQ.Client" Version="7.0.0" />
```

> Note: Verify exact versions on nuget.org before adding. Insert alphabetically among existing entries.

**Step 3: Verify the file parses correctly**

```powershell
dotnet restore
```

Expected: no errors.

**Step 4: Commit**

```bash
git add Directory.Packages.props
git commit -m "chore: add RabbitMQ.Client and AWSSDK.S3 package versions"
```

---

## Task 2: Create ISynergy.Framework.MessageBus.RabbitMQ project

**Files:**
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/ISynergy.Framework.MessageBus.RabbitMQ.csproj`

**Step 1: Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <PackageId>I-Synergy.Framework.MessageBus.RabbitMQ</PackageId>
        <Description>I-Synergy Framework RabbitMQ MessageBus</Description>
        <PackageTags>I-Synergy, Framework, MessageBus, RabbitMQ</PackageTags>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="RabbitMQ.Client" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="..\ISynergy.Framework.MessageBus\ISynergy.Framework.MessageBus.csproj" />
    </ItemGroup>

</Project>
```

**Step 2: Add project to solution**

```powershell
dotnet sln add src/ISynergy.Framework.MessageBus.RabbitMQ/ISynergy.Framework.MessageBus.RabbitMQ.csproj
```

**Step 3: Verify build**

```powershell
dotnet build src/ISynergy.Framework.MessageBus.RabbitMQ
```

Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/ISynergy.Framework.MessageBus.RabbitMQ/
git commit -m "feat: scaffold ISynergy.Framework.MessageBus.RabbitMQ project"
```

---

## Task 3: Create RabbitMQ Options

**Files:**
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/Options/Queue/PublisherOptions.cs`
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/Options/Queue/SubscriberOptions.cs`

**Step 1: Create PublisherOptions**

`ConnectionString` holds the AMQP URI (e.g. `amqp://user:pass@host:5672/vhost`).
`QueueName` is the routing key / queue name.
`ExchangeName` is the RabbitMQ exchange (empty string = default exchange).

```csharp
using ISynergy.Framework.MessageBus.Options.Base;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;

/// <summary>
/// Queue MessageBus publisher options for RabbitMQ.
/// </summary>
internal class PublisherOptions : BaseQueueOption
{
    /// <summary>
    /// Gets or sets the exchange name. Empty string uses the RabbitMQ default exchange.
    /// </summary>
    public string ExchangeName { get; set; } = string.Empty;
}
```

**Step 2: Create SubscriberOptions**

```csharp
using ISynergy.Framework.MessageBus.Options.Base;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;

/// <summary>
/// Queue MessageBus subscriber options for RabbitMQ.
/// </summary>
internal class SubscriberOptions : BaseQueueOption
{
    /// <summary>
    /// Gets or sets the exchange name. Empty string uses the RabbitMQ default exchange.
    /// </summary>
    public string ExchangeName { get; set; } = string.Empty;
}
```

**Step 3: Build**

```powershell
dotnet build src/ISynergy.Framework.MessageBus.RabbitMQ
```

Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/ISynergy.Framework.MessageBus.RabbitMQ/
git commit -m "feat(rabbitmq): add PublisherOptions and SubscriberOptions"
```

---

## Task 4: Implement RabbitMQ Publisher

**Files:**
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/Services/Queue/PublisherServiceBus{TQueueMessage}.cs`

**Step 1: Implement PublisherServiceBus**

RabbitMQ.Client v7 uses async `CreateConnectionAsync` / `CreateChannelAsync`. The connection string is an AMQP URI.

```csharp
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;

/// <summary>
/// Message bus publisher implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TQueueMessage">The type of the queue message.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
internal class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>
    where TQueueMessage : class, IBaseMessage
    where TOption : class, IQueueOption, new()
{
    /// <summary>
    /// The option.
    /// </summary>
    protected readonly TOption _option;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="PublisherServiceBus{TQueueMessage, TOption}"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    protected PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
    }

    /// <summary>
    /// Sends a message to the RabbitMQ queue.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    /// <param name="sessionId">Used as CorrelationId when not <see cref="Guid.Empty"/>.</param>
    /// <returns>Task.</returns>
    public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId)
    {
        if (queueMessage is not { } model)
            throw new ArgumentException("Entity should be type of IQueueMessage<TModel>");

        var exchangeName = _option is PublisherOptions publisherOptions
            ? publisherOptions.ExchangeName
            : string.Empty;

        var factory = new ConnectionFactory { Uri = new Uri(_option.ConnectionString) };

        await using var connection = await factory.CreateConnectionAsync().ConfigureAwait(false);
        await using var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true).ConfigureAwait(false);

        var body = JsonSerializer.SerializeToUtf8Bytes(queueMessage);

        var props = new BasicProperties
        {
            ContentType = model.ContentType,
            Persistent = true
        };

        if (!string.IsNullOrEmpty(model.Tag))
            props.Type = model.Tag;

        if (sessionId != Guid.Empty)
            props.CorrelationId = sessionId.ToString();

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: _option.QueueName,
            mandatory: false,
            basicProperties: props,
            body: body).ConfigureAwait(false);

        _logger.LogInformation("Published message to queue {QueueName}", _option.QueueName);
    }
}
```

**Step 2: Build**

```powershell
dotnet build src/ISynergy.Framework.MessageBus.RabbitMQ
```

Expected: Build succeeded.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.MessageBus.RabbitMQ/
git commit -m "feat(rabbitmq): implement PublisherServiceBus"
```

---

## Task 5: Implement RabbitMQ Subscriber

**Files:**
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/Services/Queue/SubscriberServiceBus{TEntity,TOption}.cs`

**Step 1: Implement SubscriberServiceBus**

The subscriber is `abstract` — consumers subclass it and implement `ProcessDataAsync` and `ValidateMessage`. Uses `AsyncEventingBasicConsumer` from RabbitMQ.Client v7.

```csharp
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;

/// <summary>
/// Message bus subscriber implementation on RabbitMQ.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TOption">The type of the option.</typeparam>
internal abstract class SubscriberServiceBus<TEntity, TOption> : ISubscriberServiceBus<TEntity>
    where TOption : class, IQueueOption, new()
{
    /// <summary>
    /// The option.
    /// </summary>
    protected readonly TOption _option;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    /// <summary>
    /// Initializes a new instance of <see cref="SubscriberServiceBus{TEntity, TOption}"/>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    protected SubscriberServiceBus(
        IOptions<TOption> options,
        ILogger<SubscriberServiceBus<TEntity, TOption>> logger)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
    }

    /// <inheritdoc />
    public abstract Task<bool> ProcessDataAsync(TEntity queueMessage);

    /// <inheritdoc />
    public abstract bool ValidateMessage(TEntity message);

    /// <inheritdoc />
    public virtual async Task SubscribeToMessageBusAsync(CancellationToken cancellationToken = default)
    {
        var exchangeName = _option is SubscriberOptions subscriberOptions
            ? subscriberOptions.ExchangeName
            : string.Empty;

        var factory = new ConnectionFactory { Uri = new Uri(_option.ConnectionString) };

        _connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.QueueDeclareAsync(
            queue: _option.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(exchangeName))
            await _channel.QueueBindAsync(_option.QueueName, exchangeName, _option.QueueName, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken).ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _option.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Subscribed to RabbitMQ queue {QueueName}", _option.QueueName);
    }

    /// <inheritdoc />
    public virtual async Task UnSubscribeFromMessageBusAsync(CancellationToken cancellationToken = default)
    {
        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken).ConfigureAwait(false);

        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            var body = args.Body.ToArray();
            var data = JsonSerializer.Deserialize<TEntity>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data is not null && ValidateMessage(data))
            {
                if (await ProcessDataAsync(data).ConfigureAwait(false))
                {
                    await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false).ConfigureAwait(false);
                }
                else
                {
                    // Nack without requeue — moves to dead-letter queue if configured
                    await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
                }
            }
            else
            {
                _logger.LogWarning("Message validation failed, dead-lettering message");
                await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RabbitMQ message from queue {QueueName}", _option.QueueName);
            await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false).ConfigureAwait(false);
        }
    }
}
```

**Step 2: Build**

```powershell
dotnet build src/ISynergy.Framework.MessageBus.RabbitMQ
```

Expected: Build succeeded.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.MessageBus.RabbitMQ/
git commit -m "feat(rabbitmq): implement SubscriberServiceBus"
```

---

## Task 6: Create RabbitMQ ServiceCollectionExtensions

**Files:**
- Create: `src/ISynergy.Framework.MessageBus.RabbitMQ/Extensions/ServiceCollectionExtensions.cs`

**Step 1: Implement the extension methods**

Pattern mirrors `ISynergy.Framework.MessageBus.Azure.Extensions.ServiceCollectionExtensions` exactly.

```csharp
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Extensions;
using ISynergy.Framework.MessageBus.RabbitMQ.Options.Queue;
using ISynergy.Framework.MessageBus.RabbitMQ.Services.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.MessageBus.RabbitMQ.Extensions;

/// <summary>
/// Service collection extensions for RabbitMQ message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ message bus publish integration.
    /// </summary>
    /// <typeparam name="TQueuePublishMessage">The type of the queue publish message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQPublishIntegration<TQueuePublishMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueuePublishMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<PublisherOptions>(configuration.GetSection($"{prefix}{nameof(PublisherOptions)}").BindWithReload);
        services.AddPublishingQueueMessageBus<TQueuePublishMessage, PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>();
        return services;
    }

    /// <summary>
    /// Adds RabbitMQ message bus subscribe integration.
    /// </summary>
    /// <typeparam name="TQueueSubscribeMessage">The type of the queue subscribe message.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessageBusRabbitMQSubscribeIntegration<TQueueSubscribeMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
        where TQueueSubscribeMessage : class, IBaseMessage
    {
        services.AddOptions();
        services.Configure<SubscriberOptions>(configuration.GetSection($"{prefix}{nameof(SubscriberOptions)}").BindWithReload);
        services.AddSubscribingQueueMessageBus<TQueueSubscribeMessage, SubscriberServiceBus<TQueueSubscribeMessage, SubscriberOptions>>();
        return services;
    }
}
```

**Step 2: Build**

```powershell
dotnet build src/ISynergy.Framework.MessageBus.RabbitMQ
```

Expected: Build succeeded.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.MessageBus.RabbitMQ/
git commit -m "feat(rabbitmq): add ServiceCollectionExtensions"
```

---

## Task 7: Create ISynergy.Framework.Storage.S3 project

**Files:**
- Create: `src/ISynergy.Framework.Storage.S3/ISynergy.Framework.Storage.S3.csproj`

**Step 1: Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <PackageId>I-Synergy.Framework.Storage.S3</PackageId>
        <Description>I-Synergy Framework S3 Storage (AWS S3 and S3-compatible providers such as MinIO)</Description>
        <PackageTags>I-Synergy, Framework, Storage, S3, AWS, MinIO</PackageTags>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="AWSSDK.S3" />
        <PackageReference Include="Microsoft.Extensions.Options" />
        <PackageReference Include="Microsoft.Extensions.Configuration" />
        <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include="..\ISynergy.Framework.Storage\ISynergy.Framework.Storage.csproj" />
    </ItemGroup>

</Project>
```

**Step 2: Add project to solution**

```powershell
dotnet sln add src/ISynergy.Framework.Storage.S3/ISynergy.Framework.Storage.S3.csproj
```

**Step 3: Build**

```powershell
dotnet build src/ISynergy.Framework.Storage.S3
```

Expected: Build succeeded.

**Step 4: Commit**

```bash
git add src/ISynergy.Framework.Storage.S3/
git commit -m "feat: scaffold ISynergy.Framework.Storage.S3 project"
```

---

## Task 8: Create S3StorageOptions

**Files:**
- Create: `src/ISynergy.Framework.Storage.S3/Options/S3StorageOptions.cs`

**Step 1: Implement S3StorageOptions**

This class holds all connection/credentials config needed for both AWS S3 and S3-compatible providers (MinIO, etc.).

```csharp
namespace ISynergy.Framework.Storage.S3.Options;

/// <summary>
/// Configuration options for S3 and S3-compatible storage providers (AWS S3, MinIO, etc.).
/// </summary>
public class S3StorageOptions
{
    /// <summary>
    /// Gets or sets the service URL for S3-compatible providers (e.g. http://minio:9000).
    /// Leave empty or null to use the default AWS S3 endpoint.
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the AWS access key or S3-compatible provider access key.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS secret key or S3-compatible provider secret key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS region (e.g. us-east-1). Required for AWS S3; may be arbitrary for MinIO.
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Gets or sets a value indicating whether to use path-style addressing.
    /// Set to <c>true</c> for MinIO and other S3-compatible providers that require path-style.
    /// AWS S3 uses virtual-hosted style by default (<c>false</c>).
    /// </summary>
    public bool ForcePathStyle { get; set; } = false;
}
```

**appsettings.json examples:**

For MinIO:
```json
"S3StorageOptions": {
  "ServiceUrl": "http://minio:9000",
  "AccessKey": "minioadmin",
  "SecretKey": "minioadmin",
  "Region": "us-east-1",
  "ForcePathStyle": true
}
```

For AWS S3:
```json
"S3StorageOptions": {
  "AccessKey": "AKIAIOSFODNN7EXAMPLE",
  "SecretKey": "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
  "Region": "eu-west-1"
}
```

**Step 2: Build**

```powershell
dotnet build src/ISynergy.Framework.Storage.S3
```

Expected: Build succeeded.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.Storage.S3/
git commit -m "feat(s3): add S3StorageOptions"
```

---

## Task 9: Implement S3StorageService

**Files:**
- Create: `src/ISynergy.Framework.Storage.S3/Services/S3StorageService.cs`

**Step 1: Implement S3StorageService**

Key mappings from `IStorageService`:
- `connectionStringName` parameter: not used for S3 (credentials come from `IOptions<S3StorageOptions>`)
- `containerName` → S3 bucket name
- `Path.Combine(folder, filename)` → S3 object key (use forward slash: `$"{folder}/{filename}"`)

The `AmazonS3Client` is created once (singleton-safe since options don't change at runtime).

```csharp
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Storage.S3.Services;

/// <summary>
/// Storage service implementation for AWS S3 and S3-compatible providers (MinIO, etc.).
/// Implements <see cref="IStorageService"/>.
/// </summary>
internal class S3StorageService : IStorageService, IDisposable
{
    private readonly ILogger<S3StorageService> _logger;
    private readonly AmazonS3Client _client;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="S3StorageService"/>.
    /// </summary>
    /// <param name="options">The S3 storage options.</param>
    /// <param name="logger">The logger.</param>
    public S3StorageService(
        IOptions<S3StorageOptions> options,
        ILogger<S3StorageService> logger)
    {
        var s3Options = options.Value;

        Argument.IsNotNullOrEmpty(s3Options.AccessKey);
        Argument.IsNotNullOrEmpty(s3Options.SecretKey);

        _logger = logger;

        var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);
        var config = new AmazonS3Config
        {
            ForcePathStyle = s3Options.ForcePathStyle
        };

        if (!string.IsNullOrEmpty(s3Options.ServiceUrl))
        {
            config.ServiceURL = s3Options.ServiceUrl;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(s3Options.Region);
        }

        _client = new AmazonS3Client(credentials, config);
    }

    /// <inheritdoc />
    public async Task<Uri> UploadFileAsync(
        string connectionStringName,
        string containerName,
        byte[] fileBytes,
        string contentType,
        string filename,
        string folder,
        bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        if (!overwrite)
        {
            var exists = await ObjectExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false);
            if (exists)
                throw new IOException($"Object '{key}' already exists in bucket '{containerName}'. Use overwrite=true to replace it.");
        }

        var request = new PutObjectRequest
        {
            BucketName = containerName,
            Key = key,
            InputStream = new MemoryStream(fileBytes),
            ContentType = contentType,
            AutoCloseStream = true
        };

        await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Uploaded object {Key} to bucket {Bucket}", key, containerName);

        return BuildObjectUri(containerName, key);
    }

    /// <inheritdoc />
    public async Task<byte[]> DownloadFileAsync(
        string connectionStringName,
        string containerName,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        var request = new GetObjectRequest
        {
            BucketName = containerName,
            Key = key
        };

        using var response = await _client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false);
        using var stream = new MemoryStream();

        await response.ResponseStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Downloaded object {Key} from bucket {Bucket}", key, containerName);

        return stream.ToArray();
    }

    /// <inheritdoc />
    public async Task<Uri> UpdateFileAsync(
        string connectionStringName,
        string containerName,
        byte[] fileBytes,
        string contentType,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        // Delete existing object first (mirrors Azure pattern)
        await DeleteObjectIfExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false);

        var request = new PutObjectRequest
        {
            BucketName = containerName,
            Key = key,
            InputStream = new MemoryStream(fileBytes),
            ContentType = contentType,
            AutoCloseStream = true
        };

        await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Updated object {Key} in bucket {Bucket}", key, containerName);

        return BuildObjectUri(containerName, key);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveFileAsync(
        string connectionStringName,
        string containerName,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        return await DeleteObjectIfExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _client.Dispose();
            _disposed = true;
        }
    }

    private static string GetObjectKey(string folder, string filename)
        => string.IsNullOrEmpty(folder)
            ? filename
            : $"{folder.TrimEnd('/')}/{filename}";

    private Uri BuildObjectUri(string bucketName, string key)
    {
        // Use the endpoint from the underlying config for URI construction
        var endpoint = _client.Config.ServiceURL;
        return string.IsNullOrEmpty(endpoint)
            ? new Uri($"https://{bucketName}.s3.amazonaws.com/{key}")
            : new Uri($"{endpoint.TrimEnd('/')}/{bucketName}/{key}");
    }

    private async Task<bool> ObjectExistsAsync(string bucketName, string key, CancellationToken cancellationToken)
    {
        try
        {
            await _client.GetObjectMetadataAsync(bucketName, key, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task<bool> DeleteObjectIfExistsAsync(string bucketName, string key, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteObjectRequest { BucketName = bucketName, Key = key };
            await _client.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
```

**Step 2: Build**

```powershell
dotnet build src/ISynergy.Framework.Storage.S3
```

Expected: Build succeeded.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.Storage.S3/
git commit -m "feat(s3): implement S3StorageService"
```

---

## Task 10: Create S3 ServiceCollectionExtensions

**Files:**
- Create: `src/ISynergy.Framework.Storage.S3/Extensions/ServiceCollectionExtensions.cs`

**Step 1: Implement the extension method**

Pattern mirrors `ISynergy.Framework.Storage.Azure.Extensions.ServiceCollectionExtensions`.

```csharp
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Options;
using ISynergy.Framework.Storage.S3.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Storage.S3.Extensions;

/// <summary>
/// Service collection extensions for S3 storage.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds S3 storage integration (supports AWS S3, MinIO, and other S3-compatible providers).
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddS3StorageIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
    {
        services.AddOptions();
        services.Configure<S3StorageOptions>(configuration.GetSection($"{prefix}{nameof(S3StorageOptions)}"));
        services.TryAddSingleton<IStorageService, S3StorageService>();
        return services;
    }
}
```

**Step 2: Build the full solution**

```powershell
dotnet build
```

Expected: Build succeeded, 0 errors, 0 warnings.

**Step 3: Commit**

```bash
git add src/ISynergy.Framework.Storage.S3/
git commit -m "feat(s3): add ServiceCollectionExtensions"
```

---

## Task 11: Full solution build and verification

**Step 1: Build full solution**

```powershell
dotnet build
```

Expected: Build succeeded, 0 errors.

**Step 2: Run all tests**

```powershell
dotnet test
```

Expected: All existing tests pass. (New projects have no tests — they are infrastructure-only, consistent with the existing Azure provider projects which also have no separate test projects.)

**Step 3: Verify project files are in solution**

```powershell
dotnet sln list
```

Expected: Both `ISynergy.Framework.MessageBus.RabbitMQ` and `ISynergy.Framework.Storage.S3` appear in the list.

**Step 4: Final commit**

```bash
git add .
git commit -m "feat: add RabbitMQ MessageBus and S3 Storage provider implementations"
```

---

## Configuration Reference

### RabbitMQ — appsettings.json

```json
{
  "PublisherOptions": {
    "ConnectionString": "amqp://user:password@rabbitmq-host:5672/vhost",
    "QueueName": "my-queue",
    "ExchangeName": "my-exchange"
  },
  "SubscriberOptions": {
    "ConnectionString": "amqp://user:password@rabbitmq-host:5672/vhost",
    "QueueName": "my-queue",
    "ExchangeName": "my-exchange"
  }
}
```

### RabbitMQ — Program.cs

```csharp
builder.Services.AddMessageBusRabbitMQPublishIntegration<MyMessage>(builder.Configuration);
builder.Services.AddMessageBusRabbitMQSubscribeIntegration<MyMessage>(builder.Configuration);
```

### S3 (MinIO) — appsettings.json

```json
{
  "S3StorageOptions": {
    "ServiceUrl": "http://minio:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Region": "us-east-1",
    "ForcePathStyle": true
  }
}
```

### S3 (AWS) — appsettings.json

```json
{
  "S3StorageOptions": {
    "AccessKey": "AKIAIOSFODNN7EXAMPLE",
    "SecretKey": "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
    "Region": "eu-west-1"
  }
}
```

### S3 — Program.cs

```csharp
builder.Services.AddS3StorageIntegration(builder.Configuration);
```
