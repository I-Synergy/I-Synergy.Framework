# PowerShell script to add tinted grays to all MAUI theme files

function Get-TintedColor {
    param(
        [int]$r1, [int]$g1, [int]$b1,
        [int]$r2, [int]$g2, [int]$b2,
        [double]$tintAmount = 0.15
    )
    
    $r = [Math]::Round($r2 * (1 - $tintAmount) + $r1 * $tintAmount)
    $g = [Math]::Round($g2 * (1 - $tintAmount) + $g1 * $tintAmount)
    $b = [Math]::Round($b2 * (1 - $tintAmount) + $b1 * $tintAmount)
    
    return ("{0:X2}{1:X2}{2:X2}" -f $r, $g, $b)
}

$baseGrays = @{
    "000" = @(0xF0, 0xF0, 0xF0)
    "100" = @(0xE1, 0xE1, 0xE1)
    "200" = @(0xC8, 0xC8, 0xC8)
    "300" = @(0xAC, 0xAC, 0xAC)
    "400" = @(0x91, 0x91, 0x91)
    "500" = @(0x6E, 0x6E, 0x6E)
    "600" = @(0x40, 0x40, 0x40)
    "900" = @(0x21, 0x21, 0x21)
    "950" = @(0x14, 0x14, 0x14)
}

$themesPath = "..\src\ISynergy.Framework.UI.Maui\Resources\Styles\Themes"
$themeFiles = Get-ChildItem -Path $themesPath -Filter "Theme*.xaml" | Where-Object { $_.Name -notlike "*.cs" }

foreach ($file in $themeFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Skip if already has TintedGray
    if ($content -match 'TintedGray') {
        Write-Host "Skipping $($file.Name) - already has tinted grays" -ForegroundColor Yellow
continue
    }
    
    # Extract primary color
    if ($content -match '<Color x:Key="Primary">#([0-9A-Fa-f]{6})</Color>') {
        $primaryHex = $matches[1].ToUpper()
        $r1 = [Convert]::ToInt32($primaryHex.Substring(0,2), 16)
        $g1 = [Convert]::ToInt32($primaryHex.Substring(2,2), 16)
        $b1 = [Convert]::ToInt32($primaryHex.Substring(4,2), 16)
        
        # Generate tinted grays
      $tintedGraysXaml = "`n    <!-- Tinted Grays (15% blend with Primary #$primaryHex) -->`n"
 
        foreach ($grayName in @("000", "100", "200", "300", "400", "500", "600", "900", "950")) {
     $gray = $baseGrays[$grayName]
            $tinted = Get-TintedColor -r1 $r1 -g1 $g1 -b1 $b1 -r2 $gray[0] -g2 $gray[1] -b2 $gray[2]
      $tintedGraysXaml += "    <Color x:Key=`"TintedGray$grayName`">#$tinted</Color>`n"
  }
        
        # Insert before closing ResourceDictionary tag
        $content = $content -replace '(</ResourceDictionary>)', "$tintedGraysXaml`$1"
        
   # Write back
        Set-Content -Path $file.FullName -Value $content -NoNewline
        
        Write-Host "Updated $($file.Name)" -ForegroundColor Green
    }
}

Write-Host "`nDone!" -ForegroundColor Cyan
