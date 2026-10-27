# WinUI 3 UI Automation Testing

Scripted `winapp ui` batch testing: the test-script template, the assertion reference, and the gotchas that cost the most time. Load this when writing or debugging UI automation tests.
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
