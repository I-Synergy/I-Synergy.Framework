# WinUI 3 Build & Run Workflow

Prerequisites, the `BuildAndRun.ps1` loop, and the build-error table. Load this when scaffolding a new WinUI 3 app or diagnosing a build failure.

## Prerequisites

Check these are present before building; if any are missing, tell the user and reference `winui-setup` guidance:

| Tool | Minimum | Check |
|------|---------|-------|
| Developer Mode | enabled | `(Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock').AllowDevelopmentWithoutDevLicense -eq 1` |
| .NET SDK | 8.0 (10.0 recommended) | `dotnet --list-sdks` |
| WinApp CLI | 0.3 | `winapp --version` |
| WinUI 3 templates | any | `dotnet new list winui` |

**Install missing tools:**
```powershell
# .NET 10 (only if no SDK >= 8.0 found)
winget install --id Microsoft.DotNet.SDK.10 --exact --silent --accept-package-agreements --accept-source-agreements

# WinApp CLI — install then always upgrade to latest
winget install --id Microsoft.WinAppCLI --exact --silent --accept-package-agreements --accept-source-agreements
winget upgrade  --id Microsoft.WinAppCLI --exact --silent --accept-package-agreements --accept-source-agreements

# Refresh PATH after winget installs
$env:Path = [Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [Environment]::GetEnvironmentVariable('Path','User')

# WinUI 3 templates — always reinstall to get latest
dotnet new install Microsoft.WindowsAppSDK.WinUI.CSharp.Templates

# Developer Mode — ASK USER first (requires UAC elevation)
Start-Process powershell -Verb RunAs -ArgumentList @(
  '-NoProfile','-Command',
  "Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock' -Name AllowDevelopmentWithoutDevLicense -Type DWord -Value 1"
) -Wait
```

---

## Build & Run Workflow

### Scaffold a New App

```powershell
dotnet new winui-mvvm -n <AppName>
cd <AppName>
```

Creates: MVVM project with CommunityToolkit.Mvvm, TitleBar, MicaBackdrop, Frame navigation. Do NOT `mkdir` first.

### Build & Run

Use `BuildAndRun.ps1` (from [win-dev-skills](https://github.com/microsoft/win-dev-skills/raw/main/plugins/winui/skills/winui-dev-workflow/BuildAndRun.ps1)):

```powershell
.\BuildAndRun.ps1                          # auto-detect, build, run (use async invocation)
.\BuildAndRun.ps1 -SkipRun                 # build only
.\BuildAndRun.ps1 /p:Configuration=Release # release build
.\BuildAndRun.ps1 -Detach                  # run detached (sync-safe)
```

**Always invoke async** — the script stays attached to the running app; sync mode blocks for the app's lifetime.

What it does: checks Developer Mode → finds `.csproj` → detects x64/ARM64 → builds with MSBuild (falls back to `dotnet build`) → `winapp run --debug-output`.

### Common Build Errors

| Error | Fix |
|-------|-----|
| `Developer Mode not enabled` | Enable in Settings → System → For developers |
| `CS0234/CS0246` missing type | Add `using` or `dotnet add package <Name>` |
| `NETSDK1136` platform required | `BuildAndRun.ps1` handles this automatically |
| `XLS0414` XAML type not found | Add `xmlns` declaration |
| `XDG0062` binding path missing | Verify ViewModel property exists |
| Blank window after launch | `x:Bind` defaults OneTime → add `Mode=OneWay` |
| App silently exits | Use `winapp run`, never run `.exe` directly |
| `0x80073CF6` package install failed | Run `winapp init`, check manifest publisher matches cert |
| `0x8007000B` bad image format | Wrong platform — use x64 or ARM64, not AnyCPU |
| XAML compiler silent crash | Remove any `PresentationCore.dll` / `System.Windows` references |

### Critical Rules

- ❌ NEVER run the packaged `.exe` directly — always use `winapp run` or `BuildAndRun.ps1`
- ❌ NEVER add `<WindowsPackageType>None</WindowsPackageType>`
- ❌ NEVER delete `Package.appxmanifest`
- ❌ NEVER use `AnyCPU` — always x64 or ARM64
- ❌ NEVER specify `--version` when adding packages — omit it to get latest stable
