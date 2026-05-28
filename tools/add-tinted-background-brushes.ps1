# Adds theme-aware TintedGray semantic brushes inside Default and Light
# ThemeDictionaries of each WinUI theme file, then updates sample Style.xaml.

$repoRoot = "D:\Projects\I-Synergy\I-Synergy.Framework"
$themesPath = "$repoRoot\src\ISynergy.Framework.UI.WinUI\Styles"

# Semantic brush definitions for each theme mode
$darkBrushes = @'
        <!-- Tinted Gray semantic background brushes (Dark theme) -->
        <SolidColorBrush x:Key="TintedBackgroundBrush" Color="{StaticResource TintedGray950}" />
        <SolidColorBrush x:Key="TintedSurfaceBrush" Color="{StaticResource TintedGray900}" />
        <SolidColorBrush x:Key="TintedControlBrush" Color="{StaticResource TintedGray900}" />
        <SolidColorBrush x:Key="TintedDisabledBrush" Color="{StaticResource TintedGray600}" />
        <SolidColorBrush x:Key="TintedBorderBrush" Color="{StaticResource TintedGray600}" />
        <SolidColorBrush x:Key="TintedSecondaryBrush" Color="{StaticResource TintedGray500}" />
'@

$lightBrushes = @'
        <!-- Tinted Gray semantic background brushes (Light theme) -->
        <SolidColorBrush x:Key="TintedBackgroundBrush" Color="{StaticResource TintedGray000}" />
        <SolidColorBrush x:Key="TintedSurfaceBrush" Color="{StaticResource TintedGray100}" />
        <SolidColorBrush x:Key="TintedControlBrush" Color="{StaticResource TintedGray100}" />
        <SolidColorBrush x:Key="TintedDisabledBrush" Color="{StaticResource TintedGray200}" />
        <SolidColorBrush x:Key="TintedBorderBrush" Color="{StaticResource TintedGray200}" />
        <SolidColorBrush x:Key="TintedSecondaryBrush" Color="{StaticResource TintedGray300}" />
'@

$themeFiles = Get-ChildItem -Path $themesPath -Filter "Theme*.xaml" | Where-Object { $_.Name -notlike "*.cs" }

$updated = 0
$skipped = 0

foreach ($file in $themeFiles) {
    $content = Get-Content $file.FullName -Raw

    # Skip if already has TintedBackgroundBrush
    if ($content -match 'TintedBackgroundBrush') {
        Write-Host "SKIP: $($file.Name) already has semantic brushes" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Insert dark brushes after SystemControlPageBackgroundMediumAltMediumBrush in Default (dark) dict
    # The dark version has Color="#99000000"
    $darkMarker = 'SystemControlPageBackgroundMediumAltMediumBrush" Color="#99000000" />'
    if ($content -notmatch [regex]::Escape($darkMarker)) {
        Write-Host "SKIP: Dark marker not found in $($file.Name)" -ForegroundColor Red
        $skipped++
        continue
    }
    $content = $content -replace ([regex]::Escape($darkMarker)), ($darkMarker + "`n" + $darkBrushes)

    # Insert light brushes after SystemControlPageBackgroundMediumAltMediumBrush in Light dict
    # The light version has Color="#99FFFFFF"
    $lightMarker = 'SystemControlPageBackgroundMediumAltMediumBrush" Color="#99FFFFFF" />'
    if ($content -notmatch [regex]::Escape($lightMarker)) {
        Write-Host "SKIP: Light marker not found in $($file.Name)" -ForegroundColor Red
        $skipped++
        continue
    }
    $content = $content -replace ([regex]::Escape($lightMarker)), ($lightMarker + "`n" + $lightBrushes)

    Set-Content -Path $file.FullName -Value $content -NoNewline
    Write-Host "OK: $($file.Name)" -ForegroundColor Green
    $updated++
}

Write-Host "`nDone! Updated: $updated, Skipped: $skipped" -ForegroundColor Cyan
