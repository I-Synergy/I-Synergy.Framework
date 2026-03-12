# ISynergy.Framework.Physics AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix one reflection-based AOT blocker in `ISynergy.Framework.Physics` without a source generator.
**Architecture:** The sole AOT issue is in `Extensions/EnumExtensions.cs`, which uses `type.GetMember()` and `memberInfo[0].GetCustomAttributes()` to retrieve a `[SymbolAttribute]` at runtime. The fix is to replace the runtime reflection with a compile-time-safe alternative: either annotate with `[RequiresUnreferencedCode]` (conservative), or rewrite to use `Enum.GetName()` + a static dictionary lookup mapping enum values to their symbol strings. Because `SymbolAttribute` is a custom attribute in `ISynergy.Framework.Core`, the static dictionary approach is preferred for true AOT safety.
**Tech Stack:** .NET 10, C# 14, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Physics/Extensions/EnumExtensions.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 28 | `type.GetMember(enumerationValue.ToString()!)` | `GetMember` on runtime enum type — trimmer may remove enum member metadata |
| 32 | `memberInfo[0].GetCustomAttributes(typeof(SymbolAttribute), false)` | `GetCustomAttributes` with a `Type` argument — trimmer will strip custom attributes unless annotated |
| 21 | `Type type = enumerationValue.GetType()` | Used to drive reflection — the generic constraint `where T : struct` does not guarantee enum type is preserved |

---

## Implementation Plan

### Option A (Conservative — annotation only)

- [ ] Open `src/ISynergy.Framework.Physics/Extensions/EnumExtensions.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;`
- [ ] Annotate `GetSymbol<T>` with:
  ```csharp
  [RequiresUnreferencedCode("GetSymbol uses reflection to read SymbolAttribute from enum members. Use a static lookup table for AOT-compatible scenarios.")]
  ```
- [ ] Verify build passes; this preserves all existing behaviour for non-AOT consumers

### Option B (Preferred — AOT-safe rewrite)

- [ ] Define a private static `Dictionary<Enum, string>` populated at static constructor time using `typeof(T).GetFields()` + `DynamicallyAccessedMembersAttribute` — this moves the reflection to a known-generic context where the trimmer can preserve it
- [ ] Alternatively, for a fully AOT-safe solution, introduce an overload `GetSymbol<T>(T value, IReadOnlyDictionary<T, string> symbolMap)` that lets the caller supply the mapping, and mark the reflection-based overload `[Obsolete]`

### Recommended approach
Implement **Option A** immediately (one-line annotation) so AOT builds get a clear diagnostic. File a follow-up issue to implement Option B for a future minor release when the `SymbolAttribute`-to-string mapping can be generated at compile time via a source generator in `ISynergy.Framework.Core`.

### Step 3 — Update documentation
- [ ] Add `<remarks>` to `GetSymbol<T>` noting the AOT limitation and directing callers to the future static overload

### Notes
- The `Resources.Designer.cs` (auto-generated) follows the standard `ResourceManager` pattern and is AOT-safe.
- All other files in the library (`IUnit`, `IUnitConversionService`, `UnitBase`, `SIUnit`, `Unit`, `UnitConversionService`, `Units`, `UnitTypes`) are pure domain types with no reflection.

---

## Migration Guide

`EnumExtensions.GetSymbol<T>` is annotated with `[RequiresUnreferencedCode]`. AOT-published callers will receive a build-time IL2026 warning. All other types in the library are unaffected.

### Before

```csharp
// Retrieve physics unit symbol via reflection — previously no AOT warning
string symbol = PhysicsUnitType.Meter.GetSymbol<PhysicsUnitType>();
```

### After

```csharp
// Option A: suppress the warning at the call site (safe if enum type is compile-time known)
#pragma warning disable IL2026
string symbol = PhysicsUnitType.Meter.GetSymbol<PhysicsUnitType>();
#pragma warning restore IL2026

// Option B (future): use a static lookup overload when available
// string symbol = PhysicsUnitType.Meter.GetSymbol(SymbolMap.Physics);
```

### Steps to Migrate

1. Locate all calls to `GetSymbol<T>()` in your application code.
2. For each call where `T` is a compile-time-known enum type, add `#pragma warning disable IL2026` with a justification comment — the call is safe because the enum type is statically referenced.
3. For fully AOT-safe scenarios, replace the `GetSymbol<T>()` call with a static dictionary lookup mapping enum values to their symbol strings; populate the dictionary with compile-time-known string literals.
4. All other `ISynergy.Framework.Physics` types (`IUnit`, `UnitConversionService`, etc.) require no changes.
