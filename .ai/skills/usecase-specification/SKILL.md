---
name: usecase-specification
description: Drafts and finalizes use case specifications for bounded contexts. Use when a bounded context's main flow becomes clear during design sessions, or to extract use cases from existing documents. Trigger whenever the user says "use case", "user flow", "system interaction", or asks to document what the system does. Outputs structured specifications with main flow, alternate flows, and Gherkin scenarios to docs/bounded-contexts/{BC}/use-cases.md.
---

# Use Case Specification

Captures bounded context use cases as they emerge and produces structured specifications with Gherkin.

## Output Format

Each bounded context gets its own file:
`docs/bounded-contexts/{BoundedContext}/use-cases.md`

```markdown
## [DRAFT] Use Cases — {BoundedContext}

### UC-{BC}-{NNN}: {Use Case Name}

**Actor:** {Primary actor}
**Goal:** {What the actor wants to achieve}
**Preconditions:** {What must be true before the use case starts}

**Main Flow:**
1. {Step 1}
2. {Step 2}
3. {Step N}

**Alternate Flows:**
- AF-{BC}-{NNN}.1: {Condition} → {Steps}
- AF-{BC}-{NNN}.2: {Condition} → {Steps}

**Postconditions:** {What is true after successful completion}

**Gherkin:**
```gherkin
Feature: {Use Case Name}

  Scenario: {Main flow scenario name}
    Given {precondition}
    When {action}
    Then {outcome}

  @AF-{BC}-{NNN}.1
  Scenario: {Alternate flow 1 scenario}
    Given {precondition}
    When {triggering condition}
    Then {alternate outcome}
```
```

Mark as `[DRAFT]` until finalization. Remove markers only during the finalization pass.

## Identifiers

Use case and alternate-flow identifiers follow `.ai/reference/traceability.md` — the source of
truth for formats, allocation and legacy handling.

- `{BC}` is the bounded context's name as a single PascalCase token, copied verbatim from
  `UBIQUITOUS_LANGUAGE.md`. `{NNN}` is exactly three digits, zero-padded from `001`. `{n}` is an
  unpadded alternate-flow number within its use case.
- `UC-` and `US-` are independent numbering spaces: `UC-Budget-014` and `US-Budget-014` may
  coexist in one context without collision.
- Allocate when the entry is first written: the next `{NNN}` **above the high-water mark** of the
  context; alternate flows take the next `.n` within their use case. Allocation is append-only —
  deleting an alternate flow never frees its number, and reordering never moves one. Retired
  identifiers are tombstoned in a `## Retired IDs` section of the owning file.
- Every alternate flow appears twice: as `- AF-{BC}-{NNN}.{n}: {Condition} → {Steps}` in the
  list, and as an `@AF-{BC}-{NNN}.{n}` tag directly above the scenario that realizes it.
- **Main-flow scenarios carry no traceability tag.** There is no `@UC-…` form in this scheme: a
  main flow is traced through the slice that implements it, and its scenario's existence cannot be
  checked mechanically. This is a deliberate gap, not an omission.
- A use-case document with no identifiers is in **legacy mode** — a supported state, not a defect.
  It gains identifiers on the next finalization pass, and anything referencing it is labelled
  afterwards, never before.

## Mode 0 — Extraction from Documents

Use when existing documents are provided.

1. Read all input documents
2. Identify every user-initiated workflow or system interaction
3. Draft a use case entry for each workflow — mark gaps `[OPEN]`
4. Identify the bounded context each use case belongs to
5. Write draft files to `docs/bounded-contexts/{BC}/use-cases.md`
6. Announce: "Extracted N use cases — M gaps marked [OPEN]."

## Mode 1 — Progressive Capture (During Sessions)

Use as soon as the main flow of a bounded context resolves during design interrogation.

When a main flow becomes clear:
1. Create `docs/bounded-contexts/{BC}/use-cases.md` if it does not exist
2. Write the draft use case entry (main flow captured — alternate flows marked `[OPEN]`)
3. Announce: "📝 Draft use case logged to `docs/bounded-contexts/{BC}/use-cases.md`."

## Finalization Pass

Run when interrogation is complete.

1. Read all draft use case files
2. Complete all alternate flows — every `[OPEN]` must be resolved or explicitly deferred
3. Allocate identifiers to every use case and alternate flow that lacks one. For an unlabelled
   legacy document this is the migration: the next `{NNN}` above the context's high-water mark,
   alternate flows numbered `.1`, `.2`, … top to bottom in current document order. Numbers are
   frozen once written (`.ai/reference/traceability.md`)
4. Add Gherkin scenarios for every main flow and every alternate flow. An alternate-flow scenario
   carries exactly one `@AF-{BC}-{NNN}.{n}` tag on the line directly above `Scenario:`, matching
   its list item; main-flow scenarios carry no traceability tag
5. Ensure all terms match the finalized `UBIQUITOUS_LANGUAGE.md`
6. Remove all `[DRAFT]` markers
7. Write the finalized files

Announce:
> ✅ Use case specifications finalized — N use cases, M Gherkin scenarios written.
