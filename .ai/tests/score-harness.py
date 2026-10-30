#!/usr/bin/env python3
"""
Harness Health Score — a weighted 0-100 reporting layer over the template validators.

The point of this script is to *report*, not to *check*. Every individual check it
scores is an existing script in `.ai/tests/` or `.ai/scripts/`; this file runs those
scripts as subprocesses, reads their exit code and their console output, and maps the
result onto a weighted score. No validator logic is reimplemented here, and no
validator is modified.

Scoring model
-------------
Each of the 17 checks below carries a weight; the weights sum to exactly 100. A check
is classified from its exit code and output:

    exit != 0                        -> FAIL     credit 0.0
    exit == 0, no warning lines      -> PASS     credit 1.0
    exit == 0, >= 1 warning line     -> WARN     credit 0.8

A "warning line" is a line carrying the warning sign (`\u26a0\ufe0f`) that every
validator already uses for advisories. Lines marked `SKIP` are *not applicable here*
(the standards and traceability suites legitimately skip work in a template tree with
no solution file and no test sources), so they cost nothing: a check whose only
warning lines are SKIPs stays PASS.

    score = sum(weight * credit)      max 100
    grade = A+ >= 95, A >= 90, B >= 80, C >= 70, D >= 60, F < 60

Invoking the 17 checks means invoking the same validators `run-all-tests.sh` runs, so
`run-all-tests.sh` deliberately does not call this script — that would double the
runtime without adding a single new check. This scorer is a convenience/reporting
entry point, not part of the pass/fail gate.

Usage
-----
    python .ai/tests/score-harness.py
    python .ai/tests/score-harness.py --json harness-score.json --badge harness-score.svg
    python .ai/tests/score-harness.py --fail-under 90
    python .ai/tests/score-harness.py --from-json harness-score.json --badge badge.svg
    python .ai/tests/score-harness.py --list

The module filename contains a hyphen, so it cannot be imported by module name; tests
(and anything else) load it by path with importlib. Every pure function it exposes
(`classify`, `credit_for`, `grade_for`, `badge_color`, `render_badge`, `compute`,
`render_report`) is importable that way.
"""

import argparse
import json
import re
import shutil
import subprocess
import sys
import time
from dataclasses import dataclass, field
from datetime import datetime
from html import escape as xml_escape
from pathlib import Path

TESTS_DIR = Path(__file__).resolve().parent
TEMPLATE_ROOT = TESTS_DIR.parent.parent

SCHEMA_VERSION = 1
MAX_SCORE = 100.0
OUTPUT_TAIL_LINES = 40

# The warning sign every validator uses for advisory output. Both the two-codepoint
# form (`\u26a0\ufe0f`) and the bare sign are recognised; a line carrying it is a
# warning, unless it also says SKIP.
WARNING_MARK = "\u26a0"
SKIP_MARK = "SKIP"

PASS_ICON = "\u2705"       # white heavy check mark
WARN_ICON = "\u26a0\ufe0f"  # warning sign
FAIL_ICON = "\u274c"       # cross mark

CREDITS = {"PASS": 1.0, "WARN": 0.8, "FAIL": 0.0}

CATEGORY_ORDER = ("Integrity", "Platform", "Quality", "Standards", "Release")


@dataclass(frozen=True)
class Check:
    """One weighted check: which existing script it runs and how much it is worth."""

    id: str
    label: str
    category: str
    weight: float
    runner: str            # "py" or "bash"
    script: str            # path relative to TEMPLATE_ROOT
    args: tuple[str, ...] = field(default=())


# The 17 checks, in report order. Weights sum to exactly 100.
#   Integrity 40  (5+5+5+4+4+3+3+6+5)
#   Platform  15  (4+4+7)
#   Quality   15  (7+8)
#   Standards 14  (14)
#   Release   16  (8+8)
CHECKS: tuple[Check, ...] = (
    Check("structure",    "Directory structure",  "Integrity", 5,  "bash", ".ai/tests/validate-structure.sh"),
    Check("skills",       "Skill frontmatter",    "Integrity", 5,  "py",   ".ai/tests/validate-skills.py"),
    Check("references",   "File references",      "Integrity", 5,  "bash", ".ai/tests/validate-references.sh"),
    Check("content",      "Content quality",      "Integrity", 4,  "py",   ".ai/tests/validate-content.py"),
    Check("tokens",       "Token consistency",    "Integrity", 4,  "bash", ".ai/tests/validate-tokens.sh"),
    Check("agents-md",    "AGENTS.md references", "Integrity", 3,  "py",   ".ai/tests/validate-agents-md.py"),
    Check("smoke",        "Integration smoke",    "Integrity", 3,  "py",   ".ai/tests/smoke-test.py"),
    Check("triggers",     "Trigger lines",        "Integrity", 6,  "py",   ".ai/tests/validate-triggers.py"),
    Check("prose",        "Prose to skills",      "Integrity", 5,  "py",   ".ai/tests/validate-prose.py"),
    Check("settings",     "Claude settings",      "Platform",  4,  "py",   ".ai/tests/validate-settings.py"),
    Check("copilot",      "Copilot integration",  "Platform",  4,  "py",   ".ai/tests/validate-copilot.py"),
    Check("platform-sync", "Platform sync drift", "Platform",  7,  "py",   ".ai/scripts/sync-platforms.py", ("--dry-run",)),
    Check("traceability", "Traceability",         "Quality",   7,  "py",   ".ai/tests/validate-traceability.py"),
    Check("hygiene",      "Process hygiene",      "Quality",   8,  "py",   ".ai/scripts/hygiene-lint.py"),
    Check("standards",    "Standards manifest",   "Standards", 14, "py",   ".ai/tests/validate-standards.py"),
    Check("release",      "Release versioning",   "Release",   8,  "py",   ".ai/tests/validate-release.py"),
    Check("upgrade",      "Upgrade script",       "Release",   8,  "py",   ".ai/tests/validate-upgrade-script.py"),
)


# ---------------------------------------------------------------------------
# Pure functions
# ---------------------------------------------------------------------------
def classify(exit_code: int, output: str) -> dict:
    """Map an exit code plus console output onto PASS / WARN / FAIL.

    `warnings` counts advisory lines (which dock credit); `skips` counts lines that
    carry the warning mark but say SKIP, i.e. work that is not applicable here and
    therefore costs nothing.
    """
    if exit_code != 0:
        return {"status": "FAIL", "warnings": 0, "skips": 0}

    warnings = 0
    skips = 0
    for line in (output or "").splitlines():
        if WARNING_MARK not in line:
            continue
        if SKIP_MARK in line:
            skips += 1
        else:
            warnings += 1
    return {"status": "WARN" if warnings else "PASS", "warnings": warnings, "skips": skips}


def credit_for(status: str) -> float:
    """Fraction of a check's weight earned by a status."""
    return CREDITS.get(str(status).upper(), 0.0)


# `sync-platforms.py` always exits 0: its change count is the real signal. Read the
# summary line instead of trusting the exit code, or drift can never be scored.
DRIFT_RE = re.compile(r"(\d+)\s+item\(s\)\s+(?:would be\s+)?changed")


def drift_count(output: str) -> int:
    """Junction changes a sync-platforms run reported (0 when the line is absent)."""
    match = DRIFT_RE.search(output or "")
    return int(match.group(1)) if match else 0


def apply_drift(verdict: dict, check_id: str, output: str) -> tuple[dict, int, str]:
    """Promote a clean platform-sync exit to FAIL when junctions would change.

    Returns (verdict, changed, note). Every other check is returned untouched.
    """
    if check_id != "platform-sync":
        return verdict, 0, ""
    changed = drift_count(output)
    if changed and verdict["status"] == "PASS":
        promoted = {"status": "FAIL", "warnings": 0, "skips": verdict.get("skips", 0)}
        return promoted, changed, f"{changed} junction change(s) pending"
    return verdict, changed, ""


def grade_for(score: float) -> str:
    """Letter grade for a 0-100 score."""
    if score >= 95:
        return "A+"
    if score >= 90:
        return "A"
    if score >= 80:
        return "B"
    if score >= 70:
        return "C"
    if score >= 60:
        return "D"
    return "F"


def badge_color(score: float) -> str:
    """shields.io colour for a score band."""
    if score >= 90:
        return "#4c1"
    if score >= 80:
        return "#97ca00"
    if score >= 70:
        return "#dfb317"
    if score >= 60:
        return "#fe7d37"
    return "#e05d44"


def fmt_score(score: float) -> str:
    """1-decimal score without a trailing '.0' when the value is integral."""
    value = round(float(score), 1)
    if value == int(value):
        return str(int(value))
    return f"{value:.1f}"


def compute(checks) -> dict:
    """Turn per-check results into the report document.

    `checks` is a sequence of dicts carrying at least id, label, category, weight and
    status; `credit` is derived from status when absent and `earned` is recomputed.
    Category totals follow CATEGORY_ORDER and every category is always present.
    """
    rows = []
    totals = {name: [0.0, 0.0] for name in CATEGORY_ORDER}  # name -> [score, max]

    for raw in checks:
        weight = float(raw["weight"])
        status = str(raw.get("status", "FAIL")).upper()
        credit = raw.get("credit")
        credit = credit_for(status) if credit is None else float(credit)
        earned = round(weight * credit, 2)

        row = {
            "id": raw["id"],
            "label": raw["label"],
            "category": raw["category"],
            "weight": weight,
            "status": status,
            "credit": credit,
            "earned": earned,
            "exit_code": raw.get("exit_code"),
            "warnings": int(raw.get("warnings", 0)),
            "skips": int(raw.get("skips", 0)),
            "duration_ms": int(raw.get("duration_ms", 0)),
            "note": raw.get("note", ""),
            "output_tail": raw.get("output_tail", ""),
        }
        rows.append(row)

        if row["category"] not in totals:
            totals[row["category"]] = [0.0, 0.0]
        totals[row["category"]][0] += weight * credit
        totals[row["category"]][1] += weight

    categories = {}
    for name, (cat_score, cat_max) in totals.items():
        cat_score = round(cat_score, 2)
        cat_max = round(cat_max, 2)
        percent = round((cat_score / cat_max) * 100, 1) if cat_max else 0.0
        categories[name] = {"score": cat_score, "max": cat_max, "percent": percent}

    score = round(sum(row["earned"] for row in rows), 1)
    score = min(score, MAX_SCORE)

    return {
        "schema_version": SCHEMA_VERSION,
        "generated_at": datetime.now().astimezone().isoformat(timespec="seconds"),
        "score": score,
        "grade": grade_for(score),
        "max_score": MAX_SCORE,
        "categories": categories,
        "checks": rows,
    }


def render_badge(label: str, message: str, color: str) -> str:
    """Render a shields.io 'flat' style SVG badge.

    Geometry mirrors shields.io: 5px padding either side of each text run and an
    approximate 6.5px advance per character, 20px tall, 3px corner radius.
    """
    label = str(label)
    message = str(message)
    pad = 5
    per_char = 6.5
    label_w = int(round(len(label) * per_char)) + pad * 2
    message_w = int(round(len(message) * per_char)) + pad * 2
    total_w = label_w + message_w
    label_x = label_w / 2
    message_x = label_w + message_w / 2

    esc_label = xml_escape(label)
    esc_message = xml_escape(message)
    aria = xml_escape(f"{label}: {message}")

    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{total_w}" height="20" '
        f'role="img" aria-label="{aria}">\n'
        f"  <title>{aria}</title>\n"
        f'  <linearGradient id="s" x2="0" y2="100%">\n'
        f'    <stop offset="0" stop-color="#bbb" stop-opacity=".1"/>\n'
        f'    <stop offset="1" stop-opacity=".1"/>\n'
        f"  </linearGradient>\n"
        f'  <clipPath id="r">\n'
        f'    <rect width="{total_w}" height="20" rx="3" fill="#fff"/>\n'
        f"  </clipPath>\n"
        f'  <g clip-path="url(#r)">\n'
        f'    <rect width="{label_w}" height="20" fill="#555"/>\n'
        f'    <rect x="{label_w}" width="{message_w}" height="20" fill="{xml_escape(color)}"/>\n'
        f'    <rect width="{total_w}" height="20" fill="url(#s)"/>\n'
        f"  </g>\n"
        f'  <g fill="#fff" text-anchor="middle" '
        f'font-family="Verdana,Geneva,DejaVu Sans,sans-serif" font-size="11">\n'
        f'    <text x="{label_x:.1f}" y="15" fill="#010101" fill-opacity=".3">{esc_label}</text>\n'
        f'    <text x="{label_x:.1f}" y="14">{esc_label}</text>\n'
        f'    <text x="{message_x:.1f}" y="15" fill="#010101" fill-opacity=".3">{esc_message}</text>\n'
        f'    <text x="{message_x:.1f}" y="14">{esc_message}</text>\n'
        f"  </g>\n"
        f"</svg>\n"
    )


def badge_message(doc: dict) -> str:
    """The badge's message text: '<score>/100 <grade>'."""
    return f"{fmt_score(doc['score'])}/100 {doc['grade']}"


def render_report(doc: dict) -> str:
    """Render the console report for a document produced by `compute`."""
    rows = doc["checks"]
    lines = []
    lines.append("")
    lines.append("Harness Health Score")
    lines.append("=" * 74)
    lines.append("")

    icons = {"PASS": PASS_ICON, "WARN": WARN_ICON, "FAIL": FAIL_ICON}
    for row in rows:
        icon = icons.get(row["status"], FAIL_ICON)
        lines.append(
            f"  {icon}  {row['id']:<17}{row['category']:<11}"
            f"{fmt_score(row['earned'])}/{fmt_score(row['weight'])}  {row['status']}"
        )

    lines.append("")
    lines.append("-" * 74)
    lines.append(f"  {'Category':<12}{'Score':>8}{'Max':>8}{'Percent':>10}")
    lines.append("-" * 74)
    categories = doc["categories"]
    for name in list(CATEGORY_ORDER) + [
        n for n in categories if n not in CATEGORY_ORDER
    ]:
        if name not in categories:
            continue
        cat = categories[name]
        lines.append(
            f"  {name:<12}{fmt_score(cat['score']):>8}"
            f"{fmt_score(cat['max']):>8}{cat['percent']:>9.1f}%"
        )
    lines.append("-" * 74)
    lines.append("")
    lines.append(f"TOTAL: {fmt_score(doc['score'])}/100  Grade {doc['grade']}")
    lines.append("")

    deductions = [row for row in rows if row["status"] != "PASS"]
    if not deductions:
        lines.append("No deductions - every check is clean.")
    else:
        lines.append("Deductions:")
        for row in deductions:
            if row["status"] == "WARN":
                lost = round(float(row["weight"]) - float(row["earned"]), 2)
                lines.append(
                    f"  - {row['id']} ({row['category']}, -{fmt_score(lost)}): "
                    f"WARN: {row['warnings']} advisory warning(s)"
                )
            else:
                exit_code = row.get("exit_code")
                exit_text = "n/a" if exit_code is None else str(exit_code)
                note = f" - {row['note']}" if row.get("note") else ""
                lines.append(
                    f"  - {row['id']} ({row['category']}, "
                    f"-{fmt_score(row['weight'])}): FAIL: exit {exit_text}{note}"
                )
    lines.append("")
    lines.append("Lines marked SKIP are not applicable here and cost no points.")
    lines.append("")
    return "\n".join(lines)


# ---------------------------------------------------------------------------
# Running the checks
# ---------------------------------------------------------------------------
def find_bash():
    """Prefer Git Bash on Windows; fall back to whatever bash is on PATH."""
    if sys.platform == "win32":
        for candidate in (
            r"C:\Program Files\Git\bin\bash.exe",
            r"C:\Program Files\Git\usr\bin\bash.exe",
        ):
            if Path(candidate).exists():
                return candidate
    return shutil.which("bash")


def _tail(output: str) -> str:
    lines = (output or "").splitlines()
    return "\n".join(lines[-OUTPUT_TAIL_LINES:])


def run_check(check: Check, timeout: float) -> dict:
    """Run one check and return its result dict. Never raises for a bad script."""
    started = time.monotonic()
    script_path = TEMPLATE_ROOT / check.script
    result = {
        "id": check.id,
        "label": check.label,
        "category": check.category,
        "weight": float(check.weight),
        "status": "FAIL",
        "credit": 0.0,
        "earned": 0.0,
        "exit_code": None,
        "warnings": 0,
        "skips": 0,
        "duration_ms": 0,
        "note": "",
        "output_tail": "",
    }

    if not script_path.exists():
        result["duration_ms"] = int((time.monotonic() - started) * 1000)
        result["note"] = f"script not found: {check.script}"
        return result

    if check.runner == "bash":
        bash = find_bash()
        if bash is None:
            result["duration_ms"] = int((time.monotonic() - started) * 1000)
            result["note"] = "bash not found (install Git Bash or put bash on PATH)"
            return result
        # A plain filename with cwd=TESTS_DIR, so bash never sees a Windows path.
        cmd = [bash, Path(check.script).name]
        cwd = TESTS_DIR
    else:
        cmd = [sys.executable, str(script_path), *check.args]
        cwd = TEMPLATE_ROOT

    try:
        completed = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            cwd=str(cwd),
            timeout=timeout,
        )
    except subprocess.TimeoutExpired as exc:
        result["duration_ms"] = int((time.monotonic() - started) * 1000)
        partial = exc.output if isinstance(exc.output, str) else ""
        if isinstance(partial, bytes):
            partial = partial.decode("utf-8", "replace")
        result["note"] = f"timed out after {timeout:g}s"
        result["output_tail"] = _tail(partial)
        return result
    except OSError as exc:
        result["duration_ms"] = int((time.monotonic() - started) * 1000)
        result["note"] = f"{type(exc).__name__}: {exc}"
        return result

    output = (completed.stdout or "") + (completed.stderr or "")
    verdict = classify(completed.returncode, output)
    verdict, _drifted, drift_note = apply_drift(verdict, check.id, output)

    result["duration_ms"] = int((time.monotonic() - started) * 1000)
    result["exit_code"] = completed.returncode
    result["status"] = verdict["status"]
    result["warnings"] = verdict["warnings"]
    result["skips"] = verdict["skips"]
    result["credit"] = credit_for(verdict["status"])
    result["earned"] = round(float(check.weight) * result["credit"], 2)
    result["note"] = drift_note
    result["output_tail"] = _tail(output)
    return result


def score_all(timeout: float) -> dict:
    """Run every check and compute the document."""
    results = [run_check(check, timeout) for check in CHECKS]
    return compute(results)


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------
def _print_list() -> None:
    total = sum(check.weight for check in CHECKS)
    print(f"{'ID':<17}{'Category':<11}{'Weight':>7}  Script")
    print("-" * 74)
    for check in CHECKS:
        args = " " + " ".join(check.args) if check.args else ""
        print(
            f"{check.id:<17}{check.category:<11}{check.weight:>7.1f}  "
            f"{check.runner}: {check.script}{args}"
        )
    print("-" * 74)
    print(f"{len(CHECKS)} checks, weights total {fmt_score(total)}")


def _force_utf8_console() -> None:
    """Windows consoles default to cp1252, which cannot encode the status icons."""
    for stream in (sys.stdout, sys.stderr):
        reconfigure = getattr(stream, "reconfigure", None)
        if reconfigure is None:
            continue
        try:
            reconfigure(encoding="utf-8", errors="replace")
        except (OSError, ValueError):  # detached or already-wrapped stream
            pass


def main(argv=None) -> int:
    _force_utf8_console()
    parser = argparse.ArgumentParser(
        prog="score-harness.py",
        description=(
            "Score the template validation harness out of 100 by running the existing "
            "validators and weighting their results."
        ),
    )
    parser.add_argument("--json", metavar="PATH", help="write the result document as JSON")
    parser.add_argument("--badge", metavar="PATH", help="write a shields.io SVG badge")
    parser.add_argument(
        "--from-json",
        metavar="PATH",
        help="re-render a saved document without running any check",
    )
    parser.add_argument(
        "--fail-under",
        type=float,
        default=None,
        metavar="N",
        help="exit 1 when the score is below N",
    )
    parser.add_argument(
        "--timeout",
        type=float,
        default=300.0,
        metavar="SECONDS",
        help="per-check timeout in seconds (default 300)",
    )
    parser.add_argument(
        "--quiet",
        action="store_true",
        help="print only the final '<score>/100 <grade>' line",
    )
    parser.add_argument(
        "--list",
        action="store_true",
        help="print the checks with their weights and exit",
    )
    args = parser.parse_args(argv)

    if args.list:
        _print_list()
        return 0

    if args.from_json:
        path = Path(args.from_json)
        if not path.exists():
            print(f"error: {path} not found", file=sys.stderr)
            return 2
        doc = json.loads(path.read_text(encoding="utf-8"))
    else:
        doc = score_all(args.timeout)

    if args.json and not args.from_json:
        out = Path(args.json)
        if out.parent and not out.parent.exists():
            out.parent.mkdir(parents=True, exist_ok=True)
        out.write_text(json.dumps(doc, indent=2) + "\n", encoding="utf-8")

    if args.badge:
        out = Path(args.badge)
        if out.parent and not out.parent.exists():
            out.parent.mkdir(parents=True, exist_ok=True)
        svg = render_badge("harness", badge_message(doc), badge_color(doc["score"]))
        out.write_text(svg, encoding="utf-8")

    if args.quiet:
        print(f"{fmt_score(doc['score'])}/100 {doc['grade']}")
    else:
        print(render_report(doc))

    if args.fail_under is not None and doc["score"] < args.fail_under:
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
