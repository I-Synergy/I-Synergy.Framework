# ISynergy.Framework.Synchronization AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Annotate all reflection-based code in `ISynergy.Framework.Synchronization` with appropriate trimmer attributes and document the fundamental AOT incompatibilities introduced by its dependencies (`Dotmim.Sync` and `MessagePack` with `ContractlessStandardResolver`).

**Architecture:** The library has two distinct AOT problem areas: (1) `SyncSetupExtensions.WithTenantFilter` uses `Type.GetCustomAttributes()` and `Type.GetProperty()` to read table names and filter tenant ID columns from a caller-supplied `Type[]` array at runtime, and (2) `DefaultMessagePackSerializer` uses `MessagePack`'s `ContractlessStandardResolver` which is a reflection-based resolver that inspects object members at runtime — fundamentally incompatible with AOT. The `Dotmim.Sync` framework itself is also not AOT-compatible. Both issues require `[RequiresUnreferencedCode]` annotations; the MessagePack issue also requires `[RequiresDynamicCode]`.

**Tech Stack:** .NET 10 / C# 14, `Dotmim.Sync`, `MessagePack`, `System.ComponentModel.DataAnnotations`, central package management via `Directory.Packages.props`.

---

## AOT Issues Found

### `Extensions/SyncSetupExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 17 | `entity.GetCustomAttributes(typeof(TableAttribute), false)` — attribute reflection on runtime `Type` elements from caller's `Type[]` array | High |
| 23 | `entity.GetProperty(GenericConstants.TenantId)` — property reflection on runtime types | High |

**Detail:** The method accepts `Type[] entities` where element types are not statically known at compile time. The linker cannot know which `Type` values will be passed, so it cannot guarantee that `TableAttribute` metadata or `TenantId` property descriptors will be preserved on those types. Both `GetCustomAttributes` and `GetProperty` are unsafe under trimming.

### `Serializers/DefaultMessagePackSerializer.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 10 | `ContractlessStandardResolver.Instance` — MessagePack's contractless resolver uses runtime reflection to enumerate all members of any object passed to it | **Critical** |
| 20 | `MessagePackSerializer.DeserializeAsync(type, ms, this.options)` — deserializes into a `Type` known only at runtime | **Critical** |
| 28 | `MessagePackSerializer.Deserialize(typeof(T), ms, this.options)` — still uses the contractless resolver internally | High |
| 35 | `MessagePackSerializer.SerializeAsync(ms, obj, this.options)` — serializes `object` via reflection | **Critical** |
| 48 | `MessagePackSerializer.SerializeAsync(type, ms, obj, this.options)` — serializes by runtime `Type` | **Critical** |
| 54 | `MessagePackSerializer.Serialize(type, obj, this.options)` — same | **Critical** |

**Detail:** `ContractlessStandardResolver` is explicitly designed to handle types without any `[MessagePackObject]` attributes — it discovers members entirely through reflection at runtime. This is the definition of an AOT-unsafe serialization strategy. MessagePack does provide AOT support via `[MessagePackObject]` attributes combined with `StaticCompositeResolver` or generated formatters, but switching to those requires changes in both this library and all consuming types.

### `Factories/MessagePackSerializerFactory.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 9 | Returns `new DefaultMessagePackSerializer()` — indirectly exposes all of the above | High |

### `Messages/SyncMessage.cs`, `SyncProgressMessage.cs`, `SyncSessionStateChangedMessage.cs`

No AOT issues in the message classes themselves. They are simple wrappers around `BaseMessage<T>` with primitive and SDK-defined payload types.

### `Abstractions/Services/ISynchronizationService.cs` and `Abstractions/Settings/ISynchronizationSettings.cs`

No AOT issues. Pure interface definitions.

---

## Fundamental AOT Incompatibility: `Dotmim.Sync`

`Dotmim.Sync` (the underlying sync framework) itself is not AOT-compatible as of early 2026. It uses extensive internal reflection for:
- Dynamic table schema discovery
- Runtime type-to-SQL-column mapping
- `SyncAgent` orchestration with runtime dispatch

This library (`ISynergy.Framework.Synchronization`) is a thin wrapper around `Dotmim.Sync`. **Applications publishing with `<PublishAot>true</PublishAot>` should not use this library until `Dotmim.Sync` itself gains AOT support.** The framework library's job is to annotate its own reflection usage correctly.

---

## Why No Source Generator Is Needed

The `WithTenantFilter` method accepts a `Type[]` array provided by the caller at runtime — the types are not discoverable at compile time from this library's perspective. A source generator in this library would not help because it cannot see the caller's entity types during its own compilation. The correct fix is `[RequiresUnreferencedCode]` annotation.

For `DefaultMessagePackSerializer`, the issue is the choice of resolver (`ContractlessStandardResolver`) rather than a missing code-generation step. Switching to AOT-safe MessagePack would require replacing the resolver with `StaticCompositeResolver` and adding `[MessagePackObject]` attributes to all serialized types — a consumer-facing breaking change that goes beyond what this library can do unilaterally.

---

## Implementation Steps

### Phase 1 — Annotate `SyncSetupExtensions.WithTenantFilter`

- [ ] Add `using System.Diagnostics.CodeAnalysis;` to `Extensions/SyncSetupExtensions.cs`.

- [ ] Annotate `WithTenantFilter` with `[RequiresUnreferencedCode]`:
  ```csharp
  [RequiresUnreferencedCode(
      "WithTenantFilter uses reflection to read TableAttribute and property names from runtime types. " +
      "Ensure entity types and their attributes are preserved in trimmed builds.")]
  public static SyncSetup WithTenantFilter(this SyncSetup setup, Type[] entities)
  ```

- [ ] Update XML documentation with an `<remarks>` block describing the AOT concern and suggesting that consuming apps preserve their entity types via `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]` on the parameter or via a linker root descriptor.

### Phase 2 — Annotate `DefaultMessagePackSerializer`

- [ ] Add `using System.Diagnostics.CodeAnalysis;` to `Serializers/DefaultMessagePackSerializer.cs`.

- [ ] Annotate the class itself with both attributes (the entire class is reflection-dependent):
  ```csharp
  [RequiresUnreferencedCode(
      "DefaultMessagePackSerializer uses ContractlessStandardResolver which requires runtime reflection " +
      "on serialized types. Use MessagePackObject-attributed types with StaticCompositeResolver for AOT compatibility.")]
  [RequiresDynamicCode(
      "DefaultMessagePackSerializer uses ContractlessStandardResolver which generates dynamic code at runtime.")]
  public class DefaultMessagePackSerializer : ISerializer
  ```

- [ ] Add XML documentation to the class with an `<remarks>` block explaining:
  - Why `ContractlessStandardResolver` is not AOT-compatible
  - What consumers need to do to achieve AOT compatibility (use `[MessagePackObject]` attributes and switch to `StaticCompositeResolver`)
  - That this class is suitable for use in non-AOT scenarios only

### Phase 3 — Annotate `MessagePackSerializerFactory`

- [ ] Propagate the `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` annotations to `MessagePackSerializerFactory`:
  ```csharp
  [RequiresUnreferencedCode("Returns DefaultMessagePackSerializer which is not AOT-compatible.")]
  [RequiresDynamicCode("Returns DefaultMessagePackSerializer which uses dynamic code generation.")]
  public class MessagePackSerializerFactory : ISerializerFactory
  ```

### Phase 4 — Add top-level AOT warning to the library

- [ ] Add an assembly-level `[assembly: RequiresUnreferencedCode(...)]` attribute in a `GlobalUsings.cs` or `AssemblyInfo.cs` file to surface a compile-time warning whenever this library is referenced from an AOT-publishing project:
  ```csharp
  [assembly: System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
      "ISynergy.Framework.Synchronization depends on Dotmim.Sync and MessagePack ContractlessStandardResolver " +
      "which are not compatible with trimming or Native AOT.")]
  ```

### Phase 5 — Document migration path for AOT consumers

- [ ] Add a `<remarks>` block to `ISynchronizationService` noting that Native AOT support depends on `Dotmim.Sync` gaining AOT compatibility, and link to the `Dotmim.Sync` project's AOT tracking issue when available.

- [ ] In the library's project file (`ISynergy.Framework.Synchronization.csproj`), add:
  ```xml
  <PropertyGroup>
    <!-- Suppress ILLink warnings from Dotmim.Sync internals that we cannot fix -->
    <NoWarn>$(NoWarn);IL2026;IL3050</NoWarn>
  </PropertyGroup>
  ```
  **Scope:** This suppression is only appropriate for the *library project build* itself. Consuming applications should not inherit this suppression — they should see the warnings propagated from their call sites.

### Phase 6 — Testing

- [ ] Verify that adding the annotations causes the compiler to emit `IL2026`/`IL3050` warnings at call sites in test projects that have trimming enabled, confirming that the annotations propagate correctly.
- [ ] Confirm that existing non-AOT tests still pass (the annotations do not change runtime behavior).

---

## Migration Guide

`ISynergy.Framework.Synchronization` is annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` at the class and assembly levels because both `Dotmim.Sync` and `MessagePack` with `ContractlessStandardResolver` are fundamentally incompatible with AOT. AOT-publishing applications cannot use this library.

### Before

```csharp
// SyncSetupExtensions.WithTenantFilter — no AOT warning previously
var setup = new SyncSetup(tableNames);
setup.WithTenantFilter(new[] { typeof(UserEntity), typeof(OrderEntity) });

// DefaultMessagePackSerializer — no AOT warning previously
var serializer = serializerFactory.GetSerializer();
```

### After

```csharp
// WithTenantFilter is now annotated [RequiresUnreferencedCode]
// For non-AOT code: add suppression
#pragma warning disable IL2026
setup.WithTenantFilter(new[] { typeof(UserEntity), typeof(OrderEntity) });
#pragma warning restore IL2026

// DefaultMessagePackSerializer is now annotated [RequiresUnreferencedCode] + [RequiresDynamicCode]
// For AOT-safe MessagePack: use [MessagePackObject] attributes on your types
// and switch to StaticCompositeResolver — requires consumer-side changes
```

### Steps to Migrate

1. For applications targeting `<PublishAot>true</PublishAot>`: do not use this library until `Dotmim.Sync` gains AOT support. Monitor the Dotmim.Sync GitHub for updates.
2. For non-AOT applications: add `#pragma warning disable IL2026, IL3050` suppressions at `WithTenantFilter`, `DefaultMessagePackSerializer`, and `MessagePackSerializerFactory` call sites with justification comments.
3. If you need AOT-safe serialization with MessagePack, decorate your sync entity types with `[MessagePackObject]` and `[Key(n)]` attributes, then replace `ContractlessStandardResolver` with `StaticCompositeResolver` — this is a breaking change for your serialized data format.
4. The assembly-level `[assembly: RequiresUnreferencedCode(...)]` attribute will surface warnings for any AOT-publishing consumer automatically, even without calling specific methods.
