# Upstream bug report — DRAFT (not filed)

**Target repo:** https://github.com/microsoft/fluentui-blazor
**Title:** `FluentLayout.IsMobile` is never set when the page is first loaded at ≤ `MobileBreakdownWidth`, leaving mobile navigation unreachable

**Suggested labels:** `bug`, `blazor`, `layout`

---

## Environment

| | |
|---|---|
| Package | `Microsoft.FluentUI.AspNetCore.Components` **5.0.0** (latest published; verified against nuget.org) |
| Hosting | Blazor Web App, `InteractiveServer`, .NET 10 |
| Browser | Chromium (Playwright), mobile viewport 480×800 |
| Render mode | Interactive (verified — `FluentLayout.OnAfterRenderAsync` runs and the JS helper is attached) |

## Summary

When the app is **first loaded** at a viewport width ≤ `MobileBreakdownWidth` (default 768), the
layout's `mobile` attribute is applied and the hamburger is shown, but `FluentLayout.IsMobile` in .NET
**stays `false`**. The two states disagree, and because both the navigation area and the drawer read
`IsMobile`, mobile navigation becomes unusable:

- `FluentLayoutItem.RenderThisArea()` keeps rendering `[area="nav"]` (it only drops it when
  `LayoutContainer.IsMobile` is true), so the nav items stay in the column that the container query
  hides via `display: none`.
- `FluentLayoutHamburger.RenderDrawer()` returns `false` (it only renders drawer content when
  `LayoutContainer.IsMobile` is true), so **the drawer opens empty**.

Net effect: `visibleNavItems = 0`. A user on a phone cannot navigate at all.

**Loading at desktop width and then crossing the breakpoint at runtime works correctly** — this
asymmetry is the key diagnostic and is what isolates the cause to initialisation ordering.

## Steps to reproduce

1. Run any app using `<FluentLayout NavigationDeferredLoading="true">` with a
   `<FluentLayoutHamburger Display="HamburgerDisplay.DesktopMobile" />` in the header and nav content
   in the `LayoutArea.Navigation` item (the standard arrangement from the docs/demo).
2. Load the page **directly at ≤768px** (mobile viewport, or a narrow desktop window that is narrow
   *before* the first render).
3. Tap the hamburger.

**Actual:** the drawer opens but is empty; the 4 nav items remain inside a `display: none` column.
**Expected:** the nav content relocates into the drawer (as it does on a runtime breakpoint crossing).

### Control: the same app loaded at desktop width, then resized across the breakpoint

Works — `navItemsInDrawer: 4`, `navAreaPresent: false`, `mainMenuIds: 1`.

## Evidence (measured DOM, 480×800, after clicking the hamburger)

```json
{"layoutMobileAttribute": true, "layoutWidth": 480,
 "navAreaPresent": true, "navAreaDisplay": "none", "navAreaBox": {"w":0,"h":0},
 "navItemsInNavArea": 4,
 "hamburgerDisplay": "flex", "hamburgerAriaExpanded": "true",
 "drawerPresent": true, "drawerDialogOpen": true,
 "navInDrawerCount": 0, "navItemsInDrawer": 0,
 "mainMenuIds": 1, "totalNavItems": 4, "visibleNavItems": 0,
 "layoutJs": {"layoutInitialized": true, "layoutHasDotNetHelper": true}}
```

Note `layoutMobileAttribute: true` **and** an attached .NET helper, yet .NET still behaves as desktop.
There are **no console or page errors** (0 across 45 diagnostic runs), which rules out an exception
inside the `[JSInvokable]` method.

## Root cause

The `mobile` **attribute** (set purely in JS) and `FluentLayout.IsMobile` (**only** set by the
`FluentLayout_MediaChangedAsync` JS→.NET callback) fall out of sync on an initial mobile load.

`src/Core.Scripts/src/Components/Layout/FluentLayout.ts` — `Initialize()`:

```js
if (layoutElement.fluentLayoutInitialized) {
    if (dotNetHelper !== null) {
        layoutElement._fluentDotNetHelper = dotNetHelper;
    }
    return;                       // ← returns WITHOUT re-creating the ResizeObserver
}
layoutElement.fluentLayoutInitialized = true;
layoutElement._fluentDotNetHelper = dotNetHelper;
...
const resizeObserver = new ResizeObserver(_entries => { ... invokeMethodAsync('FluentLayout_MediaChangedAsync', ...) ... });
resizeObserver.observe(layoutElement);
```

The callback also only fires on a **change** of mobile state:

```js
const hasMobileAttribute = layoutElement.hasAttribute('mobile');
const isMobileSize = isMobile(layoutElement);
if (!hasMobileAttribute && isMobileSize) { ...set attr, invoke 'mobile'... }
else if (hasMobileAttribute && !isMobileSize) { ...remove attr, invoke 'desktop'... }
```

The resulting sequence on an initial load at ≤768px:

1. `Startup.afterStarted()` calls `FluentLayout.LayoutAutoInitialize()`
   (`src/Core.Scripts/src/Startup.ts`, called from `afterStarted`), which runs
   `Initialize(null, layoutId)` — i.e. **with `dotNetHelper = null`**. This sets
   `fluentLayoutInitialized = true` and creates the `ResizeObserver`.
2. The observer's one-shot initial callback fires. It **does** set the `mobile` attribute, but
   `layoutElement._fluentDotNetHelper?.invokeMethodAsync(...)` is a **no-op** — the helper is still null.
3. `FluentLayout.OnAfterRenderAsync(firstRender)` then calls `Initialize(dotNetHelper, Id)`, which hits
   the `fluentLayoutInitialized` early-return branch above. It attaches the helper — **and never
   re-creates the observer, nor replays the current mobile state.**
4. No further box change occurs, so the callback never runs again and `FluentLayout_MediaChangedAsync`
   is **never invoked**. `IsMobile` keeps its default `false`.

Both consumers then take the wrong branch, as described in the summary:

- `src/Core/Components/Layout/FluentLayoutItem.razor.cs` → `RenderThisArea()`
  ```csharp
  if (Area == LayoutArea.Navigation && LayoutContainer.NavigationDeferredLoading && LayoutContainer.IsMobile)
      return false;   // never reached
  ```
- `src/Core/Components/Layout/FluentLayoutHamburger.razor.cs` → `RenderDrawer()`
  ```csharp
  if (LayoutContainer.NavigationDeferredLoading && !LayoutContainer.IsMobile)
      return false;   // taken incorrectly → empty drawer
  ```

`LayoutAutoInitialize()` is what makes this reachable in a normal interactive app: it exists so a
*statically rendered* layout can still wire the hamburger, but it wins the race against the
interactive `OnAfterRenderAsync`, and step 3's early return means the interactive pass can never
correct the state.

## Suggested fix

In the `fluentLayoutInitialized` early-return branch of `Initialize()`, when a real `dotNetHelper` is
attached for the first time, **replay the current mobile state** to .NET rather than assuming the
initial callback already delivered it. Something like:

```js
if (layoutElement.fluentLayoutInitialized) {
    const wasStatic = layoutElement._fluentDotNetHelper == null;
    if (dotNetHelper !== null) {
        layoutElement._fluentDotNetHelper = dotNetHelper;

        // The one-shot initial callback may have run before the interactive helper
        // was attached, in which case .NET never learned the current state. Replay it.
        if (wasStatic) {
            try {
                layoutElement._fluentDotNetHelper.invokeMethodAsync(
                    'FluentLayout_MediaChangedAsync',
                    isMobile(layoutElement) ? 'mobile' : 'desktop');
            } catch (error) { }
        }
    }
    return;
}
```

(Alternatively, make the observer callback idempotent by invoking the callback on every resize rather
than only on state *change* — but `FluentLayout_MediaChangedAsync` early-returns when the value is
unchanged, so replay-on-attach is the smaller change.)

## Impact / workaround

Any app that renders `FluentLayout` with `NavigationDeferredLoading="true"` (the documented pattern)
has **no working navigation on a cold mobile load**. Rotating the device or any viewport resize
self-heals it at runtime, which can make the bug appear intermittent and environment-dependent.

Consumer-side workaround: after the interactive circuit attaches, force the observed element's box to
change by ~0.5px and restore it on the next frame, so the existing `ResizeObserver` re-fires now that
`_fluentDotNetHelper` is set. Nudging the **height** (not width) avoids a transient wrong
mobile/desktop flip, since the callback's decision reads `offsetWidth` only.
