# ISynergy.Framework.Mathematics AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix two reflection-based AOT blockers in `ISynergy.Framework.Mathematics` without a source generator.
**Architecture:** Both issues are confined to `OctaveEnvironment.cs`. The constructor uses `GetFields()` + `SetValue()` to auto-initialize `mat`-typed fields on subclasses — this is a classic AOT blocker. The fix is to annotate the constructor with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` and document the constraint, or refactor subclasses to initialise their own fields explicitly. Additionally, `OctaveEnvironment.cs` imports `System.Reflection` which flags it for trimmer analysis. No source generator is needed.
**Tech Stack:** .NET 10, C# 14, `System.Diagnostics.CodeAnalysis`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Mathematics/Environments/OctaveEnvironment.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 548 | `type.GetFields(BindingFlags.NonPublic \| BindingFlags.Public \| BindingFlags.Instance)` | `GetFields()` on a runtime-unknown subclass type — trimmer will remove private fields of derived classes |
| 553 | `field.SetValue(this, new mat(null))` | `FieldInfo.SetValue` is a reflection write — blocked under AOT strict mode |
| 546 | `var type = this.GetType()` | Used to enumerate derived class fields — subclass field list is not statically known |

The `REnvironment.cs` file does **not** use reflection and is AOT-safe.

---

## Implementation Plan

### Step 1 — Annotate `OctaveEnvironment` constructor

- [ ] Open `src/ISynergy.Framework.Mathematics/Environments/OctaveEnvironment.cs`
- [ ] Add `using System.Diagnostics.CodeAnalysis;` to the file's using directives
- [ ] Annotate the `protected OctaveEnvironment()` constructor with:
  ```csharp
  [RequiresUnreferencedCode("OctaveEnvironment uses reflection to initialise mat fields on derived types. Subclasses may not be AOT-compatible.")]
  [RequiresDynamicCode("OctaveEnvironment uses reflection to set field values at runtime.")]
  protected OctaveEnvironment()
  ```
- [ ] Add XML `<remarks>` to the constructor noting that AOT-published applications should not inherit from `OctaveEnvironment` or must initialise their `mat` fields manually
- [ ] Verify no other files in the `Environments/` folder contain reflection

### Step 2 — Verify no other AOT blockers exist in the library

- [ ] Confirm that the 90+ remaining files (Decompositions, Distances, Statistics, Transforms, Comparers, Convergence, Differentiation, Formats, Common) contain no `GetType()` + member access, no `Activator.CreateInstance`, no `dynamic`, and no `Assembly.*` calls
- [ ] Run `dotnet build` with `<PublishAot>true</PublishAot>` on a test consumer project and confirm the only warnings are from `OctaveEnvironment`

### Step 3 — Update XML documentation

- [ ] Add `<para>` in the `OctaveEnvironment` class summary noting the AOT limitation
- [ ] Ensure all modified public APIs still have complete XML documentation

### Notes
- A source generator is **not** needed: the pattern is an optional DSL helper class; annotating it with `[RequiresUnreferencedCode]` is the correct conservative fix that preserves all functionality for non-AOT consumers while giving AOT tooling the information it needs.
- The `GetFields` usage is intrinsic to the Octave DSL design (auto-populate `mat` placeholders). If a future major version wants true AOT support for `OctaveEnvironment`, it would need to remove the auto-init loop and require subclasses to call `InitializeMat(ref mat field)` helpers manually.

---

## Migration Guide

`OctaveEnvironment` constructor is annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`. AOT-published code that subclasses `OctaveEnvironment` will receive a build-time warning. All other types in the library are unaffected.

### Before

```csharp
// Subclassing OctaveEnvironment — previously no AOT warning
public class MyOctaveScript : OctaveEnvironment
{
    mat x;
    mat y;
    // Fields auto-initialized via reflection in base constructor
}
```

### After

```csharp
// Subclassing OctaveEnvironment — now produces IL2026/IL3050 warning in AOT builds
[RequiresUnreferencedCode("...")]  // propagate if method calls this
public class MyOctaveScript : OctaveEnvironment
{
    mat x;
    mat y;
}

// AOT-safe alternative: initialize mat fields manually
public class MyAotSafeScript : OctaveEnvironment
{
    mat x = new mat(null);
    mat y = new mat(null);
}
```

### Steps to Migrate

1. If your application is AOT-published and inherits from `OctaveEnvironment`, add `#pragma warning disable IL2026, IL3050` with a justification at the subclass declaration, or initialize `mat` fields manually to avoid using the reflection-based auto-initialization.
2. The 90+ remaining classes in the library (Decompositions, Statistics, Transforms, etc.) are unaffected and require no changes.
3. No `[JsonSerializable]` entries are needed for this library.
