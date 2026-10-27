#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates the traceability chain: story -> criterion -> scenario -> slice -> test.

The convention and the severities implemented here are defined in
`.ai/reference/traceability.md`, which is authoritative. The split, in one line: an identifier
that does not resolve is a broken reference (FAIL); a criterion that is not fully covered is an
incomplete trace (WARN).

    T1 FAIL  duplicate US-/UC-/AC- identifier within a bounded context
    T2 FAIL  every @AC- tag in a .feature file resolves to a declared criterion
    T3 FAIL  every identifier a blueprint's source names resolves, and the source names a
             story or a use case at all
    T4 FAIL  every [TestCategory("AC-...")] resolves to a declared criterion
    T5 WARN  every declared criterion has at least one tagged scenario
    T6 WARN  every declared criterion has at least one slice and one test-side reference
    T7 WARN  a story or use-case document with no identifiers is legacy -- one summary warning

`docs/bounded-contexts/`, `docs/slices/` and the test sources belong to downstream projects:
this template ships none of them, so here the validator finds nothing to check and exits 0.

Exit code: 0 when no FAIL was found (warnings do not fail the build), 1 otherwise.
"""

import json
import os
import re
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Optional, Sequence, Set, Tuple

# Fix encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')


def get_template_root() -> Path:
    """Get the repository root (this script lives at `<root>/.ai/tests/`)."""
    return Path(__file__).resolve().parent.parent.parent


# --------------------------------------------------------------------------------------
# Identifier grammar -- `.ai/reference/traceability.md` -> "The Format"
#
# Identifiers are matched literally and case-sensitively and must be complete tokens:
# `ac-budget-014.2` is a typo, `AC-Budget-14` is malformed (`{NNN}` is exactly three digits)
# and `AC-Budget-014.2.1` is not a longer form of anything. The lookarounds are what make
# that literal: without the trailing `(?![\w.-])`, `AC-Budget-014.2.1` would silently match
# the prefix `AC-Budget-014.2` and the check would pass on an identifier nobody declared.
# --------------------------------------------------------------------------------------

_BC = r'[A-Z][A-Za-z0-9]*'   # one PascalCase token: `Budget`, `HouseholdPlanning`
_NNN = r'\d{3}'              # exactly three digits, zero-padded

_PARENT_SCAN = re.compile(rf'(?<![\w.-])(US|UC)-{_BC}-{_NNN}(?![\w-])(?!\.\d)')
_CHILD_SCAN = re.compile(rf'(?<![\w.-])(AC|AF)-{_BC}-{_NNN}\.\d+(?![\w.-])')
_PARENT_FULL = re.compile(rf'(?:US|UC)-{_BC}-{_NNN}')
_AC_FULL = re.compile(rf'AC-{_BC}-{_NNN}\.\d+')

# `{n}` is unpadded (`traceability.md` -> "The Format"): the child number's first digit is not
# a zero. The match above stays tolerant on purpose -- tightening it to `[1-9]\d*` would drop
# the *declaration* of a padded criterion, turning a formatting slip into a coverage hole. A
# padded number therefore resolves, and is reported as an advisory warning instead. Only a
# leading zero is flagged: `AC-Budget-014.10` is the tenth criterion, unpadded and correct.
_PADDED_CHILD = re.compile(r'\.0\d')


def is_parent_id(value: str) -> bool:
    """True for a complete `US-{BC}-{NNN}` / `UC-{BC}-{NNN}` identifier."""
    return _PARENT_FULL.fullmatch(value) is not None


def is_ac_id(value: str) -> bool:
    """True for a complete `AC-{BC}-{NNN}.{n}` identifier."""
    return _AC_FULL.fullmatch(value) is not None


def parent_story_of(ac_id: str) -> str:
    """`AC-Budget-014.2` -> `US-Budget-014` (the criterion's parent number, same context)."""
    return 'US-' + ac_id[3:].split('.', 1)[0]


def unpadded(identifier: str) -> str:
    """`AC-Budget-014.02` -> `AC-Budget-014.2`: the form the convention defines, for the hint."""
    return re.sub(r'\.0+(\d)', r'.\1', identifier)


# --------------------------------------------------------------------------------------
# Line-shape patterns
# --------------------------------------------------------------------------------------

_FENCE = re.compile(r'^\s*(?:```|~~~)')
_HEADING = re.compile(r'^(#{1,6})\s+(.*)$')
_CHECKBOX = re.compile(r'^\s*[-*+]\s+\[[ xX]\]\s+(.*)$')
_SCENARIO = re.compile(r'^\s*Scenario(?:\s+Outline)?\s*:', re.IGNORECASE)
_RETIRED_HEADING = re.compile(r'^#{1,6}\s+Retired IDs\s*$', re.IGNORECASE)
# `- {ID}: retired {yyyy-mm-dd} -- {reason}` (`traceability.md` -> "Deletion")
_RETIRED_LINE = re.compile(
    rf'^\s*[-*+]\s+((?:(?:US|UC)-{_BC}-{_NNN})|(?:(?:AC|AF)-{_BC}-{_NNN}\.\d+))\s*:\s*retired\b'
)
_TESTCATEGORY = re.compile(r'\[TestCategory\s*\(\s*"([^"]*)"\s*\)\]')

# The working shapes the skills emit (`user-story`, `usecase-specification`), used only to
# count units in a document that carries no identifiers (T7).
_STORY_MARKER = '**as a**'
_USE_CASE_MARKER = '**actor:**'

# A story document's *stem*. A bare `stor` substring would sweep in `history.md` and
# `storage-notes.md` and warn about stories neither of them holds; the stem has to be
# story-shaped (`user-stories`, `user_stories`, `stories`, `userstories`).
_STORY_STEM = re.compile(r'(^|[-_])stor(y|ies)|user[-_]?stor')

# Directories that never hold traceability artifacts (or are already covered elsewhere).
_SKIP_DIRS = {
    '.git', '.vs', '.idea', 'bin', 'obj', 'node_modules', 'TestResults', '__pycache__',
    '.ai', '.claude', '.pi', '.github',
}

# A path is "design-time" when it lives under `docs/`; everything else is an executable copy.
_DESIGN_ROOT = 'docs'

FAIL = 'FAIL'
WARN = 'WARN'


# --------------------------------------------------------------------------------------
# Collected data
# --------------------------------------------------------------------------------------

@dataclass(frozen=True)
class Decl:
    """A declaration: a heading (US-/UC-) or a criterion checkbox line (AC-)."""
    id: str
    path: str
    line: int


@dataclass(frozen=True)
class Ref:
    """A reference to an identifier, from a scenario tag, a blueprint or a test attribute."""
    id: str
    check: str          # 'T2' | 'T3' | 'T4'
    vehicle: str        # human-readable description of where the reference sits
    path: str
    line: int           # 0 when the vehicle has no meaningful line


@dataclass
class DocReport:
    """A story/use-case document, counted for T7 legacy/mixed detection."""
    path: str
    noun: str           # 'stories' | 'use cases' | 'stories/use cases'
    labelled: int
    total: int


class Scan:
    """Everything the artifact walk collects, before any check runs."""

    def __init__(self) -> None:
        self.declarations: Dict[str, List[Decl]] = {}
        self.refs: List[Ref] = []
        self.retired: Set[str] = set()
        self.scenario_ids: Set[str] = set()             # distinct tagged scenarios, T5
        self.executable_scenario_ids: Set[str] = set()  # tagged executable scenarios, T6
        self.tested_ids: Set[str] = set()               # T6 test side
        self.sliced_ids: Set[str] = set()               # T6 slice side
        self.story_traced: List[Tuple[Set[str], str]] = []  # legacy blueprint story traces
        self.malformed_ids: List[Tuple[str, str, str, int]] = []  # (check, value, path, line)
        self.free_text_blueprints: List[Tuple[str, int]] = []
        self.structural: List[Tuple[str, str]] = []     # blueprint integrity failures
        self.documents: List[DocReport] = []
        self.doc_count = 0
        self.blueprint_count = 0
        self.feature_count = 0
        self.cs_count = 0

    def declare(self, identifier: str, path: str, line: int) -> None:
        self.declarations.setdefault(identifier, []).append(Decl(identifier, path, line))

    def criteria(self) -> List[str]:
        """Declared acceptance criteria, sorted."""
        return sorted(i for i in self.declarations if i.startswith('AC-'))

    def stories_and_use_cases(self) -> List[str]:
        return sorted(i for i in self.declarations if i.startswith(('US-', 'UC-')))


# --------------------------------------------------------------------------------------
# Text helpers
# --------------------------------------------------------------------------------------

def read_text(path: Path) -> Optional[str]:
    try:
        return path.read_text(encoding='utf-8', errors='replace')
    except OSError:
        return None


def line_of(lines: Sequence[str], needle: str) -> int:
    """1-based line number of the first line containing `needle`, or 0."""
    for index, raw in enumerate(lines, start=1):
        if needle in raw:
            return index
    return 0


def retired_section_range(lines: Sequence[str]) -> Tuple[Optional[int], Optional[int]]:
    """Half-open 0-based line range of the `## Retired IDs` section, if present.

    Retired identifiers are not declarations -- a lingering reference to one must fail
    resolution (`traceability.md` -> "Deletion"), so the section is masked out before the
    document is walked for declarations and scenarios.
    """
    start = None
    for index, raw in enumerate(lines):
        if _RETIRED_HEADING.match(raw):
            start = index
            continue
        if start is not None and _HEADING.match(raw):
            return start, index
    if start is not None:
        return start, len(lines)
    return None, None


def mask_retired_section(lines: Sequence[str]) -> Tuple[List[str], Set[str]]:
    """Return the document with the retired section blanked, plus the retired identifiers.

    Blanking (rather than dropping) keeps every line number valid for reporting.
    """
    start, end = retired_section_range(lines)
    if start is None or end is None:
        return list(lines), set()

    retired: Set[str] = set()
    masked = list(lines)
    for index in range(start, end):
        match = _RETIRED_LINE.match(lines[index])
        if match:
            retired.add(match.group(1))
        masked[index] = ''
    return masked, retired


def tag_tokens(raw: str) -> List[str]:
    """The `@`-tags on a Gherkin tag line, or [] when the line is not one.

    Every whitespace-separated token on the line must be a tag (`@smoke @AC-...`); a line
    that merely starts with `@` mid-sentence is not a tag line.
    """
    stripped = raw.strip()
    if not stripped.startswith('@'):
        return []
    stripped = stripped.split(' #', 1)[0]  # tolerate a trailing comment
    tokens = stripped.split()
    if not tokens or not all(token.startswith('@') for token in tokens):
        return []
    return [token[1:] for token in tokens]


def iter_scenario_tag_runs(lines: Sequence[str]) -> List[Tuple[int, int, List[str]]]:
    """Yield (first_tag_index, scenario_index, tags) for tag runs attached to a scenario.

    A tag run is one or more consecutive tag lines; blank lines between the run and the
    `Scenario:`/`Scenario Outline:` line are tolerated. Tags that are not attached to a
    scenario are still checked for resolution (T2) but do not count as scenarios (T5).
    """
    runs: List[Tuple[int, int, List[str]]] = []
    index = 0
    total = len(lines)
    while index < total:
        tags = tag_tokens(lines[index])
        if not tags:
            index += 1
            continue

        first = index
        collected = list(tags)
        last = index
        while last + 1 < total:
            more = tag_tokens(lines[last + 1])
            if not more:
                break
            last += 1
            collected.extend(more)

        probe = last + 1
        while probe < total and not lines[probe].strip():
            probe += 1
        if probe < total and _SCENARIO.match(lines[probe]):
            runs.append((first, probe, collected))
        index = last + 1
    return runs


# --------------------------------------------------------------------------------------
# Artifact scanners
# --------------------------------------------------------------------------------------

def document_kind(stem_lower: str) -> Tuple[bool, bool]:
    """(is_story_document, is_use_case_document) from a file stem.

    T7 applies to the two documents the skills own -- the story and the use-case document --
    and to no other file in the context (`traceability.md` -> "Enforcement", "Legacy Mode").
    Any other markdown file there (glossary, history, README, ubiquitous language) still
    declares identifiers, but its incompleteness is not a traceability finding.
    """
    is_story = _STORY_STEM.search(stem_lower) is not None
    is_use_case = (
        'use-case' in stem_lower or 'use_case' in stem_lower or 'usecase' in stem_lower
    )
    return is_story, is_use_case


def scan_markdown_document(path: Path, rel: str, scan: Scan) -> None:
    """Declarations, scenario tags and unit counts from one bounded-context document."""
    text = read_text(path)
    if text is None:
        return

    lines, retired = mask_retired_section(text.splitlines())
    scan.retired |= retired

    is_story, is_use_case = document_kind(Path(rel).stem.lower())
    story_labelled = use_case_labelled = 0
    story_units = use_case_units = 0
    headings = 0
    in_fence = False

    for index, raw in enumerate(lines):
        if _FENCE.match(raw):
            in_fence = not in_fence
            continue
        if in_fence:
            continue  # format examples live in fenced blocks; the Gherkin scan below ignores fences

        heading = _HEADING.match(raw)
        if heading:
            if len(heading.group(1)) >= 3:
                headings += 1
            for match in _PARENT_SCAN.finditer(heading.group(2)):
                identifier = match.group(0)
                if identifier.startswith('US-'):
                    story_labelled += 1
                else:
                    use_case_labelled += 1
                scan.declare(identifier, rel, index + 1)
        elif _CHECKBOX.match(raw):
            # The criterion's vehicle is the checkbox line (`traceability.md` -> "Where Each
            # ID Appears"); a bare list item mentioning an identifier is a reference, not a
            # declaration, and is deliberately not counted here.
            for match in _CHILD_SCAN.finditer(raw):
                if match.group(1) == 'AC':
                    scan.declare(match.group(0), rel, index + 1)

        lowered = raw.strip().lower()
        if lowered.startswith(_STORY_MARKER):
            story_units += 1
        elif lowered.startswith(_USE_CASE_MARKER):
            use_case_units += 1

    # Scenario tags: fenced or not -- in the emitted shape the Gherkin block is a fence.
    for _, _, tags in iter_scenario_tag_runs(lines):
        for tag in tags:
            if is_ac_id(tag):
                scan.scenario_ids.add(tag)

    if is_story or is_use_case:
        labelled = (story_labelled if is_story else 0) + (use_case_labelled if is_use_case else 0)
        # The emitted shape carries one `**As a**` / `**Actor:**` line per unit; fall back to
        # level-3+ headings for a document written in an older format.
        units = (story_units if is_story else 0) + (use_case_units if is_use_case else 0)
        total = units or headings
        nouns = []
        if is_story:
            nouns.append('stories')
        if is_use_case:
            nouns.append('use cases')
        scan.documents.append(
            DocReport(rel, ' / '.join(nouns), labelled, total)
        )


def scan_feature_file(path: Path, rel: str, scan: Scan, executable: bool) -> None:
    """Scenario tags from one `.feature` file (T2; the executable copy also feeds T5/T6)."""
    text = read_text(path)
    if text is None:
        return

    lines = text.splitlines()
    for _, _, tags in iter_scenario_tag_runs(lines):
        for tag in tags:
            if is_ac_id(tag):
                scan.scenario_ids.add(tag)
                if executable:
                    scan.executable_scenario_ids.add(tag)

    for index, raw in enumerate(lines, start=1):
        for tag in tag_tokens(raw):
            if not tag.startswith('AC-'):
                continue  # `@AF-` links an alternate flow: outside T1-T7 by design
            if is_ac_id(tag):
                scan.refs.append(Ref(tag, 'T2', '@' + tag, rel, index))
            else:
                scan.malformed_ids.append(('T2', '@' + tag, rel, index))


def normalise_id_list(plural, singular) -> List[str]:
    """Read an identifier array, tolerating the legacy singular string key.

    `traceability.md` (blueprint tolerance): blueprints written before this convention may
    carry singular `use_case` / `user_story` string keys; read those as one-element arrays.
    A bare string under the plural key is read the same way.
    """
    values: List[str] = []
    for candidate in (plural, singular):
        if isinstance(candidate, str):
            values.append(candidate)
        elif isinstance(candidate, list):
            values.extend(v for v in candidate if isinstance(v, str))
    return values


def scan_blueprint(path: Path, rel: str, scan: Scan) -> None:
    """`source` identifiers from one slice blueprint (T3; also feeds the T6 slice side)."""
    text = read_text(path)
    if text is None:
        return

    try:
        data = json.loads(text)
    except json.JSONDecodeError as exc:
        scan.structural.append((rel, f'is not valid JSON ({exc.msg} at line {exc.lineno})'))
        return

    if not isinstance(data, dict):
        scan.structural.append((rel, 'does not hold a JSON object'))
        return

    lines = text.splitlines()
    slice_name = data.get('slice')
    label = slice_name if isinstance(slice_name, str) and slice_name else rel

    raw_source = data.get('source')
    if raw_source is None:
        # No `source` key at all is the pre-convention blueprint shape, which the skills read
        # as a legacy slice (`solution-generator` -> step 7, "a slice whose `source` is absent
        # or unlabelled is in legacy mode"). It names no story, so there is no reference to
        # resolve and nothing to fail.
        source = {}
    elif isinstance(raw_source, dict):
        source = raw_source
    else:
        scan.structural.append((rel, 'holds a `source` that is not a JSON object'))
        return
    has_source = isinstance(raw_source, dict)

    user_stories = normalise_id_list(source.get('user_stories'), source.get('user_story'))
    use_cases = normalise_id_list(source.get('use_cases'), source.get('use_case'))

    # `source` is a trace, not a free-text field: every US-/UC- identifier it names must
    # resolve (T3), and a `source` that names neither key is untraceable by construction
    # (`traceability.md` -> "Where Each ID Appears", "Enforcement"). The three-way split is
    # the same one the criteria values get below, and it is what keeps this quiet on an
    # un-migrated blueprint: a well-formed identifier is a reference that must resolve, a
    # value carrying the prefix but not the shape is a near-miss (advisory), and anything
    # else is legacy free text and is not reported at all.
    story_ids: List[str] = []
    for key, values in (('user_stories', user_stories), ('use_cases', use_cases)):
        for value in values:
            cleaned = value.strip()
            if is_parent_id(cleaned):
                scan.refs.append(
                    Ref(cleaned, 'T3', f'source.{key} value {cleaned}', rel,
                        line_of(lines, cleaned))
                )
                if cleaned.startswith('US-'):
                    story_ids.append(cleaned)
            elif cleaned.startswith(('US-', 'UC-')):
                scan.malformed_ids.append(('T3', cleaned, rel, line_of(lines, cleaned)))

    if has_source and not user_stories and not use_cases:
        scan.structural.append((
            rel,
            'names neither `use_cases` nor `user_stories` in `source`; a slice that names '
            'neither is untraceable by construction',
        ))

    criteria_values = normalise_id_list(source.get('acceptance_criteria'), None)
    well_formed = 0
    free_text = 0
    for value in criteria_values:
        cleaned = value.strip()
        if is_ac_id(cleaned):
            well_formed += 1
            scan.sliced_ids.add(cleaned)
            scan.refs.append(
                Ref(cleaned, 'T3', f'source.acceptance_criteria value {cleaned}', rel,
                    line_of(lines, cleaned))
            )
        elif cleaned.startswith('AC-'):
            scan.malformed_ids.append(('T3', cleaned, rel, line_of(lines, cleaned)))
        else:
            free_text += 1

    if free_text:
        scan.free_text_blueprints.append((rel, free_text))

    # A blueprint that names no acceptance criterion traces at story granularity only (the
    # pre-convention shape, or a slice carved before its criteria were labelled). Its story
    # reference is the slice's link for T6, so un-migrated blueprints do not emit one
    # spurious "no slice" warning per criterion of the stories they serve.
    if not well_formed and story_ids:
        scan.story_traced.append((set(story_ids), label))


def scan_csharp_source(path: Path, rel: str, scan: Scan) -> None:
    """`[TestCategory("AC-...")]` from one C# file (T4).

    A test method with no traceability category is simply outside the trace -- legitimate for
    guard-clause, logging and infrastructure tests -- so nothing is reported for it.
    """
    text = read_text(path)
    if text is None:
        return

    for index, raw in enumerate(text.splitlines(), start=1):
        if raw.lstrip().startswith('//'):
            continue  # a commented-out attribute is not a reference
        for match in _TESTCATEGORY.finditer(raw):
            value = match.group(1).strip()
            if is_ac_id(value):
                scan.refs.append(
                    Ref(value, 'T4', f'[TestCategory("{value}")]', rel, index)
                )
                scan.tested_ids.add(value)
            elif value.startswith('AC-'):
                scan.malformed_ids.append(('T4', value, rel, index))


# --------------------------------------------------------------------------------------
# Checks
# --------------------------------------------------------------------------------------

Finding = Tuple[str, str]  # (severity, text)


def run_checks(scan: Scan) -> Dict[str, List[Finding]]:
    """Run T1-T7. FAIL means a reference is broken; WARN means the trace is incomplete."""
    results: Dict[str, List[Finding]] = {f'T{n}': [] for n in range(1, 8)}

    # -- T1: duplicate declarations -----------------------------------------------------
    for identifier in sorted(scan.declarations):
        decls = scan.declarations[identifier]
        if len(decls) > 1:
            places = ', '.join(f'{d.path}:{d.line}' for d in decls)
            results['T1'].append(
                (FAIL, f'{identifier} is declared {len(decls)} times -- {places}')
            )

    # -- T2/T3/T4: references must resolve ----------------------------------------------
    for ref in scan.refs:
        if ref.id in scan.declarations:
            continue
        where = f'{ref.path}:{ref.line}' if ref.line else ref.path
        # An `AC-` reference names a declared criterion; a blueprint's `US-`/`UC-` reference
        # names a declared story or use case. Name what was not found.
        noun = 'criterion' if ref.id.startswith('AC-') else 'identifier'
        hint = ' -- the identifier is retired: re-point or delete the reference' \
            if ref.id in scan.retired else ''
        results[ref.check].append(
            (FAIL, f'{where} -- {ref.vehicle} resolves to no declared {noun}{hint}')
        )

    for rel, message in scan.structural:
        results['T3'].append((FAIL, f'{rel} {message}'))

    # Near-miss typos are advisory: they are outside the trace, not broken references.
    for check, value, path, line in scan.malformed_ids:
        where = f'{path}:{line}' if line else path
        results[check].append((
            WARN,
            f'{where} -- {value} is not a well-formed identifier; it stays outside the trace',
        ))

    # A padded child number resolves -- the tolerant match keeps it in the trace, so a
    # formatting slip never becomes a coverage hole -- but it is a shape the convention does
    # not define (`{n}` is unpadded), and left unreported a project drifts into it
    # permanently. Advisory, in the same voice as the near-miss path above; the declaration
    # is reported once and each reference that carries it once.
    for identifier in sorted(scan.declarations):
        if not _PADDED_CHILD.search(identifier):
            continue
        for decl in scan.declarations[identifier]:
            results['T1'].append((
                WARN,
                f'{decl.path}:{decl.line} -- {identifier} carries a padded child number; '
                f'`{{n}}` is unpadded ({unpadded(identifier)})',
            ))
    for ref in scan.refs:
        if not _PADDED_CHILD.search(ref.id):
            continue
        where = f'{ref.path}:{ref.line}' if ref.line else ref.path
        results[ref.check].append((
            WARN,
            f'{where} -- {ref.id} carries a padded child number; `{{n}}` is unpadded '
            f'({unpadded(ref.id)})',
        ))

    for rel, count in scan.free_text_blueprints:
        results['T3'].append(
            (WARN, f'{rel} -- {count} value(s) in source.acceptance_criteria are not AC- '
                   f'identifiers (legacy free text?)')
        )

    # -- T5: every declared criterion has at least one tagged scenario -------------------
    # Counted per criterion, not per occurrence: the same tag appears in the story document
    # and in the slice's `.feature` file, and counting both would double every scenario.
    for criterion in scan.criteria():
        if criterion not in scan.scenario_ids:
            decl = scan.declarations[criterion][0]
            results['T5'].append(
                (WARN, f'{criterion} ({decl.path}:{decl.line}) -- no scenario tagged @{criterion}')
            )

    # -- T6: every declared criterion is sliced and tested ------------------------------
    sliced = set(scan.sliced_ids)
    for story_ids, _label in scan.story_traced:
        for criterion in scan.criteria():
            if parent_story_of(criterion) in story_ids:
                sliced.add(criterion)
    tested = scan.tested_ids | scan.executable_scenario_ids

    for criterion in scan.criteria():
        missing = []
        if criterion not in sliced:
            missing.append('no slice')
        if criterion not in tested:
            missing.append('no test')
        if missing:
            decl = scan.declarations[criterion][0]
            results['T6'].append(
                (WARN, f'{criterion} ({decl.path}:{decl.line}) -- ' + ' and '.join(missing))
            )

    # -- T7: legacy and mixed documents -------------------------------------------------
    for report in scan.documents:
        if report.total <= 0:
            continue
        if report.labelled == 0:
            results['T7'].append(
                (WARN,
                 f'{report.path} -- legacy mode: 0 of {report.total} {report.noun} carry '
                 f'identifiers; criterion-level checks (T5, T6) skipped for this file')
            )
        elif report.labelled < report.total:
            results['T7'].append(
                (WARN,
                 f'{report.path} -- mixed mode: {report.labelled} of {report.total} '
                 f'{report.noun} carry identifiers; unlabelled entries produce no per-item '
                 f'findings (advisory only)')
            )

    return results


# --------------------------------------------------------------------------------------
# Output
# --------------------------------------------------------------------------------------

SECTIONS = [
    ('T1', 'Duplicate identifiers within a bounded context', 'FAIL',
     'no duplicate US-/UC-/AC- identifier'),
    ('T2', 'Scenario tags in .feature files resolve', 'FAIL',
     'every @AC- tag resolves to a declared criterion'),
    ('T3', 'Blueprint source identifiers resolve', 'FAIL',
     'every identifier a blueprint `source` names resolves, and every `source` names a story '
     'or a use case'),
    ('T4', 'Test categories resolve', 'FAIL',
     'every [TestCategory("AC-...")] resolves to a declared criterion'),
    ('T5', 'Declared criteria have scenarios', 'WARN (advisory)',
     'every declared criterion has at least one tagged scenario'),
    ('T6', 'Declared criteria have slices and tests', 'WARN (advisory)',
     'every declared criterion has at least one slice and one test-side reference'),
    ('T7', 'Legacy and mixed documents', 'WARN (advisory)',
     'every story/use-case document carries identifiers'),
]


def print_section(number: int, findings: List[Finding]) -> None:
    key, title, severity, clean = SECTIONS[number]
    print('=' * 60)
    print(f'TEST {number + 1} ({key}): {title} -- {severity}')
    print('=' * 60)
    if not findings:
        print(f'✅ {clean}')
    else:
        for finding_severity, text in findings:
            marker = '❌ FAIL' if finding_severity == FAIL else '⚠️  WARNING:'
            print(f'{marker} {text}')
    print()


def print_empty_summary(scan: Scan) -> bool:
    """Nothing to check: say so plainly, then report a clean run."""
    print('=' * 60)
    print('  SUMMARY')
    print('=' * 60)
    print()
    print(f'Bounded-context documents scanned: {scan.doc_count}')
    print(f'Blueprints scanned: {scan.blueprint_count}')
    print(f'Feature files scanned: {scan.feature_count}')
    print(f'C# sources scanned: {scan.cs_count}')
    print()
    print('Declared identifiers: 0')
    print('References checked: 0')
    print('Failures: 0')
    print('Warnings: 0')
    print()
    print('✅✅✅ ALL TRACEABILITY CHECKS PASSED ✅✅✅')
    return True


def main() -> bool:
    root = get_template_root()
    scan = scan_repository(root)

    print('=' * 60)
    print('  Traceability Validation')
    print('=' * 60)
    print()
    print('Chain: docs/bounded-contexts/ -> docs/slices/ -> .feature files -> test sources')
    print(f'Scanned: {scan.doc_count} bounded-context document(s), '
          f'{scan.blueprint_count} blueprint(s), '
          f'{scan.feature_count} feature file(s), '
          f'{scan.cs_count} C# source file(s)')
    print()

    artifact_count = scan.doc_count + scan.blueprint_count + scan.feature_count + scan.cs_count
    if artifact_count == 0:
        print('  No traceability artifacts found -- docs/bounded-contexts/, docs/slices/ and the')
        print('  test sources hold nothing to check. (Downstream projects own those paths; this')
        print('  template ships none of them.) Nothing to check, nothing failed.')
        print()
        return print_empty_summary(scan)

    if not any((scan.declarations, scan.refs, scan.documents, scan.malformed_ids,
                scan.structural, scan.sliced_ids, scan.story_traced)):
        print(f'  No traceability identifiers found in the {artifact_count} artifact(s) that exist')
        print('  -- nothing to check. Documents without identifiers are in legacy mode (T7) and')
        print('  are advisory only. Nothing to check, nothing failed.')
        print()
        return print_empty_summary(scan)

    print(f'Declared: {len(scan.stories_and_use_cases())} story/use-case identifier(s), '
          f'{len(scan.criteria())} acceptance criterion(s)')
    print(f'References: {len(scan.refs)} traceability reference(s), '
          f'{len(scan.retired)} retired identifier(s)')
    print()

    results = run_checks(scan)

    failures = 0
    warnings = 0
    for number, (key, *_) in enumerate(SECTIONS):
        print_section(number, results[key])
        for severity, _text in results[key]:
            if severity == FAIL:
                failures += 1
            else:
                warnings += 1

    print('=' * 60)
    print('  SUMMARY')
    print('=' * 60)
    print()
    print(f'Bounded-context documents scanned: {scan.doc_count}')
    print(f'Blueprints scanned: {scan.blueprint_count}')
    print(f'Feature files scanned: {scan.feature_count}')
    print(f'C# sources scanned: {scan.cs_count}')
    print()
    print(f'Declared identifiers: {len(scan.stories_and_use_cases())} story/use-case, '
          f'{len(scan.criteria())} criterion')
    print(f'References checked: {len(scan.refs)}')
    print(f'Failures: {failures}')
    print(f'Warnings: {warnings}')
    print()

    if failures:
        print('❌❌❌ SOME TRACEABILITY CHECKS FAILED ❌❌❌')
        return False

    print('✅✅✅ ALL TRACEABILITY CHECKS PASSED ✅✅✅')
    if warnings:
        print(f'   ({warnings} advisory warning(s) -- the trace is incomplete, '
              f'the repository is not)')
    return True


def scan_repository(root: Path) -> Scan:
    """Walk the three artifact roots and collect declarations, references and tallies."""
    scan = Scan()
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = sorted(d for d in dirnames if d not in _SKIP_DIRS)
        current = Path(dirpath)
        rel_parts = current.relative_to(root).parts

        in_bounded_contexts = rel_parts[:2] == (_DESIGN_ROOT, 'bounded-contexts')
        in_slices = rel_parts[:2] == (_DESIGN_ROOT, 'slices')
        executable = not rel_parts or rel_parts[0] != _DESIGN_ROOT

        for name in sorted(filenames):
            path = current / name
            rel = path.relative_to(root).as_posix()
            suffix = path.suffix.lower()
            lower = name.lower()

            if suffix == '.md' and in_bounded_contexts:
                scan.doc_count += 1
                scan_markdown_document(path, rel, scan)
            elif suffix == '.json' and lower == 'blueprint.json' and in_slices:
                scan.blueprint_count += 1
                scan_blueprint(path, rel, scan)
            elif suffix == '.feature':
                scan.feature_count += 1
                scan_feature_file(path, rel, scan, executable)
            elif suffix == '.cs':
                scan.cs_count += 1
                scan_csharp_source(path, rel, scan)

    return scan


if __name__ == '__main__':
    success = main()
    sys.exit(0 if success else 1)
