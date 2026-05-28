# ISynergy.Framework.Documents AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm that `ISynergy.Framework.Documents` (the abstractions-only library) is fully AOT-compatible with no code changes required.
**Architecture:** No changes needed — the library contains only interfaces, enumerations, and simple model classes with no reflection, no JSON serialization, no dynamic dispatch, and no assembly scanning.
**Tech Stack:** .NET 10, C# 14.

---

## AOT Assessment: Compatible

`ISynergy.Framework.Documents` contains 7 source files: `IDocumentService` (interface), `DocumentTypes` (enum), `BaseRequest`, `DocumentRequest`, `SpreadsheetRequest` (model records/classes). All are pure data contracts and interfaces with no reflection, no `dynamic`, no `JsonSerializer`, no `Activator.CreateInstance`, and no assembly scanning. The library is fully AOT-compatible as written.

---

## Migration Guide

`ISynergy.Framework.Documents` is fully AOT-compatible. No code changes or migration steps are required.

### Before

```csharp
// No changes needed
services.AddScoped<IDocumentService, MyDocumentService>();
var request = new DocumentRequest { Template = "invoice.docx" };
```

### After

```csharp
// Same — pure contracts and request models
services.AddScoped<IDocumentService, MyDocumentService>();
var request = new DocumentRequest { Template = "invoice.docx" };
```

### Steps to Migrate

1. No migration steps required for this abstractions library.
2. Verify that the concrete `IDocumentService` implementation (e.g., `ISynergy.Framework.Documents.Syncfusion`) is AOT-compatible — see the Syncfusion migration guide.
3. If you serialize `DocumentRequest` to JSON, register it with `[JsonSerializable]` in your application's `JsonSerializerContext`.
