# Security Audit & Fix Loop — I-Synergy Framework

**Status:** DONE — Build clean, all tests pass (2026-04-09)  
**Date:** 2026-04-09  
**Scope:** Full framework (`src/`, all projects)

---

## Context

The I-Synergy Framework is an open-source .NET 10 library used by downstream enterprise applications. Security vulnerabilities here propagate to every consumer. The goal is a systematic, phased security audit-and-fix loop covering all OWASP Top 10 categories and .NET-specific concerns, working from highest to lowest severity.

---

## Findings Summary (Pre-Fix)

| Severity | Count | Categories |
|----------|-------|-----------|
| **Critical** | 1 | Secrets in public API models |
| **High** | 10 | Blob public access, S3/email credentials, JWT, HTML injection, KV token handling |
| **Medium** | 15 | Path traversal, SSRF, tenant isolation gaps, exception disclosure, logging |
| **Low** | 8 | Sensitive model markers, entropy, JSON config, assembly loading |

---

## Loop Architecture

The loop runs in 5 sequential phases. Each phase produces a diff that is reviewed before proceeding to the next. After all phases, a regression build + security re-scan is run.

```
Phase 1 → Critical
Phase 2 → High
Phase 3 → Medium
Phase 4 → Low
Phase 5 → Hardening + Automation
           ↓
      Regression scan (re-run audit agents)
           ↓
      Commit per phase with conventional commit messages
```

---

## Phase 1 — Critical (Must Fix Now)

### 1.1 `ClientSecret` in `ClientApplicationOptions`

**File:** `src/ISynergy.Framework.Core/Options/ClientApplicationOptions.cs`

**Problem:** `ClientSecret` is a plain `string?` property, serializable to JSON and loggable.

**Fix:**
- Remove `ClientSecret` from this options class entirely
- Add XML doc comment on the class noting that secrets must come from KeyVault or environment only
- Downstream: callers must retrieve the secret from `IKeyVaultService` at runtime, not from bound configuration

---

## Phase 2 — High Severity

### 2.1 Azure Blob `PublicAccessType.Blob` on all containers

**File:** `src/ISynergy.Framework.Storage.Azure/Services/AzureStorageService.cs` (lines ~60, 97, 133, 173)

**Problem:** Every `CreateIfNotExists()` call passes `PublicAccessType.Blob`, making all uploaded files world-readable without authentication.

**Fix:**
- Change all four occurrences to `PublicAccessType.None`
- Add a `GenerateSasTokenAsync(string blobName, TimeSpan expiry)` method to `AzureStorageService` for time-limited access
- Add `UsePublicAccess` property to `AzureStorageOptions` (default `false`) for cases where public CDN is intentional
- Update XML documentation

---

### 2.2 HTML Injection in Email Bodies

**Files:**
- `src/ISynergy.Framework.Mail.SendGrid/Services/SendGridMailService.cs` (line ~84-85)
- `src/ISynergy.Framework.Mail.Microsoft365/Services/Microsoft365MailService.cs` (line ~113-117)

**Problem:** `emailMessage.Message` is assigned directly to `HtmlContent` / `Body.Content` without sanitization — XSS in email.

**Fix:**
- Accept raw HTML as input but strip dangerous tags using .NET's `System.Text.RegularExpressions` minimal sanitizer OR use `HtmlAgilityPack` (already a transitive dep check first)
- Alternatively: split `MailMessage` into `PlainTextBody` + `HtmlBody` and require callers to provide pre-sanitized HTML
- Preferred approach: add XML doc warning "Caller is responsible for sanitizing HTML" + add a `SanitizeHtml(string html)` utility in `ISynergy.Framework.Core` using a whitelist regex strip
- Document the security contract clearly

---

### 2.3 Email Header Injection (To/CC/BCC)

**File:** `src/ISynergy.Framework.Mail/Models/MailMessage.cs`

**Problem:** Email addresses accept any string, including `\r\n` characters that could inject SMTP headers.

**Fix:**
- Add `ValidateEmailAddress(string address)` guard in `MailMessage` property setters or add `[EmailAddress]` validation
- Strip or reject any address containing `\n`, `\r`, or `;` characters
- Throw `ArgumentException` on invalid addresses with descriptive message
- Add validation in the collection setters

---

### 2.4 S3 Credentials in Options

**File:** `src/ISynergy.Framework.Storage.S3/Services/S3StorageService.cs` (lines ~39-40, 44, 59)

**Problem:** `AccessKey` and `SecretKey` are stored in `S3StorageOptions`, logged in debug assertions.

**Fix:**
- Guard against `Argument.IsNotNullOrEmpty` (keep validation, remove from log output)
- Add XML doc on `S3StorageOptions.AccessKey` / `.SecretKey`: "Should be sourced from Key Vault or environment variables, never from appsettings.json in production"
- Add `[ConfigurationKeyName]` and validation attributes
- Recommend AWS IAM role via default credential chain in docs

---

### 2.5 SendGrid API Key Exposure

**File:** `src/ISynergy.Framework.Mail.SendGrid/Services/SendGridMailService.cs`

**Problem:** `SendGridClient` is instantiated on every `SendAsync()` call using `_sendGridOptions.Key`, which shows the key in stack traces if construction fails.

**Fix:**
- Register `SendGridClient` as a singleton in `ServiceCollectionExtensions` and inject it instead of constructing per-call
- Add `[DataProtected]` / sensitive marker doc on `SendGridMailOptions.Key`

---

### 2.6 OpenBao Vault State File in Plaintext

**File:** `src/ISynergy.Framework.KeyVault.OpenBao/Services/OpenBaoVaultTokenProvider.cs`

**Problem:** Root Vault token is persisted to `.secrets/vault-state.json` in plaintext JSON.

**Fix:**
- Wrap file read/write through `IDataProtector` (inject `IDataProtectionProvider` and create a named protector)
- Encrypt the JSON content using `protector.Protect(json)` before writing, `protector.Unprotect(content)` before parsing
- Update XML documentation to describe this protection mechanism
- Add fallback behavior when file is missing (first run)

---

### 2.7 Vault Token in `KeyVaultOptions` / Extension Parameters

**Files:**
- `src/ISynergy.Framework.KeyVault/Options/KeyVaultOptions.cs`
- `src/ISynergy.Framework.KeyVault.OpenBao/Extensions/DataProtectionBuilderExtensions.cs`

**Problem:** `Token` property in options and `vaultToken` parameter in extension method expose secrets in configuration binding and stack traces.

**Fix:**
- Mark `Token` as `[Obsolete("Source tokens from environment variable VAULT_TOKEN or IVaultTokenProvider, never from appsettings")]`
- Add XML doc warning on `vaultToken` parameter
- Provide `AddOpenBaoDataProtection(IVaultTokenProvider tokenProvider)` overload as the preferred API

---

### 2.8 Azure Service Bus Exception Logging (Connection String Leak)

**File:** `src/ISynergy.Framework.MessageBus.Azure/Services/Queue/SubscriberServiceBus{TEntity,TOption}.cs` (line ~157)

**Problem:** `_logger.LogDebug($"- Exception: {arg.Exception.ToString()}")` — `.ToString()` includes the full stack trace and can include connection strings embedded in exceptions.

**Fix:**
- Change to `_logger.LogDebug("- Exception: {ExceptionType}: {ExceptionMessage}", arg.Exception.GetType().Name, arg.Exception.Message)`
- Never log `.ToString()` on exceptions containing infrastructure details

---

### 2.9 JWT `SymmetricKeySecret` in Memory / Missing Validation

**File:** `src/ISynergy.Framework.AspNetCore.Authentication/Options/JwtOptions.cs`

**Problem:** `SymmetricKeySecret` is a plain string with no runtime validation enforcing minimum length or entropy.

**Fix:**
- Add `[ValidateOnStart]` and `IValidateOptions<JwtOptions>` that checks:
  - `SymmetricKeySecret` is not null/empty
  - Has minimum length of 32 characters (256-bit minimum for HMAC-SHA256)
- Add XML doc recommending Key Vault as the source

---

## Phase 3 — Medium Severity

### 3.1 `Process.Start` with `UseShellExecute = true`

**Files:**
- `src/ISynergy.Framework.UI.WPF/Services/FileService.cs` (line ~75-83)
- `src/ISynergy.Framework.UI.WPF/Services/DownloadFileService.cs` (line ~36)
- `src/ISynergy.Framework.UI.WPF/Services/UpdateService.cs` (line ~77)

**Fix:**
- Replace `UseShellExecute = true` with explicit process setup using `UseShellExecute = false` + `FileName` pointing to known executables only
- For document opening: use `Process.Start(new ProcessStartInfo { FileName = "cmd", Arguments = $"/c start \"\" \"{sanitizedPath}\"", UseShellExecute = false })` only after path validation
- Add `ValidateLocalPath(string path)` helper that checks: `Path.GetFullPath()` does not traverse outside allowed base dirs
- For `UpdateService`: validate that `updatePath` was downloaded from a known trusted URL and verify file hash before execution

---

### 3.2 Path Traversal in `CustomFileTypeAnalyzer`

**File:** `src/ISynergy.Framework.IO/Analyzers/CustomFileTypeAnalyzer.cs` (lines ~27-30)

**Fix:**
- Before calling `File.ReadAllText(filePath)`, validate:
  ```csharp
  var fullPath = Path.GetFullPath(filePath);
  if (!fullPath.StartsWith(allowedBasePath, StringComparison.OrdinalIgnoreCase))
      throw new ArgumentException("File path is outside the allowed directory", nameof(filePath));
  ```
- Pass `allowedBasePath` via constructor parameter with a sensible default

---

### 3.3 SSRF in `StaticAssetService`

**File:** `src/ISynergy.Framework.UI.Blazor/Services/StaticAssetService.cs` (line ~57)

**Fix:**
- Add URL validation before creating the `HttpRequestMessage`:
  - Parse to `Uri`, verify scheme is `https` or `http`
  - Reject loopback (`IsLoopback`), link-local, private IP ranges
  - Optionally: accept only relative paths or URLs matching a configured base URL
- Add `IsAllowedAssetUrl(string url)` static method

---

### 3.4 Stream DoS in `BaseFileTypeAnalyzer`

**File:** `src/ISynergy.Framework.IO/Analyzers/Base/BaseFileTypeAnalyzer.cs` (lines ~56-71)

**Fix:**
- Add max size check: `if (inputStream.Length > MaxStreamSize) throw new ArgumentException("Stream exceeds maximum allowed size")`
- Expose `MaxStreamSize` as a configurable constant (default 50 MB)

---

### 3.5 Exception Message Exposed to HTTP Clients

**File:** `src/ISynergy.Framework.AspNetCore/Handlers/GlobalExceptionHandler.cs` (line ~31)

**Fix:**
- Replace `detail: exception.Message` with `detail: "An unexpected error occurred."`
- Log the full exception server-side only with structured logging including correlation ID
- Expose `TraceId` in `ProblemDetails` so the client can reference it in support tickets

---

### 3.6 HTML Attribute Injection in `HtmlHelperExtensions`

**File:** `src/ISynergy.Framework.AspNetCore/Extensions/HtmlHelperExtensions.cs` (lines ~122-157)

**Fix:**
- Wrap attribute values with `HtmlEncoder.Default.Encode(x.Value)` in the LINQ projection

---

### 3.7 Tenant Middleware Ordering Guard

**File:** `src/ISynergy.Framework.AspNetCore.MultiTenancy/Middleware/TenantResolutionMiddleware.cs`

**Fix:**
- After the `if (IsAuthenticated)` block, add a check: if the user is authenticated but `TenantId` is `Guid.Empty`, throw `InvalidOperationException("Tenant ID could not be resolved for authenticated user. Verify middleware ordering: UseAuthentication() must precede UseTenantResolution().")`
- Update extension method `UseTenantResolution()` XML docs with explicit middleware ordering requirement

---

### 3.8 RabbitMQ URI Validation

**File:** `src/ISynergy.Framework.MessageBus.RabbitMQ/Services/Queue/...`

**Fix:**
- Validate that `ConnectionString` is a valid `amqp://` or `amqps://` URI
- Reject plain-text `amqp://` in production via a configurable `RequireTls` property (default `true`)
- XML doc: "Use amqps:// in production"

---

### 3.9 EF Core Tenant Filter Auto-Enforcement

**File:** `src/ISynergy.Framework.EntityFramework/Extensions/ModelBuilderExtensions.cs`

**Fix:**
- Create a base `TenantDbContext : DbContext` that:
  - Requires `ITenantProvider` injection
  - Automatically calls `ApplyTenantFilters()` in `OnModelCreating()`
- Update documentation to recommend inheriting from `TenantDbContext`

---

## Phase 4 — Low Severity

### 4.1 `Grant` Model Sensitive Properties

**File:** `src/ISynergy.Framework.Core/Models/Grant.cs`

**Fix:**
- Override `ToString()` to return `"[Redacted Grant]"` to prevent accidental logging
- Add XML doc warning on `password` and `client_secret` properties

### 4.2 `Token` Model Sensitive Properties

**File:** `src/ISynergy.Framework.Core/Models/Token.cs`

**Fix:**
- Override `ToString()` to return `"[Token]"` 
- Add XML doc warning on `access_token`, `refresh_token`, `id_token`

### 4.3 `SecretUtility` Entropy Improvement

**File:** `src/ISynergy.Framework.Core/Utilities/SecretUtility.cs`

**Fix:**
- Remove the regex replacement that maps non-alphanumeric chars to `"X"` (reduces entropy)
- Use `Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))` directly — produces 44-char Base64 string with full entropy
- Or use `RandomNumberGenerator.GetHexString(64)` (.NET 8+) for a URL-safe 64-char hex string

### 4.4 JSON Deserialization Config (`ReferenceHandler.Preserve`)

**File:** `src/ISynergy.Framework.Core/Serializers/DefaultJsonSerializers.cs`

**Fix:**
- Document the reason `ReferenceHandler.Preserve` is used in the non-web profile (circular reference support for internal use)
- Ensure the "Web" profile (with `ReferenceHandler = null`) is the documented default for public APIs
- Add XML doc comment

### 4.5 Exception Swallowing in `Argument`

**File:** `src/ISynergy.Framework.Core/Validation/Argument.cs`

**Fix:**
- Add `#pragma warning disable` suppression with explanation, or add a `Debug.WriteLine` inside the catch for dev visibility

---

## Phase 5 — Hardening & Automation

### 5.1 Message Bus Rate Limiting

**Files:** Azure Service Bus and RabbitMQ subscriber implementations

**Fix:**
- Make `MaxConcurrentCalls` configurable in subscriber options (add `MaxConcurrentMessages` property, default 2)
- Add message size limit validation: reject messages > configurable max bytes before deserialization
- Add circuit breaker pattern documentation

### 5.2 Email Input Validation (Size Limits)

**Files:** `MailMessage.cs` and both mail service implementations

**Fix:**
- Add max length checks: `Subject` ≤ 998 chars (RFC 2822), `Message` ≤ 10 MB configurable, `EmailAddressesTo` ≤ configurable max recipients

### 5.3 Audit Logging for Sensitive Operations

**Scope:** Storage, email, and tenant-switching operations

**Fix:**
- Add structured `LogInformation` with EventId in range 9000-9099 (Security Events) for:
  - Every file upload/download (include `TenantId`, `UserId`, `FileName`)
  - Every email send (include `TenantId`, `RecipientCount`, exclude bodies)
  - Tenant resolution successes and failures

### 5.4 `IOptions<T>` Validation at Startup

**Scope:** All critical options classes

**Fix:**
- Add `[ValidateOnStart]` + `IValidateOptions<T>` validators for:
  - `JwtOptions` (key length)
  - `S3StorageOptions` (non-empty keys, valid region)
  - `AzureStorageOptions` (valid connection string format)
  - `SendGridMailOptions` (non-empty API key)

### 5.5 Security-Focused Unit Tests

**Scope:** New tests in existing test projects

**Fix:** Add unit tests verifying:
- `TenantResolutionMiddleware` throws when user is authenticated but TenantId is empty
- `MailMessage` rejects addresses containing `\r\n`
- `BaseFileTypeAnalyzer` throws on stream exceeding max size
- `AzureStorageService` creates containers with `PublicAccessType.None`
- `GlobalExceptionHandler` never returns `exception.Message` in response body

---

## File Index (Critical Paths)

| File | Phase | Severity |
|------|-------|----------|
| `src/ISynergy.Framework.Core/Options/ClientApplicationOptions.cs` | 1 | Critical |
| `src/ISynergy.Framework.Storage.Azure/Services/AzureStorageService.cs` | 2 | High |
| `src/ISynergy.Framework.Mail.SendGrid/Services/SendGridMailService.cs` | 2 | High |
| `src/ISynergy.Framework.Mail.Microsoft365/Services/Microsoft365MailService.cs` | 2 | High |
| `src/ISynergy.Framework.Mail/Models/MailMessage.cs` | 2 | High |
| `src/ISynergy.Framework.Storage.S3/Services/S3StorageService.cs` | 2 | High |
| `src/ISynergy.Framework.KeyVault.OpenBao/Services/OpenBaoVaultTokenProvider.cs` | 2 | High |
| `src/ISynergy.Framework.KeyVault/Options/KeyVaultOptions.cs` | 2 | High |
| `src/ISynergy.Framework.KeyVault.OpenBao/Extensions/DataProtectionBuilderExtensions.cs` | 2 | High |
| `src/ISynergy.Framework.MessageBus.Azure/Services/Queue/SubscriberServiceBus*.cs` | 2 | High |
| `src/ISynergy.Framework.AspNetCore.Authentication/Options/JwtOptions.cs` | 2 | High |
| `src/ISynergy.Framework.UI.WPF/Services/FileService.cs` | 3 | Medium |
| `src/ISynergy.Framework.UI.WPF/Services/DownloadFileService.cs` | 3 | Medium |
| `src/ISynergy.Framework.UI.WPF/Services/UpdateService.cs` | 3 | Medium |
| `src/ISynergy.Framework.IO/Analyzers/CustomFileTypeAnalyzer.cs` | 3 | Medium |
| `src/ISynergy.Framework.IO/Analyzers/Base/BaseFileTypeAnalyzer.cs` | 3 | Medium |
| `src/ISynergy.Framework.UI.Blazor/Services/StaticAssetService.cs` | 3 | Medium |
| `src/ISynergy.Framework.AspNetCore/Handlers/GlobalExceptionHandler.cs` | 3 | Medium |
| `src/ISynergy.Framework.AspNetCore/Extensions/HtmlHelperExtensions.cs` | 3 | Medium |
| `src/ISynergy.Framework.AspNetCore.MultiTenancy/Middleware/TenantResolutionMiddleware.cs` | 3 | Medium |
| `src/ISynergy.Framework.MessageBus.RabbitMQ/Services/Queue/...` | 3 | Medium |
| `src/ISynergy.Framework.EntityFramework/Extensions/ModelBuilderExtensions.cs` | 3 | Medium |
| `src/ISynergy.Framework.Core/Models/Grant.cs` | 4 | Low |
| `src/ISynergy.Framework.Core/Models/Token.cs` | 4 | Low |
| `src/ISynergy.Framework.Core/Utilities/SecretUtility.cs` | 4 | Low |
| `src/ISynergy.Framework.Core/Serializers/DefaultJsonSerializers.cs` | 4 | Low |
| `src/ISynergy.Framework.Core/Validation/Argument.cs` | 4 | Low |

---

## Verification

After each phase:

```powershell
dotnet build                          # Must produce 0 errors, 0 warnings
dotnet test                           # All tests must pass
```

After all phases — re-run the three security audit agents to verify all findings are resolved.

Commit format per phase:
```
fix(security): [phase N] <short description>
```
Example: `fix(security): [phase 1] remove ClientSecret from ClientApplicationOptions`

---

## Out of Scope (This Loop)

- Penetration testing of consuming applications
- Network-layer security (TLS configuration on hosting)
- Static analysis tooling integration into CI (separate task)
- Azure Key Vault managed identity setup (infrastructure, not framework code)
