# Align MAUI & WinUI Telemetry with WPF/Blazor Pattern

**Status:** PLAN
**Started:** 2026-06-14

## Context

Telemetry setup is inconsistent across UI frameworks. WPF and Blazor follow a clean `ILoggingBuilder` extension pattern with `TelemetryBuilder` return for fluent exporter chaining. MAUI and WinUI instead inline all OpenTelemetry setup inside monolithic `ConfigureLogging` methods — duplicating `SetResourceBuilder()` 3×, using compile-time `#if DEBUG` instead of runtime environment checks, hardcoding log levels, and not returning `TelemetryBuilder` (preventing `.AddOtlpExporter()` etc. chaining).

The untracked MAUI `TelemetryExtensions.cs` references `ApplicationMonitor` which doesn't exist yet in the MAUI project (compile error). Need to create a MAUI-specific `ApplicationMonitor` alongside the alignment.

## Goal

Make MAUI and WinUI `TelemetryExtensions.AddTelemetry()` extend `ILoggingBuilder`, return `TelemetryBuilder`, use `ConfigureResource()` once, use runtime `hostEnvironment.IsDevelopment()`, register a MAUI-specific `ApplicationMonitor`, and support fluent exporter chaining — matching the WPF/Blazor pattern.

## Design Decisions

- **`AddTelemetry` extends `ILoggingBuilder`** (not `MauiAppBuilder` / `IHostApplicationBuilder`)
- **Parameters**: `IConfiguration` + `IHostEnvironment` instead of `HostBuilderContext` (MAUI/WinUI builders don't expose `HostBuilderContext` but do expose `.Configuration` and `.Environment` separately)
- **`ConfigureLogging` retains its signature** for backward compat, but delegates internally to the new `AddTelemetry`
- **MAUI `ApplicationMonitor`** created in `src/ISynergy.Framework.UI.Maui/OpenTelemetry/ApplicationMonitor.cs`, namespace `ISynergy.Framework.UI.OpenTelemetry`. Same metric surface as WPF (render/layout histograms, periodic memory/CPU/thread gauges) but no dispatcher hooks (MAUI's `IDispatcher` doesn't expose them). No `RenderingPerformanceMonitor` for now (no MAUI equivalent of `CompositionTarget.Rendering`).

## Steps

### Step 1: Create MAUI `ApplicationMonitor`
**File:** `src/ISynergy.Framework.UI.Maui/OpenTelemetry/ApplicationMonitor.cs` (NEW)

- Namespace: `ISynergy.Framework.UI.OpenTelemetry`
- Metric prefix: `maui.*` (rendering, layout, memory, cpu, threads)
- App name from `Microsoft.Maui.AppInfo.Current?.Name` with fallback to assembly name
- Instruments: `maui.render.time` histogram, `maui.layout.time` histogram, `maui.memory.usage.mb` gauge, `maui.cpu.usage.percent` gauge, `maui.threads.count` gauge
- Periodic timer (30s) for system metrics (same as WPF/WinUI)
- `RecordRenderTime(double)` and `RecordLayoutTime(double)` public methods
- `IDisposable` (dispose timer, meter)
- No dispatcher operations counter (MAUI `IDispatcher` has no hooks system)
- Uses `OpenTelemetry.Sdk.CreateMeterProviderBuilder().AddInstrumentation<Meter>()` for self-registration

### Step 2: Rewrite MAUI `TelemetryExtensions.cs`
**File:** `src/ISynergy.Framework.UI.Maui/Extensions/TelemetryExtensions.cs`

- Change `this MauiAppBuilder` → `this ILoggingBuilder`
- Replace `HostBuilderContext hostBuilderContext` → `IConfiguration configuration, IHostEnvironment hostEnvironment`
- Add `using ISynergy.Framework.UI.OpenTelemetry;` to resolve `ApplicationMonitor`
- KEEP `builder.Services.TryAddSingleton<ApplicationMonitor>()` (now resolves to MAUI's own monitor)
- Use `hostEnvironment.IsDevelopment()` instead of `#if DEBUG`
- Use `configuration.GetDefaultLogLevel()` instead of hardcoded `LogLevel.Trace`
- Keep `ConfigureResource()` once, return `TelemetryBuilder`

### Step 3: Refactor MAUI `ConfigureLogging` to delegate
**File:** `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs`

- Replace body of first `ConfigureLogging` overload (lines 40–119) with delegation:
  ```csharp
  appBuilder.Logging.AddTelemetry(appBuilder.Configuration, appBuilder.Environment, infoService, ...);
  return appBuilder;
  ```
- Remove now-unused usings: `OpenTelemetry.Logs`, `OpenTelemetry.Metrics`, `OpenTelemetry.Resources`, `OpenTelemetry.Trace`

### Step 4: Create WinUI framework `TelemetryExtensions.cs`
**File:** `src/ISynergy.Framework.UI.WinUI/Extensions/TelemetryExtensions.cs` (NEW)

- Same pattern as MAUI Step 2 — extend `ILoggingBuilder`, take `IConfiguration` + `IHostEnvironment`
- Promoted from `samples/Sample.WinUI/Extensions/TelemetryExtensions.cs`
- No `ApplicationMonitor` registration (WinUI sample's monitor stays in the sample — it references WinRT APIs not available in the framework)

### Step 5: Refactor WinUI `ConfigureLogging` to delegate
**File:** `src/ISynergy.Framework.UI.WinUI/Extensions/WindowsAppBuilderExtensions.cs`

- Replace body of first `ConfigureLogging` overload (lines 43–114) with delegation to `AddTelemetry`
- Remove now-unused OpenTelemetry usings

### Step 6: Delete sample WinUI `TelemetryExtensions.cs`
**File:** `samples/Sample.WinUI/Extensions/TelemetryExtensions.cs` — DELETE (promoted to framework)

### Step 7: Optionally update samples to fluent pattern
**Files:** `samples/Sample.Maui/MauiProgram.cs`, `samples/Sample.WinUI/Program.cs`
- Add comment demonstrating `builder.Logging.AddTelemetry(...).AddOtlpExporter()` pattern
- Keep existing `ConfigureLogging(...)` calls (still work, backward compat)

## Verification

1. `dotnet build src/ISynergy.Framework.UI.Maui/` — no compile errors, `ApplicationMonitor` resolves
2. `dotnet build src/ISynergy.Framework.UI.WinUI/` — new TelemetryExtensions compiles
3. `dotnet build samples/Sample.Maui/` — existing calls still resolve
4. `dotnet build samples/Sample.WinUI/` — existing calls still resolve
5. Grep for `#if DEBUG` in `*TelemetryExtensions.cs` — should be absent
6. Grep for `SetResourceBuilder` in rewritten files — should be absent (replaced by single `ConfigureResource`)
7. Grep for `ConfigureResource` in new `AddTelemetry` methods — should appear exactly once each
