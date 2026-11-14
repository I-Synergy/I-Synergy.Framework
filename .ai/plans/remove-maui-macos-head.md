# Plan: remove the native macOS (AppKit) head from MAUI

Status: APPROVED (requested by the user)

## Why

The `net10.0-macos` / `net11.0-macos` heads depend on `Microsoft.Maui.Platforms.MacOS`
(`0.1.0-preview.*`), which the official docs class as **experimental and not officially supported by
Microsoft**. MAUI's supported macOS target is Mac Catalyst. The experimental head cannot be shipped in
the published package either (it is macOS-host only, and CI packs on Windows), so it adds build
conditions, package pins and a per-host TFM matrix for an unsupported surface.

## Supported heads after this change

`windows`, `android`, `ios`, `maccatalyst` — nothing else.

## Steps

- [ ] 1. `src/ISynergy.Framework.UI.Maui`: drop the two osx-gated TFMs, the macos
      `SupportedOSPlatformVersion`, and the `Microsoft.Maui.Platforms.MacOS*` PackageReferences
- [ ] 2. `src/ISynergy.Framework.UI.Maui`: remove `#if MACOS` code — delete
      `Extensions/CursorExtensions.MacOS.cs`, simplify `UseMauiAppForCurrentHead<TApplication>()` to
      `UseMauiApp<T>()`, drop the MediaElement guards in `LoadingView`
- [ ] 3. `samples/Sample.Maui`: drop the `net11.0-macos` TFM, macos conditions, AppKit package
      references, `_IncludeMacOSPlatformSources` target, `Platforms/MacOS/`, and the `#if !MACOS` /
      `#elif !MACOS` guards
- [ ] 4. `Directory.Packages.props`: remove the two `Microsoft.Maui.Platforms.MacOS*` pins
- [ ] 5. `.vscode`: remove the macOS build/run tasks and the launch configuration
- [ ] 6. Docs: `samples/Sample.Maui/readme.md`, `docs/developers/ui-heads-comparison.md`,
      `screenshots/README.md` (+ delete the two AppKit screenshots), test-project comment
- [ ] 7. `CHANGELOG.md` entry, build the framework + sample, update `.ai/session-context.md`

## Non-goals

- `ISynergy.Framework.UI.Maui.Mac.Tests` stays: it targets **Mac Catalyst** (`net11.0-maccatalyst`),
  not AppKit.
- `UseMauiAppForCurrentHead<TApplication>()` stays public (consumers call it); only its AppKit branch
  goes away.
- Tizen, GTK4 and WPF backends are untouched.
