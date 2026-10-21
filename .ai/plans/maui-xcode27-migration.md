# Prepare Sample.Maui for Xcode 27 (UIScene lifecycle)

Status: DONE — see `.ai/completed/maui-xcode27-migration.md`

## Inventory (verified locally)

| Item | Value |
|---|---|
| Project | `samples/Sample.Maui/Sample.Maui.csproj` |
| Apple TFMs | `net10.0-ios`, `net10.0-maccatalyst` (unconditional) + `net10.0-android`, `net10.0-windows10.0.26100.0` |
| MAUI | `Microsoft.Maui.Controls` 10.0.120 via `Directory.Packages.props` (floor 10.0.110 met — preserved) |
| iOS `SupportedOSPlatformVersion` | 15.0 (= SDK 27 minimum, unchanged) |
| Catalyst `SupportedOSPlatformVersion` | 15.0 → **17.0** (approved) |
| global.json SDK | 10.0.402, `workloadVersion` **10.0.401.1** (approved) |
| Workload set | Installed 10.0.401.1; `ios`/`maccatalyst` 27.0.10722 |
| Xcode | Not available (Windows host) |
| Custom lifecycle code | None — fast path applied |
| CI | `azure-pipelines.yml`, `windows-2025-vs2026`; no macOS Apple build step (user chose not to add one) |

## Applied changes

1. `Platforms/iOS/SceneDelegate.cs` and `Platforms/MacCatalyst/SceneDelegate.cs` — `[Register("SceneDelegate")] : MauiUISceneDelegate`.
2. `UIApplicationSceneManifest` merged into both `Info.plist` files.
3. UserDefaults / CA92.1 entry enabled in `PrivacyInfo.xcprivacy`.
4. Catalyst minimum raised to 17.0.
5. `global.json` workload set pin.

## Validation results

- plist/XML validation: PASS (both plists, privacy manifest, entitlements).
- Effective properties: iOS 15.0 / Catalyst 17.0, both `TargetPlatformVersion=27.0`.
- `dotnet build -f net10.0-ios -t:Compile` and `-f net10.0-maccatalyst -t:Compile`: 0 warnings, 0 errors.
- `SceneDelegate` present in both compiled assemblies.
- Blocked/untested: `audit-lifecycle.mjs` (no Node.js), built-bundle manifest check (needs Xcode), simulator/device runs, iOS device + Release Catalyst builds.

## Notes

- Host is Windows with no Xcode; Apple packaging cannot be validated here.
- Leftover `Sample.Maui.app` in `bin/` is a stale zero-byte artifact from an earlier macOS build.
