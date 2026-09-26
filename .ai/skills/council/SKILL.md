---
name: council
description: Convenes a council for a decision that is expensive to reverse — seats briefed with conflicting mandates return positions formed independently, each carrying a self-objection, and a chair synthesizes on the merits while recording dissent verbatim instead of averaging it away. Use when a change alters an established architecture decision, breaks a public contract that other code or consumers depend on, performs a destructive data migration, alters the existing security posture rather than applying the documented pattern, or when two documented rules conflict with no obvious winner. Trigger whenever the user says "run a council", "convene a council", "council this decision", "get independent positions on this", or flags a choice as irreversible. Advisory verdict, never a vote. Do not trigger for bug fixes with a known root cause, additive or backward-compatible changes, behavior-preserving refactors, tests, or documentation.
---

# Council

A council is a decision process for choices that are expensive to reverse: seats briefed with
**conflicting mandates** return **positions formed independently**, a chair reconciles them on the
merits, and dissent is **recorded verbatim, never averaged away**. The verdict is **advisory** — the
chair decides, and a council never decides what the user actually wants.

The canonical document is [`.ai/reference/council.md`](../../reference/council.md). This skill is
its invocable form; where the two appear to disagree, that document wins.

## When to Run

Convene when **any one** of these fires — one trigger is enough, more than one is a firmer signal:

- The change alters an **established architecture decision** — bounded contexts, layering, project
  structure, technology choice, or a pattern documented in `.ai/patterns/`
- The change **breaks a public contract** — a contract that other code or consumers depend on
  cannot follow the change without modification. An additive, backward-compatible change (a new
  optional parameter, a regenerated client) does not convene.
- The change is a **destructive or transform-in-place data migration** — one that drops, rewrites,
  or cannot be replayed from a backup
- The change alters the **existing** security posture — it affects the authn/authz model, secret
  handling, or the exposure surface, rather than applying the documented pattern
- The user or a reviewer **flags it irreversible** — "we can't easily undo this"
- Two documented rules or references **conflict**, and neither obviously wins

Triggers describe the *decision*, not the size of the diff: a one-line route change that breaks a
published client is high-stakes; a 40-file behavior-preserving refactor is not.

**Do not convene** for a bug fix with a known root cause, work copying an existing reference
implementation, an existing documented pattern applied as written (a new endpoint, handler,
migration, or component that follows the template's rules and a reference implementation), a
behavior-preserving refactor, tests or documentation, or a decision already covered by a recorded
council (revisit only when that record's reopen conditions fire, and cite them when you do). The
full negative list is in the reference document. If a trigger fires, it outranks the work-kind
exclusions above — convene even when one of them also seems to apply. The one exception is a
decision already covered by a recorded council: revisit it only when that record's reopen conditions
fire.

If you cannot tell whether a trigger fires, the default is **not** to convene: state the framing
question to the user in one line and let them decide. That costs one message where a mis-triggered
council costs five seats, five cross-examination answers, and the chair's three steps.

## Inputs

| Input | Source |
|-------|--------|
| The decision, framed as one neutral question | Written by the chair in Step 1 |
| Constraints any answer must satisfy | The chair, from the task's plan and hard requirements |
| Known options, or "open — no options proposed yet" | The chair, from the plan or the user |
| The repo's own rules and patterns | `.ai/reference/critical-rules.md`, `.ai/patterns/` |
| The agent roster the seats are drawn from | `.ai/agents/` |
| An existing council record, if one covers this decision | The task's `.ai/plans/{task-slug}.md`, `.ai/progress/{task-slug}.md`, or `.ai/completed/{task-slug}.md` (the progress file moves to `.ai/completed/` on completion) |

## Steps

Run the phases in order. The chair orchestrates and writes; every seat is a **separate invocation
with a fresh context**.

### 1. Frame (chair)

Write the framing **before any seat runs**, into the task's `.ai/plans/{task-slug}.md` or
`.ai/progress/{task-slug}.md`, so it is auditable: one neutral decision question (no preferred
answer, no leading option ordering), the constraints any answer must satisfy, the known options —
or the explicit words *"open — no options proposed yet"* — and the deadline, or "none".

Neutrality is checkable: if the first-listed option is the chair's preference, it is not neutral.

### 2. Write the seat briefs

Pick 3–5 seats (see **Seat Selection**) and write one brief per seat using the
**Seat Brief Template** below. Each seat receives **the question and the constraints only** —
nothing else. Size is bounded: three seats minimum, five seats and one cross-examination round
maximum.

### 3. Collect independent positions

Run each seat as its **own invocation with a fresh context**. No seat sees another seat's position
or the chair's leaning before its own is formed — if your runner shares one conversation across
steps (some chain runners do), the seats are not independent: invoke each one separately instead. A
council that violates this is not a council — it is one opinion billed N times. If you cannot run
each seat as its own invocation with a fresh context, do not convene a council — say so plainly and
use a single `reviewer` with a stated mandate instead.

Every position must carry a **self-objection**: the strongest case against the seat's own answer. A
position returned without one is **incomplete** and goes back to the seat — the chair never invents
a seat's counter-argument.

### 4. Cross-examine — one round, maximum

Return the positions to all seats **anonymized** (seat A/B/C, not agent names) and ask each seat
where it still disagrees, and why. One round, bounded: no open-ended debate, no second round, no
negotiation between seats. A seat that changes position does so once, in writing.

### 5. Synthesize (chair)

The chair writes the decision. Because the chair does not hold a seat, it can weigh the positions
on the merits rather than defend one of them. The synthesis must contain all five of:

1. the **decision**, as an instruction someone can act on
2. the **rationale**, in the terms of the positions
3. **which positions were adopted and which rejected**, and on what grounds
4. **unresolved dissent, recorded verbatim** — the dissenting seat's own words, quoted, not
   paraphrased and not softened
5. the **conditions that would reopen the decision** — specific and checkable

Averaging is forbidden. A middle position no seat argued for is mush, not consensus.

### 6. Escalate when the chair cannot decide

Escalate to the user via the escalation path in `.ai/reference/task-execution.md` when the chair
cannot decide on the merits after synthesis, or when the decision would override a critical-rule
violation or a security blocker a seat raised. Unresolved dissent is **never silently absorbed**;
every escalation names what was tried, the positions, and the options.

### 7. Record

Write the synthesis into the same plan or progress file, using the **Council Record** template in
**Output Format**. Council records are the audit trail — positions, dissent, and reopen conditions
are what a future session reads. A decision that cannot be traced back to a record did not go
through a council.

## Seat Selection

Seats are chosen for **conflicting mandates**, not coverage, and are named by mandate — the mandate
is what the seat is briefed with. The default roster:

| Seat (mandate) | The question it must answer | Agent type |
|---|---|---|
| **Design** | Does the design hold up under this change? | `architect` |
| **Rules** | Does it comply with the critical rules and documented patterns? | `reviewer` |
| **Security** | What is the blast radius, and who or what is exposed? | `security` |
| **Verifiability** | Is the claim in the positions verifiable, and how? | `tester` |
| **Feasibility** | Is it implementable exactly as described? | `developer` / `ui-developer` |

- **The Security seat is the [`security`](../../agents/security.md) agent** — a read-only security
  persona (it reports, it does not fix) that loads the [`security`](../security/SKILL.md) skill
  alongside `api-security` and `software-security`. It must still be a **separate invocation** from
  the Rules seat: the two seats hold different mandates, and one invocation is never briefed to
  "cover both".
- **The Rules and Security seats are distinct agents, but their skill sets still overlap** — Rules
  runs as `reviewer`, which still loads the `security` skill alongside `code-reviewer` and
  `gap-review`, so the Rules position can still carry security reasoning; Security now runs as the
  `security` agent. The personas differ, but the overlap has not vanished: the two positions can
  still duplicate each other in substance even though the mandates differ. The chair must still
  check them for duplication before synthesis.
- **Do not add a seat for redundancy** — a sixth seat that will agree adds cost and no information.
- **Do not add a seat for coverage** — if every seat would agree, do not convene at all.
- For a UI-facing decision swap Feasibility to `ui-developer` and Verifiability to `ui-tester`;
  `designer` may hold a seat on visual-language decisions. `writer` is never a seat — documentation
  records decisions, it does not contest them.

**Tier guidance.** The chair runs at the **deep tier** — that call is the point of the process.
Seats run at the **deep tier** by default; a seat may run at the **fast tier** only when its mandate
is a lookup against a written artifact rather than a judgment, e.g. "does this violate a numbered
rule in `.ai/reference/critical-rules.md`", where the answer is checked, not weighed. Cheap seats on
an irreversible decision is false economy; five expensive seats on a reversible decision is theatre.

## Seat Brief Template

Hand each seat this brief — the question and the constraints only, nothing else:

```
# Council Seat Brief — {decision-slug}

**Decision question:** {one neutral question, no preferred answer}
**Seat:** {mandate — e.g. "Security: blast radius and exposure"}
**Constraints:** {non-negotiable constraints any answer must satisfy}
**Known options:** {A / B / C}   or:  open — no options proposed yet
**Deadline:** {date}   or:  none

You do not see any other seat's position, and you do not see the chair's leaning.

Return exactly these six items:
1. Position   — the answer you would commit to, in one paragraph.
2. Reasoning  — why, in the order you would defend it.
3. Evidence   — file paths, rule numbers, measured behavior, documented references. Cite, do not assert.
4. Confidence — high / medium / low, and what it is conditional on.
5. Strongest argument against your own position — the best case for the answer you rejected. Required.
6. What would change your mind — a specific, checkable condition.
```

## Output Format

Write the record into the task's existing `.ai/plans/{task-slug}.md` or
`.ai/progress/{task-slug}.md` — there is no separate council directory and no separate format:

```
# Council Record — {decision-slug}

**Status:** DECIDED | ESCALATED TO USER
**Date:** {date}
**Trigger:** {which high-stakes trigger fired}
**Decision question:** {the Phase 0 framing, verbatim}
**Seats:** {mandate → agent type}, one per line

## Positions
| Seat | Position (summary) | Confidence |
|---|---|---|

## Decision
{the decision, as an instruction someone can act on}

## Rationale
- Adopted: {seat + what was taken from it}
- Rejected: {seat + what was rejected, and on what grounds}

## Recorded dissent (verbatim)
> **{seat}:** {the dissenting argument, quoted as written}

## Reopen conditions
- {specific, checkable condition} → reconvene

## Escalation
{what was escalated, with which options}   (omit when Status is DECIDED)
```

When dissent is absent, say so explicitly as "none" — never leave the section out, so a reader can
tell the difference between "no dissent" and "dissent not looked for".

## Anti-Patterns

| Anti-pattern | What it looks like |
|---|---|
| **Shared context** | Seats run in one conversation; seat 2 has read seat 1's answer, so everything after seat 1 is anchoring. |
| **Asking "do you agree?"** | The brief asks for agreement with a proposal instead of independent positions. The question itself is the anchor. |
| **Averaging** | The synthesis is a middle position no seat argued for — "we'll do a bit of both". |
| **Convening everything** | The trigger list is stretched to cover ordinary work; the real trigger is lost in the noise. |
| **The chair voting** | The chair counts seats — 3–2 becomes the answer — instead of deciding on the merits and recording the dissent. |
| **Ownership laundering** | A council is convened to diffuse responsibility for a decision nobody wants to own. The chair still owns the decision. |

## Checklist

- [ ] I can name the trigger from **When to Run** that fired
- [ ] Framing written down, neutral, and predating every seat position
- [ ] 3–5 seats with distinct mandates, none added for redundancy; the chair held no seat
- [ ] Each seat ran as a separate invocation with a fresh context, briefed with the question and constraints only
- [ ] Every position carries a self-objection; incomplete positions went back to the seat
- [ ] At most one cross-examination round
- [ ] Synthesis names adopted and rejected positions
- [ ] Dissent recorded verbatim, or explicitly stated as "none"
- [ ] Reopen conditions are specific and checkable
- [ ] Unresolvable dissent escalated to the user, not absorbed
- [ ] The record is written into the task's plan or progress file

## Related Skills

Use the council for the last row only. Where an existing mechanism covers the need, use it instead:

| Situation | Use |
|-----------|-----|
| Open questions are about **what the user wants** — scope, intent, requirements | [`design-interrogation`](../design-interrogation/SKILL.md) interrogates the *user* to resolve a design tree. Seats argue positions; they cannot decide someone else's intent. |
| The design **is already fixed** and you need to know whether the code matches it | [`gap-review`](../gap-review/SKILL.md) — a check against a fixed design, not a decision. |
| **One mandate is enough** — bug fix, refactor, pattern work, a change to be reviewed | [`code-reviewer`](../code-reviewer/SKILL.md). If every seat would agree, do not convene. |
| You need a **design produced**, with no expensive-to-reverse decision in it | The `architect` agent. Producing a design and contesting a decision are different jobs. |
| You are **stuck on how**, after failed retries | The escalation path in `.ai/reference/task-execution.md`; council Step 6 reuses its shape. |
| The decision is **expensive to reverse** and two mandates would genuinely disagree | **This skill.** |
