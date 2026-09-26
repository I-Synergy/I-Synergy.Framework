---
name: code-reviewer
description: Code quality and architecture review specialist. Use when reviewing code for SOLID principles, CQRS patterns, security issues, or architecture compliance before merging.
---

# Code Reviewer Skill

Specialized agent for code quality assurance and review.

## Expertise Areas

- Code quality assessment
- SOLID principles verification
- Security vulnerability detection
- Performance optimization
- Architecture compliance
- Best practices enforcement

## Responsibilities

1. **Architectural Review**
   - Verify Clean Architecture compliance
   - Check CQRS pattern implementation
   - Validate layer separation
   - Ensure proper dependency flow

2. **Code Quality**
   - Check SOLID principles adherence
   - Verify naming conventions
   - Assess code readability
   - Identify code smells

3. **Security Review**
   - Check for exposed secrets
   - Verify input validation
   - Review authorization checks
   - Ensure PII not logged

4. **Performance Review**
   - Identify N+1 query problems
   - Check async/await usage
   - Review allocation patterns
   - Verify caching strategy

## Critical Rules to Verify

### Commands & Queries
- [ ] Commands use individual parameters (NOT model objects)
- [ ] Queries use named parameters for optional filters
- [ ] Handlers inject DataContext directly (NO repositories)
- [ ] All async methods include CancellationToken

### Data Access
- [ ] Delete operations use `FirstOrDefaultAsync` + null check + `Remove` + `SaveChangesAsync`, and check `rowsAffected > 0`
- [ ] No N+1 query problems (proper Include usage)
- [ ] LINQ queries are efficient
- [ ] No blocking on async (.Wait() or .Result)

### Domain Entities
- [ ] Domain entities never exposed directly

### Validation & Error Handling
- [ ] Guard clauses on all public methods
- [ ] Data Annotations used for validation
- [ ] Proper exception types thrown
- [ ] Errors logged with context

### Logging
- [ ] Structured logging templates used
- [ ] No string interpolation in logs
- [ ] Appropriate log levels
- [ ] No PII or secrets logged

### Testing
- [ ] MSTest used (NOT xUnit or NUnit)
- [ ] Test names follow convention
- [ ] AAA pattern used
- [ ] Mocks properly verified

## Security Checklist

- [ ] No hard-coded secrets
- [ ] No secrets in logs
- [ ] Input validation present
- [ ] Authorization checks at endpoints
- [ ] No PII exposed
- [ ] Proper error messages (no implementation details)

## Performance Checklist

- [ ] No N+1 queries
- [ ] Batch operations used where appropriate
- [ ] Caching for reference data
- [ ] Async all the way
- [ ] No unnecessary allocations

## Quality Characteristics

Findings are attributed to the ISO/IEC 25010:2023 model in `.ai/reference/quality-model.md`, which
also defines the severity taxonomy (`Blocker`, `Major`, `Minor`). The checklists above cover two of the
nine characteristics; this table records which, so the gap stays visible instead of being implied away.

| # | Characteristic | Covered in this skill by |
|---|---|---|
| 1 | Functional suitability | Partly — `## Critical Rules to Verify` (validation, error handling) and the review process |
| 2 | Performance efficiency | `## Performance Checklist` |
| 3 | Compatibility | Partly — `## Forbidden Technologies` and the entity-exposure and API rules |
| 4 | **Interaction capability** | **Nothing** |
| 5 | **Reliability** | **Nothing** |
| 6 | Security | `## Security Checklist` |
| 7 | Maintainability | Partly — `## Critical Rules to Verify` (structure, data access, naming) |
| 8 | **Flexibility** | **Nothing** |
| 9 | **Safety** | **Nothing** |

**Four of the nine have no checklist in this skill: interaction capability, reliability, flexibility
and safety.** That is missing coverage, not an implied pass. When a change touches one of them — a UI
or API surface a caller has to interpret, a failure path, a hosting, provider or configuration
boundary, or a domain where a wrong answer causes harm — review the characteristic explicitly, using
its review question in `.ai/reference/quality-model.md`.

Attribution does not change which issues are reported. The high-confidence bar in
`.ai/agents/reviewer.md` is unchanged: a finding carries one characteristic and one severity, and a
characteristic the change does not implicate is not mentioned.

## Forbidden Technologies

Immediately flag if found:
- MediatR (use project's CQRS framework)
- xUnit/NUnit (use MSTest)
- FluentValidation (use Data Annotations)
- Swashbuckle (use Microsoft.AspNetCore.OpenApi)
- Standalone Polly (use Microsoft.Extensions.Resilience)

## Review Process

1. **First Pass - Architecture**
   - Verify file locations correct
   - Check namespace conventions
   - Validate project references

2. **Second Pass - Implementation**
   - Review handler logic
   - Check data access patterns

3. **Third Pass - Quality**
   - Check naming conventions
   - Verify documentation
   - Assess code clarity

4. **Final Pass - Testing**
   - Verify tests exist
   - Check test coverage
   - Review test quality

## Checklist Before Approval

- [ ] All critical rules verified
- [ ] Security checklist completed
- [ ] Performance checklist completed
- [ ] Findings attributed to one quality characteristic and one severity (`.ai/reference/quality-model.md`)
- [ ] No forbidden technologies found
- [ ] Code builds with 0 errors, 0 warnings
- [ ] All tests pass
- [ ] Documentation complete
