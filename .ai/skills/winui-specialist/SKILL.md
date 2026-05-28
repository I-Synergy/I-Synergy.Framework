---
name: winui-specialist
description: WinUI 3 / Windows App SDK development specialist. Use when building WinUI 3 desktop apps, designing Fluent UI layouts, packaging MSIX, migrating from WPF, writing UI automation tests, or fixing WinUI 3 build errors. Covers the full inner loop: scaffold → design → build → run → test → package.
---

# WinUI 3 Specialist Skill

Specialized agent for WinUI 3, Windows App SDK, XAML, CommunityToolkit.Mvvm, and MSIX packaging.

Based on the [microsoft/win-dev-skills](https://github.com/microsoft/win-dev-skills) playbook.

## Role

You are a WinUI 3 Desktop App Specialist. You build native Windows apps with WinUI 3 and the Windows App SDK following Fluent Design principles — from project scaffold through signed MSIX distribution.

## Expertise Areas

- WinUI 3 and Windows App SDK
- XAML layout, controls, and theming (Light/Dark/HighContrast)
- Fluent Design System (Mica, Acrylic, motion, iconography)
- MVVM with CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`)
- `x:Bind` compiled bindings, `DataTemplate`, `VisualStateManager`
- MSIX packaging, code signing, CI/CD with GitHub Actions
- UI automation testing with `winapp ui`
- WPF → WinUI 3 migration
- BuildAndRun.ps1 build workflow and `winapp run`
- Accessibility (`AutomationProperties`, keyboard navigation, screen readers)

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

---

## UI Design and XAML Correctness

### App Type → Anchor Control

| App Type | Anchor Control | Reference App |
|----------|---------------|---------------|
| Settings / config tool | `NavigationView` Left + `SettingsCard` | Windows Settings |
| Document / session editor | `TabView` + full-width content | Windows Terminal |
| Hierarchical browser | `TreeView` + `ListView` + `BreadcrumbBar` | File Explorer |
| Developer tool / dashboard | `NavigationView` + card layout | Dev Home |
| Single-purpose utility | Mode switcher + compact grid | Calculator |

### Navigation
- 2–7 sections → `NavigationView`
- Document tabs → `TabView`
- 2–3 modes → `SelectorBar`
- Breadcrumb trail → `BreadcrumbBar`

### Data Display
- Vertical list → `ListView`
- Grid/tiles → `GridView` or `ItemsRepeater` + `UniformGridLayout`
- Hierarchy → `TreeView`
- Master-detail → `ListView` + detail `Grid`

### Input
- Text → `TextBox` | Number → `NumberBox` | Search → `AutoSuggestBox`
- Boolean → `ToggleSwitch` | One-of-2/3 → `RadioButtons` | One-of-4+ → `ComboBox`

### Feedback
- Blocking decision → `ContentDialog`
- Contextual action → `Flyout` / `MenuFlyout`
- Inline status → `InfoBar`

### Theming Rules

```xml
<!-- CORRECT: StaticResource redirect in theme dictionary -->
<StaticResource x:Key="MyBrush" ResourceKey="ControlFillColorDefaultBrush" />

<!-- WRONG: inline SolidColorBrush allocates new object per theme -->
<SolidColorBrush x:Key="MyBrush" Color="{StaticResource ControlFillColorDefault}" />
```

- `{ThemeResource BrushName}` at usage sites — updates on theme change
- `ResourceKey` must end in `Brush` — target the `SolidColorBrush`, not the `Color`
- Always define all three variants: `Light`, `Dark`, `HighContrast` — never `Default`
- No hardcoded hex colors or `Color="Blue"` anywhere in production XAML

### High Contrast

Only 8 system brushes in HC dictionaries: `SystemColorWindowColorBrush`, `SystemColorWindowTextColorBrush`, `SystemColorHighlightColorBrush`, `SystemColorHighlightTextColorBrush`, `SystemColorButtonFaceColorBrush`, `SystemColorButtonTextColorBrush`, `SystemColorHotlightColorBrush`, `SystemColorGrayTextColorBrush`.

Set `HighContrastAdjustment = None` at app level.

### Typography Styles (use styles, never raw FontSize)

| Style | Size | Weight | Use For |
|-------|------|--------|---------|
| `CaptionTextBlockStyle` | 12px | Regular | Labels, timestamps |
| `BodyTextBlockStyle` | 14px | Regular | Body (default — don't set explicitly) |
| `BodyStrongTextBlockStyle` | 14px | Semibold | Emphasized body |
| `SubtitleTextBlockStyle` | 20px | Semibold | Section headers |
| `TitleTextBlockStyle` | 28px | Semibold | Page titles |
| `TitleLargeTextBlockStyle` | 40px | Semibold | Large feature titles |
| `DisplayTextBlockStyle` | 68px | Semibold | Hero text |

Use `SemiBold`, never `Bold`. Minimum 12px.

### Spacing Grid

Margins, padding, and sizes must be multiples of 4: **4, 8, 12, 16, 24, 32, 48**.

- `ControlCornerRadius` (4px) for controls — never hardcode
- `OverlayCornerRadius` (8px) for overlays — never hardcode
- `RowSpacing`/`ColumnSpacing` instead of spacer elements
- No negative margins

### Data Binding

```xml
<!-- CORRECT: TwoWay with PropertyChanged so UIA set-value commits immediately -->
<TextBox Text="{x:Bind ViewModel.Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

- `{x:Bind}` over `{Binding}`, always explicit `Mode=OneWay`/`TwoWay`
- `x:DataType` on every `DataTemplate`
- Commands over Click/Tapped handlers (MVVM)
- `VisualStateManager` for visual property changes, not code-behind
- No `IValueConverter` — prefer `x:Bind` with static functions

**Bool negation and Visibility helpers** (define as static in code-behind):
```csharp
public static Visibility BoolToVisibility(bool v) => v ? Visibility.Visible : Visibility.Collapsed;
public static Visibility InvertBoolToVisibility(bool v) => v ? Visibility.Collapsed : Visibility.Visible;
public static bool IsNotBusy(bool isLoading) => !isLoading;
```
```xml
Visibility="{x:Bind local:MainPage.BoolToVisibility(ViewModel.IsLoading), Mode=OneWay}"
```
❌ NEVER use `Converter={x:Null}` — crashes at runtime.

### Attached Properties in Code-Behind

```csharp
// ❌ WRONG — object initializer doesn't work for attached properties
var btn = new Button { AutomationProperties = { AutomationId = "BtnSave" } };

// ✅ CORRECT
var btn = new Button { Content = "Save" };
AutomationProperties.SetAutomationId(btn, "BtnSave");
AutomationProperties.SetName(btn, "Save button");
Grid.SetRow(btn, 1);
```

### Layout Anti-Patterns

| ❌ Don't | ✅ Do Instead |
|----------|--------------|
| Centered floating card on background | Content fills window with padding |
| Custom pill/segment tab switcher | `NavigationView` Top or `SelectorBar` |
| Equal-width 50/50 split | Fixed sidebar (300–360px) + flexible main |
| Hardcoded colors (`#FF0000`) | `{ThemeResource}` brushes |
| `ScrollViewer` around `ListView` | ListView has built-in scrolling |

### Accessibility (required on every control)

```csharp
using Microsoft.UI.Xaml.Automation;
AutomationProperties.SetAutomationId(btn, "BtnSave");
AutomationProperties.SetName(btn, "Save document");
```

- `AutomationProperties.AutomationId` on **every** interactive control
- `AutomationProperties.Name` on icon-only controls
- Semantic elements (`Button`, `HyperlinkButton`) — never clickable `Border`/`TextBlock`
- No information conveyed by color alone

---

## MVVM Code Review Checklist

### MVVM Compliance
- [ ] ViewModels extend `ObservableObject`, use `[ObservableProperty]` partial properties (not fields)
- [ ] Commands use `[RelayCommand]` — no manual `ICommand` implementations
- [ ] No UI types in ViewModels (`SolidColorBrush`, `Visibility`, `BitmapImage`)
- [ ] No business logic in code-behind — only navigation, dialog coordination, event wiring
- [ ] `async Task` for async methods, `async void` only for event handlers
- [ ] Never replace `ObservableCollection<T>` — use `.Clear()` + re-add

### x:Bind and Data Binding
- [ ] All bindings use `{x:Bind}`, not `{Binding}`
- [ ] `Mode=OneWay` or `TwoWay` set explicitly — `OneTime` default causes blank UI
- [ ] `x:DataType` on every `DataTemplate`
- [ ] No nested nullable paths without `FallbackValue`
- [ ] Command bindings can use `OneTime` (commands don't change)

### Performance
- [ ] Long lists use `ListView`/`GridView` (virtualized), not `StackPanel` + `foreach`
- [ ] `x:Load` for content not always visible
- [ ] Heavy work off UI thread via `Task.Run` or `async/await`
- [ ] No `.Result` / `.Wait()` / `.GetAwaiter().GetResult()` — deadlocks UI thread
- [ ] `using` on all disposable objects

### Security
- [ ] No secrets or API keys in source code
- [ ] No `Process.Start` with unsanitized user input
- [ ] File paths from user input validated before `File.Delete` / `File.WriteAllText`

### Globalization
- [ ] User-facing strings use `x:Uid` (XAML) / `ResourceLoader` (C#)
- [ ] Resources in `Strings/en-us/Resources.resw`
- [ ] Date/number formatting uses `CultureInfo.CurrentCulture`
- [ ] No string concatenation for user-facing messages

---

## MSIX Packaging

```powershell
# Build release
.\BuildAndRun.ps1 /p:Configuration=Release -SkipRun

# Generate dev certificate (one-time, matches manifest Publisher)
winapp cert generate --manifest .

# Trust certificate (one-time, requires admin)
winapp cert install ./devcert.pfx

# Package and sign
winapp package <build-output-dir> --cert ./devcert.pfx

# With timestamp (required for production — without it signature expires with cert)
winapp package <build-output-dir> --cert prod.pfx --timestamp http://timestamp.digicert.com

# Self-contained (bundles Windows App SDK runtime)
winapp package <build-output-dir> --cert ./devcert.pfx --self-contained
```

### Key Packaging Rules
- Publisher must match between certificate and manifest `Identity.Publisher` — use `winapp cert generate --manifest`
- `cert install` requires admin elevation
- Default PFX password is `password` — override with `--password`

### CI/CD (GitHub Actions)

```yaml
name: Build and Package
on: [push]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: microsoft/setup-WinAppCli@v0.1
      - name: Build
        run: dotnet build -c Release -p:Platform=x64
      - name: Package
        run: |
          winapp cert generate --if-exists skip --quiet
          winapp package ./bin/x64/Release/ --cert ./devcert.pfx --quiet
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: msix-package
          path: "*.msix"
```

### Packaging Troubleshooting

| Error | Solution |
|-------|----------|
| Publisher mismatch | `winapp cert generate --manifest` |
| Certificate not trusted | `winapp cert install ./devcert.pfx` (admin) |
| appxmanifest.xml not found | `winapp init` or pass `--manifest <path>` |
| Package install failed | Trust cert first; remove stale: `Get-AppxPackage <name> \| Remove-AppxPackage` |

---

## UI Automation Testing

Use `winapp ui` verbs for automated testing. **Prefer scripted batch tests** over interactive exploration.

### Test Script Template

```powershell
# ui-tests.ps1
param([Parameter(Mandatory)][int]$AppPid)  # Do NOT name $Pid — read-only in PowerShell

$ErrorActionPreference = 'Continue'
$pass = 0; $fail = 0; $results = @()

$windows = winapp ui list-windows -a $AppPid --json 2>$null | ConvertFrom-Json
$hwnd = ($windows | Where-Object { $_.title -ne "PopupHost" } | Select-Object -First 1).hwnd

function Test-UI {
    param([string]$Name, [scriptblock]$Script)
    try {
        $output = & $Script 2>&1
        if ($LASTEXITCODE -eq 0) {
            $script:pass++; $script:results += @{ name = $Name; status = "PASS" }
        } else {
            $script:fail++; $script:results += @{ name = $Name; status = "FAIL"; detail = "$output" }
        }
    } catch {
        $script:fail++; $script:results += @{ name = $Name; status = "FAIL"; detail = "$_" }
    }
}

# Element existence
Test-UI "NavHome exists"     { winapp ui wait-for "NavHome"     -a $AppPid -t 3000 }
Test-UI "NavSettings exists" { winapp ui wait-for "NavSettings" -a $AppPid -t 3000 }

# Navigation
Test-UI "Navigate to Settings" { winapp ui invoke "NavSettings" -a $AppPid }
Test-UI "Settings page loaded" { winapp ui wait-for "TxtUserName" -a $AppPid -t 3000 }

# Value assertions
Test-UI "Set username" { winapp ui set-value "TxtUserName" "TestUser" -a $AppPid }
Test-UI "Click Save"   { winapp ui invoke "BtnSave" -a $AppPid }
Test-UI "Username persisted" { winapp ui wait-for "TxtUserName" -a $AppPid --value "TestUser" -t 2000 }

# Accessibility audit (app controls only — excludes OS chrome)
$allElems  = (winapp ui inspect -a $AppPid --interactive --json 2>$null | ConvertFrom-Json).elements
$appElems  = @($allElems | Where-Object {
    $_.type -match 'Button|TextBox|ComboBox|CheckBox|ToggleSwitch|TabItem|Edit' -and
    $_.name -notmatch 'Minimize|Maximize|Close|System' -and
    $_.className -notmatch 'PickerHost|#32770|CabinetWClass'
})
$missingId = @($appElems | Where-Object { -not $_.automationId })
if ($missingId.Count -eq 0) {
    $pass++; $results += @{ name = "All controls have AutomationId"; status = "PASS" }
} else {
    $fail++
    $names = ($missingId | ForEach-Object { "$($_.type) '$($_.name)'" }) -join ", "
    $results += @{ name = "AutomationId coverage"; status = "FAIL"; detail = "Missing: $names" }
}

winapp ui screenshot -a $AppPid -o "test-screenshot.png" 2>$null
Write-Host "`nPassed: $pass | Failed: $fail"
$results | Where-Object { $_.status -eq "FAIL" } | ForEach-Object {
    Write-Host "  FAIL: $($_.name) — $($_.detail)" -ForegroundColor Red
}
$results | ConvertTo-Json | Out-File "test-results.json"
if ($fail -gt 0) { exit 1 } else { exit 0 }
```

### Assertion Reference

| Control | `wait-for --value` reads | Example |
|---------|--------------------------|---------|
| TextBlock / Label | Name property | `wait-for "LblTitle" --value "Home"` |
| TextBox / NumberBox | ValuePattern | `wait-for "TxtName" --value "John"` |
| ComboBox | Selected item | `wait-for "CmbTheme" --value "Dark"` |
| ToggleSwitch | Toggle state | `wait-for "TglDark" --value "On"` |
| CheckBox | Toggle state | `wait-for "ChkAgree" --value "On"` |

### Key Testing Gotchas

- **`set-value` doesn't commit TextBox bindings** — add `UpdateSourceTrigger=PropertyChanged` in XAML, or `invoke` a button after `set-value` to trigger `LostFocus`
- **File pickers need `-w <HWND>`** — they run in a separate `PickerHost` process; use `list-windows` to find the picker HWND
- **Flyouts need `Start-Sleep 0.5`** after triggering — items appear asynchronously
- **ContentDialog buttons** often lack custom AutomationIds — use `inspect` to discover the selector
- **Use `$AppPid` not `$Pid`** — `$Pid` is read-only in PowerShell

**Maximum 2 fix-and-rerun cycles** — if tests still fail, report as known issues and move on.

---

## WPF → WinUI 3 Migration

### Namespace Map

| WPF | WinUI 3 |
|-----|---------|
| `System.Windows` | `Microsoft.UI.Xaml` |
| `System.Windows.Controls` | `Microsoft.UI.Xaml.Controls` |
| `System.Windows.Media` | `Microsoft.UI.Xaml.Media` |
| `System.Windows.Input` | `Microsoft.UI.Xaml.Input` |
| `System.Windows.Data` | `Microsoft.UI.Xaml.Data` |
| `System.Windows.Threading.Dispatcher` | `Microsoft.UI.Dispatching.DispatcherQueue` |
| `PresentationCore` / `PresentationFramework` | Remove entirely |

### Control Map

| WPF | WinUI 3 |
|-----|---------|
| `DataGrid` | `ListView` with Grid column headers |
| `WrapPanel` | `ItemsRepeater` + `UniformGridLayout` |
| `TabControl` | `TabView` |
| `Menu` / `MenuItem` | `MenuBar` / `MenuFlyoutItem` |
| `ToolBar` | `CommandBar` |

### Threading

```csharp
// WPF
Application.Current.Dispatcher.Invoke(() => { /* UI work */ });

// WinUI 3
DispatcherQueue.GetForCurrentThread().TryEnqueue(() => { /* UI work */ });
```

### Critical Migration Rules

- ❌ NEVER reference `PresentationCore`, `PresentationFramework`, or `System.Windows.Controls`
- ❌ NEVER add `<UseWPF>true</UseWPF>` — silently corrupts the build
- ❌ NEVER overwrite `App.xaml` / `App.xaml.cs` — merge WPF code into the WinUI 3 boilerplate
- ✅ Remove ALL `System.Windows.Media.Imaging` references at migration start
- ✅ Replace with `Microsoft.UI.Xaml.Media.Imaging.BitmapImage`
- ✅ Replace custom MVVM → CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`)
- ✅ Replace `.resx` → `.resw` in `Strings\en-us\`
- ✅ Replace `DynamicResource` → `{ThemeResource}`

### Audit WPF Usage

```powershell
Select-String -Path (Get-ChildItem -Recurse -Filter "*.cs" |
    Where-Object { $_.FullName -notlike "*\obj\*" }) -Pattern "System\.Windows\."
```

---

## Code Quality Guidelines

- **File-scoped namespaces**, `_camelCase` private fields, `PascalCase` types/methods/properties
- `Async` suffix on async methods, `Is/Has/Can` prefix on booleans
- Batch file creates/edits in one pass — don't re-read files you just wrote
- Chain dependent commands with `&&`
- YAGNI: no speculative abstractions; KISS: simplest solution that works

## Checklist Before Completion

- [ ] App builds with 0 errors, 0 warnings via `BuildAndRun.ps1`
- [ ] App launches and runs correctly with `winapp run`
- [ ] All interactive controls have `AutomationProperties.AutomationId`
- [ ] All colors use `{ThemeResource}` brushes — no hardcoded values
- [ ] Typography uses built-in TextBlock styles — no raw `FontSize`
- [ ] Spacing uses 4px grid multiples
- [ ] `x:Bind` used throughout with explicit `Mode`
- [ ] MVVM: `[ObservableProperty]` partial properties, `[RelayCommand]` attributes
- [ ] No `.Result` / `.Wait()` on async operations
- [ ] Developer Mode is enabled
- [ ] If packaging: certificate generated, trusted, and MSIX signed
