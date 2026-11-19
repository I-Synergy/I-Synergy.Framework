## GitHub Copilot Runtime

This section tells GitHub Copilot how to carry out the protocol above. Copilot has no SessionStart hook and no Claude `Agent` tool, so these steps are explicit.

**In VS Code, run the session in the `orchestrator` agent** (select it in the agent dropdown). It has the delegation tool and may run the specialist agents. If another agent or mode is active and no delegation tool is available, follow the fallback in section 2.

### 1. Session start (every session, before any other work)

1. Read `.ai/session-context.md`. Skip it if its first line starts with `# Session Context Template`, which is the unfilled placeholder.
2. List `.ai/progress/` and resume any in-flight task.
3. Skim the most recent files in `.ai/completed/`.
4. If `.github/skills/` or `.github/agents/` is missing, run `python .ai/scripts/sync-platforms.py` first (use `python3` on macOS and Linux). Both are gitignored links to `.ai/`.

### 2. Delegation

The Subagent Delegation table above names agents defined in `.ai/agents/` and exposed through `.github/agents/`. Delegate by agent name:

- **VS Code Local harness:** the `runSubagent` tool, with the agent name. Enable "Run Subagent" under Configure Tools.
- **Copilot harness, Copilot CLI and cloud agent:** the `task` tool, with the agent name.
- **No delegation tool in the current mode:** say so in one line, then read `.ai/agents/{name}.md` and every skill it lists, and follow them yourself. Never skip the rules because delegation is unavailable.

Every delegated prompt includes the progress-file instruction block from the Subagent Delegation section above.

### 3. Rules

- The Critical Coding Rules above always apply.
- Scoped files in `.ai/instructions/` attach by path match (`applyTo`). If one did not load for a file you are editing, read it yourself.
- Before finishing, verify the work against `.ai/checklists/pre-submission.md`.

### 4. Session end (only when project state changed)

1. Build check: if a `*.sln` or `*.slnx` file exists at the repository root and files under `src/` or `tests/` changed in this session, run `dotnet build --nologo --verbosity minimal -nodeReuse:false` with `MSBUILDDISABLENODEREUSE=1` set before the final reply. Fix any failure first. (Claude Code runs this as a Stop hook; Copilot follows this instruction instead.)
2. Mark completed steps in the progress file with Edit, one line at a time.
3. Update `README.md` for the change, as the README rule above requires.
4. Add `**Status:** DONE` to the progress file and move it to `.ai/completed/`.
5. Write the handoff to `.ai/session-context.md` using `.ai/reference/templates/session-handoff.md.txt`, with Written By set to GitHub Copilot. Update sections in place and never overwrite another client's entries.

### 5. Skills

Reusable workflows, including the former prompt files, are skills in `.ai/skills/`. Invoke them by name, for example `/ef-core` or `/dotnet-best-practices`. Load a skill's `SKILL.md` when a task matches its description.
