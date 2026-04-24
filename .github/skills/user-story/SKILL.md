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

### US-{N}: {Story Title}

**As a** {role}
**I want** {feature/capability}
**So that** {business value}

**Acceptance Criteria:**
- [ ] {Criterion 1}
- [ ] {Criterion 2}
- [ ] {Criterion 3 (minimum)}

**Gherkin:**
```gherkin
Feature: {Story Title}

  Scenario: {Criterion 1 scenario}
    Given {context}
    When {action}
    Then {outcome}
```

**INVEST:** Independent ✅ | Negotiable ✅ | Valuable ✅ | Estimable ✅ | Small ✅ | Testable ✅
```

Mark as `[DRAFT]` until finalization. Remove markers only during the finalization pass.

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
3. Add Gherkin scenarios traced to every acceptance criterion
4. Validate each story against INVEST:
   - **Independent** — can be delivered without depending on another story
   - **Negotiable** — details can still be discussed
   - **Valuable** — delivers clear value to the actor
   - **Estimable** — the team can estimate effort
   - **Small** — completable within one sprint
   - **Testable** — acceptance criteria are unambiguous and verifiable
5. Ensure minimum 3 acceptance criteria per story
6. Ensure all terms match the finalized `UBIQUITOUS_LANGUAGE.md`
7. Remove all `[DRAFT]` markers
8. Write the finalized files

Announce:
> ✅ User stories finalized — N stories, M Gherkin scenarios, all INVEST-validated.
