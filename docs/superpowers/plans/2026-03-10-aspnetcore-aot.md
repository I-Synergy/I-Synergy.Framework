# ISynergy.Framework.AspNetCore AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Annotate and fix all AOT-incompatible patterns in `ISynergy.Framework.AspNetCore` so the library builds cleanly under `PublishAot=true` without trimmer warnings.

**Architecture:** No source generator is required. All issues are simple annotation-based fixes: replacing unregistered-type `GetService(typeof(...))` calls with their generic equivalents, annotating reflection-based `GetType().GetProperties()` calls with `[RequiresUnreferencedCode]`, and adding `[JsonSerializable]` context registrations for the anonymous-type JSON serialisation in the health-check helper. Each fix is self-contained to a single file.

**Tech Stack:** .NET 10 / C# 14, `System.Text.Json` source-generation (`JsonSerializerContext`), `System.Diagnostics.CodeAnalysis` trimmer attributes.

---

## AOT Issues Found

### `src/ISynergy.Framework.AspNetCore/Extensions/HtmlHelperExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 59 | `GetService(typeof(IWebHostEnvironment))` — non-generic DI resolution (trimmer cannot see the type) | Medium |
| 82 | `attributes?.GetType().GetProperties()` — open-ended runtime reflection over an arbitrary `object` parameter; property set cannot be statically determined | High |

**Notes:**
- Line 59 should be replaced with the generic `GetService<IWebHostEnvironment>()`.
- Line 82 is an `object? attributes` parameter used to build HTML attribute strings. The method is an internal Razor helper unlikely to be called in AOT scenarios, but it must be annotated with `[RequiresUnreferencedCode]` (and `[RequiresDynamicCode]`) to suppress linker errors. Alternatively, the `object attributes` overload could be replaced with an `IDictionary<string, object?>` overload, which is fully AOT-safe and removes reflection entirely.

---

### `src/ISynergy.Framework.AspNetCore/Extensions/ImageHelperExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 22 | `GetService(typeof(IWebHostEnvironment))` — non-generic DI resolution | Medium |
| 26 | `attributes?.GetType().GetProperties()` — same open-ended reflection pattern as `HtmlHelperExtensions` | High |

**Notes:** Same fixes as above apply.

---

### `src/ISynergy.Framework.AspNetCore/Extensions/HealthCheckExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 28–44 | `JsonSerializer.SerializeAsync(…, response, DefaultJsonSerializers.Web)` where `response` is an **anonymous type** (`new { Status, Duration, Info = … }`). Anonymous types are not visible to the static JSON source generator and will trigger trimmer warnings at publish time. | High |

**Notes:** The anonymous type must be replaced with a named `sealed record` (e.g., `HealthCheckResponse` and `HealthCheckEntryResponse`) and a `JsonSerializerContext` with `[JsonSerializable]` attributes registered for those types, or the call must be annotated with `[RequiresUnreferencedCode]`.

---

### `src/ISynergy.Framework.AspNetCore/Extensions/ResultExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 13 | `JsonSerializer.Deserialize<Result<T>>(…, DefaultJsonSerializers.Web)` — `Result<T>` is an open generic; without a `JsonSerializerContext` the trimmer may strip serialisation metadata | Low-Medium |
| 20 | Same for `Result` | Low-Medium |
| 27 | Same for `PaginatedResult<T>` | Low-Medium |

**Notes:** `DefaultJsonSerializers.Web` is a plain `JsonSerializerOptions` instance — not a source-generated context. In full-AOT publishing these will produce IL2026 / IL3050 warnings. The fix is either:
1. Create a `JsonSerializerContext` in `ISynergy.Framework.Core` that covers `Result`, `Result<T>`, and `PaginatedResult<T>`, or
2. Annotate `ToResult<T>`, `ToResult`, and `ToPaginatedResult<T>` with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`.

---

### `src/ISynergy.Framework.AspNetCore/Extensions/HttpContentExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 24 | `JsonSerializer.Deserialize<T>(json, _options)` — `_options` is a plain `new JsonSerializerOptions(…)`, not a source-generated context; `T` is unconstrained | Low-Medium |

**Notes:** Annotate `ReadAsAsync<T>` with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`, or provide a `JsonTypeInfo<T>` overload.

---

## Implementation Steps

- [ ] **Step 1 – Fix non-generic DI resolution in `HtmlHelperExtensions` and `ImageHelperExtensions`**
  - Replace `RequestServices.GetService(typeof(IWebHostEnvironment))` with `RequestServices.GetService<IWebHostEnvironment>()` on lines 59 (`HtmlHelperExtensions.cs`) and 22 (`ImageHelperExtensions.cs`).
  - This is a fully AOT-safe call; the trimmer can see the type argument statically.

- [ ] **Step 2 – Fix reflection-based attribute-to-HTML helpers**
  - In `HtmlHelperExtensions.cs` `ConvertArrayToHtmlString(…, object? attributes)` (line 82) and `ImageHelperExtensions.cs` `InlineImageAsync(…, object? attributes)` (line 26):
  - **Option A (preferred):** Add an `IDictionary<string, object?>` overload and mark the `object?` overload `[Obsolete]` plus `[RequiresUnreferencedCode("Reflects over arbitrary attribute object type")]` and `[RequiresDynamicCode("Reflects over arbitrary attribute object type")]`.
  - **Option B:** Mark only with trimmer/dynamic-code attributes if no refactoring is desired in this pass.

- [ ] **Step 3 – Replace anonymous type in `HealthCheckExtensions.MapTelemetryHealthChecks`**
  - Add `internal sealed record HealthCheckEntryResponse(string Key, string? Description, string Status, TimeSpan Duration, IReadOnlyDictionary<string, object> Data);`
  - Add `internal sealed record HealthCheckResponse(string Status, TimeSpan Duration, IEnumerable<HealthCheckEntryResponse> Info);`
  - Add `[JsonSerializable(typeof(HealthCheckResponse))]` to a new `internal partial class HealthCheckJsonContext : JsonSerializerContext` in the same file or a dedicated `HealthCheckJsonContext.cs`.
  - Update the `SerializeAsync` call to use `HealthCheckJsonContext.Default.HealthCheckResponse`.

- [ ] **Step 4 – Annotate JSON deserialization helpers in `ResultExtensions.cs`**
  - Add `[RequiresUnreferencedCode("JSON deserialization of open-generic Result types requires metadata preservation.")]` and `[RequiresDynamicCode("…")]` to `ToResult<T>`, `ToResult`, and `ToPaginatedResult<T>`.
  - Long-term: wire these to a source-generated `JsonSerializerContext` from `ISynergy.Framework.Core` once one is established.

- [ ] **Step 5 – Annotate `HttpContentExtensions.ReadAsAsync<T>`**
  - Add `[RequiresUnreferencedCode("JSON deserialization requires type metadata to be preserved.")]` and `[RequiresDynamicCode("…")]` to `ReadAsAsync<T>`.

- [ ] **Step 6 – Verify build**
  - Run `dotnet build src/ISynergy.Framework.AspNetCore` and confirm zero IL2026 / IL3050 warnings with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.

---

## Migration Guide

`ISynergy.Framework.AspNetCore` replaces anonymous-type JSON serialization in health checks with named records and a source-generated `JsonSerializerContext`, and annotates helper methods that use runtime reflection. JSON deserialization helpers (`ToResult<T>`, `ReadAsAsync<T>`) are annotated with `[RequiresUnreferencedCode]`.

### Before

```csharp
// Health check response used anonymous types (AOT-unsafe)
await JsonSerializer.SerializeAsync(context.Response.Body, new { Status = "Healthy" });

// HTTP content deserialization used reflection-based options
var result = await response.Content.ReadAsAsync<MyDto>();

// HTML helper accepted object attributes via reflection
helper.ConvertArrayToHtmlString(items, new { @class = "list" });
```

### After

```csharp
// Health check now uses named records serialized via HealthCheckJsonContext
// (internal change; no consumer-facing API change)

// ReadAsAsync<T> is annotated [RequiresUnreferencedCode]; suppress or avoid in AOT:
// Option A: suppress at call site
#pragma warning disable IL2026
var result = await response.Content.ReadAsAsync<MyDto>();
#pragma warning restore IL2026

// Option B: pass a JsonTypeInfo<T> overload (if added in future minor version)

// HTML helper: prefer the IDictionary<string, object?> overload for AOT safety
helper.ConvertArrayToHtmlString(items,
    new Dictionary<string, object?> { ["class"] = "list" });
```

### Steps to Migrate

1. In health check pipelines, no action is required — the serialization now uses the internal `HealthCheckJsonContext` automatically.
2. Where `ToResult<T>`, `ToResult`, `ToPaginatedResult<T>`, or `ReadAsAsync<T>` are called in AOT-published code, add `#pragma warning disable IL2026 / IL3050` with a justification, or replace with a `JsonTypeInfo<T>`-aware overload when one is available.
3. Replace any `object? attributes` overloads in HTML helpers with `IDictionary<string, object?>` overloads to avoid the runtime-reflection path entirely.
4. Replace `GetService(typeof(...))` calls with the generic `GetService<T>()` equivalent — the non-generic overloads are now replaced in `HtmlHelperExtensions` and `ImageHelperExtensions`.
