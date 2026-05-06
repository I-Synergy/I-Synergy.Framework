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

### UC-{N}: {Use Case Name}

**Actor:** {Primary actor}
**Goal:** {What the actor wants to achieve}
**Preconditions:** {What must be true before the use case starts}

**Main Flow:**
1. {Step 1}
2. {Step 2}
3. {Step N}

**Alternate Flows:**
- {AF-1}: {Condition} → {Steps}

**Postconditions:** {What is true after successful completion}

**Gherkin:**
```gherkin
Feature: {Use Case Name}

  Scenario: {Main flow scenario name}
    Given {precondition}
    When {action}
    Then {outcome}
```
```

Mark as `[DRAFT]` until finalization. Remove markers only during the finalization pass.

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
3. Add Gherkin scenarios for every main flow and every alternate flow
4. Ensure all terms match the finalized `UBIQUITOUS_LANGUAGE.md`
5. Remove all `[DRAFT]` markers
6. Write the finalized files

Announce:
> ✅ Use case specifications finalized — N use cases, M Gherkin scenarios written.
