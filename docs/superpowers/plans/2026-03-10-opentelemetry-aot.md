# ISynergy.Framework.OpenTelemetry AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix reflection-based property copying (`GetProperties` / `GetValue` / `SetValue`) and unannotated `JsonSerializer` usage in `ISynergy.Framework.OpenTelemetry`.
**Architecture:** Two files require changes. `OtlpTelemetryExtensions.cs` contains a private `Map` helper that uses `source.GetType().GetProperties()` / `prop.GetValue` / `targetProp.SetValue` to shallow-copy `OtlpExporterOptions` — this is a reflection write/read blocked under AOT. `UserContextEnrichingLogProcessor.cs` calls `JsonSerializer.Serialize(context.Profile.Modules)` and `JsonSerializer.Serialize(context.Profile.Roles)` without a `JsonSerializerContext`. Neither requires a source generator; both are fixed by explicit property mapping and a `JsonSerializerContext` partial class respectively.
**Tech Stack:** .NET 10, C# 14, `System.Text.Json` source generation, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.OpenTelemetry/Extensions/OtlpTelemetryExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 48 | `source.GetType().GetProperties()` | Runtime property enumeration on `OtlpExporterOptions` — trimmer may remove properties |
| 53 | `target.GetType().GetProperty(prop.Name)` | Runtime property lookup by name — AOT blocker |
| 58–59 | `prop.GetValue(source)` / `targetProp.SetValue(target, value)` | Reflection read/write — AOT blocker |

### `src/ISynergy.Framework.OpenTelemetry/Processors/UserContextEnrichingLogProcessor.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 31 | `JsonSerializer.Serialize(context.Profile.Modules)` | No `JsonSerializerContext` — type of `Modules` collection unknown to trimmer |
| 32 | `JsonSerializer.Serialize(context.Profile.Roles)` | Same — `Roles` collection serialized without context |

---

## Implementation Plan

### Step 1 — Replace reflection-based `Map` in `OtlpTelemetryExtensions`

- [ ] Open `src/ISynergy.Framework.OpenTelemetry/Extensions/OtlpTelemetryExtensions.cs`
- [ ] Replace the private `Map(OtlpExporterOptions source, OtlpExporterOptions target)` method with explicit property assignments:
  ```csharp
  private static void Map(this OtlpExporterOptions source, OtlpExporterOptions target)
  {
      target.Endpoint = source.Endpoint;
      target.Protocol = source.Protocol;
      target.Headers = source.Headers;
      target.TimeoutMilliseconds = source.TimeoutMilliseconds;
      target.ExportProcessorType = source.ExportProcessorType;
      target.BatchExportProcessorOptions = source.BatchExportProcessorOptions;
      target.HttpClientFactory = source.HttpClientFactory;
  }
  ```
  _(Enumerate all public settable properties of `OtlpExporterOptions` from the OpenTelemetry SDK version in use and map them explicitly.)_

### Step 2 — Add `JsonSerializerContext` for profile collections

- [ ] Determine the concrete types of `IProfile.Modules` and `IProfile.Roles` from `ISynergy.Framework.Core`
- [ ] Create `src/ISynergy.Framework.OpenTelemetry/Serialization/TelemetryJsonContext.cs`:
  ```csharp
  using System.Text.Json.Serialization;

  namespace ISynergy.Framework.OpenTelemetry.Serialization;

  [JsonSerializable(typeof(IEnumerable<string>))]
  [JsonSerializable(typeof(List<string>))]
  // Add concrete module/role types here based on IProfile definition
  internal partial class TelemetryJsonContext : JsonSerializerContext { }
  ```
- [ ] Update `UserContextEnrichingLogProcessor.cs` to use the context:
  ```csharp
  using ISynergy.Framework.OpenTelemetry.Serialization;
  // ...
  attributes.Add(new KeyValuePair<string, object?>("Modules",
      JsonSerializer.Serialize(context.Profile.Modules, TelemetryJsonContext.Default.IEnumerableString)));
  attributes.Add(new KeyValuePair<string, object?>("Roles",
      JsonSerializer.Serialize(context.Profile.Roles, TelemetryJsonContext.Default.IEnumerableString)));
  ```
  _(Adjust the type token based on the actual `Modules` and `Roles` property types.)_

### Step 3 — Verify `TelemetryBuilder`, `OtlpTelemetryExtensions` registration methods, and constants

- [ ] `TelemetryBuilder.cs` — pure wrapper holding references, no reflection, AOT-safe
- [ ] `TelemetryConstants.cs` — string constants only, AOT-safe
- [ ] `OtlpTelemetryExtensions.AddOtlpExporter` call-chains — all typed extension method calls on known SDK types, AOT-safe after Step 1

### Step 4 — Update XML documentation

- [ ] Add XML `<remarks>` to the `Map` method noting it now uses explicit property mapping for AOT compatibility
- [ ] Document `TelemetryJsonContext`

---

## Migration Guide

`ISynergy.Framework.OpenTelemetry` replaces the reflection-based `OtlpExporterOptions` property copying with explicit property assignments, and replaces unregistered `JsonSerializer.Serialize` calls in `UserContextEnrichingLogProcessor` with a source-generated `TelemetryJsonContext`. No consumer-facing API changes occur.

### Before

```csharp
// Internal Map method used reflection to copy OtlpExporterOptions properties
// JsonSerializer.Serialize(context.Profile.Modules) — no JsonSerializerContext
// These caused IL2026 warnings in AOT builds
services.AddOtlpTelemetryIntegration(configuration);
```

### After

```csharp
// No consumer-facing change — internal fixes only
// Map method now uses explicit property assignments
// UserContextEnrichingLogProcessor uses TelemetryJsonContext
services.AddOtlpTelemetryIntegration(configuration); // unchanged
```

### Steps to Migrate

1. No consumer-facing API changes — the internal `Map` method and JSON serialization are implementation details.
2. Rebuild your application against the updated library to confirm no IL2026/IL3050 warnings originate from `ISynergy.Framework.OpenTelemetry`.
3. If you extend `UserContextEnrichingLogProcessor`, ensure any additional `JsonSerializer.Serialize` calls you introduce use a `JsonSerializerContext` or the `TelemetryJsonContext.Default` instance.
