#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates `.ai/reference/standards.md` under the parsing contract it documents, and checks
that a generated compatibility surface makes no claim the manifest cannot back.

The manifest is the specification for this file -- `.ai/reference/standards.md` ->
"How This File Is Parsed". Two points are load-bearing, and both are implemented literally:

  * the status table is selected by its heading anchor (`## Compatibility`), never by looking
    for a table with a `Compatibility` column: the vocabulary table carries one of those too,
    and reading its three definitions as three claims would pass everything;
  * a status is computed from validator exit codes and artifact presence, never authored. An
    `Evidence` row whose proving validator exits non-zero, whose implementing artifacts include
    a missing path, or whose `Evidence a project produces` patterns match nothing the project
    produced, renders one step lower -- `Aligned` -- with a mandatory note. One step, never a
    fourth value. That last test asks about artifacts a *generated project* produces, so it is
    applied only where a solution file (`*.sln` or `*.slnx`) exists; in this template none does
    and the rows describe a project that was never generated here (manifest rule 9).

There is no exemption list here. Contract rule 4 exempts one planned path "until it lands";
the exempted path is this file, so the exemption ends in the change that creates it. A checker
that carried the exemption forward would be checking nothing.

Checks (1-7 fail the build; 8 is advisory, because it guards a prose claim rather than a value):

  1 FAIL  the status table is selected by the `## Compatibility` heading and carries the
          eight exact headers; it is the only table in the file whose header is exactly those
  2 FAIL  every `Compatibility` cell holds exactly one of `Evidence` / `Aligned` /
          `Organizational`, capitalised, backticked, one per cell, no fourth value
  3 FAIL  every `Standard` cell is unique; an `Evidence` row names exactly one proving
          validator and at least one artifact path or pattern; a non-`Evidence` row names
          no validator; no cell is empty and none carries loose text outside its backticks
  4 FAIL  every path claim resolves -- the `Implementing artifact`, `Evidence a project
          produces` and `Proving validator` cells, plus every backticked token anywhere in
          the file that starts with `.` and contains a `/`. `{...}` patterns name a file a
          project will produce and are never existence-checked
  5 FAIL  every proving validator an `Evidence` row names exits 0
  6 FAIL  the computed status of every row equals the token the manifest carries -- a
          downgraded row is a claim the run does not support, and the note is mandatory
  7 FAIL  a generated surface present in the tree agrees with the manifest: one row per row
          in order, the computed token, a mandatory note on a downgraded row, and exactly one
          provenance line whose recorded exit codes agree with a re-run
  8 WARN  the Validators table's declared status agrees with the file on disk

When no generated surface is present, check 7 reports a skip rather than a failure: this
tree ships no solution file, so nothing is generated here (manifest rule 9, "Applicability").
For the same reason, check 6 skips the evidence-pattern test in such a tree: the files an
`Evidence` row names are a generated project's, and this is the template that generates them.

Exit code: 0 when the manifest parses and every claim resolves, 1 otherwise.
"""

import os
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Optional, Sequence, Tuple

# Fix encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')


def get_template_root() -> Path:
    """Get the repository root (this script lives at `<root>/.ai/tests/`)."""
    return Path(__file__).resolve().parent.parent.parent


# --------------------------------------------------------------------------------------
# The contract, as constants -- `.ai/reference/standards.md` -> "How This File Is Parsed"
# --------------------------------------------------------------------------------------

MANIFEST_REL = '.ai/reference/standards.md'
COMPLIANCE_REL = 'COMPLIANCE.md'
README_REL = 'README.md'
BEGIN_MARKER = '<!-- BEGIN standards-status -->'
END_MARKER = '<!-- END standards-status -->'

EVIDENCE = 'Evidence'
ALIGNED = 'Aligned'
ORGANIZATIONAL = 'Organizational'
TOKENS = (EVIDENCE, ALIGNED, ORGANIZATIONAL)

# Rule 2: a cell holds one token or the em dash that means "there is nothing here". The
# dash is a value -- an empty cell is a defect, because a parser cannot tell it from a
# malformed row ("Changing This File").
NONE_CELL = '—'

STATUS_HEADING = 'Compatibility'
VALIDATORS_HEADING = 'Validators'

# Rule 1: the columns of the status table, in this order and with these exact texts. A table
# whose header row differs is not the status table, whatever its columns are called.
STATUS_HEADERS = [
    '#',
    'Standard',
    'Compatibility',
    'What this repository provides to a project',
    'Implementing artifact',
    'Evidence a project produces',
    'Proving validator',
    'Remains organizational',
]

# Rule 7: the generated COMPLIANCE.md reproduces those columns and appends one, at position 9.
NOTE_HEADER = 'Note'

# The cells whose every backticked token is a path claim (contract rule 4).
CLAIM_HEADERS = ('Implementing artifact', 'Evidence a project produces', 'Proving validator')

VALIDATOR_TIMEOUT_SECONDS = 300

FAIL = 'FAIL'
WARN = 'WARN'

Finding = Tuple[str, str]  # (severity, text)

_FENCE = re.compile(r'^\s*(?:```|~~~)')
_HEADING = re.compile(r'^(#{1,6})\s+(.*)$')
_SEPARATOR = re.compile(r'^\|[\s:|-]+\|$')
_BACKTICK = re.compile(r'`([^`]+)`')
_TOKEN_CELL = re.compile(r'^`(Evidence|Aligned|Organizational)`$')
_DASH_LINE = re.compile(r'^-{3,}$')
_PROVENANCE = re.compile(
    r'^\s*<!--\s*standards-status:\s*source=(?P<source>\S+)\s+'
    r'generated=(?P<generated>\S+)\s+revision=(?P<revision>\S+)\s+'
    r'validators=(?P<validators>\S+)\s*-->\s*$'
)
_DATE = re.compile(r'^\d{4}-\d{2}-\d{2}$')


def fail(text: str) -> Finding:
    return (FAIL, text)


def warn(text: str) -> Finding:
    return (WARN, text)


def print_findings(findings: Sequence[Finding]) -> int:
    """Print findings in house style; return the number of failures among them."""
    failures = 0
    for severity, text in findings:
        if severity == FAIL:
            failures += 1
            print(f'❌ FAIL {text}')
        else:
            print(f'⚠️  WARNING: {text}')
    return failures


# --------------------------------------------------------------------------------------
# Reading and parsing
# --------------------------------------------------------------------------------------

@dataclass(frozen=True)
class Table:
    """One contiguous Markdown table: a header row, a separator row, then data rows."""
    headers: List[str]
    rows: List[Tuple[int, List[str]]]   # (0-based line index, cells)
    start: int                          # 0-based index of the header row

    @property
    def line(self) -> int:
        return self.start + 1


@dataclass
class ManifestRow:
    """One row of the status table, with its cells and the tokens they carry."""
    number: str
    standard: str
    token: Optional[str]                # None when the cell is not exactly one of the three
    token_cell: str
    provides: str
    organizational: str
    artifacts: List[str]                # backticked tokens in `Implementing artifact`
    evidence: List[str]                 # backticked tokens in `Evidence a project produces`
    validator_tokens: List[str]         # backticked tokens in `Proving validator`
    artifacts_cell: str
    evidence_cell: str
    validator_cell: str
    line: int

    @property
    def validator(self) -> Optional[str]:
        return self.validator_tokens[0] if len(self.validator_tokens) == 1 else None


def read_lines(path: Path) -> List[str]:
    try:
        return path.read_text(encoding='utf-8', errors='replace').splitlines()
    except OSError:
        return []


def mask_fenced_lines(lines: Sequence[str]) -> List[str]:
    """Blank every line inside a fenced block, fences included.

    The manifest fences the provenance-line example (` ```html `). Left in place, a fence
    desynchronises backtick pairing for the rest of the file -- a three-backtick fence offers
    its first two backticks as a complete opening/closing pair -- which manufactures path
    claims that exist nowhere in the prose. Masking keeps the token scan of rule 4 literal.
    """
    masked: List[str] = []
    in_fence = False
    for raw in lines:
        if _FENCE.match(raw):
            masked.append('')
            in_fence = not in_fence
            continue
        masked.append('' if in_fence else raw)
    return masked


def split_cells(raw: str) -> List[str]:
    """The cells of one table line, without the outer pipes and without padding."""
    return [cell.strip() for cell in raw.strip().strip('|').split('|')]


def find_tables(masked: Sequence[str]) -> List[Table]:
    """Every well-formed table in the document: a pipe row, a separator row, data rows."""
    tables: List[Table] = []
    index = 0
    total = len(masked)
    while index < total:
        if not masked[index].lstrip().startswith('|'):
            index += 1
            continue

        block: List[Tuple[int, List[str]]] = []
        while index < total and masked[index].lstrip().startswith('|'):
            block.append((index, split_cells(masked[index])))
            index += 1

        # A table needs a header row and a separator row; anything else is not a table.
        if len(block) >= 2 and _DASH_LINE.match(
            masked[block[1][0]].strip().strip('|').split('|')[0].strip()
        ):
            tables.append(Table(block[0][1], block[2:], block[0][0]))
    return tables


def table_under_heading(masked: Sequence[str], tables: Sequence[Table],
                        heading: str, level: int) -> Tuple[List[Table], Optional[int]]:
    """The tables that sit in the section opened by a heading of the given level and text."""
    heading_index: Optional[int] = None
    for index, raw in enumerate(masked):
        match = _HEADING.match(raw)
        if match and len(match.group(1)) == level and match.group(2).strip() == heading:
            heading_index = index
            break
    if heading_index is None:
        return [], None

    section_end = len(masked)
    for index in range(heading_index + 1, len(masked)):
        if _HEADING.match(masked[index]):
            section_end = index
            break

    inside = [table for table in tables if heading_index < table.start < section_end]
    return inside, heading_index


def select_status_table(masked: Sequence[str],
                        tables: Sequence[Table]) -> Tuple[Optional[Table], Optional[str]]:
    """Rule 1: the status table is the table in the `## Compatibility` section.

    Selection is by heading anchor, never by column name. The vocabulary table under
    `## The Three Compatibility Values` also carries a `Compatibility` column, and a parser
    that searched by column name would find that one and read three definitions as three
    claims -- which would pass everything.
    """
    inside, heading_index = table_under_heading(masked, tables, STATUS_HEADING, 2)
    if heading_index is None:
        return None, (f'no `## {STATUS_HEADING}` heading in {MANIFEST_REL} -- the status '
                      f'table cannot be selected by its anchor')

    if len(inside) != 1:
        return None, (f'the `## {STATUS_HEADING}` section (line {heading_index + 1}) holds '
                      f'{len(inside)} tables; exactly one carries status')
    return inside[0], None


def line_of_token(masked: Sequence[str], token: str) -> int:
    """1-based line of the first line holding the token, or 0 when it spans a line break."""
    for index, raw in enumerate(masked, start=1):
        if token in raw:
            return index
    return 0


def backticked(text: str) -> List[str]:
    return [token.strip() for token in _BACKTICK.findall(text)]


def is_pattern(token: str) -> bool:
    """A path pattern contains `{...}`: it names a file a project will produce, not one here."""
    return '{' in token and '}' in token


def normalise_cell(cell: str) -> str:
    """The cell without its formatting -- backticks and emphasis are not part of a value."""
    return re.sub(r'[`*]', '', cell).strip()


def parse_rows(table: Table) -> Tuple[List[ManifestRow], List[Finding]]:
    """Read the status table's data rows. Malformed rows are reported, never guessed at."""
    rows: List[ManifestRow] = []
    findings: List[Finding] = []

    for index, cells in table.rows:
        line = index + 1
        if not cells or all(cell == '' for cell in cells):
            continue  # a blank filler row is not a claim
        if len(cells) != len(STATUS_HEADERS):
            findings.append(fail(
                f'{MANIFEST_REL}:{line} -- the row holds {len(cells)} cells where the header '
                f'holds {len(STATUS_HEADERS)}'
            ))
            continue

        record = dict(zip(STATUS_HEADERS, cells))
        token_cell = record['Compatibility']
        match = _TOKEN_CELL.match(token_cell)
        rows.append(ManifestRow(
            number=record['#'],
            standard=record['Standard'],
            token=match.group(1) if match else None,
            token_cell=token_cell,
            provides=record['What this repository provides to a project'],
            organizational=record['Remains organizational'],
            artifacts=backticked(record['Implementing artifact']),
            evidence=backticked(record['Evidence a project produces']),
            validator_tokens=backticked(record['Proving validator']),
            artifacts_cell=record['Implementing artifact'],
            evidence_cell=record['Evidence a project produces'],
            validator_cell=record['Proving validator'],
            line=line,
        ))
    return rows, findings


def loose_text(cell: str) -> str:
    """The text in a claim cell that is not a backticked token and not a separator.

    Rule 1 fixes the shape of the three path cells: `-`, or backticked paths separated by
    `; `. Anything else is a path that escapes the claim scan, so it is reported rather than
    ignored.
    """
    residue = _BACKTICK.sub('', cell)
    residue = residue.replace(';', '')
    return residue.strip()


# --------------------------------------------------------------------------------------
# Running the proving validators
# --------------------------------------------------------------------------------------

def run_validators(root: Path,
                   paths: Sequence[str]) -> Dict[str, Tuple[Optional[int], str]]:
    """Run each validator once, from the repository root.

    The result is the evidence a status is computed from (contract rule 3), so the exit code
    is kept verbatim and nothing about the validator's output is interpreted.
    """
    results: Dict[str, Tuple[Optional[int], str]] = {}
    for rel in paths:
        script = root / rel
        if not script.exists():
            results[rel] = (None, 'the file does not exist')
            continue
        try:
            proc = subprocess.run(
                [sys.executable, str(script)],
                cwd=str(root),
                capture_output=True,
                text=True,
                encoding='utf-8',
                errors='replace',
                timeout=VALIDATOR_TIMEOUT_SECONDS,
            )
        except subprocess.TimeoutExpired:
            results[rel] = (None, f'timed out after {VALIDATOR_TIMEOUT_SECONDS}s')
            continue
        except OSError as exc:
            results[rel] = (None, f'could not be run ({exc})')
            continue
        results[rel] = (proc.returncode, '')
    return results


# --------------------------------------------------------------------------------------
# Applicability and produced evidence (rule 9, rule 6's third cause)
# --------------------------------------------------------------------------------------

_SKIP_DIRS = {'.git', '.vs', '.idea', 'bin', 'obj', 'node_modules', 'TestResults', '__pycache__'}

# `{BC}`, `{Entity}.{Operation}`, `{yyyy-mm-dd}` -- each names a value a project fills in.
_PATTERN_PLACEHOLDER = re.compile(r'\{[^}]*\}')


def has_solution(root: Path) -> bool:
    """Rule 9's applicability condition: a generated solution, not the template.

    The rows describe what a *generated solution* provides. Where no solution file exists -- this
    template repository is the case -- the files an `Evidence` row names belong to a project
    that was never generated here, so the row is not downgraded for artifacts nobody was meant
    to produce. The condition is structural, exactly as the manifest states it.

    A solution file is `*.sln` or its XML successor `*.slnx`; both describe a generated solution
    and both make the rows describe a project that exists in this tree. `*.slnf` is deliberately
    not matched: it is a solution *filter*, naming a subset of a solution rather than one.
    """
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = sorted(d for d in dirnames if d not in _SKIP_DIRS)
        if any(name.lower().endswith(('.sln', '.slnx')) for name in filenames):
            return True
    return False


def evidence_instances(root: Path, tokens: Sequence[str]) -> Tuple[bool, List[str]]:
    """Whether the tree holds an instance of at least one `Evidence a project produces` token.

    Rule 4 stands: a `{...}` pattern "names a file a project will produce" and is never
    existence-checked as a claim about *this* file. What is checked is whether the project
    produced **any** of them. An `Evidence` row whose evidence cell names three files and whose
    tree holds none of them renders `Aligned`, because "an artifact an auditor samples
    directly" then resolves to nothing to sample.

    Returns `(found, unmatched)`. Every error reads as *not found*: an unreadable directory or
    a malformed pattern must never manufacture the stronger claim.
    """
    unmatched: List[str] = []
    found = False
    for token in tokens:
        try:
            if is_pattern(token):
                matches = any(root.glob(_PATTERN_PLACEHOLDER.sub('*', token)))
            else:
                matches = (root / token).exists()
        except (OSError, ValueError):
            matches = False
        if matches:
            found = True
        else:
            unmatched.append(token)
    return found, unmatched


def compute_status(row: ManifestRow, root: Path,
                   results: Dict[str, Tuple[Optional[int], str]],
                   solution_present: bool = True) -> Tuple[str, str]:
    """Rule 6: the computed token and its note. One step down, never a fourth value.

    Three causes, exactly as the contract names them: the row's implementing artifacts include
    a missing path, its proving validator exits non-zero, or the evidence patterns it names for
    a project matched nothing the project produced. All are read as *not evidenced* ("read a
    downgraded row as **not evidenced**"), and the note is mandatory.

    The third cause is tested only where a solution file exists (rule 9): the artifacts belong
    to a generated project, and the template is not one.
    """
    if row.token != EVIDENCE:
        return row.token or '?', NONE_CELL

    causes: List[str] = []
    for path in row.artifacts:
        if is_pattern(path):
            continue
        if not (root / path).exists():
            causes.append(f'implementing artifact {path} is missing')
    if row.validator is None:
        causes.append('no proving validator is named')
    else:
        code, detail = results.get(row.validator, (None, 'it was not run'))
        if code is None:
            causes.append(f'proving validator {row.validator} did not run ({detail})')
        elif code != 0:
            causes.append(f'proving validator {row.validator} exited {code}')
    if row.evidence and solution_present:
        found, unmatched = evidence_instances(root, row.evidence)
        if not found:
            causes.append('nothing in this project matches ' + '; '.join(unmatched))

    if causes:
        return ALIGNED, 'downgraded: ' + '; '.join(causes)
    return EVIDENCE, NONE_CELL


def ordered_validators(rows: Sequence[ManifestRow]) -> List[str]:
    """The distinct proving validators, in the order the rows name them (rule 8's order)."""
    ordered: List[str] = []
    for row in rows:
        if row.token == EVIDENCE and row.validator and row.validator not in ordered:
            ordered.append(row.validator)
    return ordered


# --------------------------------------------------------------------------------------
# The generated surfaces (rules 7 and 8)
# --------------------------------------------------------------------------------------

def provenance_line(lines: Sequence[str]) -> Tuple[Optional[Dict[str, str]], Optional[Finding]]:
    """Parse the one provenance line a generated surface carries (contract rule 8)."""
    found = [raw for raw in lines if _PROVENANCE.match(raw)]
    if not found:
        return None, fail('no provenance line -- `<!-- standards-status: source=... '
                          'generated=... revision=... validators=... -->` is required')
    if len(found) > 1:
        return None, fail(f'{len(found)} provenance lines; exactly one is required')

    match = _PROVENANCE.match(found[0])
    assert match is not None  # guarded by the filter above
    fields = match.groupdict()

    findings: List[Finding] = []
    if fields['source'] != MANIFEST_REL:
        findings.append(fail(f'provenance `source={fields["source"]}` -- it must be the '
                             f'literal `{MANIFEST_REL}`'))
    if not _DATE.match(fields['generated']):
        findings.append(fail(f'provenance `generated={fields["generated"]}` -- expected a '
                             f'`{{yyyy-mm-dd}}` date in UTC'))
    if not fields['revision'] or fields['revision'] == '{short':
        findings.append(fail(f'provenance `revision={fields["revision"]}` -- expected a '
                             f'short commit or `unknown`'))
    if findings:
        return None, findings[0]
    return fields, None


def check_provenance_against_run(fields: Dict[str, str], validators_run: Sequence[str],
                                 results: Dict[str, Tuple[Optional[int], str]]) -> List[Finding]:
    """Rule 8: "the recorded exit codes must agree with a re-run".

    That equality is the mechanism -- a surface cannot outlive the run that supported it.
    `{filename}` is not defined further by the contract, so a recorded name matches either the
    validator's repository-relative path or its basename.
    """
    recorded = fields['validators']
    if recorded == 'none':
        entries: List[Tuple[str, Optional[int]]] = []
    else:
        entries = []
        for entry in recorded.split(','):
            name, _, code = entry.rpartition(':')
            if not name or not code.lstrip('-').isdigit():
                return [fail(f'provenance `validators={recorded}` -- each entry must be '
                             f'`{{filename}}:{{exit}}`, comma-separated with no spaces')]
            entries.append((name, int(code)))

    expected = [(path, results.get(path, (None, ''))[0]) for path in validators_run]
    if len(entries) != len(expected):
        return [fail(f'provenance lists {len(entries)} validator run(s); the re-run has '
                     f'{len(expected)} -- a surface cannot outlive the run that supported it')]

    findings: List[Finding] = []
    for (name, code), (path, rerun) in zip(entries, expected):
        if name not in (path, Path(path).name):
            findings.append(fail(f'provenance names validator `{name}`, which is not `{path}`'))
        elif code != rerun:
            findings.append(fail(f'provenance records `{name}:{code}`; the re-run exited '
                                 f'`{rerun}`'))
    return findings


def check_compliance_surface(lines: Sequence[str], rows: Sequence[ManifestRow],
                             computed: Dict[str, Tuple[str, str]],
                             validators_run: Sequence[str],
                             results: Dict[str, Tuple[Optional[int], str]]) -> List[Finding]:
    """`COMPLIANCE.md`: one row per manifest row, in order, plus the Note column (rule 7)."""
    findings: List[Finding] = []

    fields, provenance_finding = provenance_line(lines)
    if provenance_finding:
        findings.append(provenance_finding)
    elif fields:
        findings.extend(check_provenance_against_run(fields, validators_run, results))

    tables = [table for table in find_tables(mask_fenced_lines(lines))
              if table.headers == STATUS_HEADERS + [NOTE_HEADER]]
    if len(tables) != 1:
        findings.append(fail(f'{COMPLIANCE_REL} -- expected exactly one status table with the '
                             f'manifest columns plus `{NOTE_HEADER}`; found {len(tables)}'))
        return findings

    table = tables[0]
    if len(table.rows) != len(rows):
        findings.append(fail(f'{COMPLIANCE_REL}:{table.line} -- the table holds '
                             f'{len(table.rows)} row(s); the manifest holds {len(rows)}'))
        return findings

    for manifest_row, (index, cells) in zip(rows, table.rows):
        line = index + 1
        if len(cells) != len(STATUS_HEADERS) + 1:
            findings.append(fail(f'{COMPLIANCE_REL}:{line} -- the row holds {len(cells)} cells '
                                 f'where the header holds {len(STATUS_HEADERS) + 1}'))
            continue
        token, note = computed.get(manifest_row.standard, ('?', NONE_CELL))
        if normalise_cell(cells[1]) != manifest_row.standard:
            findings.append(fail(f'{COMPLIANCE_REL}:{line} -- row key '
                                 f'`{normalise_cell(cells[1])}` matches no manifest row'))
        if normalise_cell(cells[2]) != token:
            findings.append(fail(f'{COMPLIANCE_REL}:{line} -- `{manifest_row.standard}` carries '
                                 f'`{normalise_cell(cells[2])}`; the run computes `{token}`'))
        if note == NONE_CELL and normalise_cell(cells[8]) != NONE_CELL:
            findings.append(fail(f'{COMPLIANCE_REL}:{line} -- `{manifest_row.standard}` is not '
                                 f'downgraded; its `{NOTE_HEADER}` cell must be `{NONE_CELL}`'))
        if note != NONE_CELL and normalise_cell(cells[8]) == NONE_CELL:
            findings.append(fail(f'{COMPLIANCE_REL}:{line} -- `{manifest_row.standard}` '
                                 f'downgrades to `{token}` and the note is mandatory'))
    return findings


def check_readme_surface(lines: Sequence[str], rows: Sequence[ManifestRow],
                         computed: Dict[str, Tuple[str, str]],
                         validators_run: Sequence[str],
                         results: Dict[str, Tuple[Optional[int], str]]) -> List[Finding]:
    """The marked README block: markers balanced, same computed tokens, same designators."""
    findings: List[Finding] = []
    begins = [i for i, raw in enumerate(lines) if raw.strip() == BEGIN_MARKER]
    ends = [i for i, raw in enumerate(lines) if raw.strip() == END_MARKER]

    if len(begins) != 1 or len(ends) != 1:
        return [fail(f'{README_REL} -- {len(begins)} `{BEGIN_MARKER}` and {len(ends)} '
                     f'`{END_MARKER}`; exactly one of each is required')]
    if ends[0] < begins[0]:
        return [fail(f'{README_REL} -- the closing marker precedes the opening one '
                     f'(lines {ends[0] + 1} and {begins[0] + 1})')]

    block = lines[begins[0] + 1:ends[0]]
    fields, provenance_finding = provenance_line(block)
    if provenance_finding:
        findings.append(provenance_finding)
    elif fields:
        findings.extend(check_provenance_against_run(fields, validators_run, results))

    tables = [table for table in find_tables(mask_fenced_lines(block))
              if table.headers and table.headers[0] == STATUS_HEADING]
    if len(tables) != 1:
        findings.append(fail(f'{README_REL} -- the marked block holds {len(tables)} status '
                             f'table(s); exactly one is required'))
        return findings

    for index, cells in tables[0].rows:
        if len(cells) < 3:
            findings.append(fail(f'{README_REL}:{index + 1} -- the group row holds {len(cells)} '
                                 f'cells; a group, a meaning and its standards are required'))
            continue
        group = normalise_cell(cells[0])
        if group not in TOKENS:
            findings.append(fail(f'{README_REL}:{index + 1} -- `{group}` is not one of the '
                                 f'three tokens'))
            continue

        listed = [item.strip() for item in cells[2].split(',') if item.strip()]
        if listed == ['*(none)*']:
            listed = []
        expected = [row.standard for row in rows
                    if computed.get(row.standard, ('?', NONE_CELL))[0] == group]
        for designator in listed:
            if designator not in {row.standard for row in rows}:
                findings.append(fail(f'{README_REL}:{index + 1} -- `{designator}` matches no '
                                     f'manifest row'))
            elif designator not in expected:
                findings.append(fail(f'{README_REL}:{index + 1} -- `{designator}` is listed '
                                     f'under `{group}`; the run computes a different token'))
        for designator in expected:
            if designator not in listed:
                findings.append(fail(f'{README_REL}:{index + 1} -- `{designator}` computes to '
                                     f'`{group}` and is missing from the group'))
    return findings


# --------------------------------------------------------------------------------------
# Main
# --------------------------------------------------------------------------------------

def main() -> bool:
    root = get_template_root()
    manifest_path = root / MANIFEST_REL

    print('=' * 60)
    print('  Standards Manifest Validation')
    print('=' * 60)
    print()
    print('Chain: .ai/reference/standards.md -> proving validators -> computed status')
    print('Status is computed from validator exit codes and artifact presence, never authored.')
    print()

    if not manifest_path.exists():
        print(f'❌ FAIL {MANIFEST_REL} does not exist -- there is no manifest to parse and no '
              f'claim to check.')
        print()
        print('=' * 60)
        print('  SUMMARY')
        print('=' * 60)
        print()
        print('❌❌❌ SOME STANDARDS CHECKS FAILED ❌❌❌')
        return False

    raw_lines = read_lines(manifest_path)
    lines = mask_fenced_lines(raw_lines)
    tables = find_tables(lines)

    failures = 0
    warnings = 0

    # --- TEST 1: the status table, selected by its heading anchor ----------------------
    print('=' * 60)
    print('TEST 1: Status table selected by the `## Compatibility` heading')
    print('=' * 60)

    status_table, selection_error = select_status_table(lines, tables)
    findings: List[Finding] = []
    if selection_error:
        findings.append(fail(selection_error))
    else:
        assert status_table is not None
        _, heading_index = table_under_heading(lines, tables, STATUS_HEADING, 2)
        print(f'✅ `## {STATUS_HEADING}` at line {heading_index + 1}; status table header at '
              f'line {status_table.line}, {len(status_table.headers)} columns')
        decoys = [table for table in tables
                  if STATUS_HEADING in table.headers and table.headers != STATUS_HEADERS]
        for decoy in decoys:
            print(f'   Selection is by heading anchor, not by column name: the table at line '
                  f'{decoy.line} also carries a `{STATUS_HEADING}` column '
                  f'({" | ".join(decoy.headers)}) and carries no status.')
        if status_table.headers != STATUS_HEADERS:
            findings.append(fail(
                f'{MANIFEST_REL}:{status_table.line} -- the selected table header is '
                f'`{" | ".join(status_table.headers)}`; the contract fixes '
                f'`{" | ".join(STATUS_HEADERS)}`'
            ))
        identical = [table for table in tables if table.headers == STATUS_HEADERS]
        if len(identical) != 1:
            findings.append(fail(
                f'{len(identical)} tables in {MANIFEST_REL} carry exactly the eight status '
                f'headers; exactly one must (lines {[t.line for t in identical]})'
            ))

    failures += print_findings(findings)
    warnings += sum(1 for severity, _ in findings if severity == WARN)
    print()

    if status_table is None or status_table.headers != STATUS_HEADERS:
        print('=' * 60)
        print('  SUMMARY')
        print('=' * 60)
        print()
        print('❌❌❌ SOME STANDARDS CHECKS FAILED ❌❌❌')
        return False

    rows, row_findings = parse_rows(status_table)

    # --- TEST 2: three tokens, three spellings -----------------------------------------
    print('=' * 60)
    print('TEST 2: Compatibility tokens are exactly Evidence / Aligned / Organizational')
    print('=' * 60)

    findings = list(row_findings)
    for row in rows:
        if row.token is None:
            findings.append(fail(
                f'{MANIFEST_REL}:{row.line} -- `{row.standard}` holds `{row.token_cell}`; the '
                f'cell must hold exactly one of `Evidence` / `Aligned` / `Organizational`, '
                f'capitalised and backticked, one per cell'
            ))
    if not findings:
        print(f'✅ {len(rows)} row(s) carry one token each, from a vocabulary of '
              f'{len(TOKENS)}: {" / ".join(TOKENS)}')
    failures += print_findings(findings)
    print()

    # --- TEST 3: row validity ----------------------------------------------------------
    print('=' * 60)
    print('TEST 3: Row keys, validator placement and Evidence artifacts')
    print('=' * 60)

    findings = []
    seen: Dict[str, int] = {}
    for row in rows:
        if row.standard in seen:
            findings.append(fail(f'{MANIFEST_REL}:{row.line} -- Standard `{row.standard}` is '
                                 f'declared twice (also line {seen[row.standard]}); the row '
                                 f'key must be unique'))
        else:
            seen[row.standard] = row.line

        for header, cell in zip(STATUS_HEADERS, [row.number, row.standard, row.token_cell,
                                                 row.provides, row.artifacts_cell,
                                                 row.evidence_cell, row.validator_cell,
                                                 row.organizational]):
            if cell.strip() == '':
                findings.append(fail(f'{MANIFEST_REL}:{row.line} -- `{header}` is empty; a '
                                     f'`{NONE_CELL}` is a value, an empty cell is a defect'))

        if row.token == EVIDENCE:
            if len(row.validator_tokens) != 1:
                findings.append(fail(
                    f'{MANIFEST_REL}:{row.line} -- `{row.standard}` is `{EVIDENCE}` and names '
                    f'{len(row.validator_tokens)} proving validator(s); exactly one is required'
                ))
            if not row.artifacts and not row.evidence:
                findings.append(fail(
                    f'{MANIFEST_REL}:{row.line} -- `{row.standard}` is `{EVIDENCE}` and names '
                    f'no artifact path or pattern'
                ))
        elif row.validator_tokens:
            findings.append(fail(
                f'{MANIFEST_REL}:{row.line} -- `{row.standard}` is `{row.token}` and names a '
                f'proving validator; only an `{EVIDENCE}` row may name one'
            ))

        for header, cell in zip(CLAIM_HEADERS, [row.artifacts_cell, row.evidence_cell,
                                                row.validator_cell]):
            if normalise_cell(cell) == NONE_CELL:
                continue
            residue = loose_text(cell)
            if residue:
                findings.append(fail(
                    f'{MANIFEST_REL}:{row.line} -- `{header}` carries text outside backticks '
                    f'(`{residue}`); the cell is `{NONE_CELL}` or backticked paths separated '
                    f'by `; `'
                ))

    if not findings:
        print(f'✅ {len(rows)} row(s): keys unique; {sum(1 for r in rows if r.token == EVIDENCE)} '
              f'`{EVIDENCE}` row(s) name one validator and an artifact; the other '
              f'{len(rows) - sum(1 for r in rows if r.token == EVIDENCE)} name none')
    failures += print_findings(findings)
    print()

    # --- TEST 4: path claims resolve ---------------------------------------------------
    print('=' * 60)
    print('TEST 4: Path claims resolve (`{...}` patterns are not existence-checked)')
    print('=' * 60)

    findings = []
    claims: Dict[str, str] = {}          # path -> where the claim sits
    patterns: List[str] = []

    def note_claim(token: str, where: str) -> None:
        if is_pattern(token):
            if token not in patterns:
                patterns.append(token)
            return
        claims.setdefault(token, where)

    for row in rows:
        for header, tokens in ((CLAIM_HEADERS[0], row.artifacts),
                               (CLAIM_HEADERS[1], row.evidence),
                               (CLAIM_HEADERS[2], row.validator_tokens)):
            for token in tokens:
                note_claim(token, f'row {row.number} (`{header}`) at line {row.line}')

    # Rule 4's first branch: a backticked token anywhere in the file that starts with `.`
    # and contains a `/` is a path claim too -- prose is what fails both tests.
    for token in _BACKTICK.findall('\n'.join(lines)):
        token = token.strip()
        if token.startswith('.') and '/' in token and not is_pattern(token):
            location = line_of_token(lines, token)
            note_claim(token, f'{MANIFEST_REL}:{location}' if location else MANIFEST_REL)

    for path in sorted(claims):
        if (root / path).exists():
            print(f'✅ {path}')
        else:
            findings.append(fail(f'{path} does not resolve -- claimed by {claims[path]}'))

    if patterns:
        print(f'✅ {len(patterns)} path pattern(s) contain `{{...}}` and name a file a project '
              f'produces -- never existence-checked')
    failures += print_findings(findings)
    print()

    # --- TEST 5: the proving validators run green --------------------------------------
    print('=' * 60)
    print('TEST 5: Every proving validator an `Evidence` row names exits 0')
    print('=' * 60)

    validators = ordered_validators(rows)
    results: Dict[str, Tuple[Optional[int], str]] = {}
    findings = []
    if not validators:
        print(f'✅ no `{EVIDENCE}` row names a proving validator -- nothing to run')
    else:
        results = run_validators(root, validators)
        for path in validators:
            code, detail = results[path]
            if code == 0:
                print(f'✅ {path} exited 0')
            elif code is None:
                findings.append(fail(f'{path} could not be run -- {detail}'))
            else:
                findings.append(fail(f'{path} exited {code} -- the evidence for the row that '
                                     f'names it does not hold on this run'))
    failures += print_findings(findings)
    print()

    # --- TEST 6: the computed status ---------------------------------------------------
    print('=' * 60)
    print('TEST 6: Computed status -- a downgraded row is a claim the run does not support')
    print('=' * 60)

    computed: Dict[str, Tuple[str, str]] = {}
    findings = []
    downgraded = 0
    solution_present = has_solution(root)
    if solution_present:
        print(f'✅ a solution file (`*.sln` or `*.slnx`) is present -- the rows describe this '
              f'project, so an `{EVIDENCE}` row is tested against the evidence it says the '
              f'project produces')
    else:
        print(f'⚠️  SKIP: no solution file (`*.sln` or `*.slnx`) in the tree. Manifest rule 9 '
              f'applies -- the rows describe a generated solution, so `{EVIDENCE}` rows are not '
              f'tested against artifacts nobody was to produce here; the other two causes still '
              f'are.')
    for row in rows:
        token, note = compute_status(row, root, results, solution_present)
        computed[row.standard] = (token, note)
        if token == row.token:
            print(f'✅ {row.standard} -- {token} (as the manifest declares)')
        else:
            downgraded += 1
            findings.append(fail(
                f'{row.standard} -- declared `{row.token}`, computed `{token}` (mandatory '
                f'note: {note})'
            ))
    if not findings:
        print()
        print(f'✅ {len(rows)} row(s) computed; no row moved, so every claim resolves')
    failures += print_findings(findings)
    print()

    # --- TEST 7: the generated surfaces ------------------------------------------------
    print('=' * 60)
    print('TEST 7: Generated surfaces agree with the manifest')
    print('=' * 60)

    findings = []
    compliance_path = root / COMPLIANCE_REL
    if compliance_path.exists():
        print(f'✅ {COMPLIANCE_REL} present -- checking it against the manifest')
        findings.extend(check_compliance_surface(read_lines(compliance_path), rows, computed,
                                                 validators, results))
    else:
        print(f'⚠️  SKIP: {COMPLIANCE_REL} is not present. It is generated into a project '
              f'that has a solution file; this tree has none (manifest rule 9), so there is '
              f'nothing generated to check.')

    readme_path = root / README_REL
    readme_lines = read_lines(readme_path) if readme_path.exists() else []
    if any(raw.strip() == BEGIN_MARKER for raw in readme_lines):
        print(f'✅ {README_REL} carries the standards-status markers -- checking the block')
        findings.extend(check_readme_surface(readme_lines, rows, computed, validators,
                                             results))
    else:
        print(f'⚠️  SKIP: {README_REL} carries no `{BEGIN_MARKER}` marker; the section is '
              f'written only into a generated project, and this tree is the template.')

    failures += print_findings(findings)
    print()

    # --- TEST 8: the Validators table, advisory ----------------------------------------
    print('=' * 60)
    print('TEST 8 (advisory): The Validators table against the file on disk')
    print('=' * 60)

    findings = []
    validator_tables, _ = table_under_heading(lines, tables, VALIDATORS_HEADING, 2)
    if len(validator_tables) != 1:
        findings.append(warn(f'the `## {VALIDATORS_HEADING}` section holds '
                             f'{len(validator_tables)} tables; the advisory check is skipped'))
    else:
        checked = 0
        for index, cells in validator_tables[0].rows:
            if not cells:
                continue
            tokens = backticked(cells[0])
            if not tokens:
                continue
            path = tokens[0]
            status = cells[1].lower() if len(cells) > 1 else ''
            checked += 1
            exists = (root / path).exists()
            if 'planned' in status and exists:
                findings.append(warn(
                    f'{path} is marked `planned` in the `## {VALIDATORS_HEADING}` table but the '
                    f'file exists (line {index + 1}) -- the exemption it relies on has ended, '
                    f'and the manifest should record it as `exists`'
                ))
            elif 'exists' in status and not exists:
                findings.append(warn(f'{path} is marked `exists` but is not on disk '
                                     f'(line {index + 1})'))
        if not findings:
            print(f'✅ {checked} validator row(s): the declared status matches the file on disk')
    failures += print_findings(findings)
    warnings += sum(1 for severity, _ in findings if severity == WARN)
    print()

    # --- SUMMARY -----------------------------------------------------------------------
    print('=' * 60)
    print('  SUMMARY')
    print('=' * 60)
    print()
    print(f'Status table: line {status_table.line}, {len(status_table.headers)} columns')
    print(f'Rows: {len(rows)}')
    print(f'`{EVIDENCE}` rows: {sum(1 for row in rows if row.token == EVIDENCE)}')
    print(f'Path claims checked: {len(claims)}')
    print(f'Validators run: {len(validators)}')
    print(f'Rows downgraded: {downgraded}')
    print(f'Failures: {failures}')
    print(f'Warnings: {warnings}')
    print()

    if failures:
        print('❌❌❌ SOME STANDARDS CHECKS FAILED ❌❌❌')
        return False

    print('✅✅✅ ALL STANDARDS CHECKS PASSED ✅✅✅')
    if warnings:
        print(f'   ({warnings} advisory warning(s) -- a prose claim in the manifest needs a '
              f'look, no value is wrong)')
    return True


if __name__ == '__main__':
    success = main()
    sys.exit(0 if success else 1)
