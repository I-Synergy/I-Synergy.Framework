# Traceability Identifiers

> Each artifact in the design → story → slice → test chain carries a scoped identifier, so the chain
> can be walked in both directions: a test states which acceptance criterion it is evidence for, and
> a criterion states which scenarios, slices and tests exist for it. This document defines that
> convention; it is the source of truth read by the `user-story`, `usecase-specification`,
> `vertical-slices` and `solution-generator` skills, the templates in `.ai/reference/templates/`,
> and the traceability validator `.ai/tests/validate-traceability.py`.

## Purpose and Scope

Today the chain is carried entirely by human-readable names — a Gherkin scenario by its free-text
title, an acceptance criterion by its position in a checkbox list, a test by its method name. None
of those can be checked mechanically, so a missing link stays invisible until a reviewer notices it.

This convention makes one question mechanically answerable, in both directions: **does every
declared acceptance criterion have at least one scenario, one slice and one test, and does every
scenario tag, blueprint reference and test attribute resolve to a declared criterion?** It applies
to the artifacts that already exist — user story documents, use case documents, slice blueprints,
feature files and MSTest classes — and introduces no new artifact type, directory or tool.

The acceptance criterion is the join key. `AC-…` is the only identifier expected in all three
surfaces (story document, feature file, test code), which is why the rules below are written around
it and why its cardinality rule — one scenario satisfies exactly one criterion — is the one that
must not be relaxed.

## The Format

| Entity | Format | Example | Vehicle |
|---|---|---|---|
| User story | `US-{BC}-{NNN}` | `US-Budget-014` | heading |
| Acceptance criterion | `AC-{BC}-{NNN}.{n}` | `AC-Budget-014.2` | checkbox line |
| Gherkin scenario | `@AC-{BC}-{NNN}.{n}` | `@AC-Budget-014.2` | tag — one per scenario |
| Use case | `UC-{BC}-{NNN}` | `UC-Budget-007` | heading |
| Alternate flow | `AF-{BC}-{NNN}.{n}` | `AF-Budget-007.1` | list item and Gherkin tag |
| Slice | existing `{Entity}.{Operation}` | `Budget.Create` | blueprint `slice` key |
| Test | `[TestCategory("AC-…")]` | `AC-Budget-014.2` | MSTest attribute |

- **`{BC}` is the bounded context's name, as one PascalCase token.** It is the context's name as
  written in `UBIQUITOUS_LANGUAGE.md`, and it is the same string used as the directory segment in
  `docs/bounded-contexts/{BC}/` and `docs/slices/{BC}/`, and as the project segment in
  `{Solution}.{BC}.Tests`. A multi-word name joins in PascalCase (`HouseholdPlanning`) — never a
  space, hyphen or underscore. The token is copied verbatim: not abbreviated (`Budget`, never
  `BUD`), not re-cased (`Budget`, never `budget`), not pluralised or singularised. If the ubiquitous
  language says `Budgets`, every identifier says `Budgets`.
- **`{NNN}` is exactly three digits, zero-padded**, starting at `001`. `1` and `0001` are both
  malformed. The range is `001`–`999`; a bounded context approaching 999 stories is a signal it
  should be split, not that the field should widen.
- **`{n}` is an unpadded sequence number** allocated inside its parent. The parent is named by the
  `{NNN}` that precedes it: `AC-Budget-014.2` is the second acceptance criterion of story
  `US-Budget-014`, and `AF-Budget-007.1` the first alternate flow of use case `UC-Budget-007`.
  `{n}` never appears without its parent.
- **Prefixes are upper case** — `US-`, `AC-`, `UC-`, `AF-` — and identifiers are matched literally
  and case-sensitively. `ac-budget-014.2` is not a variant of `AC-Budget-014.2`; it is a typo.
- **The only separators are `-` between segments and `.` before the child number.** No `_`, no
  spaces, no slashes.

## Scoping

Identifiers are unique **within one bounded context**, not across the repository.

- `US-` and `UC-` are independent numbering spaces in the same context: a context may contain both
  `US-Budget-014` and `UC-Budget-014`, and that is not a collision. `AC-` and `AF-` numbers are
  derived from their parent's number, so they inherit the parent's uniqueness; `.n` completes it.
- The `{BC}` token in an identifier must match the context whose document declares it. A story
  declared in `docs/bounded-contexts/Budget/user-stories.md` uses `US-Budget-…`; an identifier with
  a different context token in that file names a context the file does not belong to.
- A reference may point at an identifier owned by another context — a cross-context end-to-end
  scenario, for example. It resolves against the owning context's documents, named by the
  identifier's own `{BC}` token; where the referencing artifact lives is not constrained.
- The slice is the exception that proves the rule: `Budget.Create` is scoped by its path, so its
  full identity is `docs/slices/{BC}/{Entity}.{Operation}/` plus the blueprint's `slice` key. The
  blueprint's `bounded_context` value is the same `{BC}` token.

**Why scoped rather than bare.** The generated artifacts are already scoped by path — story files
live at `docs/bounded-contexts/{BC}/…`, feature files at `docs/slices/{BC}/…`, tests in
`{Solution}.{BC}.Tests/` — so a bare `AC-014.2` would normally resolve inside any one of them, and
the shorter form reads better. Scoping is not a fix for a collision that exists today; it is
insurance against the aggregation this convention introduces. A per-release test completion report
spans bounded contexts, and so does the validator's own coverage output; at that point a bare
`AC-014.2` is either ambiguous or has to be re-scoped across every artifact at exactly the moment
they are being aggregated.

## Assigning IDs

Identifiers are allocated by whoever writes the artifact — there is no central registry; the file is
the registry.

**Allocation**

- A new story or use case takes the next number **above the high-water mark** of its context and
  prefix, starting at `001`. Allocation is never "the first free number": a retired number stays
  retired and is not filled.
- A new acceptance criterion takes the next `.n` above the high-water mark of its story, starting at
  `1`; a new alternate flow does the same within its use case.
- **Migrating an unlabelled document** (see *Legacy Mode*) is the one case where `.n` follows
  position: criteria are labelled top to bottom in their current document order, `1`…`n` per story.
  Once written, those numbers are frozen and the rules below apply.

**Editing and reordering**

- A wording change never changes an identifier. The test for identity: *would the existing scenario
  or test still be the same check?* If yes, keep the identifier. If no, it is a different criterion:
  allocate a new one and retire the old.
- Reordering criteria or alternate flows does not move identifiers. The list may then read `.1`,
  `.3`, `.2`; that is expected and is not a defect. Renumbering to match display order is forbidden —
  it silently re-points every existing tag and test at a different criterion.
- An identifier cannot change owner. Moving a criterion to another story, or a story to another
  bounded context, retires the old identifier and allocates a new one at the destination; the
  references move in the same change. Splitting a story works the same way: the part that keeps the
  story's identity keeps the number, the other part gets a new number, and criteria that move are
  retired and re-allocated.

**Deletion**

Identifiers are append-only. Deleting a criterion, alternate flow, story or use case removes the
block; its numbers are never reused and never renumbered. Retired identifiers are tombstoned in a
`## Retired IDs` section at the end of the owning document, one line each:

```markdown
## Retired IDs

- US-Budget-009: retired 2026-09-18 — the bulk-import story moved to the Import context.
- AC-Budget-014.3: retired 2026-09-25 — merged into AC-Budget-014.2.
```

The line format is `- {ID}: retired {yyyy-mm-dd} — {reason}`. Omit the section while nothing has been
retired.

A retired identifier is no longer a declared criterion, and that is how "never reuse" is enforced: a
lingering `@AC-…` tag, blueprint reference or `[TestCategory]` that points at a retired criterion
**fails** resolution rather than silently passing, and the fix is to delete or re-point the
reference, not to resurrect the number. Gaps and out-of-order sequences are the visible, expected
consequence of deletion; the validator never reports them.

**Branch merges**

Two branches can allocate the same next number. On merge, the batch merged second re-allocates its
identifiers above the merged high-water mark and updates its own references in the same commit; the
first-merged numbers are not touched. Identifiers that have already left the repository (tagged
releases, published reports) are retired and re-allocated rather than renumbered in place. This is
the cost of identifiers living in files instead of a shared tracker, and it is accepted deliberately.

## One Scenario, One Criterion

A Gherkin scenario satisfies **exactly one** acceptance criterion. A criterion, in the other
direction, may be satisfied by more than one scenario — the rule constrains the tag, not the count of
scenarios per criterion.

**Correct** — one tag, on the line directly above the scenario:

```gherkin
  @AC-Budget-014.2
  Scenario: Reject a non-positive amount
    Given I am an authenticated user
    When I record a budget of 0.00
    Then the creation fails with a validation error
```

- A `Scenario` or `Scenario Outline` carries at most one traceability tag, and a scenario that
  realizes an acceptance criterion carries exactly one. A `Scenario Outline` is a single scenario;
  its `Examples:` rows are data variants of the same criterion and share its one tag.
- Other tags (`@smoke`, `@e2e`) are allowed on the same scenario; the rule is one `@AC-` (or `@AF-`)
  tag, not one tag in total.
- `Background:` and `Feature:` carry no traceability tag.
- A scenario that only arranges state for another criterion is not covering it — steps are not
  criteria.

**When a scenario genuinely covers two criteria**, one of two things is true, and neither is "tag it
twice":

1. **The scenario can be split** — write two scenarios, each tagged once. This is the preferred
   answer; the shared Given/When steps may be duplicated or moved to a `Background:`.
2. **It cannot be split** — the two criteria are not independently testable, which means they are
   one criterion. Merge them: keep the lower `.n`, retire the other, and re-point its references.

Two tags on one scenario are never written, and no combined identifier
(`AC-Budget-014.1-014.2`) exists. **Why the rule is absolute:** the tag is the only edge the validator
follows in both directions. With one tag per scenario, a red test maps to exactly one criterion, and
a criterion enumerates exactly the scenarios that are evidence for it. Relax it and neither direction
can be computed without a disjunction in every check.

## Where Each ID Appears

| Artifact | Where the identifier goes |
|---|---|
| Story heading | `docs/bounded-contexts/{BC}/user-stories.md`: `### US-Budget-014: Record a budget` |
| Criterion | same file, as a checkbox line: `- [ ] AC-Budget-014.2: A non-positive amount is rejected.` |
| Scenario | the Gherkin block in the story file, and `docs/slices/{BC}/{Entity}.{Operation}/{Entity}.{Operation}.feature`: a tag line directly above `Scenario:` |
| Use case heading | `docs/bounded-contexts/{BC}/use-cases.md`: `### UC-Budget-007: Record a budget` |
| Alternate flow | same file: `- AF-Budget-007.1: {Condition} → {Steps}`, and a `@AF-Budget-007.1` tag on the scenario that realizes it |
| Slice | `docs/slices/{BC}/{Entity}.{Operation}/blueprint.json`: the existing `slice` key, plus identifiers in `source` |
| Test | `{Solution}.{BC}.Tests/`: the `// Traces:` header comment and `[TestCategory("AC-…")]` on the method |

**Story and use-case documents.** The working shape, which the skills emit exactly:

````markdown
### US-Budget-014: Record a budget

**Acceptance Criteria:**
- [ ] AC-Budget-014.1: A budget is created and stored with a unique identifier.
- [ ] AC-Budget-014.2: A non-positive amount is rejected with a validation error.
````

The criterion text follows the identifier after a colon, mirroring the story heading and the existing
alternate-flow list. Do not bold or code-format the identifier — the validator reads it as literal
text.

**Blueprint `source`.** The slice's trace is identifier arrays:

```json
  "source": {
    "use_cases": ["UC-Budget-007"],
    "user_stories": ["US-Budget-014"],
    "acceptance_criteria": ["AC-Budget-014.1", "AC-Budget-014.2"]
  }
```

- Every value is an array, even when it holds one element — a slice may serve several stories, and a
  uniform shape keeps the validator simple.
- Omit a key that does not apply. At least one of `use_cases` or `user_stories` must be non-empty; a
  slice that names neither is untraceable by construction, and a `source` object that names neither
  fails T3. A blueprint carrying no `source` key at all is the pre-convention shape and is read as a
  legacy slice — silent, not failed.
- The free-text `"acceptance_criteria": ["Given … When … Then …"]` array that used to sit at the top
  level of the blueprint is replaced by `source.acceptance_criteria`. The criterion's Gherkin text
  lives once, tagged, in the `.feature` file; it is not duplicated into JSON.
- Blueprints written before this convention may still carry the singular `use_case` / `user_story`
  string keys. Read those as one-element arrays — the same tolerance as legacy story documents.
- A `user_stories` or `use_cases` value that names no identifier is read as pre-identifier legacy
  content and accepted without a finding, the same tolerance as the pre-convention top-level
  free-text array above.

**Feature files and the runner.** The tag lives with the scenario in every copy of the file. Step
definitions are shared across scenarios and carry no identifiers — they cannot, because Reqnroll
binds them by step text, not by scenario. Where a project keeps both a design-time copy under
`docs/slices/{BC}/…` and an executable copy under `{Solution}.{BC}.Tests/Features/`, the tags travel
with the scenarios; the executable copy is never stripped.

**Test classes.** Two identifiers, two vehicles:

```csharp
// File: tests/{ApplicationName}.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs
// Traces: US-Budget-014 / AC-Budget-014.1, AC-Budget-014.2

[TestClass]
public class CreateBudgetCommandHandlerTests
{
    [TestMethod]
    [TestCategory("AC-Budget-014.1")]
    public async Task HandleAsync_ValidCommand_CreatesBudgetSuccessfully()
    {
        // ...
    }
}
```

- The `// Traces:` line lists the file's story identifiers, a `/`, then its criterion identifiers,
  comma-separated. It goes directly below the `// File:` header.
- Each test method that is evidence for an acceptance criterion carries exactly one
  `[TestCategory("AC-…")]` naming that criterion. A method that is evidence for no criterion — a
  guard-clause check, an infrastructure or logging assertion — carries no `AC-` category and sits
  outside the trace. Additional `[TestCategory]` values (`Unit`, `Integration`) are allowed and
  ignored by this convention. This is not licence to omit the category where it belongs: an untagged
  test for a real criterion leaves that criterion with no test-side reference, and T6 warns on it.
- The order of `[TestMethod]` and `[TestCategory]` is not significant; write `[TestMethod]` first.

**The join key.** `AC-` is the only identifier expected in all three surfaces — story document,
feature file, test code. `US-` appears in the story document, the blueprint `source` and the test
header; `UC-`/`AF-` link the use-case document to its scenarios and to the slice's `source`. `AC-` is
the validator's primary key.

## Enforcement

The validator walks `docs/bounded-contexts/**`, `docs/slices/**` and the test sources, and implements
these checks and severities:

| # | Check | Severity |
|---|---|---|
| T1 | Duplicate `US-`/`UC-`/`AC-` identifier within a bounded context | FAIL |
| T2 | Every `@AC-…` tag resolves to a declared criterion | FAIL |
| T3 | Every identifier in a blueprint's `source` resolves — `use_cases`, `user_stories` and `acceptance_criteria` — and the `source` names at least one use case or story | FAIL |
| T4 | Every `[TestCategory("AC-…")]` resolves to a declared criterion | FAIL |
| T5 | A declared criterion has at least one scenario | WARN |
| T6 | A declared criterion has at least one slice and at least one test-side reference | WARN |
| T7 | A story or use-case document with zero identifiers reports one summary warning (see *Legacy Mode*) | WARN |

FAIL means a reference is broken — the identifier it names does not exist, or two artifacts claim the
same one. WARN is the advisory pattern already used by `validate-claude-md.py`: the trace is
incomplete, the repository is not. The FAIL/WARN split is deliberate; *Legacy Mode* explains why the
incompleteness of an un-migrated project must never fail a build.

Two implementation notes for the validator, so it does not report false positives:

- T2 reads `.feature` files. The identical tags inside story and use-case documents are the
  design-time source and are not separately checked.
- For T6, count a criterion as tested when either an MSTest method carries its `[TestCategory("AC-…")]`
  or an executable Reqnroll scenario carries its `@AC-…` tag.

**T7's scope is the story and use-case documents, not every markdown file in a context.** A file
whose name is story- or use-case-shaped (`user-stories.md`, `user-stories-2026.md`, `use-cases.md`)
is in scope; a glossary, a context history, a storage note or a README sitting beside them is not.
Those files still *declare* identifiers and are walked for declarations, references and scenario
tags like any other — only the legacy and mixed mode warnings are withheld from them, because an
absence of story identifiers in a document that holds no stories is not an incomplete trace.

**A padded child number resolves, and is reported.** `AC-Budget-014.02` is read as a criterion
rather than dropped: tightening the match to `[1-9]\d*` would remove the *declaration*, which turns
a formatting slip into a coverage hole and makes every reference to it dangle. Instead the
tolerance stays and the shape is reported as an advisory warning, once at the declaration and once
at each reference carrying it — `{n}` is unpadded, so the fix is to rewrite it as
`AC-Budget-014.2`. Like every other advisory here, it does not change the exit code.

**Not enforced by these checks**, though the convention requires them — author discipline for now:
the `{BC}` token matching the containing directory, the one-traceability-tag-per-scenario cardinality,
the `// Traces:` header format, and the single `[TestCategory("AC-…")]` per method.

## Legacy Mode

**Legacy mode is a supported state, not a defect.** Every story or use-case document written before
this convention existed has no identifiers at all, and every already-upgraded downstream project
carries those documents. A validator that failed on them would be disabled within a day, and a
disabled validator protects nothing.

**Detection.** A story or use-case document — a bounded-context file whose name is story- or
use-case-shaped (see *Enforcement* for the scope) — containing zero `US-` / `UC-` identifiers is in
legacy mode. Detection is by content, not configuration: there is no flag, marker or allow-list to
maintain, and a document leaves legacy mode by gaining identifiers, not by being registered
somewhere.

**Effect.** The validator emits **one summary warning for the whole document**, never per-item
failures —

```text
WARN docs/bounded-contexts/Budget/user-stories.md — legacy mode: 0 of 6 stories carry identifiers;
     criterion-level checks (T5, T6) skipped for this file
```

— and skips T1/T5/T6 for that file. Nothing in a legacy document can fail.

**Exit.** Labelling is done by re-running the finalization pass of the `user-story` or
`usecase-specification` skill, which assigns identifiers top to bottom in the document's current
order (see *Assigning IDs*). The migration order matters: label the documents **first**, then the
blueprints and tests that reference them — a reference to a criterion that has not been labelled yet
is a dangling reference and fails T2/T3/T4. Documents, then slices, then tests.

**Partially labelled documents.** A document with some identifiers is not in legacy mode; its
labelled stories are checked normally. Unlabelled stories in it produce one advisory warning, never
per-item failures — the same principle as legacy mode, for the same reason: a half-finished migration
must not block a build.

## Worked Example

One bounded context (`Budget`), one story, taken through every vehicle. This is the text an agent is
expected to emit; copy the shapes, not the words.

**1 — Story, criteria and scenarios.** `docs/bounded-contexts/Budget/user-stories.md`

````markdown
## User Stories — Budget

### US-Budget-014: Record a budget

**As a** household planner
**I want** to record a monthly budget
**So that** I can track spending against it

**Acceptance Criteria:**
- [ ] AC-Budget-014.1: A budget is created and stored with a unique identifier.
- [ ] AC-Budget-014.2: A non-positive amount is rejected with a validation error.

**Gherkin:**
```gherkin
Feature: Record a budget

  @AC-Budget-014.1
  Scenario: Create a valid budget
    Given I am an authenticated user
    When I record a budget of 100.00
    Then the budget is stored
    And the budget has a unique identifier

  @AC-Budget-014.2
  Scenario: Reject a non-positive amount
    Given I am an authenticated user
    When I record a budget of 0.00
    Then the creation fails with a validation error
```
````

**2 — Slice blueprint.** `docs/slices/Budget/Budget.Create/blueprint.json`

```json
{
  "slice": "Budget.Create",
  "bounded_context": "Budget",
  "operation_type": "Command",
  "entity": "Budget",
  "handler_class": "CreateBudgetCommandHandler",
  "inputs": [
    { "name": "Amount", "type": "decimal", "required": true }
  ],
  "outputs": [
    { "name": "BudgetId", "type": "Guid" }
  ],
  "files_to_create": [
    "{Solution}.Domain.Budget/Features/Budget/Commands/CreateBudget/CreateBudgetCommand.cs",
    "{Solution}.Domain.Budget/Features/Budget/Commands/CreateBudget/CreateBudgetCommandHandler.cs",
    "{Solution}.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs"
  ],
  "source": {
    "user_stories": ["US-Budget-014"],
    "acceptance_criteria": ["AC-Budget-014.1", "AC-Budget-014.2"]
  }
}
```

**3 — Feature file.** `docs/slices/Budget/Budget.Create/Budget.Create.feature`

```gherkin
# File: docs/slices/Budget/Budget.Create/Budget.Create.feature

Feature: Record a budget

  @AC-Budget-014.1
  Scenario: Create a valid budget
    Given I am an authenticated user
    When I record a budget of 100.00
    Then the budget is stored
    And the budget has a unique identifier

  @AC-Budget-014.2
  Scenario: Reject a non-positive amount
    Given I am an authenticated user
    When I record a budget of 0.00
    Then the creation fails with a validation error
```

**4 — Test class.** `tests/{ApplicationName}.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs`

```csharp
// File: tests/{ApplicationName}.Budget.Tests/Budget/CreateBudgetCommandHandlerTests.cs
// Traces: US-Budget-014 / AC-Budget-014.1, AC-Budget-014.2

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {ApplicationName}.Budget.Tests.Budget;

[TestClass]
public class CreateBudgetCommandHandlerTests
{
    [TestMethod]
    [TestCategory("AC-Budget-014.1")]
    public async Task HandleAsync_ValidCommand_CreatesBudgetSuccessfully()
    {
        // Arrange / Act / Assert — the budget is stored and gets a unique identifier
    }

    [TestMethod]
    [TestCategory("AC-Budget-014.2")]
    [ExpectedException(typeof(ArgumentException))]
    public async Task HandleAsync_NonPositiveAmount_ThrowsArgumentException()
    {
        // Arrange / Act / Assert — a non-positive amount is rejected
    }
}
```

**The chain, assembled**

```text
docs/bounded-contexts/Budget/user-stories.md
  US-Budget-014
    ├─ AC-Budget-014.1 ─ @AC-Budget-014.1 ─ [TestCategory("AC-Budget-014.1")]
    └─ AC-Budget-014.2 ─ @AC-Budget-014.2 ─ [TestCategory("AC-Budget-014.2")]

docs/slices/Budget/Budget.Create/blueprint.json
  source.user_stories        → US-Budget-014
  source.acceptance_criteria → AC-Budget-014.1, AC-Budget-014.2
```

## What This Does Not Do

- **It does not measure coverage or quality.** A test carrying `[TestCategory("AC-Budget-014.2")]`
  proves that someone asserted the link — not that the test verifies the criterion, not that the
  criterion is meaningful, and not that the scenario's steps are correct. The validator checks that
  references resolve; it cannot check meaning.
- **It is not a certification artifact and asserts no conformity with any standard.** It is a
  repository convention that makes existing artifacts checkable. Where a standard's test
  documentation is produced as a by-product — the ISO/IEC/IEEE 29119-3 test case specification is the
  acceptance criteria ↔ scenario ↔ test mapping this convention already carries — that is evidence an
  auditor can sample, not a claim of compliance.
- **It does not trace non-functional requirements.** Performance, security and reliability targets
  are quality attributes rather than acceptance criteria, and they are not numbered here.
- **It does not check use-case main flows.** Only alternate flows carry a tag (`@AF-…`). There is no
  `@UC-…` tag in this scheme, so a main-flow scenario's existence cannot be checked mechanically;
  main flows are traced through the slice that implements them. This is a known gap.
- **It does not replace a requirements tracker.** Identifiers live in the repository's files and are
  allocated by whoever writes them; nothing here syncs with or overrides Jira, Azure Boards or
  similar. That also means two branches can allocate the same number — see *Assigning IDs* → branch
  merges for the merge rule.

## Quick Checklist

- [ ] Story and use-case headings carry `US-{BC}-{NNN}` / `UC-{BC}-{NNN}`, with `{BC}` copied from the ubiquitous language
- [ ] Every criterion is `- [ ] AC-{BC}-{NNN}.{n}: {text}`; every alternate flow `- AF-{BC}-{NNN}.{n}: {…}`
- [ ] Every scenario carries exactly one `@AC-…` (or `@AF-…`) tag, directly above `Scenario:`
- [ ] Every blueprint `source` holds identifier arrays and no free-text criteria
- [ ] Every test method that is evidence for a criterion has one `[TestCategory("AC-…")]`; the file header lists `US-… / AC-…`
- [ ] No identifier was renumbered or reused; retired identifiers are tombstoned in `## Retired IDs`
- [ ] A legacy document was left in legacy mode unless it was being finalized
