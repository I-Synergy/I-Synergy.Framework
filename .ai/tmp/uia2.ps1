param([string]$Tf = 'net11.0', [string]$Steps = 'Open Navigation', [int]$Wait = 10, [switch]$KeepAlive)
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$AE = [System.Windows.Automation.AutomationElement]
$TS = [System.Windows.Automation.TreeScope]
$Exe = "D:\Projects\I-Synergy\I-Synergy.Framework\samples\Sample.Maui\bin\Debug\$Tf-windows10.0.26100.0\win-arm64\Sample.Maui.exe"

function Get-Win([int]$ProcId) {
  $c = New-Object System.Windows.Automation.PropertyCondition($AE::ProcessIdProperty, $ProcId)
  $AE::RootElement.FindAll($TS::Children, $c)
}
function Dump($win, [int]$max = 80) {
  $all = $win.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
  "  -- {0} elements" -f $all.Count
  $i = 0
  foreach ($e in $all) {
    if ($i++ -ge $max) { break }
    $n = $e.Current.Name; $id = $e.Current.AutomationId
    if ($n -or $id) { "  {0,-10} | {1} | {2}" -f $e.Current.ControlType.ProgrammaticName.Replace('ControlType.', ''), $n, $id }
  }
}
function Click-ByName($win, [string]$name) {
  $c = New-Object System.Windows.Automation.PropertyCondition($AE::NameProperty, $name)
  $e = $win.FindFirst($TS::Descendants, $c)
  if (-not $e) { "  !! not found: $name"; return $false }
  $ip = $null
  if ($e.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$ip)) { $ip.Invoke(); return $true }
  $sp = $null
  if ($e.TryGetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern, [ref]$sp)) { $sp.Select(); return $true }
  $ep = $null
  if ($e.TryGetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$ep)) { $ep.Expand(); return $true }
  $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
  $a = $walker.GetParent($e)
  for ($k = 0; $k -lt 4 -and $a; $k++) {
    $ip = $null; $sp = $null
    if ($a.TryGetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern, [ref]$ip)) { $ip.Invoke(); return $true }
    if ($a.TryGetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern, [ref]$sp)) { $sp.Select(); return $true }
    $a = $walker.GetParent($a)
  }
  "  !! no pattern for: $name"; return $false
}

Add-Type @'
using System; using System.Runtime.InteropServices;
public class Mouse { [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
 [DllImport("user32.dll")] public static extern void mouse_event(int f, int dx, int dy, int d, int e);
 [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h); }
'@
function Mouse-Click($win, [string]$name) {
  $c = New-Object System.Windows.Automation.PropertyCondition($AE::NameProperty, $name)
  $e = $win.FindFirst($TS::Descendants, $c)
  if (-not $e) { "  !! not found: $name"; return }
  $r = $e.Current.BoundingRectangle
  [Mouse]::SetCursorPos([int]($r.X + $r.Width/2), [int]($r.Y + $r.Height/2)) | Out-Null
  Start-Sleep -Milliseconds 300
  [Mouse]::mouse_event(2,0,0,0,0); Start-Sleep -Milliseconds 100; [Mouse]::mouse_event(4,0,0,0,0)
  "  mouse-clicked $name at $([int]$r.X),$([int]$r.Y) $([int]$r.Width)x$([int]$r.Height)"
}
$p = Start-Process $Exe -PassThru
Start-Sleep $Wait
foreach ($step in ($Steps -split ';')) {
  $wins = Get-Win $p.Id
  if (-not $wins -or $wins.Count -eq 0) { "no window (exited=$($p.HasExited))"; break }
  $w = $wins[0]
  "STEP: $step (windows=$($wins.Count))"
  if ($step.StartsWith('SHOT:')) { Add-Type -AssemblyName System.Windows.Forms,System.Drawing; $b=[System.Windows.Forms.SystemInformation]::VirtualScreen; $bmp=New-Object System.Drawing.Bitmap $b.Width,$b.Height; $g=[System.Drawing.Graphics]::FromImage($bmp); $g.CopyFromScreen($b.Location,[System.Drawing.Point]::Empty,$b.Size); $bmp.Save($step.Substring(5)); '  saved '+$step.Substring(5) } elseif ($step -eq 'DUMP') { Dump $w 150 } elseif ($step.StartsWith('M:')) { [Mouse]::SetForegroundWindow([IntPtr]$w.Current.NativeWindowHandle) | Out-Null; Mouse-Click $w $step.Substring(2) } else { $ok = Click-ByName $w $step; "  clicked=$ok" }
  Start-Sleep 3
}
if (-not $KeepAlive) { Get-Process -Name Sample.Maui -ErrorAction SilentlyContinue | Stop-Process -Force }
