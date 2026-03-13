# ISynergy.Framework.AspNetCore.Authentication AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document that `ISynergy.Framework.AspNetCore.Authentication` is free of AOT-blocking patterns.

**Architecture:** This is a very small library (four source files). After a full read of every `.cs` file the analysis found no AOT-incompatible patterns. All types are used as concrete generic constraints or through DI abstractions; no runtime type discovery, `MakeGenericType`, `Activator.CreateInstance`, assembly scanning, unregistered `GetService(typeof(…))`, or unannotated `JsonSerializer` calls are present.

**Tech Stack:** .NET 10 / C# 14, ASP.NET Core Identity.

---

## AOT Issues Found

**None.** The library consists of:

| File | Assessment |
|------|------------|
| `Exceptions/ClaimNotFoundExceptionFilterAttribute.cs` | Pattern-matches on concrete exception type — AOT-safe |
| `Options/IdentityPasswordOptions.cs` | Plain POCO options class — AOT-safe |
| `Options/JwtOptions.cs` | Plain POCO options class — AOT-safe |
| `Validators/IdentityPasswordValidator.cs` | Generic class constrained to `class`; uses only `IdentityResult` and `Regex.IsMatch` — AOT-safe |

`IdentityPasswordOptions.RequiredRegexMatch` is a `Regex` instance constructed at compile time with a literal pattern; this does not trigger `RegexOptions.Compiled` or `[GeneratedRegex]` concerns unless the pattern is dynamic, which it is not.

## Recommendation

No implementation steps are required for this library at this time. If the library is extended in the future with dynamic JSON handling or type discovery, revisit this assessment.

---

## Migration Guide

`ISynergy.Framework.AspNetCore.Authentication` is fully AOT-compatible. No code changes or migration steps are required when upgrading to the AOT-compatible release.

### Before

```csharp
// Existing registration — no change needed
services.AddIdentityPasswordValidator<ApplicationUser>(options =>
{
    options.RequiredLength = 8;
    options.RequiredRegexMatch = new Regex(@"[A-Z]");
});
```

### After

```csharp
// Same registration — fully AOT-safe
services.AddIdentityPasswordValidator<ApplicationUser>(options =>
{
    options.RequiredLength = 8;
    options.RequiredRegexMatch = new Regex(@"[A-Z]");
});
```

### Steps to Migrate

1. No migration steps required — this library contains no AOT-incompatible patterns.
2. If you add custom extensions to this library in the future, ensure they avoid `Activator.CreateInstance`, `GetExportedTypes()`, and reflection-based JSON serialization.
3. Keep the `Azure.Identity` and ASP.NET Core Identity packages up to date to benefit from their own AOT improvements.
