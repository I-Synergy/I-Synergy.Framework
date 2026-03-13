# ISynergy.Framework.Geography AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Confirm that `ISynergy.Framework.Geography` is fully AOT-compatible with no code changes required.
**Architecture:** No changes needed — the library is AOT-compatible. All source files implement pure coordinate geometry, geodetic calculations, and Mercator projections using mathematical operations on primitive types. The `Properties/Resources.Designer.cs` uses the standard strongly-typed `ResourceManager` pattern, which is supported under AOT with the `[RequiresUnreferencedCode]` trimmer-safe path that `ResourceManager` already carries in .NET 10.
**Tech Stack:** .NET 10, C# 14.

---

## AOT Assessment: Compatible

`ISynergy.Framework.Geography` contains 18 source files covering geodetic curves, global coordinates, Mercator/UTM projections, and ellipsoid definitions. All computation is pure mathematics with no reflection, no `dynamic`, no `JsonSerializer`, no `Activator.CreateInstance`, and no assembly scanning. The auto-generated `Resources.Designer.cs` uses the standard `ResourceManager` constructor (`new ResourceManager("...", typeof(Resources).Assembly)`) — this is a trimmer-safe pattern under .NET 10 because `typeof(T)` is a statically-resolvable token. No changes are required.

---

## Migration Guide

`ISynergy.Framework.Geography` is fully AOT-compatible. No code changes or migration steps are required when upgrading to the AOT-compatible release.

### Before

```csharp
// No changes — pure geodetic calculation API
var distance = GlobalCoordinates.DistanceTo(pointA, pointB, Ellipsoid.WGS84);
var mercator = MercatorProjection.ToMeters(latitude, longitude);
```

### After

```csharp
// Identical — no migration needed
var distance = GlobalCoordinates.DistanceTo(pointA, pointB, Ellipsoid.WGS84);
var mercator = MercatorProjection.ToMeters(latitude, longitude);
```

### Steps to Migrate

1. No migration steps required — all computation in this library is pure mathematics with no AOT-incompatible patterns.
2. No `[JsonSerializable]` entries or trimmer suppressions are required when consuming this library.
3. If you serialize geography types (e.g., `GlobalCoordinates`) to JSON in your application, ensure those types are included in your application-level `JsonSerializerContext`.
