#Requires -Version 7.0
<#
.SYNOPSIS
    Renders the repository documentation into a GitHub wiki page set.

.DESCRIPTION
    Reads every Markdown file under -DocsPath and writes a flat set of wiki pages into -OutPath:
    path segments become hyphen-joined page names, relative links become /wiki/<Page> links,
    images are copied next to the pages, and _Sidebar.md is generated.

    Every generated page ends with an ownership marker (<!-- generated from docs/<path> -->) so a
    later sync only overwrites and only deletes pages it created. Hand-written wiki pages are left
    alone.

    The script does not talk to GitHub. The workflow pushes -OutPath to repo.wiki.git.

.PARAMETER DocsPath
    Documentation directory, relative to the repository root.

.PARAMETER OutPath
    Directory that receives the page set. Created when missing.

.PARAMETER Repository
    owner/name, used for blob links to files that are not published as wiki pages.

.PARAMETER BaseBranch
    Branch used in those blob links.

.PARAMETER Exclude
    Glob patterns (relative to -DocsPath) that stay out of the wiki.

.PARAMETER DryRun
    Render and report, but exit non-zero on collisions or unresolved links. Same rendering as a
    real run; nothing is ever pushed by this script.

.PARAMETER Check
    Run the built-in mapping and link-rewrite assertions only, then exit.

.EXAMPLE
    ./scripts/Sync-DocsToWiki.ps1 -OutPath ./artifacts/wiki -DryRun

.EXAMPLE
    ./scripts/Sync-DocsToWiki.ps1 -OutPath "$env:RUNNER_TEMP/wiki" -Repository I-Synergy/I-Synergy.Framework
#>
[CmdletBinding()]
param(
    [string]$DocsPath = 'docs',

    [Parameter(Mandatory = $true)]
    [string]$OutPath,

    [string]$Repository = 'I-Synergy/I-Synergy.Framework',

    [string]$BaseBranch = 'main',

    [string[]]$Exclude = @(
        'superpowers/*',
        'FRAMEWORK_REVIEW_*.md',
        'SOLID_PRINCIPLES_ANALYSIS.md',
        'POTENTIAL_ISSUES_AND_RISKS.md',
        'MVVM_AND_UI_FRAMEWORK_ISSUES_REPORT.md',
        'ARCHITECTURAL_ISSUES_SUMMARY.md',
        'ASYNC_EXCEPTION_ISSUES_FIXED.md'
    ),

    [switch]$DryRun,

    [switch]$Check
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$MarkerPrefix = '<!-- generated from docs/'
$Script:Problems = [System.Collections.Generic.List[string]]::new()

# ---------------------------------------------------------------------------------------------
# Paths and names
# ---------------------------------------------------------------------------------------------

function Get-NormalizedPath {
    param([string]$Path)

    $stack = [System.Collections.Generic.List[string]]::new()
    foreach ($part in ($Path -replace '\\', '/').Split('/')) {
        if ($part -eq '' -or $part -eq '.') { continue }
        if ($part -eq '..') {
            if ($stack.Count -gt 0) { $stack.RemoveAt($stack.Count - 1) }
            continue
        }
        $stack.Add($part)
    }

    return ($stack -join '/')
}

function Get-EscapeCount {
    # How many levels a relative path climbs above its starting directory.
    param([string]$Path)

    $depth = 0
    $escapes = 0
    foreach ($part in ($Path -replace '\\', '/').Split('/')) {
        if ($part -eq '' -or $part -eq '.') { continue }
        if ($part -eq '..') {
            if ($depth -gt 0) { $depth-- } else { $escapes++ }
            continue
        }
        $depth++
    }
    return $escapes
}

function Get-ParentPath {
    param([string]$Path)

    $segments = $Path.Split('/')
    if ($segments.Count -le 1) { return '' }
    return ($segments[0..($segments.Count - 2)] -join '/')
}

function Get-PageName {
    # docs/libraries/ISynergy.Framework.Core/index.md -> Libraries-ISynergy.Framework.Core
    param([string]$RelativePath)

    $segments = [System.Collections.Generic.List[string]]::new()
    foreach ($part in ($RelativePath -replace '\\', '/').Split('/')) {
        if ($part -ne '' -and $part -ne '.') { $segments.Add($part) }
    }

    if ($segments.Count -eq 0) { return 'Home' }

    $lastIndex = $segments.Count - 1
    if ($segments[$lastIndex] -match '^index\.md$') {
        $segments.RemoveAt($lastIndex)
    }
    else {
        $segments[$lastIndex] = $segments[$lastIndex] -replace '\.md$', ''
    }

    if ($segments.Count -eq 0) { return 'Home' }

    $slugs = foreach ($segment in $segments) {
        $slug = ($segment -replace '[\s_]+', '-') -replace '-{2,}', '-'
        $slug = $slug.Trim('-')
        if ($slug.Length -gt 0) { $slug = $slug.Substring(0, 1).ToUpperInvariant() + $slug.Substring(1) }
        $slug
    }

    return ($slugs -join '-')
}

function Test-Excluded {
    param([string]$RelativePath)

    foreach ($pattern in $Exclude) {
        if ($RelativePath -like $pattern) { return $true }
    }
    return $false
}

# ---------------------------------------------------------------------------------------------
# Markdown handling
# ---------------------------------------------------------------------------------------------

$Script:LinkPattern = [regex]'(?<image>!?)\[(?<text>[^\]]*)\]\((?<target>[^)\s]+)(?<title>\s+"[^"]*")?\)'
$Script:SchemePattern = [regex]'^(?:[A-Za-z][A-Za-z0-9+.\-]*:|//|#)'

function Get-AssetName {
    param([byte[]]$Bytes, [string]$Extension)

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { $hash = $sha.ComputeHash($Bytes) } finally { $sha.Dispose() }
    $short = -join ($hash[0..3] | ForEach-Object { $_.ToString('x2') })
    return "images/$short$Extension"
}

function Read-Title {
    param([string]$Path, [string]$Fallback)

    foreach ($line in (Get-Content -Path $Path -TotalCount 40)) {
        if ($line -match '^#\s+(.+?)\s*$') { return $Matches[1] }
    }
    return $Fallback
}

# ---------------------------------------------------------------------------------------------
# Assertions (-Check)
# ---------------------------------------------------------------------------------------------

function Invoke-SelfCheck {
    $cases = @(
        @{ In = 'MIGRATION_GUIDE.md';          Expected = 'MIGRATION-GUIDE' },
        @{ In = 'index.md';                    Expected = 'Home' },
        @{ In = 'libraries/index.md';           Expected = 'Libraries' },
        @{ In = 'libraries/ISynergy.Framework.Core/index.md'; Expected = 'Libraries-ISynergy.Framework.Core' },
        @{ In = 'developers/maui/nodejs_installation_windows.md'; Expected = 'Developers-Maui-Nodejs-installation-windows' },
        @{ In = 'a//b/./c.md';                  Expected = 'A-B-C' }
    )

    $failures = 0
    foreach ($case in $cases) {
        $actual = Get-PageName -RelativePath $case.In
        if ($actual -ne $case.Expected) {
            Write-Host "::error::Get-PageName('$($case.In)') = '$actual', expected '$($case.Expected)'"
            $failures++
        }
    }

    $normalizeCases = @(
        @{ In = './a/../b/c.md'; Expected = 'b/c.md' },
        @{ In = '../src/x.cs';   Expected = 'src/x.cs' }
    )
    foreach ($case in $normalizeCases) {
        $actual = Get-NormalizedPath -Path $case.In
        if ($actual -ne $case.Expected) {
            Write-Host "::error::Get-NormalizedPath('$($case.In)') = '$actual', expected '$($case.Expected)'"
            $failures++
        }
    }

    if ($failures -gt 0) {
        throw "$failures self-check assertion(s) failed."
    }

    Write-Host "Self-check passed: $(($cases.Count + $normalizeCases.Count)) assertions."
}

# ---------------------------------------------------------------------------------------------
# Rendering
# ---------------------------------------------------------------------------------------------

if ($Check) {
    Invoke-SelfCheck
    return
}

$docsRoot = (Resolve-Path -Path $DocsPath).Path
$docsPrefix = Get-NormalizedPath -Path $DocsPath

$allFiles = @(Get-ChildItem -Path $docsRoot -Recurse -File)
$markdownFiles = @($allFiles | Where-Object { $_.Extension -eq '.md' })

$sources = foreach ($file in $markdownFiles) {
    $relative = Get-NormalizedPath -Path ([System.IO.Path]::GetRelativePath($docsRoot, $file.FullName))
    [pscustomobject]@{
        Relative = $relative
        FullName = $file.FullName
        Page     = Get-PageName -RelativePath $relative
        Excluded = Test-Excluded -RelativePath $relative
    }
}
$sources = @($sources)

# A page name may only come from one source, otherwise the wiki would silently lose a page.
$collisions = $sources | Where-Object { -not $_.Excluded } | Group-Object -Property Page | Where-Object { $_.Count -gt 1 }
foreach ($collision in $collisions) {
    $Script:Problems.Add("Page name '$($collision.Name)' is produced by: $($collision.Group.Relative -join ', ')")
}

$pageByRelative = @{}
foreach ($source in ($sources | Where-Object { -not $_.Excluded })) {
    $pageByRelative[$source.Relative] = $source.Page
}

$assetByRelative = @{}
$assetFiles = @($allFiles | Where-Object { $_.Extension -ne '.md' })

if (Test-Path -Path $OutPath) { Remove-Item -Path $OutPath -Recurse -Force }
New-Item -ItemType Directory -Path $OutPath -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $OutPath 'images') -Force | Out-Null

$blobBase = "https://github.com/$Repository/blob/$BaseBranch"

function Resolve-Target {
    <#
        Resolves one Markdown link target to a wiki page, a blob URL, or an asset path.
        Returns $null when the target must be left exactly as written.
    #>
    param(
        [string]$SourceRelative,
        [string]$Target
    )

    if ($Target -match $SchemePattern) { return $null }

    $anchor = ''
    $path = $Target
    $hashIndex = $Target.IndexOf('#')
    if ($hashIndex -ge 0) {
        $path = $Target.Substring(0, $hashIndex)
        $anchor = $Target.Substring($hashIndex)
    }
    if ($path -eq '') { return $null }

    $parent = Get-ParentPath -Path $SourceRelative
    $combinedDirs = if ($parent -eq '') { $path } else { "$parent/$path" }
    $resolved = Get-NormalizedPath -Path $combinedDirs
    $repoPath = "$docsPrefix/$resolved"

    if ((Get-EscapeCount -Path $combinedDirs) -gt 0) {
        # The link climbs out of docs/ and points at repository content; the normalized path is
        # already repo-relative because the climbing segments were dropped.
        return [pscustomobject]@{ Kind = 'blob'; Value = "$blobBase/$resolved$anchor" }
    }

    $absolute = Join-Path $docsRoot $resolved
    $exists = Test-Path -Path $absolute -PathType Leaf

    if ($pageByRelative.ContainsKey($resolved)) {
        return [pscustomobject]@{ Kind = 'page'; Value = "/wiki/$($pageByRelative[$resolved])$anchor" }
    }

    if (-not $exists) {
        $Script:Problems.Add("Unresolved link in docs/$SourceRelative -> $Target")
        return [pscustomobject]@{ Kind = 'blob'; Value = "$blobBase/$repoPath$anchor" }
    }

    return [pscustomobject]@{ Kind = 'blob'; Value = "$blobBase/$repoPath$anchor" }
}

function Convert-MarkdownContent {
    param(
        [string]$SourceRelative,
        [string]$Content
    )

    $rewritten = $Content
    foreach ($match in $Script:LinkPattern.Matches($Content)) {
        $target = $match.Groups['target'].Value
        $isImage = $match.Groups['image'].Value -eq '!'

        $replacement = $null
        if ($isImage) {
            $parent = Get-ParentPath -Path $SourceRelative
            $combined = if ($parent -eq '') { $target } else { "$parent/$target" }
            $resolved = Get-NormalizedPath -Path $combined
            $absolute = Join-Path $docsRoot $resolved
            if ($resolved -notmatch '^\.\.' -and $target -notmatch $SchemePattern -and (Test-Path -Path $absolute -PathType Leaf)) {
                if (-not $assetByRelative.ContainsKey($resolved)) {
                    $bytes = [System.IO.File]::ReadAllBytes($absolute)
                    $name = Get-AssetName -Bytes $bytes -Extension ([System.IO.Path]::GetExtension($resolved))
                    Copy-Item -Path $absolute -Destination (Join-Path $OutPath $name) -Force
                    $assetByRelative[$resolved] = $name
                }
                $replacement = $assetByRelative[$resolved]
            }
        }
        else {
            $resolved = Resolve-Target -SourceRelative $SourceRelative -Target $target
            if ($null -ne $resolved) { $replacement = $resolved.Value }
        }

        if ($null -eq $replacement) { continue }

        $title = $match.Groups['title'].Value
        $newToken = "$($match.Groups['image'].Value)[$($match.Groups['text'].Value)]($replacement$title)"
        $rewritten = $rewritten.Replace($match.Value, $newToken)
    }

    return $rewritten
}

$written = [System.Collections.Generic.List[object]]::new()

foreach ($source in ($sources | Where-Object { -not $_.Excluded })) {
    $content = Get-Content -Path $source.FullName -Raw
    if ($null -eq $content) { $content = '' }

    $rendered = Convert-MarkdownContent -SourceRelative $source.Relative -Content $content
    $rendered = $rendered.TrimEnd() + "`n`n$MarkerPrefix$($source.Relative) -->`n"

    $destination = Join-Path $OutPath "$($source.Page).md"
    Set-Content -Path $destination -Value $rendered -NoNewline -Encoding utf8NoBOM

    $title = Read-Title -Path $source.FullName -Fallback (($source.Page -split '-')[-1])
    $written.Add([pscustomobject]@{ Page = $source.Page; Relative = $source.Relative; Title = $title })
}

# ---------------------------------------------------------------------------------------------
# _Sidebar.md
# ---------------------------------------------------------------------------------------------

function Get-TocOrder {
    param([string]$FolderRelative)

    $tocPath = if ($FolderRelative -eq '') { Join-Path $docsRoot 'toc.yml' } else { Join-Path (Join-Path $docsRoot $FolderRelative) 'toc.yml' }
    if (-not (Test-Path -Path $tocPath -PathType Leaf)) { return @{} }

    $order = @{}
    $index = 0
    foreach ($line in (Get-Content -Path $tocPath)) {
        if ($line -match '^\s*-\s*href:\s*(.+?)\s*$') {
            $href = $Matches[1].Trim('"', "'")
            $resolved = if ($FolderRelative -eq '') { Get-NormalizedPath -Path $href } else { Get-NormalizedPath -Path "$FolderRelative/$href" }
            if (-not $order.ContainsKey($resolved)) {
                $order[$resolved] = $index
                $index++
            }
        }
    }
    return $order
}

$groups = [ordered]@{}
$rootPages = @($written | Where-Object { $_.Relative -notmatch '/' })
$folders = @($written | Where-Object { $_.Relative -match '/' } | ForEach-Object { ($_.Relative -split '/')[0] } | Sort-Object -Unique)

if ($rootPages.Count -gt 0) { $groups[''] = $rootPages }

foreach ($folder in $folders) {
    $order = Get-TocOrder -FolderRelative $folder
    $pages = @($written | Where-Object { $_.Relative -match "^$([regex]::Escape($folder))/" })
    $sorted = $pages | Sort-Object -Property @(
        @{ Expression = { if ($order.ContainsKey($_.Relative)) { $order[$_.Relative] } else { [int]::MaxValue } } },
        @{ Expression = { $_.Relative } }
    )
    $groups[$folder] = @($sorted)
}

$sidebar = [System.Collections.Generic.List[string]]::new()
$sidebar.Add('### [Home](/wiki/Home)')
$sidebar.Add('')

foreach ($key in $groups.Keys) {
    $heading = if ($key -eq '') { 'General' } else { $key.Substring(0, 1).ToUpperInvariant() + $key.Substring(1) }
    $sidebar.Add("### $heading")
    foreach ($page in $groups[$key]) {
        $label = $page.Title -replace '[\[\]]', ''
        $sidebar.Add("- [$label](/wiki/$($page.Page))")
    }
    $sidebar.Add('')
}

Set-Content -Path (Join-Path $OutPath '_Sidebar.md') -Value ($sidebar -join "`n") -NoNewline -Encoding utf8NoBOM

# ---------------------------------------------------------------------------------------------
# Report
# ---------------------------------------------------------------------------------------------

$excludedCount = @($sources | Where-Object { $_.Excluded }).Count
Write-Host "Rendered $($written.Count) page(s) from $($sources.Count) Markdown file(s); $excludedCount excluded."
Write-Host "Assets copied: $($assetByRelative.Count)."
Write-Host "Pages per section:"
foreach ($key in $groups.Keys) {
    $name = if ($key -eq '') { '(root)' } else { $key }
    Write-Host "  $name : $($groups[$key].Count)"
}

if ($Script:Problems.Count -gt 0) {
    foreach ($problem in $Script:Problems) { Write-Host "::error::$problem" }
    throw "$($Script:Problems.Count) problem(s) found while rendering the wiki page set."
}

Write-Host 'No collisions and no unresolved links.'
if ($DryRun) { Write-Host 'Dry run: nothing is pushed by this script.' }
