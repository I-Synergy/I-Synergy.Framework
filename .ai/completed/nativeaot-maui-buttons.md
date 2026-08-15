# NativeAOT MAUI Buttons Investigation

Status: IN PROGRESS
Started: 2026-08-14

## Context
Continuing an existing (uncommitted) investigation left in
src/ISynergy.Framework.UI.Maui/Controls/BusyIndicator.cs — diagnostic logging to
~/Documents/iquran-diag.log tracking IsVisible/InputTransparent changes, from debugging
a button-click bug in an external app ("iquran") built on this framework.

Goal: determine whether Sample.Maui (this repo's bundled sample) reproduces "buttons don't
respond" when published with NativeAOT (PublishAot=true), on Mac Catalyst (native window on
this dev Mac, driven via UI scripting since no Android/simulator automation is set up).

## Steps
- [x] Baseline: build & run Sample.Maui maccatalyst normally (JIT), confirm buttons work
- [x] Build Sample.Maui maccatalyst with PublishAot=true, capture build warnings/errors
- [x] Run the AOT-published app, drive it via osascript/System Events, click buttons, observe
- [x] Check Console.app / stdout for exceptions (trimming, missing DAM, event wiring)
- [ ] Correlate with BusyIndicator IsVisible/InputTransparent theory (overlay eating touches) — NOT the cause, see findings
- [x] Report root cause; if fix identified, delegate implementation to maui-specialist/developer agent — reported, fix not yet attempted

**Status:** DONE (investigation) — awaiting user decision on whether to pursue a fix

## Findings
1. **Real NativeAOT (`PublishAot=true`) is not supported at all** for maccatalyst-arm64/ios/android
   in this SDK (10.0.400) — fails immediately with `NETSDK1203: Ahead-of-time compilation is not
   supported for the target runtime identifier`. NativeAOT (ILCompiler) only targets CoreCLR RIDs
   (win/linux/osx-x64/arm64); Apple mobile + Android use the Mono runtime, which has no NativeAOT
   path in this SDK. So "NativeAOT" isn't literally reproducible on mobile targets — the closest
   real equivalent is a trimmed Release build (Mono linker), which is what actual published/device
   builds (and presumably "iquran") use.
2. **Reproduced a real regression in Release/trimmed Mac Catalyst builds.** Built and ran
   `samples/Sample.Maui` both as Debug (JIT, untrimmed) and Release (`-c Release`, Mono
   linker/trimming enabled — confirmed via "Optimizing assemblies for size" in build output) on
   Mac Catalyst, driven with `cliclick`/`osascript` UI automation:
   - **Debug**: Shell flyout hamburger button renders as the native "≡" icon and opens the flyout
     menu correctly on click. Confirmed via screenshot.
   - **Release (trimmed)**: same hamburger button renders as a **blank box (icon missing)** and
     **does not respond to clicks at all** — no flyout opens. Same code, same click coordinates,
     only the build configuration differs.
   - No exceptions/crashes appear in stdout in either build — the failure is silent.
3. This hamburger button is the **stock `Microsoft.Maui.Controls.Shell` flyout toggle**
   (`ShellView.xaml` just sets `FlyoutBehavior="Flyout"`) — `ISynergy.Framework.UI.Maui` has no
   custom handler/mapper touching Shell's flyout icon (`grep` for `ShellHandler`/`Mapper[`/
   `FlyoutIconImageSource` found nothing relevant). So this looks like a **MAUI SDK / Mono-linker
   trimming issue surfacing through this framework's sample**, not framework-authored code —
   though not yet confirmed against a bare (non-ISynergy) MAUI Shell app.
4. The BusyIndicator `IsVisible`/`InputTransparent` diagnostic-logging theory (leftover uncommitted
   code from the "iquran" investigation) is very unlikely to be the actual cause: `LoadingView`
   (the only place a bare `BusyIndicator` sits as an overlay) is only the splash screen, swapped out
   before the main Shell/pages load, and its own button (`_signInButton`) is rendered on top of it
   in z-order. The broken button found here (Shell flyout toggle) has no relationship to
   `BusyIndicator` at all.

## Recommended next steps (not yet done)
- Confirm whether this is MAUI-SDK-wide (create a minimal non-ISynergy MAUI Shell app, trimmed
  Release, macCatalyst) vs. something specific to how this framework wires up `Application.cs` /
  DI / Shell construction.
- If MAUI-SDK-wide: workaround is likely disabling/relaxing trimming for Mac Catalyst/iOS Release
  (`TrimMode`, `PublishTrimmed=false`, or per-assembly trimmer roots) — a real code/config change,
  should go through the `maui-specialist` skill or `developer`/`architect` agent, not done inline.
- Test the same Debug-vs-Release comparison on iOS (simulator) and Android (device, since no
  emulator here) to see whether the regression is Mac-Catalyst-specific or affects all trimmed
  Apple/Mono targets — that determines how broadly this affects "iquran" and other consumer apps.

## Notes
- No Android SDK/emulator on this machine — Mac Catalyst chosen as most directly testable
  (native macOS window, no simulator plumbing needed).
- ~/Documents/iquran-diag.log does not exist yet on this machine (never run here).
- Building/publishing this multi-targeted MAUI project for a single TFM on this machine is flaky:
  `dotnet publish -f <tfm>` still restores ALL `TargetFrameworks` (including `net10.0-android`,
  whose `mobile-librarybuilder` workload is NOT installed here — confirmed via `dotnet workload
  list`). Worked around by temporarily removing `net10.0-android` from `TargetFrameworks` in
  `Sample.Maui.csproj` and `ISynergy.Framework.UI.Maui.csproj`, building, then reverting (now
  reverted — `git diff` on those two files is clean). Installing the `android` workload requires
  sudo, which was not run.
- UI-automation gotcha for future sessions: `cliclick`/screen coordinates from a Retina screenshot
  must be halved (pixels → points) before use — the standard window-control buttons (16x16 @
  points) confirm the scale. Also, taking a screenshot in a separate Bash call after activating the
  app frequently raced back to Terminal being frontmost — combine
  `osascript activate` + `cliclick` + `screencapture` in one Bash invocation.
