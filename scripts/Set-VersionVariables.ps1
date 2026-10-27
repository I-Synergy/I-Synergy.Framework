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

function Set-PipelineVariable {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    # GitHub Actions reads step outputs from GITHUB_OUTPUT; Azure Pipelines uses the logging command.
    if (-not [string]::IsNullOrEmpty($env:GITHUB_OUTPUT)) {
        Add-Content -Path $env:GITHUB_OUTPUT -Value "$Name=$Value"
    }
    else {
        Write-Host "##vso[task.setvariable variable=$Name]$Value"
    }
}

Write-Host "Version Number: $versionNumber"
Write-Host "Version Informational: $versionInformational"
Write-Host "Version Package: $versionPackage"

Set-PipelineVariable -Name 'VersionNumber' -Value $versionNumber
Set-PipelineVariable -Name 'VersionInformational' -Value $versionInformational
Set-PipelineVariable -Name 'VersionPackage' -Value $versionPackage
