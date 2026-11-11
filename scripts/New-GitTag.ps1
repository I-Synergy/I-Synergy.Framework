param(
    [Parameter(Mandatory = $true)]
    [string]$BuildNumber,
    
    [Parameter(Mandatory = $true)]
    [string]$SourceVersion,
    
    [Parameter(Mandatory = $false)]
    [string]$UserEmail = "azure-pipeline@i-synergy.com",
    
    [Parameter(Mandatory = $false)]
    [string]$UserName = "Azure Pipeline"
)

$ErrorActionPreference = 'Stop'
$tag = "v$BuildNumber"

function Show-ErrorAnnotation {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if ($env:GITHUB_ACTIONS -eq 'true') {
        Write-Host "::error::$Message"
    }
    else {
        Write-Host "##vso[task.logissue type=error]$Message"
    }
}

Write-Host "Checking if tag '$tag' already exists locally..."
$localTagExists = git tag -l "$tag"
if (-not [string]::IsNullOrWhiteSpace($localTagExists)) {
    Show-ErrorAnnotation "Tag '$tag' already exists locally"
    exit 1
}
Write-Host "Tag does not exist locally."

Write-Host "Checking if tag '$tag' exists on remote origin..."
$remoteTagExists = git ls-remote --tags origin "refs/tags/$tag"
if (-not [string]::IsNullOrWhiteSpace($remoteTagExists)) {
    Show-ErrorAnnotation "Tag '$tag' already exists on remote"
    exit 1
}
Write-Host "Tag does not exist on remote."

Write-Host "Configuring Git user for tag creation..."
git config user.email $UserEmail
if ($LASTEXITCODE -ne 0) { throw "Failed to set git user.email" }

git config user.name $UserName
if ($LASTEXITCODE -ne 0) { throw "Failed to set git user.name" }

Write-Host "Creating annotated tag '$tag'..."
git tag -a "$tag" -m "Release $tag" $SourceVersion
if ($LASTEXITCODE -ne 0) { throw "Failed to create tag" }

Write-Host "Pushing tag to origin..."
# Capture the output so the two distinct failure modes stay distinguishable: a refusal caused by
# the token's missing `workflows` permission is a configuration problem, not a transient error,
# and it must name the fix instead of looking like an ordinary rejected push.
$ErrorActionPreference = 'Continue'
$pushOutput = @(git push origin "$tag" 2>&1 | ForEach-Object { "$_" })
$pushExitCode = $LASTEXITCODE
$ErrorActionPreference = 'Stop'

$pushOutput | ForEach-Object { Write-Host $_ }

if ($pushExitCode -ne 0) {
    if (($pushOutput -join "`n") -match 'refusing to allow a GitHub App') {
        throw ("Failed to push tag '$tag': the push token may not create a ref whose commit changes " +
            ".github/workflows/. GITHUB_TOKEN can never carry the 'workflows' permission; configure " +
            "the RELEASE_TOKEN secret (fine-grained PAT with Contents: Read and write and Workflows: " +
            "Read and write) and check out with it.")
    }
    throw "Failed to push tag"
}

Write-Host "Tag '$tag' created and pushed successfully."
