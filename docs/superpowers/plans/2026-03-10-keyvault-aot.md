# ISynergy.Framework.KeyVault AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm that `ISynergy.Framework.KeyVault` (the shared abstractions library) is fully AOT-compatible with no code changes required.
**Architecture:** No changes needed — the library contains only interfaces (`IKeyVaultService`, `IVaultTokenProvider`) and options POCOs (`KeyVaultOptions`, `VaultStateOptions`) with no reflection, no serialization, and no assembly scanning.
**Tech Stack:** .NET 10, C# 14.

---

## AOT Assessment: Compatible

`ISynergy.Framework.KeyVault` contains 4 source files: `IKeyVaultService` (interface), `IVaultTokenProvider` (interface), `KeyVaultOptions`, and `VaultStateOptions` (options POCOs). All are pure contracts and configuration models with no reflection, no `dynamic`, no `JsonSerializer`, no `Activator.CreateInstance`, and no assembly scanning. The library is fully AOT-compatible as written.

---

## Migration Guide

`ISynergy.Framework.KeyVault` is fully AOT-compatible. No code changes or migration steps are required.

### Before

```csharp
// No changes needed
services.AddScoped<IKeyVaultService, MyKeyVaultService>();
var options = new KeyVaultOptions { VaultUri = "https://myvault.vault.azure.net/" };
```

### After

```csharp
// Same — IKeyVaultService and options classes are pure contracts
services.AddScoped<IKeyVaultService, MyKeyVaultService>();
var options = new KeyVaultOptions { VaultUri = "https://myvault.vault.azure.net/" };
```

### Steps to Migrate

1. No migration steps required for this abstractions library.
2. Verify that the concrete `IKeyVaultService` implementation (e.g., `ISynergy.Framework.KeyVault.Azure` or `ISynergy.Framework.KeyVault.OpenBao`) is AOT-compatible — see the respective migration guides.
3. If you serialize `KeyVaultOptions` or `VaultStateOptions` to JSON, register them with `[JsonSerializable]` in your application's `JsonSerializerContext`.
