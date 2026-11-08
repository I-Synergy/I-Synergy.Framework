# MAUI: replace head-specific `#if` blocks with platform structure

Status: APPLIED (hybrid) 2026-11-08 — 13 of 22 conditional blocks removed; the rest are listed below with the reason they stay.
Date: 2026-11-08
Scope: `src/ISynergy.Framework.UI.Maui` (22 conditional blocks in 11 files). Samples are a follow-up.

## The problem

`#if WINDOWS` / `ANDROID` / `IOS` / `MACCATALYST` / `MACOS` sits inside shared files, so the same file
mixes platform APIs, makes the mobile heads hard to read, and hides compile errors on heads the developer
is not building. Inventory (block → enclosing member):

| File | Condition | Member / purpose |
|---|---|---|
| `Application/Application.cs` | WINDOWS | `CreateWindow` — window setup |
| `Behaviors/SelectAllOnFocusBehavior.cs` | ANDROID ×3 | `OnAttachedTo`/`OnDetachingFrom`/`Focused` — native `EditText` selection |
| `Controls/LoadingView.cs` | MACOS ×2, `!MACOS` ×1 | AppKit-specific loading surface |
| `Exceptions/MauiExceptions.cs` | WINDOWS, `IOS \|\| MACCATALYST` | platform exception hooks |
| `Extensions/ApplicationExtensions.cs` | WINDOWS ×2 | launch-arguments parsing (WinRT `AppInstance`) |
| `Extensions/CursorExtensions.MacOS.cs` | MACOS | cursor mapping |
| `Extensions/MauiAppBuilderExtensions.cs` | DEBUG | logging only (not platform) |
| `Extensions/PageExtensions.cs` | WINDOWS | platform page tweak |
| `Services/ClipboardService.cs` | WINDOWS ×2 | WinRT bitmap decoding |
| `Services/DialogService.cs` | WINDOWS | `TryEventLogNotificationAsync` |
| `Services/ThemeService.cs` | ANDROID, IOS, WINDOWS ×2 | `UpdatePlatformColors` + title bar |

## Options

1. **Platform folders + partial classes (recommended base)** — MAUI already excludes
   `Platforms/<head>/**` from the other TFMs, so a file under `Platforms/Windows/` compiles only for
   Windows without any `#if`. A `partial class ThemeService` in `Services/ThemeService.cs` plus
   `Services/ThemeService.Windows.cs` / `.Android.cs` / `.iOS.cs` keeps one public type and moves the
   native API into head-specific files.
   - Pros: no public API change, directly readable per head, compile errors surface per TFM, no new
     abstractions.
   - Cons: more files; the shared file must declare partial hooks (`partial void`).
2. **Handler mappers** — for behaviour that belongs to a *control*, use
   `ConfigureMauiHandlers`/`Handler.Mapper.AppendToMapping` inside `Platforms/<head>`. The repo already
   does this for the Windows accent/focus work (`Platforms/Windows/Extensions/ControlAccentStyler.cs`).
   Fits `SelectAllOnFocusBehavior` (→ `EntryHandler` mapper on Android) and the focus-rectangle work, but
   not services such as clipboard/theme/dialog.
3. **Extension methods** — only useful for *pure* API sugar over a platform type. `CursorExtensions.MacOS`
   is already that shape; it does not solve service-level conditionals.

**Recommendation: 1 + 2 as a hybrid** — partial classes + platform folders for the services and
application/exception hooks (15 blocks), handler mappers for the control behaviour (Android selection,
3 blocks), and keep the single `DEBUG` block where it is (it is not platform-specific). Extension methods
remain the pattern for cursor sugar.

## Target layout (library)

```
Services/ThemeService.cs              partial class, shared flow + partial void hooks
Services/ThemeService.Windows.cs      Platforms-independent partial, guarded by folder? see below
Platforms/Windows/Services/ThemeService.Windows.cs
Platforms/Android/Services/ThemeService.Android.cs
Platforms/iOS/Services/ThemeService.iOS.cs
Platforms/Windows/Services/ClipboardService.Windows.cs
Platforms/Windows/Services/DialogService.Windows.cs
Platforms/Windows/Application/Application.Windows.cs
Platforms/Windows/Exceptions/MauiExceptions.Windows.cs
Platforms/iOS/Exceptions/MauiExceptions.iOS.cs
Platforms/Android/Handlers/SelectAllOnFocusMapper.cs
```

Note: `Platforms/<head>/**` is included per TFM by the MAUI SDK, so files there need **no** `#if`. The
`MACOS` head (MAUI Labs, `net*-macos`) uses `Platforms/MacOS/`; that folder must be confirmed as
auto-included for the `macos` TFM before moving `LoadingView`/`CursorExtensions`.

## Constraints and risks

- **Public API unchanged**: types, members and names stay exactly as they are (the alignment work just
  finished). Only file layout and `partial` modifiers change.
- **Behaviour unchanged**: this is a move, not a rewrite. Each moved block keeps its body verbatim.
- **`partial void` hooks** are required where the shared flow calls into the platform part; they are
  silently dropped when not implemented, so a missing head file fails silently — mitigate by giving each
  head a file (even an empty one) and adding a test that the platform hook is wired.
- **Build matrix**: must stay green for `net*-windows`, `-android`, `-ios`, `-maccatalyst`, `-macos`
  (macos only on macOS) plus `Sample.Maui` and the MAUI test project.
- **Not in scope**: the `DEBUG` logging block, Blazor/WPF/WinUI heads, and sample-level `#if` blocks
  (separate pass).

## Verification per step

- `dotnet build src/ISynergy.Framework.UI.Maui -f net11.0-windows10.0.26100.0`
- same for `-f net11.0-android`, `-f net11.0-ios`, `-f net11.0-maccatalyst` (workloads required)
- `dotnet build samples/Sample.Maui -f net11.0-windows10.0.26100.0`
- `dotnet test tests/ISynergy.Framework.UI.Maui.Windows.Tests`
- `grep -rn "#if \(WINDOWS\|ANDROID\|IOS\|MACCATALYST\|MACOS\)" src/ISynergy.Framework.UI.Maui`
  returns only files that genuinely need it (target: zero in `Services/`, `Application/`, `Exceptions/`).

## Order of work

1. `ThemeService` (4 blocks, biggest win) — proves the partial + platform-folder pattern.
2. `ClipboardService`, `DialogService`, `Application`, `MauiExceptions`, `ApplicationExtensions`,
   `PageExtensions` (move verbatim).
3. `SelectAllOnFocusBehavior` → Android handler mapper.
4. `LoadingView` / `CursorExtensions` macOS after confirming the `Platforms/MacOS` include.
5. Re-run the build matrix and the grep above; update `docs/developers/ui-heads-comparison.md`.

## Applied

| File | Before | After |
|---|---|---|
| `Services/ThemeService.cs` | ANDROID, IOS, WINDOWS ×2 | shared flow + `partial void ApplyPlatformColors`; `Platforms/Windows/Services/ThemeService.Windows.cs` (accent resources, `ControlAccentStyler`, `UpdateWindowsTitleBar`), `Platforms/Android/...Android.cs`, `Platforms/iOS/...iOS.cs` |
| `Services/DialogService.cs` | WINDOWS | `partial void WritePlatformEventLog(..., ref bool)` + `Platforms/Windows/Services/DialogService.Windows.cs` |
| `Exceptions/MauiExceptions.cs` | WINDOWS, `IOS \|\| MACCATALYST`, ANDROID | `static partial void RegisterPlatformHandlers()`; Windows / Android / iOS / MacCatalyst files |
| `Extensions/PageExtensions.cs` | WINDOWS | `static partial void ApplyPlatformFlowDirection<T>` + `Platforms/Windows/Extensions/PageExtensions.Windows.cs` |
| `Application/Application.cs` | WINDOWS + `#else` | **no conditional at all**: `DeviceInfo.Platform == DevicePlatform.WinUI` picks the custom title bar, every other head keeps the caption clock (`Window.TitleBar` is a cross-platform API) |
| `Behaviors/SelectAllOnFocusBehavior.cs` | ANDROID ×3 | shared Focused handler + `partial void OnAttachedPlatform/OnDetachedPlatform`; `Platforms/Android/Behaviors/SelectAllOnFocusBehavior.Android.cs` sets the native flag. Side effect: Android now also runs the shared Focused handler (idempotent). |

Verification: library builds for `net11.0-windows10.0.26100.0`, `net11.0-android`, `net11.0-ios`,
`net11.0-maccatalyst`; `Sample.Maui` builds and runs (settings blade opens, theme/language correct);
`ISynergy.Framework.UI.Maui.Windows.Tests` 47 passed / 4 skipped; the full UI build matrix (WPF, WinUI,
MAUI, Blazor, all four samples) is green.

## What still has a conditional, and why

| File | Condition | Reason |
|---|---|---|
| `Services/ClipboardService.cs` | WINDOWS (+ ANDROID / iOS in usings) | The method *returns* an image; a `partial void` hook cannot return it, and the head matrix includes macOS, which has no platform folder to put a partial in yet. |
| `Extensions/ApplicationExtensions.cs` | WINDOWS | WinRT `AppInstance` launch arguments; the other heads return `null`. Splitting needs one tiny per-head file each. |
| `Controls/LoadingView.cs` | MACOS ×2, `!MACOS` | AppKit-only surface; move to `Platforms/MacOS` once that folder is confirmed for the `net*-macos` TFM (the sample already uses it). |
| `Extensions/CursorExtensions.MacOS.cs` | MACOS | Same macOS-folder question; the file is already platform-suffixed. |
| `Extensions/MauiAppBuilderExtensions.cs` | DEBUG | Not platform-specific (debug logging). |

## Follow-up

1. Confirm `Platforms/MacOS/**` inclusion for the `net*-macos` TFM, then move the macOS blocks (3 files).
2. Convert `ClipboardService` by giving each head a partial file (Windows, Android, iOS, MacCatalyst,
   MacOS) that implements `GetPlatformImageFromClipboardAsync()`.
3. Same for `ApplicationExtensions` (5 one-line files) if the remaining two `#if`s are worth it.

## Samples: no platform logic left in Sample.Maui

The MAUI sample now calls framework APIs only; the platform decisions live in the library:

| Was in the sample | Now |
|---|---|
| `#if WINDOWS` focus-visual + hand-cursor handler mappers and `ApplyHandCursor` | `Platforms/Windows/Extensions/ShellChromeStyler` (library) registers the mappers from `ConfigureServices`; the sample only sets `StyleId` values (now published as `ShellChromeStyler.*StyleId` constants) |
| `#if MACCATALYST` window menu bar (File/View/Help) | `PageExtensions.AddShellMenuBar(...)`; the library decides which heads render a menu bar (`PageMenuExtensions.SupportsMenuBar`) |
| `#if WINDOWS` single instance + activation handlers with dialogs | `ActivationExtensions.ConfigureActivationDiagnostics()` — real implementation in `Platforms/Windows/Extensions`, no-op on every other head |
| `#if MACOS` `UseMauiAppMacOS` + `AddMacOSEssentials` (and its usings) | `MauiAppBuilderExtensions.UseMauiAppForCurrentHead<TApplication>()` |

What remains in the sample (and why it is not platform logic):
- `#if !MACOS` around `UseMauiCommunityToolkitMediaElement` and `ConfigureSyncfusionCore` — **package**
  limitations (those packages ship no AppKit asset), not head logic. Grouped under one comment.
- `#if DEBUG` around the DevFlow agent — a build configuration, not a platform.
- `Platforms/{Android,MacCatalyst,MacOS,Windows,iOS}/**` — the app entry points MAUI requires
  (`MainActivity`, `AppDelegate`, `Main`, manifests); scaffolding, not behaviour.
- XAML `OnPlatform` values (header padding, a keyboard accelerator, a dialog width) — layout values, not
  logic; they stay next to the layout they affect.
