# ISynergy.Framework.Printer.Label.Dymo AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility of `ISynergy.Framework.Printer.Label.Dymo` given its dependency on the `DymoSDK` package.
**Architecture:** The library's own code (`LabelPrinterService.cs`, `ServiceCollectionExtensions.cs`) contains no direct reflection or `dynamic`. The AOT concern is the `DymoSDK` package — it wraps COM interop or native DLL calls and may use `Activator.CreateInstance` or reflection internally for printer enumeration and label templating. The library currently uses `DymoSDK.App.Init()`, `DymoLabel.LabelSharedInstance`, and `DymoPrinter.Instance` which are static singletons; these patterns are typically reflection-heavy in COM-wrapper SDKs. The fix is to annotate the service class and registration as AOT-incompatible.
**Tech Stack:** .NET 10, C# 14, `DymoSDK`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Printer.Label.Dymo/Services/LabelPrinterService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 24 | `DymoSDK.App.Init()` | DymoSDK initialization — likely uses reflection or COM interop internally |
| 26 | `DymoLabel.LabelSharedInstance` | Static singleton from DymoSDK — COM/reflection-based printer access |
| 38 | `DymoPrinter.Instance.GetPrinters()` | Runtime printer enumeration via DymoSDK — likely not AOT-annotated |
| 43–45 | `DymoPrinter.Instance.PrintLabel(...)` | Native print dispatch — reflection/COM-based |

---

## Implementation Plan

### Step 1 — Verify DymoSDK AOT status

- [ ] Check the DymoSDK package version in `Directory.Packages.props`
- [ ] DymoSDK is a Windows-only SDK wrapping the Dymo Web Service or COM objects — very unlikely to carry AOT annotations
- [ ] Confirm by checking whether the package ships a `<packageId>.trimmerroot.xml` or carries `[RequiresUnreferencedCode]` on its public API

### Step 2 — Annotate `LabelPrinterService`

- [ ] Open `src/ISynergy.Framework.Printer.Label.Dymo/Services/LabelPrinterService.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the class:
  ```csharp
  [RequiresUnreferencedCode("DymoSDK uses COM interop and reflection for printer access. Not compatible with AOT publishing.")]
  [RequiresDynamicCode("DymoSDK requires dynamic code generation for COM/native interop.")]
  internal class LabelPrinterService : ILabelPrinterService
  ```

### Step 3 — Annotate `ServiceCollectionExtensions`

- [ ] Open `src/ISynergy.Framework.Printer.Label.Dymo/Extensions/ServiceCollectionExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the registration method to propagate the constraint:
  ```csharp
  [RequiresUnreferencedCode("Registers LabelPrinterService which depends on DymoSDK COM interop.")]
  [RequiresDynamicCode("DymoSDK requires dynamic code generation.")]
  public static IServiceCollection AddDymoLabelPrinterIntegration(...)
  ```

### Step 4 — Update XML documentation

- [ ] Add `<remarks>` to both annotated members noting the DymoSDK AOT limitation and that the service is Windows-only

### Notes
- The DymoSDK is inherently Windows-only; AOT publishing for this library would only make sense for Windows NativeAOT scenarios, which currently have limited COM interop support regardless of SDK annotations.
- If `DymoSDK` does carry AOT annotations in future versions, the annotations added here can be removed.

---

## Migration Guide

`ISynergy.Framework.Printer.Label.Dymo` is annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` because `DymoSDK` uses COM interop and reflection internally. AOT-publishing applications cannot use this library.

### Before

```csharp
// Program.cs — no AOT warning previously
services.AddDymoLabelPrinterIntegration(configuration);
```

### After

```csharp
// For AOT-published applications: this library cannot be used
// Implement label printing via a COM-free alternative

// For non-AOT Windows applications: add suppression
#pragma warning disable IL2026, IL3050 // DymoSDK uses COM interop — not AOT-safe
services.AddDymoLabelPrinterIntegration(configuration);
#pragma warning restore IL2026, IL3050
```

### Steps to Migrate

1. For applications targeting `<PublishAot>true</PublishAot>`: do not use this library. COM interop is not supported in NativeAOT on Windows without specific P/Invoke wrappers, and DymoSDK does not provide those.
2. For non-AOT Windows applications: add `#pragma warning disable IL2026, IL3050` with a justification comment around `AddDymoLabelPrinterIntegration`.
3. The `ILabelPrinterService` interface is unchanged — switching to an alternative implementation does not require changes to consuming code beyond the DI registration.
4. Monitor DymoSDK releases for AOT/NativeAOT support. If Dymo provides an AOT-compatible SDK in the future, the framework annotations can be removed without API changes.
