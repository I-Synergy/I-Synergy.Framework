# ISynergy.Framework.Storage.Azure AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document the AOT compatibility status of `ISynergy.Framework.Storage.Azure`, and identify any trimmer-relevant patterns in the Azure SDK usage.

**Architecture:** The library contains two files: `AzureStorageService` (an `internal` implementation of `IStorageService`) and `ServiceCollectionExtensions`. Both are straightforward: `AzureStorageService` wraps `Azure.Storage.Blobs` SDK calls operating on `byte[]` streams, and the DI extension registers it with a closed generic `TryAddSingleton`. No serialization, no reflection, no generic type discovery occurs in this library itself.

**Tech Stack:** .NET 10 / C# 14, `Azure.Storage.Blobs`.

---

## AOT Issues Found

**None in framework code.**

After full inspection of all source files:

- `AzureStorageService.cs` — uses `Azure.Storage.Blobs` SDK (`BlobContainerClient`, `BlobHttpHeaders`, `BlobRequestConditions`, `ETag`). All operations work on `Stream`/`byte[]` and return `Uri`. No reflection, no JSON serialization, no generic type parameters in the library code.
- `Extensions/ServiceCollectionExtensions.cs` — uses `TryAddSingleton<IStorageService, AzureStorageService>()` which is a closed-generic, AOT-safe DI registration.

---

## Azure SDK AOT Status Note

The `Azure.Storage.Blobs` package (6.x) uses `System.Text.Json` internally for its own wire protocol. As of the Azure SDK for .NET "Azure.Core" 1.38+ and `Azure.Storage.Blobs` 12.19+, the Azure SDK has added AOT/trim compatibility annotations and uses source-generated serialization for its own internal types. Consuming the SDK in an AOT application is supported but may require the latest package versions.

**Action required in consuming applications (not this library):**
- Ensure `Azure.Storage.Blobs` is at version 12.19.0 or later in `Directory.Packages.props`.
- Set `<PublishTrimmed>true</PublishTrimmed>` and verify no ILLink warnings originate from the Azure SDK packages.

---

## Assessment

`ISynergy.Framework.Storage.Azure` is **fully AOT-compatible** from the framework library's perspective with no changes required in the library source.

The only AOT concern in this area lies with the `Azure.Storage.Blobs` SDK dependency itself (a NuGet package version concern for consuming applications), not with any code written in this framework library.

---

## Migration Guide

`ISynergy.Framework.Storage.Azure` is fully AOT-compatible from a framework code perspective. The only action required is ensuring the `Azure.Storage.Blobs` SDK dependency is at a version that supports AOT.

### Before

```csharp
// Program.cs — may use older Azure SDK not AOT-annotated
services.AddAzureStorageIntegration(configuration);
// Directory.Packages.props: Azure.Storage.Blobs 12.x (older)
```

### After

```csharp
// Program.cs — same call; ensure SDK version is up to date
services.AddAzureStorageIntegration(configuration);
// Directory.Packages.props: Azure.Storage.Blobs 12.19.0 or later
```

### Steps to Migrate

1. No framework library code changes required.
2. Update `Azure.Storage.Blobs` to version 12.19.0 or later in `Directory.Packages.props` to get the Azure SDK's own AOT compatibility annotations.
3. When publishing with `<PublishTrimmed>true</PublishTrimmed>`, verify that no `IL2xxx` warnings originate from `Azure.Storage.Blobs` by running `dotnet publish` and reviewing the output.
4. If you use `<PublishAot>true</PublishAot>`, verify the Azure SDK version in use explicitly states NativeAOT support in its release notes.
