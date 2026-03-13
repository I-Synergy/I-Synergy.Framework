# ISynergy.Framework.AspNetCore.Monitoring AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document that `ISynergy.Framework.AspNetCore.Monitoring` is free of AOT-blocking patterns.

**Architecture:** This is a small library (five source files). After a full read of every `.cs` file the analysis found no AOT-incompatible patterns. All types are used through generic constraints or concrete DI registrations; no runtime type discovery, `MakeGenericType`, `Activator.CreateInstance`, assembly scanning, non-generic `GetService(typeof(…))`, or unannotated JSON operations are present.

**Tech Stack:** .NET 10 / C# 14, ASP.NET Core SignalR (`IHubContext<THub>`), `ILogger<T>`.

---

## AOT Issues Found

**None.** Assessment by file:

| File | Assessment |
|------|------------|
| `Hubs/MonitorHub.cs` | Inherits `Hub`; uses `Context.User` extension methods and concrete `HubMessage<string>` — AOT-safe |
| `Services/MonitorService{TEntity}.cs` | Closed generic over `TEntity : class`; calls `IClientProxy.SendAsync` with a typed `data` argument — AOT-safe |
| `Extensions/ServiceCollectionExtensions.cs` | `AddMonitorSignalR<TEntity>()` registers `IMonitorService<TEntity>` and `MonitorHub` as concrete singletons; no `typeof(…)` escape — AOT-safe |
| `Extensions/ApplicationBuilderExtensions.cs` | `MapHub<MonitorHub>(…)` is a statically typed call — AOT-safe |
| `Extensions/HostApplicationBuilderExtensions.cs` | Delegates to `ServiceCollectionExtensions` — AOT-safe |

**Note on SignalR `SendAsync`:** `IClientProxy.SendAsync(string method, object? arg)` ultimately uses `System.Text.Json` serialisation for `arg`. At runtime `TEntity` is a concrete type, so the trimmer can statically verify it through the generic type constraint. No additional annotation is required in this library. If consumers pass unannotated polymorphic types as `TEntity`, that responsibility lies with the consumer.

## Recommendation

No implementation steps are required for this library at this time.

---

## Migration Guide

`ISynergy.Framework.AspNetCore.Monitoring` is fully AOT-compatible. No code changes or migration steps are required when upgrading to the AOT-compatible release.

### Before

```csharp
// Existing registration — no change needed
services.AddMonitorSignalR<MyEntity>(configuration);
app.MapHub<MonitorHub>("/monitoring");
```

### After

```csharp
// Same registration — fully AOT-safe
services.AddMonitorSignalR<MyEntity>(configuration);
app.MapHub<MonitorHub>("/monitoring");
```

### Steps to Migrate

1. No migration steps required — all types in this library are AOT-safe.
2. Ensure that any `TEntity` type you pass as the generic argument to `AddMonitorSignalR<TEntity>()` is serializable by `System.Text.Json` source generation in your application's `JsonSerializerContext` if you use AOT publishing.
3. If your application sends polymorphic types as hub message data, register the concrete types with `[JsonSerializable]` in your application-level `JsonSerializerContext`.
