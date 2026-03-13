# ISynergy.Framework.OpenTelemetry.ApplicationInsights AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix reflection-based property copying (`GetProperties` / `GetValue` / `SetValue`) used to clone `AzureMonitorExporterOptions` in `ISynergy.Framework.OpenTelemetry.ApplicationInsights`.
**Architecture:** One file requires a simple code change: replace the private `Map(AzureMonitorExporterOptions source, AzureMonitorExporterOptions target)` reflection loop with explicit property assignments, matching the same pattern recommended in the base `ISynergy.Framework.OpenTelemetry` plan. No source generator is needed.
**Tech Stack:** .NET 10, C# 14.

---

## AOT Issues Found

### `src/ISynergy.Framework.OpenTelemetry.ApplicationInsights/Extensions/ApplicationInsightsTelemetryExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 45 | `source.GetType().GetProperties()` | Runtime property enumeration on `AzureMonitorExporterOptions` |
| 50 | `target.GetType().GetProperty(prop.Name)` | Runtime property lookup by name |
| 55–56 | `prop.GetValue(source)` / `targetProp.SetValue(target, value)` | Reflection read/write — AOT blocker |

---

## Implementation Plan

### Step 1 — Replace the reflection `Map` method

- [ ] Open `src/ISynergy.Framework.OpenTelemetry.ApplicationInsights/Extensions/ApplicationInsightsTelemetryExtensions.cs`
- [ ] Replace the private `Map(AzureMonitorExporterOptions source, AzureMonitorExporterOptions target)` method with explicit property assignments for all public settable properties of `AzureMonitorExporterOptions` from the Azure Monitor OpenTelemetry Exporter SDK version in use. Key properties typically include:
  - `target.ConnectionString = source.ConnectionString;`
  - `target.SamplingRatio = source.SamplingRatio;`
  - `target.Credential = source.Credential;`
  - `target.StorageDirectory = source.StorageDirectory;`
  - `target.Diagnostics = source.Diagnostics;` _(if settable)_
  _(Review the current SDK version's `AzureMonitorExporterOptions` public surface and map all writable properties explicitly.)_

### Step 2 — Verify no other AOT issues

- [ ] Confirm `ApplicationInsightsTelemetryExtensions.AddApplicationInsightsExporter` call-chains are all typed SDK extension method calls — AOT-safe after Step 1
- [ ] Confirm no other files exist in this library

### Step 3 — Update XML documentation

- [ ] Add `<remarks>` to `Map` noting the explicit-mapping approach for AOT compatibility

---

## Migration Guide

`ISynergy.Framework.OpenTelemetry.ApplicationInsights` replaces the reflection-based `AzureMonitorExporterOptions` property copying with explicit property assignments. No consumer-facing API changes occur.

### Before

```csharp
// Internal Map method used reflection to copy AzureMonitorExporterOptions properties
// Caused IL2026 warnings in AOT builds when publishing
services.AddApplicationInsightsTelemetryIntegration(configuration);
```

### After

```csharp
// No consumer-facing change — internal Map method now uses explicit assignments
services.AddApplicationInsightsTelemetryIntegration(configuration); // unchanged
```

### Steps to Migrate

1. No consumer-facing API changes — the `Map` method is an internal implementation detail.
2. Rebuild your application against the updated library to confirm no IL2026/IL3050 warnings originate from `ISynergy.Framework.OpenTelemetry.ApplicationInsights`.
3. Ensure the `Azure.Monitor.OpenTelemetry.Exporter` package in `Directory.Packages.props` is at a version that itself has AOT-compatible annotations for any types it exposes publicly.
