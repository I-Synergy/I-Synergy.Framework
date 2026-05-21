# Navigation Architecture Refactor — Scope-Based Lifetime Management

## Context

The navigation backstack currently stores live `IViewModel` instances. ViewModels are registered as scoped but share a single `ScopedContextService` scope. This causes three interconnected problems:

1. **Backstack stores live objects** — stores `IViewModel` references, not navigation state  
2. **ViewModels leak on back navigation** — evicted ViewModels are `Cleanup()`'d but never disposed  
3. **No per-navigation scope isolation** — all ViewModels share one DI scope  

## Design

### NavigationFrame (scope-per-entry)

```csharp
private sealed class NavigationFrame : IDisposable
{
    public IServiceScope Scope { get; init; }
    public Type ViewModelType { get; init; }
    public object? Parameter { get; init; }

    public void Dispose() => Scope.Dispose();
}
```

Each frame owns a child `IServiceScope`. The scope caches one ViewModel (registered as scoped). Disposing the frame disposes the scope, which cascades to the ViewModel and all scoped dependencies.

### Backstack stores frames, not ViewModels

```
Current:  Frame{Scope=childScope#3, Type=InfoViewModel}
Backstack: [Frame{Scope=childScope#2, Type=SettingsVM}, Frame{Scope=childScope#1, Type=ShellVM}]
```

### Navigation flow

**Forward navigation (Shell → Info):**
1. Create child scope from root provider: `_serviceProvider.CreateScope()`
2. Resolve ViewModel from child scope: `scope.ServiceProvider.GetRequiredService<InfoViewModel>()`
3. Wrap current frame (Shell) in `NavigationFrame` → push to backstack
4. New frame (Info) becomes `_currentFrame`
5. Create view, set content, initialize

**Back navigation (GoBack):**
1. Dispose `_currentFrame.Scope` → ViewModel + scoped deps disposed
2. Pop frame from backstack → `_currentFrame = popped`
3. ViewModel already cached in the popped scope → resolve by Type
4. `IsInitialized = false` → force re-initialization
5. Create view, set content, initialize

**Reuse from backstack (navigate to type already in stack):**
1. Search backstack for frame with matching `ViewModelType`
2. Remove frame from backstack (reorder)
3. Push current frame to backstack
4. Found frame becomes `_currentFrame`
5. Same VM instance reused (still cached in its scope)

## Files to modify

| File | Changes |
|------|---------|
| `src/ISynergy.Framework.UI/Services/BaseNavigationService.cs` | Complete NavigationFrame-based backstack rewrite |
| `src/ISynergy.Framework.UI/Extensions/ReflectionExtensions.cs` | Already done — ViewModels are scoped |
| `src/ISynergy.Framework.Core/Services/ScopedContextService.cs` | No changes needed — NS creates scopes from root provider |
| `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs` | No changes — CreateView uses scopedContextService, remains compatible |
| `src/ISynergy.Framework.UI.WPF/Services/NavigationService.cs` | No changes — same pattern as WinUI |

**Not in scope:**
- MAUI NavigationService — uses platform-native `INavigation.PushAsync/PopAsync`, not `BaseNavigationService`
- Blade navigation — ViewModels are passed by caller, not resolved from DI  
- DialogService — resolves dialogs from shared scope; they're short-lived and self-disposing
- BaseShellViewModel sub-ViewModels (Language, Theme) — resolved from shared scope for modal dialogs

## Specific code changes in BaseNavigationService

### 1. Fields — replace Stack<IViewModel> with Stack<NavigationFrame>

```csharp
// BEFORE
private Stack<IViewModel> _backStack = new Stack<IViewModel>();
private NavigationFrame? _currentFrame; // partial — never assigned

// AFTER
private Stack<NavigationFrame> _backStack = new();
private NavigationFrame? _currentFrame;
private IServiceProvider RootProvider => _scopedContextService.ServiceProvider;
```

### 2. HandleCurrentViewModel — push frame, not ViewModel

```csharp
// BEFORE (broken)
_backStack.Push(currentViewModel);

// AFTER
if (_currentFrame is not null)
{
    _currentFrame.Parameter = currentViewModel.Parameter;
    _backStack.Push(_currentFrame);
}
_currentFrame.Dispose(); // for backNavigation
```

### 3. NavigateAsync — resolve from child scope

```csharp
// BEFORE
viewModel = _scopedContextService.GetRequiredService<TViewModel>();

// AFTER
var newScope = RootProvider.CreateScope();
viewModel = newScope.ServiceProvider.GetRequiredService<TViewModel>();
_currentFrame = new NavigationFrame { Scope = newScope, ViewModelType = typeof(TViewModel) };
```

### 4. TryRemoveFromBackStack — search by ViewModelType

```csharp
// BEFORE (broken)
var match = _backStack.FirstOrDefault(vm => vm is TViewModel);

// AFTER
var match = _backStack.FirstOrDefault(f => f.ViewModelType == typeof(TViewModel));
```

### 5. GoBackAsync — dispose current, pop frame

```csharp
// BEFORE (broken)
_backStack.Pop() is IViewModel viewModel

// AFTER
_currentFrame?.Dispose();
var frame = _backStack.Pop();
_currentFrame = frame;
var vm = frame.Scope.ServiceProvider.GetRequiredService(frame.ViewModelType);
```

### 6. CleanBackStackAsync — dispose all frame scopes

```csharp
foreach (var frame in _backStack) frame.Dispose();
_backStack.Clear();
```

### 7. Dispose — same pattern

```csharp
_currentFrame?.Dispose();
foreach (var frame in _backStack) frame.Dispose();
_backStack.Clear();
```

### 8. Modal navigation — not changed

Modal navigation doesn't use the backstack. It resolves ViewModels from `_scopedContextService`. This stays as-is — modals are ephemeral and self-disposing via the window lifecycle.

## Verification

1. **Build** all projects: `dotnet build` on Core, UI, WinUI, WPF, MAUI
2. **Unit tests** if any exist for navigation  
3. **Manual test in WinUI sample:**
   - Navigate Shell → Info → Settings → Info → Back → verify no COM exceptions, no memory growth
   - Navigate deep stack (>10) → verify oldest frames are trimmed (disposed)
   - Modal dialog (Language picker) → verify still works
   - Back navigation → verify ViewModels reinitialize correctly
