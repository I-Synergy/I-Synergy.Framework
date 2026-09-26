# Council Principle (High-Stakes Decisions)

> A decision that is expensive to reverse is not made by one agent's judgment. It is made by a
> council: seats form their positions **independently**, a chair reconciles them on the merits,
> and dissent is **recorded verbatim, never averaged away**.

This is a decision *process*, not a coding rule: it is not a numbered entry in the Critical Coding
Rules and does not appear in `.ai/reference/critical-rules.md`. Those describe how code must be
written. This describes how a decision that cannot be cheaply undone must be taken.

## What Makes It a Council

Four properties separate a council from N redundant reviewers. Remove any one and you have paid N×
the cost for a single opinion:

1. **Independence first** — no seat sees another seat's position, or the chair's leaning, before its
   own is formed. Without this, seats herd toward the first answer they read.
2. **Conflicting mandates** — seats are chosen because they will disagree, not because they will
   agree. Agreement is the signal to re-examine the seat list, not to stop.
3. **Self-objection required** — every seat returns the strongest argument against its own position.
4. **Dissent recorded verbatim** — the chair writes down what was rejected and why, plus the
   conditions that would reopen the decision. Averaging is forbidden.

The verdict is **advisory**: the chair decides on the merits after positions are fixed. A council is
not a vote, and it never decides what the user actually wants (see *Relationship to Existing
Mechanisms*).

## When to Convene

Convene when **any one** of these is true. One trigger is enough; more than one is a firmer signal.

- [ ] The change alters an **established architecture decision** — bounded contexts, layering,
      project structure, technology choice, or a pattern documented in `.ai/patterns/`
- [ ] The change **breaks a public contract** — a contract that other code or consumers depend on
      cannot follow the change without modification. An additive, backward-compatible change (a new
      optional parameter, a regenerated client) does not convene.
- [ ] The change is a **destructive or transform-in-place data migration** — one that drops,
      rewrites, or cannot be replayed from a backup
- [ ] The change alters the **existing** security posture — it affects the authn/authz model, secret
      handling, or the exposure surface, rather than applying the documented pattern
- [ ] The user or a reviewer **flags it as irreversible** — "we can't easily undo this"
- [ ] Two documented rules or references **conflict**, and neither obviously wins

Triggers describe the *decision*, not the size of the diff: a one-line route change that breaks a
published client is high-stakes; a 40-file behavior-preserving refactor is not.

## When NOT to Convene

The negative list is what stops the council decaying into ceremony. Do **not** convene for:

- **Bug fixes with a known root cause** — the answer is already determined
- **Work copying an existing reference implementation** or template in this repo
- **Applies an existing documented pattern** — a new endpoint, handler, migration, or component that
  follows the template's rules and a reference implementation as written
- **Behavior-preserving refactors** — renames, moves, extraction, file reorganization
- **Tests and documentation**
- **A decision already covered by a recorded council** — revisit only when that record's *reopen
  conditions* fire, and cite them when you do

If a trigger fires, it outranks the work-kind exclusions above — convene even when one of them also
seems to apply. The one exception is a decision already covered by a recorded council: revisit it
only when that record's reopen conditions fire.

If you cannot tell whether a trigger fires, the default is **not** to convene: state the framing
question to the user in one line and let them decide. That costs one message where a mis-triggered
council costs five seats, five cross-examination answers, and the chair's three steps.

## Seat Selection

**Three seats is the minimum useful council; five seats is the ceiling, with one cross-examination
round.** Seats are chosen for **conflicting mandates**, not coverage, and are named by mandate —
the mandate is what the seat is briefed with. The default roster:

| Seat (mandate) | The question it must answer | Agent type |
|---|---|---|
| **Design** | Does the design hold up under this change? | `architect` |
| **Rules** | Does it comply with the critical rules and documented patterns? | `reviewer` |
| **Security** | What is the blast radius, and who or what is exposed? | `security` |
| **Verifiability** | Is the claim in the positions verifiable, and how? | `tester` |
| **Feasibility** | Is it implementable exactly as described? | `developer` / `ui-developer` |

- **Two seats may share an agent type only when their mandates are explicitly different** — one
  `developer` seat arguing the migration path, a second arguing the rollback. They must be
  **separate invocations**, each briefed on its own mandate; never brief one invocation to "cover
  both".
- **The Rules and Security seats are distinct agents, but their skill sets still overlap** — Rules
  runs as `reviewer`, which still loads the `security` skill alongside its review skills, so the
  Rules position can still carry security reasoning; Security runs as the `security` agent. The
  personas now differ, yet the two positions can duplicate each other in substance even though the
  mandates differ. The chair must still check them for duplication before synthesis.
- **Do not add a seat for redundancy** — a sixth seat that will agree adds cost and no information.
- **The chair does not hold a seat** — a chair that argued a position cannot brief neutrally.
- For a UI-facing decision swap Feasibility to `ui-developer` and Verifiability to `ui-tester`;
  `designer` may hold a seat on visual-language decisions. `writer` is never a seat — documentation
  records decisions, it does not contest them.

## The Protocol

### Phase 0 — Framing (chair)

Write the framing **before any seat runs**, into the plan or progress file, so it is auditable: one
neutral decision question (no preferred answer, no leading option ordering), the constraints any
answer must satisfy, the known options — or the explicit words *"open — no options proposed yet"* —
and the deadline, or "none". Neutrality is checkable: if the first-listed option is the chair's
preference, it is not neutral.

### Phase 1 — Seat briefs

Each seat receives **the question and the constraints only** — nothing else — and returns the six
items in the seat-brief template: position, reasoning, evidence, confidence, **the strongest argument
against its own position**, and what would change its mind.

### Phase 2 — Independent positions

Each seat is briefed **separately and sees neither the other seats' positions nor the chair's
leaning** — in practice, a separate agent invocation with a fresh context per seat. If your runner
shares one conversation across steps (some chain runners do), the seats are not independent: run each
seat as its own invocation instead. A council that violates this is not a council — it is one opinion
billed N times.

### Phase 3 — Cross-examination (one round, maximum)

The chair returns the positions to all seats **anonymized** (seat A/B/C, not agent names), and each
seat states where it still disagrees and why. One round, bounded: no open-ended debate, no second
round, no negotiation between seats. A seat that changes position does so once, in writing.

### Phase 4 — Synthesis (chair)

The chair writes the decision. It must contain all five of: (1) the **decision**, as an instruction
someone can act on; (2) the **rationale**, in the terms of the positions; (3) **which positions were
adopted and which rejected**, and on what grounds; (4) **unresolved dissent, recorded verbatim** —
the dissenting seat's own words, quoted, not paraphrased and not softened; (5) the **conditions that
would reopen the decision** — specific and checkable.

### Phase 5 — Escalation

Escalate to the user via the escalation path in `.ai/reference/task-execution.md` when the chair
cannot decide on the merits after synthesis, or when the decision overrides a critical-rule violation
or a security blocker a seat raised. Unresolved dissent is **never silently absorbed**; every
escalation names what was tried, the positions, and the options.

### Phase 6 — Record

The synthesis is written into the plan or progress file. A record is looked up in the task's
`.ai/plans/{task-slug}.md`, `.ai/progress/{task-slug}.md`, or `.ai/completed/{task-slug}.md` — the
progress file is moved to `.ai/completed/` when the task completes, so a settled decision stays
findable and does not convene a second council. **Council records are the audit trail** — positions,
dissent, and reopen conditions are what a future session reads. A decision that cannot be traced
back to a record did not go through a council.

## The Self-Objection Requirement

Item 5 of the seat brief is not a courtesy. An agent asked for "risks" produces reassuring risks, and
an agent asked to review its own position agrees with it. Demanding **the strongest case against the
seat's own position** is the cheap way to force the counter-argument into the record before the chair
reads it. A position returned without a self-objection is **incomplete** and goes back to the seat —
the chair never invents a seat's counter-argument.

## Anti-Patterns

| Anti-pattern | What it looks like |
|---|---|
| **Shared context** | Seats run in one conversation; seat 2 has read seat 1's answer, so everything after seat 1 is anchoring. |
| **Asking "do you agree?"** | The brief asks for agreement with a proposal instead of independent positions. The question itself is the anchor. |
| **Averaging** | The synthesis is a middle position no seat argued for — "we'll do a bit of both". Mush is not consensus. |
| **Convening everything** | The trigger list is stretched to cover ordinary work; output gets ignored and the real trigger is lost in the noise. |
| **The chair voting** | The chair counts seats — 3–2 becomes the answer — instead of deciding on the merits and recording the dissent. |
| **Ownership laundering** | A council is convened to diffuse responsibility for a decision nobody wants to own. The chair still owns the decision; a council does not transfer that. |

## Cost Discipline

Cost scales with **how expensive the decision is to reverse**, never with how large the diff is.

- **3 seats** — the minimum useful council. Two seats cannot produce a genuine three-way conflict, and
  a chair tiebreak is not synthesis.
- **5 seats and one cross-examination round** — the ceiling. Beyond it the marginal seat repeats an
  existing mandate and the marginal round repeats the first.
- **Cheap seats on an irreversible decision is false economy** — the decision that cannot be undone is
  exactly the one worth the deep-tier tokens.
- **Five expensive seats on a reversible decision is theatre** — if a change is cheap to undo, undo it
  and learn, or take it to a single `reviewer`.

Tier guidance: the chair runs at the **deep tier** — that call is the point of the process. Seats run
at the **deep tier** by default; a seat may run at the **fast tier** only when its mandate is a lookup
against a written artifact rather than a judgment, e.g. "does this violate a numbered rule in
`.ai/reference/critical-rules.md`", where the answer is checked, not weighed.

## Templates

### Seat brief — what the chair hands each seat

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

### Council record — what the chair writes after synthesis

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

## Worked Example

**Framing (Phase 0).** *Should `PUT /budgets/{id}` be replaced by `PATCH /budgets/{id}`, or should
PATCH be added alongside PUT?* Constraints: the Kiota client is regenerated on every build (the
OpenAPI MSBuild target in `.ai/reference/critical-rules.md`), so in-repo breakage surfaces at build
time; external consumers are unknown. Known options: (A) keep PUT, add PATCH; (B) replace PUT.
Seats: Design, Rules, Verifiability.

**Positions (Phase 2), formed independently, each with its self-objection.**

| Seat | Position | Conf. | Strongest argument against itself |
|---|---|---|---|
| Design | **B** — PUT advertises full replacement while the handler accepts partial updates. The contract already lies; fix it, do not duplicate it. | high | *"a client that ships on a slower cadence than this build breaks at runtime with 405 — my position assumes the consumer inventory is empty, and nothing proves that."* |
| Rules | **A** — additive under the route and `.Produces` conventions; B forces a version bump with no deprecation window and leaves two write paths in no better order. | medium | *"keeping both means two write paths to the same command, which forks validation and doubles the test surface."* |
| Verifiability | **A** — B's load-bearing claim ("no external consumer breaks") is not checkable from this repo; A's claim is per-route and testable now. | medium | *"I am choosing the option I can test, not the option that is correct."* |

**Cross-examination (Phase 3).** Rules and Verifiability hold A. Design does not move but concedes
the load-bearing point: *"my argument depends on an inventory no seat verified."*

**Synthesis (Phase 4).**

- **Decision:** adopt **A** — add `PATCH /budgets/{id}` with explicit `.Produces` metadata and
  validation; keep `PUT /budgets/{id}` unchanged.
- **Rationale:** two seats reached A on independent grounds (surface area, testability); the only
  surviving argument for B was conditional on an unverified fact. B would spend a breaking change to
  fix a documentation defect.
- **Adopted:** Rules (additive under the route conventions); Verifiability (per-route verification).
  **Rejected:** Design's B — its merit is real and is carried forward as the follow-up.
- **Recorded dissent (verbatim):** *"Design seat: the PUT route still advertises full replacement
  while the handler accepts partial updates. Keeping it means the OpenAPI spec keeps lying to every
  generated client, and we have now documented that as intentional."*
- **Reopen conditions:** (1) a consumer inventory showing no external caller of `PUT /budgets/{id}`;
  (2) a versioned route plus a deprecation window. Either fires → reconvene; expect B to carry.

## Relationship to Existing Mechanisms

Use the council for the last row only. Where an existing mechanism covers the need, use it instead:

| Situation | Mechanism |
|---|---|
| Open questions are about **what the user wants** — scope, intent, requirements | `design-interrogation` (`.ai/skills/design-interrogation/SKILL.md`) interrogates the *user* to resolve a design tree. Seats argue positions; they cannot decide someone else's intent. |
| The design **is already fixed** and you need to know whether the code matches it | `gap-review` (`.ai/skills/gap-review/SKILL.md`) — a check against a fixed design, not a decision. |
| **One mandate is enough** — bug fix, behavior-preserving refactor, pattern work, a change to be reviewed | `reviewer`, or the `code-reviewer` skill (`.ai/skills/code-reviewer/SKILL.md`). Test: if every seat would agree, do not convene. |
| You need a **design produced**, with no expensive-to-reverse decision in it | The `architect` agent. Producing a design and contesting a decision are different jobs. |
| You are **stuck on how**, after failed retries | The escalation path in `.ai/reference/task-execution.md`. Council Phase 5 reuses its shape: stop, state what was tried, present options, ask. |
| The decision is **expensive to reverse** and two mandates would genuinely disagree | **This document.** |

Recording is not a separate mechanism: framing and synthesis go into the task's existing
`.ai/plans/{task-slug}.md` or `.ai/progress/{task-slug}.md`. There is no separate council record
format, and no separate directory.

## Quick Checklist

- [ ] I can name the trigger from *When to Convene* that fired
- [ ] Phase 0 framing written down, neutral, and predating every seat position
- [ ] 3–5 seats with distinct mandates, none added for redundancy; the chair held no seat
- [ ] Each seat ran as a separate invocation with a fresh context, briefed with the question and
      constraints only
- [ ] Every position carries a self-objection; incomplete positions were sent back
- [ ] At most one cross-examination round
- [ ] Synthesis names adopted and rejected positions
- [ ] Dissent recorded verbatim, or explicitly stated as "none"
- [ ] Reopen conditions are specific and checkable
- [ ] Unresolvable dissent escalated to the user, not absorbed
- [ ] The record is written into the task's plan or progress file
