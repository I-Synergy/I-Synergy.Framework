# ISynergy.Framework.EntityFramework AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.EntityFramework` fully AOT-compatible by annotating all reflection-heavy APIs with trimmer attributes and migrating to EF Core's compiled-model story rather than a custom source generator.

**Architecture:** The library has two distinct AOT problem areas: (1) `ModelBuilderExtensions` uses assembly scanning (`GetExportedTypes()`), `Type.GetCustomAttributes()`, `Type.GetProperties()`, and `Expression.Lambda` with runtime types to build query filters and configure EF models, and (2) `DbContextExtensions` uses `Expression.Parameter(typeof(TEntity))` and runtime navigation inclusion chains. EF Core itself provides a compiled-model AOT solution via `dotnet ef dbcontext optimize`; the framework's helper layer needs trimmer annotations and, for `ApplyModelBuilderConfigurations`, a caller-supplied type list to eliminate the assembly scan.

**Tech Stack:** .NET 10 / C# 14, Entity Framework Core 9+, `System.Diagnostics.CodeAnalysis` trimmer attributes, central package management via `Directory.Packages.props`.

---

## AOT Issues Found

### `ModelBuilderExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 23 | `entityType.ClrType.GetProperties()` — reflection over unknown entity properties | Medium |
| 48 | `t.GetCustomAttributes(typeof(IgnoreVersioningAttribute), true)` — attribute reflection on runtime types | Medium |
| 114 | `t.GetCustomAttributes(typeof(IgnoreSoftDeleteAttribute), true)` — attribute reflection on runtime types | Medium |
| 73–77 | `Expression.Parameter(type)`, `Expression.Property(parameter, ...)`, `Expression.Lambda(...)` — runtime lambda construction over unknown types | High |
| 119–122 | Same `Expression.Parameter/Property/Lambda` pattern for soft-delete filter | High |
| 192–214 | `a.GetExportedTypes()` — **assembly scanning** at runtime, the most severe AOT blocker | Critical |
| 200–205 | `t.GetInterfaces()`, `i.IsGenericType`, `i.GetGenericTypeDefinition()` — interface introspection on unknown types | High |

### `DbContextExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 43 | `ReflectionExtensions.GetIdentityPropertyName<TEntity>()` — uses `typeof(TEntity).GetProperties()` internally | Medium |
| 48–50 | `Expression.Parameter(typeof(TEntity))`, `Expression.Property(parameterExpression, entityPropertyName)`, `Expression.Lambda<Func<TEntity, bool>>(...)` — dynamic LINQ expression building | High |
| 56 | `context.Model.FindEntityType(typeof(TEntity))` — EF metadata lookup by type | Low (EF manages this) |
| 61–65 | `entityType.GetDerivedTypesInclusive()` + `.GetNavigations()` → dynamic `Include(property.Name)` strings | Medium |
| 138–150 | Same Expression pattern for `UpdateItemAsync` | High |
| 213–215 | Same Expression pattern for `AddUpdateItemAsync` | High |

### `SyncSetupExtensions.cs` (not in this library, but flagged here for cross-reference)
This file lives in `ISynergy.Framework.Synchronization` — see that library's plan.

---

## Why No Source Generator Is Needed Here

The EF Core team provides its own AOT/trimming story via **compiled models** (`dotnet ef dbcontext optimize`), which eliminates the vast majority of runtime model-building reflection. The issues in this library fall into two categories:

1. **Assembly scanning in `ApplyModelBuilderConfigurations`** — this can be replaced with a caller-supplied `IEnumerable<Type>` or with EF Core's `ApplyConfigurationsFromAssembly` which is itself not AOT-safe but is the consuming app's concern. The library's helper just needs to be annotated or replaced with a type-list overload.

2. **Dynamic `Expression.Lambda` over generic type parameters** — all usages are already generic methods where `TEntity` is statically known at the call site. Annotating with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` is the correct signal; actually eliminating these expressions at the framework level would require compiled EF queries (`EF.CompileAsyncQuery`) which belong in consuming repositories, not in generic helpers.

A Roslyn source generator is **not** the right tool here because the problem is not unknown-at-compile-time handler discovery — EF Core's model is always known to the consuming application.

---

## Implementation Steps

### Phase 1 — Annotate reflection APIs with trimmer attributes

- [ ] Add `[RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]` and `[RequiresDynamicCode("Builds LINQ expression trees at runtime.")]` to all methods in `ModelBuilderExtensions` that use `GetProperties()`, `GetCustomAttributes()`, `GetExportedTypes()`, or `Expression.Lambda` with `Type` parameters:
  - `ApplyDecimalPrecision` (line 19)
  - `ApplyVersioning` (line 44)
  - `ApplyTenantFilters` (line 67)
  - `ApplySoftDeleteFilters` (line 110)
  - `CombineQueryFilters` (line 161)
  - `ApplyModelBuilderConfigurations` (line 192)

- [ ] Add `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` to all affected methods in `DbContextExtensions`:
  - `GetItemByIdAsync<TEntity, TId>` (line 39)
  - `UpdateItemAsync` (line 130)
  - `AddUpdateItemAsync` (line 195)

- [ ] Add `using System.Diagnostics.CodeAnalysis;` to both files.

### Phase 2 — Eliminate the assembly scan in `ApplyModelBuilderConfigurations`

The method at line 192 calls `a.GetExportedTypes()` which is a hard AOT blocker — the linker cannot know which types are exported from unknown assemblies at trim time.

- [ ] Add a new overload `ApplyModelBuilderConfigurations(this ModelBuilder modelBuilder, IReadOnlyList<Type> configurationTypes)` that accepts an explicit list of `IEntityTypeConfiguration<>` implementation types. This overload calls `modelBuilder.ApplyConfiguration((dynamic)Activator.CreateInstance(type)!)` — still requires `[RequiresDynamicCode]` but avoids the assembly scan.

- [ ] Mark the existing `ApplyModelBuilderConfigurations(ModelBuilder, Assembly[])` overload with both `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]` and add an `[Obsolete]` notice pointing to the new type-list overload:
  ```csharp
  [Obsolete("Prefer the overload accepting IReadOnlyList<Type> for AOT/trimming compatibility.")]
  [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible.")]
  [RequiresDynamicCode("Assembly scanning uses reflection.")]
  public static ModelBuilder ApplyModelBuilderConfigurations(this ModelBuilder modelBuilder, Assembly[] assemblies)
  ```

- [ ] Add XML documentation to the new overload explaining AOT usage.

### Phase 3 — Document EF compiled-model guidance in XML docs

- [ ] Add `<remarks>` to `ApplyTenantFilters`, `ApplySoftDeleteFilters`, and `ApplyVersioning` explaining that consuming apps using Native AOT should use EF Core compiled models (`dotnet ef dbcontext optimize`) and register them via `UseModel()` to avoid runtime model-building reflection.

### Phase 4 — Verify `ReflectionExtensions.GetIdentityPropertyName<TEntity>`

- [ ] Locate `ReflectionExtensions.GetIdentityPropertyName<T>()` in `ISynergy.Framework.Core` and confirm it is annotated with `[RequiresUnreferencedCode]`. If not, add the annotation there (tracked in the Core library's AOT plan, not here — but cross-reference the attribute propagation).

### Phase 5 — Testing

- [ ] Verify all public methods with the new attributes produce no new compiler warnings when consumed from a project with `<PublishTrimmed>true</PublishTrimmed>` and `<PublishAot>true</PublishAot>`.
- [ ] Confirm `ApplyModelBuilderConfigurations` with the type-list overload works in a test that registers a concrete `IEntityTypeConfiguration<>` type without any assembly scanning.

---

## Notes on EF Core's Own AOT Story

EF Core 8+ supports compiled models via:
```bash
dotnet ef dbcontext optimize --output-dir CompiledModels --namespace MyApp.CompiledModels
```
Then registered in `OnConfiguring`:
```csharp
options.UseModel(MyAppCompiledModels.MyDbContextModel.Instance);
```

This is the consuming application's responsibility. The framework helpers in this library cannot pre-compile models; they can only annotate their own reflection usage correctly so the linker can make informed decisions.

The `Expression.Lambda`-based query filters (`ApplyTenantFilters`, `ApplySoftDeleteFilters`) are inherently dynamic and will always require `[RequiresDynamicCode]`. Consuming apps using AOT should define their query filters directly in `OnModelCreating` using the strongly-typed `HasQueryFilter<T>(Expression<Func<T, bool>>)` overload instead of calling the framework extension methods.

---

## Migration Guide

All model-building extension methods are now annotated with `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`. A new `ApplyModelBuilderConfigurations(IReadOnlyList<Type>)` overload replaces the assembly-scanning `ApplyModelBuilderConfigurations(Assembly[])` call. AOT-publishing applications should also enable EF Core compiled models.

### Before

```csharp
// OnModelCreating — reflection-based assembly scan
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyModelBuilderConfigurations(typeof(MyDbContext).Assembly);
    modelBuilder.ApplyTenantFilters(_tenantProvider);
    modelBuilder.ApplySoftDeleteFilters();
}
```

### After

```csharp
// OnModelCreating — explicit type list (AOT-safe) + compiled model
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Pass explicit list instead of assembly scan
    modelBuilder.ApplyModelBuilderConfigurations(new List<Type>
    {
        typeof(UserEntityConfiguration),
        typeof(OrderEntityConfiguration),
    });

    // For AOT: define query filters directly instead of using framework helpers
    modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantId && !u.IsDeleted);
}

// In OnConfiguring — use compiled model for AOT
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseModel(MyDbContextModel.Instance); // generated by dotnet ef dbcontext optimize
}
```

### Steps to Migrate

1. Replace `ApplyModelBuilderConfigurations(Assembly[])` calls with the new `ApplyModelBuilderConfigurations(IReadOnlyList<Type>)` overload, listing every `IEntityTypeConfiguration<>` implementation type explicitly.
2. Run `dotnet ef dbcontext optimize --output-dir CompiledModels --namespace MyApp.CompiledModels` to generate a compiled EF Core model and register it with `optionsBuilder.UseModel(...)` in `OnConfiguring`.
3. In `OnModelCreating`, replace calls to `ApplyTenantFilters` and `ApplySoftDeleteFilters` with inline `HasQueryFilter<T>(...)` lambda expressions.
4. Calls to `GetItemByIdAsync<TEntity, TId>`, `UpdateItemAsync`, and `AddUpdateItemAsync` that remain in AOT-published code will produce IL2026/IL3050 warnings — add `#pragma warning disable` suppressions with justification comments, or replace the LINQ expression construction with pre-compiled `EF.CompileAsyncQuery` delegates.
