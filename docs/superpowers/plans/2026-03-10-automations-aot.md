# ISynergy.Framework.Automations AOT Compatibility Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix two AOT blockers in `ISynergy.Framework.Automations` caused by `MakeGenericType` and runtime `GetInterfaces()` usage in `ActionExecutorFactory` — without introducing a source generator.
**Architecture:** The blockers are in a single file (`ActionExecutorFactory.cs`). The non-generic `GetExecutor(IAction action)` method constructs `IActionExecutor<TAction>` at runtime using `typeof(IActionExecutor<>).MakeGenericType(actionType)` and then uses `GetService(executorType)` (non-generic DI resolution). The fallback loop calls `e.GetType().GetInterfaces()` for each executor. Both patterns are AOT blockers. The fix is to register a compile-time dispatch table keyed by `Type` → `Func<IActionExecutor>` in `ServiceCollectionExtensions`, making the factory look up executors by concrete `Type` key rather than constructing generic types at runtime. No source generator is required because all action types are known at registration time.
**Tech Stack:** .NET 10, C# 14, `Microsoft.Extensions.DependencyInjection`.

---

## AOT Issues Found

### `src/ISynergy.Framework.Automations/Services/ActionExecutorFactory.cs`

| Line | Pattern | Issue |
|------|---------|-------|
| 42 | `typeof(IActionExecutor<>).MakeGenericType(actionType)` | `MakeGenericType` with runtime type — AOT blocker |
| 43 | `_serviceProvider.GetService(executorType)` | Non-generic `GetService(Type)` — AOT blocker |
| 50 | `_serviceProvider.GetServices<IActionExecutor>()` | Enumerates all executors then uses reflection to inspect interfaces |
| 51 | `e.GetType().GetInterfaces()` | Runtime interface enumeration — AOT blocker |
| 52–53 | `.IsGenericType && .GetGenericTypeDefinition() == typeof(IActionExecutor<>)` | Runtime generic type inspection — AOT blocker |
| 53 | `.GetGenericArguments()[0].IsAssignableFrom(actionType)` | `GetGenericArguments()` — AOT blocker |

---

## Implementation Plan

### Step 1 — Introduce a `ActionExecutorRegistry` (compile-time dispatch table)

- [ ] Create `src/ISynergy.Framework.Automations/Services/ActionExecutorRegistry.cs`:
  ```csharp
  /// <summary>
  /// Compile-time registry mapping concrete action types to their executor factory delegates.
  /// Replaces runtime MakeGenericType dispatch in ActionExecutorFactory.
  /// </summary>
  public sealed class ActionExecutorRegistry
  {
      private readonly Dictionary<Type, Func<IServiceProvider, IActionExecutor>> _factories = new();

      /// <summary>Registers a typed executor factory for the given action type.</summary>
      public void Register<TAction>(Func<IServiceProvider, IActionExecutor<TAction>> factory)
          where TAction : IAction
          => _factories[typeof(TAction)] = sp => factory(sp);

      /// <summary>Resolves an executor for the given action instance, or null if none registered.</summary>
      public IActionExecutor? Resolve(IAction action, IServiceProvider serviceProvider)
      {
          if (_factories.TryGetValue(action.GetType(), out var factory))
              return factory(serviceProvider);
          return null;
      }
  }
  ```

### Step 2 — Register the registry in `ServiceCollectionExtensions`

- [ ] Open `src/ISynergy.Framework.Automations/Extensions/ServiceCollectionExtensions.cs`
- [ ] Before the `AddHostedService` calls, add:
  ```csharp
  // Register compile-time action executor dispatch table
  services.TryAddSingleton(sp =>
  {
      var registry = new ActionExecutorRegistry();
      registry.Register<Actions.CommandAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.CommandAction>>());
      registry.Register<Actions.DelayAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.DelayAction>>());
      registry.Register<Actions.AutomationAction>(sp2 => sp2.GetRequiredService<IActionExecutor<Actions.AutomationAction>>());
      return registry;
  });
  ```

### Step 3 — Rewrite `ActionExecutorFactory.GetExecutor(IAction action)`

- [ ] Open `src/ISynergy.Framework.Automations/Services/ActionExecutorFactory.cs`
- [ ] Inject `ActionExecutorRegistry` into the constructor alongside `IServiceProvider`
- [ ] Replace the body of `GetExecutor(IAction action)`:
  ```csharp
  public object? GetExecutor(IAction action)
      => _registry.Resolve(action, _serviceProvider);
  ```
- [ ] Remove the `MakeGenericType`, `GetService(Type)`, `GetServices<IActionExecutor>()`, `GetInterfaces()`, and `GetGenericArguments()` calls

### Step 4 — Verify `StateTypeResolver` — no action needed

- [ ] `StateTypeResolver` uses a `Dictionary<Type, Type>` with `typeof()` literals only (no `Activator.CreateInstance`, no `MakeGenericType`) — this is AOT-safe.
- [ ] `value.GetType()` at line 41 is safe because it is used only as a dictionary key for the pre-populated `_typeMappings`, not for any reflection-based instantiation.

### Step 5 — Update XML documentation

- [ ] Document `ActionExecutorRegistry` with full XML
- [ ] Update `ActionExecutorFactory` XML to note that the non-generic path now uses the registry
- [ ] Add `<remarks>` to `AddAutomationServices` noting that custom action executors must be registered both via `TryAddScoped<IActionExecutor<TAction>>` AND via `ActionExecutorRegistry.Register<TAction>` for AOT compatibility

### Notes
- A Roslyn source generator is **not** needed because all action types are concrete and known at the call site of `AddAutomationServices`. If dynamic action types from external assemblies are needed in the future, a source generator approach (similar to the CQRS plan) would be appropriate.
- The `AutomationBackgroundService.Dispose()` method currently throws `NotImplementedException` — this is a separate bug unrelated to AOT but should be fixed (implement or remove `IDisposable`).

---

## Migration Guide

`ActionExecutorFactory` is refactored to use a compile-time `ActionExecutorRegistry` dispatch table instead of `MakeGenericType` and `GetInterfaces()` reflection. Custom action executor registrations now require an explicit entry in the registry.

### Before

```csharp
// AddAutomationServices — MakeGenericType resolved executors at runtime
services.AddAutomationServices(configuration);
// Custom action type resolved automatically via reflection in GetExecutor
```

### After

```csharp
// AddAutomationServices — now uses compile-time ActionExecutorRegistry
services.AddAutomationServices(configuration);

// If you have custom IAction types with custom IActionExecutor<T> implementations,
// register them explicitly in the registry:
services.Configure<ActionExecutorRegistry>(registry =>
{
    registry.Register<MyCustomAction>(sp =>
        sp.GetRequiredService<IActionExecutor<MyCustomAction>>());
});
```

### Steps to Migrate

1. Call `services.AddAutomationServices(configuration)` as before — built-in action types (`CommandAction`, `DelayAction`, `AutomationAction`) are registered automatically.
2. For any custom `IAction` types you have added to your application, add a `registry.Register<TAction>(...)` call in the registry configuration (see above).
3. The `IActionExecutorFactory.GetExecutor(IAction action)` public API is unchanged — existing callers continue to work.
4. Remove any workarounds that relied on the reflection-based fallback in `GetExecutor` — the fallback is now replaced by the compile-time registry lookup.
