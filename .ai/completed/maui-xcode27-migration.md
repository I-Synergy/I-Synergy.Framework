# Sample.Maui — Xcode 27 / UIScene migration

**Status:** DONE (compile-validated; packaging and runtime not testable on this host)

## What changed

| File | Change |
|---|---|
| `samples/Sample.Maui/Platforms/iOS/SceneDelegate.cs` | New. `[Register("SceneDelegate")] : MauiUISceneDelegate`, namespace `Sample`. |
| `samples/Sample.Maui/Platforms/MacCatalyst/SceneDelegate.cs` | New. Same delegate for the Catalyst head. |
| `samples/Sample.Maui/Platforms/iOS/Info.plist` | `UIApplicationSceneManifest` added to the root dict — single scene, `__MAUI_DEFAULT_SCENE_CONFIGURATION__`, delegate `SceneDelegate`, no `UISceneStoryboardFile`. |
| `samples/Sample.Maui/Platforms/MacCatalyst/Info.plist` | Same manifest. |
| `samples/Sample.Maui/Platforms/iOS/Resources/PrivacyInfo.xcprivacy` | Uncommented the `NSPrivacyAccessedAPICategoryUserDefaults` / `CA92.1` entry (the app uses `Preferences.Default` in `Services/SettingsService.cs`). |
| `samples/Sample.Maui/Sample.Maui.csproj` | Mac Catalyst `SupportedOSPlatformVersion` 15.0 → 17.0 (approved). iOS stays 15.0. |
| `global.json` | Added `sdk.workloadVersion: 10.0.401.1` (approved). |

Not changed: MAUI packages (`Microsoft.Maui.Controls` 10.0.120 already above the 10.0.110 floor), iOS minimum, Android/Windows heads, splash/icon config, `AppDelegate`s (still `MauiUIApplicationDelegate` + `CreateMauiApp` only).

## Decisions taken (user-approved)

- Raise Catalyst minimum to 17.0 / macOS 14, keeping the Catalyst head. Drops macOS 12–13 support.
- Pin workload set `10.0.401.1` in `global.json` (published on NuGet; documented pairing for Apple 27.0.10722 + Xcode 27.0).
- Do not add a macOS CI job; Apple build/device validation stays a manual Mac step.

## Evidence

- Inventory: no custom lifecycle code anywhere in the app (no `ConfigureLifecycleEvents`, `OpenUrl`, `ContinueUserActivity`, `ShortcutItem`, `Window` access in the Apple heads); no existing scene delegate or manifest; one `Info.plist` per head (no configuration-specific variants).
- `MauiUISceneDelegate` present in the resolved MAUI 10.0.120 `Microsoft.Maui.dll`.
- `dotnet workload list` → workload version `10.0.401.1`, `ios`/`maccatalyst` manifests `27.0.10722`.
- Effective properties: iOS `SupportedOSPlatformVersion=15.0`, Catalyst `17.0`, both `TargetPlatformVersion=27.0`, `MauiVersion=10.0.110`.
- `dotnet build -f net10.0-ios -t:Compile` → **0 warnings, 0 errors**; same for `net10.0-maccatalyst`.
- Compiled `Sample.Maui.dll` for both heads contains the `SceneDelegate` type; `Compile` items show each head picking up its own platform folder copy only (a duplicate would have failed with CS0101).
- Plist validation script (plistlib parse + manifest/delegate/privacy assertions) → PASS for both `Info.plist`s, `PrivacyInfo.xcprivacy`, `Entitlements.plist`.
- Lifecycle audit searches (step 5 of the skill) → no hits in the Apple heads.

## Not validated / open items

- `scripts/audit-lifecycle.mjs` was **not run** — no Node.js on this host. The equivalent manual searches were run instead.
- App-bundle `Info.plist` check was **not possible**: the Apple packaging/manifest targets need Xcode, and the leftover `bin/Debug/net10.0-ios/.../Sample.Maui.app` is a stale zero-byte artifact from an earlier macOS build. Verify on a Mac with `plutil -p <App>.app/Info.plist`.
- No simulator/device run: startup past splash, foreground/background, warm/cold links, auth return and quick actions are untested. Xcode 27.0 requires macOS 26.6+ on the build host.
- Full iOS device build (`ios-arm64`) and Release Catalyst builds untested.
- Unrelated: 16 `SKILL.md` files under `skills/` and `.ai/skills/` show as modified (frontmatter format conversion). Not produced by this session — left untouched.
