# ISynergy.Framework.Mail.SendGrid AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Assess and document AOT compatibility of `ISynergy.Framework.Mail.SendGrid` given its dependency on the `SendGrid` NuGet package.
**Architecture:** The library's own code (`MailService.cs`, `ServiceCollectionExtensions.cs`, `MailOptions.cs`) contains no direct reflection or `dynamic`. The AOT concern is entirely in the `SendGrid` package, which uses `Newtonsoft.Json` internally in older versions (definitely not AOT-safe) or `System.Text.Json` in newer versions. The fix depends on the SendGrid package version in use: if it uses Newtonsoft.Json, the entire service must be annotated as AOT-incompatible.
**Tech Stack:** .NET 10, C# 14, `SendGrid`, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Mail.SendGrid/Services/MailService.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 48 | `new SendGridClient(_sendGridOptions.Key)` | `SendGridClient` uses `Newtonsoft.Json` for request/response serialization in SendGrid SDK v9.x — not AOT-compatible |
| 78 | `await client.SendEmailAsync(msg)` | Underlying HTTP request serialization via Newtonsoft.Json (in v9.x) |

---

## Implementation Plan

### Step 1 — Determine SendGrid SDK version and serializer

- [ ] Check the SendGrid package version in `Directory.Packages.props`
- [ ] SendGrid SDK v9.x uses `Newtonsoft.Json` — not AOT-compatible
- [ ] If the project is on v9.x: annotate `MailService` and proceed to Step 2
- [ ] If a future version of SendGrid ships with `System.Text.Json` support: re-assess and potentially remove annotations

### Step 2 — Annotate `MailService` (for v9.x)

- [ ] Open `src/ISynergy.Framework.Mail.SendGrid/Services/MailService.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the class:
  ```csharp
  [RequiresUnreferencedCode("SendGrid SDK uses Newtonsoft.Json for serialization, which is not AOT-compatible. Use a different mail provider for AOT publishing.")]
  [RequiresDynamicCode("Newtonsoft.Json requires dynamic code generation.")]
  internal class MailService : IMailService
  ```

### Step 3 — Annotate `ServiceCollectionExtensions` (for v9.x)

- [ ] Open `src/ISynergy.Framework.Mail.SendGrid/Extensions/ServiceCollectionExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate the registration method to propagate the constraint

### Step 4 — Update XML documentation

- [ ] Add `<remarks>` to both annotated members noting the Newtonsoft.Json AOT limitation and directing AOT users to `ISynergy.Framework.Mail.Microsoft365` as an alternative

### Notes
- The empty `catch (Exception) { throw; }` blocks in `SendEmailAsync` are no-op catches and should be removed in a future cleanup pass (unrelated to AOT).

---

## Migration Guide

`ISynergy.Framework.Mail.SendGrid` is annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` because `SendGrid` SDK v9.x uses `Newtonsoft.Json`, which is not AOT-compatible. AOT-publishing applications should use `ISynergy.Framework.Mail.Microsoft365` instead.

### Before

```csharp
// Program.cs — no AOT warning previously (worked at runtime in non-AOT builds)
services.AddSendGridMailIntegration(configuration);
```

### After

```csharp
// For AOT-published applications: switch to Microsoft365 mail provider
services.AddMicrosoft365MailIntegration(configuration);

// For non-AOT applications: SendGrid is unchanged (add suppression for trimmer warnings)
#pragma warning disable IL2026, IL3050 // SendGrid uses Newtonsoft.Json; not AOT-safe
services.AddSendGridMailIntegration(configuration);
#pragma warning restore IL2026, IL3050
```

### Steps to Migrate

1. For applications targeting `<PublishAot>true</PublishAot>`: replace `AddSendGridMailIntegration` with `AddMicrosoft365MailIntegration` (requires configuring Microsoft 365 credentials).
2. For applications that do not publish AOT: add `#pragma warning disable IL2026, IL3050` around the `AddSendGridMailIntegration` call with a justification comment.
3. Monitor SendGrid SDK releases — if a future version ships with `System.Text.Json` support and AOT annotations, the framework annotations can be removed and AOT support restored.
4. No changes are needed to `IMailService` usage — only the registration call is affected.
