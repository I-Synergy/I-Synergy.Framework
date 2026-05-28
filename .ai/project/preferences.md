# Project Development Preferences — I-Synergy Framework

## Working Relationship

### Communication Style
- **Formality level:** Direct and concise — technical first, explanations where architecture or design decisions need rationale
- **Sycophancy:** None — challenge incorrect assumptions and flag risks directly
- **Challenge my thinking:** Yes — always flag correctness issues, SOLID violations, or patterns that contradict critical-rules.md
- **Timeline estimates:** Include when asked, otherwise focus on what needs to be done

### Problem-Solving Approach
- **Shortcuts vs. Correct fixes:** Always correct fix — this is a reusable framework library published to NuGet; shortcuts become breaking changes for all consumers
- **Bug fixing:** Fix immediately with a clear root-cause analysis; do not patch symptoms
- **Technical debt:** Acceptable only with documentation and a plan — must be tracked in `.ai/plans/`
- **Assumption handling:** Always verify against the actual codebase — search_content before claiming a pattern exists or doesn't
- **"Good enough" threshold:** Production-ready only — this is a NuGet-published framework consumed by external projects
- **Decision-making:** Agent autonomy for technical details within established patterns; user decides cross-cutting trade-offs (API breaks, dependency additions, target framework changes)

## Development Workflow

### Autonomy Level
- **Agent access:** Full repository access
- **Execution model:** Execute without asking for well-scoped fixes within established patterns; submit_plan for multi-file refactors, dependency additions, API surface changes, or any NuGet-published interface changes
- **Progress tracking:** Real-time automatic updates via `.ai/progress/` files for any task with 3+ steps

### Documentation Preferences
- **XML docs:** All public APIs — this is a NuGet library consumed by external developers; every public type, method, and parameter must have `<summary>` and parameter docs
- **Code comments:** For complex logic only — document the *why*, not the *what*
- **Architecture docs:** Text with code examples preferred; Mermaid for complex flow diagrams only
- **README updates:** With every structural change — README has auto-reconciliation hooks in `.claude/settings.json`

## Code Style

### Language Features
- **Immutability:** Records preferred for DTOs, commands, queries; classes with `init` for entities
- **Null handling:** Nullable reference types enabled (globally in `Directory.Build.props`)
- **Expression-bodied members:** Use extensively for single-expression members
- **Pattern matching:** Use modern C# patterns (switch expressions, property patterns, list patterns)

### Organization
- **File organization:** One public type per file; internal/private helpers may be combined if closely related
- **Namespace structure:** Match folder structure — `ISynergy.Framework.{Module}` namespace per project
- **Using statements:** Implicit global usings enabled via `Directory.Build.props` (`ImplicitUsings=enable`)

## Review & Quality Gates

### Pre-Submission Requirements
- **Code review:** Automated checklist (`pre-submission.md`) followed by subagent review (`reviewer` agent)
- **Build status:** 0 errors, 0 warnings — warnings suppressed only via `NoWarn` in `Directory.Build.props` after explicit decision
- **Test results:** All tests pass across all test projects
- **Documentation:** Complete — XML docs on all public APIs, README in sync

### Quality Metrics
- **Code coverage:** No hard threshold, but all handlers must have unit tests and edge-case tests
- **Cyclomatic complexity:** Keep methods readable; prefer extraction over nested conditionals
- **Method length:** < 50 lines for handlers and endpoints; utility methods flexible

## Session Context Integration

**When starting a new session:**
1. Read this preferences file
2. Read `.ai/session-context.md` and `.ai/completed/` for context
3. Apply these preferences to all work
4. Document preference-based decisions in session-context.md
5. Never re-ask questions answered here

---

**Note:** This is a reusable framework library (`ISynergy.Framework.*`), not an application with business domains. The CQRS/entity patterns documented in critical-rules.md and templates are for consuming applications built on top of this framework. For technology choices, see [tech-stack.md](tech-stack.md). For architecture, see [architecture.md](architecture.md).
