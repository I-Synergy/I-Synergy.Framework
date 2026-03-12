# ISynergy.Framework.AspNetCore.MultiTenancy AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm and document that `ISynergy.Framework.AspNetCore.MultiTenancy` is free of AOT-blocking patterns.

**Architecture:** This is a small library (five source files). After a full read of every `.cs` file the analysis found no AOT-incompatible patterns. The library uses `AsyncLocal<T>` for ambient context, a conventional middleware class with a typed primary constructor, and generic DI registration through `TryAddTransient<ITenantService, TenantService>()`. No runtime type discovery, `MakeGenericType`, `Activator.CreateInstance`, assembly scanning, non-generic `GetService(typeof(…))`, or unannotated JSON operations are present.

**Tech Stack:** .NET 10 / C# 14, `AsyncLocal<T>`, ASP.NET Core middleware pipeline.

---

## AOT Issues Found

**None.** Assessment by file:

| File | Assessment |
|------|------------|
| `TenantContext.cs` | `AsyncLocal<T>` with value types (`Guid`, `string?`) — fully AOT-safe |
| `Middleware/TenantResolutionMiddleware.cs` | Typed primary constructor; accesses `ClaimsPrincipal` extension methods — AOT-safe |
| `Services/TenantService.cs` | Thin delegate to `TenantContext` static class — AOT-safe |
| `Extensions/ServiceCollectionExtensions.cs` | `TryAddTransient<ITenantService, TenantService>()` generic registration and `UseMiddleware<TenantResolutionMiddleware>()` — AOT-safe |
| `Extensions/HostApplicationBuilderExtensions.cs` | Delegates to `ServiceCollectionExtensions` — AOT-safe |

## Recommendation

No implementation steps are required for this library at this time.

---

## Migration Guide

`ISynergy.Framework.AspNetCore.MultiTenancy` is fully AOT-compatible. No code changes or migration steps are required when upgrading to the AOT-compatible release.

### Before

```csharp
// Existing registration — no change needed
builder.AddMultiTenancyIntegration();
app.UseMultiTenancyMiddleware();
```

### After

```csharp
// Same registration — fully AOT-safe
builder.AddMultiTenancyIntegration();
app.UseMultiTenancyMiddleware();
```

### Steps to Migrate

1. No migration steps required — this library uses `AsyncLocal<T>`, generic DI registration, and typed middleware, all of which are AOT-safe.
2. If your application resolves tenant information from custom sources (e.g., a database lookup), ensure those custom implementations also follow AOT-safe patterns — avoid `GetType()` reflection and unregistered `JsonSerializer` calls in your tenant resolver.
3. Verify that the JWT token claims (`Claims.KeyId` for tenant ID and `Claims.Username`) are present on your tokens; the `TenantResolutionMiddleware` reads these claims directly without reflection.
