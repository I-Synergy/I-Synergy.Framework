# ISynergy.Framework.Monitoring.Client AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility constraints imposed by the SignalR client dependency in `ISynergy.Framework.Monitoring.Client`.
**Architecture:** The library's own code (`BaseClientMonitorService.cs`, `ServiceCollectionExtensions.cs`) contains no direct reflection or `dynamic`. However, `Microsoft.AspNetCore.SignalR.Client` uses reflection-based JSON serialization for hub message deserialization (`HubConnection.On<T>`) and may not be fully AOT-compatible depending on the payload types. The `HubConnection.On<HubMessage>` calls use a concrete, known type, which is favorable. The fix is to add a `JsonSerializerContext` for `HubMessage` and `HubMessage<T>` if SignalR's source-generated JSON support is required, and to annotate `ConnectAsync` if the SignalR version does not carry its own AOT annotations.
**Tech Stack:** .NET 10, C# 14, `Microsoft.AspNetCore.SignalR.Client`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Monitoring.Client/Services/BaseClientMonitorService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 50 | `_connection.On<HubMessage>(nameof(MonitorEvents.Connected), OnConnected)` | `HubConnection.On<T>` uses JSON deserialization for `HubMessage` — may require source-generated JSON for AOT |
| 51 | `_connection.On<HubMessage>(nameof(MonitorEvents.Disconnected), OnDisconnected)` | Same for disconnect handler |
| 44 | `new HubConnectionBuilder().WithUrl(...).Build()` | `HubConnectionBuilder` uses internal DI and may carry `[RequiresUnreferencedCode]` in some versions |

---

## Implementation Plan

### Step 1 — Verify SignalR Client AOT status

- [ ] Check the `Microsoft.AspNetCore.SignalR.Client` version in `Directory.Packages.props`
- [ ] ASP.NET Core 8+ SignalR Client ships with trimmer-safe annotations for `HubConnection.On<T>` when the type parameter is a statically-known type. Verify this for the version in use.
- [ ] If the version in use is AOT-annotated: no changes needed for `BaseClientMonitorService.cs`
- [ ] If not annotated: proceed to Step 2

### Step 2 — Add `JsonSerializerContext` for monitoring message types (if needed)

- [ ] Create `src/ISynergy.Framework.Monitoring.Client/Serialization/MonitoringJsonContext.cs`:
  ```csharp
  using System.Text.Json.Serialization;
  using ISynergy.Framework.Monitoring.Messages;

  namespace ISynergy.Framework.Monitoring.Client.Serialization;

  [JsonSerializable(typeof(HubMessage))]
  [JsonSerializable(typeof(HubMessage<string>))]
  internal partial class MonitoringJsonContext : JsonSerializerContext { }
  ```
- [ ] Configure SignalR to use the source-generated context if SignalR's client API supports it (via `HubConnectionBuilder.AddJsonProtocol(opts => opts.PayloadSerializerOptions.TypeInfoResolverChain.Add(MonitoringJsonContext.Default))`)

### Step 3 — Annotate `ConnectAsync` if necessary

- [ ] If `HubConnectionBuilder` carries `[RequiresUnreferencedCode]` in the SDK version in use, propagate the annotation to `ConnectAsync`:
  ```csharp
  [RequiresUnreferencedCode("SignalR HubConnectionBuilder may use reflection for message deserialization.")]
  public virtual Task ConnectAsync(string? token, Action<HubConnection> connectionAction)
  ```

### Step 4 — `ServiceCollectionExtensions` — no changes needed

- [ ] `AddMonitorSignalRIntegration<TClientMonitorService>` uses typed generic DI registration only — AOT-safe as written

### Step 5 — Update XML documentation

- [ ] Add `<remarks>` to `ConnectAsync` noting the SignalR AOT dependency

---

## Migration Guide

`ISynergy.Framework.Monitoring.Client` may require a `JsonSerializerContext` for `HubMessage` deserialization and a `[RequiresUnreferencedCode]` annotation on `ConnectAsync`, depending on the SignalR Client version. The DI registration method is already AOT-safe.

### Before

```csharp
// Program.cs — no AOT changes needed for DI registration
services.AddMonitorSignalRIntegration<MyClientMonitorService>(configuration);

// BaseClientMonitorService.ConnectAsync — may produce IL2026 warning in newer SDK
await monitorService.ConnectAsync(token, connection => { });
```

### After

```csharp
// DI registration — unchanged and AOT-safe
services.AddMonitorSignalRIntegration<MyClientMonitorService>(configuration);

// ConnectAsync — if annotated, suppress at call site or configure JSON protocol
// (framework handles this internally via MonitoringJsonContext)
await monitorService.ConnectAsync(token, connection =>
{
    // Optionally configure SignalR JSON protocol for AOT
    connection.AddJsonProtocol(opts =>
        opts.PayloadSerializerOptions.TypeInfoResolverChain.Add(
            MonitoringJsonContext.Default));
});
```

### Steps to Migrate

1. Check the `Microsoft.AspNetCore.SignalR.Client` version in `Directory.Packages.props`. ASP.NET Core 8+ ships with trimmer-safe annotations for known types in `HubConnection.On<T>`.
2. If SignalR Client is AOT-annotated for your version: no changes needed.
3. If not: add `#pragma warning disable IL2026` at the `ConnectAsync` call site, or configure the JSON protocol to use the `MonitoringJsonContext` (emitted by the framework) as shown above.
4. The `AddMonitorSignalRIntegration` registration always uses closed-generic DI and requires no changes.
