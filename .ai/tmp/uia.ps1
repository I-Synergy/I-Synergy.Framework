param([string]$Tf = 'net11.0', [int]$Wait = 12, [string]$Click = '')
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$Exe = "D:\Projects\I-Synergy\I-Synergy.Framework\samples\Sample.Maui\bin\Debug\$Tf-windows10.0.26100.0\win-arm64\Sample.Maui.exe"
$p = Start-Process $Exe -PassThru
Start-Sleep $Wait
$p.Refresh()
if ($p.HasExited) { "EXITED $($p.ExitCode)"; return }
$AE = [System.Windows.Automation.AutomationElement]
$root = $AE::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition($AE::ProcessIdProperty, [int]$p.Id)
$wins = $root.FindAll([System.Windows.Automation.TreeScope]::Children, $cond)
"windows: $($wins.Count)"
foreach ($w in $wins) {
  $all = $w.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
  "window '$($w.Current.Name)' descendants: $($all.Count)"
  $i = 0
  foreach ($e in $all) {
    if ($i++ -gt 150) { break }
    "{0,-12} | {1} | {2}" -f $e.Current.ControlType.ProgrammaticName.Replace('ControlType.', ''), $e.Current.Name, $e.Current.AutomationId
  }
}
Stop-Process -Id $p.Id -Force
