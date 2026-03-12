# ISynergy.Framework.Storage.S3 AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Identify and document AOT compatibility concerns in `ISynergy.Framework.Storage.S3`, focusing on the AWS SDK usage patterns that are known to be problematic in trimmed/AOT builds.

**Architecture:** The library contains three source files: `S3StorageService` (an `internal` implementation of `IStorageService` using `AWSSDK.S3`), `S3StorageOptions` (a POCO options class), and `ServiceCollectionExtensions`. The framework code itself contains no reflection or serialization; however, the `AWSSDK.S3` dependency has historically used reflection-based serialization for its REST API request/response marshalling and is the primary AOT risk.

**Tech Stack:** .NET 10 / C# 14, `AWSSDK.S3`, `AWSSDK.Core`, central package management via `Directory.Packages.props`.

---

## AOT Issues Found

### Framework code in `S3StorageService.cs`

**None in the framework source code itself.**

After full inspection:
- All operations delegate to typed `AmazonS3Client` methods (`PutObjectAsync`, `GetObjectAsync`, `GetObjectMetadataAsync`, `DeleteObjectAsync`) using strongly-typed request/response objects.
- No `JsonSerializer` usage, no reflection, no `GetType()`, no `Expression.Lambda`.
- DI registration uses `TryAddSingleton<IStorageService, S3StorageService>()` — AOT-safe.

### AWS SDK AOT concern (dependency-level, not framework code)

| Component | Pattern | Severity |
|-----------|---------|----------|
| `AWSSDK.Core` + `AWSSDK.S3` | Internal marshalling uses reflection to read/write property values on request/response types | **High** (SDK concern, not framework) |

**Detail:** The AWS SDK for .NET (AWSSDK.Core 3.x) marshals REST API payloads using `System.Reflection` to enumerate properties of request objects like `PutObjectRequest`, `GetObjectRequest`, etc. As of early 2026, the AWS SDK does **not** have full Native AOT support. Microsoft's guidance and the AWS SDK team's own documentation acknowledge this limitation.

The affected `AmazonS3Client` calls in `S3StorageService` are:
- Line 103: `_client.PutObjectAsync(request, cancellationToken)` — SDK marshals `PutObjectRequest` via reflection
- Line 137: `_client.GetObjectAsync(request, cancellationToken)` — SDK marshals `GetObjectRequest` via reflection
- Line 242: `_client.GetObjectMetadataAsync(bucketName, key, cancellationToken)` — reflection-based
- Line 257: `_client.DeleteObjectAsync(request, cancellationToken)` — reflection-based

---

## Assessment

`ISynergy.Framework.Storage.S3` **contains no AOT issues in the framework library source code**. All framework-authored code is AOT-safe.

The AOT risk is entirely at the `AWSSDK.S3` / `AWSSDK.Core` dependency level. This is a known limitation of the AWS SDK for .NET at the time of writing (March 2026).

---

## Recommended Actions

### Phase 1 — Document the SDK AOT limitation

- [ ] Add a `<remarks>` XML documentation block to `ServiceCollectionExtensions.AddS3StorageIntegration` warning that the `AWSSDK.S3` dependency does not currently support Native AOT and trimming:
  ```csharp
  /// <remarks>
  /// <para>
  /// <strong>AOT/Trimming notice:</strong> The <c>AWSSDK.S3</c> dependency uses reflection-based
  /// HTTP marshalling internally and does not currently support Native AOT publishing or
  /// aggressive trimming. Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c>
  /// should use Azure Blob Storage (<see cref="ISynergy.Framework.Storage.Azure"/> ) or
  /// another storage provider with AOT-compatible SDK support until AWS SDK AOT support is
  /// available.
  /// </para>
  /// </remarks>
  ```

### Phase 2 — Track AWS SDK AOT progress

- [ ] Monitor the [aws/aws-sdk-net GitHub](https://github.com/aws/aws-sdk-net) for Native AOT support, tracking issue [#3329](https://github.com/aws/aws-sdk-net/issues/3329) or equivalent.
- [ ] When a version of `AWSSDK.S3` / `AWSSDK.Core` is released with AOT annotations and source-generated marshalling, update `Directory.Packages.props` to the AOT-compatible minimum version and remove the `<remarks>` warning.

### Phase 3 — Consider `rd.xml` / trim root workaround (optional, non-AOT trimming only)

For **trimmed** (non-AOT) publish scenarios where the linker removes SDK reflection targets:
- [ ] Evaluate adding a `TrimmerRootDescriptor.xml` file to the consuming application side (not this library) that preserves the AWS SDK's request/response types. This is a workaround, not a true fix, and should be documented as such.

---

## No Source Generator Required

The framework code has no assembly scanning, no `MakeGenericType`, and no message-type discovery. A source generator would not help with the AWS SDK's internal marshalling, which is entirely outside the scope of this framework library.

---

## Migration Guide

`ISynergy.Framework.Storage.S3` framework code is AOT-safe. The blocking issue for AOT publishing is the `AWSSDK.S3` / `AWSSDK.Core` SDK dependency, which uses reflection-based HTTP marshalling. Until AWS releases an AOT-compatible SDK, AOT-publishing applications should not use this storage provider.

### Before

```csharp
// Program.cs — S3 storage registered normally
services.AddS3StorageIntegration(configuration);
// Works in non-AOT builds; fails silently or throws at runtime in AOT builds
```

### After

```csharp
// For AOT-published applications: use Azure Blob Storage instead
services.AddAzureStorageIntegration(configuration);

// For non-AOT applications: S3 is unchanged
services.AddS3StorageIntegration(configuration);
```

### Steps to Migrate

1. If your application targets `<PublishAot>true</PublishAot>`, replace `AddS3StorageIntegration` with an AOT-compatible storage provider (`AddAzureStorageIntegration` or a custom implementation).
2. Monitor the [aws/aws-sdk-net GitHub](https://github.com/aws/aws-sdk-net) issue tracker for Native AOT support. When an AOT-compatible version is released, update `Directory.Packages.props` and remove this limitation.
3. For trimmed (non-AOT) S3 publish scenarios, consider adding a `TrimmerRootDescriptor.xml` to the application project preserving `AWSSDK.S3` request/response types.
4. No changes are needed to the framework library code itself.
