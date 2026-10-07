# Standards Compatibility — I-Synergy.Framework

<!-- standards-status: source=.ai/reference/standards.md generated=2026-10-06 revision=a98b3bbf validators=none -->

> **This document is not a certification, and nothing in it asserts conformity with any standard.**
> It records which engineering artifacts this repository produces and how they relate to published
> standards, and it computes each status from a validator run rather than asserting it. Certification
> is issued by an accredited third-party body after auditing an organizational management system. It
> cannot be established by a code repository, and the *Remains organizational* column names what
> stays outside this repository's reach.

## How To Read This Document

| Value | Means |
|---|---|
| **Evidence** | This repository produces an artifact a certification auditor can sample directly — the path is named in the same row. |
| **Aligned** | Engineering practice follows the standard's model, but no audit artifact is produced here. |
| **Organizational** | Certifiable only at organization level; this repository supplies nothing for it. |

- **The status column is computed, not written.** `verify-config` runs each row's proving validator
  and derives the value from its exit code and from whether the artifacts exist. A status edited by
  hand reverts at the next regeneration, and the checker fails on it.
- **`Evidence` means the evidence chain resolves** — it does not mean the evidence is complete. A
  criterion with no test yet leaves the chain resolving and is reported by the validator's own output
  and in the release's test completion report, not here.
- **A row marked `Aligned` with a note** is a row whose artifacts exist but whose validator did not
  pass; the note says what failed. Read it as *not evidenced*, which is what it says.
- **A `—` means none.** It is a stated absence, not an unanswered field.

## Compatibility

| # | Standard | Compatibility | What this repository provides to a project | Implementing artifact | Evidence a project produces | Proving validator | Remains organizational | Note |
|---|---|---|---|---|---|---|---|---|
| 1 | ISO/IEC/IEEE 29119-3:2021 | `Aligned` | Four of the standard's information items, right-sized: a test plan per vertical slice, derived from the slice blueprint; a test case specification generated as the acceptance-criteria ↔ scenario ↔ test mapping, produced rather than authored and never filed as a second document; a test completion report per release, carrying test measures and residual risks; and a test incident report per failure that outlives its change. The remaining items are omitted with a recorded reason each — the clause and annex for every item, adopted and omitted, are in the implementing document. | `.ai/reference/test-documentation.md`; `.ai/reference/templates/test-plan.md.txt`; `.ai/reference/templates/test-completion-report.md.txt`; `.ai/reference/templates/test-incident-report.md.txt` | `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`; `docs/test-reports/{release}-test-completion-report.md`; `docs/test-reports/incidents/TIR-{yyyy-mm-dd}-{NN}.md` | — | The organizational test tier: a test policy and organizational test practices. A policy is set by an organization for all its projects, and a repository is one project — what it contributes is the project-level evidence on the left, not the policy. | — |
| 2 | ISO/IEC 25010:2023 | `Aligned` | Review vocabulary and attribution: the nine product quality characteristics of the 2023 edition, the question a reviewer asks of a change for each, and a three-level severity taxonomy (`Blocker`, `Major`, `Minor`). It names what a review is about; it measures nothing and evaluates nothing. | `.ai/reference/quality-model.md` | — | — | Nothing. 25010 is not a standard a body certifies against — it is an evaluation model. Measurement (ISO/IEC 25023) and evaluation (ISO/IEC 25040) are out of scope in this template and are supplied at no tier; they are named here so that the absence is visible rather than inferred. | — |
| 3 | ISO/IEC 27001 | `Organizational` | Part of the Annex A technological slice, as engineering practice rather than as a management-system artifact: committed-secret detection over tracked files, and dependency-audit and code-scanning steps published as a sample CI workflow in the `security` skill — a sample to adopt, not a wired-up default. Configuration management is git plus the template's file-ownership model. | `.ai/scripts/hygiene-lint.py`; `.ai/skills/security/SKILL.md` | — | — | The management system itself: ISMS scope, the organizational, people and physical controls of Annex A, risk assessment and treatment, the statement of applicability, internal audit, management review, and the certification audit. A passing hygiene scan is a CI step, not a management-system record. | — |
| 4 | ISO/IEC 27701 / ISO/IEC 27017 / ISO/IEC 27018 | `Organizational` | Nothing today. The plan's Wave 3 provision — PII classification, redaction and retention as code — is not written, and this repository holds no privacy artifact of any kind. | — | — | — | The privacy programme each of the three certifies against: the controller/processor relationship, records of processing, data protection impact assessments, sub-processor governance, and jurisdictional retention policy. Controls as a cloud service provider are what 27017 and 27018 address, and a repository is not the subject they address. | — |
| 5 | ISO 9001 | `Organizational` | Nothing. The word *quality* in `.ai/reference/quality-model.md` refers to ISO/IEC 25010 product quality characteristics, not to a quality management system, and that document must not be cited as a QMS artifact. | — | — | — | The entire QMS: quality policy and objectives, the process approach, customer focus, monitoring and measurement, management review, nonconformity and corrective action, internal audit, and the certification audit. | — |
| 6 | ISO/IEC 15504 / ISO/IEC 33001 | `Organizational` | A process evidence trail, produced as a by-product of doing the work: a plan, a step-by-step progress record and a completion record per task, in `.ai/plans/`, `.ai/progress/` and `.ai/completed/`, under the protocol in the implementing document. | `.ai/reference/task-execution.md` | — | — | The assessment itself: process attributes, capability levels, the rating, and the assessor's judgment. This repository produces a trail; it produces no rating, and nothing here is a capability claim. | — |
| 7 | IEC 62304 | `Organizational` | Nothing. A bill of materials for third-party components — the SOUP inventory the standard requires — would be the in-repo part if a project needed one, and this template does not produce one. The row exists so that the absence is named rather than discovered in an audit. | — | — | — | The device manufacturer's quality system: software safety classification, the software development plan under the standard, the risk management file, SOUP assessment, and the clinical evaluation the standard assumes. This repository is not a medical device and produces no part of that file set. | — |
| 8 | ISO/IEC 42001 | `Organizational` | Nothing. The position is worth stating explicitly: this repository is itself an AI-agent development template, so the in-repo half of an AI management system would concern how the generated codebase uses AI — and nothing here governs that. | — | — | — | The AI management system: AI policy, roles and responsibilities, AI impact assessment, data governance for AI systems, and the AI-specific controls. An AIMS is certifiable for an organization's management system, not for a code repository. | — |

## Validators

| Validator | Command | Result at generation | Proves | Does not prove |
|---|---|---|---|---|
| — | — | — | — | — |

## Roadmap — Wave 2 Targets (Not a Status)

**These are targets, not values.** A target is a plan; nothing in this section is a claim about this repository, and no row here may be counted as evidence.

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

## Regenerating This Document

Run `verify-config`. It computes the values from `.ai/reference/standards.md` and the validator runs,
shows the proposed diff, and writes on approval. The short section in `README.md` between
`<!-- BEGIN standards-status -->` and `<!-- END standards-status -->` is regenerated in the same
pass and carries the same values.

Do not edit the status column, the `Note` column or the provenance line by hand. Everything else in
this document is prose from the template and can be extended by the project — but a project that
edits a row's `Compatibility` cell directly is writing a claim no validator supports.

## What This Document Is Not

- **It is not a certification.** No accredited body has audited this repository, and none can certify
  a repository: certification is issued against an organizational management system.
- **It asserts no conformity with any standard**, at any level, including for the rows marked
  `Evidence`.
- **It is not the evidence.** Each row names where the evidence lives; this document is the index to
  it, and an auditor samples the artifact, not the row.
- **It is not a claim about an organization.** The *Remains organizational* column names what a
  management system would have to hold; nothing here says any organization does or does not hold it.
- **It does not cite any standard's conformance scheme.** Where a standard defines conformance levels
  or per-item obligation levels, they are not repeated here, and every omission of an artifact is a
  right-sizing decision of this project — not a statement that the standard permits it. The reasons,
  item by item, are in `.ai/reference/test-documentation.md`.
- **It does not measure or evaluate quality.** ISO/IEC 25023 (measurement) and ISO/IEC 25040
  (evaluation) are out of scope and are named so that the absence is visible.
- **It is generated from an engineering view of this repository.** It is not a legal, privacy or
  contractual review, and it does not replace one.

## Sources

- `.ai/reference/standards.md` — the manifest this document is generated from; authoritative on
  every value here.
- `.ai/reference/traceability.md` — the identifier convention behind the generated test case
  specification.
- `.ai/reference/test-documentation.md` — which test-documentation items are produced, which are
  omitted, and why.
- `.ai/reference/quality-model.md` — the quality vocabulary and severity taxonomy.
