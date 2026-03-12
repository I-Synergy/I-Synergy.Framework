# ISynergy.Framework.MessageBus.RabbitMQ AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.MessageBus.RabbitMQ` fully AOT-compatible by replacing unattributed `JsonSerializer.Serialize/Deserialize` calls with `System.Text.Json` source-generated `JsonTypeInfo<T>`-driven serialization.

**Architecture:** The RabbitMQ publisher and subscriber both delegate serialization to `JsonSerializer.SerializeToUtf8Bytes(queueMessage, DefaultJsonSerializers.Web)` and `JsonSerializer.Deserialize<TEntity>(body, DefaultJsonSerializers.Web)` respectively. `DefaultJsonSerializers.Web` is a runtime-constructed `JsonSerializerOptions` instance with no associated `JsonSerializerContext`, making both calls use the reflection-based serializer path. The fix mirrors the Azure MessageBus plan: inject `JsonTypeInfo<T>` at construction time and use the typed `JsonSerializer` overloads. The two libraries should be kept in sync so the same DI registration pattern works for both transports.

**Tech Stack:** .NET 10 / C# 14, `RabbitMQ.Client` 7+, `System.Text.Json` source generation, `ISynergy.Framework.Core.Serializers.DefaultJsonSerializers`, central package management via `Directory.Packages.props`.

---

## AOT Issues Found

### `Services/Queue/PublisherServiceBus{TQueueMessage}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 93 | `JsonSerializer.SerializeToUtf8Bytes(queueMessage, DefaultJsonSerializers.Web)` — reflection-based serialization via `JsonSerializerOptions` with no `JsonTypeInfo<TQueueMessage>` | **Critical** |

**Detail:** `DefaultJsonSerializers.Web` is a plain `JsonSerializerOptions` with no attached `JsonSerializerContext`. At AOT publish time, the `System.Text.Json` reflection serializer is not available and this call will throw `NotSupportedException` unless a `[JsonSerializable]` context has been registered globally. The generic `TQueueMessage` type's structure is not statically known by the linker.

### `Services/Queue/SubscriberServiceBus{TEntity,TOption}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 132 | `JsonSerializer.Deserialize<TEntity>(body, DefaultJsonSerializers.Web)` — reflection-based deserialization | **Critical** |

**Detail:** Same root cause. `DefaultJsonSerializers.Web` carries no `JsonSerializerContext`, so `TEntity`'s metadata is not pre-generated.

### `Extensions/ServiceCollectionExtensions.cs`

No AOT issues. All DI registrations use closed generic `TryAddSingleton` variants.

### `Options/Queue/PublisherOptions.cs` and `Options/Queue/SubscriberOptions.cs`

No AOT issues. Plain POCO option classes.

---

## Relationship to `DefaultJsonSerializers`

`DefaultJsonSerializers.Web` (in `ISynergy.Framework.Core`) is itself an AOT concern in the Core library. However, even if the Core library were to expose a `JsonSerializerContext`-based path, the message bus services would still need to be taught which specific `TQueueMessage`/`TEntity` type to serialize. The `JsonTypeInfo<T>` injection approach is therefore the correct fix regardless of what happens to `DefaultJsonSerializers`.

---

## Why No Source Generator Is Needed

Identical reasoning to the Azure MessageBus plan: `TQueueMessage` and `TEntity` are explicit, statically-known generic type arguments at every call site. The consuming application always knows which concrete types are sent/received. The fix is purely additive — accept a `JsonTypeInfo<T>` in the constructor and use it in the serialization calls.

---

## Implementation Steps

### Phase 1 — Add `JsonTypeInfo<T>` to `PublisherServiceBus`

- [ ] Add constructor parameter `JsonTypeInfo<TQueueMessage> jsonTypeInfo` and store as `private readonly JsonTypeInfo<TQueueMessage> _jsonTypeInfo`.

- [ ] Replace line 93:
  ```csharp
  // Before
  var body = JsonSerializer.SerializeToUtf8Bytes(queueMessage, DefaultJsonSerializers.Web);
  // After
  var body = JsonSerializer.SerializeToUtf8Bytes(queueMessage, _jsonTypeInfo);
  ```
  Note: `JsonSerializer.SerializeToUtf8Bytes<T>(T, JsonTypeInfo<T>)` is the AOT-safe overload.

- [ ] Add XML documentation for the new `jsonTypeInfo` constructor parameter.

### Phase 2 — Add `JsonTypeInfo<T>` to `SubscriberServiceBus`

- [ ] Add constructor parameter `JsonTypeInfo<TEntity> jsonTypeInfo` and store as `protected readonly JsonTypeInfo<TEntity> _jsonTypeInfo`.

- [ ] Replace line 132:
  ```csharp
  // Before
  var data = JsonSerializer.Deserialize<TEntity>(body, DefaultJsonSerializers.Web);
  // After
  var data = JsonSerializer.Deserialize<TEntity>(body, _jsonTypeInfo);
  ```

- [ ] Add XML documentation for the new constructor parameter.

### Phase 3 — Update DI registration extensions

- [ ] In `Extensions/ServiceCollectionExtensions.cs`, update `AddMessageBusRabbitMQPublishIntegration<TQueuePublishMessage>` to accept `JsonTypeInfo<TQueuePublishMessage> jsonTypeInfo` as a parameter and pass it through via a factory delegate when registering `PublisherServiceBus`:
  ```csharp
  public static IServiceCollection AddMessageBusRabbitMQPublishIntegration<TQueuePublishMessage>(
      this IServiceCollection services,
      IConfiguration configuration,
      JsonTypeInfo<TQueuePublishMessage> jsonTypeInfo,
      string prefix = "")
      where TQueuePublishMessage : class, IBaseMessage
  ```

- [ ] Similarly update `AddMessageBusRabbitMQSubscribeIntegration<TQueueSubscribeMessage>` to accept `JsonTypeInfo<TQueueSubscribeMessage>`.

### Phase 4 — Optional backwards-compatible reflection overloads

If non-AOT consumers must not be broken:

- [ ] Keep the existing constructors without `jsonTypeInfo` as `internal` or annotate them:
  ```csharp
  [RequiresUnreferencedCode("Serialization uses reflection. Pass a JsonTypeInfo<T> for AOT compatibility.")]
  [RequiresDynamicCode("Serialization uses dynamic code generation. Pass a JsonTypeInfo<T> for AOT compatibility.")]
  public PublisherServiceBus(IOptions<TOption> options, ILogger<...> logger) { ... }
  ```

### Phase 5 — Sync with Azure MessageBus plan

- [ ] Ensure the constructor signature, XML documentation, and DI helper pattern are identical between the Azure and RabbitMQ transports so that consuming application code can switch transports by changing only the registration call, not the serialization setup.

### Phase 6 — Testing

- [ ] Add a unit test for `PublisherServiceBus` serialization using a source-generated `JsonTypeInfo<T>`.
- [ ] Add a unit test for `SubscriberServiceBus` deserialization.
- [ ] Verify `<PublishTrimmed>true</PublishTrimmed>` build produces no `IL2026` or `IL3050` warnings for these classes.

---

## Example Consumer Pattern

```csharp
// Consumer assembly:
[JsonSerializable(typeof(OrderQueueMessage))]
internal partial class OrderJsonContext : JsonSerializerContext { }

// Registration:
services.AddMessageBusRabbitMQPublishIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);

services.AddMessageBusRabbitMQSubscribeIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);
```

---

## Migration Guide

`PublisherServiceBus` and `SubscriberServiceBus` for RabbitMQ now require a `JsonTypeInfo<T>` parameter, replacing the reflection-based `DefaultJsonSerializers.Web` path. This change mirrors the Azure MessageBus plan and enables the same DI registration pattern across both transports.

### Before

```csharp
// Program.cs — no JsonTypeInfo required previously
services.AddMessageBusRabbitMQPublishIntegration<OrderQueueMessage>(configuration);
services.AddMessageBusRabbitMQSubscribeIntegration<OrderQueueMessage>(configuration);
```

### After

```csharp
// Define a JsonSerializerContext for your message types
[JsonSerializable(typeof(OrderQueueMessage))]
internal partial class OrderJsonContext : JsonSerializerContext { }

// Program.cs — pass JsonTypeInfo<T> for AOT-safe serialization
services.AddMessageBusRabbitMQPublishIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);

services.AddMessageBusRabbitMQSubscribeIntegration<OrderQueueMessage>(
    configuration,
    OrderJsonContext.Default.OrderQueueMessage);
```

### Steps to Migrate

1. Create a `[JsonSerializable]`-attributed `JsonSerializerContext` partial class for each message type used with RabbitMQ.
2. Pass the corresponding `JsonTypeInfo<T>` to each `AddMessageBusRabbitMQPublishIntegration<T>` and `AddMessageBusRabbitMQSubscribeIntegration<T>` call.
3. The same `JsonSerializerContext` can cover both Azure and RabbitMQ message types if you use both transports.
4. Old constructors without `jsonTypeInfo` are now annotated `[RequiresUnreferencedCode]`; add suppressions if you must keep them for non-AOT scenarios.
