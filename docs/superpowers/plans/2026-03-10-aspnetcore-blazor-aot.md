# ISynergy.Framework.AspNetCore.Blazor AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.AspNetCore.Blazor` fully AOT-compatible by replacing runtime assembly scanning and `AppDomain.CurrentDomain` enumeration with a Roslyn incremental source generator that emits compile-time view/viewmodel/window type registrations, and by annotating remaining reflection-based helpers that cannot be statically resolved.

**Architecture:** A new `ISynergy.Framework.AspNetCore.Blazor.SourceGenerator` project (targeting `netstandard2.0`, bundled as a Roslyn analyzer) scans the consuming Blazor assembly at compile time for types implementing `IView`, `IWindow`, and `IViewModel`. It generates a static extension method `AddBlazorRegistrations(IServiceCollection services)` that replaces all `RegisterAssemblies(…)` / `ToViewTypes()` / `ToWindowTypes()` / `ToViewModelTypes()` / `AppDomain.CurrentDomain.GetAssemblies()` calls. The runtime `ReflectionExtensions` methods that enumerate `GetExportedTypes()` are preserved but annotated with `[RequiresUnreferencedCode]` for non-AOT usage paths. The `View<TViewModel>.SubscribeToViewModelCommands()` reflection in `View.cs` is replaced with an interface-based or source-generator-driven approach.

**Tech Stack:** .NET 10 / C# 14, Roslyn Incremental Source Generators (`IIncrementalGenerator`), `Microsoft.CodeAnalysis.CSharp` 4.x, `netstandard2.0` generator target, `System.Diagnostics.CodeAnalysis` trimmer attributes.

---

## AOT Issues Found

### `src/ISynergy.Framework.AspNetCore.Blazor/Extensions/ReflectionExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 22 | `assembly.GetExportedTypes()` — iterates all exported types in each passed assembly | **Critical** |
| 36–43 | `AppDomain.CurrentDomain.GetAssemblies()` + `.GetExportedTypes()` — enumerates all assemblies at runtime | **Critical** |
| 53–60 | `assembly.GetExportedTypes()` (view scan) | **Critical** |
| 68–77 | `AppDomain.CurrentDomain.GetAssemblies()` + `.GetExportedTypes()` (view scan global) | **Critical** |
| 87–93 | `assembly.GetExportedTypes()` (window scan) | **Critical** |
| 101–108 | `AppDomain.CurrentDomain.GetAssemblies()` + `.GetExportedTypes()` (window scan global) | **Critical** |
| 23, 39 | `type.GetInterfaces()` — runtime interface inspection on discovered types | **Critical** |
| 54, 71, 88, 104 | `type.GetInterfaces()` (view/window interface check) | **Critical** |
| 129–130 | `window.GetInterfaces()` + `.GetInterfaces()` — finding the correct abstraction interface | High |
| 154–155 | `view.GetInterfaces()` + `.GetInterfaces()` | High |
| 179–180 | `viewmodel.GetInterfaces()` + `.GetInterfaces()` | High |

**Assessment:** The `ToViewModelTypes`, `GetViewModelTypes`, `ToViewTypes`, `GetViewTypes`, `ToWindowTypes`, `GetWindowTypes`, `RegisterView`, `RegisterWindow`, `RegisterViewModel` methods together form a **complete assembly-scanning type-discovery pipeline** that is fundamentally incompatible with AOT. A source generator is required to produce equivalent compile-time registrations.

---

### `src/ISynergy.Framework.AspNetCore.Blazor/Extensions/ServiceCollectionExtensions.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 101 | `assembly.GetAllReferencedAssemblyNames()` — queries `Assembly.GetReferencedAssemblies()` at runtime | **Critical** |
| 106 | `Assembly.Load(item)` — loads assemblies by name at runtime based on dynamic filter | **Critical** |
| 111 | `Assembly.Load(item)` — same pattern for framework assemblies | **Critical** |
| 123–129 | `RegisterAssemblies(assemblies)` delegates to the scanning methods above | **Critical** |

**Assessment:** `RegisterAssemblies(Assembly, Func<AssemblyName, bool>)` is the top-level entry point for the entire assembly-scanning registration pipeline. It must be replaced by a generated `AddBlazorRegistrations()` call.

---

### `src/ISynergy.Framework.AspNetCore.Blazor/Components/Controls/View.cs`

| Line | Pattern | Severity |
|------|---------|----------|
| 67–79 | `ViewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)` — runtime property enumeration to discover `ICommand` properties | High |

**Assessment:** The `SubscribeToViewModelCommands()` method iterates all public instance properties of the ViewModel type at runtime to find `ICommand` properties. This is incompatible with AOT. Fix options: (a) introduce an `ICommandProvider` interface that ViewModels implement to expose their commands, (b) use a source generator to emit a `GetCommands()` method on each ViewModel, or (c) annotate with `[RequiresUnreferencedCode]` as an interim measure.

---

### `src/ISynergy.Framework.AspNetCore.Blazor/Properties/Resources.Designer.cs`

| Line | Pattern | Notes |
|------|---------|-------|
| Auto-generated | `new ResourceManager(typeof(Resources))` — constructed from a compile-time-known `typeof(…)` | Low – the concrete `typeof` call is trim-safe; no action needed unless tooling generates `ResourceManager(string, Assembly)` instead |

---

## File Map

### New files (source generator project)

| File | Responsibility |
|------|----------------|
| `src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator.csproj` | Generator project (`netstandard2.0`, packaged as Roslyn analyzer) |
| `src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator/BlazorRegistrationGenerator.cs` | `IIncrementalGenerator` implementation — discovers `IView`, `IWindow`, `IViewModel` implementors and emits `AddBlazorRegistrations` |
| `src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator/BlazorRegistrationEmitter.cs` | Emits the C# source text for the generated extension method |
| `src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator/TypeClassifier.cs` | Determines `Transient`/`Scoped`/`Singleton` lifetime from `[Lifetime]` attribute on the discovered type |

### Modified files (runtime library)

| File | Change |
|------|--------|
| `src/ISynergy.Framework.AspNetCore.Blazor/Extensions/ReflectionExtensions.cs` | Add `[RequiresUnreferencedCode]` + `[RequiresDynamicCode]` to all six scanning methods; keep for non-AOT scenarios |
| `src/ISynergy.Framework.AspNetCore.Blazor/Extensions/ServiceCollectionExtensions.cs` | Add `RegisterAssemblies` overload that delegates to generated `AddBlazorRegistrations()`; mark the `Assembly`-based overload with `[RequiresUnreferencedCode]` |
| `src/ISynergy.Framework.AspNetCore.Blazor/Components/Controls/View.cs` | Annotate `SubscribeToViewModelCommands` with `[RequiresUnreferencedCode]` as interim fix; long-term: adopt `ICommandProvider` interface |
| `src/ISynergy.Framework.AspNetCore.Blazor/ISynergy.Framework.AspNetCore.Blazor.csproj` | Add `<ProjectReference>` to generator project with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"` |

---

## Implementation Steps

- [ ] **Step 1 – Create source generator project**
  - Create `src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator/` folder.
  - Create `.csproj` targeting `netstandard2.0` with `<PackageReference Include="Microsoft.CodeAnalysis.CSharp" />` (version from `Directory.Packages.props`). Set `<IsRoslynComponent>true</IsRoslynComponent>`.
  - Verify the project compiles cleanly before proceeding.

- [ ] **Step 2 – Implement `BlazorRegistrationGenerator`**
  - Register a `SyntaxProvider` that finds all class declarations in the compilation.
  - Filter to classes that implement `IView`, `IWindow`, or `IViewModel` (by checking the symbol's interface list, not by name pattern — the name-pattern check currently done in `ReflectionExtensions` is fragile under AOT).
  - For each discovered type, read the `[Lifetime]` attribute (from `ISynergy.Framework.Core.Attributes`) to determine `Transient`/`Scoped`/`Singleton`.
  - Also resolve the matching abstraction interface (the first `IView`-derived interface, `IWindow`-derived, or `IViewModel`-derived interface that is not the base interface itself) — mirroring the logic in `RegisterView`, `RegisterWindow`, `RegisterViewModel`.

- [ ] **Step 3 – Implement `BlazorRegistrationEmitter`**
  - Emit a `partial class` extension named `GeneratedBlazorRegistrations` in the consuming assembly's root namespace.
  - Emit one `AddBlazorRegistrations(this IServiceCollection services)` extension method that calls `services.TryAdd[Lifetime]<IAbstraction, ConcreteType>()` and `services.TryAdd[Lifetime]<ConcreteType>()` for each discovered type.
  - The emitted file must have an `[assembly: System.Reflection.AssemblyMetadata("BlazorRegistrationsGenerated", "true")]` marker to allow detection.

- [ ] **Step 4 – Wire generator into runtime library project**
  - Add the `<ProjectReference>` to the generator with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"` in `ISynergy.Framework.AspNetCore.Blazor.csproj`.
  - Add an `AddBlazorRegistrations` overload to `ServiceCollectionExtensions` that calls the generated method.

- [ ] **Step 5 – Annotate reflection-based scanning methods with trimmer attributes**
  - In `ReflectionExtensions.cs`, add `[RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddBlazorRegistrations() instead.")]` and `[RequiresDynamicCode("…")]` to: `ToViewModelTypes`, `GetViewModelTypes`, `ToViewTypes`, `GetViewTypes`, `ToWindowTypes`, `GetWindowTypes`.
  - In `ServiceCollectionExtensions.cs`, add the same attributes to `RegisterAssemblies(Assembly, Func<AssemblyName, bool>)` and `RegisterAssemblies(IEnumerable<Assembly>)`.

- [ ] **Step 6 – Annotate `View<TViewModel>.SubscribeToViewModelCommands`**
  - Add `[RequiresUnreferencedCode("Reflects over ViewModel properties to discover ICommand instances. Not AOT-compatible.")]` and `[RequiresDynamicCode("…")]` to `SubscribeToViewModelCommands()` in `View.cs`.
  - Open a follow-up tracking item to introduce `ICommandProvider` on `IViewModel` as the AOT-safe replacement.

- [ ] **Step 7 – Write generator unit tests**
  - Create `tests/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator.Tests/` using `Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing` (MSTest variant).
  - Test: view type with `[Lifetime(Lifetimes.Scoped)]` → emits `TryAddScoped<IMyView, MyView>` and `TryAddScoped<MyView>`.
  - Test: viewmodel type with default (transient) lifetime → emits `TryAddTransient<IMyViewModel, MyViewModel>` and `TryAddTransient<MyViewModel>`.
  - Test: abstract type is excluded.
  - Test: interface type is excluded.

- [ ] **Step 8 – Verify build**
  - Run `dotnet build src/ISynergy.Framework.AspNetCore.Blazor` and `dotnet build src/ISynergy.Framework.AspNetCore.Blazor.SourceGenerator`.
  - Confirm zero IL2026 / IL3050 warnings on the unannotated code paths.
  - Confirm the generated `AddBlazorRegistrations` extension is visible in a sample project.

---

## Migration Guide

The assembly-scanning `RegisterAssemblies(Assembly, ...)` call is replaced by a Roslyn source-generator-emitted `AddBlazorRegistrations()` extension method. Adding the `ISynergy.Framework.AspNetCore.Blazor` NuGet package automatically includes the source generator, which scans your application's compilation for `IView`, `IWindow`, and `IViewModel` implementors and emits the registration call at build time.

### Before

```csharp
// Program.cs — reflection-based scanning at startup
builder.Services.RegisterAssemblies(
    Assembly.GetExecutingAssembly(),
    name => name.Name?.StartsWith("MyApp") == true);
```

### After

```csharp
// Program.cs — source-generator-emitted compile-time registration
builder.Services.AddBlazorRegistrations(); // generated by ISynergy.Framework.AspNetCore.Blazor.SourceGenerator
```

### Steps to Migrate

1. Remove the `RegisterAssemblies(Assembly, ...)` call from your application startup.
2. Build the project — the `ISynergy.Framework.AspNetCore.Blazor.SourceGenerator` Roslyn analyzer (bundled with the NuGet) will emit `AddBlazorRegistrations()` in the `<RootNamespace>.Generated` namespace.
3. Add `services.AddBlazorRegistrations()` in place of the old `RegisterAssemblies` call.
4. If you have custom `IView`/`IWindow`/`IViewModel` types in separate assemblies, add a `ProjectReference` or ensure the source generator can see them (generators run in the primary compilation unit, not transitively across assemblies).
5. For any remaining `[RequiresUnreferencedCode]`-annotated paths (e.g., `SubscribeToViewModelCommands` in `View.cs`), implement `ICommandProvider` on your ViewModels to expose commands without reflection.
