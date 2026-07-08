# Refactor NavigateModalAsync to Use MAUI PushModalAsync

**Status:** PLANNING
**Started:** 2026-07-08

## Context

`NavigateModalAsync` currently replaces `Application.Current.Windows[0].Page` — it swaps the window root. This causes two problems:
1. **Tab bar disappears**: Shell singleton gets detached, BindingContext nulled, Handler preserved. On re-attach, MAUI skips Shell renderer init — no tab bar.
2. **Anti-pattern**: It violates standard MAUI navigation (PushAsync for modeless, PushModalAsync for modal, Window.Page only for initial setup).

The user wants: `NavigateAsync` → `PushAsync`, `NavigateModalAsync` → `PushModalAsync`. Shell must never be replaced.

## Design

### NavigateModalAsync Decision Rule

```
If mainPage is Shell → PushModalAsync (modal on top, Shell stays root)
Otherwise            → set Window.Page directly (initial setup, replacing LoadingView)
```

### Frame Management (critical)

The MAUI `GoBackAsync` already handles both modal and regular navigation stacks correctly. For `NavigateModalAsync` to work with it, the modal push path must mirror `NavigateAsync`'s frame setup:

```
1. Create scope
2. Create NavigationFrame { Scope, ViewModelType, Parameter }
3. HandleCurrentFrame(backNavigation: false)  → pushes current frame to _backStack
4. _currentFrame = newFrame
5. _pageFrames.Remove(page); _pageFrames.Add(page, newFrame)
6. _modalScope = scope
7. await mainPage.Navigation.PushModalAsync(page, Animated)
8. await view.ViewModel.InitializeAsync()
```

This ensures `GoBackAsync` can: dispose the modal frame via `HandleCurrentFrame(backNavigation: true)`, restore the previous frame from `_backStack`, and clean up via the `_pageFrames` safety net. No changes needed to `GoBackAsync`.

---

## Files to Modify

### 1. `src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs`

**`NavigateModalAsync` (lines 338-394)** — Rewrite with two code paths:

Path A — Initial setup (mainPage is null or NOT a Shell):
- Keep existing behavior: set `Window.Page = page`, clean up old page, dispose old scope
- Use `_modalScope` field for scope tracking

Path B — Modal push (mainPage IS a Shell):
- Create scope, NavigationFrame
- `HandleCurrentFrame(backNavigation: false)` to save current frame to backstack
- Set `_currentFrame = newFrame`
- Register in `_pageFrames`
- `await mainPage.Navigation.PushModalAsync(page, Animated)`
- `oldScope?.Dispose()` for previous modal scope cleanup

Common to both paths:
- Unconditional `page.Handler = null` (already in place from prior fix)
- `await view.ViewModel.InitializeAsync()`

### 2. `samples/Sample.Maui/App.xaml.cs`

**`OnUserSignedIn` (line 170)** — Was: `NavigateModalAsync<ShellViewModel>()`. Change to:
```csharp
// Pop all modals back to Shell (Shell was never disposed — it's still the root)
while (_navigationService.CanGoBack)
    await _navigationService.GoBackAsync();
```

**`OnUserSignedOut` (line 202)** — Was: `NavigateModalAsync<SignInViewModel>()`. Keep as-is. It now pushes SignIn as a modal on top of Shell instead of replacing the window root.

### 3. `samples/Sample.Maui/ViewModels/SignUpViewModel.cs`

Need to read this file to confirm, but based on agent exploration:
- Lines ~237 and ~280 call `NavigateModalAsync<SignInViewModel>()` 
- With modal push, this would stack another SignIn on top of SignUp+SignIn modals
- **Change to**: `_navigationService.GoBackAsync()` — pops SignUp modal, revealing SignIn underneath

### 4. Already done (prior fixes — no further changes)

- `SettingsViewModel.CancelAsync`: now uses `GoBackAsync()`
- `ShellViewModel.OpenSettingsAsync`: now uses `NavigateAsync<SettingsViewModel>()`
- `NavigationService.cs` handler nulling: now unconditional

---

## Navigation Flow After Changes

```
App startup:
  Window.Page = LoadingView
  OnApplicationLoaded → NavigateModalAsync<ShellViewModel>()
    → NOT Shell → set Window.Page = Shell  ✅

Open Settings:
  NavigateAsync<SettingsViewModel>()
    → PushAsync onto Shell's navigation stack
    → Shell stays root, tab bar stays visible  ✅
Cancel Settings:
  GoBackAsync() → PopAsync from navigation stack  ✅

Sign Out:
  NavigateModalAsync<SignInViewModel>()
    → IS Shell → PushModalAsync(SignIn)
    → Modal: Shell(root) → SignIn(modal)  ✅
Sign In (success):
  while (CanGoBack) GoBackAsync()
    → PopModalAsync → Shell revealed, ViewModel intact  ✅

Sign Up (from SignIn):
  NavigateModalAsync<SignUpViewModel>()
    → PushModalAsync → Modal: Shell → SignIn → SignUp  ✅
Back to Sign In (from SignUp):
  GoBackAsync() → PopModalAsync → Modal: Shell → SignIn  ✅
```

---

## Verification

1. `dotnet build src/ISynergy.Framework.UI.Maui` — 0 errors
2. `dotnet build samples/Sample.Maui` — 0 errors
3. Run on iOS/Android:
   - **Settings Cancel**: bottom tab bar visible after dismissing Settings
   - **Sign Out → Sign In**: Shell stays underneath, SignIn appears/disappears as modal
   - **Sign Up flow**: SignUp pushes on SignIn, back button returns to SignIn
   - **Platform back button**: properly dismisses modals
   - **Multiple sign-in/out cycles**: no modal stacking, no memory leaks
