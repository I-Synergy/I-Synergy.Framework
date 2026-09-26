# Standards Compatibility Manifest

> One table states what this template supplies a project for each published standard, and what stays
> outside a code repository's reach. `verify-config` reads this file to compute a project's
> compatibility view; `.ai/tests/validate-standards.py` reads it to check that a generated view makes
> no claim this file cannot back. **Status is computed from validator exit codes, never authored** —
> which is what makes the view honest by construction rather than by care.

## Purpose and Scope

A team is asked which standards their codebase "covers", and the honest answer is rarely yes or no.
This file gives the three-part answer, one row per standard: an artifact a certification auditor can
sample directly, engineering practice that follows a standard's model without producing an audit
artifact, or something a code repository cannot do at all.

**This is not a certification artifact and nothing here asserts conformity with any standard.**
Certification is issued by an accredited third-party body after auditing an organizational
management system. It cannot be established by a code repository, and the *Remains organizational*
column names what stays outside this repository's reach. The vocabulary exists so that a project can
say something true and specific instead of something impressive and empty.

**Scope.** The rows cover the standards this template's roadmap names. This is not a survey of every
standard a .NET project could meet, and a standard's absence from the table is not a statement about
that standard — it is outside this file's scope.

## The Three Compatibility Values

| Compatibility | Means | Audit value |
|---|---|---|
| **Evidence** | The repository generates an artifact a certification auditor samples directly | Followable to a file |
| **Aligned** | Engineering practice follows the standard's model, but no audit artifact is produced here | Structural, not evidential |
| **Organizational** | Certifiable only at organization level; this repository supplies nothing for it | Named, not claimed |

Three values, no fourth. A row that has nothing to claim says so with `Organizational` and a `—`
artifact — a named absence, not a shrug — and a row that follows a model without filing anything
says `Aligned` rather than stretching `Evidence`.

**Only an `Evidence` row may name a proving validator.** `Aligned` and `Organizational` rows name
none, because a validator exists to make a claim checkable, and those two values are precisely the
claims this repository does not make checkable. A validator on a non-`Evidence` row is a manifest
error, not a bonus.

## Compatibility

Ordered strongest claim first: a reader who stops after row 1 has read the entire evidence claim. The
`Standard` cell is the row key — a generated surface matches against it by its exact text.

| # | Standard | Compatibility | What this repository provides to a project | Implementing artifact | Evidence a project produces | Proving validator | Remains organizational |
|---|---|---|---|---|---|---|---|
| 1 | ISO/IEC/IEEE 29119-3:2021 | `Evidence` | Four of the standard's information items, right-sized: a test plan per vertical slice, derived from the slice blueprint; a test case specification generated as the acceptance-criteria ↔ scenario ↔ test mapping, produced rather than authored and never filed as a second document; a test completion report per release, carrying test measures and residual risks; and a test incident report per failure that outlives its change. The remaining items are omitted with a recorded reason each — the clause and annex for every item, adopted and omitted, are in the implementing document. | `.ai/reference/test-documentation.md`; `.ai/reference/templates/test-plan.md.txt`; `.ai/reference/templates/test-completion-report.md.txt`; `.ai/reference/templates/test-incident-report.md.txt` | `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`; `docs/test-reports/{release}-test-completion-report.md`; `docs/test-reports/incidents/TIR-{yyyy-mm-dd}-{NN}.md` | `.ai/tests/validate-traceability.py` | The organizational test tier: a test policy and organizational test practices. A policy is set by an organization for all its projects, and a repository is one project — what it contributes is the project-level evidence on the left, not the policy. |
| 2 | ISO/IEC 25010:2023 | `Aligned` | Review vocabulary and attribution: the nine product quality characteristics of the 2023 edition, the question a reviewer asks of a change for each, and a three-level severity taxonomy (`Blocker`, `Major`, `Minor`). It names what a review is about; it measures nothing and evaluates nothing. | `.ai/reference/quality-model.md` | — | — | Nothing. 25010 is not a standard a body certifies against — it is an evaluation model. Measurement (ISO/IEC 25023) and evaluation (ISO/IEC 25040) are out of scope in this template and are supplied at no tier; they are named here so that the absence is visible rather than inferred. |
| 3 | ISO/IEC 27001 | `Organizational` | Part of the Annex A technological slice, as engineering practice rather than as a management-system artifact: committed-secret detection over tracked files, and dependency-audit and code-scanning steps published as a sample CI workflow in the `security` skill — a sample to adopt, not a wired-up default. Configuration management is git plus the template's file-ownership model. | `.ai/scripts/hygiene-lint.py`; `.ai/skills/security/SKILL.md` | — | — | The management system itself: ISMS scope, the organizational, people and physical controls of Annex A, risk assessment and treatment, the statement of applicability, internal audit, management review, and the certification audit. A passing hygiene scan is a CI step, not a management-system record. |
| 4 | ISO/IEC 27701 / ISO/IEC 27017 / ISO/IEC 27018 | `Organizational` | Nothing today. The plan's Wave 3 provision — PII classification, redaction and retention as code — is not written, and this repository holds no privacy artifact of any kind. | — | — | — | The privacy programme each of the three certifies against: the controller/processor relationship, records of processing, data protection impact assessments, sub-processor governance, and jurisdictional retention policy. Controls as a cloud service provider are what 27017 and 27018 address, and a repository is not the subject they address. |
| 5 | ISO 9001 | `Organizational` | Nothing. The word *quality* in `.ai/reference/quality-model.md` refers to ISO/IEC 25010 product quality characteristics, not to a quality management system, and that document must not be cited as a QMS artifact. | — | — | — | The entire QMS: quality policy and objectives, the process approach, customer focus, monitoring and measurement, management review, nonconformity and corrective action, internal audit, and the certification audit. |
| 6 | ISO/IEC 15504 / ISO/IEC 33001 | `Organizational` | A process evidence trail, produced as a by-product of doing the work: a plan, a step-by-step progress record and a completion record per task, in `.ai/plans/`, `.ai/progress/` and `.ai/completed/`, under the protocol in the implementing document. | `.ai/reference/task-execution.md` | — | — | The assessment itself: process attributes, capability levels, the rating, and the assessor's judgment. This repository produces a trail; it produces no rating, and nothing here is a capability claim. |
| 7 | IEC 62304 | `Organizational` | Nothing. A bill of materials for third-party components — the SOUP inventory the standard requires — would be the in-repo part if a project needed one, and this template does not produce one. The row exists so that the absence is named rather than discovered in an audit. | — | — | — | The device manufacturer's quality system: software safety classification, the software development plan under the standard, the risk management file, SOUP assessment, and the clinical evaluation the standard assumes. This repository is not a medical device and produces no part of that file set. |
| 8 | ISO/IEC 42001 | `Organizational` | Nothing. The position is worth stating explicitly: this repository is itself an AI-agent development template, so the in-repo half of an AI management system would concern how the generated codebase uses AI — and nothing here governs that. | — | — | — | The AI management system: AI policy, roles and responsibilities, AI impact assessment, data governance for AI systems, and the AI-specific controls. An AIMS is certifiable for an organization's management system, not for a code repository. |

**The fourth adopted test item has no path, and that is the design.** ISO/IEC/IEEE 29119-3's test
case specification is generated from the traceability chain, so row 1's *Evidence a project produces*
cell lists three files and not four — nothing is filed for the fourth, and
`.ai/reference/test-documentation.md` records why a second document would be the copy that goes
stale.

## How This File Is Parsed

`verify-config` and `.ai/tests/validate-standards.py` must read this file the same way, or the
generator and the checker disagree about what a claim is. The contract is the following.

**1. One status table.** The status table is the table whose header row immediately follows the
heading `## Compatibility`. Tables under any other heading in this file are descriptive and carry no
status. **Select it by that heading, not by looking for a table with a `Compatibility` column** — the
vocabulary table above has one of those too, and a parser that searches by column name will find the
wrong table and read three definitions as three claims. Its columns are, in this order and with these
exact header texts:

| Position | Header | Contains |
|---|---|---|
| 1 | `#` | Row number. Display only — never a key, and never carried into a generated surface as an authority. |
| 2 | `Standard` | The row key: the standard's designator, matched case-sensitively as written. |
| 3 | `Compatibility` | Exactly one token: `Evidence`, `Aligned` or `Organizational`. |
| 4 | `What this repository provides to a project` | Free text. |
| 5 | `Implementing artifact` | `—`, or backticked repository-relative paths separated by `; `. |
| 6 | `Evidence a project produces` | `—`, or backticked paths and path patterns separated by `; `. |
| 7 | `Proving validator` | `—`, or exactly one backticked repository-relative path. |
| 8 | `Remains organizational` | Free text. |

**2. Three tokens, three spellings.** `Evidence`, `Aligned`, `Organizational` — capitalised as
written here, never abbreviated, never qualified (`Partially Aligned`, `Evidence-based`), never
pluralised, never substituted (`Full`, `Partial`, `None`, `N/A`, `Yes`, `No`). A cell holds exactly
one of them, wrapped in backticks; the backticks are formatting and are not part of the token.

**3. Status is computed, never authored.** The value a project publishes is derived from the exit
code of the row's proving validator and from whether the named artifacts exist. No surface may carry
a token this file does not carry for that standard, and no surface may carry a value the run does not
support. A hand-edited status reverts at the next regeneration, and the checker fails on it before
that.

**4. A path is a claim; a pattern is not.** A path claim is a backticked token that either starts
with `.` *and* contains a `/` — `.ai/reference/quality-model.md` — or sits in the `Implementing
artifact`, `Evidence a project produces` or `Proving validator` cell of the status table. Every path
claim must resolve. A backticked token that fails both tests is prose, and a checker must not report
it as a missing file: that covers `.txt` as a file extension, `N/A` in rule 2's list of substitutes,
a shell command line in the Validators table, and a display date. A path pattern contains `{…}`
(`{BC}`, `{release}`, `{yyyy-mm-dd}`) and is never checked for existence as a path claim: it names a
file a project will produce, not one this repository ships. The one exception asks a different
question — rule 6's third cause globs such a pattern for an instance a project produced, and only for
an `Evidence` row in a tree that holds a solution file (`*.sln` or `*.slnx`). **One exemption was
opened here, and it has closed:**
the checker named in the Validators table was itself planned rather than present, so until it landed
its path was the only path claim in this file that did not resolve — and the row that names it said
so. The exemption ended the moment the file existed, which is why that row now records `exists`:
every path claim in this file resolves today, and no path may be added under the closed exemption.

**5. Validity rules the checker enforces.**

- Every `Compatibility` cell holds exactly one of the three tokens.
- Every `Standard` cell is unique.
- An `Evidence` row names exactly one proving validator and at least one artifact path or pattern.
- A row whose token is not `Evidence` names no proving validator.
- Every path in `Implementing artifact` resolves.
- The status table is the only table whose header row is exactly the eight headers in rule 1.

**6. The downgrade.** An `Evidence` row whose validator exits non-zero, whose implementing
artifacts include a missing path, or whose `Evidence a project produces` patterns match nothing the
project produced is rendered one step lower — `Aligned` — with the cause in the generated view's
note column, naming what was not found. One step, never to a fourth value, and never silently: the
note is mandatory on a downgraded row. The third cause substitutes `*` for each `{…}` and globs the
pattern against the working tree — `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md` is tested as
`docs/slices/*/*/test-plan.md` — and it is asked only where a `*.sln` or `*.slnx` exists, because the
artifacts belong to a generated project (rule 9). Read a downgraded row as **not evidenced**, which is
what the note says; the generator can verify that an artifact and a passing run are absent, and it
cannot verify that practice follows the model instead.

**7. The generated view.** A generated `COMPLIANCE.md` status table reproduces this file's columns,
in this file's order, and appends one column — `Note`, at position 9. Every cell is copied verbatim
except `Compatibility`, which carries the computed token, and `Note`, which is `—` or the downgrade
reason. The generator authors no prose about a standard: a sentence it invents is a sentence no
validator reads. The README section carries the same computed tokens, grouped, in the layout fixed by
its template.

**8. Provenance, one line, both surfaces.** Each generated surface carries exactly one line of this
shape, on one line, in an HTML comment so it does not render:

```html
<!-- standards-status: source=.ai/reference/standards.md generated={yyyy-mm-dd} revision={short commit or unknown} validators={filename}:{exit},… -->
```

- `source` is always the literal `.ai/reference/standards.md`.
- `generated` is `{yyyy-mm-dd}` in UTC.
- `revision` is `git rev-parse --short HEAD` at generation time, or `unknown`. **Stated gap:** a
  downstream project records no template revision anywhere, so this field carries the revision of the
  tree that was generated, which is what reproduces the run — not the template's revision.
- `validators` lists one `{filename}:{exit}` entry per validator run, deduplicated, in the order
  the rows appear in the table above, comma-separated with no spaces, or `none` when no validator
  ran. `{filename}` is a bare filename resolved inside `.ai/tests/` — the directory both validators
  this file names live in — and an entry naming the script's repository-relative path resolves to the
  same run.
- The checker requires the line, and requires the recorded exit codes to agree with a re-run: it
  re-runs the script each entry names, by that same resolution, and fails on an entry that resolves
  to no run or on a recorded `{exit}` that disagrees with the re-run. That equality is the mechanism:
  a surface cannot outlive the run that supported it.

**9. Applicability — why this is a no-op in the template.** The rows describe what a *generated
solution* provides. `verify-config` generates a view only when a solution is present — at least one
`*.sln` or `*.slnx` in the working tree. This repository is the template: it has no solution file, no
documentation tree and no test projects, so no `Evidence` row can be satisfied and nothing is written
here. The condition is structural rather than a guard, which is what keeps the template's own file
set unchanged by a tool that runs inside it.

## Validators

Two validators are named across this file, and no others. **ISO/IEC 25010's row names none, and none
is planned:** its value is `Aligned`, not `Evidence`, because a review vocabulary is not an artifact
a validator can check for existence. A validator that asserted the rubric was "applied" would turn
that status into a formality, so the row states what it is instead of implying coverage it does not
have.

| Validator | Status | Command | Proves | Does not prove | Exit codes |
|---|---|---|---|---|---|
| `.ai/tests/validate-traceability.py` | exists | `python .ai/tests/validate-traceability.py` | That every identifier reference in the traceability chain resolves — acceptance criteria, `@AC-…` tags, blueprint `source` references and `[TestCategory("AC-…")]` attributes. The generated test case specification *is* this chain, so a passing run is what row 1's `Evidence` rests on. | That a criterion is semantically covered, that a test is correct, or that the test plan, the completion report and the incident reports exist. It resolves references; it judges nothing. | `0` — no broken reference. `1` — at least one broken reference. Advisory warnings do not change the exit code. |
| `.ai/tests/validate-standards.py` | exists | `python .ai/tests/validate-standards.py` | That this file parses under the contract above, that every `Evidence` row's validator passes and its artifacts exist, and that a generated view makes no claim this file does not back. | Anything about the underlying engineering. It checks the honesty of the view, not its subject. | `0` — the view and this file agree. `1` — a claim could not be resolved. |

**No row names the checker as its prover.** `validate-standards.py` guards the claims, and a
validator cannot be the proving validator of the claim it checks — that would be circular. The roles
are separate: a *proving validator* is evidence for a standard; the *checker* guards the manifest.

**The checker is present, and the gap it left is closed.** Until `.ai/tests/validate-standards.py`
existed, the `Evidence` claim in row 1 rested on `validate-traceability.py` alone and nothing verified
that a generated surface agrees with this file. It exists now, so every path claim in this manifest
resolves and a generated surface is checked against the claims above — see rule 4 of the contract for
how the exemption that covered it closed.

**Warnings do not change a status, and that is a stated limit.** `validate-traceability.py` exits `0`
when no reference is broken, even when many criteria lack a test. So `Evidence` in this vocabulary
means *the evidence chain resolves*; it does not mean *the evidence is complete*. Incompleteness is
reported by the validator's own output and recorded in a release's test completion report under
residual risks. It is not hidden — but this file is not where it shows.

## Roadmap — Wave 2 Targets (Not a Status)

The table above states what holds today. This one states where a standard could move, and it is
deliberately a different table: a target is a plan, a value is a claim, and the checker reads values
only from `## Compatibility`. Nothing in this section is a claim about any repository.

| Standard | Wave | Target value after the wave | What would move it | What stays organizational |
|---|---|---|---|---|
| ISO/IEC/IEEE 12207 | Wave 2 | `Aligned` | A lifecycle-process map: the standard's processes mapped onto `.ai/plans/` → `.ai/progress/` → `.ai/completed/`, the `gap-review` skill and the review gates | Nothing — a project-level lifecycle standard has no organizational-only remainder |
| ISO/IEC 27034 | Wave 2 | `Evidence` | A per-solution application-security profile and a control library, produced by a skill the way the traceability chain is | The enterprise application-security programme and the organizational roles the standard's model assumes |
| ISO/IEC 90003 | Wave 2 | `Aligned` | Naming the design inputs and outputs — the `design-interrogation` record and `.ai/plans/` — as the software-lifecycle artifacts the guidance describes | The quality management system that the guidance is guidance for |

**Not scheduled here.** Other Wave 2–3 work named in the plan — ISO/IEC 27001's Annex A technological
slice, the ISO/IEC 27701 family, and ISO/IEC 42001 — has no fixed scope yet. Those rows carry
`Organizational` above, which is what holds today. No target value is claimed for them, because a
target invented to fill a column is the unbacked claim this file exists to prevent.

**A target is not a promise.** Waves move or slip. When one lands, the row's value above moves in the
same change that adds the artifact and the validator backing it.

## The Two Generated Surfaces

| Surface | Template | Owned by | Regenerated |
|---|---|---|---|
| `COMPLIANCE.md` at the project root | `.ai/reference/templates/compliance.md.txt` | the project — it is listed in `PROJECT_OWNED`, so no template upgrade can overwrite it | whole file |
| A `## Standards Compatibility` section in `README.md`, between `<!-- BEGIN standards-status -->` and `<!-- END standards-status -->` | `.ai/reference/templates/standards-section.md.txt` | the project; only the marked block is generated | the marked block only |

Both surfaces open with the same statement of what they are not, and both carry the provenance line.
`verify-config` computes the values, shows the diff for both, and waits for approval before writing —
it is never a silent mutator, and the approval gate is part of the existing contract, not an addition
to it.

## Changing This File

- **Promoting a row to `Evidence` requires the artifact and the validator in the same change.** A
  token that moves without them is a claim nobody can check, and the checker will fail it.
- **Adding a row requires all eight cells.** A `—` is a value; an empty cell is a defect, because a
  parser cannot tell it from a malformed row.
- **Demoting a row requires a sentence in the row saying why.** The downward direction is how this
  file stays true when reality moves; make the reason visible rather than leaving the change
  unexplained in history.
- **Do not add a standard to the roadmap table without a wave and a target value.** An unscheduled
  standard belongs in no table.

## What This Does Not Do

- **It does not certify anything, and it asserts no conformity with any standard.** It is a manifest
  of artifacts and the checks that resolve them.
- **It does not author a status.** Every token a project publishes is computed from a validator run.
- **It does not measure or evaluate quality.** ISO/IEC 25023 and ISO/IEC 25040 are out of scope and
  are named as such rather than omitted.
- **It does not cite any standard's conformance scheme.** Where a standard defines conformance levels
  or item-by-item obligation levels, this repository does not cite them. Every omission recorded here
  is this template's own right-sizing decision — **not** a statement that the standard permits the
  omission.
- **It does not claim any edition except where this repository already states one.** Editions appear
  for ISO/IEC 25010 and ISO/IEC/IEEE 29119-3 because `.ai/reference/quality-model.md` and
  `.ai/reference/test-documentation.md` name them. Other rows use the designator alone; the claim in
  the row does not turn on an edition, and asserting one this repository cannot source would be the
  kind of claim the file exists to avoid.
- **It does not audit an organization.** The `Organizational` column names what a management system
  would hold. Nothing here says any organization does or does not hold it.
- **It does not make the roadmap real.** The Wave 2 table is a plan.
- **It does not replace a legal, privacy or certification review.** A row that resolves to a file is
  a statement about that file, and nothing more.

## Quick Checklist

- [ ] Every `Compatibility` cell holds exactly one token: `Evidence`, `Aligned` or `Organizational`
- [ ] Every `Evidence` row names one proving validator and at least one artifact path or pattern
- [ ] Every non-`Evidence` row names no proving validator
- [ ] Every backticked path in this file resolves; patterns contain `{…}` and are not checked
- [ ] ISO/IEC 25010's row names no validator, and says why — `Aligned`, not `Evidence`
- [ ] The roadmap table's target column is a target, not a value, and no row there is claimed
- [ ] No token, artifact or validator moved in this file without the artifact that backs it
- [ ] No sentence anywhere claims certification or conformity
