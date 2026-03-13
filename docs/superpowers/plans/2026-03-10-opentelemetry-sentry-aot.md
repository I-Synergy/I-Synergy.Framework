# ISynergy.Framework.OpenTelemetry.Sentry AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix reflection-based property copying in `SentryTelemetryExtensions` and assess the AOT compatibility of the Sentry SDK dependency.
**Architecture:** One file requires a code change: replace the `Map(SentryOptions source, SentryOptions target)` reflection loop with explicit property assignments. Additionally, the Sentry SDK itself (`Sentry`, `Sentry.OpenTelemetry`) should be assessed for AOT support — as of 2025, Sentry's .NET SDK has partial AOT support; `[RequiresUnreferencedCode]` annotations may be needed on the extension method. No source generator is needed.
**Tech Stack:** .NET 10, C# 14, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.OpenTelemetry.Sentry/Extensions/SentryTelemetryExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 65 | `source.GetType().GetProperties()` | Runtime property enumeration on `SentryOptions` |
| 70 | `target.GetType().GetProperty(prop.Name)` | Runtime property lookup by name |
| 74–77 | `prop.GetValue(source)` / `targetProp.SetValue(target, value)` | Reflection read/write — AOT blocker |
| 35–40 | `SentrySdk.Init(sentryOptions => { options.Map(sentryOptions); ... })` | If `SentrySdk.Init` itself is not AOT-annotated, it propagates an AOT warning |

---

## Implementation Plan

### Step 1 — Replace the reflection `Map` method

- [ ] Open `src/ISynergy.Framework.OpenTelemetry.Sentry/Extensions/SentryTelemetryExtensions.cs`
- [ ] Replace the private `Map(SentryOptions source, SentryOptions target)` method with explicit property assignments for the `SentryOptions` properties used by this framework. Key properties to map include:
  - `target.Dsn = source.Dsn;`
  - `target.Environment = source.Environment;`
  - `target.Release = source.Release;`
  - `target.SampleRate = source.SampleRate;`
  - `target.TracesSampleRate = source.TracesSampleRate;`
  - `target.MaxBreadcrumbs = source.MaxBreadcrumbs;`
  - `target.Debug = source.Debug;`
  _(Review the Sentry SDK version in use and map all writable properties that are relevant to this integration.)_

### Step 2 — Assess Sentry SDK AOT compatibility

- [ ] Check whether the Sentry NuGet package version in `Directory.Packages.props` ships with trimmer-safe annotations (look for `[RequiresUnreferencedCode]` on `SentrySdk.Init`)
- [ ] If Sentry SDK is not annotated: annotate `AddSentryExporter` with:
  ```csharp
  [RequiresUnreferencedCode("SentrySdk.Init may use reflection internally. Verify AOT compatibility with the Sentry SDK version in use.")]
  [RequiresDynamicCode("Sentry SDK may require dynamic code generation.")]
  public static TelemetryBuilder AddSentryExporter(...)
  ```
- [ ] If Sentry SDK is annotated as AOT-safe: no annotation needed on `AddSentryExporter` itself

### Step 3 — Verify `OpenTelemetryLogProcessor`

- [ ] `OpenTelemetryLogProcessor.cs` uses only string operations and typed `SentryEvent` / `SentrySdk.CaptureEvent` — no reflection in the processor itself
- [ ] If `SentrySdk.CaptureEvent` carries `[RequiresUnreferencedCode]`, annotate `OnEnd` accordingly

### Step 4 — Update XML documentation

- [ ] Add `<remarks>` to `AddSentryExporter` noting AOT status and the Sentry SDK dependency
- [ ] Add `<remarks>` to `Map` noting the explicit-mapping approach

---

## Migration Guide

`ISynergy.Framework.OpenTelemetry.Sentry` replaces reflection-based `SentryOptions` property copying with explicit assignments. `AddSentryExporter` may also carry `[RequiresUnreferencedCode]` if the Sentry SDK is not fully AOT-annotated.

### Before

```csharp
// Internal Map method used reflection to copy SentryOptions properties
// Caused IL2026 warnings in AOT builds
services.AddSentryTelemetryIntegration(configuration);
```

### After

```csharp
// Map method now uses explicit property assignments — no AOT issue from framework code
// If Sentry SDK itself is not AOT-annotated, AddSentryExporter will carry [RequiresUnreferencedCode]
#pragma warning disable IL2026 // Sentry SDK AOT support — verify SDK version
services.AddSentryTelemetryIntegration(configuration);
#pragma warning restore IL2026
```

### Steps to Migrate

1. Rebuild against the updated library. The internal `Map` method no longer uses reflection.
2. Check whether the `Sentry` NuGet package version in `Directory.Packages.props` ships with trimmer-safe annotations on `SentrySdk.Init`.
3. If Sentry is not annotated: add `#pragma warning disable IL2026, IL3050` at the `AddSentryTelemetryIntegration` / `AddSentryExporter` call sites with a justification comment.
4. If Sentry is annotated as AOT-safe: no suppression needed — rebuild and confirm zero IL warnings from this library.
