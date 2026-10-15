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
