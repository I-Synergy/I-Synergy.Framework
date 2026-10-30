"""Fast unit tests for `.ai/tests/score-harness.py`.

No validator is executed here: everything is synthetic, so the file finishes in
well under a second. The scorer's filename contains a hyphen, so it is loaded by
path rather than imported by name.
"""

import importlib.util
import json
import subprocess
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

TESTS_DIR = Path(__file__).parent
TEMPLATE_ROOT = TESTS_DIR.parent.parent
SCORER_PATH = TESTS_DIR / "score-harness.py"


def _load():
    spec = importlib.util.spec_from_file_location("score_harness", SCORER_PATH)
    module = importlib.util.module_from_spec(spec)
    # dataclasses resolve annotations through sys.modules, so register before exec.
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


score_harness = _load()


def _check(check_id, category, weight, status, warnings=0, skips=0, exit_code=0):
    """A synthetic per-check result row, shaped like the scorer's own rows."""
    return {
        "id": check_id,
        "label": check_id,
        "category": category,
        "weight": weight,
        "status": status,
        "exit_code": exit_code,
        "warnings": warnings,
        "skips": skips,
        "duration_ms": 1,
        "note": "",
        "output_tail": "",
    }


def _all_pass():
    return [
        _check(c.id, c.category, c.weight, "PASS")
        for c in score_harness.CHECKS
    ]


def _row(checks, check_id):
    """The single synthetic row for `check_id`, selected by id not position."""
    match = [c for c in checks if c["id"] == check_id]
    assert len(match) == 1, check_id
    return match[0]


# ---------------------------------------------------------------------------
# 1. The check table
# ---------------------------------------------------------------------------
def test_weights_sum_to_exactly_100():
    weights = [c.weight for c in score_harness.CHECKS]
    assert len(weights) == 17
    assert sum(weights) == 100


def test_check_ids_are_unique_and_ordered():
    ids = [c.id for c in score_harness.CHECKS]
    assert len(set(ids)) == len(ids) == 17
    assert ids[0] == "structure"
    assert ids[-1] == "upgrade"


def test_category_weights_match_the_documented_breakdown():
    expected = {"Integrity": 40, "Platform": 15, "Quality": 15, "Standards": 14, "Release": 16}
    actual = {}
    for check in score_harness.CHECKS:
        actual[check.category] = actual.get(check.category, 0) + check.weight
    assert actual == expected


def test_new_trigger_and_prose_checks_have_expected_weights():
    weights = {c.id: c.weight for c in score_harness.CHECKS}
    assert weights["triggers"] == 6
    assert weights["prose"] == 5


def test_every_declared_script_exists():
    for check in score_harness.CHECKS:
        assert (TEMPLATE_ROOT / check.script).is_file(), check.script
        assert check.runner in {"py", "bash"}, check.id


def test_run_check_missing_script_returns_fail():
    """A check pointing at a nonexistent script fails fast with a descriptive note."""
    fabricated = score_harness.Check(
        "fabricated", "Fabricated", "Integrity", 1, "py",
        ".ai/tests/does-not-exist-xyz.py",
    )
    result = score_harness.run_check(fabricated, timeout=5)
    assert result["status"] == "FAIL"
    assert result["credit"] == 0.0
    assert "script not found" in result["note"]


# ---------------------------------------------------------------------------
# 2. Classification and credit
# ---------------------------------------------------------------------------
def test_classify_pass():
    result = score_harness.classify(0, "\u2705 all good\n")
    assert result == {"status": "PASS", "warnings": 0, "skips": 0}


def test_classify_warn():
    result = score_harness.classify(0, "\u26a0\ufe0f  WARNING: x\n")
    assert result["status"] == "WARN"
    assert result["warnings"] == 1
    assert result["skips"] == 0


def test_classify_skip_is_neutral():
    result = score_harness.classify(0, "\u26a0\ufe0f  SKIP: x\n")
    assert result["status"] == "PASS"
    assert result["skips"] == 1
    assert result["warnings"] == 0


def test_classify_fail():
    assert score_harness.classify(1, "boom")["status"] == "FAIL"


def test_classify_mixed_warnings_and_skips():
    output = "\u26a0\ufe0f  SKIP: a\n\u26a0\ufe0f  WARNING: b\n\u26a0\ufe0f  WARNING: c\n"
    result = score_harness.classify(0, output)
    assert result == {"status": "WARN", "warnings": 2, "skips": 1}


def test_credit_values():
    assert score_harness.credit_for("PASS") == 1.0
    assert score_harness.credit_for("WARN") == 0.8
    assert score_harness.credit_for("FAIL") == 0.0


# ---------------------------------------------------------------------------
# 2b. Platform drift: sync-platforms exits 0 even when junctions would change
# ---------------------------------------------------------------------------
def test_drift_count_reads_the_sync_summary():
    assert score_harness.drift_count("0 item(s) changed.") == 0
    assert score_harness.drift_count("3 item(s) changed.") == 3
    assert score_harness.drift_count("[DRY RUN] 2 item(s) would be changed.") == 2
    assert score_harness.drift_count("no summary here") == 0


def test_apply_drift_promotes_clean_sync_with_changes():
    verdict = {"status": "PASS", "warnings": 0, "skips": 0}
    promoted, changed, note = score_harness.apply_drift(
        verdict, "platform-sync", "[DRY RUN] 1 item(s) would be changed.")
    assert promoted["status"] == "FAIL"
    assert changed == 1
    assert "1 junction change" in note


def test_apply_drift_leaves_clean_sync_and_other_checks_alone():
    verdict = {"status": "PASS", "warnings": 0, "skips": 0}
    same, changed, note = score_harness.apply_drift(
        verdict, "platform-sync", "0 item(s) changed.")
    assert same is verdict and changed == 0 and note == ""

    other, changed, note = score_harness.apply_drift(
        {"status": "PASS", "warnings": 0, "skips": 0}, "structure", "1 item(s) changed.")
    assert other["status"] == "PASS" and changed == 0 and note == ""


# ---------------------------------------------------------------------------
# 3. Grade and colour boundaries
# ---------------------------------------------------------------------------
def test_grade_boundaries():
    assert score_harness.grade_for(100.0) == "A+"
    assert score_harness.grade_for(95.0) == "A+"
    assert score_harness.grade_for(94.9) == "A"
    assert score_harness.grade_for(90.0) == "A"
    assert score_harness.grade_for(89.9) == "B"
    assert score_harness.grade_for(80.0) == "B"
    assert score_harness.grade_for(79.9) == "C"
    assert score_harness.grade_for(70.0) == "C"
    assert score_harness.grade_for(69.9) == "D"
    assert score_harness.grade_for(60.0) == "D"
    assert score_harness.grade_for(59.9) == "F"
    assert score_harness.grade_for(0.0) == "F"


def test_badge_color_bands():
    assert score_harness.badge_color(100) == "#4c1"
    assert score_harness.badge_color(90) == "#4c1"
    assert score_harness.badge_color(89.9) == "#97ca00"
    assert score_harness.badge_color(80) == "#97ca00"
    assert score_harness.badge_color(79.9) == "#dfb317"
    assert score_harness.badge_color(70) == "#dfb317"
    assert score_harness.badge_color(69.9) == "#fe7d37"
    assert score_harness.badge_color(60) == "#fe7d37"
    assert score_harness.badge_color(59.9) == "#e05d44"


# ---------------------------------------------------------------------------
# 4. compute()
# ---------------------------------------------------------------------------
def test_all_pass_scores_100_with_a_plus():
    doc = score_harness.compute(_all_pass())
    assert doc["schema_version"] == 1
    assert doc["score"] == 100.0
    assert doc["grade"] == "A+"
    assert doc["max_score"] == 100
    assert len(doc["checks"]) == 17
    for name, cat in doc["categories"].items():
        assert cat["percent"] == 100.0, name


def test_one_fail_drops_exactly_its_weight():
    checks = _all_pass()
    _row(checks, "platform-sync")["status"] = "FAIL"  # weight 7
    doc = score_harness.compute(checks)
    assert doc["score"] == 93.0
    assert doc["grade"] == "A"
    failed = [c for c in doc["checks"] if c["id"] == "platform-sync"][0]
    assert failed["credit"] == 0.0
    assert failed["earned"] == 0.0


def test_one_warn_drops_twenty_percent_of_its_weight():
    checks = _all_pass()
    _row(checks, "standards")["status"] = "WARN"  # weight 14
    _row(checks, "standards")["warnings"] = 3
    doc = score_harness.compute(checks)
    assert round(100.0 - doc["score"], 2) == round(0.2 * 14, 2)
    assert doc["score"] == round(100 - 2.8, 1)


def test_category_breakdown_sums_to_total():
    checks = _all_pass()
    _row(checks, "structure")["status"] = "FAIL"   # Integrity, 5
    _row(checks, "release")["status"] = "WARN"     # Release, 8
    doc = score_harness.compute(checks)

    category_sum = round(sum(cat["score"] for cat in doc["categories"].values()), 2)
    assert category_sum == doc["score"]
    assert doc["categories"]["Integrity"]["score"] == 35.0
    assert doc["categories"]["Integrity"]["max"] == 40
    assert doc["categories"]["Release"]["score"] == 14.4
    assert doc["categories"]["Integrity"]["percent"] == round(35 / 40 * 100, 1)


def test_report_names_deductions():
    checks = _all_pass()
    _row(checks, "structure")["status"] = "FAIL"
    _row(checks, "structure")["exit_code"] = 1
    _row(checks, "skills")["status"] = "WARN"
    _row(checks, "skills")["warnings"] = 2
    doc = score_harness.compute(checks)
    report = score_harness.render_report(doc)

    assert "Harness Health Score" in report
    assert "TOTAL: 94/100  Grade A" in report
    assert "structure" in report and "FAIL: exit 1" in report
    assert "skills" in report and "2 advisory warning(s)" in report
    assert "SKIP" in report


def test_clean_report_says_no_deductions():
    report = score_harness.render_report(score_harness.compute(_all_pass()))
    assert "No deductions - every check is clean." in report


# ---------------------------------------------------------------------------
# 5. Badge rendering
# ---------------------------------------------------------------------------
def test_badge_is_well_formed_and_escaped():
    svg = score_harness.render_badge("harness", "100/100 A+", "#4c1")
    root = ET.fromstring(svg)
    assert root.tag.endswith("svg")
    assert root.get("height") == "20"
    title = root.find("{http://www.w3.org/2000/svg}title")
    assert title is not None and title.text == "harness: 100/100 A+"
    assert "#4c1" in svg


def test_badge_escapes_ampersand():
    svg = score_harness.render_badge("harness", "PASS & WARN", "#dfb317")
    assert "&amp;" in svg
    assert "PASS & WARN" not in svg.replace("&amp;", "\x00")
    root = ET.fromstring(svg)
    title = root.find("{http://www.w3.org/2000/svg}title")
    assert title.text == "harness: PASS & WARN"


def test_badge_message_formats_integral_scores_without_decimals():
    doc = score_harness.compute(_all_pass())
    assert score_harness.badge_message(doc) == "100/100 A+"


# ---------------------------------------------------------------------------
# 6. CLI: --from-json must re-render without running any check
# ---------------------------------------------------------------------------
def test_from_json_roundtrip(tmp_path):
    checks = _all_pass()
    _row(checks, "platform-sync")["status"] = "FAIL"
    _row(checks, "platform-sync")["exit_code"] = 1
    doc = score_harness.compute(checks)

    saved = tmp_path / "score.json"
    saved.write_text(json.dumps(doc, indent=2), encoding="utf-8")

    completed = subprocess.run(
        [
            sys.executable,
            str(SCORER_PATH),
            "--from-json",
            str(saved),
            "--quiet",
        ],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        timeout=60,
    )
    assert completed.returncode == 0, completed.stderr
    assert completed.stdout.strip() == "93/100 A"


def test_from_json_badge_output(tmp_path):
    doc = score_harness.compute(_all_pass())
    saved = tmp_path / "score.json"
    saved.write_text(json.dumps(doc), encoding="utf-8")
    badge = tmp_path / "badge.svg"

    completed = subprocess.run(
        [
            sys.executable,
            str(SCORER_PATH),
            "--from-json",
            str(saved),
            "--badge",
            str(badge),
            "--quiet",
        ],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        timeout=60,
    )
    assert completed.returncode == 0, completed.stderr
    assert completed.stdout.strip() == "100/100 A+"
    ET.fromstring(badge.read_text(encoding="utf-8"))


def test_fail_under_exits_nonzero(tmp_path):
    checks = _all_pass()
    _row(checks, "traceability")["status"] = "FAIL"  # weight 7 -> score 93
    doc = score_harness.compute(checks)
    saved = tmp_path / "score.json"
    saved.write_text(json.dumps(doc), encoding="utf-8")

    completed = subprocess.run(
        [
            sys.executable,
            str(SCORER_PATH),
            "--from-json",
            str(saved),
            "--fail-under",
            "95",
            "--quiet",
        ],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        timeout=60,
    )
    assert completed.returncode == 1
    assert completed.stdout.strip() == "93/100 A"
