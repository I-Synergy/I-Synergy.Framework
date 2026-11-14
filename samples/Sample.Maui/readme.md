# I-Synergy Framework Sample for Maui

> **Known build issue (environmental).** Building this sample requires the MAUI workload, and
> under .NET SDK 10.0.401 the `Microsoft.Maui.Resizetizer` 10.0.110 `GenerateSplashAssets` task
> fails with `MSB4018` / `System.MissingMethodException: HarfBuzzSharp.Font.SetVariations(...)` — a
> Resizetizer ↔ SkiaSharp/HarfBuzzSharp toolchain mismatch, not a defect in the sample's or the
> framework's source. It breaks a solution-level `dotnet build`/`dotnet test`; build the other
> projects or the `src/` tree directly until the workload/tooling is aligned. See
> `docs/FRAMEWORK_REVIEW_2026-10-07.md` §2.

## Target frameworks

The sample is **.NET 11 only**. On .NET 10 Android and Mac Catalyst ran on Mono, which cannot be
debugged with the CoreCLR debugger; .NET 11 uses CoreCLR on Android and Apple. The framework
package (`ISynergy.Framework.UI.Maui`) still multi-targets .NET 10 for its consumers.

| Target | TFM | Notes |
| --- | --- | --- |
| Android | `net11.0-android` | API 24 minimum |
| iOS | `net11.0-ios` | iOS 15 minimum |
| Mac Catalyst | `net11.0-maccatalyst` | macOS 17 minimum; the store-distributed macOS target |
| Windows | `net11.0-windows10.0.26100.0` | **Windows hosts only** |

```bash
# Mac Catalyst
dotnet build -f net11.0-maccatalyst -c Debug

# Android (then the usual deploy/run)
dotnet build -f net11.0-android -c Debug -p:EmbedAssembliesIntoApk=true
```

### Debugging from VS Code

`.vscode/launch.json` ships one configuration per platform (`Sample.Maui (Android|Mac Catalyst|Windows), .NET 11`)
for the **.NET MAUI** extension (`ms-dotnettools.dotnet-maui`), each wired to its own
`maui: Build (…)` pre-launch task in `.vscode/tasks.json`.

One-time setup per workspace: the extension takes its startup project from C# Dev Kit, and this
workspace has none selected, so F5 stops with
`Debugging canceled: startup project not found.` Set `samples/Sample.Maui` as the startup project
once — Solution Explorer → right-click **Sample.Maui** → *Set as Startup Project*, or the
**Select Startup Project** command — then pick the debug target when the extension asks.

- **Android** — the MAUI extension starts/deploys to the selected emulator or device. The machine
  needs an Android SDK and a JDK (`ANDROID_HOME` / `JAVA_HOME`); neither is installed by the MAUI
  workload.
- **Mac Catalyst** — deploys and debugs through the extension; the pre-launch task passes
  `-p:ValidateXcodeVersion=false` because the Apple packs pin an older Xcode than this machine runs.
  Debug builds use `Platforms/MacCatalyst/Entitlements.Debug.plist` (no App Sandbox,
  `get-task-allow`) so a debugger or terminal that is itself sandboxed can start the bundle
  directly; Release keeps the App Sandbox in `Platforms/MacCatalyst/Entitlements.plist`.

Troubleshooting `Debugging canceled: startup project not found.` after the MAUI workloads were
installed or upgraded: the extension derives its project list (and therefore the startup project)
from C# Dev Kit's in-memory project model, which is only rebuilt on evaluation. If the window was
open while the workloads were missing, `Sample.Maui` carries none of the `Maui` /
`AndroidApplication` / `MacCatalystApplication` capabilities yet, the list of
application projects is empty, and no picker appears. Reload the window (`Developer: Reload Window`)
or restart VS Code, then select the startup project. Verify the capabilities with:

```bash
dotnet msbuild samples/Sample.Maui/Sample.Maui.csproj -p:TargetFramework=net11.0-maccatalyst \
  -getItem:ProjectCapability | grep -E "Maui$|Application"
```

Troubleshooting `Debugging canceled: The current dotnet SDK is not supported`: the MAUI debugger
runs its CLI on a .NET host acquired by the .NET Install Tool. That host is runtime-only (no
`dotnet --info` SDK section, no `Base Path`), and `global.json` pins SDK `11.0.100-rc.3`, so the
CLI cannot evaluate the project and the extension reports the SDK as unsupported. Point the Install
Tool at the host that owns the pinned SDK — a machine-specific path, so keep it in **user** settings
and restart VS Code:

```json
"dotnetAcquisitionExtension.existingDotnetPath": [
  { "extensionId": "ms-dotnettools.dotnet-maui", "path": "/path/to/dotnet" }
]
```

The WPF/WinUI configurations do not show this error because they use the plain `coreclr` debugger:
they build with a task and start the produced `.exe` directly, without the MAUI CLI, project
system or target-framework resolution.

When the installed Xcode is newer than the Apple SDK pack expects, pass `-p:ValidateXcodeVersion=false`
(the Mac Catalyst test host does the same): the .NET 11 packs pin Xcode 27.0, so a newer Xcode needs
the override.

**Do not disable code signing for a run.** `-p:EnableCodeSigning=false` produces an unsigned bundle
that macOS kills at exec on Apple Silicon with `SIGKILL (Code Signature Invalid)`. Building without it
produces an ad-hoc signed bundle that launches locally; verify with
`codesign --verify --deep --strict Sample.app`.

`net11.0-android` targets **API 37**; if the platform is missing, install it with
`dotnet build -t:InstallAndroidDependencies -f net11.0-android -p:AcceptAndroidSDKLicenses=true`.
A Debug Android APK is a fast-deployment APK and will not start when installed on its own — build with
`-p:EmbedAssembliesIntoApk=true` to run it outside `dotnet build -t:Run`.

