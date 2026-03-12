# ISynergy.Framework.Documents.Syncfusion AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Document AOT limitations imposed by third-party Syncfusion dependencies and annotate the library accordingly.
**Architecture:** The library's own code (`DocumentService.cs`, `ServiceCollectionExtensions.cs`, `SyncfusionLicenseOptions.cs`) contains no direct reflection or `dynamic`. However, the Syncfusion packages it depends on (`Syncfusion.DocIO`, `Syncfusion.DocIORenderer`, `Syncfusion.XlsIO`) are not AOT-compatible — they use extensive internal reflection for document templating (MailMerge), charting, and Excel I/O. The correct fix is to annotate the DI registration extension and the `DocumentService` class with `[RequiresUnreferencedCode]` / `[RequiresDynamicCode]` to propagate the constraint to callers.
**Tech Stack:** .NET 10, C# 14, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Documents.Syncfusion/Services/DocumentService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| Entire class | Syncfusion MailMerge, DocIO, XlsIO | Third-party packages use reflection internally; the consuming class inherits that constraint |
| 116 | `new MailMergeDataTable(nameof(documentRequest.Document), documentRequest.Document)` | `MailMergeDataTable` reflects over the passed object's properties at runtime |
| 122 | `new MailMergeDataTable(nameof(documentRequest.DocumentDetails), documentRequest.DocumentDetails)` | Same — runtime property reflection on generic `TDetails` |

### `src/ISynergy.Framework.Documents.Syncfusion/Extensions/ServiceCollectionExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 25 | `AddDocumentsSyncfusionIntegration` | Registers `DocumentService` which transitively requires dynamic code |

---

## Implementation Plan

### Step 1 — Annotate `DocumentService`

- [ ] Open `src/ISynergy.Framework.Documents.Syncfusion/Services/DocumentService.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the class declaration:
  ```csharp
  [RequiresUnreferencedCode("DocumentService uses Syncfusion libraries that rely on reflection for MailMerge and document processing. Not compatible with AOT publishing.")]
  [RequiresDynamicCode("Syncfusion DocIO and XlsIO libraries require dynamic code generation.")]
  internal class DocumentService : IDocumentService
  ```

### Step 2 — Annotate `ServiceCollectionExtensions`

- [ ] Open `src/ISynergy.Framework.Documents.Syncfusion/Extensions/ServiceCollectionExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate `AddDocumentsSyncfusionIntegration`:
  ```csharp
  [RequiresUnreferencedCode("Registers DocumentService which requires Syncfusion reflection-based libraries.")]
  [RequiresDynamicCode("Syncfusion libraries require dynamic code generation.")]
  public static IServiceCollection AddDocumentsSyncfusionIntegration(...)
  ```

### Step 3 — Update XML documentation

- [ ] Add `<remarks>` to both annotated members noting that AOT-published applications cannot use this library and should implement a native document generation alternative

### Notes
- No source generator is needed. The annotations propagate the AOT-incompatibility signal to calling code, which is the correct library contract.
- If Syncfusion ever provides AOT-compatible packages, the annotations can be removed without breaking the public API surface.

---

## Migration Guide

`ISynergy.Framework.Documents.Syncfusion` is annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` because Syncfusion's DocIO, XlsIO, and MailMerge libraries use reflection internally. AOT-publishing applications cannot use this library until Syncfusion provides AOT-compatible packages.

### Before

```csharp
// Program.cs — no AOT warning previously
services.AddDocumentsSyncfusionIntegration(configuration);
```

### After

```csharp
// For AOT-published applications: this library cannot be used
// Implement a native document generation alternative

// For non-AOT applications: add suppression
#pragma warning disable IL2026, IL3050 // Syncfusion uses reflection-based MailMerge/DocIO
services.AddDocumentsSyncfusionIntegration(configuration);
#pragma warning restore IL2026, IL3050
```

### Steps to Migrate

1. For applications targeting `<PublishAot>true</PublishAot>`: do not use this library. Implement document generation using AOT-compatible alternatives (e.g., plain HTML-to-PDF via a native library, or a server-side non-AOT document generation service).
2. For non-AOT applications: add `#pragma warning disable IL2026, IL3050` with a justification comment around `AddDocumentsSyncfusionIntegration`.
3. Monitor Syncfusion release notes for AOT/NativeAOT support. When available, the framework annotations can be removed and AOT support restored without API changes.
4. The `IDocumentService` abstraction interface is unchanged and unaffected by this limitation.
