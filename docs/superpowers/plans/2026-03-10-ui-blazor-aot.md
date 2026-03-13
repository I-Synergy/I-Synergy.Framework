# ISynergy.Framework.UI.Blazor AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `ISynergy.Framework.UI.Blazor` fully AOT-compatible for Blazor WebAssembly (WASM AOT) by eliminating reflection-based command introspection in `View<TViewModel>` and replacing the assembly-scanning registration in `ServiceCollectionExtensions` with the compile-time generator output from `ISynergy.Framework.UI.SourceGenerator`.

**Architecture:** No new source generator is required for Blazor specifically — the base `ISynergy.Framework.UI.SourceGenerator` (planned in `2026-03-10-ui-aot.md`) already handles view/viewmodel registration. The remaining Blazor-specific work is: (1) replacing `GetProperties`-based command subscription in `View<TViewModel>` with an interface-driven approach, (2) adding `[DynamicDependency]` or `[JsonSerializable]` attributes where needed for JSInterop serialization, and (3) annotating `ServiceCollectionExtensions.RegisterAssemblies` to delegate to the generated registration.

**Tech Stack:** .NET 10 / C# 14, Blazor WebAssembly AOT, `System.Text.Json` source generation for JSInterop payloads, `Microsoft.AspNetCore.Components`.

---

## AOT Issues Found

### `src/ISynergy.Framework.UI.Blazor/Components/Controls/View.cs`

| Line | Issue | Severity |
|------|-------|----------|
| 67–68 | `ViewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)` in `SubscribeToViewModelCommands` — runtime property reflection over the generic `TViewModel`; the linker trims unknown property metadata | **High** |
| 74 | `property.GetValue(ViewModel)` — reflective property access on a type that may be trimmed | **High** |
| 86–89 | `UnsubscribeFromViewModelCommands` iterates `_subscribedCommands`; safe, but depends on commands being discovered via reflection above | **Indirect** |

### `src/ISynergy.Framework.UI.Blazor/Extensions/ServiceCollectionExtensions.cs`

| Line | Issue | Severity |
|------|-------|----------|
| 87 | `services.RegisterAssemblies(assembly, assemblyFilter)` — calls the reflection-based `ToViewTypes`/`ToWindowTypes`/`ToViewModelTypes` chain; see base UI plan for root cause | **Critical** |

### `src/ISynergy.Framework.UI.Blazor/Extensions/JsRuntimeExtensions.cs`

> To be verified: JSInterop `InvokeAsync<T>` with complex types requires `JsonSerializable` context if using System.Text.Json source generation. If `T` is `object` or an unconstrained type, that is a trimming issue for WASM AOT.

---

## No Source Generator Required

The assembly-scanning issue is resolved by consuming the output of `ISynergy.Framework.UI.SourceGenerator` (see base UI plan). No Blazor-specific generator is needed.

---

## Implementation Steps

- [ ] **Step 1 — Resolve `SubscribeToViewModelCommands` reflection in `View<TViewModel>`**

  Replace the reflection-based command discovery pattern:
  ```csharp
  // Current — reflection, AOT-unsafe
  var commandProperties = ViewModel.GetType()
      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .Where(p => typeof(ICommand).IsAssignableFrom(p.PropertyType));
  ```
  With an interface-driven approach. Define (or reuse if already present in the base) `ISynergy.Framework.Mvvm.Abstractions.ICommandProvider`:
  ```csharp
  public interface ICommandProvider
  {
      IEnumerable<ICommand> GetCommands();
  }
  ```
  Update `View<TViewModel>.SubscribeToViewModelCommands` to check if `ViewModel is ICommandProvider provider` and iterate `provider.GetCommands()`. This is a non-breaking change — ViewModels that do not implement `ICommandProvider` simply have no commands subscribed, preserving existing null-safe behavior.

  Add `[RequiresUnreferencedCode("Reflection-based command subscription is AOT-unsafe; implement ICommandProvider instead.")]` to the reflection fallback path as a build-time guardrail.

- [ ] **Step 2 — Update `ServiceCollectionExtensions` to use generated registration**

  In `ConfigureServices<...>`, replace:
  ```csharp
  services.RegisterAssemblies(assembly, assemblyFilter);
  ```
  With:
  ```csharp
  // AOT-safe: generated at compile time by ISynergy.Framework.UI.SourceGenerator
  services.AddUITypes();
  ```
  Keep the old overload marked `[RequiresUnreferencedCode]` for non-AOT scenarios.

- [ ] **Step 3 — Verify `JsRuntimeExtensions` JSInterop signatures**

  Read `src/ISynergy.Framework.UI.Blazor/Extensions/JsRuntimeExtensions.cs` and identify any `InvokeAsync<T>` calls where `T` is a non-trivially-serializable type not registered with a `JsonSerializerContext`. If found, add `[JsonSerializable(typeof(T))]` entries to a `BlazorSerializerContext` partial class (new file) and wire it into the `JsonSerializerOptions`.

- [ ] **Step 4 — Add trimmer annotations to the assembly**

  In `ISynergy.Framework.UI.Blazor.csproj`, verify or add:
  ```xml
  <IsTrimmable>true</IsTrimmable>
  <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
  ```
  Build with `dotnet publish -p:PublishTrimmed=true` targeting a Blazor WASM host and confirm zero `IL2xxx` warnings in this assembly.

- [ ] **Step 5 — Update `ICacheStorageService` serialization**

  Blazor-specific: `ICacheStorageService` implementations that serialize objects to local storage need `System.Text.Json` source generation context entries for every stored type. Verify the concrete implementation and add `[JsonSerializable]` attributes as needed.

- [ ] **Step 6 — Write unit tests**

  - Test that `View<TViewModel>` with a `TViewModel : ICommandProvider` subscribes commands without reflection
  - Test that `View<TViewModel>` with a ViewModel not implementing `ICommandProvider` does not throw
  - Test that `ConfigureServices` builds a valid `IServiceProvider` without reflection (use generated registrations)

---

## Notes

- **Blazor WASM AOT** (enabled via `<RunAOTCompilation>true</RunAOTCompilation>`) compiles IL to WASM at publish time. Reflection that accesses types not referenced statically will produce runtime `MissingMemberException` rather than compile-time errors, making this class of bug subtle.
- Blazor Server (non-WASM) runs on the .NET CLR and does not have NativeAOT restrictions; these fixes are therefore additive and non-breaking.
- The `[Inject] public required TViewModel ViewModel` pattern in `View<TViewModel>` is fully AOT-compatible because Blazor's DI resolution uses the generic `TViewModel` type parameter directly, not runtime type scanning.
- Blazor's own component infrastructure relies on `[Parameter]` / `[Inject]` attribute scanning by the Blazor build-time codegen (Razor compiler), which is already AOT-safe.

---

## Migration Guide

For Blazor WASM AOT, the `RegisterAssemblies` call is replaced by `AddUITypes()` from the base UI source generator. The `View<TViewModel>.SubscribeToViewModelCommands` reflection path is replaced by an `ICommandProvider` interface on ViewModels.

### Before

```csharp
// ServiceCollectionExtensions / Program.cs — reflection-based scan
services.RegisterAssemblies(
    Assembly.GetExecutingAssembly(),
    name => name.Name?.StartsWith("MyApp") == true);

// ViewModel — commands discovered via GetProperties reflection
public class UserViewModel : ViewModelBase
{
    public IAsyncRelayCommand SaveCommand { get; } = new AsyncRelayCommand(SaveAsync);
    // No ICommandProvider interface
}
```

### After

```csharp
// Program.cs — source-generator-emitted registration
services.AddUITypes(); // generated by ISynergy.Framework.UI.SourceGenerator

// ViewModel — implements ICommandProvider for AOT-safe command discovery
public class UserViewModel : ViewModelBase, ICommandProvider
{
    public IAsyncRelayCommand SaveCommand { get; } = new AsyncRelayCommand(SaveAsync);

    IEnumerable<ICommand> ICommandProvider.GetCommands()
        => [SaveCommand];
}
```

### Steps to Migrate

1. Replace `services.RegisterAssemblies(assembly, assemblyFilter)` with `services.AddUITypes()`.
2. Implement `ICommandProvider` on each ViewModel that has commands needing subscription in `View<TViewModel>`, returning all `ICommand` properties from `GetCommands()`.
3. ViewModels that do not implement `ICommandProvider` will simply have no commands auto-subscribed — verify this does not break existing behaviour.
4. If `ICacheStorageService` stores custom types in local storage, register those types with `[JsonSerializable]` in your application's `JsonSerializerContext`.
