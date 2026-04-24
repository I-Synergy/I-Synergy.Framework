---
name: skill-creator
description: Create new skills, modify and improve existing skills, and measure skill performance. Use when users want to create a skill from scratch, edit or optimize an existing skill, write test cases, or improve a skill's description for better triggering accuracy. Trigger whenever the user says "create a skill", "make a skill for X", "turn this into a skill", "improve this skill", or wants to capture a workflow as a reusable skill.
---

# Skill Creator

A skill for creating new skills and iteratively improving them.

At a high level, the process of creating a skill goes like this:

- Decide what you want the skill to do and roughly how it should do it
- Write a draft of the skill
- Create a few test prompts and run claude-with-access-to-the-skill on them
- Help the user evaluate the results both qualitatively and quantitatively
- Rewrite the skill based on feedback from evaluation
- Repeat until satisfied
- Expand the test set and try again at larger scale

Your job when using this skill is to figure out where the user is in this process and then jump in and help them progress through these stages.

> **Note on eval infrastructure:** The full evaluation workflow (benchmark scripts, eval-viewer, grading agents) requires additional scripts in `skill-creator/scripts/`, `skill-creator/agents/`, and `skill-creator/eval-viewer/`. These are not bundled with this base template. Without them, follow the qualitative evaluation workflow described in the **Claude.ai-specific instructions** section — it works just as well for most skill development.

---

## Communicating with the user

Pay attention to context cues to understand how to phrase your communication. Users range from experienced developers to complete beginners. Briefly explain terms if in doubt.

---

## Creating a skill

### Capture Intent

Start by understanding the user's intent. The current conversation might already contain a workflow the user wants to capture. If so, extract answers from the conversation history first — the tools used, the sequence of steps, corrections the user made, input/output formats observed. The user may need to fill gaps.

1. What should this skill enable Claude to do?
2. When should this skill trigger? (what user phrases/contexts)
3. What's the expected output format?
4. Should we set up test cases to verify the skill works?

### Interview and Research

Proactively ask questions about edge cases, input/output formats, example files, success criteria, and dependencies. Wait to write test prompts until you've got this part ironed out.

Check available MCPs — if useful for research, use them. Come prepared with context to reduce burden on the user.

### Write the SKILL.md

Based on the user interview, fill in these components:

- **name**: Skill identifier (lowercase-with-hyphens, matches directory name)
- **description**: When to trigger, what it does. This is the primary triggering mechanism — include both what the skill does AND specific contexts for when to use it. Make it slightly "pushy" — lean toward triggering when relevant, not away.
- **the rest of the skill body**: Instructions, examples, output formats

### Skill Writing Guide

#### Anatomy of a Skill

```
skill-name/
├── SKILL.md (required)
│   ├── YAML frontmatter (name, description required)
│   └── Markdown instructions
└── Bundled Resources (optional)
    ├── scripts/    - Executable code for deterministic/repetitive tasks
    ├── references/ - Docs loaded into context as needed
    └── assets/     - Files used in output (templates, icons, fonts)
```

#### Progressive Disclosure

Skills use a three-level loading system:
1. **Metadata** (name + description) — Always in context (~100 words)
2. **SKILL.md body** — In context whenever skill triggers (<500 lines ideal)
3. **Bundled resources** — As needed (unlimited)

Keep SKILL.md under 500 lines. If approaching this limit, add an additional layer of hierarchy with clear pointers to follow-up files.

**Domain organization**: When a skill supports multiple domains/frameworks, organize by variant:
```
cloud-deploy/
├── SKILL.md (workflow + selection)
└── references/
    ├── aws.md
    ├── gcp.md
    └── azure.md
```

#### Writing Patterns

Prefer the imperative form in instructions.

**Defining output formats:**
```markdown
## Report structure
ALWAYS use this exact template:
# [Title]
## Executive summary
## Key findings
## Recommendations
```

**Examples pattern:**
```markdown
## Commit message format
**Example 1:**
Input: Added user authentication with JWT tokens
Output: feat(auth): implement JWT-based authentication
```

### Writing Style

Explain to the model *why* things are important rather than heavy-handed MUSTs. Use theory of mind. Start by writing a draft, then look at it with fresh eyes and improve it. If you find yourself writing ALWAYS or NEVER in all caps, reframe and explain the reasoning instead.

### Test Cases

After writing the skill draft, come up with 2–3 realistic test prompts — the kind a real user would actually say. Share them with the user for confirmation, then run them.

---

## Running and evaluating test cases

### Without eval infrastructure (qualitative)

For each test case:
1. Read the skill's SKILL.md
2. Follow its instructions to accomplish the test prompt yourself
3. Present the output to the user
4. Ask for feedback: "How does this look? Anything you'd change?"
5. Improve the skill based on feedback and repeat

Organize outputs into iteration directories on the filesystem:
```
{skill-name}-workspace/
├── iteration-1/
│   ├── eval-0/output.md
│   └── eval-1/output.md
└── iteration-2/
    └── ...
```

### With eval infrastructure (if scripts/ is available)

If `skill-creator/scripts/` exists, follow the full eval workflow:

1. Save test cases to `evals/evals.json`
2. Spawn with-skill and baseline subagents in parallel for each test case
3. Draft assertions while runs are in progress
4. Grade results and aggregate into `benchmark.json`
5. Launch the viewer: `python -m scripts.aggregate_benchmark` + `eval-viewer/generate_review.py`
6. Read `feedback.json` after user review
7. Improve and repeat

See `agents/grader.md`, `agents/analyzer.md`, and `references/schemas.md` if those files exist.

---

## Improving the skill

### How to think about improvements

1. **Generalize from the feedback.** The skill will run many times across many prompts. Avoid overfitting to the test cases — use different metaphors or patterns if something is stubborn.

2. **Keep the prompt lean.** Remove things that aren't pulling their weight. If the skill is making the model waste time on unproductive steps, cut those parts.

3. **Explain the why.** Try hard to explain the reasoning behind each instruction. LLMs respond better to understanding than to rigid rules.

4. **Look for repeated work.** If test runs all independently wrote the same helper script, bundle it in `scripts/`.

### The iteration loop

1. Apply improvements to the skill
2. Rerun all test cases into a new `iteration-{N+1}/` directory
3. Present outputs to the user (or use viewer if available)
4. Read feedback, improve, repeat

Keep going until:
- The user says they're happy
- The feedback is all empty (everything looks good)
- You're not making meaningful progress

---

## Description Optimization

The description field is the primary mechanism that determines whether Claude invokes a skill. After creating or improving a skill, offer to optimize it.

### Manual approach (without scripts)

1. Write 10 test queries — half should trigger the skill, half should not
2. For each query, determine whether the current description would cause Claude to use this skill
3. Identify patterns in what the description misses
4. Revise the description to cover the missed cases while excluding the non-cases
5. Repeat until precision and recall are both high

### Automated approach (if scripts/run_loop.py is available)

1. Generate 20 eval queries (10 should-trigger, 10 should-not-trigger) and save as JSON
2. Run: `python -m scripts.run_loop --eval-set <path> --skill-path <path> --max-iterations 5`
3. Apply `best_description` from the JSON output to the skill's frontmatter

---

## Claude.ai-specific instructions

In Claude.ai, the core workflow is the same (draft → test → review → improve → repeat), but subagents are not available. Adapt as follows:

- **Running test cases**: Read the skill's SKILL.md, then follow its instructions yourself for each test prompt, one at a time
- **Reviewing results**: Present results directly in the conversation
- **Benchmarking**: Skip quantitative benchmarking — focus on qualitative feedback
- **Description optimization**: Run the manual approach above

---

## Updating an existing skill

- Preserve the original name (directory name and frontmatter `name` field)
- Read the existing SKILL.md before editing
- If the installed skill path may be read-only, copy to a writable location first

---

## Three-tier skill architecture (this template)

New skills created with this skill go into `.ai/skills/{name}/SKILL.md` (source of truth).
Run `/update-skills` after creating or editing to sync:
- `.claude/skills/{name}/SKILL.md` — thin wrapper for Claude Code
- `.github/skills/{name}/SKILL.md` — full copy for GitHub Copilot
