<#
.SYNOPSIS
  Reports (and optionally applies) newer versions for Directory.Packages.props.
.DESCRIPTION
  Directory.Packages.props has an unconditional group, a net10.0 group (stable 10.x) and a
  net11.0 group (11.0.0 rc/preview). Each entry is checked against NuGet within its own channel:
    - net10.0 group / unconditional: newest stable version with the same major version
    - net11.0 group: newest 11.0.0-rc.* / -preview.* (or 11.0.0 stable once released)
  Without -Apply nothing is changed.
#>
param([switch]$Apply)

$path = Join-Path $PSScriptRoot '..\Directory.Packages.props'
$text = Get-Content $path -Raw
$group = 'base'
$lines = $text -split "`n"
$changed = $false

function Get-Versions($id) {
    try { (Invoke-RestMethod "https://api.nuget.org/v3-flatcontainer/$($id.ToLower())/index.json").versions } catch { @() }
}

for ($i = 0; $i -lt $lines.Count; $i++) {
    $l = $lines[$i]
    if ($l -match "<ItemGroup Condition=.*StartsWith\('net11\.0'\)") { $group = if ($l -match '!\$') { 'net10' } else { 'net11' } }
    elseif ($l -match '</ItemGroup>') { $group = 'base' }
    if ($l -notmatch '<PackageVersion Include="([^"]+)" Version="([^"]+)"') { continue }
    $id = $Matches[1]; $cur = $Matches[2]
    $all = Get-Versions $id
    if (-not $all) { continue }
    $major = ($cur -split '\.')[0]
    $isPre = $cur -match '-'
    $candidates = if ($group -eq 'net11') { $all | Where-Object { $_ -match '^11\.0\.0(-(rc|preview)\.|$)' } }
                  elseif ($isPre)        { $all | Where-Object { $_ -match "^$major\." } }
                  else                   { $all | Where-Object { $_ -match "^$major\." -and $_ -notmatch '-' } }
    if (-not $candidates) { continue }
    $order = @($candidates)
    $best = $order[-1]
    if ($best -ne $cur) {
        Write-Host ("[{0}] {1}: {2} -> {3}" -f $group, $id, $cur, $best)
        if ($Apply) { $lines[$i] = $l.Replace("Version=`"$cur`"", "Version=`"$best`""); $changed = $true }
    }
}
if ($Apply -and $changed) { Set-Content $path ($lines -join "`n") -NoNewline; Write-Host 'Directory.Packages.props updated.' }
elseif (-not $Apply) { Write-Host 'Report only. Re-run with -Apply to write changes.' }
