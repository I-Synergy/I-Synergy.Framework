param(
    [Parameter(Mandatory = $true)]
    [string]$BuildNumber,
    
    [Parameter(Mandatory = $true)]
    [string]$BuildReason
)

$ErrorActionPreference = 'Stop'

$parts = $BuildNumber.Split('.')

$versionMajor = $parts[0]    # yyyy
$versionMinor = $parts[1]    # 1MMdd
$versionRevision = $parts[2] # 1HHmm
$versionBuild = if ($BuildReason -eq 'PullRequest') { (Get-Date).Second } else { 0 }

$versionNumber = "$versionMajor.$versionMinor.$versionRevision.$versionBuild"
$versionInformational = "$versionMajor.$versionMinor.$versionRevision"
$versionPackage = if ($BuildReason -eq 'PullRequest') { "$versionInformational-preview" } else { $versionInformational }

Write-Host "Version Number: $versionNumber"
Write-Host "Version Informational: $versionInformational"
Write-Host "Version Package: $versionPackage"

Write-Host "##vso[task.setvariable variable=VersionNumber]$versionNumber"
Write-Host "##vso[task.setvariable variable=VersionInformational]$versionInformational"
Write-Host "##vso[task.setvariable variable=VersionPackage]$versionPackage"
