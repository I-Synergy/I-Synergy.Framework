# ISynergy.Framework.MessageBus.Azure AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.MessageBus.Azure` fully AOT-compatible by replacing unattributed `JsonSerializer.Serialize/Deserialize` calls with `System.Text.Json` source-generated serialization contexts for all message types.

**Architecture:** Both `PublisherServiceBus<TQueueMessage, TOption>` and `SubscriberServiceBus<TEntity, TOption>` use `System.Text.Json` to serialize/deserialize the generic type parameter `TQueueMessage`/`TEntity`. These calls use the reflection-based serializer path (`JsonSerializer.Serialize(object)` and `JsonSerializer.Deserialize<T>(string, JsonSerializerOptions)`) which is not AOT-safe because the serializer must generate runtime metadata for types it has never seen. The fix is to accept an `JsonSerializerContext` (or a `JsonTypeInfo<T>`) at construction time so that serialization can be driven by compile-time-generated type metadata rather than runtime reflection.

**Tech Stack:** .NET 10 / C# 14, `Azure.Messaging.ServiceBus`, `System.Text.Json` source generation, central package management via `Directory.Packages.props`.

---

## AOT Issues Found

### `Services/Queue/PublisherServiceBus{TQueueMessage}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 58 | `JsonSerializer.Serialize(queueMessage)` — reflection-based serialization of generic `TQueueMessage` with no `JsonTypeInfo` or `JsonSerializerContext` | **Critical** |

**Detail:** `JsonSerializer.Serialize<T>(T value)` with no `JsonTypeInfo<T>` argument falls back to the reflection-based serializer. Under AOT publishing, this path is either trimmed away (causing a `NotSupportedException` at runtime) or triggers an ILLink warning. The trimmer cannot statically determine what properties `TQueueMessage` has.

### `Services/Queue/SubscriberServiceBus{TEntity,TOption}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 132–135 | `JsonSerializer.Deserialize<TEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })` — reflection-based deserialization into generic `TEntity` with inline options, no `JsonTypeInfo` | **Critical** |

**Detail:** Same root cause as the publisher. The inline `new JsonSerializerOptions { ... }` also bypasses any registered serialization context. Additionally, creating a new `JsonSerializerOptions` instance on every message is an unnecessary allocation and prevents metadata caching.

### `Extensions/ServiceCollectionExtensions.cs`

No AOT issues. DI registrations are all closed-generic and AOT-safe.

### `Options/Queue/PublisherOptions.cs` and `Options/Queue/SubscriberOptions.cs`

No AOT issues. Plain POCO option classes.

---

## Why No Source Generator Is Needed

Source generators are needed when types are *discovered* at compile time from the consuming assembly and cannot be named in advance. Here, the message types (`TQueueMessage` and `TEntity`) are explicit generic type arguments — they are always statically known at the consumer's call site. The correct fix is to:

1. Accept a `JsonTypeInfo<TQueueMessage>` / `JsonTypeInfo<TEntity>` parameter in the constructor, **or**
2. Accept a `JsonSerializerContext` and call `GetTypeInfo(typeof(T))` from it, **or**
3. Use a `JsonSerializerOptions` that is sourced from a registered `JsonSerializerContext`.

Option 1 (injecting `JsonTypeInfo<T>` directly) is the cleanest because it is fully explicit and requires zero runtime resolution.

Consuming code registers a `[JsonSerializable]` context once per application and passes the appropriate `JsonTypeInfo<T>` when registering the publisher/subscriber.

---

## Implementation Steps

### Phase 1 — Add `JsonTypeInfo<T>` overloads to both service classes

- [ ] In `PublisherServiceBus<TQueueMessage, TOption>`, add a constructor parameter `JsonTypeInfo<TQueueMessage> jsonTypeInfo` and store it as `private readonly JsonTypeInfo<TQueueMessage> _jsonTypeInfo`.

- [ ] Replace line 58:
  ```csharp
  // Before
  var body = JsonSerializer.Serialize(queueMessage);
  // After
  var body = JsonSerializer.Serialize(queueMessage, _jsonTypeInfo);
  ```

- [ ] In `SubscriberServiceBus<TEntity, TOption>`, add a constructor parameter `JsonTypeInfo<TEntity> jsonTypeInfo` and store it as `protected readonly JsonTypeInfo<TEntity> _jsonTypeInfo`.

- [ ] Replace lines 132–135:
  ```csharp
  // Before
  var data = JsonSerializer.Deserialize<TEntity>(body, new JsonSerializerOptions
  {
      PropertyNameCaseInsensitive = true
  });
  // After
  var data = JsonSerializer.Deserialize<TEntity>(body, _jsonTypeInfo);
  ```
  Note: `PropertyNameCaseInsensitive` should be baked into the `JsonSerializerOptions` associated with the consumer's `JsonSerializerContext`, not recreated per message.

- [ ] Add XML documentation for the new `jsonTypeInfo` constructor parameter in both classes.

### Phase 2 — Update DI registration extensions to thread `JsonTypeInfo<T>` through

- [ ] In `Extensions/ServiceCollectionExtensions.cs`, update `AddMessageBusAzurePublishIntegration<TQueuePublishMessage>` to require the caller to supply a `JsonTypeInfo<TQueuePublishMessage>` parameter and pass it through to the `PublisherServiceBus` registration:
  ```csharp
  public static IServiceCollection AddMessageBusAzurePublishIntegration<TQueuePublishMessage>(
      this IServiceCollection services,
      IConfiguration configuration,
      JsonTypeInfo<TQueuePublishMessage> jsonTypeInfo,
      string prefix = "")
      where TQueuePublishMessage : class, IBaseMessage
  ```
  Inside, register `PublisherServiceBus` using a factory delegate:
  ```csharp
  services.AddPublishingQueueMessageBus<TQueuePublishMessage,
      PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>(
          sp => new PublisherServiceBus<TQueuePublishMessage, PublisherOptions>(
              sp.GetRequiredService<IOptions<PublisherOptions>>(),
              sp.GetRequiredService<ILogger<PublisherServiceBus<TQueuePublishMessage, PublisherOptions>>>(),
              jsonTypeInfo));
  ```

- [ ] Similarly update `AddMessageBusAzureSubscribeIntegration<TQueueSubscribeMessage>` to accept `JsonTypeInfo<TQueueSubscribeMessage>`.

- [ ] The `AddPublishingQueueMessageBus` / `AddSubscribingQueueMessageBus` helpers in the base `ISynergy.Framework.MessageBus` also need factory-delegate overloads if they do not already have them (check `ServiceCollectionExtensions.cs` in the base library).

### Phase 3 — Mark the old reflection paths as warnings (if keeping backwards-compat overloads)

If overload compatibility is required for non-AOT consumers:

- [ ] Keep the existing constructors without `jsonTypeInfo` and annotate them:
  ```csharp
  [RequiresUnreferencedCode("Serialization uses reflection. Pass a JsonTypeInfo<T> for AOT compatibility.")]
  [RequiresDynamicCode("Serialization uses dynamic code generation. Pass a JsonTypeInfo<T> for AOT compatibility.")]
  public PublisherServiceBus(IOptions<TOption> options, ILogger<...> logger) { ... }
  ```

### Phase 4 — Update XML documentation

- [ ] Add XML documentation to `PublisherServiceBus` and `SubscriberServiceBus` explaining that the `JsonTypeInfo<T>` parameter is required for Native AOT publishing.
- [ ] Add an `<example>` block showing how to create a `[JsonSerializable]` context and pass its type info:
  ```csharp
  [JsonSerializable(typeof(MyQueueMessage))]
  internal partial class MyJsonContext : JsonSerializerContext { }

  // Registration:
  services.AddMessageBusAzurePublishIntegration<MyQueueMessage>(
      configuration,
      MyJsonContext.Default.MyQueueMessage);
  ```

### Phase 5 — Testing

- [ ] Add a unit test that constructs `PublisherServiceBus` with a source-generated `JsonTypeInfo<T>` and verifies serialization produces the correct JSON.
- [ ] Add a unit test that constructs `SubscriberServiceBus` and verifies deserialization of a known JSON body returns the correct typed object.
- [ ] Verify the project builds without `ILLink` warnings when `<PublishTrimmed>true</PublishTrimmed>` is set.

---

## Migration Guide

`PublisherServiceBus` and `SubscriberServiceBus` now require a `JsonTypeInfo<T>` parameter for AOT-safe JSON serialization. The DI registration extensions are updated to accept this parameter. Consumers must define a `[JsonSerializable]` context for their message types and pass the generated `JsonTypeInfo<T>` at registration time.

### Before

```csharp
// Program.cs — no JsonTypeInfo required previously
services.AddMessageBusAzurePublishIntegration<OrderQueueMessage>(configuration);
services.AddMessageBusAzureSubscribeIntegration<OrderQueueMessage>(configuration);
```

### After

```csharp
// Define a JsonSerializerContext for your message types
[JsonSerializable(typeof(OrderQueueMessage))]
internal partial class OrderJsonContext : JsonSerializerContext { }

// Program.cs — pass JsonTypeInfo<T> for AOT-safe serialization
services.AddMessageBusAzurePublishIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);

services.AddMessageBusAzureSubscribeIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);
```

### Steps to Migrate

1. Create (or update) a `[JsonSerializable]`-attributed `JsonSerializerContext` partial class in your application for each message type used with Azure Service Bus.
2. Pass the corresponding `JsonTypeInfo<T>` (e.g., `MyJsonContext.Default.MyMessageType`) to each `AddMessageBusAzurePublishIntegration<T>` and `AddMessageBusAzureSubscribeIntegration<T>` call.
3. If backwards compatibility with the old reflection-based constructors is needed, add `#pragma warning disable IL2026, IL3050` suppressions — the old constructors are now annotated with `[RequiresUnreferencedCode]`.
4. Update `Directory.Packages.props` to ensure `Azure.Messaging.ServiceBus` is at version 7.16.0 or later for its own AOT improvements.
