# Session Handoff - 2026-11-11 (release tag permission + SDK pin)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Diagnosed the production release failure and fixed both CI problems it exposed.

- **Tag push rejected:** CI run on `main` (2026-10-27, build `v2026.11027.11355`) failed at
  `Create git tag` with `refusing to allow a GitHub App to create or update workflow
  .github/workflows/ci.yml without 'workflows' permission`. The push authenticated with
  `GITHUB_TOKEN`, which is a GitHub Actions App installation token; an App token may only create a
  ref whose commit touches `.github/workflows/` when it holds the `workflows` permission, and no
  `permissions:` key can grant that to `GITHUB_TOKEN`. Packages were already on nuget.org when the
  step failed.
- **SDK pin ignored:** `global.json` pins `11.0.100` (GA, `rollForward: disable`), but the build and
  release jobs reported installing `11.0.100-rc.3.26522.148` — `setup-dotnet`'s `global-json-file`
  input was trusted without ever checking what `dotnet` resolved.
- **Fixes:** the release job checks out with `RELEASE_TOKEN` (fallback `GITHUB_TOKEN`) and passes it
  to `gh release create`; both jobs read `sdk.version` from `global.json`, install it as an explicit
  `dotnet-version`, and fail when `dotnet --version` disagrees; `New-GitTag.ps1` turns the
  App/`workflows` refusal into an actionable error naming `RELEASE_TOKEN`.
- **Verification:** `python .ai/tests/validate-release.py` passes; the workflow parses with both
  jobs carrying read → install → verify; `New-GitTag.ps1` was exercised in throwaway repos against a
  remote that refuses with the GitHub message (actionable error) and against an accepting remote
  (tag created and pushed).
- **Follow-up — self-contained sign tool:** `AzureSignTool.Cli` `2026.11111.11200` (like
  `2026.11111.11120`, and unlike `2026.11030.*` and older) is a self-contained-per-RID release
  (pointer package + `AzureSignTool.Cli.<rid>`, each RID package bundling
  `System.Private.CoreLib.dll`, verified in the tool repo's `ci.yml` and in the published package).
  The pin moved to it
  and both jobs dropped their `Install .NET 10 SDK and runtime for AzureSignTool` step — `build`
  never installed the tool at all, every test project targets `net11.0`, and `global.json`'s
  `rollForward: disable` meant SDK 10 was never selected. Signing now needs no runtime beyond the
  bundled one. The `release` job installs no .NET at all: `dotnet tool install` and
  `dotnet nuget push` run from `RUNNER_TEMP` against the CLI the `windows-2025` image already
  provides, so `global.json` (which exists for building) is never consulted there. The tool is
  installed into `RUNNER_TEMP/signing-tool` and the step asserts the version, so an older copy on a
  persistent runner (`2026.11027.11159`) cannot sign a release.

## Decisions Made
- **Fine-grained PAT (`RELEASE_TOKEN`, Contents + Workflows read/write)** rather than a GitHub App
  installation token: least privilege, one repository, nothing extra to maintain. Classic
  `repo` + `workflow` PAT also works. Recorded in `docs/MIGRATION_GUIDE.md` Step 11.1 item 6.
- **Keep `GITHUB_TOKEN` as the fallback**: PR/acceptance runs never tag, and a repository without
  the secret still releases commits that do not change a workflow file.
- **Keep tag creation after the NuGet push**: a re-run replays the same build number and
  `dotnet nuget push --skip-duplicate` makes it recoverable.
- **Install the SDK version explicitly and verify it** instead of relying on `setup-dotnet`'s
  global.json resolution.

## Open Questions
- **Updated 2026-11-11:** the `production` run on `main` (`42943664544`, pre-fix workflow) completed
  successfully at ~13:5x and created `v2026.11111.10206` **with `GITHUB_TOKEN`**, so the App
  `workflows` refusal seen on 2026-10-27 did not recur. `RELEASE_TOKEN` is therefore a guarantee
  for the tag step, not a prerequisite; add it if the refusal comes back.
- **NOT RUN:** the workflow changes are exercised only by CI runs; the PR is #177.
- `v2026.11027.11355` (2026-10-27) is still the one release whose tag step failed; its packages are
  on nuget.org but no tag/release exists.

## Files Modified
- `.github/workflows/ci.yml` - `RELEASE_TOKEN` for checkout/tag/release; read → install → verify
  SDK steps in `build` only (the `release` job installs no SDK); `AzureSignTool.Cli` pinned to
  `2026.11111.11200` and installed into `RUNNER_TEMP/signing-tool` with a version assertion; both
  .NET 10 install steps removed
- `scripts/New-GitTag.ps1` - actionable push failure for the `workflows`-permission refusal
- `docs/MIGRATION_GUIDE.md` - `RELEASE_TOKEN` documented in Step 11.1; self-contained tool in 11.2
- `.ai/project/tech-stack.md` - SDK line moved to `11.0.100` GA
- `.ai/plans/{release-tag-token-and-sdk-pin,self-contained-sign-tool}.md` and their
  `.ai/completed/` counterparts

## Next Actions (Priority Order)
1. Add the `RELEASE_TOKEN` secret (fine-grained PAT: Contents + Workflows read/write).
2. Cancel the waiting `CI` run on `main` (its workflow file predates the fix) and let the next push
   release with the fixed workflow.
3. Backfill the missing `v2026.11027.11355` tag and GitHub release.
4. Watch the first run after these changes: the build no longer installs a .NET 10 SDK, and signing
   now relies on the self-contained `AzureSignTool.Cli` package.

---

# Session Handoff - 2026-11-09 (build warnings + SonarCloud)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Created branch `fix/build-warnings-and-sonar` and cleared every warning from CI run 42532046072,
job 128895030909, plus the SonarCloud analysis error that failed it.

- **SonarCloud error:** step 16 failed with
  `Invalid regular expression 'src/ISynergy.Framework.Mathematics/**' for setting
  'sonar.issue.ignore.allfile'`. The value is not in the repository: the SonarCloud settings API
  returns it with `parentOrigin: INSTANCE`, i.e. an organization-level "ignore all issues on files"
  entry holding a glob, which scanner 12 now validates as a regex. `ci.yml` overrides the setting
  per project with `src/ISynergy\.Framework\.Mathematics/.*`. The organization setting should also
  be corrected in the SonarCloud UI.
- **Warnings:** 544 distinct locations in the Release build, 272 in the Mathematics port and 272
  elsewhere. Fixed all 272 non-Mathematics locations; the port is untouched and its diagnostics
  are suppressed in `ISynergy.Framework.Mathematics.csproj` (documented; mirrors the Sonar
  exclusions). Generated `obj/` sources are excluded from analyzers in `.editorconfig`.
- **GitHub Actions:** `checkout`, `setup-dotnet`, `setup-java`, `upload-artifact` and
  `download-artifact` moved from v4 to v5 (Node 24), removing both job-level deprecation warnings.
- **Verification:** Release solution build with the Sonar analyzer injected has 0 Sonar/compiler
  warnings outside `tests/`; the full test suite passes (Sample.Blazor.UI.Tests 36/36).

## Decisions Made
- **Fix warnings in code, suppress only where a fix breaks a public contract**: S3881 on
  finalizer-free types is suppressed with a comment because S3971 forbids `GC.SuppressFinalize`;
  S101/S110/S2436/S4035/S1133 on public API, P/Invoke structs and DNS constants are suppressed;
  everything else is a real change (S3776 refactored into helpers).
- **Fix the Math.NET port instead of suppressing it**: the user chose to resolve all 272 findings in
  the port, including overload layout and public API changes. The project-level `NoWarn` was removed.
- **Do not implement `IDisposable` on the Blazor dialog components**: doing so made Blazor dispose
  the reused view model on close and broke reopening the dialog. The cleanup method is renamed
  `DetachViewModel` instead (S2953 fix without a lifetime change).

## Open Questions
- **OPEN**: correct the organization-level `sonar.issue.ignore.allfile` value in SonarCloud; the
  repository-side override only covers this project.
- **OPEN**: `tests/` still emits ~1 500 analyzer warnings locally (not part of the CI run; the
  scanner excludes test projects). Treat as a separate cleanup.
- **ASSUMED**: the local `XA0033` warning is environmental (local JDK 25); CI installs JDK 21.
- **OPEN**: the Mathematics public API changes are breaking; release notes must call them out
  (see `CHANGELOG.md`).

## Files Modified
- `.github/workflows/ci.yml`, `.github/workflows/validate-template.yml` - Sonar override, action majors
- `.editorconfig` - no analyzers on `obj/`
- `src/ISynergy.Framework.Mathematics/ISynergy.Framework.Mathematics.csproj` - documented NoWarn
- 170 files under `src/`, `samples/`, `performance/`, `tools/` - per-warning fixes
- 104 files under `src/ISynergy.Framework.Mathematics/` - full port cleanup (0 warnings)
- 4 test files - renamed APIs (`Mat`/`Vec`/`Cdf97`, `FieldNames()`), Octave field initializers
- `CHANGELOG.md` - Fixed entries
- `.ai/plans/build-warnings-and-sonar.md`, `.ai/progress/build-warnings-and-sonar.md`

## Next Actions (Priority Order)
1. Push the branch and let CI run; confirm the SonarCloud end step succeeds.
2. Correct the SonarCloud organization setting (regex, not glob).
3. Decide whether to clean the test-project analyzer warnings.

---

# Session Handoff - 2026-11-08 (phone layouts: iOS and Android)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Checked the phone form factor (the existing set covered desktop/tablet only) and fixed the phone
content layout bug it exposed.

- **Bug:** `ShellView.SetUpPhoneDrawer` set `ColumnSpan=2` on `ContentHost` but left it in column 1
  (its desktop column), so on phones the content started after the 300pt sidebar column and
  overflowed the right edge. iOS and Android screenshots showed clipped fields.
- **Fix:** `Grid.SetColumn(ContentHost, 0)` (and the scrim) before the spans in
  `samples/Sample.Maui/Views/ShellView.xaml.cs`.
- Captured `10-shell-ios.png`, `11-settings-ios.png` (iPhone 17 simulator, 402×874 pt / 1206×2622,
  dark system theme) and `12-shell-android-phone.png`, `13-drawer-android-phone.png`,
  `14-settings-android-phone.png` (Pixel 6, 1080×2400, density 420, light theme). Both heads open
  the settings blade on first run (`GlobalSettings.IsFirstRun`); relaunch shows the shell.
- **iOS build caveat:** the framework library's `net10.0-ios` target wants
  `Microsoft.iOS.Runtime.iossimulator-arm64.net10.0_26.0`, which is not installed (only the .NET 11
  iOS workload is). The simulator build succeeded after temporarily targeting net11 only for
  `ISynergy.Framework.UI.Maui`; that csproj edit was reverted (only the earlier macOS package fix
  remains). A normal iOS build on a machine with only .NET 11 workloads needs the .NET 10 iOS
  workload, or a `SkipNet10Mobile`-style switch.
- The Android drawer/settings scrim rendered opaque black in the headless emulator: the `BoxView`
  painted the translucent colour opaque. Replaced the sample's `DrawerScrim` and the framework
  `BladeView` scrim with layout backgrounds (`Grid` + alpha), so the page behind the drawer and
  the settings blade is dimmed at full width.
- The phone header also had a 16pt left gutter that pushed the hamburger right of the page labels;
  removed it and added a -6 phone-only margin on the menu button, so the hamburger glyph lines up
  with the 16pt page labels. The desktop/tablet header stays at the content edge.
- **Drawer chrome (LTR/RTL):** the phone drawer card is flush now - LTR: no top/left/bottom gap,
  rounded on the right only; RTL: no top/right/bottom gap, rounded on the left only.
  `SidebarView.ApplySidebarChrome` sets the margin/shape, `ShellView.ApplyMenuState` flips the
  closed translation, and `AppHeaderView.UpdateFlowChrome` flips the header padding/nudge.
- **Desktop sidebar:** the same chrome now applies to the desktop/tablet column, not just the phone
  drawer: `SidebarView.ApplySidebarChrome` runs for every head in the `ShellView` constructor and on
  `FlowDirection` changes, so the sidebar is flush top/outer/bottom with the rounding on the inner
  edge only (right LTR, left RTL). The macOS/Catalyst/tablet screenshots were recaptured. The
  WPF/WinUI/Blazor samples use their own sidebar controls and keep the floating card.
- **Blade width:** the blade card was content-sized, so the native Pickers made the macOS/Catalyst
  blade narrower (the macOS settings blade clipped the fourth swatch). `BladeMaxWidth` (480) now
  sets both the card's minimum and maximum (`CreateCard`/`ApplyCardLayout`), so the settings blade
  is the same 480 width on every MAUI head. `07`, `08`, `09` recaptured. The 01/02 Windows captures
  still show the earlier 520px blade (no Windows host here).
- Captured `13-drawer-android-phone.png` (LTR) and added `15-drawer-android-rtl.png` (after
  switching the app language to Arabic and restarting); `06`, `09`, `10`, `12` were updated for the
  earlier scrim/header fixes. `11`/`14` are unchanged (full-screen settings pages with no
  header/scrim). The iOS drawer was not captured: the DevFlow mutation lease blocks and `simctl`
  has no tap; the Android drawer demonstrates the shared drawer code.
- Recaptured the macOS and Mac Catalyst set after the final changes:
  `04-shell-macos.png` / `07-settings-macos.png` (2560×1440, visually unchanged) and
  `05-shell-maccatalyst.png` / `08-settings-maccatalyst.png` (2660×1996 at the current 1330×998 pt
  window). The temporary in-app capture hook and its `ShellView` call were removed; both heads
  rebuild clean.

## Decisions Made
- **Fix the phone column in `SetUpPhoneDrawer`**, not the layout declarations: ContentHost keeps
  its desktop column and is reset only for the phone path.
- **Layout backgrounds for the scrims** instead of `BoxView`: Android painted the translucent
  `BoxView` opaque, which turned the drawer/settings background black.
- **Phone-only -6 margin** on the menu button so the 44px glyph lines up with the page labels; the
  desktop/tablet header keeps the content edge.
- **Add the mobile shots to the set** (10–14) and keep the desktop/tablet shots.
- **Do not add a permanent net10 skip switch now**; document the iOS pack caveat instead.

## Open Questions
- **OPEN**: add a `SkipNet10Mobile` property (library) + `AdditionalProperties` on the sample's
  project reference so net11-only builds do not restore the net10 mobile packs.
- **OPEN**: iOS drawer capture needs a tap-capable harness (DevFlow mutation lease / no simctl tap).

## Files Modified
- `samples/Sample.Maui/Views/ShellView.xaml.cs` - phone content column, RTL drawer translation
- `samples/Sample.Maui/Views/ShellView.xaml` - drawer scrim as a layout background
- `samples/Sample.Maui/Views/Controls/SidebarView.xaml`, `.xaml.cs` - drawer card chrome
- `samples/Sample.Maui/Views/Controls/AppHeaderView.xaml`, `.xaml.cs` - phone gutter/nudge + RTL
- `src/ISynergy.Framework.UI.Maui/Controls/BladeView.cs` - blade scrim as a layout background; fixed 480 card width
- `screenshots/maui/04`, `05`, `07`, `08` - macOS/Catalyst recaptures
- `screenshots/maui/06`, `09`, `10`, `12`, `13` - updated mobile/tablet captures
- `screenshots/maui/15-drawer-android-rtl.png` - RTL drawer capture
- `screenshots/README.md` - mobile rows and capture notes
- `.ai/session-context.md` - this handoff

## Next Actions (Priority Order)
1. Review the updated phone/tablet captures.
2. Decide on the net10 skip switch for iOS/Android local builds.
3. Commit the working tree (this fix, the header fix, the Mac Catalyst entitlements fix, the
   screenshots and the `net*-macos` package fix).

---

# Session Handoff - 2026-11-08 (macOS header alignment)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Aligned the macOS/AppKit shell header with the sidebar and removed the extra header-to-content gap.

- **Cause:** every head uses the same header/sidebar padding and margins. On AppKit the
  `AppHeaderView` `ContentView` was measured 112 high (the 48px `HeaderBar` plus a 64px inset) and
  centred the bar in that slot; the header row was 112, so the header content sat below the sidebar
  top and `NavigationHostView` started at y=144 instead of 80. Catalyst measured the same XAML at
  48.
- **Fix:** `HeightRequest="48"` on the root `ContentView` in
  `samples/Sample.Maui/Views/Controls/AppHeaderView.xaml` (with a comment); the shared header
  control owns its height, so all heads measure 48 without a shell-level or per-head workaround.
  The shell XAML and its padding/margins are unchanged.
- **Verified:** macOS `AppHeaderView` y=32/h=48, `NavigationHostView` y=80, `SidebarView` y=32
  (was 144/112); Mac Catalyst unchanged (y=0/h=48/nav 48). Recaptured
  `screenshots/maui/04-shell-macos.png` and `07-settings-macos.png` with the fixed layout; the
  temporary in-app capture hook was removed again and the clean macOS build succeeds.

## Decisions Made
- **Pin the header slot in the sample**, not change the framework safe-area handling: local,
  reversible, and the same 48px height as the other heads.
  - **Alternatives considered:** remove the page safe area on AppKit (rejected: would put the
    header under the traffic lights); change the framework `ContentView` (rejected: broad blast
    radius for a sample layout issue).
- **Recaptured only macOS**: the Catalyst/Android headers were already 48, so 05/06/08/09 stay.

## Open Questions
- **OPEN**: the macOS settings blade is content-sized and still clips the fourth accent swatch;
  the new `07-settings-macos.png` shows it. A `MinimumWidthRequest` or wider blade is the candidate.
- **OPEN**: the AppKit head still prints `Could not find System.Security.Permissions` at startup.

## Files Modified
- `samples/Sample.Maui/Views/Controls/AppHeaderView.xaml` - header control height + comment
- `screenshots/maui/04-shell-macos.png`, `07-settings-macos.png` - recaptured
- `.ai/session-context.md` - this handoff

## Next Actions (Priority Order)
1. Review the macOS header in the running app and the updated captures.
2. Decide whether to fix the macOS blade width; then commit the working tree (this fix, the Mac
   Catalyst entitlement fix, the screenshots and the `net*-macos` package fix).

---

# Session Handoff - 2026-11-08 (Mac Catalyst direct-launch crash)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Fixed the macOS crash report for `Sample.Maui` when the Mac Catalyst Debug bundle is launched
directly (debugger/terminal or the agent's shell).

- **Root cause:** the Debug bundle carried `com.apple.security.app-sandbox`. Starting a sandboxed
  app as a child of a sandboxed process aborts in `libsecinit` with
  `SYSCALL_SET_USERLAND_PROFILE` (`SIGTRAP`) before managed code runs. The 09:31 crash report is
  that abort; the 09:40 report was the temporary screenshot hook (removed).
- **Fix:** new `samples/Sample.Maui/Platforms/MacCatalyst/Entitlements.Debug.plist` (network
  client/server + `get-task-allow`, no App Sandbox), selected by `CodesignEntitlements` when
  `Configuration=Debug`; Release keeps `Platforms/MacCatalyst/Entitlements.plist`.
- **Verified:** `codesign -d --entitlements -` shows no `app-sandbox` and does show
  `get-task-allow`; a direct executable launch stays running with no new crash report; the Release
  evaluation still resolves `CodesignEntitlements` to `Platforms/MacCatalyst/Entitlements.plist`.
- Debug data/preferences now live outside the App Sandbox container; Release is unchanged.

## Decisions Made
- **Debug-only sandbox removal** instead of changing the launch flow. It keeps the existing
  F5/`coreclr` launch working from any parent and keeps the Release sandbox for distribution.
  - **Alternatives considered:** launch only via `open`/the MAUI extension (rejected: direct F5
    from a sandboxed parent would keep crashing); disable code signing (rejected: unsigned bundles
    are killed on Apple Silicon).

## Open Questions
- **OPEN**: the macOS AppKit head still prints `Could not find System.Security.Permissions` at
  startup (benign runtime loader warning). Add the `System.Security.Permissions` package if it
  should be silenced.
- **ASSUMED**: the reported crash was the Mac Catalyst direct launch; the final direct launch
  produced no crash report.

## Files Modified
- `samples/Sample.Maui/Platforms/MacCatalyst/Entitlements.Debug.plist` - new Debug entitlements
- `samples/Sample.Maui/Sample.Maui.csproj` - Debug `CodesignEntitlements`
- `samples/Sample.Maui/readme.md` - documents the Debug/Release split
- `.ai/session-context.md` - this handoff

## Next Actions (Priority Order)
1. Run/debug Mac Catalyst directly and confirm no crash report.
2. Commit this fix together with the screenshots and the macOS `net*-macos` package fix already in
   the working tree.

---

# Session Handoff - 2026-11-08 (non-Windows screenshots after the UI alignment)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Refreshed the MAUI screenshots for the non-Windows heads and added per-head settings captures.
User chose: macOS, Mac Catalyst, Android; shell + settings blade.

- Replaced `screenshots/maui/04-shell-macos.png`, `05-shell-maccatalyst.png`, `06-shell-android.png`.
- Added `screenshots/maui/07-settings-macos.png`, `08-settings-maccatalyst.png`, `09-settings-android.png`.
- Captured from the .NET 11 heads: macOS/AppKit (dark, 2560×1440), Mac Catalyst (light, 3742×2050),
  Android (API 37 arm64, 1280×800 at density 160, light, 1280×800).
- macOS/Catalyst used a **temporary** in-app capture hook (`NSView.CacheDisplay` /
  `UIGraphicsImageRenderer`) because `screencapture` and CUA window capture are blocked in this
  environment. The hook, the `ShellView` call and the bundle trigger were removed; both heads
  rebuild green without it.
- Android was deployed with `dotnet build -f net11.0-android -t:Run` (fast deployment). A plain APK
  install crashes at CoreCLR init (assemblies missing) - do not install the Debug APK directly.
- `screenshots/README.md`: rows 07–09, API 37, capture method, theme notes; `BladePane` typo fixed
  to `BladeView`.

## Decisions Made
- **Temporary hook, not committed.** Desktop screen capture is unavailable; the hook is scratch and
  was removed after the captures.
- **Android density 160** in addition to `wm size 1280x800`, otherwise the Pixel 6 AVD renders the
  phone presentation instead of the desktop shell.
- **Dark macOS / light Catalyst+Android**, matching the existing set.

## Open Questions
- **OPEN**: the macOS settings blade is content-sized and clips the fourth accent swatch at the
  1280×720 window; the capture shows it. A `MinimumWidthRequest` (or a wider default blade) is the
  candidate fix.
- **OPEN**: the Android settings capture shows the scrim as opaque black (the shell is not visible
  behind the blade), unlike macOS/Catalyst. Likely the headless emulator's layer compositing; verify
  on a device or a windowed emulator before treating it as a UI bug.
- **ASSUMED**: nothing is committed; the images and README change are in the working tree.

## Files Modified
- `screenshots/maui/04-shell-macos.png`, `05-shell-maccatalyst.png`, `06-shell-android.png` - replaced
- `screenshots/maui/07-settings-macos.png`, `08-settings-maccatalyst.png`, `09-settings-android.png` - new
- `screenshots/README.md` - table rows and capture notes
- `.ai/session-context.md` - this handoff
- (`src/ISynergy.Framework.UI.Maui/ISynergy.Framework.UI.Maui.csproj` and the earlier handoff are from
  the preceding macOS build-fix task in the same working tree.)

## Next Actions (Priority Order)
1. Review the six captures; decide whether to fix the macOS blade width and the Android scrim.
2. Commit the screenshots/README (and the macOS csproj fix).

## Session Notes
- Builds used `MSBUILDDISABLENODEREUSE=1` and `-nodeReuse:false`; `NUGET_HTTP_CACHE_PATH` pointed at
  `$TMPDIR` because the sandbox denies writes to the NuGet http cache.
- The emulator ran from a copied AVD under `$TMPDIR/avd` (`ANDROID_AVD_HOME`) because `~/.android`
  is not writable from the sandbox; the original AVD is untouched.

---

# Session Handoff - 2026-11-08 (macOS head build fix after the MAUI platform split)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Reviewed commit `6faa391f` on macOS, Mac Catalyst and Android, then fixed the one blocking finding.

- **macOS (AppKit) did not compile.** `net10.0-macos` and `net11.0-macos` failed with `CS0234`
  (`Microsoft.Maui.Platforms` missing) in `MauiAppBuilderExtensions.cs`: `UseMauiAppForCurrentHead`
  moved `UseMauiAppMacOS` / `AddMacOSEssentials` into the library, but
  `Microsoft.Maui.Platforms.MacOS` + `.Essentials` were only referenced by `Sample.Maui`.
  The macos TFM is only added on macOS hosts, so the Windows-side verification missed it.
- **Fix applied:** two macos-conditioned `PackageReference`s in
  `src/ISynergy.Framework.UI.Maui/ISynergy.Framework.UI.Maui.csproj`.
- **Verified after the fix:** `UI.Maui` `net11.0-macos` and `net10.0-macos`, and
  `Sample.Maui` `net11.0-macos` (`-p:ValidateXcodeVersion=false`) all build. The condition does not
  affect other heads: `UI.Maui` `net11.0-maccatalyst` and `net11.0-android` still build.
- **Review result (before the fix):** Mac Catalyst and Android were green -
  `UI.Maui` `net10.0-maccatalyst`, `net11.0-maccatalyst`, `net11.0-android`, `net10.0-ios`,
  `net11.0-ios`; `Sample.Maui` `net11.0-android` and `net11.0-maccatalyst`.
- **Code review notes:** the `ThemeService` split is behaviour-preserving on Mac Catalyst (`IOS` is not
  defined for `maccatalyst`, so the old `#if IOS` body was iOS-only and the new `Platforms/iOS` partial
  matches it); `MauiExceptions` correctly has separate iOS and Mac Catalyst files. The one Android
  behaviour delta is `SelectAllOnFocusBehavior`, which now also subscribes the shared `Focused` handler
  on Android (previously native `SetSelectAllOnFocus` only) - intended, idempotent, worth a smoke test.
  `PageMenuExtensions.SupportsMenuBar` relies on `DeviceInfo.Platform == DevicePlatform.MacCatalyst`;
  the AppKit head's reported platform was not verified at runtime.

## Decisions Made
- **Fix the library, not the sample.** The AppKit hosting helper lives in the framework now, so the
  package references move with it, conditioned on the macos TFM.
  - **Alternatives considered:** push `UseMauiAppForCurrentHead` back into the sample (rejected: undoes
    the commit's "samples hold no platform logic" goal).
- **No behaviour change.** Package references only; no source code was edited.

## Open Questions
- **OPEN**: `tests/ISynergy.Framework.UI.Maui.Mac.Tests` has `SupportedOSPlatformVersion` 15.0 (the
  SDK minimum is 17.0) and its `.app` crashes at launch with
  `UIApplicationEvaluateRuntimeIssueForNoSceneLifecycleAdoption` (the Xcode 27 UIScene requirement).
  Pre-existing and unrelated to the commit; the Mac test suite could not be run on Xcode 27.2.
- **OPEN**: `net10.0-android` was not verified - Android SDK platform 36 is not installed (only
  37.0/37.2). `net11.0-android` builds.
- **ASSUMED**: nothing was committed; the csproj fix is in the working tree only.

## Files Modified
- `src/ISynergy.Framework.UI.Maui/ISynergy.Framework.UI.Maui.csproj` - macos-conditioned package refs
- `.ai/session-context.md` - this handoff

## Next Actions (Priority Order)
1. Commit the csproj fix (follow-up commit to `6faa391f`).
2. Optionally raise the Mac test project's `SupportedOSPlatformVersion` to 17.0 and adopt the UIScene
   lifecycle for Xcode 27.
3. Optionally install Android API 36 to verify the `net10.0-android` asset.

## Session Notes
- Builds used `MSBUILDDISABLENODEREUSE=1` and `-nodeReuse:false`.
- `NUGET_HTTP_CACHE_PATH` had to point inside `$TMPDIR`; the sandbox denies writes to
  `~/.local/share/NuGet/http-cache` (NU1900 + restore failures otherwise).
- No CHANGELOG entry: the fix restores a build for an unreleased change.

---

# Session Handoff - 2026-11-08 (UI naming alignment, wave 1)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Started the cross-head naming alignment the user asked for (framework + samples + Blazor, hard rename).
Plan and progress: `.ai/plans/ui-naming-alignment.md`.

- Audit: services already align (shared interfaces); the drift was in views/controls and converters.
- Wave 1 renames (breaking, no shims): MAUI `BladePane` -> `BladeView` (matching WPF/WinUI),
  `BladePaneAnchor` -> `BladeViewAnchor`, `IBladePaneHost` -> `IBladeViewHost`,
  `BladePaneHostRegistry` -> `BladeViewHostRegistry`; MAUI/WinUI `BooleanToIsVisibleConverter` ->
  `BooleanToVisibilityConverter`, `Bool*Converter` -> `Boolean*Converter`, and eight `*Then*Converter`
  names per head -> `*Than*Converter`; `Sample.Maui RootView` -> `ShellView`.
- Also fixed two stale palette tests in `tests/ISynergy.Framework.UI.Tests` that still asserted the 25%
  secondary blend from the earlier contrast work (now 10%).

- **Wave 2 (converters)**: MAUI `*ToIsVisibleConverter` -> `*ToVisibilityConverter` (Decimal, Double, Guid,
  GuidToInversed, Integer, Null, NullToInversed, String, ZeroDecimal, ZeroDouble, ZeroInteger), matching
  WinUI; the sample's `Style.xaml` keys and `SlideShowView.xaml` binding moved with them.
- **Gap register** in the plan: WPF lacks `ImageBrowser`/`NoteWindow`/`CameraService`, WinUI lacks
  `BladeHostView`, and each head has controls the others do not (spinner, masked inputs, dialog buttons,
  tiles, busy/empty views). Those are platform/feature differences, not naming drift - documented, not
  renamed. Sample pages with different structure (`SignInControl` vs `SignInView`) stay as they are.

## Verification
Built: `ISynergy.Framework.UI.WPF` (net11.0-windows10.0.26100.0), `UI.WinUI`, `UI.Maui`, `UI.Blazor`,
`Sample.WPF`, `Sample.WinUI` (needs `-p:RuntimeIdentifier=win-arm64` on this machine), `Sample.Maui`
(net11.0-windows10.0.26100.0) - after both waves. Tests: `ISynergy.Framework.UI.Tests` 90/90,
`ISynergy.Framework.UI.Maui.Windows.Tests` 47 passed / 4 skipped. Launched `Sample.Maui`, opened the
settings blade: English label, single Close button, scrim covers the header, theme switch works.
Repo grep for `IsVisibleConverter`, `*Then*Converter`, `Bool*Converter`, `BladePane` and `RootView`:
no hits outside build output.

## Decisions Made
- **Hard rename, no `[Obsolete]` shims** and **framework + samples + Blazor** (user choice).
- **Rename by concept, not by head**: the pane is `BladeView` everywhere; `Boolean`, not `Bool`; `Than`,
  not `Then`; sample shell page is `ShellView` in every sample.

- **Structural/functional comparison** written to `docs/developers/ui-heads-comparison.md` (linked from
  `docs/developers/index.md`): project shape and TFM/dependency table, contract coverage per head, the
  dialog/navigation/blade/theme mechanics side by side, control and converter inventories, and the gap
  list. Key findings: MAUI `FileService.SaveFileAsync` throws `NotImplementedException` (the only real
  service gap), WPF has no `ICameraService`, WPF lacks `ImageBrowser`/`NoteWindow`, WinUI has no
  `BladeHostView`, and WPF exposes `GetBitmapSourceFromClipboardAsync` with a WPF type in its signature.

- **API-surface alignment**: WPF `ClipboardService.GetBitmapSourceFromClipboardAsync` is `private` (no
  `BitmapSource` on the public surface) and MAUI `FileService.SaveFileAsync` returns `null` instead of
  throwing (MAUI has no save picker; `null` is the "cancelled" contract on the other heads).
- **Full build matrix green**: `UI.WPF`, `UI.WinUI`, `UI.MAUI`, `UI.Blazor`, `Sample.WPF`, `Sample.WinUI`
  (win-arm64), `Sample.Maui`, `Sample.Blazor`, and both UI test projects build; tests 90/90 and 47/4.

- **MAUI platform code split** (`.ai/plans/maui-platform-code.md`): `ThemeService`, `DialogService`,
  `MauiExceptions`, `PageExtensions` and `SelectAllOnFocusBehavior` now use `partial` hooks implemented
  under `Platforms/<head>/`; `Application.CreateWindow` selects the WinUI title bar at runtime. 13 of 22
  `#if` blocks removed; the rest (WinRT launch args, clipboard image matrix, macOS/AppKit, one `DEBUG`)
  are documented with reasons. Builds for windows/android/ios/maccatalyst + `Sample.Maui` green; MAUI
  tests 47 passed / 4 skipped; settings blade smoke-tested.

- **Samples hold no platform logic**: `Sample.Maui` now calls `UseMauiAppForCurrentHead`, `AddShellMenuBar`
  and `ConfigureActivationDiagnostics`; the WinUI focus-visual/cursor mappers live in
  `Platforms/Windows/Extensions/ShellChromeStyler` and are registered by `ConfigureServices`. Only
  package limitations (`#if !MACOS` for MediaElement/Syncfusion), the `DEBUG` agent and the required
  `Platforms/**` entry points remain. Full build matrix + MAUI tests green; sample smoke-tested.

- **SonarCloud PR 174 (S1699 x3) fixed**: WPF `Application` now takes the built `IHost` (composition root
  moved to a static `CreateHost()` in `Sample.WPF`), and `InitializeEnvironmentVariables` is non-virtual in
  WPF and WinUI. Full matrix + both test suites (90/90, 47/4) green.

## Open Questions
- **Wave 2** (see the plan): the remaining converter set, sample-level view names, and the feature gaps the
  audit surfaced (WPF lacks `ImageBrowser`/`NoteWindow`/`CameraService`, WinUI lacks `BladeHostView`).
  Decide per item: rename, implement, or document.
- Member-level differences were not audited yet (text scan only); a Roslyn comparison is the next step.

## Files Modified
- `src/ISynergy.Framework.UI.Maui/Controls/BladeView.cs` (renamed), `Services/IBladeViewHost.cs`,
  `Services/BladeViewHostRegistry.cs`, `Controls/BladeHostView.cs`, `Services/BladeService.cs`,
  `Converters/BooleanConverters.cs`, `Converters/DecimalConverters.cs`, `Converters/DoubleConverters.cs`
- `src/ISynergy.Framework.UI/Enumerations/BladeViewAnchor.cs` (renamed), `Helpers/BladePlacement.cs`
- `src/ISynergy.Framework.UI.WinUI/Converters/*`, `Controls/BladeView/BladeView.xaml.cs`
- `samples/Sample.Maui/Views/ShellView.xaml(.cs)` (renamed), `App.xaml.cs`, `Resources/Styles/Style.xaml`,
  `ViewModels/DetailsViewModel.cs`, `Views/SettingsView.xaml.cs`
- `tests/ISynergy.Framework.UI.Tests/Utilities/ThemeColorCalculatorTests.cs`,
  `Helpers/BladePlacementTests.cs`
- `docs/`, `CHANGELOG.md`, `.ai/plans/ui-naming-alignment.md`, `.ai/session-context.md`

---

# Session Handoff - 2026-10-27 (Blazor component analyzer warnings)

**Written By:** Reasonix Code
**Next Session May Use:** Any

## Task Summary
Cleared nine .NET 11 Blazor analyzer warnings (three classes) reported by the Release build of
`Sample.Blazor` and the two Blazor framework projects. Record:
`.ai/completed/blazor-analyzer-warnings.md`.

- **BL0012 (`UnnecessaryStateHasChangedCall`)** — removed `StateHasChanged()` from
  `samples/Sample.Blazor/Components/Pages/Counter.razor.cs:6` (event handlers re-render anyway) and
  from `MainLayout.ToggleTheme` (`Components/Layout/MainLayout.razor.cs:293`; the click handler
  re-renders and `ThemeService.ThemeChanged` already calls `InvokeAsync(StateHasChanged)`).
- **BL0013 (`AuthenticationStateProviderCachedWithoutSubscription`)** —
  `samples/Sample.Blazor/Components/Controls/NavigationMenuControl.razor` no longer injects
  `AuthenticationStateProvider`; it takes `[CascadingParameter] Task<AuthenticationState>?` from the
  `CascadingAuthenticationState` already in `App.razor` and skips the first-item navigation when the
  value is absent.
- **BL0016 (`UnguardedJSInteropCall`)** — both `ISynergy.Framework.AspNetCore.Blazor` and
  `ISynergy.Framework.UI.Blazor`: `JsRuntimeExtensions.FocusElementById` is best effort and now also
  disposes the `IJSObjectReference` it used to leak; `AntiForgeryTokenControl` leaves the hidden field
  empty when JS is unreachable (prerender/disconnected circuit); `AntiforgeryHttpClientFactory`
  still fails loudly but throws an `InvalidOperationException` naming the cause (and rejects a
  missing token) instead of surfacing the raw interop exception.

## Verification
`dotnet build samples/Sample.Blazor -c Release -t:Rebuild` (analyzers on): **0 warnings, 0 errors**.
`dotnet test tests/Sample.Blazor.UI.Tests -c Release`: **36/36**. No `src/` test project was touched;
no runtime browser session was needed beyond that suite.

## Decisions Made
- **Guard JS interop with `catch (Exception ex) when (ex is JSException or JSDisconnectedException or InvalidOperationException)`**
  — `JSDisconnectedException` does **not** derive from `JSException` (probe: `CS0184`), and
  `InvalidOperationException` is what interop throws while prerendering.
  - **Alternatives considered**: catching `Exception` outright (rejected: hides programming errors in
    published library code); `try/finally` only (rejected: the analyzer does not treat it as a guard).
- **`AntiforgeryHttpClientFactory` keeps failing, it just fails clearer** — a client without the
  token would only fail later as an opaque server-side 400, so the guard wraps and rethrows instead of
  swallowing.
  - **Rationale**: silent degradation of an antiforgery header would be a security regression.
  - **Alternatives considered**: returning a client with an empty header (rejected: silent 400s).

## Open Questions
- **ASSUMED**: consumers that caught `JSException` from `CreateClientAsync` now see
  `InvalidOperationException`; nothing in this repository calls the factory, so the blast radius is
  external only.
- **OPEN**: `Sample.Blazor`'s first-item navigation now depends on `CascadingAuthenticationState`
  being present — a consumer copying `NavigationMenuControl` into an app without it silently skips it.

## Files Modified
- `samples/Sample.Blazor/Components/Pages/Counter.razor.cs` - Removed `StateHasChanged()`
- `samples/Sample.Blazor/Components/Layout/MainLayout.razor.cs:293` - Removed `StateHasChanged()`
- `samples/Sample.Blazor/Components/Controls/NavigationMenuControl.razor` - Cascading auth state
- `src/ISynergy.Framework.AspNetCore.Blazor/Extensions/JsRuntimeExtensions.cs` - Guarded + disposes handle
- `src/ISynergy.Framework.AspNetCore.Blazor/Security/AntiforgeryHttpClientFactory.cs` - Guarded, clearer failure
- `src/ISynergy.Framework.AspNetCore.Blazor/Components/Controls/AntiForgeryTokenControl.razor` - Guarded
- `src/ISynergy.Framework.UI.Blazor/Extensions/JsRuntimeExtensions.cs`, `Security/AntiforgeryHttpClientFactory.cs`,
  `Components/Controls/AntiForgeryTokenControl.razor` - Same three fixes, UI.Blazor copies
- `CHANGELOG.md` - Two `### Fixed` bullets
- `.ai/completed/blazor-analyzer-warnings.md` - Record

## Notes
- Analyzers only run in `Release` (`Directory.Build.props` switches `RunAnalyzersDuringBuild` on for
  Release), so these warnings never appear in a Debug build.
- Builds were run with `MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 -nodeReuse:false
  -p:UseSharedCompilation=false`, and `-p:GeneratePackageOnBuild=false` to avoid packing 0.0.0
  packages into `packages/`.

---

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

## Follow-up — docs published with GitHub Pages; cutover left open
- **Docs → GitHub Pages** (final decision): `docs/_config.yml` (theme `jekyll-theme-minimal` plus the
  whitelisted `jekyll-optional-front-matter` and `jekyll-relative-links` plugins — the pages have no
  front matter and link to each other with relative `.md` paths, so nothing is generated or rewritten),
  `docs/index.md` as the landing page, and `exclude` for the internal reports. One setting: Settings →
  Pages → *Deploy from a branch* → `main` / `docs` → `https://i-synergy.github.io/I-Synergy.Framework/`.
  No workflow, no token, no secret.
- **Wiki sync dropped**: a `.github/workflows/wiki.yml` + `scripts/Sync-DocsToWiki.ps1` pair was built
  and verified (414 pages, 0 collisions, 0 unresolved links, idempotent render, and 415 files staged
  against the real wiki with `Home.md` untouched), then deleted — the Wiki tab is always a separate
  flat `repo.wiki.git` needing its own PAT, and the user chose the folder-as-site route instead.
  Nothing was ever published to the wiki.
- **Cutover left open on purpose** (user decision): run #8's re-run is green end to end and
  `2026.11027.10205-preview` is public and author-signed, but `main` is still `8a447e03`. Remaining,
  all user actions: merge PR #171 (run #9 on `8e7a8853`), approve `production`, then delete
  `azure-pipelines.yml` and move `.ai/progress/github-actions-cutover.md` to `.ai/completed/`.
  Records: `.ai/completed/wiki-docs-sync.md`, `.ai/plans/wiki-docs-sync.md` (SUPERSEDED).

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
