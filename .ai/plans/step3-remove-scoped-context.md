# Step 3 Plan: Convert Scoped Services to Singletons and Remove ScopedContextService

## Background and Constraints

`ScopedContextService` was the session-switching mechanism: `CreateNewScope()` replaced the internal
`IServiceScope` so every call to `GetRequiredService<T>()` returned a fresh instance of whatever was
registered as scoped. Steps 1 and 2 replaced that mechanism with messenger-based lifecycle events
(`UserSignedInMessage` / `UserSignedOutMessage`). Step 3 tears out the plumbing that is no longer needed.

The removal must happen in five independent, buildable phases. Each phase is a separate commit.

---

## Phase 1: Promote infrastructure singletons — no behavior change

**Goal:** Change lifetime registrations for `ISettingsService`, `IAuthenticationProvider`,
`IMigrationService`, and `IThemeService` from scoped to singleton, and add `IApplicationLifecycleService`
direct injection where it is currently reached through `ScopedContextService`. Nothing calls
`ScopedContextService` any differently after this phase; scoped resolution still works because
`ScopedContextService` falls back to the root provider when the underlying scope returns the same
singleton instance.

### 1a — WinUI: `WindowsAppBuilderExtensions.cs`

File: `src/ISynergy.Framework.UI.WinUI/Extensions/WindowsAppBuilderExtensions.cs`

Change:
```
services.TryAddScoped<ISettingsService, TSettingsService>();
services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();
services.TryAddScoped<IMigrationService, MigrationService>();
services.TryAddScoped<IThemeService, ThemeService>();
```
To:
```
services.TryAddSingleton<ISettingsService, TSettingsService>();
services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
services.TryAddSingleton<IMigrationService, MigrationService>();
services.TryAddSingleton<IThemeService, ThemeService>();
```

The `IApplicationLifecycleService` is already registered as singleton in this file — no change.

### 1b — WPF: `WPFAppBuilderExtensions.cs`

File: `src/ISynergy.Framework.UI.WPF/Extensions/WPFAppBuilderExtensions.cs`

Same four lifetime changes. Additionally, `TContext` / `IContext` are currently scoped:
```
services.TryAddScoped<TContext>();
services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());
```
Change both to `TryAddSingleton`. This makes WPF consistent with WinUI and MAUI which already use
singleton for `IContext`.

Also remove `AddToResourceDictionary<T>` helper (line 113–127 of the file) — it takes
`IScopedContextService` as a parameter. If any WPF consumer calls it, the compiler will point it out.
Replace the helper's signature with `IServiceProvider` and call `serviceProvider.GetService<T>()`
directly, or delete it if no external caller exists (search the solution for `AddToResourceDictionary`).

### 1c — MAUI: `MauiAppBuilderExtensions.cs`

File: `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs`

Same four lifetime changes. `IContext` / `TContext` are already singleton here — no change.

### 1d — Add `IReloadable` contract to Core

File: `src/ISynergy.Framework.Core/Abstractions/Services/IReloadable.cs` (new file)

```csharp
namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Implemented by singleton services that must reload per-user state
/// when a new session begins.
/// </summary>
public interface IReloadable
{
    /// <summary>Called by the authentication layer after UserSignedInMessage is published.</summary>
    Task ReloadAsync(CancellationToken cancellationToken = default);
}
```

This interface will be implemented in Phases 2 and 3 by `ISettingsService` implementations.

**Verification:** Build all three platform projects. Zero new errors expected.

---

## Phase 2: Make singleton services self-reinitializing via IReloadable

**Goal:** Singleton services that previously got fresh state through scope recreation must now
reinitialize themselves when `UserSignedInMessage` arrives. `ISettingsService` is the primary case
(loads roaming/global settings bound to the signed-in user). `IAuthenticationProvider` and
`IThemeService` are stateless after construction and need no reload logic.

### 2a — `ISettingsService` implementations

Each app provides a concrete `TSettingsService`. The framework-side change is:

1. Add `IReloadable` to the `ISettingsService` interface (or leave the interface clean and require the
   concrete class to implement it separately — prefer the separate approach to avoid breaking all
   implementors).

2. In each concrete settings service (wherever `TSettingsService` is defined in the application code),
   implement `IReloadable.ReloadAsync()`:

```csharp
public async Task ReloadAsync(CancellationToken cancellationToken = default)
{
    LoadLocalSettings();
    await LoadRoamingSettingsAsync();
    await LoadGlobalSettingsAsync(cancellationToken);
}
```

3. Register a hosted background service or a messenger subscriber in Core that calls
   `ReloadAsync()` on all `IReloadable` singletons when `UserSignedInMessage` fires. The cleanest place
   is `IAuthenticationService`'s concrete implementation after it publishes `UserSignedInMessage` —
   it can resolve `IEnumerable<IReloadable>` from DI and call each one. Alternatively, each concrete
   singleton can subscribe to `UserSignedInMessage` directly in its constructor via `IMessengerService`.

   Recommended pattern for each reloadable singleton's constructor:
   ```csharp
   _messengerService.Register<UserSignedInMessage>(this, async _ => await ReloadAsync());
   ```

   This is consistent with how `ViewModel.OnUserSignedIn` is already wired.

### 2b — `IThemeService` (WinUI / MAUI)

After sign-in the theme is determined by the user's `LocalSettings.Theme` and `LocalSettings.Color`.
`ThemeService` should also subscribe to `UserSignedInMessage` and re-apply the theme:

```csharp
_messengerService.Register<UserSignedInMessage>(this, async _ => await ApplyThemeAsync());
```

`ApplyThemeAsync()` reads `_settingsService.LocalSettings` and calls whatever platform theming API
`ThemeService` already uses. Because `ISettingsService.ReloadAsync()` is called first (it was
registered before `IThemeService` in DI so it reinitializes first, or explicit ordering can be
enforced by having `ThemeService` depend on `ISettingsService` as a constructor argument), the
settings will already be current when the theme applies.

**Verification:** Sign in and sign out in a WinUI app; verify settings values reflect the new user.
Build passes with zero scoped-specific warnings.

---

## Phase 3: Replace ScopedContextService calls in BaseShellViewModel (all three platforms)

**Goal:** Remove all `_commonServices.ScopedContextService.GetRequiredService<X>()` calls from the
three `BaseShellViewModel` files. Replace with direct injected dependencies.

### 3a — Constructor injection changes

All three `BaseShellViewModel` constructors currently have the signature:
```csharp
protected BaseShellViewModel(ICommonServices commonServices, IDialogService dialogService,
    INavigationService navigationService, ILogger<BaseShellViewModel> logger)
```

Extend to:
```csharp
protected BaseShellViewModel(
    ICommonServices commonServices,
    IDialogService dialogService,
    INavigationService navigationService,
    ISettingsService settingsService,
    IServiceScopeFactory scopeFactory,
    ILogger<BaseShellViewModel> logger)
```

Store `_settingsService` and `_scopeFactory` as protected fields.

Files affected:
- `src/ISynergy.Framework.UI.WinUI/ViewModels/Base/BaseShellViewModel.cs`
- `src/ISynergy.Framework.UI.WPF/ViewModels/Base/BaseShellViewModel.cs`
- `src/ISynergy.Framework.UI.Maui/ViewModels/Base/BaseShellViewModel.cs`

### 3b — Dialog ViewModel factory pattern

`OpenLanguageAsync()` and `OpenColorsAsync()` need a fresh `LanguageViewModel` /
`ThemeViewModel` each time they are opened (they are dialogs — they carry transient state bound to a
single interaction). The correct pattern is to create a short-lived child scope per dialog invocation:

```csharp
protected virtual async Task OpenLanguageAsync()
{
    await using var scope = _scopeFactory.CreateAsyncScope();
    var languageVM = scope.ServiceProvider.GetRequiredService<LanguageViewModel>();
    languageVM.Submitted += LanguageVM_Submitted;
    await _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
    // scope disposes after await completes, cleaning up the ViewModel
}
```

The `IServiceScopeFactory` is already part of the .NET DI container and available as a singleton
without any registration change.

Do the same for `OpenColorsAsync()` with `ThemeViewModel`.

> Note: `LanguageViewModel` currently accesses `ScopedContextService` in its own constructor
> (line 31: `SetSelectedItem(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>()...)`).
> After Phase 1, `ISettingsService` is a singleton, so `LanguageViewModel` can have `ISettingsService`
> injected directly. Fix this in this phase by adding `ISettingsService` as a constructor parameter
> to `LanguageViewModel`.

File: `src/ISynergy.Framework.UI/ViewModels/LanguageViewModel.cs`
Change constructor to:
```csharp
public LanguageViewModel(ICommonServices commonServices, ISettingsService settingsService, ILogger<LanguageViewModel> logger)
    : base(commonServices, logger)
{
    SetSelectedItem(settingsService.LocalSettings.Language);
}
```

### 3c — Replace ISettingsService usages in event handlers

In `LanguageVM_Submitted` and `ThemeVM_Submitted` replace:
```csharp
_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings...
```
with:
```csharp
_settingsService.LocalSettings...
```

Replace:
```csharp
_commonServices?.ScopedContextService?.GetService<IExceptionHandlerService>()
```
with:
```csharp
_commonServices?.ExceptionHandlerService
```
`ExceptionHandlerService` is already on `ICommonServices` as a direct property.

### 3d — Derived shell view model constructors

Every application-level `ShellViewModel` that derives from `BaseShellViewModel` must forward the two
new parameters (`ISettingsService`, `IServiceScopeFactory`) to `base(...)`. These are automatically
resolved by the DI container — no registration change is needed. The compiler will catch any missing
parameter forwards at build time.

**Verification:** Grepping for `ScopedContextService` in the three `BaseShellViewModel` files
should return zero results.

---

## Phase 4: Replace ServiceLocator → IScopedContextService chains in infrastructure classes

**Goal:** Fix the remaining non-ViewModel usages listed below so they no longer go through
`IScopedContextService`. After this phase, `IScopedContextService` is referenced only by
`ICommonServices`, `ServiceLocator`, and the three `DialogService` implementations.

### 4a — `AuthorizationBehavior` (WinUI)

File: `src/ISynergy.Framework.UI.WinUI/Behaviors/AuthorizationBehavior.cs`

Current code in the constructor:
```csharp
var scopedContextService = ServiceLocator.Default.GetRequiredService<IScopedContextService>();
_authenticationProvider = scopedContextService.GetRequiredService<IAuthenticationProvider>();
```

After Phase 1, `IAuthenticationProvider` is a singleton, so it can be resolved directly from the
root provider:
```csharp
_authenticationProvider = ServiceLocator.Default.GetRequiredService<IAuthenticationProvider>();
```

The double indirection through `IScopedContextService` is gone. `ServiceLocator.Default` is still
in use here (the behavior is created by the XAML parser with no constructor injection available).
This is acceptable for Phase 4 — `ServiceLocator` itself will be simplified in Phase 5.

### 4b — `DateTimeOffsetToLocalDateStringConverter` (WinUI)

File: `src/ISynergy.Framework.UI.WinUI/Converters/DateTimeOffsetConverters.cs`

Current code:
```csharp
var scopedContextService = ServiceLocator.Default.GetRequiredService<IScopedContextService>();
var offset = scopedContextService.GetRequiredService<IContext>().TimeZone!.BaseUtcOffset;
```

After Phase 1, `IContext` is a singleton. Replace with:
```csharp
var context = ServiceLocator.Default.GetRequiredService<IContext>();
var offset = context.TimeZone!.BaseUtcOffset;
```

Apply the same pattern to the MAUI equivalent at:
`src/ISynergy.Framework.UI.Maui/Converters/DateTimeOffsetConverters.cs`

### 4c — `UserContextEnrichingLogProcessor`

File: `src/ISynergy.Framework.OpenTelemetry/Processors/UserContextEnrichingLogProcessor.cs`

Current:
```csharp
var scopedContextService = ServiceLocator.Default.GetRequiredService<IScopedContextService>();
var context = scopedContextService.GetRequiredService<IContext>();
```

Replace with:
```csharp
var context = ServiceLocator.Default.GetRequiredService<IContext>();
```

### 4d — `LoadingView` controls (WinUI + MAUI)

Files:
- `src/ISynergy.Framework.UI.WinUI/Controls/LoadingView.cs`
- `src/ISynergy.Framework.UI.Maui/Controls/LoadingView.cs`
- `src/ISynergy.Framework.UI.Maui/Controls/EmptyView.cs`

These reach `IApplicationLifecycleService` through `commonServices.ScopedContextService`. After
Phase 1, `IApplicationLifecycleService` is already a singleton. Replace:
```csharp
commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>()
```
with:
```csharp
commonServices.ExceptionHandlerService  // or the concrete singleton property
```
Wait — `IApplicationLifecycleService` is NOT on `ICommonServices`. The cleanest fix is to add it
as a constructor parameter to the control, or to have the control's parent pass it in. Since these
controls are constructed in code-behind (not from XAML), they can accept a direct parameter:

Change `LoadingView(ICommonServices commonServices, ...)` to
`LoadingView(ICommonServices commonServices, IApplicationLifecycleService lifecycleService, ...)`.

The call site in `Application.cs` (all three platforms) already has `_lifecycleService` as a field
— it was resolved through `ScopedContextService` in Phase 1's targets (Application constructors).
After Phase 1 makes `IApplicationLifecycleService` a direct singleton, simplify `Application.cs`:

Files:
- `src/ISynergy.Framework.UI.WinUI/Application/Application.cs`
- `src/ISynergy.Framework.UI.WPF/Application/Application.cs`
- `src/ISynergy.Framework.UI.Maui/Application/Application.cs`

Change:
```csharp
_settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();
_lifecycleService = _commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();
```
To direct DI resolution (these fields are already injected or can be added to constructor):
```csharp
_settingsService = Services.GetRequiredService<ISettingsService>();
_lifecycleService = Services.GetRequiredService<IApplicationLifecycleService>();
```
`IApplicationLifecycleService` is registered as singleton in all three extension files, so root
resolution is correct.

### 4e — `DialogService` (WinUI, WPF, MAUI)

Files:
- `src/ISynergy.Framework.UI.WinUI/Services/DialogService.cs`
- `src/ISynergy.Framework.UI.WPF/Services/DialogService.cs`
- `src/ISynergy.Framework.UI.Maui/Services/DialogService.cs`

`DialogService` uses `_scopedContextService` to resolve Windows and ViewModels by runtime type.
This is the trickiest removal because Windows (XAML views) are still registered as scoped by
`RegisterViewModel()` → `RegisterView()`.

Two-step fix within this phase:

**Step 1:** Change `DialogService` constructor injection from `IScopedContextService` to
`IServiceScopeFactory`. Store it as `_scopeFactory`.

**Step 2:** In each `ShowDialogAsync` overload that currently resolves `TWindow` and `TViewModel`
from `_scopedContextService`, create a short-lived scope instead:

```csharp
public async Task ShowDialogAsync<TWindow, TViewModel, TEntity>()
    where TWindow : IWindow
    where TViewModel : IViewModelDialog<TEntity>
{
    await using var scope = _scopeFactory.CreateAsyncScope();
    if (scope.ServiceProvider.GetRequiredService(typeof(TViewModel)) is IViewModelDialog<TEntity> viewmodel &&
        scope.ServiceProvider.GetRequiredService(typeof(TWindow)) is Window dialog)
        await CreateDialogAsync(dialog, viewmodel);
}
```

This preserves the per-dialog isolation (each dialog gets its own scope) and removes the dependency
on `IScopedContextService`. The key insight is that `DialogService` was one of the only consumers
that actually needed per-call scoping — not a long-lived one. Using `IServiceScopeFactory` directly
here is semantically correct and matches what `BaseNavigationService` already does for navigation frames.

After this change, `DialogService` no longer holds `IScopedContextService`.

### 4f — `NavigationExtensions` (WinUI + WPF)

Files:
- `src/ISynergy.Framework.UI.WinUI/Extensions/NavigationExtensions.cs`
- `src/ISynergy.Framework.UI.WPF/Extensions/NavigationExtensions.cs`

These are static helper methods that take `IScopedContextService` as a parameter. They are called
from `BaseNavigationService` which already carries an `IServiceScopeFactory`. Change the parameter
type from `IScopedContextService` to `IServiceProvider` (the scope's service provider) — callers
pass `scope.ServiceProvider`. The methods then call `GetRequiredService<T>()` on the `IServiceProvider`
directly. No caller outside of `BaseNavigationService` uses these helpers (verify with solution-wide
grep before changing).

**Verification:** Grep the solution for `.ScopedContextService` — it should appear only in
`ICommonServices.cs`, `ServiceLocator.cs`, and `ScopedContextService.cs` itself.

---

## Phase 5: Remove IScopedContextService from ICommonServices and delete ScopedContextService

**Goal:** After Phases 1–4 nothing consumes `IScopedContextService` except through `ICommonServices`.
This phase removes the contract from `ICommonServices`, obsoletes `ServiceLocator.CreateNewScope()`,
and cleans up `ServiceLocator` itself.

### 5a — Remove `ScopedContextService` property from `ICommonServices`

File: `src/ISynergy.Framework.Core/Abstractions/Services/ICommonServices.cs`

Remove the property:
```csharp
IScopedContextService ScopedContextService { get; }
```

Every concrete `CommonServices` implementation across the apps must also remove the corresponding
property implementation and constructor injection of `IScopedContextService`. The compiler will
identify every remaining usage of `commonServices.ScopedContextService` in application code —
those must be fixed before the build succeeds.

### 5b — Remove `IScopedContextService` DI registrations

In all three extension files, remove:
```csharp
appBuilder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
```

### 5c — Simplify `ServiceLocator`

File: `src/ISynergy.Framework.Core/Locators/ServiceLocator.cs`

`ServiceLocator` is still needed for XAML behaviors and converters that cannot receive injection.
After Phase 4, these call `GetRequiredService<IAuthenticationProvider>()` and
`GetRequiredService<IContext>()` directly — both are now singletons resolvable from the root provider.

Remove:
- The `_scopedContextService` field and all references to it
- The `CreateNewScope()` method
- The `ScopedChanged` event and its wiring
- The `IScopedContextService`-based constructor logic

Simplify to:
```csharp
public class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;
    private static ServiceLocator? _default;

    public ServiceLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _default = this;
    }

    public static ServiceLocator Default => _default
        ?? throw new InvalidOperationException("ServiceProvider has not been initialized.");

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _default = new ServiceLocator(serviceProvider);
    }

    public TService? GetService<TService>() where TService : class
        => _serviceProvider!.GetService<TService>();

    public TService GetRequiredService<TService>() where TService : notnull
        => _serviceProvider!.GetRequiredService<TService>();

    public IServiceProvider ServiceProvider => _serviceProvider!;
}
```

`ServiceLocator` no longer implements `IDisposable` — nothing to dispose.

### 5d — Delete `ScopedContextService.cs` and `IScopedContextService.cs`

Files to delete:
- `src/ISynergy.Framework.Core/Services/ScopedContextService.cs`
- `src/ISynergy.Framework.Core/Abstractions/Services/IScopedContextService.cs`

Run a final solution-wide grep for `IScopedContextService` and `ScopedContextService`. The only
remaining references should be in XML doc comments in `readme.md` files and possibly test projects.
Update or remove those.

### 5e — Remove `WPFAppBuilderExtensions.AddToResourceDictionary`

File: `src/ISynergy.Framework.UI.WPF/Extensions/WPFAppBuilderExtensions.cs`

The `AddToResourceDictionary<T>` extension method at line 113 takes `IScopedContextService`.
If no consumer exists in application code, delete it. If consumers exist, replace the parameter
with `IServiceProvider` (the root provider is always accessible as `app.Services`).

**Verification:**
- Solution builds with zero errors
- Grep for `IScopedContextService` returns zero results outside of git history
- Sign-in / sign-out flow tested manually: settings reload, theme reapplies, ViewModels
  reinitialize via `UserSignedInMessage` (already wired in `ViewModel.OnUserSignedIn`)

---

## Decision Record: Why IServiceScopeFactory for dialogs, not transient ViewModels

`LanguageViewModel` and `ThemeViewModel` could be registered as `Transient` instead of `Scoped`.
However, transient registration means every consumer gets a new instance even when that is not
desired (e.g., navigation). Using `IServiceScopeFactory` per dialog call is the established pattern
already in `BaseNavigationService` and preserves the ability for dialog ViewModels to have scoped
dependencies themselves. Keep `RegisterViewModel()` producing scoped registrations; the scope
lifetime is just controlled by the caller.

## Decision Record: ViewModel registration lifetime

`RegisterViewModel()` currently produces `TryAddScoped` registrations. After removing
`ScopedContextService`, ViewModels are only ever resolved through:
1. `BaseNavigationService` (already using per-frame `IServiceScopeFactory` scopes)
2. `DialogService` (using per-dialog scopes after Phase 4e)

There is no case where a ViewModel is resolved from the root provider. Scoped registration is
therefore still correct and should NOT be changed to transient or singleton.

## Decision Record: IContext in WPF

WPF currently registers `IContext` / `TContext` as scoped. Phase 1b changes this to singleton.
The implication is that `IContext.Profile` must be cleared explicitly on sign-out (which the sample
`AuthenticationService` already does: `context.Profile = null`). On sign-in,
`UserSignedInMessage(IProfile)` carries the profile and every subscriber (ViewModel, etc.) updates
its view accordingly. There is no longer any per-session isolation through separate `IContext`
instances — isolation is purely state-based.

## Execution Order and Build Gates

| Phase | Can merge when |
|-------|----------------|
| 1 | All three extension files compile; existing behavior unchanged |
| 2 | Sign-in/sign-out tested; settings reload correctly for new user |
| 3 | Three BaseShellViewModel files have zero ScopedContextService references |
| 4 | Solution-wide grep for `.ScopedContextService` shows only ICommonServices + ServiceLocator + the concrete class |
| 5 | Build succeeds with zero references to IScopedContextService or ScopedContextService anywhere in source |
