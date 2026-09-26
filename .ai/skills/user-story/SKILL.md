---
name: user-story
description: Drafts and finalizes user stories with Gherkin acceptance criteria for bounded contexts. Use when acceptance criteria for a feature are agreed during design sessions, or to extract stories from existing documents. Trigger whenever the user says "user story", "as a user", "acceptance criteria", or asks to define what a feature needs to do. Validates stories against INVEST criteria. Outputs structured stories with Gherkin to docs/bounded-contexts/{BC}/user-stories.md.
---

# User Story

Captures user stories as they emerge and produces INVEST-validated stories with Gherkin acceptance criteria.

## Output Format

Each bounded context gets its own file:
`docs/bounded-contexts/{BoundedContext}/user-stories.md`

```markdown
## [DRAFT] User Stories — {BoundedContext}

### US-{BC}-{NNN}: {Story Title}

**As a** {role}
**I want** {feature/capability}
**So that** {business value}

**Acceptance Criteria:**
- [ ] AC-{BC}-{NNN}.1: {Criterion 1}
- [ ] AC-{BC}-{NNN}.2: {Criterion 2}
- [ ] AC-{BC}-{NNN}.3: {Criterion 3 (minimum)}

**Gherkin:**
```gherkin
Feature: {Story Title}

  @AC-{BC}-{NNN}.1
  Scenario: {Criterion 1 scenario}
    Given {context}
    When {action}
    Then {outcome}

  @AC-{BC}-{NNN}.2
  Scenario: {Criterion 2 scenario}
    Given {context}
    When {action}
    Then {outcome}
```

**INVEST:** Independent ✅ | Negotiable ✅ | Valuable ✅ | Estimable ✅ | Small ✅ | Testable ✅
```

Mark as `[DRAFT]` until finalization. Remove markers only during the finalization pass.

## Identifiers

Story, criterion and scenario identifiers follow `.ai/reference/traceability.md` — the source of
truth for formats, allocation and legacy handling.

- `{BC}` is the bounded context's name as a single PascalCase token, copied verbatim from
  `UBIQUITOUS_LANGUAGE.md` (`Budget`, `HouseholdPlanning`). `{NNN}` is exactly three digits,
  zero-padded from `001`. `{n}` is an unpadded criterion number within its story.
- Allocate when the entry is first written: the next `{NNN}` **above the high-water mark** of the
  context. Allocation is append-only — deleting a criterion never frees its number, and
  reordering never moves one. Retired identifiers are tombstoned in a `## Retired IDs` section of
  the owning file.
- Every criterion is `- [ ] AC-{BC}-{NNN}.{n}: {text}`. Write the identifier as literal text —
  never bold or code-formatted.
- Every scenario carries exactly one `@AC-{BC}-{NNN}.{n}` tag directly above `Scenario:`. One
  scenario satisfies exactly one criterion; a criterion may have several scenarios. Other tags
  (`@smoke`, `@e2e`) may share the line; `Feature:` and `Background:` carry none.
- A story document with no identifiers is in **legacy mode** — a supported state, not a defect.
  It gains identifiers on the next finalization pass, and anything referencing it (blueprints,
  tests) is labelled afterwards, never before.

## Mode 0 — Extraction from Documents

Use when existing documents are provided.

1. Read all input documents
2. Identify every user-facing feature, capability, or workflow
3. Draft a user story entry for each feature — use `[OPEN]` for missing acceptance criteria
4. Identify the bounded context each story belongs to
5. Write draft files to `docs/bounded-contexts/{BC}/user-stories.md`
6. Announce: "Extracted N user stories — M gaps marked [OPEN]."

## Mode 1 — Progressive Capture (During Sessions)

Use as soon as acceptance criteria for a feature are agreed during design interrogation.

When criteria are agreed:
1. Create `docs/bounded-contexts/{BC}/user-stories.md` if it does not exist
2. Write the draft story entry (criteria captured, Gherkin `[OPEN]`)
3. Announce: "📝 Draft user story logged to `docs/bounded-contexts/{BC}/user-stories.md`."

## Mode 2 — Finalization

Run when interrogation is complete.

1. Read all draft user story files
2. Complete all `[OPEN]` acceptance criteria — resolve or defer explicitly
3. Allocate identifiers to every story, criterion and scenario that lacks one. For an unlabelled
   legacy document this is the migration: the next `{NNN}` above the context's high-water mark,
   criteria numbered `.1`, `.2`, … top to bottom in current document order. Numbers are frozen
   once written (`.ai/reference/traceability.md`)
4. Add Gherkin scenarios traced to every acceptance criterion — at least one scenario per
   criterion, each tagged with exactly one `@AC-{BC}-{NNN}.{n}` on the line directly above
   `Scenario:`. A scenario that covers two criteria is split into two, never tagged twice
5. Validate each story against INVEST:
   - **Independent** — can be delivered without depending on another story
   - **Negotiable** — details can still be discussed
   - **Valuable** — delivers clear value to the actor
   - **Estimable** — the team can estimate effort
   - **Small** — completable within one sprint
   - **Testable** — acceptance criteria are unambiguous and verifiable
6. Ensure minimum 3 acceptance criteria per story
7. Ensure all terms match the finalized `UBIQUITOUS_LANGUAGE.md`
8. Remove all `[DRAFT]` markers
9. Write the finalized files

Announce:
> ✅ User stories finalized — N stories, M Gherkin scenarios, all INVEST-validated.
