# Plan: verify MAUI on Android + macOS, add a native macOS (AppKit) target

**Status:** COMPLETE — results in `.ai/completed/maui-android-macos-heads.md`
**Started:** 2026-11-05
**Branch:** chore/sonarcloud-cleanup-2026-11-04

## Problem

CI (`.github/workflows/ci.yml`) runs on `windows-2025` only. Every MAUI TFM other than
`net10.0-windows*` — `android`, `ios`, `maccatalyst` — therefore compiled only through the
Windows agent, and the recent analyzer/refactor pass plus `0190d8be` ("drop PackageReferences
already supplied transitively") were never exercised on a non-Windows toolchain. The sample has
no native macOS target at all: `maccatalyst` is the only macOS story.

## Decisions (user-selected)

1. **Add a native AppKit head (`net10.0-macos`)** using the experimental
   `Microsoft.Maui.Platforms.MacOS` backend (`0.1.0-preview.14.26531.2`), not just Mac Catalyst.
2. **Verify Android on an emulator**, not compile-only.

## Environment constraints found

- The agent shell is sandboxed: writes are allowed only in the workspace and `$TMPDIR`.
  `dotnet workload install` (writes to the SDK root) and `brew` both fail with
  "Operation not permitted", so the toolchain is installed under `$TMPDIR` instead:
  a clone of the SDK root, `maui` + `macos` workloads, Temurin JDK 21, and the Android
  command-line tools. Nothing is added to the repository and no `sudo` is used.
- `global.json` pins SDK `11.0.100-rc.3.26522.148`; the clone keeps that exact SDK.
- Disk headroom is ~14 GB, so `NUGET_PACKAGES` points at `$TMPDIR/nuget` (the shared
  `~/.nuget/packages` cache holds older MAUI/Syncfusion versions and is not writable).

## Steps

- [ ] 1. Toolchain: SDK clone + `maui`/`macos` workloads, JDK 21, Android SDK 36 + emulator image
- [ ] 2. Baseline: build `Sample.Maui` for `net10.0-maccatalyst` and `net10.0-android`; record failures
- [ ] 3. Add `net10.0-macos` to `ISynergy.Framework.UI.Maui` (macOS host only) and fix platform guards
- [ ] 4. Add `net10.0-macos` to `samples/Sample.Maui`, `Platforms/MacOS` head, guard MediaElement/Syncfusion
- [ ] 5. Fix every platform break found in steps 2-4
- [ ] 6. Run: Mac Catalyst app, macOS AppKit app, Android emulator
- [ ] 7. Record results in `.ai/progress/` and update the sample readme

## Risks

- `Microsoft.Maui.Platforms.MacOS` is experimental and built against MAUI `10.0.41`; the repo
  pins `10.0.120` (net10) — compatible floor, not a tested pairing.
- `CommunityToolkit.Maui.MediaElement` and `Syncfusion.Maui.Core` ship **no AppKit asset**.
  They resolve through their plain `net10.0` asset, so they compile, but must be excluded from
  the macOS TFM (calls guarded with `#if !MACOS`) or the app fails at runtime.
- The Android emulator needs Hypervisor.framework; if the sandbox blocks it, Android stops at a
  successful `net10.0-android` build and the emulator run is reported as blocked.
- `ISynergy.Framework.UI.Maui` gains a TFM that ships in the NuGet package; it is additive and
  does not change any existing TFM or public API.

## Reused types / existing assets (rule 12)

- `Sample.Maui` stays the single project — no duplicated head; the macOS platform folder follows
  the existing `Platforms/{Android,iOS,MacCatalyst,Windows}` convention.
- `MauiProgram.CreateMauiApp`, `App`, `Context`, `CommonServices`, `SettingsService` are reused
  as-is; only the hosting call is branched.
- The macOS backend's `MacOSMauiApplication` replaces `MauiUIApplicationDelegate` as the app
  delegate; `Resources/Images/icon.png` is reused as the macOS app icon source.
