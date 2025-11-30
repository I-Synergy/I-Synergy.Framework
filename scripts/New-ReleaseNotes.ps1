param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$BuildNumber,
    
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$OutputPath
)

$ErrorActionPreference = 'Continue'

# Validate we're in a git repository
$null = git rev-parse --is-inside-work-tree 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "##vso[task.logissue type=error]Not inside a git repository or git is not available"
    exit 1
}

Write-Host "Attempting to fetch previous release tag..."

$previousTag = $null

# Get all tags sorted by version, take the latest
$allTags = git tag --sort=-version:refname 2>$null
if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace($allTags)) {
    $tagArray = $allTags -split "`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    if ($tagArray.Count -gt 0) {
        $previousTag = $tagArray[0].Trim()
        Write-Host "Previous tag found: $previousTag"
    }
}

if ([string]::IsNullOrWhiteSpace($previousTag)) {
    Write-Host "No previous tags found. Will generate notes for recent commits."
}

$commitLog = @()
$gitLogFailed = $false

if ([string]::IsNullOrWhiteSpace($previousTag)) {
    Write-Host "Collecting recent commits for release notes..."
    $commitLog = git log -50 --pretty=format:"- %h %s" 2>$null
    $gitLogFailed = $LASTEXITCODE -ne 0
} else {
    Write-Host "Collecting commits since tag '$previousTag'..."
    $commitLog = git log "$previousTag..HEAD" --pretty=format:"- %h %s" 2>$null
    $gitLogFailed = $LASTEXITCODE -ne 0
}

if ($gitLogFailed) {
    Write-Host "##vso[task.logissue type=warning]Git log command failed. Release notes may be incomplete."
    $commitLog = @("- Unable to retrieve commit history (git command failed).")
} elseif ($null -eq $commitLog -or ($commitLog -is [array] -and $commitLog.Count -eq 0) -or ($commitLog -is [string] -and [string]::IsNullOrWhiteSpace($commitLog))) {
    $commitLog = @("- No commits found since last release.")
}

if ($commitLog -is [string]) {
    $commitLogText = $commitLog
} else {
    $commitLogText = $commitLog -join [Environment]::NewLine
}

# Strip leading 'v' from BuildNumber if present to avoid 'vv' prefix
$cleanBuildNumber = $BuildNumber.TrimStart('v')
$releaseNotes = "# Release v$cleanBuildNumber`n`n## Changes`n`n$commitLogText"

$outputDir = Split-Path -Parent $OutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDir)) {
    if (-not (Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        Write-Host "Created directory: $outputDir"
    }
}

$releaseNotes | Out-File -FilePath $OutputPath -Encoding UTF8
if (-not $?) {
    Write-Host "##vso[task.logissue type=error]Failed to write release notes file"
    exit 1
}

Write-Host "Release notes successfully generated at: $OutputPath"
