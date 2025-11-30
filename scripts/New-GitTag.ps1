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

Write-Host "Checking if tag '$tag' already exists locally..."
$localTagExists = git tag -l "$tag"
if (-not [string]::IsNullOrWhiteSpace($localTagExists)) {
    Write-Host "##vso[task.logissue type=error]Tag '$tag' already exists locally"
    exit 1
}
Write-Host "Tag does not exist locally."

Write-Host "Checking if tag '$tag' exists on remote origin..."
$remoteTagExists = git ls-remote --tags origin "refs/tags/$tag"
if (-not [string]::IsNullOrWhiteSpace($remoteTagExists)) {
    Write-Host "##vso[task.logissue type=error]Tag '$tag' already exists on remote"
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
git push origin "$tag"
if ($LASTEXITCODE -ne 0) { throw "Failed to push tag" }

Write-Host "Tag '$tag' created and pushed successfully."
