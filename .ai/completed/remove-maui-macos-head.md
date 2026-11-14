# Remove the native macOS (AppKit) head from MAUI

Status: DONE
Started: 2026-11-14
Plan: `.ai/plans/remove-maui-macos-head.md`

## Steps

- [x] Remove the macOS TFMs and AppKit package references from `ISynergy.Framework.UI.Maui`
- [x] Remove the `#if MACOS` code from `ISynergy.Framework.UI.Maui`
- [x] Remove the macOS head from `samples/Sample.Maui`
- [x] Remove the `Microsoft.Maui.Platforms.MacOS*` package pins
- [x] Remove the macOS entries from `.vscode`
- [x] Update the docs, screenshots and test-project comment
- [x] CHANGELOG, build verification, session handoff

## Notes

- Effective TFMs after the change: `ISynergy.Framework.UI.Maui` evaluates
  `net10.0/11.0-android`, `-ios`, `-maccatalyst` plus `-windows10.0.26100.0` on Windows hosts (8);
  `Sample.Maui` evaluates `net11.0-android`, `-ios`, `-maccatalyst` plus
  `net11.0-windows10.0.26100.0` on Windows (4).
- Verified with `dotnet build samples/Sample.Maui -f net11.0-windows10.0.26100.0 -c Debug`:
  0 warnings, 0 errors (compiles the framework's Windows TFM, `MauiProgram.cs` and the XAML).
- The local `packages/I-Synergy.Framework.UI.Maui.0.0.0.nupkg` carries the same eight TFMs and no
  macOS asset.
- `ISynergy.Framework.UI.Maui.Mac.Tests` was left alone: it is Mac Catalyst, not AppKit.
- Historical records (`.ai/plans/maui-android-macos-heads.md`, `.ai/completed/`, `CHANGELOG` older
  entries, `audits/`) still describe the removed head; they are history, not live documentation.
