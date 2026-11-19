# MAUI Shell Rebuild Fix

Status: DONE
Started: 2026-11-19
Finished: 2026-11-19
Branch: `fix/maui-shell-root-navigation`

## Goal

Prevent `NavigateModalAsync<IShellViewModel>` from pushing a shell onto the native modal
stack, which crashes iOS/Mac Catalyst in `ShellFlyoutRenderer.LayoutSidebar` when a shell is
re-presented (the language-change path in The Quran).

## Steps

- [x] Add shell-root handling to `NavigationService.PresentModalPageAsync`
- [x] Diagnose why the sample shell did not render after the root swap on iOS
- [x] Re-apply the window `Content` mapping after a root swap
- [x] Keep the singleton shell alive when it is re-presented while already the root
- [x] Validate `samples/Sample.Maui` on the iOS simulator and Mac Catalyst
- [x] Multi-target compile check (`net10.0-maccatalyst`, `net11.0-ios`, `net11.0-maccatalyst`)

## What the framework now does

`NavigationService.PresentModalPageAsync` treats an `IShellView` as a root surface:

1. If the shell is already `Window.Page`, it returns without touching the page (the previous
   code nulled `page.Handler` unconditionally in `NavigateModalAsync`, which detached the live
   shell's native tree even when navigation then no-opped).
2. Otherwise it installs the shell as the window root instead of pushing it as a modal.
3. After the swap it re-applies the window's `Content` mapping on the next dispatcher turn.

`NavigateModalAsync` no longer initializes the view model of a shell; shells initialize
themselves from their `Loaded` handler (`BaseShellViewModel.ShellLoadedAsync`).

## Why the extra step is needed (root cause of the blank window)

Managed tracing showed the swapped-in page never received a layout pass
(`ShellView [-1x-1]`) and the native window kept the previous page's view controller
(`UIWindow → UIView → UIView → UIView` with no children). MAUI 11 maps a window's root page
under the handler key `Content` and re-runs that mapping from
`Window.OnPropertyChanged("Page")`, but on iOS/Mac Catalyst that automatic update did not
attach the replacement page once the window was live. Explicitly re-applying the mapping
(`IElementHandler.UpdateValue(nameof(IWindow.Content))`) attaches the new page's view
controller and the shell lays out normally; doing it inline in the same turn still lost the
mapping, so it runs on the following dispatcher turn.

The reported `Could not find System.Security.Permissions ... referenced by assembly mscorlib`
line is emitted by the runtime at start-up on **both** the sample and the working The Quran
build (neither app bundle ships that assembly, and The Quran's bundle has the identical
mscorlib reference). It is not fatal and not the cause of the blank window.

## Validation

Builds (`MSBUILDDISABLENODEREUSE=1`, `-p:NodeReuse=false`, packages restored into `$TMPDIR`):

- `samples/Sample.Maui` — `net11.0-ios` (iossimulator-arm64) Debug: 0 warnings, 0 errors.
- `samples/Sample.Maui` — `net11.0-maccatalyst` (maccatalyst-arm64) Debug: 0 warnings, 0 errors.
- `ISynergy.Framework.UI.Maui` — `net10.0-maccatalyst` Debug: 0 warnings, 0 errors.

Runtime, `SAMPLE_TEST_SHELL_REBUILD=1` (presents the shell through the navigation path, then
re-presents it two seconds later):

- iPhone 17 simulator (iOS 26.5, Debug): shell renders (header, breadcrumb, navigation host,
  content), re-presentation completes, no `ShellFlyoutRenderer` crash, process stays up.
- Mac Catalyst (Debug, maccatalyst-arm64): `ShellView` lays out at 1440×900, navigation returns,
  re-presentation completes, process stays up.

The sample hook is kept and documented in `samples/Sample.Maui/App.xaml.cs` as the manual
regression check for this path.

## Notes / follow-ups

- A unit test of this path is not practical: `PresentModalPageAsync` needs a live
  `Microsoft.Maui.Controls.Application` window and a platform handler. The Windows-only
  `NavigationService*Tests` cover the extracted helpers and are unaffected.
- The Quran's own test run and `.ai/completed/ios-surah-load-latency.md` update could not be
  performed from this session: that repository is outside this workspace and read-only here
  (restore writes to `src/TheQuran.Common/obj/` are denied), so its 190-test run must be
  redone where the repository is writable.
- The remaining reader/mushaf workarounds in The Quran target Microsoft MAUI control defects
  (`Span` font resources, long `FormattedText`, `CollectionView` header measure, per-span
  gestures) and are not I-Synergy.Framework-owned code.
