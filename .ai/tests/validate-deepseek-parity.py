#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates structural parity between CLAUDE.md and DEEPSEEK.md.

`DEEPSEEK.md` is not a mirror of `CLAUDE.md`; it *overwrites* it. The PowerShell profile backs
`CLAUDE.md` up, copies `DEEPSEEK.md` over it for the length of a DeepSeek session, and restores
the original on exit. So whatever `DEEPSEEK.md` does not carry is silently erased for the
duration of that session -- which has happened before, and is why this check exists.

**Not byte equality.** The two files differ on purpose: at HEAD the title, the identity sentence
and the model-tier paragraph are all different, and in a live DeepSeek session the working tree's
`CLAUDE.md` *is* `DEEPSEEK.md`, so a byte comparison passes there and fails everywhere else. The
invariant that holds in both states is the section structure, so this check compares level-2
headings in both directions.

Checks:

  1 SKIP  both files exist -- an absent file is nothing to compare, not a defect
  2 FAIL  every level-2 heading in one file is present in the other (advisory if only the
          order differs); the deliberate title and identity differences are informational
  3 FAIL  the core sections both files are built on are present in both
  4 ----  the deliberate differences, printed so the check is visibly not a byte comparison

Exit code: 0 when the structures agree or there is nothing to compare, 1 otherwise.
"""

import re
import sys
from pathlib import Path
from typing import List, Sequence, Tuple

# Fix encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')


def get_template_root() -> Path:
    """Get the repository root (this script lives at `<root>/.ai/tests/`)."""
    return Path(__file__).resolve().parent.parent.parent


CLAUDE_REL = 'CLAUDE.md'
DEEPSEEK_REL = 'DEEPSEEK.md'

# The frame both files are built on. A section lost from *both* files would still satisfy the
# parity check, so the frame is named here as well. CLAUDE.md is template-owned and always
# carries these; DEEPSEEK.md is hand-maintained, which is where the erasure happens.
REQUIRED_SECTIONS = [
    'Identity',
    'Session Lifecycle',
    'Task Execution Protocol',
    'Subagent Delegation',
    'Critical Coding Rules',
    'After Every Code Change',
    'Configuration',
    'Reference Appendix',
]

FAIL = 'FAIL'
WARN = 'WARN'
SKIP = 'SKIP'

Finding = Tuple[str, str]

_FENCE = re.compile(r'^\s*(?:```|~~~)')
_HEADING = re.compile(r'^(#{1,6})\s+(.*?)\s*$')


def read_lines(path: Path) -> List[str]:
    try:
        return path.read_text(encoding='utf-8', errors='replace').splitlines()
    except OSError:
        return []


def mask_fenced_lines(lines: Sequence[str]) -> List[str]:
    """Blank fenced blocks: `# comment` inside a code sample is not a section heading."""
    masked: List[str] = []
    in_fence = False
    for raw in lines:
        if _FENCE.match(raw):
            masked.append('')
            in_fence = not in_fence
            continue
        masked.append('' if in_fence else raw)
    return masked


def headings_at(lines: Sequence[str], level: int) -> List[str]:
    """The heading texts of one level, in document order."""
    titles: List[str] = []
    for raw in mask_fenced_lines(lines):
        match = _HEADING.match(raw)
        if match and len(match.group(1)) == level:
            titles.append(match.group(2))
    return titles


def print_findings(findings: Sequence[Finding]) -> int:
    failures = 0
    for severity, text in findings:
        if severity == FAIL:
            failures += 1
            print(f'❌ FAIL {text}')
        elif severity == WARN:
            print(f'⚠️  WARNING: {text}')
        else:
            print(f'⚠️  SKIP: {text}')
    return failures


def main() -> bool:
    root = get_template_root()
    claude_path = root / CLAUDE_REL
    deepseek_path = root / DEEPSEEK_REL

    print('=' * 60)
    print('  CLAUDE.md / DEEPSEEK.md Structural Parity')
    print('=' * 60)
    print()
    print('Invariant: the same level-2 section structure in both files. Byte equality is not')
    print('asserted -- DEEPSEEK.md deliberately differs, and overwrites CLAUDE.md in a')
    print('DeepSeek session (powershell/Microsoft.PowerShell.Profile.ps1).')
    print()

    failures = 0

    # --- TEST 1: both files present -----------------------------------------------------
    print('=' * 60)
    print('TEST 1: Both root documents exist')
    print('=' * 60)

    findings: List[Finding] = []
    missing_files = [rel for rel, path in ((CLAUDE_REL, claude_path),
                                           (DEEPSEEK_REL, deepseek_path))
                     if not path.exists()]
    for rel in missing_files:
        findings.append((SKIP, f'{rel} is not present -- there is nothing to compare, and an '
                               f'absent file is not a parity defect'))
    if not findings:
        print(f'✅ {CLAUDE_REL} and {DEEPSEEK_REL} are both present')
    failures += print_findings(findings)
    print()

    if missing_files:
        print('=' * 60)
        print('  SUMMARY')
        print('=' * 60)
        print()
        print(f'Sections compared: 0 ({len(missing_files)} file(s) absent)')
        print('Failures: 0')
        print()
        print('✅✅✅ NOTHING TO COMPARE -- SKIPPED, NOT FAILED ✅✅✅')
        return True

    claude_lines = read_lines(claude_path)
    deepseek_lines = read_lines(deepseek_path)
    claude_sections = headings_at(claude_lines, 2)
    deepseek_sections = headings_at(deepseek_lines, 2)

    # --- TEST 2: structural parity, both directions -------------------------------------
    print('=' * 60)
    print('TEST 2: Every level-2 heading in one file is present in the other')
    print('=' * 60)

    findings = []
    for title in claude_sections:
        if title not in deepseek_sections:
            findings.append((FAIL, f'{DEEPSEEK_REL} carries no `## {title}` section, which '
                                   f'{CLAUDE_REL} has -- a DeepSeek session would silently lose '
                                   f'it (the profile copies DEEPSEEK.md over CLAUDE.md)'))
    for title in deepseek_sections:
        if title not in claude_sections:
            findings.append((FAIL, f'{CLAUDE_REL} carries no `## {title}` section, which '
                                   f'{DEEPSEEK_REL} has'))

    if not findings:
        print(f'✅ {DEEPSEEK_REL} carries all {len(claude_sections)} section(s) of '
              f'{CLAUDE_REL}, and names none that it does not')
        if claude_sections != deepseek_sections:
            findings.append((WARN, f'the section sets agree but their order does not: '
                                   f'{CLAUDE_REL} = {claude_sections}; '
                                   f'{DEEPSEEK_REL} = {deepseek_sections}'))
    failures += print_findings(findings)
    print()

    # --- TEST 3: the core sections are present in both ----------------------------------
    print('=' * 60)
    print('TEST 3: The core sections are present in both files')
    print('=' * 60)

    findings = []
    for title in REQUIRED_SECTIONS:
        in_claude = title in claude_sections
        in_deepseek = title in deepseek_sections
        if in_claude and in_deepseek:
            continue
        where = []
        if not in_claude:
            where.append(CLAUDE_REL)
        if not in_deepseek:
            where.append(DEEPSEEK_REL)
        findings.append((FAIL, f'`## {title}` is missing from {" and ".join(where)} -- a '
                               f'section lost from both files still satisfies parity, so the '
                               f'core frame is named explicitly'))
    if not findings:
        print(f'✅ all {len(REQUIRED_SECTIONS)} core section(s) present in both files')
    failures += print_findings(findings)
    print()

    # --- TEST 4: the deliberate differences, informational ------------------------------
    print('=' * 60)
    print('TEST 4 (informational): The deliberate differences are not defects')
    print('=' * 60)

    claude_title = (headings_at(claude_lines, 1) or ['(none)'])[0]
    deepseek_title = (headings_at(deepseek_lines, 1) or ['(none)'])[0]
    print(f'   {CLAUDE_REL} title:   `# {claude_title}`')
    print(f'   {DEEPSEEK_REL} title: `# {deepseek_title}`')
    if claude_lines == deepseek_lines:
        print(f'   The files are byte-identical -- a DeepSeek profile swap is live in this '
              f'working tree, and parity holds trivially.')
    else:
        print(f'   The files differ ({len(claude_lines)} and {len(deepseek_lines)} lines) and '
              f'that is expected: the title, the identity sentence and the model-tier '
              f'paragraph differ on purpose. Only the section structure is asserted.')
    print()

    # --- SUMMARY ------------------------------------------------------------------------
    print('=' * 60)
    print('  SUMMARY')
    print('=' * 60)
    print()
    print(f'{CLAUDE_REL} sections: {len(claude_sections)}')
    print(f'{DEEPSEEK_REL} sections: {len(deepseek_sections)}')
    print(f'Core sections required in both: {len(REQUIRED_SECTIONS)}')
    print(f'Failures: {failures}')
    print()

    if failures:
        print('❌❌❌ STRUCTURAL PARITY BROKEN ❌❌❌')
        print('   A section missing from DEEPSEEK.md is erased from CLAUDE.md for the length')
        print('   of a DeepSeek session. Add the section to both files in the same commit.')
        return False

    print('✅✅✅ STRUCTURAL PARITY HOLDS ✅✅✅')
    return True


if __name__ == '__main__':
    success = main()
    sys.exit(0 if success else 1)
