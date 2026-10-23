param([string]$Tf = 'net11.0')
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$AE = [System.Windows.Automation.AutomationElement]
$TS = [System.Windows.Automation.TreeScope]
$Exe = "D:\Projects\I-Synergy\I-Synergy.Framework\samples\Sample.Maui\bin\Debug\$Tf-windows10.0.26100.0\win-arm64\Sample.Maui.exe"
$p = Start-Process $Exe -PassThru
Start-Sleep 12
function Win { $c = New-Object System.Windows.Automation.PropertyCondition($AE::ProcessIdProperty, $p.Id); $AE::RootElement.FindAll($TS::Children, $c)[0] }

function Glyphs($w) {
  @($w.FindAll($TS::Descendants, (New-Object System.Windows.Automation.PropertyCondition($AE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Button))) | Where-Object { $_.Current.Name.Length -eq 1 -and [int][char]$_.Current.Name[0] -ge 0xE000 })
}
function Show($w, $label) {
  $g = Glyphs $w
  "$label : " + (($g | ForEach-Object { 'U+{0:X4}' -f [int][char]$_.Current.Name[0] }) -join ', ')
}
function Texts($w) { @($w.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition) | Where-Object { $_.Current.Name } | ForEach-Object { $_.Current.ControlType.ProgrammaticName.Replace('ControlType.','') + ':' + $_.Current.Name }) }
function Click($w, $i) {
  $g = Glyphs $w; $ip = $null
  if ($g[$i].TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$ip)) { $ip.Invoke(); "  invoked header button #$i" } else { "  NO invoke pattern on #$i" }
}
$w = Win
Show $w 'start'
Click $w 0; Start-Sleep 2; Show (Win) 'after theme toggle 1'
Click (Win) 0; Start-Sleep 2; Show (Win) 'after theme toggle 2'
Click (Win) 1; Start-Sleep 3

function Desc($w, $ct) { @($w.FindAll($TS::Descendants, (New-Object System.Windows.Automation.PropertyCondition($AE::ControlTypeProperty, $ct)))) }
'--- drawer controls'
$w = Win
$combos = Desc $w ([System.Windows.Automation.ControlType]::ComboBox)
"combos: $($combos.Count)"
$btns = Desc $w ([System.Windows.Automation.ControlType]::Button) | Where-Object { $_.Current.BoundingRectangle.Width -gt 0 -and $_.Current.Name -eq '' }
"unnamed buttons (swatches/scrim): $($btns.Count)"
# language: open first combo, pick 2nd item
if ($combos.Count -ge 1) {
  $ec = $null
  if ($combos[0].TryGetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$ec)) { $ec.Expand(); Start-Sleep 1 }
  $items = Desc (Win) ([System.Windows.Automation.ControlType]::ListItem) | Where-Object { $_.Current.Name -match '^[a-z]{2}' -or $_.Current.Name -match 'Nederlands|Deutsch|English|Fran' }
  "language items: " + (($items | ForEach-Object { $_.Current.Name }) -join ', ')
  $pick = $items | Where-Object { $_.Current.Name -match 'nl|Nederlands|Dutch' } | Select-Object -First 1
  if ($pick) { $si = $null; if ($pick.TryGetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern, [ref]$si)) { $si.Select(); "selected: $($pick.Current.Name)" }; Start-Sleep 2 }
}
'--- texts after language pick'
Texts (Win) | Where-Object { $_ -match 'Settings|Language|Theme|Accent|Instellingen|Taal|Thema|Dashboard' }
'--- close drawer, open nav'
$close = Desc (Win) ([System.Windows.Automation.ControlType]::Button) | Where-Object { $_.Current.Name -match '^(Close|Sluiten)$' -and $_.Current.AutomationId -eq '' } | Select-Object -First 1
if ($close) { $ip=$null; if ($close.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern,[ref]$ip)) { $ip.Invoke(); 'closed drawer' } }
Start-Sleep 2
$nav = Desc (Win) ([System.Windows.Automation.ControlType]::Button) | Where-Object { $_.Current.Name -eq 'Open Navigation' } | Select-Object -First 1
if ($nav) { $ip=$null; if ($nav.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern,[ref]$ip)) { $ip.Invoke() } }
Start-Sleep 2
'nav items: ' + ((Desc (Win) ([System.Windows.Automation.ControlType]::ListItem) | ForEach-Object { $_.Current.Name }) -join ', ')
Texts (Win) | Where-Object { $_ -match 'Help|Dashboard|Instell|Taal|Language|Hulp|Overzicht|Aangepast' }
Stop-Process -Id $p.Id -Force
