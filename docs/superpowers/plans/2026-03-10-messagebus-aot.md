# ISynergy.Framework.MessageBus AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document the AOT compatibility status of the base `ISynergy.Framework.MessageBus` abstractions library, which contains only interfaces, option base classes, and DI registration helpers with no serialization or reflection.

**Architecture:** This library is a pure-abstraction package. All types are interfaces (`IPublisherServiceBus<T>`, `ISubscriberServiceBus<T>`, `IBaseMessage`, `IQueueMessage<T>`, `IHubMessage<T>`, `IEmailMessage`, `IQueueOption`, `ITopicOption`), option base classes (`BaseQueueOption`, `BaseTopicOption`), model classes (`QueueMessage<T>`, `HubMessage<T>`), and generic DI registration extension methods. No serialization, no assembly scanning, and no reflection are present in this library.

**Tech Stack:** .NET 10 / C# 14, `Microsoft.Extensions.DependencyInjection.Abstractions`.

---

## AOT Issues Found

### `Models/QueueMessage{T}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 57 | `data.GetType().Name` — runtime type name lookup to set `ContentType` | Low |

**Assessment:** `data.GetType().Name` is a benign reflection call that returns the runtime type name of the strongly-typed generic parameter `TEntity`. In an AOT scenario, the concrete `TEntity` type is always statically known at the call site (it is a generic type argument with a `class` constraint). The .NET AOT toolchain preserves `.GetType()` on instances of statically-known types, and `.Name` does not trigger any trimming concern. This is **not an AOT blocker**.

### `Models/HubMessage{T}.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 26 | `data.GetType().Name` — same as above | Low |

**Assessment:** Same reasoning as `QueueMessage<T>`. Not an AOT blocker.

### `Extensions/ServiceCollectionExtensions.cs`

No AOT issues. All DI registrations use the closed-generic `TryAddSingleton<TImplementation>()` and `TryAddSingleton<IPublisherServiceBus<TQueueMessage>, TImplementation>()` overloads which are AOT-safe.

### All other files

No AOT issues. Pure interface definitions and option POCO classes.

---

## Assessment

`ISynergy.Framework.MessageBus` contains **no AOT blockers**. The two `GetType().Name` calls in the model classes are benign — they operate on statically-typed generic instances and are preserved by the AOT compiler without any linker annotations.

No changes are required in this library for AOT compatibility. However, the two model classes should be verified to work with `System.Text.Json` source generation in consuming code, since they carry a generic `TEntity Data` property. Consumers that serialize `QueueMessage<T>` or `HubMessage<T>` instances must ensure `TEntity` is included in their `[JsonSerializable]` context. This is a consuming-application concern, not a framework library concern.

### Optional Hardening (not required for AOT)

- [ ] Replace `data.GetType().Name` with `typeof(TEntity).Name` in `QueueMessage<T>` (line 57) and `HubMessage<T>` (line 26). This is semantically equivalent when `data` is not a derived type, but is more AOT-explicit because it avoids the virtual dispatch and is more obviously a compile-time type reference. Mark this as a minor improvement, not a correctness fix.
- [ ] Add XML documentation to all public members that currently lack it (particularly `QueueMessageActions` enumeration values and the `IBaseMessage` interface) to satisfy the 100% XML documentation requirement.

---

## Migration Guide

`ISynergy.Framework.MessageBus` (the abstractions library) is fully AOT-compatible. No code changes are required. Consumers that serialize `QueueMessage<T>` or `HubMessage<T>` must ensure `T` is registered in their application-level `JsonSerializerContext`.

### Before

```csharp
// No consumer-facing changes in this library
var message = new QueueMessage<OrderDto>(order, QueueMessageActions.Create);
```

### After

```csharp
// Same API — no migration needed
var message = new QueueMessage<OrderDto>(order, QueueMessageActions.Create);

// If you serialize QueueMessage<OrderDto> via System.Text.Json in AOT code, register it:
[JsonSerializable(typeof(QueueMessage<OrderDto>))]
internal partial class AppJsonContext : JsonSerializerContext { }
```

### Steps to Migrate

1. No migration steps required in the base abstractions library.
2. In your application's AOT-published code, register `QueueMessage<YourType>` and `HubMessage<YourType>` with `[JsonSerializable]` in your `JsonSerializerContext` if you serialize these types.
3. See `ISynergy.Framework.MessageBus.Azure` and `ISynergy.Framework.MessageBus.RabbitMQ` migration guides for the transport-specific serialization changes.
