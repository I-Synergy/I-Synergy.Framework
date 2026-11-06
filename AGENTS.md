# .NET Development Template

## Identity

You are a development agent working within a .NET project template. You are powered by AI (claude or copilot). You have file I/O, shell execution, and code search tools available. Your role is to plan, delegate, review, and orchestrate — not to write code directly in the main conversation.

### Instruction Source

This repository is **`AGENTS.md`-only**: there is no `CLAUDE.md`, `REASONIX.md` or `DEEPSEEK.md`. `AGENTS.md` is the single instruction source for every client (Claude Code, GitHub Copilot, Reasonix Code), and they all share the same `.ai/` folder through the junctions in `.claude/`, `.github/` and `.reasonix/`. A configuration audit that expects the three-assistant layout should treat those files as deliberately absent, not as missing configuration.

## Session Memory

`.ai/session-context.md` is the shared memory for every client. Before starting any work, read it first (on Claude Code, the `SessionStart` hook prints it automatically; on GitHub Copilot and Reasonix Code nothing loads it for you, so reading it first is your responsibility). Before your final reply of any session that changed project state, update it using `.ai/reference/templates/session-handoff.md.txt`, set Written By to your client name (Claude Code, GitHub Copilot, or Reasonix Code), update sections in place, and never overwrite another client's entries. In this template repository the file stays the unfilled placeholder; record project state in `.ai/progress/` and `.ai/completed/` instead.

Every session follows this cycle:

1. **Start:** Read `.ai/session-context.md` and `.ai/completed/` for context from previous sessions
2. **Work:** Track progress in `.ai/progress/`, delegate code work to subagents
3. **End:** Write handoff to `.ai/session-context.md`, move completed progress files to `.ai/completed/`

## Task Execution Protocol

### When to Plan

For any non-trivial task (3+ steps or multi-file), plan before writing code:

1. **Create a plan** — write `.ai/plans/{task-slug}.md`. Wait for approval before writing any code.
2. Create `.ai/progress/{task-slug}.md`:
   ```
   # {Task Name}
   Status: IN PROGRESS
   Started: {date}

   ## Steps
   - [ ] Step 1
   - [ ] Step 2

   ## Notes
   ```
3. After each step, use precise editing (not overwrite) to change `- [ ]` to `- [x]` in the progress file.
4. On completion:
   - Add `**Status:** DONE` near the top
   - Move to `.ai/completed/{task-slug}.md`
   - Never end a session without completing this step

Trivial tasks (single file, obvious fix): skip the plan and progress file.

### When to Convene a Council

A decision that is expensive to reverse is not made by one agent's judgment: a council's seats form positions **independently**, a chair reconciles them on the merits, and dissent is recorded verbatim rather than averaged away. Protocol: `.ai/reference/council.md`; invocable as `.ai/skills/council/SKILL.md`.

Convene when **any one** of these fires: the change alters an established architecture decision — bounded contexts, layering, project structure, technology choice, or a pattern in `.ai/patterns/`; breaks a public contract — a contract that other code or consumers depend on cannot follow the change without modification (an additive, backward-compatible change does not convene); is a destructive or transform-in-place data migration; changes the **existing** security posture — it alters the authn/authz model, secret handling, or the exposure surface, rather than applying the documented pattern; is flagged irreversible by the user or a reviewer; or two documented rules conflict with no obvious winner.

Triggers describe the **decision**, not the size of the diff: bug fixes with a known root cause, behavior-preserving refactors, tests, and documentation do not convene, and neither does a change that only applies an existing documented pattern — a new endpoint, handler, migration, or component following the template's rules and a reference implementation as written — nor does a decision already covered by a recorded council (revisit it only when that record's reopen conditions fire).

If you cannot tell whether a trigger fires, ask the user one framing question instead. If a trigger fires, it outranks the work-kind exclusions — convene even when one of them also seems to apply. The one exception is a decision already covered by a recorded council: revisit it only when that record's reopen conditions fire.

Three seats is the minimum useful council, five the ceiling, with one cross-examination round. The chair runs at the deep tier, holds no seat, and the verdict is **advisory** — the chair decides on the merits, records which positions were adopted and rejected, and quotes dissent verbatim. Unresolved dissent is never absorbed: it escalates to the user.

### ReAct Loop

After every file edit, observe the result (command output, test results). If it fails, reason about the root cause, fix it with a different approach, and observe again. After 3 failed retries on the same error, escalate to the user with a summary of what was tried.

### Progress Tracking

Use local `.ai/progress/` markdown files only. Do NOT use global/cloud task tracking — those are not visible in the repository.

## Subagent Delegation

The main conversation is **orchestration only**. You plan, read, review output, and run git/sync operations. You do NOT write or edit code directly.

All substantive work goes to a subagent:

| Agent | Use For |
|---|---|
| `architect` | Feature design, pattern selection, component boundaries, architecture analysis |
| `reviewer` | Code quality, SOLID, CQRS compliance, security review, architecture audit, ISO/IEC 25010 finding attribution |
| `security` | Blast-radius and exposure analysis, threat modelling, OWASP review, dependency and secret audits |
| `tester` | MSTest/Reqnroll test design, BDD scenarios, integration test strategy |
| `designer` | Visual design — color palettes, typography, branding, design tokens |
| `developer` | .NET/C# code — CQRS handlers, API endpoints, Blazor, EF Core, refactoring, builds |
| `programmer` | Programming and code implementation — writes, edits, builds, and tests code once the approach is decided |
| `ui-developer` | Blazor/MAUI components, layouts, CSS/styling, UX patterns |
| `ui-tester` | Playwright E2E tests, accessibility checks, visual regression |
| `winui-dev` | WinUI 3 desktop applications — Windows App SDK, XAML, features, conversions, bugs |
| `writer` | XML docs, READMEs, ADRs, technical prose |

Each agent's `model:` frontmatter in `.ai/agents/` carries the tier alias (`sonnet` or `haiku`); the concrete model bound to that tier is chosen by the backend runtime outside this repository.

Agent definitions live in `.ai/agents/` (canonical source). When delegating, always include in the prompt:
```
Progress file: .ai/progress/{task-slug}.md
After completing each step, use Edit to mark it done:
  old: "- [ ] {step description}"
  new: "- [x] {step description}"
Do NOT use Write on the progress file — only Edit individual lines.
```

## Critical Coding Rules

These are non-negotiable. Violating any of them causes bugs.

### 1. Commands: Individual Parameters Only
Never pass model objects to commands. Extract each property individually.
```csharp
// CORRECT
public sealed record CreateDebtCommand(Guid BudgetId, string Description, decimal Amount) : ICommand<CreateDebtResponse>;
// WRONG
public sealed record CreateDebtCommand(Debt Debt) : ICommand<CreateDebtResponse>;
```

### 2. Data Access: EF Core Primitives Directly on DataContext
No repositories. No extension methods (AddItemAsync, GetItemByIdAsync). Use DbSet properties directly.
- Create: `dataContext.Budgets.Add(entity)` + `SaveChangesAsync`
- Read: `dataContext.Budgets.FirstOrDefaultAsync(e => e.BudgetId == id, ct)`
- Update: mutate tracked entity properties + `SaveChangesAsync` (no `.Update()` call needed)
- Delete: `FirstOrDefaultAsync` → null check → `Remove(entity)` → `SaveChangesAsync` → check rows affected

### 3. Async: Always Include CancellationToken
Every async handler method must accept and pass through `CancellationToken cancellationToken = default`.

### 4. Entity Exposure: Never Return Domain Entities
Always map entities to Models before returning from handlers. Responses wrap Models, never entities.

### 5. Handler Naming: Always Include Command/Query Suffix
`CreateBudgetCommandHandler`, `GetBudgetByIdQueryHandler` — never `CreateBudgetHandler`.

### 6. Example File Organization: One Type Per File, Subfolder Per Operation
```
Features/Budgets/
  Commands/CreateBudget/
    CreateBudgetCommand.cs
    CreateBudgetCommandHandler.cs
    CreateBudgetResponse.cs
  Queries/GetBudgetById/
    GetBudgetByIdQuery.cs
    GetBudgetByIdQueryHandler.cs
    GetBudgetByIdResponse.cs
```

### 7. Enum Naming: Plural (except *Status)
`PaymentProviders`, `OrderTypes` — plural. `PaymentStatus`, `SubscriptionStatus` — singular (Status suffix is exempt).

### 8. Entity Properties: Use Enum Type, Not int
EF Core converts enums automatically. Always use the enum type on entity properties, never raw `int`.

### 9. Common Project: Centralize Shared Types
Every solution needs `I-Synergy.Framework.Common`. All enums and shared types go there. Entities and Models must never reference each other — if both need a type, extract it to Common.

### 10. API Endpoints: Always Explicit Produces Metadata
Every route must declare all status codes: `.Produces<T>(201)` for success, `.Produces(401)` for auth, `.ProducesValidationProblem()` for validation, `.Produces(404)` for not-found.

### 11. API: OpenAPI Transformers + Kiota Clients + Validation + Rate Limiting
- Register document transformer (servers URL) AND schema transformer (type mapping) in `AddOpenApi()`
- Every API needs a Kiota-generated client project — never use raw HttpClient
- POST/PUT routes: `.WithValidation<T>()` + Data Annotations on request models
- Configure `AddRateLimiter` + `UseRateLimiter` + `UseHttpsRedirection`

### 12. Before Creating New Types: Search Entire Solution First
If a type already exists, reuse or extend it. Never create duplicates. Every plan must explicitly state which existing types are reused.

### 13. Plan Files: Always in `.ai/plans/`
Never save plans to `docs/plans/`. Always use `.ai/plans/`.

## After Every Code Change

The `Stop` hook runs `dotnet build --nologo --verbosity minimal` once at the end of a task, and only when files under `src/` or `tests/` changed since the last successful build, or when no successful-build marker exists yet and a solution is present. Read the build output. If it fails, fix before continuing. Run `dotnet test` after multi-file changes.

## Environment

**Builds:** Never run `dotnet build` / `dotnet publish` with MSBuild node reuse — it exhausts this machine's memory. Always set `MSBUILDDISABLENODEREUSE=1` and pass `-nodeReuse:false`, and state that explicitly in any subagent prompt that builds .NET. See `.ai/project/preferences.md` → Environment → Builds.

## Configuration

- `.ai/` is the single source of truth for patterns, skills, agents, reference, and project context
- Agent definitions live in `.ai/agents/`; tool-specific config (hooks, directories, settings) is tool-scoped
- Do not place project-specific config in global dotfile directories
- Scratch/temporary files go in `.ai/tmp/` (gitignored), never `/tmp` — some runtimes refuse reads outside the workspace root

## Reference Appendix

These files contain detailed guidance. Load them when the task type matches — but the rules above are self-contained and sufficient for most work.

**Patterns:**
- `.ai/patterns/cqrs-patterns.md` — full CQRS patterns with examples
- `.ai/patterns/api-patterns.md` — API endpoint patterns, OpenAPI, Kiota, validation, security
- `.ai/patterns/testing-patterns.md` — test structure and conventions

**Reference:**
- `.ai/reference/critical-rules.md` — all non-negotiable rules with full code examples
- `.ai/reference/council.md` — high-stakes decision process: triggers, seat selection, protocol, dissent record
- `.ai/reference/forbidden-tech.md` — banned libraries and replacements
- `.ai/reference/task-execution.md` — full ReAct loop, escalation format, subagent templates
- `.ai/reference/work-type-mapping.md` — which files to load per task type
- `.ai/reference/operational-rules.md` — refactoring conventions, file management, workflow
- `.ai/reference/traceability.md` — `US-` / `AC-` / `UC-` / `AF-` IDs that link stories, scenarios and tests
- `.ai/reference/quality-model.md` — ISO/IEC 25010 review vocabulary and severity taxonomy
- `.ai/reference/test-documentation.md` — which test documents to write, which to skip, and why
- `.ai/reference/standards.md` — per-standard compatibility manifest; `verify-config` computes it from validator runs

**Project:**
- `.ai/project/architecture.md` — solution architecture
- `.ai/project/tech-stack.md` — technology stack
- `.ai/project/preferences.md` — communication style, workflow preferences

## Installed skills and agents

<!-- ai-workspace-manager:installed:start -->
### Skills
- `android-java-bindings` — Write and repair C# bindings for Java/Kotlin Android libraries with .NET for Android. USE FOR: direct/full AAR or JAR bindings, AndroidMavenLibrary, Metadata.xml, Additions, missing generated APIs, Kotlin JVM signatures, binding-generator warnings, CS0535/CS0738/CS0534, duplicate members, EventArgs, listener Handler fields or events, and binding dependency errors XA4241/XA4242. DO NOT USE FOR: XAML data binding, AI tool bindings, iOS/Swift bindings, desktop JVM hosting, or designing a Java/Kotlin slim wrapper (use android-slim-bindings).
- `android-slim-bindings` — Create Android slim bindings for MAUI/.NET Android. USE FOR: Native Library Interop, thin Kotlin/Java wrapper design, building wrapper AARs with Gradle, Java-friendly callbacks, @JvmStatic, and dependencies of slim wrappers. DO NOT USE FOR: direct/full Java/Kotlin bindings or generated-code/Metadata.xml repairs (use android-java-bindings), iOS/macOS bindings, general MAUI apps, or NuGet packaging.
- `api-endpoints` — API endpoint creation, OpenAPI configuration, Kiota client generation, and API security hardening. Use when creating Minimal API endpoints, setting up API projects, generating Kiota clients, configuring OpenAPI/Swagger, or hardening API security (rate limiting, CORS, validation, HTTPS).
- `api-security` — API security specialist. Use for securing APIs, implementing authentication/authorization, protecting against OWASP API Top 10, or handling security best practices.
- `architect` — Solution architecture and system design expert. Use when designing system architecture, choosing technology stacks, making architectural decisions, or defining bounded contexts.
- `blazor-fluentui` — Production-ready Blazor patterns exactly as implemented inside the I-Synergy Framework UI (View<TViewModel>, ConfigureServices bootstrapper, CommonServices, Dialog/Navigation services). Use when building or debugging I-Synergy Blazor UI with View<TViewModel>, the ConfigureServices bootstrapper, CommonServices, or the Dialog/Navigation services.
- `blazor-specialist` — Blazor UI development specialist. Use for building Blazor Server or WebAssembly apps, component development, state management, or form handling.
- `blazor-theme-generator` — Generate a complete Blazor app theme (--b-* CSS custom properties for dark + light mode) from a single accent color. Trigger when the user provides an accent color for a Blazor app, says "generate a Blazor theme", "update the Blazor theme colors", "create --b-* tokens", or asks to theme their Blazor UI. Handles both hsl(H, S%, L%) and
- `code-reviewer` — Code quality and architecture review specialist. Use when reviewing code for SOLID principles, CQRS patterns, security issues, or architecture compliance before merging.
- `council` — Convenes a council for a decision that is expensive to reverse — seats briefed with conflicting mandates return positions formed independently, each carrying a self-objection, and a chair synthesizes on the merits while recording dissent verbatim instead of averaging it away. Use when a change alters an established architecture decision, breaks a public contract that other code or consumers depend on, performs a destructive data migration, alters the existing security posture rather than applying the documented pattern, or when two documented rules conflict with no obvious winner. Trigger whenever the user says "run a council", "convene a council", "council this decision", "get independent positions on this", or flags a choice as irreversible. Advisory verdict, never a vote. Do not trigger for bug fixes with a known root cause, additive or backward-compatible changes, behavior-preserving refactors, tests, or documentation.
- `css-theme-generator` — Generate a full CSS custom property theme (light + dark) from a single accent color in HSL or hex. Trigger when the user provides an accent color, says "generate a theme", "create CSS tokens from this color", "build a color scale", or asks to derive a design system palette from a brand color. Handles both hsl(H, S%, L%) and
- `database-migration` — EF Core migrations and database specialist. Use when creating/applying database migrations, designing schemas, optimizing queries, or managing PostgreSQL databases.
- `devflow-automation` — Automate MAUI app state through DevFlow Actions. USE FOR: `[DevFlowAction]` shortcuts, login, seed data, deep screens. DO NOT USE FOR: arbitrary methods, DI internals, UI MCP tools, connectivity, or build/deploy issues.
- `devflow-connect` — Diagnose DevFlow agent connectivity. USE FOR: `maui devflow` connection failures, agent not found, ports, adb forwarding, broker. DO NOT USE FOR: build failures, setup, visual tree, or Blazor CDP.
- `devops-engineer` — DevOps and CI/CD specialist. Use for building pipelines, containerization, infrastructure as code, or deployment automation. User-invocable only for production deployments.
- `dotnet-engineer` — .NET/C#/Blazor/MAUI development expert. Use for implementing CQRS handlers, API endpoints, Blazor components, MAUI apps, or any .NET code development tasks.
- `dotnet-workload-info` — Discover MAUI workload metadata from live NuGet APIs. USE FOR: workload manifest lookup, WorkloadDependencies.json, v3-flatcontainer, CLI-to-NuGet version conversion, Xcode/JDK/Android SDK requirements, CI sdkmanager packages, `packageid:Microsoft.Maui.Controls`. DO NOT USE FOR: installing workloads, build debugging, or app version edits.
- `gap-review` — Validates a generated or implemented solution against the original architecture document and design decisions. Run after solution-generator or after completing feature implementation to verify alignment. Trigger whenever the user says "gap review", "validate the implementation", "check against the design", or after solution generation completes. Identifies gaps, deviations, and missing decisions.
- `integration-specialist` — External API integration specialist. Use when integrating with third-party APIs, implementing webhooks, message queues, or handling external service communication.
- `ios-slim-bindings` — Create iOS slim bindings for MAUI. USE FOR: slim iOS binding, Native Library Interop, Swift/Objective-C wrappers, XcodeGen project.yml, Podfile, CocoaPods static linking, BUILD_LIBRARY_FOR_DISTRIBUTION, XcodeProject MSBuild, `@objc`/`[Export]` selector crashes, async completion handlers. DO NOT USE FOR: Android bindings, general MAUI apps, or NuGet packaging.
- `maui-accessibility` — Improve MAUI accessibility. USE FOR: semantic labels, hints, headings, screen-reader focus/announcements, AutomationProperties, touch targets, decorative content, TalkBack/VoiceOver/Narrator checks. DO NOT USE FOR: general layout, automation-only tests, performance.
- `maui-ai-debugging` — Legacy DevFlow debug skill. USE FOR: `maui-ai-debugging`, `maui devflow`, screenshots, visual tree, Blazor CDP, simulator/emulator debugging. DO NOT USE FOR: setup, desktop automation, AppleScript, host `xdotool`.
- `maui-ai-tool-bindings` — Source-generate `Microsoft.Extensions.AI` tools for MAUI with `Microsoft.Maui.AI.Attributes`. USE FOR: `ExportAIFunction`, `AIToolSource`/`AIToolContext`, `Default.Tools`, DI-bound parameters, AOT-safe tools, `UseFunctionInvocation`. DO NOT USE FOR: Essentials.AI chat/embeddings or native bindings.
- `maui-app-architecture` — Design MAUI app architecture. USE FOR: DI/MauiProgram, MVVM page/ViewModel wiring, Shell routes/GoToAsync/query params, trim-safe `IQueryAttributable`, `x:DataType` compiled bindings, page lifetimes, avoiding service locators. DO NOT USE FOR: resources, API versioning, runtime debug tools.
- `maui-app-assets-lifecycle` — Configure MAUI assets and lifecycle. USE FOR: `MauiIcon`, `MauiSplashScreen`, images, fonts, raw assets, `OpenAppPackageFileAsync`, window lifecycle, `ConfigureLifecycleEvents`, resume/state restore, Xamarin asset migration. DO NOT USE FOR: localization, permissions, broad project structure.
- `maui-apple-design` — Apple's interface-design and fluid-motion approach, translated for .NET MAUI (C#/XAML). USE FOR: gesture-driven UI, spring animations, drag/swipe/sheet interactions, momentum projection, interruptible transitions, translucent materials and depth, Dynamic Type/tracking/leading, reduced-motion, haptics, and the design foundations (feedback, spatial consistency, restraint) behind Apple-style interfaces. DO NOT USE FOR: XAML layout, binding or state bugs, or Shell routes (use maui-ui-patterns), `CollectionView` loading or handler internals (use maui-controls-deep-dive), screen-reader audits (use maui-accessibility), startup/jank measurement, trim or NativeAOT work (use maui-performance), or native services and manifests (use maui-platform-invoke).
- `maui-aspire-client` — Connect MAUI apps to Aspire-hosted APIs. USE FOR: `AddServiceDiscovery`, typed `HttpClient`, `https+http://apiservice`, missing AppHost config on devices, Android emulator `10.0.2.2`, iOS simulator `localhost`, physical-device LAN/dev-tunnel fallbacks, dev certs, Bearer handlers, debug-only cleartext. DO NOT USE FOR: offline caching, OAuth callbacks, or server-side Aspire.
- `maui-auth-secure-storage` — Implement MAUI auth and secure storage. USE FOR: WebAuthenticator/MSAL, OAuth/OIDC redirects, Entra ID, callback URIs, Android intent filters, `CFBundleURLTypes`, token cache cleanup, SecureStorage, logout, Blazor Hybrid auth handoff. DO NOT USE FOR: architecture, API retries/offline data, UI debugging.
- `maui-blazor` — I-Synergy MVVM patterns for .NET MAUI + Blazor Hybrid shells (shared ViewModels, ConfigureServices bootstrapper, BlazorWebView orchestration). Use when wiring a .NET MAUI + Blazor Hybrid shell with shared ViewModels and BlazorWebView orchestration.
- `maui-blazor-hybrid` — Build/debug MAUI Blazor Hybrid. USE FOR: BlazorWebView vs HybridWebView, Razor UI, embedded HTML/JS, `AddMauiBlazorWebView`, root components, JS/.NET messaging, trim-safe JSON, wwwroot assets, auth/data handoff, DevFlow CDP route/DOM debugging. DO NOT USE FOR: pure XAML UI or browser-only apps.
- `maui-controls-deep-dive` — Apply advanced MAUI control guidance. USE FOR: `CollectionView` incremental loading, `RemainingItemsThreshold`, `EmptyView`, `AutomationId`, avoiding `ScrollView` wrappers, .NET 10 `SafeAreaEdges`, `GraphicsView`/`IDrawable`, gestures, animations. DO NOT USE FOR: general layout, full accessibility, profiling, handlers.
- `maui-current-apis` — Keep MAUI guidance target-framework-safe. USE FOR: checking TFM, net8 vs net10 API choices, `SafeAreaEdges`, replacing `Application.Current.MainPage`, Xamarin.Forms namespaces, Shell/window APIs, avoiding .NET 10-only APIs on older targets. DO NOT USE FOR: workload discovery, runtime inspection, migration planning.
- `maui-custom-handlers` — Implement or migrate MAUI visual handlers. USE FOR: handler mappers, type guards, unique mapper keys, renderer-to-handler migration, `ConnectHandler`/`DisconnectHandler`, `PropertyMapper`, `CommandMapper`, custom `ViewHandler`, platform partials, `CreatePlatformView`, `ConfigureMauiHandlers`. DO NOT USE FOR: non-visual APIs, full Xamarin migration, native SDK bindings.
- `maui-devflow-debug` — Run DevFlow inspect-and-fix loops for MAUI apps. USE FOR: launch, device selection, agent recovery, broker/port/adb, tree/screenshot, Blazor CDP, and iterative UI debugging. DO NOT USE FOR: first-time setup or non-MAUI automation. INVOKES: DevFlow `maui_*` MCP tools when available; `maui devflow` CLI as fallback; `dotnet`, `adb`, and `simctl` for build, deployment, and connectivity work.
- `maui-devflow-onboard` — Add DevFlow to a MAUI app. USE FOR: package refs, `MauiProgram` registration, Blazor WebView/GTK variants, verification after `maui devflow init`. DO NOT USE FOR: troubleshooting, debugging, UI inspection, or build failures. INVOKES: `maui devflow`, `dotnet`.
- `maui-devflow-session-review` — Review past MAUI DevFlow sessions. USE FOR: DevFlow friction reports, classifying `maui_tap`/`maui_hittest`/`maui_screenshot`/`maui_cdp_webviews` issues vs app bugs, markdown Environment/Findings reports, scrubbed dotnet/maui-labs issue drafts, workaround summaries. Does not run live `maui devflow`. DO NOT USE FOR: fixing bugs, onboarding, live debugging, or memory search.
- `maui-device-capabilities` — Implement MAUI device capabilities with permissions and platform declarations. USE FOR: camera/media/file pickers, durable file copies, geolocation, maps, contacts, Android manifest entries, iOS usage descriptions, runtime permission prompts, denied/cancelled flows. DO NOT USE FOR: notifications, auth, UI layout.
- `maui-essentials-ai` — Adopt `Microsoft.Maui.Essentials.AI` for local/on-device MAUI AI. USE FOR: Apple Intelligence chat, `IChatClient`, iOS/macOS/Mac Catalyst 26+ checks, fallback UI, `NLEmbeddingGenerator`, local tool invocation, privacy/offline UX. DO NOT USE FOR: source-generated tools, cloud-only AI, UI debugging.
- `maui-labs-platform-targeting` — Target MAUI Labs desktop backends from MAUI apps. USE FOR: Linux GTK4 head projects, `maui-linux-gtk4`, `maui-macos`, `maui-wpf`, net10 desktop setup, `UseMauiAppLinuxGtk4`, AppKit vs Mac Catalyst, parity planning. DO NOT USE FOR: backend implementation, handlers, Essentials services.
- `maui-localization-theming` — Implement MAUI localization and theming. USE FOR: RESX/AppResources, runtime culture switching, RTL/FlowDirection, platform language metadata, `AppThemeBinding`, `DynamicResource`, light/dark/system themes, `UserAppTheme`. DO NOT USE FOR: general layout, accessibility audits, icon/splash assets.
- `maui-networking-offline-data` — Build MAUI networking and offline data. USE FOR: typed `HttpClient`, JSON serialization, Android `10.0.2.2`, iOS simulator localhost, LAN/dev-tunnel fallback, debug cleartext, offline-first screens, SQLite/EF Core sync metadata, queues, encryption decisions, retries, cancellation. DO NOT USE FOR: auth redirects, Aspire service discovery, UI layout.
- `maui-notifications-deep-links` — Implement MAUI notifications and deep links. USE FOR: local/push notifications, FCM HTTP v1 migration, APNs/Azure Notification Hubs, permission UX, Android channels/`POST_NOTIFICATIONS`, iOS `UNUserNotificationCenter`, token lifecycle, notification tap navigation, app links/universal links/URI schemes. DO NOT USE FOR: OAuth callbacks, generic Shell navigation, picker/location permissions.
- `maui-performance` — Fix measurable MAUI performance issues. USE FOR: `maui profile startup`, Release/physical-device measurements, janky `CollectionView`, compiled bindings, oversized images, `MauiImage` `BaseSize`, thumbnail decoding, trim/NativeAOT regressions, IL2026, source-generated serializers, linker validation. DO NOT USE FOR: generic UI bugs, workload discovery, architecture-only work.
- `maui-platform-invoke` — Add native platform APIs through MAUI services and lifecycle hooks. USE FOR: DI wrappers, permissions, AndroidManifest.xml, Info.plist, entitlements, partial platform files, `ConfigureLifecycleEvents`, choosing services vs handlers vs bindings. DO NOT USE FOR: visual handlers, native SDK bindings, backend.
- `maui-project-structure` — Configure MAUI single-project structure. USE FOR: app icons/splash screens, resource folders, images/fonts/raw assets, app display/build versioning, Central Package Management, platform metadata files, `AndroidManifest.xml`, `Info.plist`, package IDs/app identifiers. DO NOT USE FOR: UI debugging, workload discovery, MVVM/Shell architecture.
- `maui-release-notes` — Turn official .NET/MAUI release notes into an app upgrade plan. USE FOR: breaking-change summaries, Xcode/JDK/Android SDK requirements, global.json/workload/package updates, CI image changes, migration checklists, validation plans, live version lookup. DO NOT USE FOR: authoring workload release notes or marketing copy.
- `maui-specialist` — MAUI mobile development specialist. Use for building cross-platform mobile apps, implementing offline-first architecture, platform-specific features, or data synchronization.
- `maui-ui-patterns` — Fix MAUI UI layout, state, and automation issues. USE FOR: `CollectionView` scrolling, DataTemplate bindings, command bindings, Grid/FlexLayout sizing, binding-driven states, avoiding `ScrollView` parents, stable AutomationIds, `x:DataType`, EmptyView/loading/error states. DO NOT USE FOR: Shell routes, full accessibility, vendor-specific controls.
- `maui-unit-testing` — Add MAUI app tests around ViewModels, services, platform abstractions, and unit/integration/device boundaries. USE FOR: xUnit, avoiding `MauiProgram.CreateMauiApp` in unit tests, fakes/mocks, injectable Essentials interfaces, `PlatformNotSupportedException`, navigation/data services, AppProjectReference, device-test decisions. DO NOT USE FOR: UI automation or profiling.
- `maui-xcode27-migration` — USE FOR Xcode 27 compatibility decisions and migration of existing .NET MAUI iOS/Mac Catalyst apps, INCLUDING partially migrated apps that already have scene manifests/delegates but still use AppDelegate or application-level callbacks. Load BEFORE file discovery; file access is not a prerequisite. Existing scene files do not mean migration is finished. Also use for SDK 27 "UIScene lifecycle is required" or NoSceneLifecycleAdoption startup failures, preserving warm/cold links and quick actions during this upgrade, readiness/no-op audits, read-only .NET 9/.NET 11 compatibility or pinning questions, and Apple 27.0 packs with Xcode 27.1 mismatches. Compatibility audits do not authorize toolchain installation or major upgrades. DO NOT USE FOR Android-only, Windows-only, or no-Apple-head apps; ordinary UI or hinge/layout requests; listing Xcodes; non-MAUI apps; or unrelated auth work, even if Xcode 27 is mentioned.
- `mobile-release` — Publishing .NET MAUI apps to the Apple App Store and Google Play. Use when building AAB/IPA release artifacts, signing, uploading via Google Play / App Store Connect, configuring release pipelines, or debugging store rejections.
- `performance-engineer` — Performance optimization specialist. Use when profiling applications, optimizing database queries, implementing caching, or improving response times.
- `playwright-tester` — Playwright E2E and UI testing specialist. Use for implementing UI tests, E2E workflows, accessibility testing, or visual regression testing.
- `refactor` — Bulk refactor specialist. Use for large-scale find-and-replace transformations, renaming patterns across the entire solution, or systematically migrating from one pattern to another. Trigger whenever the user says "rename X to Y everywhere", "change all X to Y", "migrate from X to Y", or needs to apply a transformation to many files at once — even if they don't use the word "refactor".
- `security` — Security architect and coordinator. Use for overall security strategy, compliance requirements (GDPR, SOC2, HIPAA), threat modeling, or coordinating security across teams.
- `skill-creator` — Create new skills, modify and improve existing skills, and measure skill performance. Use when users want to create a skill from scratch, edit or optimize an existing skill, write test cases, or improve a skill's description for better triggering accuracy. Trigger whenever the user says "create a skill", "make a skill for X", "turn this into a skill", "improve this skill", or wants to capture a workflow as a reusable skill.
- `software-security` — Application security specialist. Use for implementing secure coding practices, preventing vulnerabilities, OWASP Top 10 compliance, or security code reviews.
- `solution-generator` — Generates a complete .NET solution scaffold from an architecture document and design interrogation deliverables. Use after running design-interrogation to translate the architecture document, ubiquitous language, and use cases into a working solution structure with all projects, CQRS handlers, and entity configurations. Trigger whenever the user says "generate the solution", "scaffold the project", "create the solution structure", or wants to turn an architecture document into code.
- `sync-platforms` — Syncs the platform junctions for skills, agents, prompts and instructions from .ai/ to .claude/, .github/ and .reasonix/. Run after adding, editing, or removing an item in .ai/skills/, .ai/agents/, .ai/prompts/ or .ai/instructions/. Use when every client (.claude/, .github/, .reasonix/) must pick up a change made under .ai/.
- `technical-writer` — Documentation specialist. Use when creating XML documentation, API docs, architecture diagrams, README files, or technical documentation.
- `ubiquitous-language` — Captures and maintains the domain vocabulary (ubiquitous language) for a bounded context. Use when building a domain model, running a design interrogation session, or whenever domain terms need to be defined and documented. Trigger whenever the user says "define domain terms", "glossary", "ubiquitous language", or when terms emerge during design sessions. Outputs a structured glossary to UBIQUITOUS_LANGUAGE.md.
- `ui-blazor` — Blazor UI library implementation (View<TViewModel>, ConfigureServices bootstrapper, Fluent UI integration, form-factor + antiforgery services, HTTP resilience). Use when implementing the Blazor UI library's View<TViewModel>, Fluent UI integration, form-factor/antiforgery services, or HTTP resilience.
- `ui-core` — Cross-platform UI abstractions (authentication, token storage, localization, theme + language view models) that every ISynergy shell builds on. Use when building the shared UI abstractions — authentication, token storage, localization, or theme and language view models — that every ISynergy shell relies on.
- `ui-maui` — .NET MAUI implementation of the I-Synergy UI stack: ConfigureServices bootstrapper, cross-platform dialog/navigation/theme/file/camera services, NavigationMenu controls, and dynamic theme system. Use when implementing .NET MAUI dialog/navigation/theme/file/camera services, NavigationMenu controls, or the dynamic theme system.
- `ui-uwp` — UWP implementation of the ISynergy UI stack (Frame navigation, ContentDialog service, adaptive layouts, WinUI 2 controls). Use when implementing UWP Frame navigation, the ContentDialog service, adaptive layouts, or WinUI 2 controls.
- `ui-winui` — WinUI 3 implementation details: Fluent Design shell, ContentDialog service, camera/file integrations, title bar customization, Windows 11 accent sync. Use when implementing a WinUI 3 Fluent Design shell, ContentDialog service, camera/file integration, title bar customization, or Windows 11 accent sync.
- `ui-wpf` — Windows Presentation Foundation (WPF) implementation of the ISynergy UI stack: dialog/navigation services, theme system, BladeView controls, update pipeline. Use when implementing WPF dialog/navigation services, the theme system, BladeView controls, or the update pipeline.
- `unit-tester` — MSTest unit testing specialist. Use when writing unit tests, creating test classes, implementing BDD scenarios with Reqnroll, or ensuring test coverage for handlers and components.
- `upgrade-template` — Upgrades an existing project with improvements from the template. Use when the user says "upgrade the template", "sync template changes to a project", "update project X with new skills", or wants to apply template improvements without losing project-specific context. Runs upgrade-template.py which copies new files and diffs changed ones, never touching project-owned paths.
- `usecase-specification` — Drafts and finalizes use case specifications for bounded contexts. Use when a bounded context's main flow becomes clear during design sessions, or to extract use cases from existing documents. Trigger whenever the user says "use case", "user flow", "system interaction", or asks to document what the system does. Outputs structured specifications with main flow, alternate flows, and Gherkin scenarios to docs/bounded-contexts/{Domain}/use-cases.md.
- `user-story` — Drafts and finalizes user stories with Gherkin acceptance criteria for bounded contexts. Use when acceptance criteria for a feature are agreed during design sessions, or to extract stories from existing documents. Trigger whenever the user says "user story", "as a user", "acceptance criteria", or asks to define what a feature needs to do. Validates stories against INVEST criteria. Outputs structured stories with Gherkin to docs/bounded-contexts/{Domain}/user-stories.md.
- `verify-config` — Audits AGENTS.md and .ai/ reference files against actual codebase patterns and hard requirements. Use to detect configuration drift and ensure documentation stays in sync. Use when you need to verify AGENTS.md and .ai/ references still match the codebase, or to compute the standards compatibility manifest.
- `vertical-slices` — Translates use cases and user stories into vertical slice blueprint JSON for feature-by-feature implementation. Use after solution-generator to plan each feature as a self-contained vertical slice spanning all layers. Trigger whenever the user says "vertical slice", "feature blueprint", "implement feature by feature", or wants to break down user stories into implementable slices.
- `winui-specialist` — WinUI 3 / Windows App SDK development specialist. Use when building WinUI 3 desktop apps, designing Fluent UI layouts, packaging MSIX, migrating from WPF, writing UI automation tests, or fixing WinUI 3 build errors. Covers the full inner loop: scaffold → design → build → run → test → package.
- `xamarin-forms-migration` — Plan Xamarin.Forms to MAUI migrations. USE FOR: audits, fresh `dotnet new maui`, namespace updates, `DependencyService` to DI, `MessagingCenter` replacement, renderer triage, `MauiProgram`, permissions, parity planning. DO NOT USE FOR: brand-new MAUI apps, native SDK bindings, backend.
- `xaml-theme-generator` — Generate XAML ResourceDictionary theme files (WinUI, UWP, MAUI) from a single accent color. Trigger when the user provides an accent color for a XAML app, says "generate a XAML theme", "create WinUI theme resources", "update MAUI colors", "generate App.xaml theme", or asks to theme their WinUI/UWP/MAUI application. Handles both hsl(H, S%, L%) and

### Agents
- `agentic-workflows` — GitHub Agentic Workflows (gh-aw) - Create, debug, and upgrade AI-powered workflows with intelligent prompt routing
- `architect` — Solution architecture and system design — designs features, chooses patterns, defines component boundaries, and produces implementation blueprints. Use for feature design, pattern selection, component boundaries, and architecture analysis.
- `Comet Squad` — AI team for Comet MVU framework development. Manages the src/Comet subproject.
- `designer` — Visual/graphical design — color palettes, typography, spacing, branding, iconography, accessibility contrast, and visual hierarchy. Use for visual design — color palettes, typography, branding, design tokens, and accessibility contrast.
- `developer` — .NET/C# code implementation — writes CQRS handlers, API endpoints, Blazor components, EF Core data access. Use for all .NET code changes including refactoring, builds, and verification.
- `expert-reviewer` — Expert .NET MAUI DevFlow code reviewer. Multi-model review with adversarial consensus.
- `programmer` — Programming and code implementation — writes, edits, builds, and tests code. Use for hands-on implementation once the approach is decided, without architecture or planning work.
- `reviewer` — Code quality and architecture review specialist — reviews for SOLID principles, CQRS patterns, security issues, and architecture compliance. Use for code quality, SOLID, CQRS compliance, security review, architecture audit, and ISO/IEC 25010 finding attribution.
- `security` — Security reviewer and blast-radius analyst — reviews authentication, authorization, secrets handling, data exposure, and dependency risk, and reports severity-ranked findings without editing code. Use for blast-radius and exposure analysis, threat modelling, OWASP review, and dependency and secret audits.
- `tester` — Unit and integration test specialist — writes MSTest tests, Moq mocks, Reqnroll BDD scenarios. Use for MSTest/Reqnroll test design, BDD scenarios, and integration test strategy.
- `ui-developer` — Frontend UI development — builds Blazor components, MAUI pages, layouts, CSS/styling, and UX patterns. Use for Blazor/MAUI components, layouts, CSS/styling, and UX patterns.
- `ui-tester` — Playwright E2E testing specialist — writes UI tests, accessibility checks, visual regression tests. Use for Playwright E2E tests, accessibility checks, and visual regression.
- `winui-dev` — Builds WinUI 3 desktop applications using Windows App SDK, XAML, and C#. Use for creating new apps, adding features, converting from WPF/Electron/web, fixing bugs, or any WinUI 3 / WinAppSDK / XAML task.
- `writer` — Documentation specialist — writes XML documentation, API docs, README files, architecture decision records, and technical documentation. Use for XML docs, READMEs, ADRs, and technical prose.

<!-- ai-workspace-manager:installed:end -->
