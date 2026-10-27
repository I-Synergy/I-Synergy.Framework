# verify-config fixes: DEEPSEEK.md and REASONIX.md
Status: DONE
Started: 2026-09-26

## Steps
- [x] DEEPSEEK.md: mark progress files MANDATORY in Task Execution Protocol
- [x] DEEPSEEK.md: add HARD RULE wording to the body of `## Subagent Delegation` (heading text unchanged)
- [x] DEEPSEEK.md: add `programmer` and `winui-dev` to the agent roster
- [x] DEEPSEEK.md: inline critical rules 24-26 (keep file within ~220 lines)
- [x] REASONIX.md: mark progress files MANDATORY in Task Execution
- [x] REASONIX.md: add a Subagent Delegation section with HARD RULE wording and the full 11-agent roster
- [x] Re-run validate-deepseek-parity.py and report the remaining failures

## Notes
- The parity validator matches level-2 headings exactly and requires `## Subagent Delegation`, so DEEPSEEK.md must NOT rename it to `... — HARD RULE`.
- Working-tree CLAUDE.md is a stale DeepSeek-swap copy and is out of scope here; parity failures caused by it are expected to remain.
- No concrete model names in shared docs; the tier lives in each agent's frontmatter.
