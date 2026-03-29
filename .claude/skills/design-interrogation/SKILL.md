---
name: design-interrogation
description: >
  Structured design interrogation — interview the user relentlessly about a plan,
  architecture, or technical decision until every branch of the decision tree is
  resolved. Use whenever a user says "grill me", "stress-test my plan", "poke holes
  in this", "challenge my design", "interview me about this design", or wants a
  rigorous review of any design, architecture, or technical proposal — even if they
  don't use these exact words. When in doubt, trigger this skill.
---

# Design Interrogation

Relentless, structured interrogation of a plan or design. Walk every branch of the
decision tree, resolve dependencies between decisions one-by-one, and reach a shared
understanding before closing.

---

## What This Skill Does

- **Interviews** the user on every aspect of their plan, one focused question at a time
- **Maps** the decision tree, tracking which branches are resolved, deferred, or blocked
- **Recommends** a concrete answer with reasoning for each question asked
- **Explores** the codebase to answer questions that can be resolved without user input
- **Summarizes** all decisions and open items when interrogation is complete

---

## Interrogation Protocol

### Session Start — Load Required Skills

Before asking the first question, load these three skills. They are active throughout the
entire session — not just at the end:

| Skill | Load | Role During Session |
|-------|------|---------------------|
| [`ubiquitous-language`](../ubiquitous-language/SKILL.md) | **Required — load first** | Captures domain terms as they emerge; draft written to `UBIQUITOUS_LANGUAGE.md` immediately |
| [`usecase-specification`](../usecase-specification/SKILL.md) | **Required — load second** | Drafts use cases as flows resolve; each bounded context gets a draft file |
| [`user-story`](../user-story/SKILL.md) | **Required — load third** | Drafts stories as acceptance criteria are agreed; finalized with Gherkin at session end |

Then create the decision log (see **Progressive Document Capture** below) and start Phase -1.

---

### Phase -1: Document Intake

Before asking any question, check if the user has existing documents to share.

> Before we start: do you have any existing documents I should read first?
> (business case, PRD, architecture spec, migration plan, functional design)
> If so, share them — I'll save them to `docs/input/` and extract what's already decided.

If documents are provided:
1. Save them to `docs/input/` (e.g. `docs/input/business-case.md`)
2. Read and analyze them — identify any BCs already named in the documents
3. Extract all decisions already made — log them to `session-decisions.md` as **Pre-Resolved**
4. Run extractions **in parallel** using the skills below — do NOT define extraction logic here:
   - If BCs already identifiable → spawn one extractor per BC, each running both skills in parallel
   - If BCs not yet known → run both skills in parallel (single BC assumed, renamed after Phase 0)
   - → [`usecase-specification/SKILL.md` — Mode 0](../usecase-specification/SKILL.md) for use cases
   - → [`user-story/SKILL.md` — Mode 0](../user-story/SKILL.md) for user stories
5. Announce: "Extracted [N] use cases and [M] user stories — [X] gaps marked [OPEN]."
6. Proceed to Phase 0 with only the open questions remaining

If no documents → proceed directly to Phase 0.

---

### Phase 0: Strategic Anchor

Before starting depth-first interrogation, ask these four questions together in a single
response. They establish the business context and architectural shape in one round and
allow the rest of the session to be focused — or skipped — accordingly.

Present all four at once, clearly numbered A1–A4:

---

**A1 — Business Case**

Is this greenfield (new system), migration (replacing an existing system), or
an extension of an existing system?

**My recommendation:** [infer from context the user provided]

---

**A2 — Purpose & Scope**

What are we building and what is explicitly out of scope?

**My recommendation:** [scoped interpretation based on what the user described]

---

**A3 — Architecture Shape**

Event-driven with event sourcing, event-driven with state-based persistence,
or direct request/response?

**My recommendation:** [based on domain complexity and existing stack]

---

**A4 — Hard Constraints**

What is non-negotiable? (team size, deadline, existing stack, compliance,
cloud vs. on-premise)

**My recommendation:** [infer from codebase — check `Directory.Packages.props` and
Aspire AppHost for existing choices before asking]

---

After all four are answered, adapt the interrogation order based on A1:
- **Greenfield** → standard dimension order (1 → 2 → 3 → … → 8)
- **Migration** → pull Dimension 8 (Migration & Rollout) forward after Dimension 2
- **Extension** → run codebase-first checks before any question; most decisions are pre-resolved

Then proceed to Phase 1 if 2+ BCs were identified, otherwise depth-first on the
highest-impact open dimension.

---

### Phase 1: Parallel BC Interrogation

When Phase 0 (A2) identifies 2 or more bounded contexts, spawn one sub-agent per BC
and run them in parallel. Each agent covers Dimensions 2–6 for its BC only.

**Trigger:**
```
BCs identified in A2 ≥ 2?
  YES → Spawn N BC agents in parallel (one per BC)
  NO  → Continue single-agent depth-first from Phase 0
```

**Per-BC dimensions:**

| Dimension | BC-specific questions |
|-----------|----------------------|
| 2 — Domain Model | Aggregates, entities, value objects for this BC |
| 3 — BC Internals | Scope boundary — what is inside this BC? |
| 4 — Data Flow | Commands, queries, events flowing through this BC |
| 5 — Integration Points | Which external systems does this BC touch? |
| 6 — Failure Modes | What happens when this BC's core component fails? |

Each BC agent writes immediately to:
- `session-decisions.md` — its own BC section
- `docs/bounded-contexts/{BC}/use-cases.md` — draft use cases as flows resolve
- `docs/bounded-contexts/{BC}/user-stories.md` — draft stories as criteria are agreed
- Follows the depth-first rule within its own BC

**Coordination agent** (main session) batches one open question per BC per round:

```
**BC: OrderManagement — Question 1 — Domain Model**
[question + recommendation]

**BC: Inventory — Question 1 — Domain Model**
[question + recommendation]
```

**After all BC agents complete:**
1. Cross-BC integration questions (how do BCs communicate?)
2. Dimension 7 — Non-functionals (global, single agent)
3. Dimension 8 — Migration & Rollout (global, single agent)

---

### Question Format

Ask one question at a time. Each question must be specific, answerable, and accompanied
by a concrete recommendation. Use this format:

---

**Question N — [Topic]**

[The question, clearly stated.]

**My recommendation:** [Your concrete recommendation and the reasoning behind it.]

---

Do not list multiple questions at once. Depth-first: fully resolve one branch before
moving to the next sibling.

### Decision Dimensions

Cover these areas in dependency order — skip dimensions that are clearly irrelevant
or already resolved:

| # | Dimension | Key Questions |
|---|-----------|---------------|
| 1 | **Purpose & Scope** | What is being built? What is explicitly out of scope? |
| 2 | **Domain Model** | What are the aggregates, entities, and value objects? |
| 3 | **Bounded Contexts** | Where are domain boundaries? How do contexts communicate? |
| 4 | **Data Flow** | How do commands, queries, and events move through the system? |
| 5 | **Integration Points** | What external systems, APIs, or services are involved? |
| 6 | **Failure Modes** | What happens when each component fails? |
| 7 | **Non-Functionals** | Performance, scalability, security, observability requirements |
| 8 | **Migration & Rollout** | How is this deployed? What is the rollout strategy? |

### Codebase Exploration

Before asking the user about something that can be determined from the codebase,
explore the codebase first. Announce what you found:

> I checked the codebase and found that X is already handled by Y — moving on.

Use the Read, Grep, and Glob tools to inspect relevant files. Do not ask the user
to confirm what the code already tells you.

### Decision Tree Tracking

Maintain a mental model of the decision tree during interrogation:

- **Open** — decision not yet reached
- **Resolved** — decision made, no open sub-questions
- **Deferred** — user consciously chose to decide later
- **Blocked** — cannot proceed without external information

When a decision opens new sub-questions (e.g., choosing an event-driven approach
raises questions about the event schema), explore those sub-questions before moving
to the next top-level topic.

---

## Progressive Document Capture

Do not wait until the end of the session to write output. Capture decisions, domain terms, use
cases, and user stories **as they emerge** — this ensures nothing is lost if the session is
interrupted, and makes Steps 4–6 a finalization pass rather than a cold generation from memory.

### Session Start — Create Decision Log

Before asking the first question, create the decision log file:

```
docs/session-decisions.md
```

Use this header:

```markdown
# Session Decisions — {SolutionName}

> Live decision log. Updated after each resolved question. Finalized into the full architecture document at session end.

## Decisions

| # | Dimension | Decision | Rationale |
|---|-----------|----------|-----------|
```

### After Each Resolved Question

Immediately append to `session-decisions.md`:

```markdown
| {N} | {Dimension} | {chosen approach} | {brief rationale} |
```

Announce:

> 📝 Logged to `session-decisions.md`.

### When a Domain Term Emerges

As soon as a term is used and agreed upon, append it to `UBIQUITOUS_LANGUAGE.md` (create the
file if it does not yet exist, mark it `[DRAFT]` in the title until the full ubiquitous-language
skill finalizes it):

```markdown
## [DRAFT] Ubiquitous Language — {SolutionName}

| Term | Definition | Bounded Context | Notes |
|------|-----------|-----------------|-------|
| {Term} | {Definition} | {BC} | [DRAFT] |
```

### When a Use Case Becomes Clear

As soon as the main flow of a bounded context resolves, use `usecase-specification`
skill **Mode 1** to write the draft entry to
`docs/bounded-contexts/{BoundedContext}/use-cases.md`.

→ Follow the draft entry format defined in
  [`usecase-specification/SKILL.md` — Mode 1](../usecase-specification/SKILL.md).
  Do NOT use a custom format here.

### When User Stories Emerge

As soon as acceptance criteria for a feature are agreed, use `user-story`
skill **Mode 1** to write the draft entry to
`docs/bounded-contexts/{BoundedContext}/user-stories.md`.

→ Follow the draft entry format defined in
  [`user-story/SKILL.md` — Mode 1](../user-story/SKILL.md).
  Do NOT use a custom format here.

### Steps 4–6 Become Finalization

When interrogation is complete, Steps 4–6 do not generate from scratch — they **complete and
finalize** the drafts already written:

- Remove all `[DRAFT]` markers
- Fill gaps in use cases (alternate flows, Gherkin)
- Add Gherkin to user stories, validate INVEST
- Complete the architecture document from `session-decisions.md`

---

## Architecture Document Verification (MANDATORY)

Before writing the architecture document, load `patterns/document-template.md` — it is the
structural scaffold every generated document must follow.

### Step 1: Load Relevant Instruction Files

Load the instruction files that apply to the technology choices made during interrogation.
The full table of instruction files per technology category lives in `checklists/dod.md`
→ section "Instruction File Alignment".

Quick reference — always required:

| Category | Instruction Files |
|----------|-----------------|
| Core Architecture | `clean-architecture-layers`, `domain-driven-design`, `naming-conventions`, `application-layer`, `infrastructure-layer` |
| CQRS + Errors | `cqrs-implementation`, `error-recovery`, `property-validation` |
| Event Sourcing | `event-sourced-aggregate`, `event-sourcing-projections`, `documentstore-integration`, `documentstore-configuration-migration`, `entityframework-core` |
| Data | `postgresql-patterns`, `data-access-patterns`, `multi-tenancy`, `timestamp-handling` |
| Messaging | `integration-event-worker`, `background-jobs` |
| Orchestration + Auth | `aspire-orchestration`, `keycloak-authentication`, `security-patterns` |
| Frontend | `blazor-fluentui` |
| Quality | `structured-logging`, `testing-patterns`, `performance-optimization` |

All instruction files live in `.github/instructions/`.

---

### Step 2: Check for Violations

Scan the draft architecture document for these **FORBIDDEN patterns**:

| ❌ Forbidden | ✅ Replace With |
|-------------|----------------|
| SQL DDL (`CREATE TABLE`, `CREATE SCHEMA`, `ALTER TABLE`, `CREATE INDEX`) | EF Core `IEntityTypeConfiguration<T>` — see `patterns/document-template.md` §4.3 |
| Hardcoded connection strings (`Host=...;Password=...`) | Aspire `.WithReference()` — see `patterns/document-template.md` §4.6 |
| MediatR (`_mediator.Send()`, `services.AddMediatR()`) | Direct `ICommandHandler<T,R>` injection — see `patterns/document-template.md` §4.4 |
| `DateTime` (timezone-naive) | `DateTimeOffset` — ref `timestamp-handling.instructions.md` |
| Pattern section with no `**Pattern Reference:**` line | Add reference to relevant `.github/instructions/*.instructions.md` |
| Missing mandatory sections | Add missing sections — see `patterns/document-template.md` for the full list |

For before/after examples of each violation, see `examples/violations.md` (Violations 8–11).

---

### Step 3: Document Pattern Rationale

For each major pattern, include an ADR explaining **why** this implementation was chosen:

> **Why EF Core IEntityTypeConfiguration instead of SQL DDL?**
>
> 1. **Code-First Migrations:** Schema evolves with code via `dotnet ef migrations add`
> 2. **Type Safety:** Compile-time validation of entity structure
> 3. **Testability:** In-memory database provider for unit tests
> 4. **snake_case Mapping:** Automatic C# PascalCase → PostgreSQL snake_case via `HasColumnName`
> 5. **Instruction Alignment:** Infrastructure agents implement exactly what's documented here

---

### Step 4: Finalize Session Deliverables (MANDATORY)

The three required skills were loaded at session start and have been capturing drafts throughout
the interrogation. This step **finalizes** those drafts — it is not a cold generation from memory.

| # | Deliverable | Skill | Output File | Action |
|---|-------------|-------|-------------|--------|
| 1 | **Ubiquitous Language Glossary** | [`ubiquitous-language`](../ubiquitous-language/SKILL.md) | `UBIQUITOUS_LANGUAGE.md` | Complete draft — remove `[DRAFT]`, resolve ambiguities, add missing terms |
| 2 | **Use Case Specifications** | [`usecase-specification`](../usecase-specification/SKILL.md) | `docs/bounded-contexts/{BC}/use-cases.md` | Complete draft — fill alternate flows, add Gherkin for every flow |
| 3 | **User Stories** | [`user-story`](../user-story/SKILL.md) | `docs/bounded-contexts/{BC}/user-stories.md` | Complete draft — add Gherkin, validate INVEST, ensure ≥3 criteria per story |

#### Finalization Order

Execute in this sequence — each builds on the previous:

1. **Ubiquitous Language first** — finalize all draft terms; canonical vocabulary must be locked
   before use cases and stories are completed.
   → Follow `ubiquitous-language/SKILL.md` → remove all `[DRAFT]` markers → write final file

2. **Use Case Specifications second** — complete all draft use cases using finalized terminology.
   Add Gherkin for every main and alternate flow.
   → Follow `usecase-specification/SKILL.md` → remove all `[DRAFT]` markers → write final files

3. **User Stories third** — complete all draft stories using finalized terminology and use cases.
   Add Gherkin traced to every acceptance criterion. Validate INVEST.
   → Follow `user-story/SKILL.md` Mode 2 (Finalization) → remove all `[DRAFT]` markers → write final files

#### Announce Each Deliverable

Before starting each deliverable, announce it:

> **Deliverable 1 of 3 — Ubiquitous Language**
> Finalizing draft terms from the interrogation…

After completing each:

> ✅ `UBIQUITOUS_LANGUAGE.md` finalized — N terms defined, M ambiguities resolved.

#### Skip Only With Explicit User Consent

If the user says "skip use cases" or "no stories needed", mark the deliverable as **Deferred**
and include it in the closing summary. Never silently skip.

---

### Step 5: Run the Definition of Done

Before closing, run every checkbox in `checklists/dod.md`. It is the single source of truth
for session completeness — covering decision tree coverage, instruction file alignment,
mandatory document sections, and all session deliverables.

---

### Step 6: Solution Generation Handoff

The session deliverables are direct pipeline inputs for solution generation and feature implementation:

| Document | Consumed By | Role |
|----------|-------------|------|
| `docs/architecture/{SolutionName}-architecture.md` | [`solution-generator`](../solution-generator/SKILL.md) | **Option B** — Architecture Document input — generator derives all parameters from it |
| `UBIQUITOUS_LANGUAGE.md` | [`solution-generator`](../solution-generator/SKILL.md) | Domain naming, aggregate identification |
| `docs/bounded-contexts/{BC}/use-cases.md` | [`vertical-slices`](../vertical-slices/SKILL.md) + [`usecase-specification`](../usecase-specification/SKILL.md) | Each use case → Gherkin `.feature` files + blueprint JSON |
| `docs/bounded-contexts/{BC}/user-stories.md` | [`vertical-slices`](../vertical-slices/SKILL.md) | Each story → individual vertical slice blueprint JSON |
| `docs/input/` | [`solution-generator`](../solution-generator/SKILL.md) | Source context — read-only, never overwritten |

**Pipeline execution order:**
1. Run [`solution-generator`](../solution-generator/SKILL.md) — pass architecture document as **Option B** input
2. The generator triggers [`multi-agent-orchestration`](../multi-agent-orchestration/SKILL.md) — creates `.ai/` folder, spawns parallel agents per layer
3. After initial solution: translate each user story to a [`vertical-slices`](../vertical-slices/SKILL.md) blueprint JSON for feature-by-feature implementation
4. Run [`gap-review`](../gap-review/SKILL.md) — mandatory after generation, validates against original design decisions

---

## Closing Summary

When all branches are Resolved or Deferred, AND verification is complete, present a structured summary:

```
## Interrogation Summary

### Decisions Made
- [Topic]: [chosen approach and brief rationale]

### Deferred Decisions
- [Topic]: Deferred because [reason]

### Remaining Open Questions
- [Any unknowns that require external input]

### Architecture Document Verification
✅ All checks in `checklists/dod.md` passed

### Session Deliverables
| Deliverable | Status | File |
|-------------|--------|------|
| Architecture Document | ✅ Complete | `docs/architecture/{SolutionName}-architecture.md` |
| Ubiquitous Language | ✅ Complete / ⏸️ Deferred | `UBIQUITOUS_LANGUAGE.md` |
| Use Case Specifications | ✅ Complete / ⏸️ Deferred | `docs/bounded-contexts/{BC}/use-cases.md` |
| User Stories | ✅ Complete / ⏸️ Deferred | `docs/bounded-contexts/{BC}/user-stories.md` |

### Recommended Next Steps
1. Run [`solution-generator`](../solution-generator/SKILL.md) — pass `docs/architecture/{SolutionName}-architecture.md` as **Option B** input; generator triggers [`multi-agent-orchestration`](../multi-agent-orchestration/SKILL.md)
2. For each BC: translate `docs/bounded-contexts/{BC}/user-stories.md` to a [`vertical-slices`](../vertical-slices/SKILL.md) blueprint JSON for feature-by-feature implementation
3. Run [`gap-review`](../gap-review/SKILL.md) after generation to verify against original design decisions
```

---

## Example Exchange

**User:** grill me on my new notification service

**Claude:**

---

**Question 1 — Delivery Channels**

Which notification channels must the service support at launch — email only, or
also SMS, push notifications, or in-app?

**My recommendation:** Start with email and in-app. SMS adds third-party costs and
compliance complexity (GDPR/TCPA). Push requires device registration infrastructure.
Both can be added in v2 once the channel abstraction is validated.

---

After the user answers, Claude follows the dependency tree. If the user says
"email and in-app", the next question targets the email provider decision — not
an unrelated topic.

---

## Supporting Files

| File | Load When |
|------|-----------|
| [patterns/questioning.md](patterns/questioning.md) | Formulating questions, handling vague answers, codebase-first rule |
| [patterns/decision-tree.md](patterns/decision-tree.md) | Mapping and traversing the decision tree, handling dependencies |
| [patterns/document-template.md](patterns/document-template.md) | **Before writing the architecture document** — use as the structural scaffold; defines all mandatory sections, forbidden patterns, and required instruction file references |
| [checklists/dod.md](checklists/dod.md) | Before closing the session — verify all branches, instruction file alignment, and mandatory document sections are complete |
| [examples/violations.md](examples/violations.md) | When the session feels off, the user pushes back, or the generated document contains SQL DDL / hardcoded strings / missing cross-references |

---

## Required Skills — Load at Session Start

These three skills are **required** and loaded before the first question.
They capture drafts progressively and are finalized in Step 4.

→ See **Session Start — Load Required Skills** in the Interrogation Protocol above.

---

## Related Skills

### Pipeline Skills (Post-Interrogation)

→ See **Step 6: Solution Generation Handoff** in the Architecture Document Verification section above.

### Optional / Conditional Skills

| Skill | When to Load |
|-------|-------------|
| [`architect`](../architect/SKILL.md) | Architectural decisions need to be formalized beyond what the architecture document covers |
| [`domain-modeling`](../domain-modeling/SKILL.md) | Domain model decisions (aggregates, entities, value objects) need to be worked out in depth |
| [`prd-author`](../prd-author/SKILL.md) | After interrogation, produce a full PRD and submit as a GitHub issue |
