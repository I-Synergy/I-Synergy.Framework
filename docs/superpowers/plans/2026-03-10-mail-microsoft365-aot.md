# ISynergy.Framework.Mail.Microsoft365 AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility of `ISynergy.Framework.Mail.Microsoft365` given its dependency on `Microsoft.Graph` and `Azure.Identity`.
**Architecture:** The library's own code (`MailService.cs`, `ServiceCollectionExtensions.cs`, `MailOptions.cs`) contains no direct reflection or `dynamic`. The AOT concern is in transitive dependencies: `Microsoft.Graph` uses internal `System.Text.Json` serialization and `Azure.Identity.ClientSecretCredential` which — unlike `DefaultAzureCredential` — is a known-type credential and is AOT-safe in Azure.Identity 1.10+. `GraphServiceClient` itself may carry `[RequiresUnreferencedCode]` depending on the version. The fix is a targeted annotation on `MailService` and `ServiceCollectionExtensions`.
**Tech Stack:** .NET 10, C# 14, `Microsoft.Graph`, `Azure.Identity`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Mail.Microsoft365/Services/MailService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 119 | `new GraphServiceClient(credentials, _mailOptions.Scopes)` | `GraphServiceClient` uses reflection-based JSON serialization for Graph API responses; may carry `[RequiresUnreferencedCode]` |
| 120–123 | `new SendMailPostRequestBody { ... }` | Graph SDK model serialization — may require source-generated JSON for AOT |

---

## Implementation Plan

### Step 1 — Verify Microsoft.Graph AOT status

- [ ] Check the `Microsoft.Graph` version in `Directory.Packages.props`
- [ ] As of Microsoft.Graph 5.x, the SDK has improved AOT support and ships `[RequiresUnreferencedCode]` annotations on non-AOT paths. Confirm whether `GraphServiceClient` and `SendMailPostRequestBody` are AOT-safe in the version in use.

### Step 2 — Annotate `MailService.SendEmailAsync` (if needed)

- [ ] Open `src/ISynergy.Framework.Mail.Microsoft365/Services/MailService.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] If Microsoft.Graph is not AOT-annotated, annotate the class:
  ```csharp
  [RequiresUnreferencedCode("Microsoft.Graph SDK uses reflection-based JSON serialization. Not compatible with AOT publishing in this configuration.")]
  internal class MailService : IMailService
  ```

### Step 3 — Annotate `ServiceCollectionExtensions` (if needed)

- [ ] Open `src/ISynergy.Framework.Mail.Microsoft365/Extensions/ServiceCollectionExtensions.cs`
- [ ] If Step 2 applies, annotate the registration method to propagate the constraint

### Step 4 — Update XML documentation

- [ ] Add `<remarks>` to both annotated members noting the Microsoft.Graph AOT status

### Notes
- `ClientSecretCredential` (used on line 113) is AOT-safe in Azure.Identity 1.10+ — it does not probe credential providers at runtime.
- The empty `catch (Exception) { throw; }` block in `SendEmailAsync` is a no-op catch and should be removed in a future cleanup pass (unrelated to AOT).

---

## Migration Guide

`ISynergy.Framework.Mail.Microsoft365` may require a `[RequiresUnreferencedCode]` annotation on `MailService` depending on the `Microsoft.Graph` version. If annotated, the `AddMicrosoft365MailIntegration` registration method will also carry the annotation, and callers in AOT-published code will see a build-time IL2026 warning.

### Before

```csharp
// Program.cs — no AOT warning previously
services.AddMicrosoft365MailIntegration(configuration);
```

### After

```csharp
// If MailService is annotated, this produces an IL2026 warning in AOT builds
// Option A: suppress if Microsoft.Graph 5.x is verified to be AOT-annotated
#pragma warning disable IL2026
services.AddMicrosoft365MailIntegration(configuration);
#pragma warning restore IL2026

// Option B: upgrade Microsoft.Graph to a version with full AOT annotations
// and remove the [RequiresUnreferencedCode] annotation from MailService
```

### Steps to Migrate

1. Check the `Microsoft.Graph` version in `Directory.Packages.props` and verify whether it ships with `[RequiresUnreferencedCode]` annotations on `GraphServiceClient`.
2. If `Microsoft.Graph` 5.x is confirmed AOT-annotated in your version, no suppression is needed.
3. If not, add `#pragma warning disable IL2026, IL3050` with a justification at the `AddMicrosoft365MailIntegration` call site in your AOT-published code.
4. `ClientSecretCredential` (Azure.Identity 1.10+) does not require a suppression — it is fully AOT-safe.
