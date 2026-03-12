# ISynergy.Framework.Monitoring AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm that `ISynergy.Framework.Monitoring` (the shared abstractions library) is fully AOT-compatible with no code changes required.
**Architecture:** No changes needed — the library contains only interfaces, enumerations, message models, and options classes with no reflection, no JSON serialization beyond simple string keys, and no assembly scanning.
**Tech Stack:** .NET 10, C# 14.

---

## AOT Assessment: Compatible

`ISynergy.Framework.Monitoring` contains 6 source files: `IMonitorService<TEntity>` (interface), `MonitorEvents` (enum), `HubMessage`, `HubMessage<T>` (simple message types), and `ClientMonitorOptions` (options POCO). All are pure data contracts, interfaces, and enumerations with no reflection, no `dynamic`, no `JsonSerializer`, no `Activator.CreateInstance`, and no assembly scanning. The library is fully AOT-compatible as written.

---

## Migration Guide

`ISynergy.Framework.Monitoring` is fully AOT-compatible. No code changes or migration steps are required.

### Before

```csharp
// No changes needed
services.AddScoped<IMonitorService<MyEntity>, MyMonitorService>();
var message = new HubMessage<MyEntity> { Data = entity };
```

### After

```csharp
// Same — pure contracts and data types
services.AddScoped<IMonitorService<MyEntity>, MyMonitorService>();
var message = new HubMessage<MyEntity> { Data = entity };
```

### Steps to Migrate

1. No migration steps required for this abstractions library.
2. If you serialize `HubMessage<TEntity>` via System.Text.Json in AOT-published code, register the concrete `HubMessage<YourType>` with `[JsonSerializable]` in your application's `JsonSerializerContext`.
3. See `ISynergy.Framework.Monitoring.Client` migration guide for SignalR-specific AOT considerations.
