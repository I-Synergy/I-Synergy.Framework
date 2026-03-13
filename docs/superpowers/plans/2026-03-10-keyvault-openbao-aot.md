# ISynergy.Framework.KeyVault.OpenBao AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility of `ISynergy.Framework.KeyVault.OpenBao` given its dependency on VaultSharp and its use of `JsonDocument` for local state parsing.
**Architecture:** Three concerns exist: (1) `OpenBaoVaultTokenProvider` uses `JsonDocument.Parse` + `GetProperty` — this is `System.Text.Json.JsonDocument` which is trimmer-safe as it operates on the raw DOM. (2) `OpenBaoKeyVaultService` uses `VaultSharp` which uses `System.Text.Json` internally and may not carry full AOT annotations. (3) `DataProtection/OpenBaoDataProtectionRepository.cs` uses ASP.NET Core Data Protection which is AOT-annotated in .NET 8+. The main concern is VaultSharp's internal serialization; the framework code itself is clean.
**Tech Stack:** .NET 10, C# 14, VaultSharp, `System.Text.Json`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.KeyVault.OpenBao/Services/OpenBaoVaultTokenProvider.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 33 | `JsonDocument.Parse(File.ReadAllText(stateFilePath))` | `JsonDocument` DOM API — trimmer-safe (no type-based deserialization) |
| 34 | `doc.RootElement.GetProperty("RootToken").GetString()` | String-keyed property access — trimmer-safe |

No AOT issues in `OpenBaoVaultTokenProvider`.

### `src/ISynergy.Framework.KeyVault.OpenBao/Services/OpenBaoKeyVaultService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| All methods | VaultSharp `IVaultClient` calls | VaultSharp uses `System.Text.Json` with reflection-based (de)serialization internally; may not be AOT-annotated depending on VaultSharp version |

### `src/ISynergy.Framework.KeyVault.OpenBao/Extensions/ServiceCollectionExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 16 | Registration lambda using `new VaultClient(...)` | If VaultSharp is not AOT-safe, propagate annotation to registration |

---

## Implementation Plan

### Step 1 — Verify VaultSharp AOT support

- [ ] Check the VaultSharp version in `Directory.Packages.props`
- [ ] Review whether VaultSharp carries trimmer-safe annotations or publishes a `.trimmerroot.xml` manifest
- [ ] If VaultSharp is not AOT-annotated: proceed to Step 2
- [ ] If VaultSharp is AOT-safe: library requires no changes

### Step 2 — Annotate `ServiceCollectionExtensions.AddKeyVaultIntegration` (if needed)

- [ ] Open `src/ISynergy.Framework.KeyVault.OpenBao/Extensions/ServiceCollectionExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the method if VaultSharp is not AOT-safe:
  ```csharp
  [RequiresUnreferencedCode("VaultSharp uses reflection-based JSON serialization internally. Verify AOT compatibility with the VaultSharp version in use.")]
  [RequiresDynamicCode("VaultSharp may require dynamic code generation for JSON deserialization.")]
  public static IServiceCollection AddKeyVaultIntegration(this IServiceCollection services, IConfiguration configuration)
  ```

### Step 3 — Annotate `OpenBaoKeyVaultService` (if needed)

- [ ] If Step 2 applies, propagate `[RequiresUnreferencedCode]` to the `OpenBaoKeyVaultService` class as well

### Step 4 — Verify Data Protection integration

- [ ] `OpenBaoDataProtectionRepository.cs` and `DataProtectionBuilderExtensions.cs` — ASP.NET Core Data Protection is trimmer-safe in .NET 8+; confirm no additional issues in those files

### Step 5 — Update XML documentation

- [ ] Add `<remarks>` to `AddKeyVaultIntegration` noting the VaultSharp AOT dependency status

---

## Migration Guide

`ISynergy.Framework.KeyVault.OpenBao` may require `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` annotations on `AddKeyVaultIntegration` depending on VaultSharp's AOT support status. The `JsonDocument`-based `OpenBaoVaultTokenProvider` is fully AOT-safe and unchanged.

### Before

```csharp
// Program.cs — no AOT warning previously
services.AddKeyVaultIntegration(configuration);
```

### After

```csharp
// If VaultSharp is not AOT-annotated, this produces IL2026/IL3050 warnings
// Option A: suppress at call site
#pragma warning disable IL2026, IL3050 // VaultSharp JSON serialization — not AOT-annotated
services.AddKeyVaultIntegration(configuration);
#pragma warning restore IL2026, IL3050

// Option B: switch to Azure Key Vault which is AOT-safe
services.AddKeyVaultIntegration(
    "https://myvault.vault.azure.net/",
    new ManagedIdentityCredential());
```

### Steps to Migrate

1. Check the VaultSharp version in `Directory.Packages.props` and verify whether it ships with trimmer-safe annotations.
2. If VaultSharp is not AOT-annotated: add `#pragma warning disable IL2026, IL3050` suppressions at the `AddKeyVaultIntegration` call site in your AOT-published code.
3. For production AOT deployments: consider switching to `ISynergy.Framework.KeyVault.Azure` with an explicit credential type, which is fully AOT-safe.
4. `OpenBaoVaultTokenProvider` (which uses `JsonDocument.Parse`) requires no changes — it is already AOT-safe.
