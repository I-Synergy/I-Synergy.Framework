# Test Documentation — ISO/IEC/IEEE 29119-3

> ISO/IEC/IEEE 29119-3 defines the documents a test process produces. This document records which of
> them this template produces, which it deliberately does not, and the reason for each — so that the
> four it does produce are read, and the twelve it omits are a decision rather than an oversight. It
> is the source of truth read by the `unit-tester` and `playwright-tester` skills and by the three
> templates in `.ai/reference/templates/` that implement it.

## Purpose and Scope

29119-3:2021 defines its documentation as a set of **information items** across three tiers —
organizational (Clause 6), test management (Clause 7) and dynamic test processes (Clause 8). The
standard's conformance scheme is defined in **Annex A** — named here as unsourced context, because
this document could not obtain that annex — and no conformance level is cited from it.

**This template adopts four items and omits twelve.** The entry criterion for adoption was not "does
the standard require it" but **"does this replace something the project already needs?"** Adopting
the whole set is the documented critique of 29119 — teams produce paperwork nobody reads, and an
unread document is worse than no document, because it implies verification that never happened.

That is why the omission table below carries a reason per item. It is the load-bearing part of this
document: it is what makes "this template follows 29119-3 for test documentation" mean something
specific, and it is what stops a future reader adding a template speculatively because an annex
exists for it.

**This is not a certification artifact and nothing here asserts conformity with any standard.** It
records what this repository produces and which clause of the standard each artifact corresponds to.
Conformance is not claimed at any level — not for the adopted four, and not for the omissions, which
are a right-sizing decision rather than a statement that the standard does not require them.

**Edition.** This document follows **ISO/IEC/IEEE 29119-3:2021**. Its predecessor, **IEEE 829:2008**,
is still the vocabulary many readers carry, and Annex S of 29119-3 maps the 2021 items onto it. That
mapping is not reproduced here; the correspondence a reader is most likely looking for is:

| IEEE 829:2008 name | 29119-3:2021 item | Adopted here |
|---|---|---|
| Test plan | Test plan (7.2 / Annex E) | **Yes** |
| Test design specification | Test model specification (8.2 / Annex H) | No |
| Test case specification | Test case specification (8.3 / Annex I) | **Yes** — generated |
| Test procedure specification | Test procedure specification (8.4 / Annex J) | No |
| Test log | Test execution log (8.10 / Annex Q) | No |
| Test incident report | Test incident report (8.11 / Annex R) | **Yes** |
| Test summary report | Test completion report (7.4 / Annex G) | **Yes** |
| Test item transmittal report | — not among the sixteen items listed in this document | No |

## The Four Adopted Items

| # | Information item | Clause / Annex | Produced where | Produced by | Why it was adopted |
|---|---|---|---|---|---|
| 1 | **Test plan** | 7.2 / Annex E | `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md` | the `tester`, per vertical slice, derived from `blueprint.json` | Its entry and exit criteria are what the traceability checks T5/T6 measure against. The only genuinely new writing in this set. |
| 2 | **Test case specification** | 8.3 / Annex I | *generated — no separate file* | free by-product of `.ai/reference/traceability.md` | The acceptance-criteria ↔ scenario ↔ test mapping **is** this item. Do not author a second document for it. |
| 3 | **Test completion report** | 7.4 / Annex G | `docs/test-reports/{release}-test-completion-report.md` | per release, at release time | Carries **Test measures** and **Residual risks** — the two sections this template treats as the ones a certification auditor would sample first. |
| 4 | **Test incident report** | 8.11 / Annex R | `docs/test-reports/incidents/{TIR-id}.md` | one per failure, by whoever found it | A failure recorded only as a red test is not reproducible by anyone else; derivable from MSTest `.trx` output. |

### 1 — Test plan (7.2, Annex E)

One plan per vertical slice, written when the slice is generated and updated when its scope changes.
It is derived from `docs/slices/{BC}/{Entity}.{Operation}/blueprint.json`, so nothing in it is
invented: the items under test come from `files_to_create` and the criteria in scope come from
`source.acceptance_criteria`.

**Entry and exit criteria are the operative content.** They are what the traceability validator's
advisory checks measure against: T5 asks whether every declared criterion has at least one scenario,
T6 whether it has at least one slice and at least one test-side reference. An exit criterion that
restates "T5 and T6 are clear" ties the plan to a check that actually runs.

The template is `.ai/reference/templates/test-plan.md.txt`.

**Provenance of the section set.** The standard's normative outline for this item is Annex E, and
**Annex E is not reproduced here.** The template's sections are this template's own right-sized set,
chosen from what a vertical slice needs and from what the repository can actually fill — not
presented as the standard's outline. A section the template has and the annex does not is a template
choice; a section the annex has and the template does not is a recorded gap, and the `Test data` and
`Test environment` sections exist precisely because the slice needs them.

### 2 — Test case specification (8.3, Annex I) — generated

**This item is already produced by the traceability convention and has no authoring step.** The
mapping the standard asks a test case specification to carry — identifier, objective, the steps and
the expected result, and a reference to the test that implements it — is exactly what
`.ai/reference/traceability.md` puts in the repository:

| What a case specification carries | Where it already lives |
|---|---|
| A stable identifier for the case | `AC-{BC}-{NNN}.{n}`, declared in the story or use-case document |
| The objective / test condition | the criterion text on that checkbox line |
| Steps and expected result | the Gherkin scenario tagged `@AC-…`, plus the blueprint's implementation detail |
| The implementing test | `[TestCategory("AC-…")]` on the MSTest method, or the executable `@AC-…` scenario |

**Do not create a `test-case-specification.md`.** A generated document would be a second copy of the
chain, and the copy is the one that goes stale. `validate-traceability.py` fails when a tag, a
blueprint reference or a category does not resolve, which is a stronger guarantee than a hand-written
table that nothing checks.

### 3 — Test completion report (7.4, Annex G)

One report per release. Its sections, as the template carries them:

Overview; Summary of testing performed; Deviations from planned testing; Test completion evaluation;
Factors that blocked progress; **Test measures**; **Residual risks**; Test deliverables; Reusable test
assets; Lessons learned.

The template is `.ai/reference/templates/test-completion-report.md.txt`, and it carries those section
names. Two are worth stating here because they are the reason the item was adopted:

- **Test measures.** The report states what was measured and what the numbers were — planned,
  executed, passed, failed, skipped; criteria covered and uncovered; incidents by severity and
  status. **Quality characteristics and measures are named in `.ai/reference/quality-model.md`,
  never here.** That document is the vocabulary source and this one references it; it also records
  the boundary that matters for this section — 25010 defines no metric, measurement is ISO/IEC
  25023, and no metric is defined by either document. Report what the run produced, not a metric
  this template invented.
- **Residual risks.** What is still unverified at the point of release, what would reveal each risk,
  and who accepted it. An empty section is a claim of complete coverage and should be written only
  when it is true.

The standard's own detail for these sections is not reproduced here; the template names each section
and prompts for what this repository can actually fill.

### 4 — Test incident report (8.11, Annex R)

One report per failure, written when a test fails for a reason that is not fixed inside the same
change. The Clause 8.11 content, as the template carries it:

- a **unique identifier** — default `TIR-{yyyy-mm-dd}-{NN}`, allocated per day, the file is the registry;
- **title / summary**;
- **date**, **reporting organization**, **author and role**;
- **test object** and **test environment**;
- **context**: the test case or activity, the SDLC phase, the technique, any checklist used, and the
  test data in play;
- a **failure description sufficient to reproduce it** — steps, logs, screenshots or video;
- **expected results** and **actual results**, stated separately;
- **severity**;
- **urgency / priority**;
- **status**;
- **references** — the traced criterion, the failing test, the `.trx`, the commit, any external
  tracker key.

**Severity uses the three levels defined in `.ai/reference/quality-model.md`** — `Blocker`, `Major`,
`Minor`. This document does not define a second severity vocabulary for defects; a finding in a
review and a failure in a test use the same three words, defined once. **Urgency is a separate
axis** — severity is how bad it is, urgency is when it has to be fixed — and reporting both is what
lets a `Minor`/urgent incident be triaged differently from a `Major`/routine one.

**Status lifecycle.** An incident is in exactly one of:
`open`, `deferred`, `duplicate`, `awaiting fix`, `awaiting confirmation testing`, `reopened`,
`closed`, `rejected`. The template carries this list so the field is filled from a closed set rather
than free text. A `deferred` or `rejected` incident is still reported in the completion report's
Residual risks if it was not fixed.

The template is `.ai/reference/templates/test-incident-report.md.txt`. The standard's internal
detail for the fields is not reproduced here.

## The Twelve Omitted Items

Listed so that the omission is visible and arguable. The reason classes are the same three the
standards-compatibility view uses: **no consumer in this template**, **organizational rather than
project-level**, or **carried by an artifact that already exists** — the last being the "does this
replace something already needed?" criterion failing, because something already does the job.

**The conformance level Annex A assigns to each item is not cited here.** Annex A was not available
to this document, and writing "`may`-level" beside an item would be a claim about the standard that
this repository cannot back. Every reason below is this template's own: an omission is a right-sizing
decision, and it is **not** a statement that the standard permits the item to be left out.

| # | Information item | Clause / Annex | Reason for omission |
|---|---|---|---|
| 1 | Test policy | 6 / Annex C | **Organizational.** A test policy is set by an organization for all its projects; this repository is one project. Named in `COMPLIANCE.md` as the organizational remainder of 29119-3. |
| 2 | Organizational test practices | 6 / Annex D | **Organizational.** Same tier as the policy — the practices an organization mandates across projects. Deferred, not refused; named in `COMPLIANCE.md`. |
| 3 | Test status report | 7.3 / Annex F | **No consumer.** It reports progress *during* an execution cycle to a stakeholder who is not running the tests. Here the runner is the reader: `dotnet test` output and the `.trx` are the status, and a hand-written interval report would transcribe them. |
| 4 | Test model specification | 8.2 / Annex H | **Carried by an artifact that already exists.** The coverage model is the declared acceptance criteria themselves, and the test conditions are the scenarios tagged to them. A separate model document would restate the traceability chain in prose that nothing checks. |
| 5 | Test procedure specification | 8.4 / Annex J | **No consumer.** The procedure is executable: MSTest methods and Reqnroll step definitions *are* the ordered, runnable procedure, and the test runner is the executor. A written procedure would be a second description of code whose correctness is checked by running it. |
| 6 | Test data requirements | 8.5 / Annex K | **Folded into an adopted item.** Each slice's data needs are a section of its test plan, at the granularity where anyone would read them. A separate requirements document has no reader of its own at slice level. |
| 7 | Test environment requirements | 8.6 / Annex L | **Folded into an adopted item.** Same reasoning as test data: the test plan's environment section names what the slice needs, and the E2E level adds the browser/device matrix there rather than in a second file. |
| 8 | Test data readiness report | 8.7 / Annex M | **No consumer.** Readiness is an entry criterion in the test plan plus the run's own setup; a report saying data was ready, produced immediately before the run that proves it, is read by nobody. |
| 9 | Test environment readiness report | 8.8 / Annex N | **No consumer.** Same reasoning: environment readiness is an entry criterion, and a green run is the evidence that it held. |
| 10 | Actual results | Annex O | **Carried by an artifact that already exists.** The runner produces them: `.trx` files, console output, Playwright traces and screenshots. The item is satisfied by a build artifact, which is more faithful than a transcription of it. |
| 11 | Test result | 8.9 / Annex P | **Carried by an artifact that already exists.** Per-criterion results are readable from the `.trx` joined to `[TestCategory("AC-…")]`; per-criterion coverage is what `validate-traceability.py` reports. No authored result document is needed to answer "did this criterion pass?". |
| 12 | Test execution log | 8.10 / Annex Q | **No consumer.** Who ran what and when is recorded by CI and by `.trx` timestamps. A maintained log would be a manual copy of a machine record. |

**The organizational tier is deferred, not discharged.** Items 1–2 are the part of 29119-3 that a
repository cannot produce at all, and they are the ones the eventual `COMPLIANCE.md` names in its
"remains organizational" column. Everything else in the table above is a right-sizing decision that
individual projects are free to revisit; the organizational tier is not.

**On the number of items.** The plan that commissioned this work, and this document's own brief, say
the standard defines **17 information items** and quote the split as four adopted and thirteen
omitted. The clause structure used to build the tables above enumerates **16** — 2 in Clause 6, 3 in
Clause 7, and 11 in Clause 8 — so this document carries 16, four adopted and twelve omitted. **The
discrepancy is recorded rather than resolved:** a seventeenth row could not be sourced from the
clause breakdown available here, and inventing a plausible one to reach the advertised total would be
exactly the failure this document exists to prevent. The tier most likely to hold it is Clause 6,
whose content this document could not enumerate beyond the two items above. If a later revision
sources the seventeenth item, it is added with its own reason and nothing else in the tables changes.

## Where Each Artifact Lives

| Artifact | Path | Written by |
|---|---|---|
| Test plan | `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md` | `tester` (`unit-tester` / `playwright-tester`), per slice |
| Test case specification | *none — generated from the traceability chain* | `.ai/reference/traceability.md` + `validate-traceability.py` |
| Test completion report | `docs/test-reports/{release}-test-completion-report.md` | release process |
| Test incident report | `docs/test-reports/incidents/{TIR-id}.md` | whoever observed the failure |

Defaults, not hard rules: a project that tracks defects in an external system may record the tracker
key in an incident's References field instead of keeping the file, and may place reports elsewhere —
but the four items must each resolve to something an auditor can open.

## What This Does Not Do

- **It does not assert conformity with 29119-3, at any conformance level.** It maps this template's
  artifacts to the standard's clauses and records what is omitted. That is all it does.
- **It does not reproduce the standard's annexes.** Annex A (conformance levels), Annex E (test plan
  outline), Annex I, Annex G, Annex R and Annex S (the 829 mapping) are named where relevant and not
  copied. Where a section's required detail is not reproduced, the document says so rather than
  filling it in.
- **It does not define quality vocabulary.** Characteristics, measures and the severity taxonomy come
  from `.ai/reference/quality-model.md`, and the measurement boundary (25023, 25040) is recorded
  there.
- **It does not measure coverage.** As `.ai/reference/traceability.md` states, a resolving reference
  proves someone asserted a link; it does not prove the test verifies the criterion.
- **It does not require a document per test run.** The plan is per slice, the completion report is
  per release, and an incident report exists only where something failed and was not fixed in place.
- **It does not make the omitted items wrong to adopt.** If a project acquires a consumer for one —
  an auditor asking for readiness reports, a manual test team needing written procedures — the item
  is added with a template, and this table is updated in the same change.

## Quick Checklist

- [ ] The slice has a test plan at `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`, derived from
      its `blueprint.json`, with entry and exit criteria
- [ ] Every criterion in the slice's `source.acceptance_criteria` is named in that plan
- [ ] No separate test case specification was authored — the `AC-` ↔ scenario ↔ test chain is it
- [ ] Failures that outlive their change have an incident report with reproduce steps, expected vs
      actual, a severity from `quality-model.md`, and a status from the lifecycle
- [ ] Each release has a completion report whose Test measures quote the run's numbers and whose
      Residual risks name what stayed unverified
- [ ] No quality characteristic or metric was invented here — `.ai/reference/quality-model.md` is
      cited instead
- [ ] No certification or conformity claim was made from any of these artifacts
