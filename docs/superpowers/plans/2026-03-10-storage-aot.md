# ISynergy.Framework.Storage AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document the AOT compatibility status of the base `ISynergy.Framework.Storage` abstractions library.

**Architecture:** This library contains a single public file: the `IStorageService` interface. It defines four async methods operating on raw `byte[]`, `string`, and `Uri` primitives. There is no serialization, no reflection, no assembly scanning, and no dynamic code in this library.

**Tech Stack:** .NET 10 / C# 14.

---

## AOT Issues Found

**None.**

`ISynergy.Framework.Storage` is a pure-abstraction library containing only `IStorageService`. All method signatures use primitives (`byte[]`, `string`, `bool`, `Uri`, `CancellationToken`) and standard BCL types. There are no AOT blockers of any kind.

---

## Assessment

`ISynergy.Framework.Storage` is **fully AOT-compatible** with no changes required.

The library is an ideal foundation for AOT-safe consuming code: the `IStorageService` contract expresses no polymorphism over serialized payloads, has no generic type parameters that would require runtime metadata, and has no DI registration code.

No implementation steps are required. The library can be shipped as-is for AOT scenarios.

---

## Migration Guide

`ISynergy.Framework.Storage` (the abstractions library) is fully AOT-compatible. No code changes or migration steps are required.

### Before

```csharp
// No changes needed
services.AddScoped<IStorageService, MyStorageService>();
```

### After

```csharp
// Same — IStorageService is a pure interface with no AOT-incompatible patterns
services.AddScoped<IStorageService, MyStorageService>();
```

### Steps to Migrate

1. No migration steps required for this library.
2. Verify that the concrete `IStorageService` implementation you register (e.g., `ISynergy.Framework.Storage.Azure`) is also AOT-compatible — see the Azure storage migration guide.
3. If you serialize storage results (byte arrays, URIs) to JSON, register the relevant types with `[JsonSerializable]` in your application's `JsonSerializerContext`.
