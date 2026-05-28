# ISynergy.Framework.KeyVault.Azure AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility of `ISynergy.Framework.KeyVault.Azure` given its dependency on `Azure.Security.KeyVault.Secrets` and `Azure.Identity`.
**Architecture:** The library's own code (`AzureKeyVaultService.cs`, `ServiceCollectionExtensions.cs`) is clean — no reflection, no `dynamic`, no `MakeGenericType`. The AOT concern is entirely in the transitive dependencies: `Azure.Security.KeyVault.Secrets` and `Azure.Identity.DefaultAzureCredential`. As of 2025, Azure SDK packages ship with trimmer-safe manifests (`.trimmerroot.xml`) and the `DefaultAzureCredential` carries `[RequiresUnreferencedCode]` because it probes many credential providers at runtime. The fix is a targeted annotation on `AddKeyVaultIntegration`.
**Tech Stack:** .NET 10, C# 14, `Azure.Security.KeyVault.Secrets`, `Azure.Identity`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.KeyVault.Azure/Extensions/ServiceCollectionExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 14 | `new DefaultAzureCredential()` | `DefaultAzureCredential` carries `[RequiresUnreferencedCode]` (probes multiple credential types via reflection at runtime) |

### `src/ISynergy.Framework.KeyVault.Azure/Services/AzureKeyVaultService.cs`

No direct AOT issues in framework code. All operations use typed `SecretClient` methods.

---

## Implementation Plan

### Step 1 — Verify `DefaultAzureCredential` AOT annotation

- [ ] Check the `Azure.Identity` version in `Directory.Packages.props`
- [ ] Confirm whether `DefaultAzureCredential` is annotated with `[RequiresUnreferencedCode]` in that version (it has been since Azure.Identity 1.10+)

### Step 2 — Annotate `AddKeyVaultIntegration`

- [ ] Open `src/ISynergy.Framework.KeyVault.Azure/Extensions/ServiceCollectionExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the method:
  ```csharp
  [RequiresUnreferencedCode("DefaultAzureCredential uses reflection to probe credential providers. For AOT publishing, use a specific credential type such as ClientSecretCredential or ManagedIdentityCredential.")]
  public static IServiceCollection AddKeyVaultIntegration(this IServiceCollection services, string vaultUri)
  ```
- [ ] Add an overload that accepts an explicit `TokenCredential` parameter (AOT-safe) as an alternative:
  ```csharp
  /// <summary>AOT-safe overload that accepts an explicit credential.</summary>
  public static IServiceCollection AddKeyVaultIntegration(
      this IServiceCollection services,
      string vaultUri,
      TokenCredential credential)
  {
      services.TryAddSingleton(_ => new SecretClient(new Uri(vaultUri), credential));
      services.TryAddSingleton<IKeyVaultService, AzureKeyVaultService>();
      return services;
  }
  ```

### Step 3 — Update XML documentation

- [ ] Add `<remarks>` to the `DefaultAzureCredential` overload noting the AOT limitation and directing callers to the explicit-credential overload for AOT scenarios
- [ ] Ensure both overloads have complete XML documentation

---

## Migration Guide

`AddKeyVaultIntegration()` (the overload that uses `DefaultAzureCredential`) is annotated with `[RequiresUnreferencedCode]`. A new overload accepting an explicit `TokenCredential` is fully AOT-safe. AOT-publishing applications should use the explicit-credential overload.

### Before

```csharp
// Program.cs — DefaultAzureCredential probes credential providers via reflection
services.AddKeyVaultIntegration("https://myvault.vault.azure.net/");
```

### After

```csharp
// Option A (AOT-safe): pass an explicit credential
using Azure.Identity;

services.AddKeyVaultIntegration(
    "https://myvault.vault.azure.net/",
    new ManagedIdentityCredential()); // or ClientSecretCredential, WorkloadIdentityCredential

// Option B (non-AOT): suppress warning and keep DefaultAzureCredential
#pragma warning disable IL2026
services.AddKeyVaultIntegration("https://myvault.vault.azure.net/");
#pragma warning restore IL2026
```

### Steps to Migrate

1. For AOT-published applications: replace the parameterless `AddKeyVaultIntegration(string)` call with the new `AddKeyVaultIntegration(string, TokenCredential)` overload, passing an explicit credential type (`ManagedIdentityCredential`, `ClientSecretCredential`, or `WorkloadIdentityCredential`).
2. For non-AOT applications: add `#pragma warning disable IL2026` with a justification comment at the call site to silence the trimmer warning.
3. Ensure `Azure.Identity` is at version 1.10.0 or later so that `ClientSecretCredential` and `ManagedIdentityCredential` are themselves AOT-annotated.
4. Do not use `DefaultAzureCredential` in AOT-published code — it probes dozens of credential providers using reflection.
