# Session Handoff - 2026-10-27 (CI/CD moved to GitHub Actions)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Ported `azure-pipelines.yml` to GitHub Actions so GitHub owns build and release. Plan:
`.ai/plans/github-actions-cutover.md`; progress: `.ai/progress/github-actions-cutover.md`
(IN PROGRESS — manual setup and the first green release remain).

- **New `.github/workflows/ci.yml`** — two jobs on GitHub-hosted Windows runners:
  `build` (version stamping → setup-dotnet from `global.json` → .NET 10 for AzureSignTool → MAUI
  workloads → restore → build → vulnerability audit → pack → net10/net11 package guard → per-project
  tests with OpenCover → test-results + nupkg artifacts) and `release`, which keeps the ADO ACC/PROD
  split: a PR runs it in the `acceptance` environment and signs + pushes `-preview` packages to
  NuGet.org, while a `main` push runs it in `production` (approval gate), publishes the release set
  and creates the `v<version>` tag. Signing uses `azure/login@v2` OIDC →
  AzureSignTool `-act WorkloadIdentityCredential`; pushing uses `NuGet/login@v1` trusted publishing.
  **Signing no longer goes through `azure/login@v2`** — see the follow-up at the end of this entry:
  the workflow exchanges GitHub's OIDC token itself into `AZURE_FEDERATED_TOKEN_FILE`.
  Version scheme `yyyy.1MMdd.1HHmm` is unchanged.
- **`scripts/Set-VersionVariables.ps1`** writes `GITHUB_OUTPUT` when present, otherwise the ADO
  `##vso[task.setvariable]` command; **`scripts/New-GitTag.ps1`** emits `::error::` on GitHub,
  otherwise `##vso[task.logissue type=error]`. Both still serve Azure Pipelines during the transition.
- **AssemblyVersion/FileVersion** are passed as MSBuild properties (`-p:AssemblyVersion`,
  `-p:FileVersion`, `-p:InformationalVersion`, `-p:PackageVersion`) because `Directory.Build.props`
  pins them to `0.0.0.0` and the old `Assembly-Info-NetCore@3` task used to overwrite them.
- **Docs**: README badges now point at the Actions workflow; `.ai/project/tech-stack.md` and
  `architecture.md` updated; `docs/MIGRATION_GUIDE.md` gained Part 11; `CHANGELOG.md` entry added;
  `azure-pipelines.yml` carries a deprecation header (delete after the first green GitHub release).

## Decisions Made
- **One workflow, two jobs, two publish lanes**: `build` runs for PRs and `main`; `release` signs and
  publishes in both lanes but uses a different environment — `acceptance` (no gate) for PR previews,
  `production` (approval) for releases. Key Vault credentials never reach `build`.
  - **Alternatives considered**: main-only publishing (rejected: ADO signed and pushed `-preview`
    packages from PR builds through the ACC variable group, and that behaviour had to be preserved);
    signing in `build` (rejected: the environment gate would then apply to every build).
- **OIDC everywhere**: one federated credential per environment scoped to
  `environment:acceptance` / `environment:production`, and `NuGet/login@v1` trusted publishing — no client
  secret, no API key. No subscription either: signing only needs the vault's data plane.
- **Keep `azure-pipelines.yml`** (deprecated, not deleted) until GitHub publishes once, so releases
  do not stop before the GitHub environment/secrets/nuget.org policy exist.

## Open Questions
- **DONE**: `production` environment (user), 7 repository variables and the `NUGET_USER` secret —
  all placeholders except `BUILD_CONFIGURATION=Release` and `TIMESTAMP_URL=http://timestamp.digicert.com`.
- **DONE**: real values for `NUGET_USER`, `AZURE_TENANT_ID`, `AZURE_CLIENT_ID`,
  `AZURE_KEYVAULT_URI`, `AZURE_SIGNING_CERTIFICATE`, plus both federated credentials and both
  nuget.org trusted-publishing policies. `AZURE_SUBSCRIPTION_ID` is still set in the repository but the
  workflow no longer reads it — the identity has no subscription role and none is needed.
- **OPEN**: `windows-2025-vs2026` is valid today (it is an alias of `windows-2025`); the comment in
  the workflow says to fall back to `windows-2025` if the label goes away.
- **ASSUMED**: `dotnet pack` on the `.slnx` produces exactly the 48 `src/` packages; the
  package-count guard fails loudly if not, so the first run will prove it.

## Files Modified
- `.github/workflows/ci.yml` - Created, build + release jobs
- `scripts/Set-VersionVariables.ps1` - `Set-PipelineVariable` (GITHUB_OUTPUT / ##vso)
- `scripts/New-GitTag.ps1` - `Write-ErrorAnnotation` (::error:: / ##vso)
- `azure-pipelines.yml` - Deprecation header only
- `README.md` - Both ADO build badges → GitHub Actions
- `.ai/project/tech-stack.md`, `.ai/project/architecture.md` - CI/CD and versioning lines
- `docs/MIGRATION_GUIDE.md` - Overview note + Part 11
- `CHANGELOG.md` - `#### CI/CD moved to GitHub Actions`
- `.ai/plans/github-actions-cutover.md`, `.ai/progress/github-actions-cutover.md` - Created

## Next Actions (Priority Order)
1. ~~Complete the manual GitHub/Azure/nuget.org setup~~ — done (11.1–11.3).
2. Push the branch to GitHub, open a PR, confirm `build` is green (done: build job green twice) and the
   `acceptance` publish signs + pushes a `-preview` package.
3. Merge to `main`, approve `production`, confirm signed packages on nuget.org and the tag on GitHub.
4. Disable the Azure DevOps pipeline, delete `azure-pipelines.yml`, move the progress file to
   `.ai/completed/`.
5. Optional: add a `macos-*` job for `ISynergy.Framework.UI.Maui.Mac.Tests` (excluded today, as in
   ADO).

## Session Notes
- Verified locally: workflow YAML parses (PyYAML; jobs `build`/`release`, main-only triggers),
  both PowerShell scripts parse, and `Set-VersionVariables.ps1` writes
  `VersionNumber=2026.11027.1123.0` / `VersionInformational` / `VersionPackage` to `GITHUB_OUTPUT`.
- No `src/` or `tests/` change; no build was run (nothing to compile).
- `NuGet/login@v1` inputs/outputs confirmed from its `action.yml` (`user` → `NUGET_API_KEY`).

## Follow-up (same day) — first PR run, signing credentials
Run #6 (`20e89cc5`) proved the `build` job (43 min: version, SDKs, MAUI workloads, restore, build,
audit, pack, net10/net11 package guard, tests, artifacts) but the `Publish preview packages` job failed
at `Azure login (OIDC)`: *"No subscriptions found for ***"*. Two facts came out of it:

- The signing identity has a vault **access policy**, not a subscription role, so `azure/login@v2`
  cannot succeed with `subscription-id`; `allow-no-subscriptions: true` would be needed.
- `azure/login@v2` no longer exports `AZURE_CLIENT_ID` / `AZURE_TENANT_ID` / `AZURE_FEDERATED_TOKEN_FILE`
  (verified in its bundled `lib/main/index.js` — the strings are absent). It only authenticates the `az`
  CLI, so `-act WorkloadIdentityCredential` could never have found credentials through it.

**Fix (commit `b2627afa`, pushed; run #8 on `a380d494`):** the `Azure login (OIDC)` step was replaced by
`Fetch OIDC token for Key Vault` — `Invoke-RestMethod` against `ACTIONS_ID_TOKEN_REQUEST_URL` with
`audience=api://AzureADTokenExchange`, the token written BOM-less to `$RUNNER_TEMP/azure-federated-token`
and `AZURE_CLIENT_ID`/`AZURE_TENANT_ID`/`AZURE_AUTHORITY_HOST`/`AZURE_FEDERATED_TOKEN_FILE` appended to
`GITHUB_ENV`. `-act WorkloadIdentityCredential` now does the exchange itself: no `az`, no subscription,
no client secret. Verified locally against a stub OIDC endpoint (four `GITHUB_ENV` lines, token file
35 bytes, no BOM, no trailing newline); YAML parses and the step body parses as PowerShell.

- Docs updated to match: `docs/MIGRATION_GUIDE.md` 11.1–11.2 (subscription dropped),
  `CHANGELOG.md`, and the plan's decision row.
- **Next:** run #8 (`a380d494`) proved Key Vault signing — `Fetch OIDC token for Key Vault` and
  `Sign NuGet packages` are both green. It then failed at `NuGet/login@v1` with
  `HTTP 401 … No matching trust policy owned by user '***'`: the `NUGET_USER` secret is `I-Synergy`,
  but a trusted-publishing policy answers to the username that **created** it. Set the secret to that
  username (or recreate both policies under the account that will push), then re-run the **failed
  jobs** of #8 — a full rebuild only costs 30 minutes. After a green push: merge to `main`, approve
  `production`, delete `azure-pipelines.yml`.
- Run #7 (`b2627afa`) was cancelled five minutes in by run #8 (`a380d494`, the docs-only follow-up):
  for `pull_request` events `paths-ignore` is evaluated against the PR's whole diff, so an `*.md`-only
  push re-runs CI. Do not push to the PR branch while a run is in flight.
- Note: unauthenticated GitHub API rate limit is 60 requests/hour here; poll the run sparsely.

## Follow-up — docs published to the wiki; cutover left open
- **Docs → wiki implemented** (`8e7a8853`, on the PR branch, pushed):
  `scripts/Sync-DocsToWiki.ps1` + `.github/workflows/wiki.yml`. 414 pages from 464 Markdown files
  (50 internal reports excluded), page names from the folder path, relative links → `/wiki/<Page>`,
  unreachable targets → `blob/main`, `_Sidebar.md` in `toc.yml` order, `<!-- generated from docs/… -->`
  ownership marker. Verified: `-Check` 8/8, `-DryRun` 0 collisions / 0 unresolved links, second render
  byte-identical (`diff -r`), workflow YAML parses, push step passes `bash -n`. Staged against a real
  clone of the wiki: 415 files, `Home.md` untouched.
- **The wiki repository now exists** (`master` at `fe457293`, only `Home.md`, created by the user).
  A manual push from here hangs (exit 124) — the stored git credential cannot write `repo.wiki.git`,
  which is why the workflow needs the `WIKI_TOKEN` secret. **Not yet published.**
- **Cutover left open on purpose** (user decision): run #8's re-run is green end to end and
  `2026.11027.10205-preview` is public and author-signed, but `main` is still `8a447e03`. Remaining,
  all user actions: add `WIKI_TOKEN`, merge PR #171 (run #9 on `8e7a8853`), approve `production`,
  then delete `azure-pipelines.yml` and move `.ai/progress/github-actions-cutover.md` to
  `.ai/completed/`. Record: `.ai/completed/wiki-docs-sync.md`.

---

# Session Handoff - 2026-10-26 (System theme in every sample app)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
`Themes.Default` is now "System" in all four heads, every sample offers it and fresh installs start on
it. Plan: `.ai/plans/system-theme-all-samples.md` (IMPLEMENTED), record:
`.ai/completed/system-theme-all-samples.md`.

- **Framework**: WPF resolves Default from `AppsUseLightTheme` and watches
  `SystemEvents.UserPreferenceChanged`; WinUI maps Default to `ElementTheme.Default` and watches the
  root's `ActualThemeChanged`; MAUI maps Default to `AppTheme.Unspecified` (it used to force Dark) and
  watches `Application.RequestedThemeChanged`; Blazor resolves `system` with `matchMedia`, watches it
  and calls `[JSInvokable] OnSystemThemeChanged`. All four raise `ThemeChanged` on a live OS change and
  persist an explicit choice made while the resolved theme already matches.
- **Samples**: `LocalSettings.Theme` defaults to `Themes.Default`; WPF/WinUI/MAUI/Blazor pickers list
  System and show localized descriptions (new WPF `EnumToDescriptionConverter`); `Themes.Default`'s
  description is "System"; Blazor applies the theme in an inline head script to avoid a light first
  paint.
- **Runtime checks**: WPF (dark + picker "System"), MAUI (dark) and Blazor (dark before and after
  hydration; stubbed matchMedia change flipped `data-theme` and called back into .NET) were run against
  the OS dark theme. WinUI's unpackaged sample cannot start on this machine (`REGDB_E_CLASSNOTREG`
  from the Windows App SDK initializer); it is covered by build + 56 tests.
- **Tests**: WPF 27/27, WinUI 56 passed / 8 skipped. All four samples build. MAUI was built against
  `net10.0-windows` because the rc.3 SDK lacks the `maui-tizen` workload.

## Follow-up
- Verify WinUI at runtime on a machine with the Windows App Runtime installed.
- The OS-level theme was not flipped during verification; the WPF/WinUI/MAUI live handlers still
  deserve one manual OS-toggle pass.

---

# Session Handoff - 2026-10-26 (WPF sample started in the wrong theme)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Fixed the WPF sample starting with light surfaces while its theme state reported dark (it took two
toggles to reach dark). Record: `.ai/completed/wpf-theme-startup.md`.

- **Root cause:** `Themes.Default` was interpreted inconsistently. `Sample.WPF` painted the light
  tokens (`Theme == Themes.Dark`), while the shell toggle, the icon and `IsLightThemeEnabled` treated
  it as dark. The framework's WPF theme dictionary also always used the dark gray ramp.
- **Fix:** WPF `ThemeService.Theme` now resolves `Themes.Default` from the Windows
  `AppsUseLightTheme` personalization value and `ApplyTheme` passes the resolved theme through;
  `DynamicThemeResourceDictionary(primary, theme)` derives `Tinted*` brushes from the matching gray
  ramp end (light or dark); the WPF sample derives tokens, icon and the settings picker from the
  resolved theme.
- **Verified** on the real sample with `"Theme":0` on an OS in dark app mode: dark on start, one
  click to light, persistence correct; explicit Light/Dark restarts and the settings-blade path were
  also checked visually. `dotnet test tests/ISynergy.Framework.UI.WPF.Tests` 27/27; `Sample.WPF`
  builds with 0 warnings.

## Follow-up
- WPF resolves `Themes.Default` per read but does not subscribe to OS theme changes, so a running app
  with `Default` keeps its painted palette until the next apply/toggle.
- WinUI and MAUI keep their stored-theme semantics; consider aligning them if `Default` is offered
  by a settings surface.

---

# Session Handoff - 2026-10-26 (Azure DevOps build 6180: test failures)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Diagnosed and fixed the three failure classes of build 6180 (PR 1685, commit `286339dd`, "target
.NET 11"), then verified everything locally on the pinned 11.0.100-rc.2 SDK. Tests only — no
production code changed. Plan: `.ai/plans/devops-6180-test-failures.md`,
record: `.ai/completed/devops-6180-test-failures.md`.

- **9 assertion failures — .NET 11 breaking change.** Since .NET 11 Preview 7, `double` → `decimal`
  conversions round the *exact* source value once instead of keeping 15 significant digits
  (`Decimal and BigInteger floating-point conversions are correctly rounded`), so
  `(decimal)0.2d` is `0.2000000000000000111022302463`. Reproduced locally (net10 = `0.2`,
  net11 = the 28-digit value) and fixed the expectations, never the conversion behaviour:
  `PercentageTests` (expected column is now a `string` parsed with `InvariantCulture`),
  `CurrencyFormatterTests.TryParse_ValidInput_ReturnsTrue(string, string)`, and
  `Matrix.Conversion.cs TableToJaggedTest` (explicit single-column `decimal[][]` instead of a
  `double[] → decimal[]` round-trip).
- **WPF + WinUI runs aborted** because their test projects were still on `net10.0-*` while the agent
  installs only the 11 runtime. Bumped to `net11.0-windows` / `net11.0-windows10.0.26100.0`.
- **`Sample.Blazor.UI.Tests`** — the `IThemeService` DI crash seen in the log was already fixed by
  `cbd17689`; added the missing `coverlet.collector` reference so the pipeline's
  `--collect:"XPlat Code Coverage"` resolves for that project too.

## Verification
Financial 29/29, Globalization 122/122, Mathematics 954 (+1 skipped) — all previously red;
WPF 20/20 on net11; WinUI builds (pre-existing CsWinRT1032 warning only); Blazor suite builds and
emits `coverage.cobertura.xml`.

## Follow-up, also done in this session
With the Blazor suite able to start, 16 of its 36 tests failed against the shell `cbd17689`
introduced (stale FluentLayout DOM, and at `<=900px` an icon rail with no drawer). Per the user's
choice, a mobile drawer was added and the tests were re-baselined —
record: `.ai/completed/blazor-shell-mobile-drawer.md`. The suite is 36/36 and the drawer was checked
in a real browser at 1280px and 480px. Remaining nicety: the dark theme's active nav icon glyph is
~1.87:1 (decorative, `aria-hidden`, therefore excluded from the text-contrast sweep).

**Pipeline is green.** Three commits were pushed and watched (6181 → 6182 → 6183). Fixes after the
first push: the dark-theme breadcrumb contrast (new `--app-accent-text` token, 10:1 measured) and the
MAUI memory test that still asserted `NavigationService._pageFrames` (now guards
`NavigationHostView._bodies`). Build 6182 proved all tests green but failed signing, because
`AzureSignTool` is a .NET 10 tool and the pinned .NET 11 SDK leaves only the 11 runtime on the agent;
6183 adds a `UseDotNet@2` step installing the `10.0.x` runtime and **succeeds end to end** (build,
tests, sign, artifacts, NuGet deploy). Also note `scripts/Update-PackageVersions.ps1` rewrote every
version in `Directory.Packages.props` during local runs — it was reverted and is not committed.

---

# Session Handoff - 2026-10-25 (Settings as a blade: WPF + WinUI + MAUI)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Converted the settings surface of every supported head into a blade (edge pane) page, opened through
`IBladeService` and closed through the blade lifecycle instead of navigating. Implementation + docs only —
no runtime visual verification was done. Plan: `.ai/plans/settings-as-blade.md` (now IMPLEMENTED),
record: `.ai/completed/settings-as-blade.md`.

- Shared: new `src/ISynergy.Framework.UI/ViewModels/BladeHostViewModel.cs` — `sealed class
  BladeHostViewModel : ViewModelBladeView<object>`, a shell-level blade owner with no page and no items
  (no-op `AddAsync`/`EditAsync`/`RemoveAsync`/`SearchAsync`). Registered as a singleton in
  `src/ISynergy.Framework.UI.WPF/Extensions/WPFAppBuilderExtensions.cs:108` and
  `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs:147`.
- WPF: new `src/ISynergy.Framework.UI.WPF/Controls/BladeHostView.xaml(.cs)` — resolves
  `BladeHostViewModel` from `Application.Services`, sets it as `DataContext`, hosts a `BladeView` bound to
  `Blades`; collapses + traces when the view model is not resolvable (design time).
  `samples/Sample.WPF/Views/ShellView.xaml:215` hosts it above the content frame and below the busy
  overlay. `samples/Sample.WPF/ViewModels/SettingsViewModel.cs` is now `ViewModelBlade<object>` (no
  `INavigationService`, no `GoBackAsync` cancel override); `ShellViewModel.OpenSettingsAsync` (:213) calls
  `_bladeService.OpenBladeAsync<SettingsViewModel, object>(_bladeHost)`. `SettingsView` keeps its layout and
  its Close button binds `CancelCommand` (CancelAsync → CloseAsync → `Closed` → `BaseBladeService.RemoveBlade`).
- MAUI: new `src/ISynergy.Framework.UI.Maui/Controls/BladeHostView.cs` — a `ContentView` that resolves
  `BladeHostViewModel` lazily (constructor + `Loaded` + `OnParentSet`) and attaches a `BladePane` with
  `ItemsSource = host.Blades`; hidden + traced when unresolvable. Declared as the last child of
  `InfoView.xaml:33`, `CustomControlsView.xaml:107`, `MauiControlsView.xaml:101`, `SlideShowView.xaml:43`,
  replacing the old per-page `SettingsDrawer`.
- MAUI sample: `samples/Sample.Maui/ViewModels/SettingsViewModel.cs` is now `ViewModelBlade<object>` with a
  `SaveCommand` (`SaveAsync`: save local settings → restart question → save global settings → `CloseAsync`)
  and the inherited `CancelCommand` for Close; the settings UI moved from the drawer into
  `samples/Sample.Maui/Views/SettingsView.xaml` (a `c:View` blade page with language/theme pickers, four
  accent swatches and a Save & close / Close footer). `SettingsDrawer.xaml(.cs)` and
  `Services/SettingsDrawerState.cs` were deleted together with their `MauiProgram` registration;
  `ShellViewModel.OpenSettingsAsync` (:174) and first run now open the blade through `IBladeService`
  against `BladeHostViewModel`.
- WinUI: no change beyond the earlier RTL work — `Sample.WinUI/ViewModels/SettingsViewModel.cs:18` is
  already `ViewModelBlade<object>`, opened by `ShellViewModel.OpenSettingsBladeAsync` (:206-212) through
  `IBladeService` into the shell's `c:BladeView` (`ShellView.xaml:160`) and closed by its `CloseCommand`.
- Blazor: out of scope — `ISynergy.Framework.UI.Blazor` has no blade infrastructure; its settings remain an
  inline drawer in `MainLayout.razor`.
- Docs: `src/ISynergy.Framework.UI.WPF/readme.md` and `src/ISynergy.Framework.UI.Maui/readme.md` gained a
  `### Shell-level pane` subsection, `src/ISynergy.Framework.UI.WinUI/readme.md` a shell-ownership sentence
  in `### BladeView`; `screenshots/README.md` MAUI rows now say "settings blade";
  `CHANGELOG.md` → `### Added` gained `#### Settings as a Blade — WPF, WinUI and MAUI`.

## Decisions Made
- **Shared `BladeHostViewModel` instead of re-basing the shells**: a new `ViewModelBladeView<object>` owner
  in `ISynergy.Framework.UI`, registered as a singleton, rather than deriving WPF/MAUI `BaseShellViewModel`
  from `ViewModelBladeView<NavigationItem>` like WinUI.
  - **Rationale**: changing a published base class is a public API break, and on MAUI it would drop
    `ViewModelNavigation`/`IShellViewModel` (WPF `BaseShellViewModel` derives from `ViewModel`).
  - **Alternatives considered**: per-sample host view models (rejected: three copies of the same owner);
    no shell owner, blades on the current page (rejected: the pane would vanish on navigation).
- **MAUI drawer replaced by an in-page blade pane**: the per-page `SettingsDrawer` became a
  `BladeHostView`/`BladePane` in the four tab pages, and the settings UI moved into a blade page
  (`Views/SettingsView.xaml`); `SettingsDrawerState` and the drawer control were deleted.
  - **Rationale**: the MAUI `Shell` root cannot host an overlay, and the drawer duplicated the blade UX the
    framework now provides.
  - **Alternatives considered**: keeping the drawer and adding a second settings surface (rejected: two
    implementations of the same screen).
- **WinUI left as-is**: it already owns the pane on the shell view model and opens Settings as a blade.
  - **Rationale**: nothing to gain from migrating a working shell to `BladeHostViewModel`; risk without
    benefit.
  - **Alternatives considered**: migrating WinUI for uniformity (deferred; not worth the churn).

## Open Questions
- **ASSUMED**: `BladeHostView`'s design-time/unregistered path (collapsed + traced) is only exercised in the
  designer; it has not been observed at runtime.
- **OPEN**: whether consumers want a shell-owned settings blade pre-registered instead of wiring
  `OpenBladeAsync<SettingsViewModel, object>(_bladeHost)` themselves.
- **ASSUMED**: the MAUI in-page pane overlays the page content correctly because `BladeHostView` is the last
  child; visual layering was not checked on a device.

## Files Modified
- `src/ISynergy.Framework.UI/ViewModels/BladeHostViewModel.cs` - Created, shell-level blade owner
- `src/ISynergy.Framework.UI.WPF/Extensions/WPFAppBuilderExtensions.cs:108` - Singleton registration
- `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs:147` - Singleton registration
- `src/ISynergy.Framework.UI.WPF/Controls/BladeHostView.xaml(.cs)` - Created, WPF pane host
- `src/ISynergy.Framework.UI.Maui/Controls/BladeHostView.cs` - Created, MAUI pane host
- `samples/Sample.WPF/Views/ShellView.xaml:215` - `BladeHostView` between content frame and busy overlay
- `samples/Sample.WPF/ViewModels/ShellViewModel.cs:213` - Settings opened through `IBladeService`
- `samples/Sample.WPF/ViewModels/SettingsViewModel.cs`, `Views/SettingsView.xaml(.cs)` - Converted to a blade
- `samples/Sample.Maui/ViewModels/SettingsViewModel.cs` - `ViewModelBlade<object>` + `SaveCommand`
- `samples/Sample.Maui/Views/SettingsView.xaml(.cs)` - Settings blade page (was a dead TabbedView demo)
- `samples/Sample.Maui/Views/InfoView.xaml:33`, `CustomControlsView.xaml:107`, `MauiControlsView.xaml:101`,
  `SlideShowView.xaml:43` - `BladeHostView` as last child
- `samples/Sample.Maui/ViewModels/ShellViewModel.cs:174` - Settings/first run open the blade
- `samples/Sample.Maui/Views/Controls/SettingsDrawer.xaml(.cs)`, `Services/SettingsDrawerState.cs` - Deleted
- `samples/Sample.Maui/MauiProgram.cs` - Drawer registration removed
- `src/ISynergy.Framework.UI.WPF/readme.md`, `src/ISynergy.Framework.UI.Maui/readme.md`,
  `src/ISynergy.Framework.UI.WinUI/readme.md`, `screenshots/README.md`, `CHANGELOG.md` - Documentation

## Conditional Branches
- **If the MAUI in-page pane does not overlay the page**: check the `BladeHostView` child index in the four
  tab pages — it must stay the last child of the root layout.
- **If a head needs a second shell-level pane owner**: register a second `BladeHostViewModel`-style type;
  do not make the singleton generic over owners.
- **If Blazor ever gets blades**: implement `IBladeService`/`IViewModelBladeView` there first; the Blazor
  settings drawer is untouched by this pass.

## Cross-Document Relationships
- `.ai/plans/settings-as-blade.md` (status IMPLEMENTED) is the plan this session executed; the record lives
  in `.ai/completed/settings-as-blade.md`.
- `BladeHostViewModel` (shared) is consumed by `BladeHostView` in both `ISynergy.Framework.UI.WPF` and
  `ISynergy.Framework.UI.Maui`; changing the owner's shape touches both hosts and both readmes.
- `CHANGELOG.md` → `#### Settings as a Blade — WPF, WinUI and MAUI`, the two `### Shell-level pane` readme
  subsections and the WinUI `### BladeView` sentence document the same contract.
- The MAUI settings blade relies on the adaptive `BladeService` (`.ai/completed/blade-pane-parity.md`):
  `DeviceInfo.Idiom == Phone` still presents the blade as a modal page.

## Next Actions (Priority Order)
1. Runtime check: open Settings from the WPF header gear and from the MAUI sidebar/menu — confirm the pane
   shows the settings card, the scrim closes it, and a second open re-creates it.
2. Runtime check: MAUI phone idiom — confirm Settings still presents as a modal page and returns cleanly.
3. Runtime check: live apply — language, theme and accent changes on screen, and the MAUI Save & close
   restart question.
4. Re-capture `screenshots/maui/*-settings-*.png` (the rows now read "settings blade").

## Files to Load Next Session
- `.ai/completed/settings-as-blade.md` - What was implemented and what remains unverified
- `src/ISynergy.Framework.UI/ViewModels/BladeHostViewModel.cs` - The shared shell-level owner
- `src/ISynergy.Framework.UI.Maui/Controls/BladeHostView.cs` - Lazy resolution + pane attachment

## Work Type for Next Session
Runtime UI verification (pane visuals, phone-idiom modal flow, live settings apply, screenshot refresh).

## Reason for Session End
- Task complete (implementation + documentation); runtime verification deferred.

## Session Notes
- Builds are clean: `ISynergy.Framework.UI` (`net11.0`), `UI.WPF` (`net10.0-windows10.0.26100.0`),
  `UI.Maui` (`net11.0-windows10.0.26100.0`), `UI.WinUI` (`net10.0-windows10.0.26100.0`, only the
  pre-existing `CsWinRT1032`), `Sample.WPF` (`net11.0-windows`), `Sample.Maui`
  (`net11.0-windows10.0.26100.0`, only the pre-existing `CS8601`), `Sample.WinUI`
  (`net11.0-windows10.0.26100.0`, x64).
- `tests/ISynergy.Framework.UI.Tests`: 90/90 pass. `git diff --check` clean.
- Nothing here breaks framework consumers; the removals (`SettingsDrawer`, `SettingsDrawerState`, the dead
  TabbedView settings page) are sample-only.
- NOT runtime-verified: pane visuals/scrim/animation, the MAUI phone modal branch, RTL rendering with the new
  host, and settings live-apply on screen.

## Follow-up (same session) — VS Code launch targets, hot reload, WPF dark-mode polish
- `.vscode/launch.json` + `.vscode/tasks.json`: added `Sample.WPF`, `Sample.WinUI` and `Sample.Maui` launch
  targets for **.NET 10 and .NET 11**, all **arm64** (WinUI `-p:Platform=arm64 -p:WindowsPackageType=None`
  so the coreclr debugger can start it; Maui `-p:RuntimeIdentifierOverride=win-arm64`). Build tasks pin the
  properties that make the output paths deterministic and keep `MSBUILDDISABLENODEREUSE=1` + `-nodeReuse:false`.
- Added `watch: Sample.WPF/.WinUI (.NET 10/11)` tasks (`dotnet watch run`, Ctrl+R works) and an
  `Attach: Sample (WPF / WinUI, dotnet watch)` configuration. Verified live: WPF applied a C# edit in ~1.1 s,
  WinUI in ~2.4 s. **Maui is intentionally excluded** — dotnet-watch's delta build compiles the
  iOS/MacCatalyst sources too and fails with a CS0518 avalanche.
- `tools/NugetUnlister/NugetUnlister.csproj`: `<TargetFrameworks>net10.0;net11.0</TargetFrameworks>`
  (was net11-only), which unblocked the WPF .NET 10 target (`NU1201`).
- WPF window chrome radius 4 → 8 in `ISynergy.Framework.UI.WPF/Application/Application.cs` (initial and
  restored) and `Sample.WPF/Views/ShellView.xaml.cs`; maximized stays 0.
- `Sample.WPF` dark-mode polish: the Help footer row now matches the navigation rows (52 px row, 36 px icon
  tile, `AppSidebarName` label) and `InfoView` uses `AppSettingsLabel` + the new `AppReadOnlyField` style
  (tokens: `AppControlBrush`/`AppBorderBrush`/`AppTextBrush`, 8 px corners) so labels and fields follow the theme.
- Note: VS Code's saved `debug.selectedconfigname` was an Aspire configuration, which is why F5 started the
  AppHost even when WPF was intended; pick `Sample.WPF (...)` in the Run and Debug dropdown once.

---

# Session Handoff - 2026-10-25 (Blade pane parity: WPF + MAUI + WinUI RTL)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Gave WPF and MAUI the WinUI "blades" (edge pane) interaction, made all three hosts RTL-correct,
and ported the selection views. Implementation only — no runtime visual verification was done.

- Shared: `src/ISynergy.Framework.UI/Enumerations/BladePaneAnchor.cs` (`Auto`/`Start`/`End`) and
  `src/ISynergy.Framework.UI/Helpers/BladePlacement.cs` (`Resolve`, `IsStartSide`, `HorizontalSign`;
  logical edge → physical edge, RTL-aware). 22 unit tests in
  `tests/ISynergy.Framework.UI.Tests/Helpers/BladePlacementTests.cs`.
- WPF: new `src/ISynergy.Framework.UI.WPF/Controls/BladeView/BladeView.xaml(.cs)` (`ItemsSource`,
  `Anchor`, `DisabledOpacity`, `BladeSpacing`, `BladeMaxWidth`, `InnerPadding`; `Page` blades hosted in a
  `Frame`; position container forced LTR; blade content mirrored from the ambient `FlowDirection`;
  slide-in direction from `HorizontalSign`). `Services/BladeService.cs` also resolves
  `ISingleSelectionViewModel`/`IMultipleSelectionViewModel`. New `Views/SingleSelectionView`,
  `Views/MultipleSelectionView`.
- MAUI: new `src/ISynergy.Framework.UI.Maui/Controls/BladePane.cs` (same bindable properties; renders each
  blade's `ContentPage.Content` because a page cannot be a child; registers itself in `IBladePaneHost`),
  `Services/IBladePaneHost.cs` + `Services/BladePaneHostRegistry.cs`, and an ADAPTIVE
  `Services/BladeService.cs` (overlay pane on tablet/desktop when a host is attached and the idiom is not
  `Phone`; otherwise `PushModalAsync` + pop on the blade's `Closed` while it is on top of the modal stack;
  falls back to the overlay with a warning when neither path is possible). New `Views/SingleSelectionView`,
  `Views/MultipleSelectionView`. Registered in `MauiAppBuilderExtensions.cs:144-145`.
- WinUI: `Controls/BladeView/BladeView.xaml(.cs)` gained `Anchor` (`BladePaneAnchor`, default `Auto`) and
  the read-only resolved `BladeAlignment` (`HorizontalBladeAlignment` kept as the LTR intent and mirrored);
  the `ListBox` is pinned to `FlowDirection=LeftToRight` and card content mirrors via `FlowDirection`.
  XAML `FlowDirection` is applied to the window root (`Application.OnLaunched:458` with
  `LocalSettings.Language`) and to window-level pages (`NavigationService.SetWindowContent:80` with
  `CultureInfo.CurrentUICulture`); `WS_EX_LAYOUTRTL` stays, via `WindowHelper.CreateWindow`, for
  non-client/caption chrome only. `FrameworkElementExtensions.ApplyFlowDirection` now takes `string?`
  (null/whitespace = LTR).
- Samples: `Sample.WPF` got a Blades page (`DetailsView`/`DetailView`/`DetailAView` + view models + the
  "Blades" nav entry in `ShellViewModel:125`); `Sample.Maui` converted its Details/Detail flow to blades
  with the pane inside `DetailsView.xaml:189`.
- Docs: `readme.md` for UI.WPF (`## Blades (edge panes)`), UI.Maui (`## Blades (edge panes)` + adaptive and
  RTL subsections), UI.WinUI (`Anchor`/`BladeAlignment` in `### BladeView` + `### Right-to-left`),
  and a `#### Blade Panes — WPF and MAUI Parity + RTL Anchoring` block under `CHANGELOG.md` → `### Added`.

## Decisions Made
- **RTL design rule — positioning container forced LTR, logical anchor resolved by `BladePlacement`**:
  the container that positions the pane (WPF `Grid`, MAUI `Grid`, WinUI `ListBox`) is explicitly
  `LeftToRight`, so `Left`/`Right` (or `Start`/`End`) inside it are *physical*; only the blade *content*
  inherits the ambient flow direction. `Anchor` stays logical and is resolved in one place.
  - **Rationale**: under RTL every platform mirrors the layout coordinate system, which makes an
    alignment applied inside a mirrored container ambiguous and made the WinUI control hardcode `Right`.
  - **Alternatives considered**: reading an explicit `IsRightToLeft` input from the language setting on
    every control (rejected: three places to keep in sync); letting the platform mirror everything
    (rejected: render transforms are never mirrored, so slide-in and shadows would go the wrong way).
- **MAUI adaptive presentation**: `BladeService` picks the overlay when `IBladePaneHost.IsHostAttached`
  and `DeviceInfo.Idiom != Phone`, otherwise pushes a modal page.
  - **Rationale**: a phone has no room for a stacked pane; the overlay needs a host in the owning page.
  - **Alternatives considered**: overlay everywhere (rejected: unusable on phone); modal everywhere
    (rejected: loses the WinUI parity the feature exists for).
- **WinUI keeps native `WS_EX_LAYOUTRTL` for chrome**: XAML `FlowDirection` mirrors app content, the
  native style remains for caption buttons and native title text.
  - **Rationale**: the two are not redundant — XAML content is composed outside GDI and is not mirrored
    by the window style.
  - **Alternatives considered**: dropping the native style entirely (deferred: needs an `ar-SA` visual
    check, which was not run).

## Open Questions
- **ASSUMED**: `WS_EX_LAYOUTRTL` and XAML `FlowDirection` do not double-mirror the content. The native
  style is applied in `WindowHelper.CreateWindow` and the XAML root carries `FlowDirection`, so a
  composition could in principle mirror twice — needs one `ar-SA` visual check on WinUI.
- **ASSUMED**: WPF `Page.Loaded` still fires for a blade hosted in a `Frame`, so
  `BaseBladeService.MonitorViewLoadAsync` completes on the WPF path.
- **OPEN**: whether a MAUI blade that loses page-level resources (only `ContentPage.Content` is
  reparented) is acceptable for consumers that put resources in page scope.

## Files Modified
- `src/ISynergy.Framework.UI/Enumerations/BladePaneAnchor.cs` - Created, logical pane anchor enum
- `src/ISynergy.Framework.UI/Helpers/BladePlacement.cs` - Created, RTL-aware edge/sign resolution
- `tests/ISynergy.Framework.UI.Tests/Helpers/BladePlacementTests.cs` - Created, 22 tests
- `src/ISynergy.Framework.UI.WPF/Controls/BladeView/BladeView.xaml(.cs)` - Created, WPF blade host
- `src/ISynergy.Framework.UI.WPF/Services/BladeService.cs` - Selection-view resolution added
- `src/ISynergy.Framework.UI.WPF/Views/SingleSelectionView.*`, `MultipleSelectionView.*` - Created
- `src/ISynergy.Framework.UI.WPF/Application/Application.cs` - RTL/root flow-direction handling
- `src/ISynergy.Framework.UI.Maui/Controls/BladePane.cs` - Created, MAUI blade host
- `src/ISynergy.Framework.UI.Maui/Services/IBladePaneHost.cs`, `BladePaneHostRegistry.cs` - Created
- `src/ISynergy.Framework.UI.Maui/Services/BladeService.cs` - Created, adaptive presentation
- `src/ISynergy.Framework.UI.Maui/Views/SingleSelectionView.*`, `MultipleSelectionView.*` - Created
- `src/ISynergy.Framework.UI.Maui/Extensions/MauiAppBuilderExtensions.cs:144-145` - DI registration
- `src/ISynergy.Framework.UI.WinUI/Controls/BladeView/BladeView.xaml(.cs)` - `Anchor`/`BladeAlignment`
- `src/ISynergy.Framework.UI.WinUI/Application/Application.cs:458` - Root `FlowDirection`
- `src/ISynergy.Framework.UI.WinUI/Services/NavigationService.cs:80` - Window-content `FlowDirection`
- `src/ISynergy.Framework.UI.WinUI/Extensions/FrameworkElementExtensions.cs:21` - `string?` overload
- `src/ISynergy.Framework.UI/Services/Base/BaseBladeService.cs` - Shared blade plumbing
- `samples/Sample.WPF/Views/DetailsView.xaml`, `DetailView.xaml`, `DetailAView.xaml` + view models,
  `samples/Sample.WPF/ViewModels/ShellViewModel.cs:125` - Blades page + nav entry
- `samples/Sample.Maui/Views/DetailsView.xaml:189`, `DetailView.xaml` + view models - Blade flow
- `src/ISynergy.Framework.UI.WPF/readme.md`, `src/ISynergy.Framework.UI.Maui/readme.md`,
  `src/ISynergy.Framework.UI.WinUI/readme.md`, `CHANGELOG.md` - Documentation

## Conditional Branches
- **If a `ar-SA` visual check shows content double-mirrored on WinUI**: drop `ApplyNativeFlowDirection`
  from `WindowHelper.CreateWindow` and keep XAML `FlowDirection` only.
- **If a MAUI blade must keep page-scoped resources**: stop reparenting `ContentPage.Content` and present
  the blade as a modal page on every idiom instead.
- **If a WPF blade needs page navigation inside it**: the per-blade `Frame` uses
  `JournalOwnership=OwnsJournal`; re-check journal leakage before adding a `NavigationService` to it.

## Cross-Document Relationships
- `BladeView` (WPF), `BladePane` (MAUI) and `BladeView` (WinUI) all depend on `BladePlacement` for the
  physical edge — never hardcode an edge in a host.
- `.ai/plans/blade-pane-parity.md` is the plan this session implemented; its step 9 (docs) is complete, so
  the file lives in `.ai/completed/blade-pane-parity.md`.
- `CHANGELOG.md` → `#### Blade Panes — WPF and MAUI Parity + RTL Anchoring` and the three readme sections
  document the same contract; change all four together.

## Next Actions (Priority Order)
1. Runtime check: WinUI `ar-SA` — confirm content mirrors once (not twice) with `WS_EX_LAYOUTRTL` active,
   and that `Anchor="Start"` puts the pane on the right.
2. Runtime check: MAUI phone idiom — open two blades, close the newest, confirm the modal stack stays
   consistent and the previous blade is re-enabled.
3. Runtime check: selection blades (`SingleSelectionView`/`MultipleSelectionView`) in WPF and MAUI — the
   ports were never opened at runtime.
4. Capture `screenshots/` for WPF/MAUI/WinUI in `en-US` and `ar-SA` with two stacked blades.

## Files to Load Next Session
- `.ai/completed/blade-pane-parity.md` - What was implemented and the remaining verification
- `src/ISynergy.Framework.UI/Helpers/BladePlacement.cs` - The single RTL rule all hosts use
- `src/ISynergy.Framework.UI.Maui/Services/BladeService.cs` - Adaptive presentation logic

## Work Type for Next Session
Runtime UI verification (RTL visual checks, phone-idiom modal flow, selection blades).

## Reason for Session End
- Task complete (implementation + documentation); runtime verification deferred.

## Session Notes
- Builds are clean: UI.WPF `net10.0-windows10.0.26100.0`, UI.Maui `net11.0-windows10.0.26100.0`,
  UI.WinUI `net10.0-windows10.0.26100.0`, Sample.WPF `net11.0-windows`, Sample.Maui `net11.0-windows`,
  Sample.WinUI `net11.0-windows` x64 — only pre-existing warnings remain.
- `BladePlacement` tests: 22/22 pass.
- Builds must use `MSBUILDDISABLENODEREUSE=1` and `-nodeReuse:false`.
- NOT runtime-verified: visual RTL (`ar-SA`) on all three heads, phone-idiom modal flow, selection-blade
  interaction, and `WS_EX_LAYOUTRTL` composition with XAML `FlowDirection`.

---

# Session Handoff - 2026-10-25 (MAUI DevFlow added to Sample.Maui)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Added MAUI DevFlow (agent) to `samples/Sample.Maui` so the app can be driven by
`maui devflow` (UI tree, screenshots, element interaction).

- `Directory.Packages.props`: `Microsoft.Maui.DevFlow.Agent` 0.1.0-preview.13.26512.5 (CPM, versionless project ref).
- `samples/Sample.Maui/Sample.Maui.csproj`: `PackageReference` added (standard MAUI, no Blazor/GTK packages).
- `samples/Sample.Maui/MauiProgram.cs`: `builder.AddMauiDevFlowAgent();` inside `#if DEBUG` only.
- `Platforms/MacCatalyst/Entitlements.plist`: added `com.apple.security.network.server` for the in-app HTTP server.

## Verified
- Debug `net10.0-windows10.0.26100.0` build (win-arm64 host): 0 errors, only the pre-existing CS8601.
- Runtime: app logged `[Microsoft.Maui.DevFlow] Agent started on port 9223`;
  `maui devflow agent status -ap 9223` -> app "Maui Sample", platform WinUI;
  `maui devflow ui tree --depth 1 -ap 9223` -> Window node.

## Notes / Not Done
- The DevFlow CLI broker is not running on this host, so `maui devflow diagnose`/`list` do not
  auto-discover the agent; use `-ap 9223` (or start the broker).
- No Apple build was run; the Mac Catalyst entitlement is unverified at runtime on this Windows host.
- Package ships net10 assets only but restores/builds for the net11 TFMs as well.

---

# Session Handoff - 2026-10-25 (Theme/language dialogs removed from all UI heads)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Removed the dialog-based theme/language flow from WPF, WinUI and MAUI (Blazor never had it).
`ThemeCommand` now toggles light/dark through `IThemeService`; language, theme and accent
selection moved into each app's settings surface.

- Deleted `ThemeViewModel`, `LanguageViewModel`, `IThemeWindow`, `ILanguageWindow`,
  `ILanguageViewModel`, the WPF/WinUI/MAUI `ThemeWindow` + `LanguageWindow`, the MAUI
  `RadioButtonToThemeConverter`, and `LanguageViewModelTests`.
- `BaseShellViewModel` (WPF/WinUI/MAUI): `LanguageCommand`/`OpenLanguageAsync`/`OpenThemesAsync`
  removed; `ThemeCommand` = `ToggleThemeAsync`; constructors now take `IThemeService`.
- Sample.WPF: new `SettingsView`/`SettingsViewModel` (language/theme/accent), `Settings` nav item,
  `OpenSettingsAsync` navigates to it; ad-hoc theme/language overrides removed.
- Sample.WinUI: `OpenSettingsAsync` and first run use `IBladeService.OpenBladeAsync<SettingsViewModel>`;
  language dialog/nav item removed; settings blade gained accent color.
- Sample.Maui: theme/language overrides removed, `ToggleThemeCommand` replaced by `ThemeCommand`
  (header + macOS View menu), `ThemeIcon` follows `IThemeService.ThemeChanged`.
- Readmes (UI, UI.WPF, UI.WinUI, UI.Maui) and CHANGELOG updated with the BREAKING migration note.
- Added the missing `BladeView` host to `Sample.WinUI/Views/ShellView.xaml` so the settings blade renders.
- Captured platform screenshots in `screenshots/` (WPF settings/shell, WinUI theme light/dark + settings blade, MAUI settings dark/light + shell).
- **Shell parity follow-up**: WPF shell rebuilt with plain WPF controls (Syncfusion `SfNavigationDrawer` removed) — sidebar with brand tile and navigation rows, breadcrumb header with theme/settings buttons, `Frame` host. WinUI shell gained the shared breadcrumb header, `PaneHeader` brand tile and tokens; theme/settings left the footer. All samples auto-authenticate the demo user and skip the login screen.
- **Footer**: WPF/WinUI/Blazor sidebars now show a Help action styled like the navigation rows plus the product version; MAUI already had this. Logout/Login was removed from the WPF/WinUI shell navigation.
- **Title bar/maximize pass**: platform-specific titles, WPF caption buttons flush right, WPF 4px corners only when restored, WPF normal maximize honors the work area, WinUI caption buttons follow the active theme.
- **Bugfix pass**: WPF live validation + working light/dark tokens; WinUI guarded settings close, aligned Help footer, breadcrumb follows selection; Blazor shell rebuilt with custom sidebar/header instead of FluentLayout.
- **Title bar**: OS title bars were removed from WPF/WinUI/MAUI. WPF uses `WindowChrome` + custom minimize/maximize/close buttons in `ShellView`; WinUI and MAUI Windows extend the shared header into the title bar with transparent native caption buttons; macOS hides the native title text in `AppDelegate` (traffic lights remain).

### Follow-up: unified sample styling (same session)
- Ported the `Sample.Maui` design language to all samples: shared surface/border/text/muted/accent
  tokens, Figtree typography, four accent swatches (teal/blue/green/orange) and the same settings
  layout; menu items remain platform-specific.
- Figtree bundled in WPF/WinUI/Blazor; default accent is now `#0B5D55` (`Sample.Shared` LocalSettings).
- Blazor gained the same settings drawer; fixed its missing `IThemeService` registration, live
  re-application and the `_content` interop path.
- 2026-10-25 screenshots regenerated for WPF, WinUI, MAUI and Blazor.

## Verified
- `dotnet build` (with `MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`):
  UI.WPF/UI.WinUI/UI.Maui (net10 windows) and Sample.WPF / Sample.WinUI (x64) / Sample.Maui
  (net11.0-windows) — 0 errors. Remaining warnings are pre-existing
  (`CsWinRT1032` in the builder extensions, `CS8601` on `CurrentRoute = route`).
- Styling pass: Sample.WPF (0 warnings), Sample.WinUI arm64 (0 errors), Sample.Maui windows
  (0 errors), Sample.Blazor (0 errors) — all rebuilt and run for the screenshots.
- `dotnet test tests/ISynergy.Framework.UI.Tests`: 68/68 passed.

## Not Done
- WPF persists the selected theme but keeps the system light control palette; the settings tokens
  are intentionally light to match the shell.
- No automated visual-regression tests; screenshots are manual captures.

## Files Modified
Framework: `src/ISynergy.Framework.UI/ViewModels/{Theme,Language}ViewModel.cs` (deleted),
`src/ISynergy.Framework.UI/Abstractions/Windows/IThemeWindow.cs` (deleted),
`src/ISynergy.Framework.Mvvm/Abstractions/ViewModels/ILanguageViewModel.cs` (deleted),
`src/ISynergy.Framework.Mvvm/Abstractions/Windows/ILanguageWindow.cs` (deleted),
`src/ISynergy.Framework.UI.{WPF,WinUI,Maui}/ViewModels/Base/BaseShellViewModel.cs`,
deleted platform windows, `src/ISynergy.Framework.UI.WinUI/ISynergy.Framework.UI.WinUI.csproj`,
the four readmes, `CHANGELOG.md`.
Samples: `Sample.WPF/{App.xaml, App.xaml.cs, Styles/Style.xaml, Assets/Fonts/Figtree-*.ttf, ViewModels/SettingsViewModel.cs, Views/SettingsView.xaml(.cs), ViewModels/ShellViewModel.cs}`,
`Sample.WinUI/{Styles/Style.xaml, Assets/Fonts/Figtree-*.ttf, ViewModels/SettingsViewModel.cs, ViewModels/ShellViewModel.cs, Views/SettingsView.xaml(.cs), Views/ShellView.xaml}`,
`Sample.Maui/{ViewModels/ShellViewModel.cs, Views/Controls/AppHeaderView.xaml, Views/ShellView.xaml.cs}`,
`Sample.Blazor/{Program.cs, Components/Layout/MainLayout.razor(.cs), wwwroot/site.css, wwwroot/fonts/*.ttf}`,
`Sample.WPF/{Views/ShellView.xaml(.cs), ViewModels/AuthenticationViewModel.cs, Sample.WPF.csproj, Assets/Fonts/FluentSystemIconsRegular.ttf}`,
`Sample.WinUI/{Views/ShellView.xaml(.cs), ViewModels/ShellViewModel.cs, App.xaml.cs}`,
`Sample.Shared/Models/LocalSettings.cs`,
`src/ISynergy.Framework.UI.WinUI/Styles/DynamicThemeResourceDictionary.cs`,
`src/ISynergy.Framework.UI.Blazor/Services/ThemeService.cs`.
Screenshots: `screenshots/{README.md,wpf/*.png,winui/*.png,maui/*.png,blazor/*.png}`.
Tests: `tests/ISynergy.Framework.UI.Tests/ViewModels/LanguageViewModelTests.cs` (deleted).

---

# Session Handoff - 2026-10-21 (Sample.Maui Xcode 27 / UIScene migration)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Migrated `samples/Sample.Maui` (iOS + Mac Catalyst heads) to the UIKit scene lifecycle required by Xcode 27 / Apple SDK 27.0, following the `maui-xcode27-migration` skill.

- Inventory found **no custom lifecycle code** (both `AppDelegate`s only override `CreateMauiApp`; no `ConfigureLifecycleEvents`, `OpenUrl`, `ContinueUserActivity`, `ShortcutItem`, no existing scene delegate/manifest) — the skill's fast path applied.
- Added `Platforms/iOS/SceneDelegate.cs` and `Platforms/MacCatalyst/SceneDelegate.cs` (`[Register("SceneDelegate")] : MauiUISceneDelegate`).
- Merged `UIApplicationSceneManifest` (single scene, `__MAUI_DEFAULT_SCENE_CONFIGURATION__`, delegate `SceneDelegate`) into both `Info.plist` files.
- Enabled the commented-out `NSPrivacyAccessedAPICategoryUserDefaults` / `CA92.1` entry in `Platforms/iOS/Resources/PrivacyInfo.xcprivacy` (app uses `Preferences.Default`).
- MAUI 10.0.120 already met the 10.0.110 floor — no package change.

## Decisions Made (user-approved)
- Mac Catalyst `SupportedOSPlatformVersion` 15.0 -> **17.0** (SDK 27 minimum; drops macOS 12/13). iOS stays 15.0.
- `global.json` gained `"workloadVersion": "10.0.401.1"` (published pairing for Apple 27.0.10722 + Xcode 27.0). The machine's workloads were updated to that set.
- No macOS CI job added; Apple build/device testing stays a manual Mac step.

## Verified
- `dotnet workload list` -> workload version 10.0.401.1, `ios`/`maccatalyst` 27.0.10722.
- Effective MSBuild properties: iOS 15.0, Catalyst 17.0, both `TargetPlatformVersion=27.0`.
- `dotnet build -f net10.0-ios -t:Compile` and `-f net10.0-maccatalyst -t:Compile`: 0 warnings / 0 errors; `SceneDelegate` present in both compiled assemblies.
- plist/privacy/entitlements parsed and asserted via `plistlib`; lifecycle audit searches had no hits.
- `git diff --check` clean.

## Open Questions / Not Done
- `audit-lifecycle.mjs` helper NOT run (no Node.js on this host) — manual step-5 searches were used instead.
- Built-bundle `Info.plist` check NOT possible here (Apple packaging needs Xcode; the `bin/.../Sample.Maui.app` is a stale zero-byte artifact from an older macOS build). Run `plutil -p <App>.app/Info.plist` on a Mac.
- No simulator/device validation: launch past splash, foreground/background, warm/cold links, auth return, quick actions all untested. Xcode 27.0 needs macOS 26.6+.
- `ios-arm64` device and Release Catalyst builds untested.
- **Not mine:** 16 `SKILL.md` files under `skills/` and `.ai/skills/` show as modified (frontmatter converted from a fenced JSON block to YAML). Left untouched.

## Files Modified
`samples/Sample.Maui/{Platforms/iOS/SceneDelegate.cs, Platforms/MacCatalyst/SceneDelegate.cs, Platforms/iOS/Info.plist, Platforms/MacCatalyst/Info.plist, Platforms/iOS/Resources/PrivacyInfo.xcprivacy, Sample.Maui.csproj}`, `global.json`,
`.ai/plans/maui-xcode27-migration.md`, `.ai/completed/maui-xcode27-migration.md`.

---

# Session Handoff - 2026-10-09 (EventSourcing Web JWT auth)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Fixed the `401 (Unauthorized)` the Blazor front-end (`Sample.EventSourcing.Web`) got when calling the API. Root cause: the auth-hardening commit `bc6aa648` switched the API to JWT bearer (fallback policy requires an authenticated user for everything but `/health`) but **never updated the Web**, which still sent only a spoofable `X-Tenant-Id` header and no token. No token issuer existed, so the Web had no way to authenticate.

Plan: `.ai/plans/eventsourcing-web-jwt-auth.md` (approved by the user). Implementation (delegated to two parallel writers):
- **API** — new `Services/DevTokenIssuer.cs` (mints HS256 JWTs from `Jwt:*` config via `JsonWebTokenHandler`; `tenant`/`username`/`sub` claims) and `Endpoints/DevTokenEndpoints.cs` (`POST /dev/token`, anonymous, **gated on `env.IsDevelopment()`** exactly like `/api/archive/run`). `Program.cs` registers the singleton + `app.MapDevTokenEndpoints()`. Readme updated.
- **Web** — new `Services/DevTokenProvider.cs` (typed `HttpClient`, caches token per tenant, semaphore-guarded, 60s expiry skew). `OrdersApiClient` now injects `DevTokenProvider`, sends `Authorization: Bearer` in `BuildRequestAsync`, and **drops `X-Tenant-Id`** (and its now-unused `TenantContext` ctor param). `Program.cs` registers the `DevTokenProvider` HttpClient on the Aspire base address. Readme updated.

## Verified
- `dotnet build` of both projects (with `MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`): **0 warnings, 0 errors**.
- `git diff --check` clean.
- End-to-end browser check NOT run (needs Docker for the AppHost Postgres/Azurite containers — not installed here). **The user should run the AppHost and load https://localhost:7201** to confirm the Orders page loads and "Place Order" works.

## Decisions Made
- Dev token endpoint is **Development-only** (mirrors the existing `/api/archive/run` gate); production exposure unchanged — so no council was convened. The signing key stays only in the API; the Web never sees it.
- The Web's username is hardcoded to `demo-user` for the demo (no real login).

## Open Questions / Not Done
- **Pre-existing, NOT mine:** `samples/Sample.EventSourcing.Api/appsettings.json` was edited (removed the `"SymmetricKeySecret": ""` line) outside this session's tooling — likely the user; left as-is.
- `azure-pipelines.yml` was already modified before this session began — not mine.
- `DevTokenProvider` is registered via `AddHttpClient<T>` (transient), so its per-instance cache is only reused within a component's lifetime, not across the app. Fine for the sample; make it scoped (via `IHttpClientFactory`) if a real cache is wanted.
- Real user authentication (Keycloak) remains out of scope.

## Files Modified
`samples/Sample.EventSourcing.Api/{Services/DevTokenIssuer.cs, Endpoints/DevTokenEndpoints.cs, Program.cs, readme.md}`,
`samples/Sample.EventSourcing.Web/{Services/DevTokenProvider.cs, Services/OrdersApiClient.cs, Program.cs, readme.md}`,
`.ai/plans/eventsourcing-web-jwt-auth.md`, `.ai/completed/eventsourcing-web-jwt-auth.md`.

---

# Session Handoff - 2026-10-09 (EventSourcing sample UserSecretsId)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Diagnosed the startup exception on `Sample.EventSourcing.Api`:
`InvalidOperationException: Jwt:SymmetricKeySecret is not configured` (`Program.cs:26`). The throw is an intentional fail-closed guard; the real defect was that **neither documented run path could supply the key**:
- The error message points at `dotnet user-secrets set ...`, but the project had no `<UserSecretsId>`, so that command fails with *"Could not find the global property 'UserSecretsId'"*.
- The Aspire path (`Program.cs:66-68` says "run through the AppHost") also fails: `samples/Sample.AppHost/Program.cs:21-25` attaches only the database/blob references to `eventsourcing-api`, never `Jwt__SymmetricKeySecret`.

**Fix (user's choice):** added `<UserSecretsId>25878716-9461-4861-a00e-d6d1deb2b082</UserSecretsId>` to `samples/Sample.EventSourcing.Api/Sample.EventSourcing.Api.csproj`, matching the sibling `Sample.MessageBus.*` samples. This makes `dotnet user-secrets` work for both the standalone and Aspire-hosted paths (the API runs as its own process and loads its own user secrets in Development).

## Verified
`dotnet user-secrets list --project samples/Sample.EventSourcing.Api` → exit 0, *"No secrets configured for this application."* (errored before the change). `git diff` shows only the one csproj line added.

## Open Questions / Not Done
- **The secret value is still unset** — the user must run `dotnet user-secrets set "Jwt:SymmetricKeySecret" "<32+ char key>" --project samples/Sample.EventSourcing.Api` before the sample starts. Intentionally not committed.
- **Separate gap, not fixed:** `Sample.EventSourcing.Web/Services/OrdersApiClient.cs` calls the API with no bearer token, but the API's authorization fallback policy (`Program.cs:49-52`) requires an authenticated caller for everything except `/health` — so Web→API calls will 401 even once the key is set.
- `sample-api-secrets` option (AppHost injecting a dev key) was offered but declined in favour of the UserSecretsId approach.

---

# Session Handoff - 2026-10-07 (Release Notes & Progress Tidy)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
- **CHANGELOG.md** — documented the whole review's changes under `[Unreleased]`: a new `### Changed` block (the breaking `Core → Globalization` layering change with migration note; Sentry OTLP migration; async/robustness; `NuGetAuditMode=all` + CI audit; the internal god-class decompositions), a `### Removed` entry (the breaking `MessageBoxImage`/`MessageBoxResult` → plural rename with migration), `### Fixed` entries (the `FileResult.File` CS0618 cleanup, `EventArchiver` resolution, S3 key sanitization, Sample.Maui note), a new `### Security` section (rotated-out sample telemetry credentials, sample tenant/auth, PII opt-in), and a `### Deprecated` entry (`FileResult.File`/`GetStream()`).
- **README.md** — corrected two inaccurate claims: "Advanced data access with repository pattern" → EF Core `DbSet<T>` primitives on the `DataContext`; "Native AOT / Trim … eliminate runtime reflection across all libraries" → the real status (trim enabled only for the platform UI packages; reflection-heavy APIs annotated; AOT a direction, not a guarantee).
- **`.ai/progress/` tidy** (per the user's steer) — moved the finished records to `.ai/completed/`: `async-command-exception-fix.md` and `theme-viewmodel-updates.md` (both `Status: DONE`) and `navigatemodal-pushmodal-refactor.md` (all steps checked incl. "Build and verify"; its stale `Status: IN PROGRESS` was flipped to `DONE`). Left `merge-globalization-resx.md` in progress — it still has an unchecked "Verify all files" step.

## Note
- **CHANGELOG/README are documentation only** — no rebuild needed; `git diff --check` clean.

---

# Session Handoff - 2026-10-07 (Review Leftovers)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Fixed the remaining review leftovers (`docs/FRAMEWORK_REVIEW_2026-10-07.md`). Full solution build: **0 non-`Sample.Maui` errors, 0 CS0618 warnings**; test sweep green; `git diff --check` clean. Nothing committed.
- **L1 `QueryDispatcher`** — reflection fallback no longer uses `dynamic`; `HandleAsync` is reflected once per handler shape (cache) and invoked via `MethodInfo` (unwrapping `TargetInvocationException`).
- **L2 `EventArchiver.CreateAggregate`** — resolution cached + deterministic (ordinal full-name order, ambiguity traced); `GetLoadableTypes` tolerates reflection load errors.
- **L3 `MessageBox*` enums** — `MessageBoxImage`/`MessageBoxResult` renamed to `MessageBoxImages`/`MessageBoxResults` across `Mvvm`, `UI.Blazor`, `AspNetCore.Blazor` and all references; enum files renamed. WPF's `System.Windows.MessageBox*` untouched.
- **CS0618 cleanup** — nine obsolete `FileResult.File` reads switched to `await GetFileAsync()` (a regression surfaced from the earlier High-issue `FileResult` deprecation).

## Decisions Made
- **L3 was a deliberately breaking public rename** (user chose the plural form over keeping the WPF-mirroring names).

## Open Questions
- **ASSUMED:** nothing committed this session.

## Files Modified
`src/ISynergy.Framework.CQRS/Dispatchers/QueryDispatcher.cs`,
`src/ISynergy.Framework.EventSourcing.Storage/Services/EventArchiver.cs`,
the `MessageBox*` enum files (renamed) + `Mvvm/Abstractions/Services/IDialogService.cs`,
`Mvvm/Messages/MessageBoxRequest.cs`, the platform `DialogService`/`ImageBrowser`/`FileService`/`CameraService`
files across `UI.WPF`/`UI.WinUI`/`UI.Maui`, samples (`Sample.Maui`, `Sample.WinUI`),
`.ai/completed/leftovers-fix.md`.

## Next Actions (Priority Order)
1. Commit the whole working tree (nothing from this engagement is committed).
2. **L3 is breaking** — update the CHANGELOG/a release note for the `MessageBox*` rename.

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- No committing was performed.

---

# Session Handoff - 2026-10-07 (Split God Classes)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Split the three god classes from `docs/FRAMEWORK_REVIEW_2026-10-07.md` §4.4. Full solution build: **0 non-`Sample.Maui` errors**; test sweep across 33 projects: **0 failures**; `git diff --check` clean. No public API changed. Nothing committed.
- **G1 `BaseAsyncRelayCommand`** — extracted the static pooling/timeout-cache machinery into a new internal `CommandExecutionResourcePool`.
- **G2 `MessengerService`** — extracted recipient storage into a new internal `MessengerRecipientRegistry` (+ top-level `WeakActionAndToken`).
- **G3 `RedBlackTree<T>`** — extracted the insert/delete balancing fix-ups into a nested `private static class Balancing`; class is now `partial`.

## Decisions Made
- **Deeper redesign, but no public API change.** The user selected the aggressive option; in practice every split preserved the public/protected surface (subclasses, `ICollection`, and the tree invariant tests unchanged). A transient CS1710 was fixed.

## Open Questions
- **ASSUMED:** nothing committed this session.

## Files Modified
`src/ISynergy.Framework.Mvvm/Commands/Base/{CommandExecutionResourcePool.cs (new),BaseAsyncRelayCommand.cs}`,
`src/ISynergy.Framework.Core/Services/{MessengerRecipientRegistry.cs (new),WeakActionAndToken.cs (new),MessengerService.cs}`,
`src/ISynergy.Framework.Core/Collections/{RedBlackTree{T}.Balancing.cs (new),RedBlackTree{T}.cs}`,
`.ai/completed/god-class-refactor.md`.

## Next Actions (Priority Order)
1. Commit the whole working tree (High + Medium + Low + god-class refactor all uncommitted).
2. Optional remaining items: fragile seams (`QueryDispatcher` `dynamic`, `EventArchiver.CreateAggregate` AppDomain scan); Mvvm `MessageBox*` enum naming.

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- No committing was performed.

---

# Session Handoff - 2026-10-07 (Fix Low Issues)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Addressed the Low issues from `docs/FRAMEWORK_REVIEW_2026-10-07.md`. Full solution build: **0 non-`Sample.Maui` errors**; test sweep across 33 projects: **0 failures**; `git diff --check` clean. Nothing committed.
- **R1 robustness** — `ActionQueuingBackgroundService` execution timer now swapped under a lock with the previous instance disposed (leak/race fixed); `SafeFireAndForgetExtensions` static fields are `private volatile`; the swallowed catches (`StringExtensions`, `DelayTimer`, `EventArchiver`, `NetworkUtility` ×2) log via `Trace`.
- **R2 enums/docs** — explicit values on `ActionTypes`/`QueueMessageActions`/`MonitorEvents`; placeholder `XXXX` XML docs replaced; `CommandDispatcher` documented.
- **R3 warnings/security** — 8 Moq expression-tree sites use `v!.ToString()!` (fixes CS8602; `?.` is illegal in expression trees); `S3StorageService.GetObjectKey` sanitizes `.`/`..`/separators.
- **R4 Sentry** — migrated to `Sentry.OpenTelemetry.Exporter` (`UseOtlp` + `AddSentryOtlpExporter`, extension in namespace `OpenTelemetry.Trace`); CS0618 gone.

## Decisions Made
- **God-class breakup excluded** (user, via ask): high risk, low value.
- Not done (still open Low items): fragile seams (`QueryDispatcher` `dynamic`, `EventArchiver.CreateAggregate` AppDomain scan); Mvvm `MessageBox*` enum naming.

## Open Questions
- **ASSUMED:** nothing committed this session.

## Files Modified
`src/ISynergy.Framework.Automations/BackgroundServices/ActionQueuingBackgroundService.cs`, `src/ISynergy.Framework.Core/Extensions/SafeFireAndForgetExtensions.cs`, `src/ISynergy.Framework.Core/Extensions/StringExtensions.cs`, `src/ISynergy.Framework.Core/Helpers/DelayTimer.cs`, `src/ISynergy.Framework.Core/Utilities/{NetworkUtility,CompareUtility,EnumUtility}.cs`, `src/ISynergy.Framework.EventSourcing.Storage/Services/EventArchiver.cs`, the three enums, `src/ISynergy.Framework.CQRS/Dispatchers/CommandDispatcher.cs`, `src/ISynergy.Framework.Storage.S3/Services/S3StorageService.cs`, three test files, `src/ISynergy.Framework.OpenTelemetry.Sentry/*` + `Directory.Packages.props`, `.ai/completed/low-issues-fix.md`.

## Next Actions (Priority Order)
1. Commit the whole working tree (High + Medium + Low fixes are all uncommitted).
2. Optional remaining Low items: fragile seams, Mvvm enum naming, god-class breakup.

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- No committing was performed.

---

# Session Handoff - 2026-10-07 (Fix Medium Issues)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Addressed the Medium issues from `docs/FRAMEWORK_REVIEW_2026-10-07.md`. Full solution build: **0 non-`Sample.Maui` errors**; test sweep across 33 projects: **0 failures**; `git diff --check` clean. Nothing committed.
- **M1 docs** — rewrote `.ai/project/architecture.md` to reality (real dependency graph, DI-registration coverage, actual trim/AOT status, new Known-Limitations note).
- **M2 layering (council-adjudicated)** — removed the `Core → Globalization` project reference. Moved `ILanguageService`, `GlobalizationConstants`, and the three date/JSON converters into the **Core** assembly keeping their `ISynergy.Framework.Globalization.*` namespaces; added a Core-owned `LanguageServiceProvider` ambient; `Globalization` now references Core and `TypeForwardedTo`-forwards the moved types. Five hub projects (`UI`, `AspNetCore.Blazor`, `AspNetCore.Globalization`, `Documents.Syncfusion`, `Geography`) gained explicit Globalization references (they previously relied on the transitive edge).
- **M3 async** — `async void LogIfFailed` → fault-only `ContinueWith`; `CancellationToken` added to `NetworkUtility`'s async methods and the `IActionService`/`IAutomationService` surface (+ impls/background services).
- **M4 PII** — `UserContextEnrichingLogProcessor.IncludeUserContext` (default false) gates the PII block; `SendDefaultPii=false` in the 4 samples.
- **M5 audit** — `NuGetAuditMode=all`; added a `dotnet list --vulnerable --include-transitive` CI step.
- **M6 sample auth** — `Sample.EventSourcing.Api` no longer trusts `X-Tenant-Id`/`X-User-Name`; requires an authenticated JWT and derives tenant/user from claims via `AddMultiTenancyIntegration`; CORS restricted to configured origins; `DemoTenantService` deleted.
- **M7 (NOT fixed)** — see below.

## Decisions Made
- **Council for M2** — 3 seats converged on "invert the edge + move shared types into Core + ambient seam"; record in `.ai/completed/medium-fixes.md`.
- **M7 cannot be done as specified.** `UI.Maui.Mac.Tests` is a self-running Microsoft.Testing.Platform app bundle; on .NET 10 `dotnet test` hard-errors when VSTest targets it. The documented opt-in (repo-wide `global.json` `test.runner=Microsoft.Testing.Platform`) was **verified to break every other (VSTest) test project**; project-level knobs (`TestingPlatformDotnetTestSupport`, `IsTestProject=false`) do not suppress the error. CI already excludes the project. Documented the constraint in the csproj; all experiments reverted.

## Open Questions
- **M7** may warrant the user's input: the only way to make it runnable is either a repo-wide migration of every test project to MTP, or leaving it excluded (status quo).
- **ASSUMED:** nothing committed this session.

## Files Modified
A large set under `src/` (Core/Globalization type moves + Automations/OpenTelemetry/Mvvm edits + 5 csproj refs), `samples/Sample.EventSourcing.Api/*`, `samples/*/appsettings.json`, `Directory.Build.props`, `Directory.Packages.props`, `azure-pipelines.yml`, `.ai/project/architecture.md`, `CHANGELOG`-adjacent docs, `.ai/completed/medium-fixes.md`. (`docs/FRAMEWORK_REVIEW_2026-10-07.md` is from the prior review session.)

## Next Actions (Priority Order)
1. Commit the working tree (High + Medium fixes uncommitted).
2. Decide M7 (repo-wide MTP migration vs. keep excluded).
3. Remaining review findings (Low items) are in `docs/FRAMEWORK_REVIEW_2026-10-07.md` §6.

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- `core.whitespace=cr-at-eol` was set locally so `git diff --check` treats CRLF correctly (see prior session note).
- No committing was performed.

---

# Session Handoff - 2026-10-07 (Fix 4 High Issues)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Started fixing the four **High** issues from `docs/FRAMEWORK_REVIEW_2026-10-07.md`. All four addressed; affected tests green. Nothing committed.
- **H1 credentials** — blanked the committed Sentry DSN and Azure Application Insights connection string in `samples/Sample.Api|WPF|WinUI/appsettings.json` (Sample.Maui was already blank) and documented env-var/user-secrets loading in `samples/Sample.Shared/readme.md`. **Key rotation is still the user's action.**
- **H2 Sample.Maui build** — user chose *document only*: added a "Known build issue (environmental)" banner to `samples/Sample.Maui/readme.md`; no build change.
- **H3 blocking-over-async** — `EventTrigger` `.Wait()` → `.SafeFireAndForget(...)`; `NetworkUtility` per-call `new HttpClient()` → one shared static `HttpClient`; `FileResult` gained async `GetFileAsync()` (an async *property* is illegal in C#) and `File`/`GetStream()` are now `[Obsolete]`.
- **H4 DbContextExtensions** — **deleted** the class and its tests (council-adjudicated breaking change), with migration guidance (readme rewritten to EF Core `DbSet` primitives) and a `[BREAKING]` CHANGELOG entry.
- **Verify** — `dotnet test` on the four affected projects: Core 572/0, Mvvm 262/2-skipped, Automations 19/4-skipped, EntityFramework 49/9-skipped; **0 failures**.

## Decisions Made
- **Council convened for H4** (breaking public API) per AGENTS.md. 3 seats, one cross-examination round. After cross-exam, 2/3 seats landed on **(A) remove outright** (matching the user's choice). Chair decided **(A)**; dissent recorded verbatim; record in `.ai/completed/high-issues-fix.md`.
  - **Dissent recorded:** "A deprecation with no enforced end is just removal deferred…" (Compatibility seat); "The rules prohibit the symbol, not merely its call sites…" (Rules seat).
- **`FileResult` async accessor is a method, not a property** — C# forbids `async` on property accessors (`CS0106`), so the user's requested "async File property" became `GetFileAsync()`.
- **`NetworkUtility.GetInternetIPAddress()`** (already `[Obsolete]`, still called by a sample) was left in place; flagged as a candidate for the same treatment.

## Open Questions
- **OPEN:** H4 is a deliberate breaking change — decide whether it needs a version bump / release note at publish time.
- **ASSUMED:** nothing committed this session.

## Files Modified
- `samples/Sample.Api/appsettings.json`, `samples/Sample.WPF/appsettings.json`, `samples/Sample.WinUI/appsettings.json` — telemetry values blanked
- `samples/Sample.Shared/readme.md` — telemetry config docs; `samples/Sample.Maui/readme.md` — known build issue
- `src/ISynergy.Framework.Automations/Triggers/EventTrigger.cs` — SafeFireAndForget
- `src/ISynergy.Framework.Core/Models/Results/FileResult.cs` — GetFileAsync + deprecations
- `src/ISynergy.Framework.Core/Utilities/NetworkUtility.cs` — shared HttpClient
- `src/ISynergy.Framework.EntityFramework/Extensions/DbContextExtensions.cs` — **deleted**
- `tests/ISynergy.Framework.EntityFramework.Tests/Extensions/DbContextExtensionsTests.cs` — **deleted**
- `tests/ISynergy.Framework.Mvvm.Tests/Models/FileResultTests.cs` — async-accessor tests
- `src/ISynergy.Framework.EntityFramework/readme.md`, `CHANGELOG.md` — docs/migration
- `.ai/completed/high-issues-fix.md` — progress + council record

## Next Actions (Priority Order)
1. Rotate the exposed Sentry DSN and App Insights key (now removed from the tree but still in git history).
2. Decide the three Medium items and the remaining review findings (`docs/FRAMEWORK_REVIEW_2026-10-07.md` §6).

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- No committing was performed.

---

# Session Handoff - 2026-10-07 (Whole-Framework Review)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Performed a whole-framework review across four dimensions (code quality, security, architecture, build/test health) and produced `docs/FRAMEWORK_REVIEW_2026-10-07.md`. No source was changed — review only.
- **Build/test baseline:** Release solution build → FAILED: 2 errors (both `MSB4018` in `samples/Sample.Maui`, MAUI Resizetizer `GenerateSplashAssets` / HarfBuzzSharp `MissingMethodException` — MAUI-workload tooling, not framework source), 18 warnings. All 48 `src/` projects compile clean; only 2 `src/` warnings (`CS0618` obsolete Sentry API in `OpenTelemetry.Sentry`). Per-project `dotnet test` → 2,837 passed / 29 skipped / 0 failed; `UI.Maui.Mac.Tests` errors on the .NET 10 VSTest/Microsoft.Testing.Platform incompatibility; `Mail.Microsoft365.Tests` all-skipped.
- **Top findings:** (High) committed Sentry DSN + Azure App Insights connection string in 4 git-tracked sample `appsettings.json`; blocking-over-async in `EventTrigger.cs:29`, `FileResult.cs:102`, `NetworkUtility.cs:75`; `new HttpClient()` per call; repo-style `DbContextExtensions` contradicting the framework's own rule; solution build break from Sample.Maui. (Medium) architecture.md overstates reality (Core→Globalization, AOT/trim claim unbacked — 0/48 AOT, ~17 projects with no DI extension); missing `CancellationToken`s in Automations/NetworkUtility; PII to telemetry (`SendDefaultPii=true` + `UserContextEnrichingLogProcessor`); `NuGetAuditMode=direct`; header-spoofable tenant in one sample. (Low) timer leak/race, mutable static state in `SafeFireAndForgetExtensions`, god classes, implicit serialized-enum values, placeholder XML docs.

## Decisions Made
- **Report artifact, not code changes.** The user chose (via scoping prompt) the full four-dimension review, a written report in `docs/`, and a real build+test baseline. Nothing was edited in `src/`/`tests/`/`samples/`.
- **Evidence-first:** every cited finding was reproduced with the working tree (`git grep`/direct reads); the two subagent outputs flagged "partial" by the host (architecture) were independently re-verified before being reported.

## Open Questions
- **ASSUMED:** the committed App Insights/Sentry keys are live-looking but their liveness was not verified — treat as compromised and rotate.
- **ASSUMED:** nothing was committed in this session (working tree shows only the new report as untracked).

## Files Modified
- `docs/FRAMEWORK_REVIEW_2026-10-07.md` — created; the review report (the deliverable)
- `.ai/completed/whole-framework-review.md` — created; completion record

## Next Actions (Priority Order)
1. Triage the report's High items: rotate committed telemetry credentials; fix/quarantine the Sample.Maui build break; remove blocking-over-async; replace per-call `HttpClient`; decide the fate of `DbContextExtensions`.
2. Decide whether to reconcile `architecture.md` to reality or fix the code (Core→Globalization, AOT/trim claim, DI-extension claim).

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- Scratch (test script/logs/summary) was written under `.ai/tmp/` and removed at the end; note `.ai/tmp/` is **not** gitignored (only `*.log` is), so only logs are auto-ignored there.
- No committing was performed.

---

# Session Handoff - 2026-10-07

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Fixed the EF Core row-limiting warning in the upstream framework helper `ToPage`:
- `src/ISynergy.Framework.EntityFramework/Extensions/QueryableExtensions.cs`: added an ordering-aware overload
  `ToPage<TEntity, TKey>(query, Expression<Func<TEntity,TKey>> orderBy, pageIndex, pageSize)` that applies `OrderBy`
  before `Skip`/`Take`; the legacy `ToPage(pageIndex, pageSize)` overload is kept but now documents that it is unordered
  and triggers `CoreEventId.RowLimitingOperationWithoutOrderByWarning`. Shared `pageIndex`/`pageSize` validation extracted
  to a private `ValidatePaging` helper.
- `src/ISynergy.Framework.EntityFramework/readme.md`: pagination sample now uses the ordered overload.
- Tests: `QueryableExtensionTests.cs` (+4 tests: ordered paging, ordering-before-paging, negative pageIndex, non-positive
  pageSize, null orderBy) and new `ToPageRowLimitingWarningTests.cs` (asserts the ordered overload builds `Take(Skip(OrderBy(...)))`
  and the legacy overload does not order).
- Verified: `MSBUILDDISABLENODEREUSE=1 dotnet test tests/ISynergy.Framework.EntityFramework.Tests -nodeReuse:false` → Passed 52, Failed 0, Skipped 9.
- **Follow-up (same session, user "fix them all")** — a repo sweep found three more instances of the same class and fixed them:
  `EventArchiver.ArchiveOldEventsAsync` (EF `Take` without `OrderBy` → added `.OrderBy(TenantId).ThenBy(AggregateType).ThenBy(AggregateId)`);
  `EnumerableExtensions.CountPages` (floor → ceiling division, now matches the EF overload); `EnumerableExtensions.ToPage`
  (wrong `nameof(pageSize)` → `nameof(pageIndex)`, plus an ordered overload for parity). Verified: Core.Tests 572/572,
  EventSourcing.EntityFramework.Tests 27/27, EventSourcing.Storage build 0 warnings/0 errors.
  Record: `.ai/completed/ef-row-limiting-followups.md`.
- Clean repo-wide hits confirmed not affected: sample controllers order before `Take`; `Last`/`LastOrDefault` hits are LINQ-to-objects/UI.

## Decisions Made
- **Additive fix, not a breaking change**: keep the unordered `ToPage` overload, add an ordered overload.
  - **Rationale**: the framework cannot know a caller's intended ordering, and changing/removing the existing
    signature would break downstream consumers.
  - **Alternatives considered**: making the unordered overload throw when no OrderBy is present (rejected — breaks
    existing callers and the existing `GetPageTest`); configuring `ConfigureWarnings` to ignore the warning (rejected —
    hides a genuine non-determinism bug and is a consumer concern).
- **Structural test instead of a runtime-warning test**: EF Core's InMemory provider does not raise
  `RowLimitingOperationWithoutOrderByWarning` (it is produced by the relational pipeline), so the test asserts the
  expression-tree shape (`OrderBy` precedes `Skip`/`Take`) rather than a captured log event.

## Open Questions
- **OPEN**: Two files show as modified in the working tree that are NOT part of this task and were not edited by this
  session — `Directory.Packages.props` (Aspire `13.6.0→13.6.1`, `Azure.Monitor.OpenTelemetry.Exporter` `1.9.0→1.10.0`,
  MSTest `4.4.1→4.5.1`) and `samples/Sample.AppHost/Sample.AppHost.csproj` (Aspire SDK `13.6.0→13.6.1`). Cause
  unidentified; they appear to be someone else's in-flight dependency bump. Still to be confirmed/committed/reverted by the user.
- **RESOLVED**: Repo-wide `git diff --check` was failing on `Directory.Packages.props` because the file is stored with
  CRLF endings (no `.gitattributes`, `core.autocrlf=true`), so added lines carried a `^M` reported as trailing whitespace.
  Per the user's choice, the file's endings were converted to LF (version content unchanged) so `git diff --check` now passes.
  `samples/Sample.AppHost/Sample.AppHost.csproj` was left untouched.
- **ASSUMED**: Nothing in this session has been committed.

## Files Modified
- `src/ISynergy.Framework.EntityFramework/Extensions/QueryableExtensions.cs` - ordered `ToPage` overload + validation helper, doc remarks
- `src/ISynergy.Framework.EntityFramework/readme.md` - pagination sample uses the ordered overload
- `tests/ISynergy.Framework.EntityFramework.Tests/Extensions/QueryableExtensionTests.cs` - ordered-overload + validation tests
- `tests/ISynergy.Framework.EntityFramework.Tests/Extensions/ToPageRowLimitingWarningTests.cs` - created; expression-tree ordering guarantee
- `.ai/plans/upstream-framework-ef-row-limiting-warning.md` - created; plan
- `.ai/completed/upstream-framework-ef-row-limiting-warning.md` - created; completed progress record
- `Directory.Packages.props` - line endings normalized CRLF→LF (content unchanged) so `git diff --check` passes — not this task's change
- `src/ISynergy.Framework.Core/Extensions/EnumerableExtensions.cs` - CountPages ceiling fix; ToPage param-name fix + ordered overload
- `src/ISynergy.Framework.EventSourcing.Storage/Services/EventArchiver.cs` - deterministic `OrderBy` chain before `Take`
- `tests/ISynergy.Framework.Core.Tests/Extensions/EnumerableExtensionsTest.cs` - CountPages/ToPage/ordered-overload tests
- `.ai/plans/ef-row-limiting-followups.md`, `.ai/completed/ef-row-limiting-followups.md` - created; follow-up plan + completed record

## Next Actions (Priority Order)
1. Decide what to do with the unrelated `Directory.Packages.props` / `Sample.AppHost.csproj` bumps, then commit the EF change.
2. Optional: add a relational-provider (SQLite) test fixture if a true runtime assertion of the row-limiting warning
   disappearing is wanted; the InMemory provider never raises it, so the `ToPage` and `EventArchiver` `ORDER BY`
   guarantees are currently verified structurally / by inspection only.

## Files to Load Next Session
- `.ai/completed/upstream-framework-ef-row-limiting-warning.md` - this task's record
- `src/ISynergy.Framework.EntityFramework/Extensions/QueryableExtensions.cs` - the changed code

## Session Notes
- Build rule honoured: node reuse disabled (`MSBUILDDISABLENODEREUSE=1`, `-nodeReuse:false`).
- No committing was performed.

---

<!-- Previous handoff preserved below -->

# Session Handoff - 2026-10-06

**Written By:** Claude Code
**Next Session May Use:** Any

## Task Summary
Applied the AI template upgrade (`/upgrade-template`, non-interactive) and reconciled the repo with the new template layout:
- 53 files added (skill `references/` folders, `book-to-skill/scripts/extract.py`, `validate-agents-md.py`); `.claude/settings.json` merged; `upgrade-template.py` self-updated. 35 changed template files kept the project version.
- Between the upgrade and the audit, `CLAUDE.md`, `DEEPSEEK.md`, `REASONIX.md` and `.ai/chains/*.chain.md` were deleted from the working tree and a new `AGENTS.md` appeared. Cause not identified; the upgrade scripts contain no code that deletes them. User chose to treat it as an intentional migration to `AGENTS.md`.
- `verify-config` run against `AGENTS.md`; findings fixed (below).

## Decisions Made
- **AGENTS.md is the instruction file**; CLAUDE.md / DEEPSEEK.md / REASONIX.md are not restored.
  - **Rationale**: user chose option 2 (intentional migration).
  - **Alternatives considered**: `git restore` the old files — rejected by the user. Their uncommitted edits were never in git and are likely lost.
- **Pi chains dropped**: removed `.ai/chains` from `sync-skills.py` and `validate-settings.py`; deleted `validate-pi.py` and removed it from `run-all-tests.sh`, `test_suite.py` and the three test docs. `.pi/skills` and `.pi/agents` junctions are still created by `sync-skills.py`.
- **Project rules 14–16** (persisted enums, package-range bounds, no secrets) now live in `.ai/project/preferences.md`, because `AGENTS.md` is template-owned and overwritten on upgrade. `AGENTS.md` roster gained `programmer` and `winui-dev`; that edit may be overwritten by a future upgrade.
- **settings.json**: removed the stale DEEPSEEK.md sync hook and the older duplicate `Stop` build hook (builds were running twice).
- **Test runners retargeted** from CLAUDE.md/REASONIX.md to AGENTS.md: `validate-structure.sh`, `validate-references.sh`, `run-all-tests.sh`, `test_suite.py`. The Reasonix suite was removed from both runners.

## Open Questions
- **RESOLVED**: Standards surfaces written (`COMPLIANCE.md` and the `## Standards Compatibility` block in `README.md`, provenance `validators=none`). `standards.md` row 1 was changed to `Aligned` (validator `—`) because this project holds no `docs/slices/…` or `docs/test-reports/…`; `validate-standards.py` passes and the full suite is 12/12. The override is recorded in `.ai/project/preferences.md` → Template Overrides and may be reverted by an interactive upgrade.
- **OPEN**: `~/.reasonix/` (config.json, locks, memory) was not inspected for leaked project config.
- **ASSUMED**: Nothing in this session has been committed.

## Files Modified
`.claude/settings.json`, `.ai/scripts/{upgrade-template,sync-skills}.py`, `.ai/tests/{validate-settings.py,validate-structure.sh,validate-references.sh,run-all-tests.sh,test_suite.py,README.md,TESTING-GUIDE.md,QUICK-REFERENCE.md}`, `.ai/tests/validate-pi.py` (deleted), `.ai/tests/validate-tokens.sh`, `.ai/reference/{task-execution,standards}.md`, `.ai/project/preferences.md`, `AGENTS.md`, `README.md` (standards block appended), new `COMPLIANCE.md`, plus the added template files. Nothing committed.

## Next Actions
- Review and commit the working tree (note the unexpected deletions of `CLAUDE.md`, `DEEPSEEK.md`, `REASONIX.md`, `.ai/chains/`).
- `bash .ai/tests/run-all-tests.sh` passes 12/12 as of this session; re-run after any upgrade.

## Files to Load Next Session
- `.ai/session-context.md` — this file
- `AGENTS.md` — instruction file
- `.ai/project/preferences.md` — includes rules 14–16

## Session Notes
- The auto-mode classifier blocks edits to `.claude/settings.json` and file deletion from the agent; those were done by the user running a script and `rm` via the `!` prefix.
- Earlier handoff (2026-07-15, trim-warning work) is superseded here. Its decisions are recorded in `.ai/completed/fix-trim-warnings-winui.md` and `.ai/completed/fix-remaining-warnings.md`; the local `development/main` commits from that work were still unpushed and may still need a push decision.
