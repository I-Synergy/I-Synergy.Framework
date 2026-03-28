# Grill Me — Definition of Done

Run this checklist before closing a grill-me session.

---

## Progressive Document Capture

- [ ] `docs/architecture/session-decisions.md` was created at session start
- [ ] Every resolved decision was logged to `session-decisions.md` during the session
- [ ] Domain terms were written to `UBIQUITOUS_LANGUAGE.md` (draft) as they emerged
- [ ] Use case drafts were written to `docs/use-cases/*.md` as flows resolved
- [ ] User story drafts were written to `docs/user-stories/*.md` as acceptance criteria emerged
- [ ] All `[DRAFT]` markers removed after Steps 4–6 finalization

---

## Coverage Complete

- [ ] All relevant decision dimensions have been addressed or consciously skipped
- [ ] A reason was stated for each skipped dimension (e.g., "Out of scope for this session")
- [ ] No major dependency between decisions was left unresolved

**Dimensions to verify:**

| Dimension | Status |
|-----------|--------|
| Purpose & Scope | Resolved / Deferred / Skipped |
| Domain Model | Resolved / Deferred / Skipped |
| Bounded Contexts | Resolved / Deferred / Skipped |
| Data Flow | Resolved / Deferred / Skipped |
| Integration Points | Resolved / Deferred / Skipped |
| Failure Modes | Resolved / Deferred / Skipped |
| Non-Functionals | Resolved / Deferred / Skipped |
| Migration & Rollout | Resolved / Deferred / Skipped |

---

## Decision Tree Complete

- [ ] Every branch that was opened during the session is in one of these states:
  - **Resolved** — decision made, no open sub-questions
  - **Deferred** — user consciously chose to decide later, reason documented
  - **Blocked** — waiting on external input, blocker named explicitly
- [ ] No branch was left implicitly hanging (e.g., user gave a vague answer and was not pressed)
- [ ] All deferred decisions have a stated reason

---

## Codebase Exploration

- [ ] All questions that could be answered from the codebase were explored before asking the user
- [ ] Any discovered conflicts between the plan and the current codebase were surfaced

---

## Architecture Document Verification (MANDATORY)

**Every checkbox below is a hard gate — a missing check = the session is NOT complete.**

### Forbidden Patterns — None May Be Present

- [ ] **No SQL DDL** — no `CREATE TABLE`, `CREATE SCHEMA`, `ALTER TABLE`, `CREATE INDEX`
- [ ] **No hardcoded connection strings** — no `Host=localhost;Username=x;Password=...`
- [ ] **No MediatR** — no `_mediator.Send()`, no `services.AddMediatR()`
- [ ] **No `DateTime`** — only `DateTimeOffset` used throughout

### Instruction File Alignment — Core (Always Required)

- [ ] `clean-architecture-layers.instructions.md` — layer boundaries respected
- [ ] `domain-driven-design.instructions.md` — aggregates, entities, value objects correct
- [ ] `naming-conventions.instructions.md` — `{Domain}DataContext` (not DbContext), snake_case DB
- [ ] `naming-conventions.instructions.md` — project naming and folder structure correct
- [ ] `application-layer.instructions.md` — application layer patterns followed
- [ ] `infrastructure-layer.instructions.md` — infrastructure layer patterns followed

### Instruction File Alignment — CQRS + Errors

- [ ] `cqrs-implementation.instructions.md` — `ICommand<TResponse>`, `ICommandHandler<T,R>`, no MediatR
- [ ] `error-recovery.instructions.md` — `Result<T>` pattern, no exceptions for business logic
- [ ] `property-validation.instructions.md` — FluentValidation in Application layer

### Instruction File Alignment — Event Sourcing (if chosen)

- [ ] `event-sourced-aggregate.instructions.md` — 8-file blueprint per aggregate, event-first pattern, repository contract
- [ ] `event-sourcing-projections.instructions.md` — `IProjector`, `IProjectionReader<T>`, no SQL DDL
- [ ] `documentstore-integration.instructions.md` — DocumentStore usage patterns
- [ ] `documentstore-configuration-migration.instructions.md` — migration patterns
- [ ] `entityframework-core.instructions.md` — `IEntityTypeConfiguration<T>` for all projections

### Instruction File Alignment — Data + Infrastructure

- [ ] `postgresql-patterns.instructions.md` — snake_case, indexes, query patterns
- [ ] `data-access-patterns.instructions.md` — repository patterns
- [ ] `multi-tenancy.instructions.md` — `ICurrentTenant` used, TenantId always filtered
- [ ] `timestamp-handling.instructions.md` — DateTimeOffset in all projections and entities

### Instruction File Alignment — Integration + Messaging (if mailbox chosen)

- [ ] `integration-event-worker.instructions.md` — `Microsoft.NET.Sdk.Worker`, `PeriodicTimer`, `IServiceScopeFactory`
- [ ] `background-jobs.instructions.md` — background service patterns

### Instruction File Alignment — Orchestration + Auth

- [ ] `aspire-orchestration.instructions.md` — `.WithReference()`, no hardcoded strings
- [ ] `keycloak-authentication.instructions.md` — JWT propagation, bearer-only services (if Keycloak chosen)
- [ ] `security-patterns.instructions.md` — auth patterns aligned

### Instruction File Alignment — Frontend (if applicable)

- [ ] `blazor-fluentui.instructions.md` — FluentUI v4.13.2+ component patterns (if Blazor chosen)

### Instruction File Alignment — Observability + Quality

- [ ] `structured-logging.instructions.md` — structured log entries with correlation IDs
- [ ] `testing-patterns.instructions.md` — per-domain `Tests.{Domain}` project structure
- [ ] `performance-optimization.instructions.md` — SLOs documented

### Mandatory Sections Present in Architecture Document

- [ ] **CQRS Implementation section** — handler interfaces + `Result<T>` pattern
- [ ] **Multi-Tenancy Strategy section** — `ICurrentTenant` + tenant-per-row explanation
- [ ] **Testing Strategy section** — per-domain test project structure (14+ files)
- [ ] **Worker Projects section** — SDK, `PeriodicTimer`, `IServiceScopeFactory` (if mailbox used)
- [ ] **Naming Conventions section** — `{Domain}DataContext`, snake_case mapping
- [ ] **Connection String Management section** — Aspire `.WithReference()` pattern
- [ ] **Related Skills section** — skill links for implementation agents
- [ ] **Architecture Document Verification appendix** — completed checklist

### Pattern References in Architecture Document

Every section that describes a technical implementation must contain a `**Pattern Reference:**` line.
Verify these are present:

| Section | Expected Pattern Reference |
|---------|--------------------------|
| Bounded Contexts / Aggregates | `domain-driven-design.instructions.md` |
| Projections | `event-sourcing-projections.instructions.md`, `entityframework-core.instructions.md` |
| Event Store | `documentstore-integration.instructions.md` |
| CQRS | `cqrs-implementation.instructions.md` |
| Mailbox / Worker | `integration-event-worker.instructions.md` |
| Aspire / Service Discovery | `aspire-orchestration.instructions.md` |
| Authentication / JWT | `keycloak-authentication.instructions.md` |
| Observability / Logging | `structured-logging.instructions.md` |
| Frontend | `blazor-fluentui.instructions.md` |
| Testing | `testing-patterns.instructions.md` |
| Multi-Tenancy | `multi-tenancy.instructions.md` |
| Naming | `naming-conventions.instructions.md` |

---

## Session Deliverables (MANDATORY)

**Every checkbox below is a hard gate — a missing check = the session is NOT complete.**

### Ubiquitous Language
- [ ] `UBIQUITOUS_LANGUAGE.md` written to working directory
- [ ] All domain terms surfaced during interrogation are captured
- [ ] Synonyms, homonyms, and vague terms are resolved or flagged
- [ ] Canonical terms used consistently in all subsequent deliverables

### Use Case Specifications
- [ ] One use case document written per bounded context
- [ ] Every resolved decision with a user-observable outcome has a corresponding use case
- [ ] Every use case has all 8 fields (Title, Summary, Actors, Triggers, Main Flow, Alternate Flows, Pre/Postconditions)
- [ ] Every error condition has an Alternate Flow AND a Gherkin scenario
- [ ] Actors use canonical terms from `UBIQUITOUS_LANGUAGE.md` — no "User", "Admin", "System"

### User Stories
- [ ] One user story document written per bounded context
- [ ] Every use case is decomposed into INVEST-compliant sprint-ready stories
- [ ] Every story has ≥3 acceptance criteria (happy path, validation failure, authorisation failure)
- [ ] Every acceptance criterion has a Gherkin scenario (`ACN — ...`)
- [ ] Roles use canonical terms from `UBIQUITOUS_LANGUAGE.md`

### Deferred Deliverables
- [ ] Any skipped deliverable is documented with an explicit reason (user consent required)

---

## Summary Delivered

- [ ] A closing summary was written using the standard format
- [ ] All Decisions Made are listed with their chosen approach
- [ ] All Deferred Decisions are listed with their reason
- [ ] All Remaining Open Questions are listed (if any)
- [ ] **Architecture Document Verification section included** in the summary
- [ ] **Session Deliverables table included** in the summary (all four rows: architecture doc + 3 skills)
- [ ] Recommended Next Steps are provided

## Handoff Complete

**Every checkbox below is a hard gate — a missing check = the session is NOT complete.**

- [ ] Recommended Next Steps reference [`solution-generator`](../../../skills/solution-generator/SKILL.md) with explicit **Option B** input — architecture document path named
- [ ] Recommended Next Steps reference [`multi-agent-orchestration`](../../../skills/multi-agent-orchestration/SKILL.md) as the generator's coordination mechanism
- [ ] Recommended Next Steps reference [`vertical-slices`](../../../skills/vertical-slices/SKILL.md) for implementing user stories after initial solution generation
- [ ] Recommended Next Steps reference [`gap-review`](../../../skills/gap-review/SKILL.md) as the mandatory post-generation validation step

---

## Final Sign-Off

All sections above must be fully checked before the session is closed.

**If ANY checkbox is unchecked → the session is NOT complete.**
