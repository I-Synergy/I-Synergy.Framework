# Project Development Preferences — I-Synergy Framework

## Environment

### Builds — memory constraint (IMPORTANT)

**Never run `dotnet build` / `dotnet publish` with MSBuild node reuse enabled.** This machine runs out of physical memory whenever it is left on.

MSBuild's default node reuse keeps a pool of worker processes alive after a build completes (one per core) so the next build starts faster. A single solution publish left **18 MSBuild processes plus a `VBCSCompiler` alive — roughly 3.8 GB resident** — and successive builds multiply that.

- **All three knobs are already set** in `.claude/settings.json` → `env`, so builds run through Claude Code get them automatically — including subagents and hooks:

  | Setting | Kills | Typical footprint |
  |---|---|---|
  | `MSBUILDDISABLENODEREUSE=1` | MSBuild worker nodes | ~18 processes, ~3.5 GB |
  | `DOTNET_CLI_USE_MSBUILD_SERVER=0` | MSBuild server | — |
  | `UseSharedCompilation=false` | `VBCSCompiler` (Roslyn compiler server) | ~430 MB |

  `UseSharedCompilation` works via `env` because MSBuild surfaces environment variables as properties.

- **Verified:** after `dotnet build` on the full solution, process count for `MSBuild` + `VBCSCompiler` + `dotnet` is **0**. Expect the build itself to be slower (~40 s vs ~19 s for an incremental solution build) — that is the cost of not reusing anything.
- When invoking `dotnet` outside the harness (or if the `env` block is ever removed), pass the flags explicitly:
  ```bash
  MSBUILDDISABLENODEREUSE=1 dotnet build <proj> -nodeReuse:false -p:UseSharedCompilation=false
  ```
- Run `dotnet build-server shutdown` after heavy builds. **Caveat:** that shuts down the MSBuild *server* and the compiler server, but it does **not** kill node-reuse *worker* processes — those need `Stop-Process`, or the ~15-minute idle timeout. Killing them is safe once CPU time is flat across two samples, which proves they are idle rather than mid-build.
- **This applies to delegated subagents too.** Any subagent prompt that triggers a .NET build must state this requirement explicitly — several agents each running `dotnet build` is what multiplies the node pools.

## Project-Specific Coding Rules

These extend the numbered rules in `AGENTS.md` and `.ai/reference/critical-rules.md`. They live here because `AGENTS.md` is template-owned and is overwritten by `/upgrade-template`.

### 14. Enums: Explicit Values When Persisted or Serialized
Any enum persisted to a database or serialized (JSON, OpenAPI) MUST declare explicit `= 0, = 1, …` values, with a comment that values are persisted and must never be renumbered. Inserting a member in declaration order relabels every stored row.

### 15. Bound Breaking Package Ranges
Pin known-incompatible transitive packages with an upper bound in `Directory.Packages.props` so the mismatch fails at restore instead of compile. Without the bound, a newer transitive version is resolved and the build fails deep inside a consuming project.

### 16. No Secrets in the Repo or Runbooks
Never commit `.p8` private keys, keystore material, or service-account JSON. Reference pipeline secret variable names and put a placeholder in the doc; the actual key material is stored in your CI/CD system only.

## Template Overrides

Edits to template-owned files that `/upgrade-template` can overwrite. Upgrade non-interactively (changed files keep the project version); if a file is accepted interactively, re-apply the override and re-run `python .ai/tests/validate-standards.py`.

| File | Override | Why |
|---|---|---|
| `.ai/reference/standards.md` | Row 1 (ISO/IEC/IEEE 29119-3:2021) is `Aligned` with proving validator `—`, not `Evidence` | This framework repo produces no `docs/slices/…` or `docs/test-reports/…`, so the `Evidence` claim cannot be supported and `validate-standards.py` fails on it. After re-applying, regenerate `COMPLIANCE.md` and the README `standards-status` block with `verify-config`. |
| `AGENTS.md` | Agent roster includes `programmer` and `winui-dev` | The template roster lists 9 of the 11 agents in `.ai/agents/`. |
| `.ai/scripts/sync-skills.py`, `.ai/tests/validate-settings.py`, `.ai/tests/run-all-tests.sh`, `.ai/tests/test_suite.py` | No `.ai/chains` / `.pi/chains` handling; `validate-pi.py` removed | Pi chains were dropped. |
| `.ai/tests/validate-structure.sh`, `validate-references.sh`, `validate-tokens.sh` | Check `AGENTS.md` instead of `CLAUDE.md` | `CLAUDE.md` no longer exists. |

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
