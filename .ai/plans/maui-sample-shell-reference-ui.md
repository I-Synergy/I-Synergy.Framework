# Sample.Maui shell: mimic the reference app UI

Status: IN PROGRESS (direction given by user 2026-10-23: "mimic the reference app")
Reference: `D:\Projects\International Intelligence\The Quran` — `src/TheQuran.App/Views/ShellView.xaml`,
`Views/Controls/AppHeaderView.xaml`, `Views/Controls/SidebarView.xaml`, `Resources/Styles/QuranStyles.xaml`
(`QuranVerseActionButton`, `QuranSidebarRowBorder`, labels), `Colors.xaml`; screenshots in `screenshots/` (01 light, 03 dark, 04 settings drawer).

## Target (from the reference)
- **Sidebar**: permanent on desktop (`FlyoutBehavior` Locked on WinUI/MacCatalyst, Disabled on phones with a bottom TabBar),
  300 wide, surface background + 1px right border. Brand tile (40px rounded 12, accent fill, white mark) + app name + tagline,
  then rows (60px / 44px footer): icon tile + title (+ muted subtitle), active row tinted with accent-soft, Help in the footer + version/copyright.
- **Header** (`Shell.TitleView`): 880px centred column, breadcrumb `Section / Current page` (accent link, muted `/`, bold current),
  right side two ghost icon buttons (theme sun/moon, settings sliders) in muted colour, no fill, subtle fill on hover/pressed.
- **Tokens/fonts**: Figtree font family, surface/bg/border/muted/accent/accent-soft tokens for light + dark.

## Decisions
- Sample-neutral token names (not `Quran*`); accent tokens must follow `IThemeService.SetAccentColor` (derive from `Primary`).
- No search pill / Surah-Saved segmented control (the sample has no search); nav rows are the four sample pages.
- Icons: reuse fonts already bundled (FluentUI/FontAwesome); do not copy FontAwesome Pro files from the other repo.
  Figtree (OFL) may be copied.
- Keep the existing settings drawer, theme toggle behaviour and live localization working.

## Steps
- [ ] 1. Baseline screenshot of current Sample.Maui (light + dark)
- [ ] 2. Port tokens/styles/fonts into Sample.Maui resources
- [ ] 3. Sidebar view + Shell flyout/tab bar setup
- [ ] 4. Header view (breadcrumb + ghost theme/settings buttons)
- [ ] 5. Build, screenshot light + dark, compare with reference, fix deltas
