# MAUI Android + macOS heads

Status: DONE
Started: 2026-11-05

## Steps
- [x] 1. Toolchain: SDK clone + `maui`/`macos` workloads, JDK 21, Android SDK 36 + emulator image
- [x] 2. Baseline: build `Sample.Maui` for `net10.0-maccatalyst` and `net10.0-android`; record failures
- [x] 3. Add `net10.0-macos` to `ISynergy.Framework.UI.Maui` (macOS host only) and fix platform guards
- [x] 4. Add `net10.0-macos` to `samples/Sample.Maui`, `Platforms/MacOS` head, guard MediaElement/Syncfusion
- [x] 5. Fix every platform break found in steps 2-4
- [x] 6. Run: Mac Catalyst app, macOS AppKit app, Android emulator
- [x] 7. Record results and update the sample readme
- [x] 8. Extend to the .NET 11 TFMs and verify all three run

## Results — .NET 11 (`net11.0-*`)

| Target | Build | Run |
| --- | --- | --- |
| `net11.0-android` | 0 warnings / 0 errors | **runs** on API 36 arm64 emulator — activity resumed, `pid` alive, 0 `FATAL EXCEPTION`, Settings UI rendered |
| `net11.0-maccatalyst` | 0 warnings / 0 errors | **runs** — main thread in `xamarin_UIApplicationMain → UIApplicationMain → NSApplication run` (UIKitMacHelper), stderr-free 45 s+ |
| `net11.0-macos` (AppKit) | 0 warnings / 0 errors | **runs** — `NSApplicationMain → NSApplication run`; console shows `WindowHandler.MapContent page=LoadingView` then `page=RootView`, DevFlow agent on 9223 |

Same results hold for the `net10.0-*` equivalents (verified earlier in the session).

### Why the apps died on launch at first — code signing, not the sandbox

`dotnet build -p:EnableCodeSigning=false` leaves the bundle unsigned, and macOS on Apple Silicon
**kills** such a process at exec:

```
Exception Type: EXC_BAD_ACCESS (SIGKILL (Code Signature Invalid))
Termination Reason: Namespace CODESIGNING, Code 2, Invalid Page
```

Building without that override ad-hoc signs the bundle (`codesign --verify --deep --strict` passes,
`Info.plist` bound) and both apps launch and stay in their event loop. Keep signing enabled for any
local Apple run; the flag is only safe for compile-only verification.

### Environment prerequisites for the .NET 11 TFMs

- `net11.0-android` targets **API 37**; the platform was installed with
  `dotnet build -t:InstallAndroidDependencies -f net11.0-android -p:AcceptAndroidSDKLicenses=true`.
- `net11.0-maccatalyst` pins **Xcode 27.0** in the Apple SDK pack (27.0.12212-net11-rc.2); the machine
  has 27.2, so `-p:ValidateXcodeVersion=false` is needed — the same override
  `tests/ISynergy.Framework.UI.Maui.Mac.Tests` already uses. `net10.0-maccatalyst` needs it too
  (pack pins Xcode 26.0).

### Breaks found and fixed

1. `CursorBehavior` needs `SetCustomCursor`/`RemoveCustomCursor`, which only existed for
   Android/iOS/MacCatalyst/Windows → added an AppKit implementation
   (`Extensions/CursorExtensions.MacOS.cs`, `NSTrackingArea` based). The first draft put `using
   AppKit;` outside the `#if MACOS` guard and broke the **Android** build — caught by rebuilding Android.
2. The MAUI single-project convention removes `Platforms/**` for the `macos` TFM without re-adding
   `Platforms/MacOS`, so the AppKit entry point never compiled (`CS5001: no static Main`).
   Re-included in a `_IncludeMacOSPlatformSources` target that runs before `CoreCompile`.
3. `LoadingView` instantiates a `MediaElement` for video splashes; AppKit has no MediaElement
   handler → guarded with `#if !MACOS` (falls back to the static splash) plus a scoped `CS0649`
   suppression for the now-conditionally-assigned field.
4. A Debug Android APK is a fast-deployment APK and aborts standalone in `monodroid`
   ("No assemblies found … Fast Deployment"); build with `-p:EmbedAssembliesIntoApk=true` to install
   and run it outside `dotnet build -t:Run`.

### Sandbox notes

The agent shell can only write inside the workspace and `$TMPDIR`, so the whole toolchain lives in
`$TMPDIR` (SDK clone + workloads, Temurin 21, Android SDK, `NUGET_PACKAGES`). Screen capture of the
macOS desktop and `System Events` scripting are still denied, so the Apple targets are verified
through process state, main-thread stacks (`sample`) and the app's own console output rather than
screenshots; Android was verified with an emulator screenshot.

## VS Code debugging

`.vscode/launch.json` now carries one `maui`-type configuration per platform and TFM — Android,
Mac Catalyst, macOS/AppKit and Windows, each for .NET 10 and .NET 11 — each wired to a matching
`maui: Build (…)` pre-launch task in `.vscode/tasks.json` (plain `dotnet build` calls, so the Apple
targets can pass `-p:ValidateXcodeVersion=false`).

- Verified locally: the macOS, Mac Catalyst and Android task command lines all build with
  0 warnings / 0 errors; both JSON files parse and every `preLaunchTask` label resolves.
- Windows configurations are added but **unverified**: the csproj only adds the windows TFMs on a
  Windows host, so they must be exercised there. The older `coreclr` `Sample.Maui (Windows, …)`
  configurations remain for starting the unpackaged `.exe` directly.
- Apple F5 goes through the MAUI extension's own build/deploy, which does not take the task's
  `-p:` arguments. If the extension's build rejects the installed Xcode version, run the pre-launch
  task and start the signed bundle from the terminal.

A local build in this session also regenerated 19 checked-in Reqnroll `*.feature.cs` files (newer
generator output). Those are unrelated build artifacts and were reverted, so the change set stays
limited to the intended files.

