# MAUI NavigationService → Inherit from BaseNavigationService

**Status:** IN PROGRESS
**Started:** 2026-07-05

## Context

The MAUI `NavigationService` currently implements `INavigationService` directly with a standalone architecture using `ConditionalWeakTable<Page, IServiceScope>` (`_pageScopes`) to tie DI scopes to MAUI Page instances. The WinUI/WPF platforms both inherit from `BaseNavigationService` which uses a cleaner `Stack<NavigationFrame>` backstack where each frame directly owns its scope.

The `_pageScopes` approach caused a crash bug (fixed in `6c4abfcb`) where navigating to the same ViewModel twice threw `ArgumentException` because singleton views return the same Page instance from DI. While the fix was simple, the root cause is the architectural mismatch — WinUI/WPF's `NavigationFrame` pattern doesn't have this problem because scopes are owned by stack entries, not tied to Page instances via a weak table.

**Goal:** Refactor MAUI `NavigationService` to inherit from `BaseNavigationService`, replacing `_pageScopes` with the `NavigationFrame` backstack pattern. Eliminate the duplicate-key problem at the architectural level.

## Key Architectural Difference

WinUI/WPF use **frame content replacement** (`SetFrameContent(frame, page)`) while MAUI uses **push/pop** (`INavigation.PushAsync/PopAsync`). This means MAUI must override all navigation methods to use MAUI primitives while leveraging the base's `NavigationFrame` scope management.

## Design Challenge: Back-Navigation "Page Already in Stack" Path

When navigating to a ViewModel whose page is already on the MAUI navigation stack, intermediate pages are popped. Their scopes were previously tracked via `_pageScopes.TryGetValue(page)` — a direct Page→Scope lookup. Without `_pageScopes`, we must track which NavigationFrame corresponds to each page.

**Solution:** Maintain a 1:1 correspondence between the MAUI navigation stack and the `_backStack`. Each forward navigation pushes a frame to `_backStack`. When popping intermediate pages, pop and dispose frames from `_backStack` in LIFO order — they naturally correspond to the pages being popped from MAUI's stack.

## Phases

### Phase 1: Open BaseNavigationService for Inheritance

**File:** `src/ISynergy.Framework.UI/Services/Base/BaseNavigationService.cs`

| Member | Current | Target | Reason |
|--------|---------|--------|--------|
| `NavigationFrame` class (L61) | `private sealed` | `protected` (remove sealed) | MAUI must instantiate NavigationFrame directly |
| `_backStack` field (L74) | `private` | `protected` | MAUI needs to pop/push for GoBack and NavigateAsync |
| `_currentFrame` field (L79) | `private` | `protected` | MAUI sets this in NavigateAsync and GoBackAsync |
| `_modalScope` field (L91) | `private` | `protected` | MAUI's NavigateModalAsync uses this field |
| `HandleCurrentFrame` method (L334) | `private` | `protected virtual` | MAUI calls this for frame eviction |
| `InitializeViewAndViewModel` (L379) | `private` | `protected virtual` | MAUI may call after PushAsync |
| `TryRemoveFromBackStack<T>` (L435) | `private` | `protected virtual` | MAUI calls this in NavigateAsync for frame recycling |
| `NavigateAsync<T>(T,object?,bool)` (L526) | non-virtual | `public virtual` | MAUI overrides with push/pop logic |
| `NavigateAsync<T,V>(T,object?,bool)` (L596) | non-virtual | `public virtual` | MAUI overrides (keeps Obsolete throw) |
| `NavigateModalAsync<T>(object?)` (L657) | non-virtual | `public virtual` | MAUI overrides with root-page replacement |
| `GoBackAsync()` (L455) | non-virtual | `public virtual` | MAUI overrides with PopAsync |
| `CleanBackStackAsync(bool)` (L487) | non-virtual | `public virtual` | MAUI overrides with PopToRootAsync |
| `CanGoBack` (L97) | non-virtual | `public virtual` | MAUI checks MAUI's native stacks |
| `Dispose()` (L709) | non-virtual | `public virtual` | MAUI may add cleanup |

### Phase 2: Refactor MAUI NavigationService

**File:** `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs`

#### 2.1 Class declaration
Change `public class NavigationService : INavigationService` → `public class NavigationService : BaseNavigationService`

#### 2.2 Constructor
New signature: `(IExceptionHandlerService, ICommonServices, ILocalSettings, IServiceScopeFactory, ILogger<NavigationService>)`, chaining to `base(...)`.

`ICommonServices` is already registered in MAUI DI (`MauiAppBuilderExtensions.cs:112`). The rest are unchanged.

#### 2.3 Implement 8 abstract methods
- `GetNavigationFrame()` → `null` (never called, all nav methods overridden)
- `GetFrameContent/SetFrameContent` → throw `NotSupportedException` (MAUI uses push/pop)
- `GetWindowContent/SetWindowContent` → map to `Application.Current.Windows[0].Page`
- `CreateView` → full implementation matching WinUI pattern + MAUI RTL handling (used by inherited `NavigateAsync<T,V>` overloads)
- `MonitorViewLoadAsync` → `Task.CompletedTask` (MAUI pages use `OnNavigatedTo`, no Loaded event)
- `DispatchOnUIThread` → `MainThread.BeginInvokeOnMainThread` / direct call if already on main thread

#### 2.4 Override navigation members

**`CanGoBack`**: Checks MAUI's actual `NavigationStack.Count > 0 || ModalStack.Count > 0` (preserves existing behavior). Does NOT use `_backStack.Count` because MAUI's native back button can pop pages without going through `GoBackAsync`, causing backstack drift.

**`NavigateAsync<T>(TViewModel?, object?, bool)`**: Core override:
- Creates scoped provider, calls `NavigationExtensions.CreatePage<TViewModel>()` (preserves RTL, ViewModel resolution)
- If page already in MAUI stack (back-navigation): pops intermediate pages from MAUI, then pops and disposes corresponding frames from `_backStack`
- If forward navigation: creates `NavigationFrame`, calls `HandleCurrentFrame(false)` to push old frame, sets `_currentFrame`, calls `PushAsync`
- Calls `InitializeAsync` on ViewModel after navigation

**`NavigateAsync<T,V>` overloads**: Keep as `[Obsolete]` with `new` keyword throwing `NotImplementedException`

**`GoBackAsync()`**: 
- Pops from MAUI stack (`PopModalAsync` or `PopAsync`)
- `HandleCurrentFrame(true)` disposes current frame
- Pops from `_backStack` to restore `_currentFrame`
- Calls `CleanupPageResources(poppedPage)` for MAUI-specific cleanup
- Does NOT re-initialize ViewModel — MAUI handles this via `View.OnNavigatedTo`

**`NavigateModalAsync<T>(object?)`**:
- Creates `_modalScope`, creates page via `NavigationExtensions.CreatePage`
- Replaces `Application.Current.Windows[0].Page`
- `CleanupPageResources(oldPage)` on old root
- Disposes old `_modalScope`
- Preserves TabbedPage handler workaround

**`CleanBackStackAsync(bool)`**: Calls MAUI's `PopToRootAsync`, then disposes all `_backStack` frames.

#### 2.5 Replace `CleanupPage` with `CleanupPageResources`
New private static method that only handles MAUI-specific cleanup:
- Sets `page.BindingContext = null`
- Sets `page.Handler = null` (skipped for Singleton pages)
- ViewModel disposal and scope disposal are removed — those are now handled by `HandleCurrentFrame` / `NavigationFrame.Dispose()`

### Phase 3: Remove Dead Code

From `NavigationService.cs`, delete:
- `_pageScopes` static ConditionalWeakTable (L38)
- `_modalScope` field (L31) — inherited from base
- `_localSettings`, `_exceptionHandlerService`, `_logger`, `_serviceScopeFactory` fields — inherited from base
- `CleanupPage` method (L335-378) — replaced by `CleanupPageResources`
- `Dispose()` method — inherited from base
- `OnBackStackChanged()` — inherited from base (better dead-handler cleanup)
- `BackStackChanged` event — inherited from base
- `GoForwardAsync` throw — dead code, not on interface

### Phase 4: Update Tests

**`NavigationServicePageScopesTests.cs`**: Delete all 4 tests. `_pageScopes` no longer exists; the ConditionalWeakTable pattern they test is no longer used.

**`NavigationServiceCleanupTests.cs`**: Update reflection to look up `"CleanupPageResources"` instead of `"CleanupPage"`. Rename test methods accordingly.

**`NavigationServiceRetentionCycleTests.cs`**: Same method name update. Add `Mock<ICommonServices>` to constructor setup.

**`NavigationServiceExceptionHandlingTests.cs`**: Update NavigationService instantiation to pass `Mock<ICommonServices>`.

## Verification

1. Build `BaseNavigationService` → MAUI project → full solution (0 errors, 0 warnings)
2. Run `ISynergy.Framework.UI.Maui.Tests` — all tests pass
3. Run `ISynergy.Framework.UI.WinUI.Tests` — verify no regressions from Phase 1 changes
4. Run `Sample.Maui` and exercise: forward nav → back → forward again, modal nav, sign-in/sign-out ×20, backstack cleanup
5. Verify no `ArgumentException` from duplicate keys (original bug)
6. Verify RTL FlowDirection still applied
