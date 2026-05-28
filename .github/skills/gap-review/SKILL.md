---
name: gap-review
description: Validates a generated or implemented solution against the original architecture document and design decisions. Run after solution-generator or after completing feature implementation to verify alignment. Trigger whenever the user says "gap review", "validate the implementation", "check against the design", or after solution generation completes. Identifies gaps, deviations, and missing decisions.
---

# Gap Review

Mandatory post-generation validation — checks the implemented solution against the original architecture document and design session decisions.

## When to Run

- After `solution-generator` completes
- After implementing a feature or bounded context
- Before a milestone or release checkpoint
- When something "feels off" about the implementation

## Inputs

| Input | Source |
|-------|--------|
| Architecture document | `docs/architecture/{SolutionName}-architecture.md` |
| Session decisions | `docs/session-decisions.md` |
| Ubiquitous Language | `UBIQUITOUS_LANGUAGE.md` |
| Use Cases | `docs/bounded-contexts/{BC}/use-cases.md` |
| Generated solution | Current codebase |

## Review Dimensions

### 1. Structural Alignment

Compare the generated project structure against the architecture document.

| Check | Pass Condition |
|-------|---------------|
| All bounded contexts have corresponding projects | Each BC has `Entities`, `Models`, `Domain`, `Services` projects |
| Vertical slice organization | Every entity has `Features/{Entity}/Commands/` and `Queries/` subfolders |
| Service registration | Each domain has `Extensions/ServiceCollectionExtensions.cs` |

### 2. Domain Model Alignment

Compare generated entities against the architecture document's domain model.

| Check | Pass Condition |
|-------|---------------|
| All aggregates have entity classes | Every aggregate root has a corresponding EF Core entity |
| Naming matches ubiquitous language | All class/property names use canonical terms from `UBIQUITOUS_LANGUAGE.md` |
| Entity properties match agreed attributes | Properties reflect what was decided in the design session |

### 3. CQRS Coverage

Verify all use cases have corresponding CQRS handlers.

| Check | Pass Condition |
|-------|---------------|
| Use case → Command/Query | Every use case in `use-cases.md` has a corresponding handler |
| Command parameters | Commands use individual parameters, not model objects |
| Response wrapping | All responses wrap model types — no domain entities returned directly |
| Async pattern | All handlers include `CancellationToken` |

### 4. Technology Compliance

Verify the implementation matches the technology choices from the architecture document.

| Check | Pass Condition |
|-------|---------------|
| Data access | EF Core primitives only (`Add`, `FirstOrDefaultAsync`, `Remove`, `SaveChangesAsync`) |
| No repositories | No `IRepository` or similar abstraction layers |
| Forbidden libraries | None of the forbidden technologies from `.ai/reference/forbidden-tech.md` |

### 5. Decision Traceability

Verify the implementation reflects the decisions in `docs/session-decisions.md`.

For each decision in the log:
1. Find the corresponding implementation artifact
2. Confirm the decision was implemented as agreed
3. Flag any deviation with DEVIATION status and the reason

## Output Format

Write the report to `docs/gap-review-{YYYY-MM-DD}.md`:

```markdown
# Gap Review — {SolutionName} — {Date}

## Summary
- Total checks: N
- ✅ Passed: M
- ❌ Gaps: K
- ⚠️ Deviations: J

## Gaps

### GAP-{N}: {Description}
- **Location:** {file or project}
- **Expected:** {what the architecture document specifies}
- **Actual:** {what was generated or implemented}
- **Action:** fix | defer | accept

## Deviations

### DEV-{N}: {Description}
- **Decision:** {original decision from session-decisions.md}
- **Deviation:** {how the implementation differs}
- **Justification:** {why acceptable, or recommendation to fix}

## Sign-off
- [ ] All gaps addressed or explicitly deferred
- [ ] All deviations accepted or scheduled for fix
```

Announce:
> ✅ Gap review complete — N gaps, M deviations. Report written to `docs/gap-review-{date}.md`.
