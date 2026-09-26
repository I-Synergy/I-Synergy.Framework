---
name: council
description: High-stakes decision council — five seats form positions independently, one cross-examination round, the chair decides on the merits and records dissent verbatim
---

> **Independence caveat — the limitation ships with this chain.**
> A chain is a linear pipeline (`phase:` steps joined by `reads:`), and a pipeline cannot enforce
> that one seat never sees another seat's position. If your runner executes these steps in **one
> shared conversation**, seat 2 has already read seat 1's answer, the positions are not independent,
> and the result is a panel — one opinion billed five times — not a council
> (`.ai/reference/council.md` → Phase 2 → *Independence first*). If your runner shares context, do
> not run the seats as chain steps: run **each seat as its own separate invocation**, briefed with
> the framing and the constraints only, and have the chair synthesize afterwards. A council that
> violates this is not a council.
>
> The same test applies to the chair. `architect` runs the Framing, the Cross-examination digest,
> and the Synthesis, and it also holds the Design seat and answers its own rebuttal step; each of
> those steps must be a fresh invocation. If contexts are shared, the council must run as separate
> invocations instead — one per seat, plus a chair invocation that formed no position. A chair that
> argued a position cannot brief or weigh it neutrally, and no instruction can un-bias one.
>
> **Roster and tier note.** This chain runs the principle's full five-seat default roster; for a
> UI-facing decision, follow `.ai/reference/council.md` directly and swap Feasibility to
> `ui-developer` and Verifiability to `ui-tester`. It sets no `model:` keys — the template routes
> model tiers per backend through environment variables, so a hardcoded model name here would break
> the DeepSeek, Claude, Copilot, and Reasonix mappings. The
> principle wants the chair and every judgment seat at the deep tier
> (`.ai/reference/council.md` → *Cost Discipline*); if your backend maps a seat's agent to the fast
> tier, that seat is under-tiered for its mandate. A decision that needs fewer seats should follow
> `.ai/reference/council.md` directly with three rather than run seats here that will agree.

## architect
phase: Framing
label: Chair — neutral framing (Phase 0)
as: framing

Frame the high-stakes decision for: {task}

Before any seat runs, write the framing into this task's plan or progress file (`.ai/plans/` or
`.ai/progress/`), so it is auditable and predates every seat position; if the task has neither file
yet, create one under `.ai/progress/`. The framing must contain exactly:

- **Decision question** — one neutral question, no preferred answer, no leading option ordering. If
  the first-listed option is the one you prefer, it is not neutral; fix the framing, not the option.
- **Constraints** — the non-negotiable constraints any answer must satisfy.
- **Known options** — A / B / C, or the exact words `open — no options proposed yet`.
- **Deadline** — a date, or `none`.
- **Trigger** — which trigger from `.ai/reference/council.md` → *When to Convene* fired.

If you cannot name a trigger, write no framing and stop: the default is not to convene
(`.ai/reference/council.md` → *When NOT to Convene*), and the remaining steps must not run.

Return the framing text as your output for the seats to read. Do not state or imply a preferred
option — this step frames, it does not argue a position.

## architect
phase: Design seat
label: Seat A — Design
as: design
reads: {outputs.framing}

**Mandate — Design:** Does the design hold up under this change? Judge bounded contexts, layering,
project structure, technology choice, and the patterns documented in `.ai/patterns/`
(`.ai/reference/council.md` → *Seat Selection*).

You hold the Design seat only. Form your position from the framing alone: do not read, request, or
infer any other step's output, and if any other seat's position or the chair's leaning is already
present in your context, ignore it entirely — it is not part of your brief.

Return exactly these six items:
1. **Position** — the answer you would commit to, in one paragraph.
2. **Reasoning** — why, in the order you would defend it.
3. **Evidence** — file paths, rule numbers, measured behavior, documented references. Cite, do not
   assert.
4. **Confidence** — high / medium / low, and what it is conditional on.
5. **Strongest argument against your own position** — the best case for the answer you rejected.
   Required; a position returned without it is incomplete, and the chair never invents your
   counter-argument for you.
6. **What would change your mind** — a specific, checkable condition.

## reviewer
phase: Rules seat
label: Seat B — Rules
as: rules
reads: {outputs.framing}

**Mandate — Rules:** Does the change comply with the critical rules and the documented patterns?
Check against `.ai/reference/critical-rules.md` and `.ai/patterns/`. This mandate is
a check against written artifacts rather than a weighing of trade-offs, so the answer is looked up,
not judged (`.ai/reference/council.md` → *Cost Discipline*).

The Security seat below is a **separate invocation** with its own agent and a different mandate.
You still load the `security` skill alongside your review skills, so hold only the Rules mandate —
do not fold security blast radius into it.

Form your position from the framing alone: do not read, request, or infer any other step's output,
and if any other seat's position or the chair's leaning is already present in your context, ignore
it entirely — it is not part of your brief.

Return exactly these six items:
1. **Position** — the answer you would commit to, in one paragraph.
2. **Reasoning** — why, in the order you would defend it.
3. **Evidence** — file paths, rule numbers, measured behavior, documented references. Cite, do not
   assert.
4. **Confidence** — high / medium / low, and what it is conditional on.
5. **Strongest argument against your own position** — the best case for the answer you rejected.
   Required; a position returned without it is incomplete, and the chair never invents your
   counter-argument for you.
6. **What would change your mind** — a specific, checkable condition.

## security
phase: Security seat
label: Seat C — Security
as: security
reads: {outputs.framing}

**Mandate — Security:** What is the blast radius, and who or what is exposed? Hold the Security
mandate as the `security` agent, whose standing skills are `.ai/skills/security/`,
`.ai/skills/api-security/`, and `.ai/skills/software-security/` (`.ai/reference/council.md` →
*Seat Selection*).

This is a **separate invocation** from the Rules seat above, which holds the Rules mandate. Hold
only the Security mandate; do not restate or extend the Rules question, and do not let another
seat's question shape your answer.

Form your position from the framing alone: do not read, request, or infer any other step's output,
and if any other seat's position or the chair's leaning is already present in your context, ignore
it entirely — it is not part of your brief.

Return exactly these six items:
1. **Position** — the answer you would commit to, in one paragraph.
2. **Reasoning** — why, in the order you would defend it.
3. **Evidence** — file paths, rule numbers, measured behavior, documented references. Cite, do not
   assert.
4. **Confidence** — high / medium / low, and what it is conditional on.
5. **Strongest argument against your own position** — the best case for the answer you rejected.
   Required; a position returned without it is incomplete, and the chair never invents your
   counter-argument for you.
6. **What would change your mind** — a specific, checkable condition.

## tester
phase: Verifiability seat
label: Seat D — Verifiability
as: verifiability
reads: {outputs.framing}

**Mandate — Verifiability:** Which load-bearing claims does each known option depend on, and which
of them are checkable from this repository — by which test, suite, or measurement? Commit to the
option whose claims can be verified here (`.ai/reference/council.md` → *Seat Selection*).

Form your position from the framing alone: do not read, request, or infer any other step's output,
and if any other seat's position or the chair's leaning is already present in your context, ignore
it entirely — it is not part of your brief.

Return exactly these six items:
1. **Position** — the answer you would commit to, in one paragraph.
2. **Reasoning** — why, in the order you would defend it.
3. **Evidence** — file paths, rule numbers, measured behavior, documented references. Cite, do not
   assert.
4. **Confidence** — high / medium / low, and what it is conditional on.
5. **Strongest argument against your own position** — the best case for the answer you rejected.
   Required; a position returned without it is incomplete, and the chair never invents your
   counter-argument for you.
6. **What would change your mind** — a specific, checkable condition.

## developer
phase: Feasibility seat
label: Seat E — Feasibility
as: feasibility
reads: {outputs.framing}

**Mandate — Feasibility:** Is the change implementable exactly as described, against the actual
codebase? Inspect the real files, the migrations, the generated client, and the build — not a
plausible sketch of them. Commit to the option you can implement as described
(`.ai/reference/council.md` → *Seat Selection*).

Form your position from the framing alone: do not read, request, or infer any other step's output,
and if any other seat's position or the chair's leaning is already present in your context, ignore
it entirely — it is not part of your brief.

Return exactly these six items:
1. **Position** — the answer you would commit to, in one paragraph.
2. **Reasoning** — why, in the order you would defend it.
3. **Evidence** — file paths, rule numbers, measured behavior, documented references. Cite, do not
   assert.
4. **Confidence** — high / medium / low, and what it is conditional on.
5. **Strongest argument against your own position** — the best case for the answer you rejected.
   Required; a position returned without it is incomplete, and the chair never invents your
   counter-argument for you.
6. **What would change your mind** — a specific, checkable condition.

## architect
phase: Cross-examination digest
label: Chair — anonymized digest for the seats (Phase 3)
as: crossexamination

Convene the single cross-examination round. Read the Phase 0 framing and the five seat positions
above; take no other input.

Return a digest that opens with the decision question verbatim, then restates the five positions
anonymized by letter and mandate — A Design, B Rules, C Security, D Verifiability, E Feasibility —
never by agent name. Restate each position faithfully enough that a seat re-invoked with a fresh
context recognizes its own: the position, its reasoning in brief, and its own self-objection.

Where two positions conflict, name the conflict precisely. Do not invent an argument a seat did not
make, and do not resolve anything — the seats answer next, in one round, each writing once.

Decide nothing in this step.

## architect
phase: Design rebuttal
label: Seat A — Design rebuttal
reads: {outputs.crossexamination}

Seat A — Design. The digest above anonymized the positions as A–E by mandate, and A is yours.

Your mandate stands — Design: does the design hold up under this change? Judge bounded contexts,
layering, project structure, technology choice, and the patterns in `.ai/patterns/`. Answer from
that mandate only.

State only where you still disagree and why, naming the letters you disagree with, never agent
names. Hold your position if nothing moved you; if you change position, do so once, in writing, and
say exactly what moved you. Answer the digest alone — do not read another seat's answer to it — and
expect no second round: the cross-examination round ends with the five written answers.

## reviewer
phase: Rules rebuttal
label: Seat B — Rules rebuttal
reads: {outputs.crossexamination}

Seat B — Rules. The digest above anonymized the positions as A–E by mandate, and B is yours.

Your mandate stands — Rules: does the change comply with `.ai/reference/critical-rules.md` and
`.ai/patterns/`? The Security seat is a separate invocation with its own agent; hold the Rules
mandate only.

State only where you still disagree and why, naming the letters you disagree with, never agent
names. Hold your position if nothing moved you; if you change position, do so once, in writing, and
say exactly what moved you. Answer the digest alone — do not read another seat's answer to it — and
expect no second round: the cross-examination round ends with the five written answers.

## security
phase: Security rebuttal
label: Seat C — Security rebuttal
reads: {outputs.crossexamination}

Seat C — Security. The digest above anonymized the positions as A–E by mandate, and C is yours.

Your mandate stands — Security: what is the blast radius, and who or what is exposed? Hold the
Security mandate; your standing skills are `.ai/skills/security/`, `.ai/skills/api-security/`, and
`.ai/skills/software-security/`. The Rules seat above is a different agent; answer from your own
mandate only.

State only where you still disagree and why, naming the letters you disagree with, never agent
names. Hold your position if nothing moved you; if you change position, do so once, in writing, and
say exactly what moved you. Answer the digest alone — do not read another seat's answer to it — and
expect no second round: the cross-examination round ends with the five written answers.

## tester
phase: Verifiability rebuttal
label: Seat D — Verifiability rebuttal
reads: {outputs.crossexamination}

Seat D — Verifiability. The digest above anonymized the positions as A–E by mandate, and D is yours.

Your mandate stands — Verifiability: which load-bearing claims does each option depend on, and which
of them are checkable from this repository — by which test, suite, or measurement? Answer from that
mandate only.

State only where you still disagree and why, naming the letters you disagree with, never agent
names. Hold your position if nothing moved you; if you change position, do so once, in writing, and
say exactly what moved you. Answer the digest alone — do not read another seat's answer to it — and
expect no second round: the cross-examination round ends with the five written answers.

## developer
phase: Feasibility rebuttal
label: Seat E — Feasibility rebuttal
reads: {outputs.crossexamination}

Seat E — Feasibility. The digest above anonymized the positions as A–E by mandate, and E is yours.

Your mandate stands — Feasibility: is the change implementable exactly as described, against the
actual codebase? Inspect the real files, the migrations, the generated client, and the build — not a
plausible sketch of them. Answer from that mandate only.

State only where you still disagree and why, naming the letters you disagree with, never agent
names. Hold your position if nothing moved you; if you change position, do so once, in writing, and
say exactly what moved you. Answer the digest alone — do not read another seat's answer to it — and
expect no second round: the cross-examination round ends with the five written answers.

## architect
phase: Synthesis
label: Chair — decision and record (Phases 4-6)

Act as the chair. Read the Phase 0 framing, the five seat positions, your anonymized digest, and the
five seat rebuttal answers above; take no other input. Decide on the merits once the positions are
fixed, write the council record into the same plan or progress file that holds the Phase 0 framing,
and return it.

A council is not a vote: do not count seats, do not average the positions into a middle that no seat
argued for, and do not give the Design position any privilege — it is one input among five. Never
invent a seat's counter-argument; a position that arrived without its self-objection stays
incomplete.

Use the council-record template in `.ai/reference/council.md` → *Templates*. The record must contain
all of:
- **Status** — DECIDED or ESCALATED TO USER
- **Trigger** — which high-stakes trigger fired
- **Decision question** — the Phase 0 framing, verbatim
- **Seats** — mandate to agent type, one per line
- **Positions** — one row per seat: mandate, position summary, confidence
- **Decision** — the decision, as an instruction someone can act on
- **Rationale** — in the terms of the positions
- **Adopted / Rejected** — which positions were adopted and which rejected, and on what grounds
- **Recorded dissent (verbatim)** — the dissenting seat's own words, quoted, not paraphrased and not
  softened; or the exact words `none`
- **Reopen conditions** — specific, checkable conditions that would reopen the decision

Escalate to the user instead of deciding when you cannot decide on the merits, or when the decision
would override a critical-rule violation or a security blocker a seat raised. Use the escalation
path in `.ai/reference/task-execution.md`: state what was tried, the positions, and the options.
Unresolved dissent is never silently absorbed.
