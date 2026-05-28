# Adds TintedGray Color + SolidColorBrush resources to all WinUI theme files
# by reading pre-computed values from corresponding MAUI theme files.

$repoRoot = "D:\Projects\I-Synergy\I-Synergy.Framework"
$mauiThemesPath = "$repoRoot\src\ISynergy.Framework.UI.Maui\Resources\Styles\Themes"
$winuiThemesPath = "$repoRoot\src\ISynergy.Framework.UI.WinUI\Styles"

$winuiThemeFiles = Get-ChildItem -Path $winuiThemesPath -Filter "Theme*.xaml" | Where-Object { $_.Name -notlike "*.cs" }

$updatedCount = 0
$skippedCount = 0

foreach ($file in $winuiThemeFiles) {
    $mauiFile = Join-Path $mauiThemesPath $file.Name

    if (-not (Test-Path $mauiFile)) {
        Write-Host "SKIP: No MAUI counterpart for $($file.Name)" -ForegroundColor Yellow
        $skippedCount++
        continue
    }

    $winuiContent = Get-Content $file.FullName -Raw

    # Skip if already has TintedGray
    if ($winuiContent -match 'TintedGray') {
        Write-Host "SKIP: $($file.Name) already has TintedGray" -ForegroundColor Yellow
        $skippedCount++
        continue
    }

    $mauiContent = Get-Content $mauiFile -Raw

    # Extract Primary color comment line and TintedGray values from MAUI file
    $tintedGrayPattern = '<Color x:Key="TintedGray(\d{3})">#([0-9A-Fa-f]{6})</Color>'
    $tintedMatches = [regex]::Matches($mauiContent, $tintedGrayPattern)

    if ($tintedMatches.Count -eq 0) {
        Write-Host "SKIP: No TintedGray values found in MAUI $($file.Name)" -ForegroundColor Yellow
        $skippedCount++
        continue
    }

    # Also extract the primary color from the comment in MAUI
    $primaryMatch = [regex]::Match($mauiContent, '<!-- Tinted Grays \(15% blend with Primary #([0-9A-Fa-f]{6})\) -->')
    $primaryHex = if ($primaryMatch.Success) { $primaryMatch.Groups[1].Value.ToUpper() } else { "UNKNOWN" }

    # Build the XAML snippet
    $snippet = "`n    <!-- Tinted Grays (15% blend with Primary #$primaryHex) -->`n"
    foreach ($match in $tintedMatches) {
        $key = $match.Groups[1].Value
        $hex = $match.Groups[2].Value.ToUpper()
        $snippet += "    <Color x:Key=`"TintedGray$key`">#FF$hex</Color>`n"
    }
    $snippet += "`n"
    # Add SolidColorBrush for each tinted gray
    foreach ($match in $tintedMatches) {
        $key = $match.Groups[1].Value
        $snippet += "    <SolidColorBrush x:Key=`"TintedGray${key}Brush`" Color=`"{StaticResource TintedGray$key}`" />`n"
    }

    # Insert after </ResourceDictionary.ThemeDictionaries>
    $insertMarker = '</ResourceDictionary.ThemeDictionaries>'
    $newContent = $winuiContent -replace ([regex]::Escape($insertMarker)), ($insertMarker + $snippet)

    if ($newContent -eq $winuiContent) {
        Write-Host "SKIP: Could not find insertion point in $($file.Name)" -ForegroundColor Red
        $skippedCount++
        continue
    }

    Set-Content -Path $file.FullName -Value $newContent -NoNewline
    Write-Host "OK: $($file.Name) (Primary: #$primaryHex)" -ForegroundColor Green
    $updatedCount++
}

Write-Host "`nDone! Updated: $updatedCount, Skipped: $skippedCount" -ForegroundColor Cyan
