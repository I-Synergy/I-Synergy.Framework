# Remove-UnusedPackages.ps1
# PowerShell script to remove unused package versions from Directory.Packages.props

param(
    [string]$DirectoryPackagesPath = "Directory.Packages.props",
    [switch]$WhatIf = $false,
    [switch]$Verbose = $false
)

Write-Host "=== Remove Unused Packages Script ===" -ForegroundColor Green

# Check if Directory.Packages.props exists
if (-not (Test-Path $DirectoryPackagesPath)) {
    Write-Error "Directory.Packages.props not found at path: $DirectoryPackagesPath"
    exit 1
}

Write-Host "Analyzing workspace for unused packages..." -ForegroundColor Yellow

# Get all defined packages from Directory.Packages.props
Write-Host "1. Reading defined packages from Directory.Packages.props..." -ForegroundColor Cyan
$definedPackagesContent = Get-Content $DirectoryPackagesPath -Raw
$definedPackagesMatches = [regex]::Matches($definedPackagesContent, '<PackageVersion Include="([^"]*)"')
$definedPackages = @()
foreach ($match in $definedPackagesMatches) {
    $definedPackages += $match.Groups[1].Value
}
$definedPackages = $definedPackages | Sort-Object -Unique

Write-Host "   Found $($definedPackages.Count) defined packages" -ForegroundColor Gray

# Get all used packages from project files
Write-Host "2. Scanning all .csproj files for package references..." -ForegroundColor Cyan
$projectFiles = Get-ChildItem -Path "." -Filter "*.csproj" -Recurse
$usedPackages = @()

foreach ($projectFile in $projectFiles) {
    if ($Verbose) {
        Write-Host "   Scanning: $($projectFile.Name)" -ForegroundColor Gray
    }
    
    $content = Get-Content $projectFile.FullName -Raw
    $matches = [regex]::Matches($content, '<PackageReference Include="([^"]*)"')
    
    foreach ($match in $matches) {
        $usedPackages += $match.Groups[1].Value
    }
}

$usedPackages = $usedPackages | Sort-Object -Unique
Write-Host "   Found $($usedPackages.Count) used packages across $($projectFiles.Count) project files" -ForegroundColor Gray

# Identify unused packages
Write-Host "3. Identifying unused packages..." -ForegroundColor Cyan
$unusedPackages = $definedPackages | Where-Object { $_ -notin $usedPackages }
$missingPackages = $usedPackages | Where-Object { $_ -notin $definedPackages }

# Display results
Write-Host "`n=== ANALYSIS RESULTS ===" -ForegroundColor Green

if ($unusedPackages.Count -gt 0) {
    Write-Host "`nUNUSED PACKAGES ($($unusedPackages.Count)):" -ForegroundColor Red
    $unusedPackages | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
} else {
    Write-Host "`nNo unused packages found!" -ForegroundColor Green
}

if ($missingPackages.Count -gt 0) {
    Write-Host "`nUSED PACKAGES NOT DEFINED ($($missingPackages.Count)):" -ForegroundColor Magenta
    $missingPackages | ForEach-Object { Write-Host "  - $_" -ForegroundColor Cyan }
    Write-Host "  (These packages are referenced but not defined in Directory.Packages.props)" -ForegroundColor Gray
}

# Remove unused packages if not in WhatIf mode
if ($unusedPackages.Count -gt 0) {
    if ($WhatIf) {
        Write-Host "`n[WHATIF] Would remove $($unusedPackages.Count) unused packages" -ForegroundColor Yellow
    } else {
        Write-Host "`n4. Removing unused packages from Directory.Packages.props..." -ForegroundColor Cyan
        
        # Create backup
        $backupPath = "$DirectoryPackagesPath.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        Copy-Item $DirectoryPackagesPath $backupPath
        Write-Host "   Backup created: $backupPath" -ForegroundColor Gray
        
        # Read the file content
        $content = Get-Content $DirectoryPackagesPath -Raw
        
        # Remove unused packages
        foreach ($unusedPackage in $unusedPackages) {
            $pattern = '(\s*)<PackageVersion Include="' + [regex]::Escape($unusedPackage) + '"[^/>]*\/>\s*\r?\n?'
            $content = $content -replace $pattern, ''
            
            if ($Verbose) {
                Write-Host "   Removed: $unusedPackage" -ForegroundColor Gray
            }
        }
        
        # Write the updated content back
        $content | Set-Content $DirectoryPackagesPath -NoNewline
        
        Write-Host "   Successfully removed $($unusedPackages.Count) unused packages" -ForegroundColor Green
        Write-Host "   Updated: $DirectoryPackagesPath" -ForegroundColor Green
    }
}

Write-Host "`n=== SCRIPT COMPLETED ===" -ForegroundColor Green

# Summary statistics
Write-Host "`nSUMMARY:" -ForegroundColor White
Write-Host "  - Total defined packages: $($definedPackages.Count)" -ForegroundColor Gray
Write-Host "  - Total used packages: $($usedPackages.Count)" -ForegroundColor Gray
Write-Host "  - Unused packages: $($unusedPackages.Count)" -ForegroundColor Gray
Write-Host "  - Missing definitions: $($missingPackages.Count)" -ForegroundColor Gray

if ($unusedPackages.Count -eq 0) {
    Write-Host "`nYour Directory.Packages.props is already optimized! ?" -ForegroundColor Green
}