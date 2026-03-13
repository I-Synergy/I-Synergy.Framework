# ISynergy.Framework.AspNetCore.Globalization AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Resolve the single AOT-incompatible pattern in `ISynergy.Framework.AspNetCore.Globalization` with a targeted annotation-based fix.

**Architecture:** No source generator is required. The only issue is `new ResourceManager(resourceType)` in `LanguageService`, which accepts a `Type` at runtime. This is an established pattern for .NET resource management; the fix is to annotate the public method with `[RequiresUnreferencedCode]` so consumers are warned that the resource type must be preserved by the linker.

**Tech Stack:** .NET 10 / C# 14, `System.Resources.ResourceManager`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.AspNetCore.Globalization/Services/LanguageService.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 29 | `new ResourceManager(resourceType)` — `ResourceManager(Type)` constructor resolves the resource name from `Type.FullName` and then calls `Assembly.GetManifestResourceStream` at runtime. The trimmer cannot statically prove the resource stream will survive tree-shaking. | Medium |

**Notes:**
- `AddResourceManager(Type resourceType)` is called from `ServiceCollectionExtensions.ConfigureServices` (in the Blazor extension package) with concrete, compile-time-known types like `typeof(Framework.Mvvm.Properties.Resources)`. These calls are AOT-safe in practice because the compiler knows the type.
- However, `ILanguageService.AddResourceManager(Type)` is a public API whose contract allows any `Type` to be passed, so the trimmer must be told to preserve the member's annotated requirements.
- The `ResourceManager(Type)` overload is itself annotated `[RequiresUnreferencedCode]` in .NET 6+, so this will already produce an IL2026 warning at the call site without annotation propagation.

---

## Implementation Steps

- [ ] **Step 1 – Annotate `LanguageService.AddResourceManager`**
  - Add `[RequiresUnreferencedCode("ResourceManager requires the resource type and its satellite assemblies to be preserved by the linker.")]` to the method in `LanguageService.cs` (line 29 area).
  - Add the same attribute to `ILanguageService.AddResourceManager(Type)` in the interface definition so callers receive the warning at their call sites.

- [ ] **Step 2 – Update callers in `ISynergy.Framework.AspNetCore.Blazor`**
  - In `ServiceCollectionExtensions.ConfigureServices(…)` (lines 56-58 in the Blazor extension), the three `languageService.AddResourceManager(typeof(…))` calls pass compile-time-known `typeof(…)` arguments and are inherently safe. Add a `#pragma warning disable IL2026` / `IL3050` suppression with a justification comment on those three lines, or propagate `[RequiresUnreferencedCode]` up the call chain if `ConfigureServices` itself should be annotated.

- [ ] **Step 3 – Consider `[GeneratedRegex]` for `ResourceManager`-free alternative (optional)**
  - As a long-term improvement, `LanguageService` could be refactored to accept pre-built `ResourceManager` instances (constructed by callers using the strongly-typed generated `Properties.Resources.ResourceManager` property, which is trim-safe). This would remove the `[RequiresUnreferencedCode]` annotation entirely. This is optional in this pass.

- [ ] **Step 4 – Verify build**
  - Run `dotnet build src/ISynergy.Framework.AspNetCore.Globalization` and confirm zero IL2026 warnings with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.

---

## Migration Guide

`ILanguageService.AddResourceManager(Type)` and its implementation in `LanguageService` are now annotated with `[RequiresUnreferencedCode]`. Callers that pass a compile-time-known `typeof(...)` are safe and only need a one-line suppression; callers in AOT-published code should switch to passing a pre-built `ResourceManager` instance.

### Before

```csharp
// In Blazor ServiceCollectionExtensions — no suppression needed previously
languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
languageService.AddResourceManager(typeof(MyApp.Properties.Resources));
```

### After

```csharp
// Option A: suppress warning at the known-safe call site
#pragma warning disable IL2026 // typeof() is statically known; resource type is preserved
languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
languageService.AddResourceManager(typeof(MyApp.Properties.Resources));
#pragma warning restore IL2026

// Option B (fully AOT-safe): pass a pre-built ResourceManager
var rm = ISynergy.Framework.Mvvm.Properties.Resources.ResourceManager; // trim-safe property
languageService.AddResourceManager(rm);
```

### Steps to Migrate

1. Locate every call site of `ILanguageService.AddResourceManager(Type)` in your application.
2. If the `typeof(...)` argument is a compile-time constant (which it always should be), add a `#pragma warning disable IL2026` suppression with a comment explaining the safety justification.
3. For a fully AOT-safe alternative, refactor `ILanguageService` to accept a pre-built `ResourceManager` instance (the strongly-typed `.ResourceManager` property on generated resource classes is trim-safe).
4. Do not pass a runtime-resolved `Type` to `AddResourceManager` in AOT-published applications.
