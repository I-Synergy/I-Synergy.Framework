# MAUI Leak Intelligence Audit: I-Synergy.Framework

Repository: `https://dev.azure.com/i-synergy/I-Synergy.Framework`

Reviewed commit: `91c708f49e1d0f74399d7d8c5f091f90bacba684`

Audit mode: static source review using the `maui-memory-intelligence` skill. This report identifies leak-prone retention conditions from source evidence. It is not runtime heap proof.

## Summary

- Likely high risk: root page replacement can bypass teardown and leave old Shell view models subscribed to a long-lived navigation service.
- Possible medium-high risk: cursor behavior installs native platform callbacks that strongly capture `VisualElement` instances without a clear terminal detach path.
- Several initially suspicious patterns were not reported because they appear intentionally cleaned up or app-lifetime scoped.

## Finding 1: Root Page Replacement Can Bypass Teardown

Confidence: Likely

Severity: High

Primary rule: `MAUI-MEM-0023`

Subcondition: `MAUI-MEM-0023-S01` - MainPage or Window.Page replaced during login/logout

Related rule: `MAUI-MEM-0001-S01` - static/singleton event publisher subscribed by page/control

### Evidence

`NavigationService.NavigateModalAsync<TViewModel>` replaces the current window root directly:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs:157`

```csharp
Application.Current.Windows[0].Page = page;
```

The sample app exercises this path repeatedly during startup, sign-in, and sign-out:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/App.xaml.cs:122`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/App.xaml.cs:170`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/App.xaml.cs:202`

`BaseShellViewModel` subscribes to the navigation service:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/ViewModels/Base/BaseShellViewModel.cs:83`

```csharp
_navigationService.BackStackChanged += NavigationService_BackStackChanged;
```

It unsubscribes only if disposal runs:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/ViewModels/Base/BaseShellViewModel.cs:253`

`ShellView` has disposal logic that calls `ViewModel?.Dispose()`:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/Views/ShellView.xaml.cs:41`

However, the root page replacement path does not visibly call this disposal path before assigning the new root page.

`NavigationExtensions.CreatePage<TViewModel>` resolves pages and view models through `ServiceLocator.Default`:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Extensions/NavigationExtensions.cs:52`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Extensions/NavigationExtensions.cs:60`

If `ServiceLocator.Default` is the root service provider, scoped views/viewmodels can become effectively root-scoped and survive until app shutdown.

### Why I Think This

The rule fired because the reviewed code sets `Window.Page` to a new root page, and the app exercises that root replacement during authentication transitions. The `MAUI-MEM-0023-S01` detector calls out this exact shape: `Application.MainPage` or `Window.Page` replaced during login/logout while the old root is not visibly cleared or disposed.

The lifetime concern is concrete here: window, activity, scene, and app lifecycle roots naturally outlive individual pages. If an old root page, Shell, handler, native peer, or service callback remains reachable from one of those roots, it can retain the old page graph. The related `MAUI-MEM-0001-S01` condition also applies because `BaseShellViewModel` subscribes to a navigation service event and only unsubscribes when `Dispose` runs.

### Closest Known Match

Strong match: `MAUI-MEM-0023-S01` - root page replacement during login/logout.

Partial/supporting match: `MAUI-MEM-0001-S01` - transient page/control/viewmodel subscribed to a longer-lived publisher.

Closest catalog background: Android flyout/root native child retention, stale ShellContent retention, iOS/Mac Catalyst FlyoutPage replacement retention, and Windows MainPage replacement retention.

### Framework Sensitivity

MAUI app roots are tied to `Window`, `Activity`, scene, lifecycle, Shell, handlers, and native peers. Replacing a root page is framework-sensitive because the old root may have a managed page graph, binding contexts, handlers, platform views, and platform listeners. If teardown is not explicit, platform/app-lifetime objects can keep parts of that graph alive longer than app code expects.

### What Exactly Happens If It Is Real

The old Shell/root page is replaced visually, but some reference path still keeps it or its view model alive. The most visible app-owned path is the navigation service event subscription:

Likely retained graph:

```text
singleton/global NavigationService
  -> BackStackChanged subscriber
  -> old ShellViewModel
  -> service/viewmodel state
```

Potential platform-amplified graph:

```text
platform Window/Shell/native peer
  -> old root Shell/Page
  -> BindingContext
  -> old ShellViewModel
```

This matters because authentication root replacement can be repeated in one process lifetime, so any retained old root graph can multiply with sign-in/sign-out churn.

### Retained Graph

```text
Window/Activity/scene/lifecycle root
  -> old MainPage/Shell/Window
  -> page graph
  -> handlers/native peers
  -> services and BindingContexts
```

App-owned event path:

```text
NavigationService
  -> BackStackChanged event handler
  -> old ShellViewModel
  -> viewmodel/service state
```

### Blast Radius

Technical effects selected from the skill vocabulary:

- `retains-page`
- `retains-bindingcontext`
- `retains-viewmodel`
- `retains-visual-tree`
- `retains-handler`
- `retains-native-view`
- `retains-platform-listener`
- `contributes-gref-pressure`
- `ios-native-retain-cycle`
- `multiplies-with-navigation`

### Why Severity Is High

Severity is high because this is a whole-root replacement path. If retained, it can hold a full Shell/page/viewmodel graph and can multiply across repeated authentication transitions. The path may also include native handlers and native views, which makes Android GREF/native-memory pressure and iOS/Mac Catalyst native retain cycles relevant.

### Why Confidence Is Likely

More-likely signals observed:

- Root replacement occurs through `Window.Page`.
- The transition is tied to startup/sign-in/sign-out, so churn is plausible.
- `BaseShellViewModel` subscribes to a service event.
- The unsubscribe path depends on `Dispose`, but the root replacement path does not visibly dispose the old root.

Confidence reducers considered:

- `ShellView` does implement `Dispose`, which would unsubscribe through the view model if called.
- The platform may release some old root state naturally.
- Runtime proof was not collected in this pass.

The finding remains likely because the source clearly matches the known condition and no terminal old-root teardown is visible in the reviewed replacement path.

### How I Would Investigate This

- Exercise sign-in/sign-out or root page replacement 25-50 times.
- After each cycle or batch, force 3-4 full GCs and wait for finalizers.
- Capture before/after managed heap snapshots.
- Verify old `Window`, root page, `ShellView`, `ShellViewModel`, and `BindingContext` instances do not keep increasing.
- Inspect `NavigationService.BackStackChanged` subscriber state.
- On Android, monitor GREF count and native heap.
- On iOS/Mac Catalyst, inspect old controllers, native views, delegates, actions, and managed wrappers where tooling allows.

### Expected Validation After Mitigation

- Old root pages, Shells, view models, and binding contexts are collected after forced GC.
- The retained graph stabilizes at the currently visible root only.
- Android GREF/native heap returns near baseline after teardown.
- iOS/Mac Catalyst observers/controllers/delegates no longer keep old managed wrappers or native views alive.

### Recommended Mitigation

- Capture the old root page before `Application.Current.Windows[0].Page = page`.
- Before assigning the new root, explicitly dispose old page/viewmodel graphs where they implement `IDisposable`.
- Clear old `BindingContext` and other app-owned references during teardown.
- Disconnect or clear old handlers where safe for the platform.
- Prefer a per-root or per-page `IServiceScope` for pages/viewmodels and dispose that scope on root replacement.
- Avoid resolving scoped pages/viewmodels from the root provider unless they are intentionally app-lifetime.

### False-Positive Considerations

This would be a false positive if the old root is disposed elsewhere on replacement, if all old root references are weak or ID-only, or if the retained root is intentionally app-session lifetime. I did not find an old-root disposal call in the visible `NavigateModalAsync` assignment path, so I did not classify it as safe.

### Learn More

- `C412`: Android FlyoutViewHandler disconnect/native child retention - https://github.com/AdamEssenmacher/maui/blob/repro/android-flyoutview-native-content-retention-20260701/src/Controls/samples/AndroidFlyoutViewNativeContentRetentionRepro/README.md
- `M051`: HideSoftInputOnTapped can retain stale ShellContent pages - https://github.com/dotnet/maui/issues/35890
- `M066`: iOS/Mac Catalyst FlyoutPage replacement leaked handler/control graphs - https://github.com/dotnet/maui/issues/25505
- `M070`: Windows VisualDiagnosticsOverlay retained replaced MainPage content - https://github.com/dotnet/maui/issues/23034

## Finding 2: Cursor Native Callbacks Capture VisualElement Without Terminal Detach

Confidence: Possible

Severity: Medium-high

Primary rule: `MAUI-MEM-0009`

Subcondition: `MAUI-MEM-0009-S01` - custom handler attaches native event without disconnect cleanup

Related rule: `MAUI-MEM-0014`

Related subcondition: `MAUI-MEM-0014-S06` - gesture recognizer or display link captures view

### Evidence

`CursorBehavior` attaches to `VisualElement.PropertyChanged` and reapplies cursor behavior when `IsEnabled` changes:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs:23`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs:35`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Behaviors/CursorBehavior.cs:81`

On Windows, the extension attempts to remove existing event handlers, but the handlers are local functions created per invocation:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs:44`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs:48`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs:60`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs:75`

Because each call creates new delegate instances, the removal at lines 44-46 does not remove handlers from prior calls. The pointer-entered handler captures `visualElement`:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/Windows/Extensions/CursorExtensions.cs:65`

On iOS, `UIPointerInteraction` is added with a delegate that stores the `VisualElement`:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/iOS/Extensions/CursorExtensions.cs:22`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/iOS/Extensions/CursorExtensions.cs:27`

On Mac Catalyst, `PointerUIHoverGestureRecognizer` stores the `VisualElement`, and the callback lambda also captures it:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/MacCatalyst/Extensions/CursorExtensions.cs:23`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/MacCatalyst/Extensions/CursorExtensions.cs:28`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Platforms/MacCatalyst/Extensions/CursorExtensions.cs:56`

The behavior is applied broadly in styles and navigation item controls:

- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/Resources/Styles/Style.xaml:108`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/Resources/Styles/Style.xaml:215`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/Resources/Styles/Style.xaml:316`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/samples/Sample.Maui/Resources/Styles/Style.xaml:348`
- `/Users/adam/Documents/Codex/2026-07-02/use/work/I-Synergy.Framework/src/ISynergy.Framework.UI.Maui/Controls/NavigationItem/NavigationItem.xaml:44`

### Why I Think This

The rule fired because the cursor implementation registers native platform callbacks from transient MAUI visual elements, and the reviewed code does not show a terminal detach path for those native callbacks when the visual element/page is removed.

For Windows, the implementation attaches `PointerEntered` and `PointerExited` handlers to a native `UIElement`. The handlers are local functions created on each call, so the attempted `-=` removal uses different delegate instances from the delegates that were previously added. That means repeated cursor application can accumulate native event handlers, and those handlers capture `visualElement`.

For iOS and Mac Catalyst, the code adds `UIPointerInteraction` or `UIHoverGestureRecognizer` objects whose delegate/action paths hold the `VisualElement`. These native objects can participate in native reference-counted lifetimes that outlive managed view teardown unless they are removed.

### Closest Known Match

Strong match on Windows: `MAUI-MEM-0009-S01` - native event registered without symmetric disconnect cleanup.

Partial/supporting match on iOS/Mac Catalyst: `MAUI-MEM-0014-S06` - gesture recognizer or native callback captures a view.

Closest catalog background: native peer/listener retention after disconnect and iOS/Mac Catalyst gesture/native interaction retention.

### Framework Sensitivity

MAUI handlers bridge managed controls to native platform views. Native callbacks, gestures, delegates, and interactions can retain managed wrappers or closures. MAUI cannot infer cleanup for custom platform extension methods, so app code must detach native state symmetrically when the MAUI owner is removed.

### What Exactly Happens If It Is Real

Native callback state survives longer than the MAUI control that installed it. Because the callback captures `VisualElement`, the native peer can keep the managed visual element, binding context, page subtree, and handler graph reachable.

Windows retained graph:

```text
WinUI UIElement
  -> PointerEntered/PointerExited delegate
  -> closure
  -> VisualElement
  -> BindingContext/page graph
```

iOS/Mac Catalyst retained graph:

```text
UIPointerInteraction/UIHoverGestureRecognizer
  -> delegate/action
  -> VisualElement
  -> BindingContext/page graph
```

On Windows, repeated `SetCustomCursor` calls can also multiply handlers because prior local delegates are not removable by newly created local delegates.

### Retained Graph

```text
Native peer/listener/adapter
  -> handler/delegate/recognizer
  -> MAUI control
  -> page/BindingContext
  -> native view tree
```

iOS/Mac Catalyst variant:

```text
UIKit/AppKit interaction or gesture recognizer
  -> delegate/closure/native slot
  -> MAUI wrapper/control
  -> page or native view hierarchy
  -> payload state
```

### Blast Radius

Technical effects selected from the skill vocabulary:

- `retains-page`
- `retains-bindingcontext`
- `retains-visual-tree`
- `retains-handler`
- `retains-native-view`
- `retains-platform-listener`
- `contributes-gref-pressure`
- `ios-native-retain-cycle`
- `multiplies-with-navigation`

### Why Severity Is Medium-high

The retained target can include controls, binding contexts, handlers, native views, and potentially page subtrees. The cursor behavior is applied broadly in styles and navigation item controls, so a small callback issue could appear many times in a visual tree. I kept this below high confidence/high severity because the exact native peer lifetime after page teardown must be proven per platform.

### Why Confidence Is Possible

More-likely signals observed:

- Native events/delegates/recognizers are registered.
- Callbacks strongly reference `VisualElement`.
- Windows handler removal is mechanically ineffective for prior local-function delegates.
- No terminal behavior detach, unload, or handler-disconnect cleanup is visible in the reviewed files.

Confidence reducers considered:

- iOS/Mac Catalyst remove existing pointer interactions/recognizers on reapply, reducing duplicate accumulation during reapplication.
- Some platform native peers may be collected together with the MAUI owner in normal teardown.
- The reviewed code is an extension/behavior path rather than a full custom handler with explicit `DisconnectHandler`, so runtime ownership needs verification.

The finding remains possible because the callback shape is clearly risky, but the final retaining root depends on platform teardown behavior not proven by static review.

### How I Would Investigate This

- Open a cursor-heavy page with many styled buttons/navigation items.
- Toggle `IsEnabled` repeatedly to force cursor reapplication.
- Navigate away and repeat 25-50 cycles.
- After each cycle or batch, force 3-4 full GCs and wait for finalizers.
- Capture managed heap snapshots and verify old styled controls, handlers, pages, and binding contexts do not keep increasing.
- On Windows, inspect pointer handler counts or retained delegates on the native `UIElement`.
- On iOS/Mac Catalyst, inspect pointer interactions, gesture recognizers, delegates, native views, and managed wrappers.

### Expected Validation After Mitigation

- Old controls, handlers, pages, binding contexts, and native peers are collected after forced GC.
- Windows pointer handler counts do not increase after repeated reapplication.
- iOS/Mac Catalyst interactions/gesture recognizers do not keep old managed wrappers or native views alive.
- Native memory and platform peer counts return near baseline after navigation teardown.

### Recommended Mitigation

- Store exact native delegate instances per platform view or `VisualElement`.
- Detach those exact delegates during behavior removal, unload, handler disconnect, or handler change-to-null.
- On Windows, avoid per-call local functions for handlers unless the created delegates are stored and reused for removal.
- On iOS and Mac Catalyst, remove pointer interactions and hover recognizers during terminal teardown.
- Prefer weak references inside native callback delegates where feasible, with null checks.

### False-Positive Considerations

This would be safe if the native object and MAUI owner always share the same lifetime, if the native callbacks are removed during terminal teardown elsewhere, or if the callbacks do not strongly retain transient page/control state. The Windows delegate accumulation issue is still actionable even if page-level retention does not reproduce, because the attempted unsubscribe does not remove prior local-function handlers.

### Learn More

- `C043`: BlazorWebView handlers/managers can remain rooted by native WebView hooks - https://github.com/dotnet/maui/issues/17717
- `C055`: Android ShellFlyoutTemplatedContentRenderer native hook retention - https://github.com/AdamEssenmacher/maui/tree/repro/android-shell-flyout-native-hook-leak-20260626/src/Controls/samples/AndroidShellFlyoutNativeHookLeakRepro
- `C084`: iOS/Mac Catalyst GesturePlatformManager native interaction retention - https://github.com/AdamEssenmacher/maui/blob/repro/ios-dragdrop-interaction-retention-20260626/src/Controls/samples/DragDropInteractionRetentionLeakRepro/README.md
- `C155`: iOS/Mac Catalyst compatibility TableViewModelRenderer gesture retention - https://github.com/AdamEssenmacher/maui/blob/repro/ios-tableview-source-gesture-retention-20260627/src/Controls/samples/TableViewSourceGestureRetentionRepro/README.md
- `C161`: Legacy compatibility PhoneFlyoutPageRenderer element retention - https://github.com/AdamEssenmacher/maui/blob/repro/ios-phoneflyoutpagerenderer-element-retention-20260628/src/Controls/samples/PhoneFlyoutPageRendererElementRetentionRepro/README.md

## Patterns Reviewed But Not Reported As Findings

These areas looked potentially relevant during triage but did not meet the threshold for a leak finding in this static pass:

- `Application` global timer, exception, lifecycle, and messenger hooks: disposal paths appear to stop/unsubscribe these app-lifetime hooks.
- `MauiExceptions` static exception bridge: process-lifetime hooks do not appear to capture pages or transient view models directly.
- `SingleSelectionWindow` and `MultipleSelectionWindow`: page events are detached in disposal, and selected item handling does not appear to bind `SelectedItems` to a shared long-lived collection.
- `MauiControlsView` Picker/CollectionView collections: collections appear viewmodel-owned rather than shared singleton collections. Any extended lifetime risk is covered by Finding 1 if viewmodels are root-resolved.
- `LoadingView`: event and media cleanup paths are present, and the view appears startup/app-lifetime oriented.

## Recommended Next Step

Address Finding 1 first. It has the clearest source evidence and the broadest retention impact because authentication root replacement can retain whole UI and service graphs. Then fix the cursor callback lifecycle, especially the Windows delegate accumulation issue, because it is both broad and mechanically actionable.