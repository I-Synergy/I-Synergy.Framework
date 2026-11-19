#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Validates the Copilot-parity adapter layout:
  1. No Copilot prompt variables (dollar-brace) in skills
  2. The template ships no .ai/prompts/ (prompts are skills now; projects may install catalog prompts)
  3. Prompt adapters are links to .ai/prompts/ only while it has files; .claude/instructions is absent
  4. Agent bodies reference their skills and critical rules (Copilot ignores `skills:`)
  5. copilot-instructions.md embeds .ai/reference/copilot-runtime.md
  6. .claude/rules matches what sync-platforms.py would generate
  7. Hook commands point at existing files; no .github/hooks/
  8. .vscode/tasks.json is tracked and not auto-run; Agents.md auto-load is off;
     Copilot custom agents, Claude hooks, agent skills and instruction files are pinned on
  9. orchestrator.md is the Copilot coordinator: no delegation target, lists every other agent
 10. VS Code fragments (.ai/reference/templates/vscode/) are valid, free of template-only keys,
     contained in the template's own .vscode files, and consistent with merge-rules.json
"""

import importlib.util
import io
import json
import os
import re
import subprocess
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8")

SCRIPT_DIR = Path(__file__).parent
ROOT = SCRIPT_DIR.parent.parent

RUNTIME_START = "<!-- ai-workspace-manager:copilot-runtime:start -->"
RUNTIME_END = "<!-- ai-workspace-manager:copilot-runtime:end -->"
PROMPT_VAR = "$" + "{"

# Workspace settings that must stay on so user settings cannot silently disable them.
PINNED_SETTINGS = (
    "github.copilot.chat.cli.customAgents.enabled",
    "chat.useClaudeHooks",
    "chat.useAgentSkills",
    "chat.includeApplyingInstructions",
    "github.copilot.chat.codeGeneration.useInstructionFiles",
)


def _read(path: Path) -> str:
    return path.read_bytes().decode("utf-8", errors="replace").replace("\r\n", "\n")


def test_no_prompt_variables() -> bool:
    print("TEST 1: No Copilot prompt variables in skills")
    print("-" * 40)
    bad = []
    for skill in sorted((ROOT / ".ai" / "skills").glob("*/SKILL.md")):
        if PROMPT_VAR in _read(skill):
            bad.append(skill.parent.name)
    if bad:
        print(f"  FAIL: prompt variable token found in: {bad}")
        return False
    print(f"  PASS: no {PROMPT_VAR!r} token in any SKILL.md")
    return True


def test_prompts_dir_absent() -> bool:
    print("\nTEST 2: Template ships no .ai/prompts/")
    print("-" * 40)
    if (ROOT / ".ai" / "prompts").exists():
        print("  FAIL: .ai/prompts/ exists in the template (prompts are skills now; "
              "catalog-installed prompts belong in projects, not in the template)")
        return False
    print("  PASS: .ai/prompts/ does not exist (projects may install prompts from a catalog)")
    return True


def _load_sync_module():
    script = ROOT / ".ai" / "scripts" / "sync-platforms.py"
    spec = importlib.util.spec_from_file_location("sync_platforms_adapter_check", script)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


def test_retired_adapters() -> bool:
    print("\nTEST 3: Prompt adapters follow .ai/prompts/; retired adapters absent")
    print("-" * 40)
    sync = _load_sync_module()

    def present(path: Path) -> bool:
        return path.exists() or sync._is_link(path)

    passed = True
    retired = ROOT / ".claude" / "instructions"
    if present(retired):
        print("  FAIL: .claude/instructions exists (retired; rules are generated into .claude/rules/)")
        passed = False
    else:
        print("  PASS: .claude/instructions absent")

    prompts = ROOT / ".ai" / "prompts"
    has_prompts = prompts.is_dir() and any(p.is_file() for p in prompts.rglob("*"))
    for rel in (".claude/commands", ".github/prompts"):
        path = ROOT / rel
        if not has_prompts:
            if present(path):
                print(f"  FAIL: {rel} exists but .ai/prompts/ has no files (must be absent)")
                passed = False
            else:
                print(f"  PASS: {rel} absent (.ai/prompts/ has no files)")
        elif not sync._is_link(path):
            print(f"  FAIL: {rel} must be a link to .ai/prompts/ (it has files)")
            passed = False
        elif not path.exists() or not os.path.samefile(path, prompts):
            print(f"  FAIL: {rel} is a link but does not point to .ai/prompts/")
            passed = False
        else:
            print(f"  PASS: {rel} links to .ai/prompts/")
    return passed


def _frontmatter_skills(text: str) -> list:
    lines = text.split("\n")
    if not lines or lines[0].strip() != "---":
        return []
    for i in range(1, len(lines)):
        if lines[i].strip() == "---":
            block = lines[1:i]
            break
    else:
        return []
    skills: list = []
    for idx, line in enumerate(block):
        m = re.match(r"^skills\s*:\s*(.*)$", line)
        if not m:
            continue
        value = m.group(1).strip()
        if value:
            value = value.strip("[]")
            skills = [s.strip().strip("\"'") for s in value.split(",") if s.strip()]
        else:
            for follow in block[idx + 1:]:
                item = re.match(r"^\s+-\s*(.+)$", follow)
                if not item:
                    break
                skills.append(item.group(1).strip().strip("\"'"))
    return skills


def _body(text: str) -> str:
    lines = text.split("\n")
    if lines and lines[0].strip() == "---":
        for i in range(1, len(lines)):
            if lines[i].strip() == "---":
                return "\n".join(lines[i + 1:])
    return text


def test_agent_bodies() -> bool:
    print("\nTEST 4: Agent bodies reference their skills and critical rules")
    print("-" * 40)
    passed = True
    agents = sorted((ROOT / ".ai" / "agents").glob("*.md"))
    if not agents:
        print("  FAIL: no agents found")
        return False
    for agent in agents:
        text = _read(agent)
        body = _body(text)
        missing = [f".ai/skills/{s}/SKILL.md" for s in _frontmatter_skills(text)
                   if f".ai/skills/{s}/SKILL.md" not in body]
        if ".ai/reference/critical-rules.md" not in body:
            missing.append(".ai/reference/critical-rules.md")
        if missing:
            print(f"  FAIL: {agent.name} body does not reference: {missing}")
            passed = False
        else:
            print(f"  PASS: {agent.name}")
    return passed


def test_copilot_runtime_block() -> bool:
    print("\nTEST 5: copilot-instructions.md embeds copilot-runtime.md")
    print("-" * 40)
    instr = ROOT / ".github" / "copilot-instructions.md"
    runtime = ROOT / ".ai" / "reference" / "copilot-runtime.md"
    if not instr.is_file() or not runtime.is_file():
        print("  FAIL: copilot-instructions.md or copilot-runtime.md missing")
        return False
    text = _read(instr)
    if RUNTIME_START not in text or RUNTIME_END not in text:
        print("  FAIL: runtime start/end markers not found")
        return False
    start = text.index(RUNTIME_START) + len(RUNTIME_START)
    end = text.index(RUNTIME_END)
    if end < start:
        print("  FAIL: runtime markers out of order")
        return False
    if text[start:end].strip() != _read(runtime).strip():
        print("  FAIL: embedded block differs from .ai/reference/copilot-runtime.md")
        return False
    print("  PASS: embedded block equals .ai/reference/copilot-runtime.md")
    return True


def test_rules_drift() -> bool:
    print("\nTEST 6: .claude/rules matches the generator output")
    print("-" * 40)
    rules_dir = ROOT / ".claude" / "rules"
    if not rules_dir.is_dir():
        print("  SKIP: .claude/rules absent (claude tool not linked)")
        return True
    script = ROOT / ".ai" / "scripts" / "sync-platforms.py"
    spec = importlib.util.spec_from_file_location("sync_platforms_under_test", script)
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)

    suffix = ".instructions.md"
    sources = sorted((ROOT / ".ai" / "instructions").glob(f"*{suffix}"))
    if not sources:
        print("  FAIL: no .ai/instructions/*.instructions.md found")
        return False
    passed = True
    for source in sources:
        name = source.name[: -len(suffix)]
        target = rules_dir / source.name
        if not target.is_file():
            print(f"  FAIL: .claude/rules/{source.name} missing (run sync-platforms.py)")
            passed = False
            continue
        expected = module.render_rule(name, _read(source))
        if _read(target) != expected:
            print(f"  FAIL: .claude/rules/{source.name} drifted from generator (run sync-platforms.py)")
            passed = False
        else:
            print(f"  PASS: .claude/rules/{source.name}")
    return passed


def test_hooks_wiring() -> bool:
    print("\nTEST 7: Hook commands reference existing files; no .github/hooks/")
    print("-" * 40)
    passed = True
    settings_path = ROOT / ".claude" / "settings.json"
    try:
        settings = json.loads(settings_path.read_text(encoding="utf-8"))
    except (OSError, ValueError) as exc:
        print(f"  FAIL: cannot read .claude/settings.json: {exc}")
        return False
    pattern = re.compile(r"\.ai/scripts/[\w./-]+")
    count = 0
    for event, entries in (settings.get("hooks") or {}).items():
        for entry in entries if isinstance(entries, list) else []:
            for hook in entry.get("hooks", []) if isinstance(entry, dict) else []:
                for ref in pattern.findall(str(hook.get("command", ""))):
                    count += 1
                    if (ROOT / ref).is_file():
                        print(f"  PASS: {event} -> {ref}")
                    else:
                        print(f"  FAIL: {event} hook references missing file {ref}")
                        passed = False
    if count == 0:
        print("  FAIL: no hook command references .ai/scripts/")
        passed = False
    if (ROOT / ".github" / "hooks").exists():
        print("  FAIL: .github/hooks/ exists (hooks are wired only in .claude/settings.json)")
        passed = False
    else:
        print("  PASS: .github/hooks/ absent")
    return passed


def test_vscode_files() -> bool:
    print("\nTEST 8: .vscode tasks and settings")
    print("-" * 40)
    passed = True
    try:
        out = subprocess.run(["git", "ls-files", "--", ".vscode/tasks.json"], cwd=str(ROOT),
                             capture_output=True, text=True, timeout=30)
        tracked = ".vscode/tasks.json" in out.stdout.split()
    except (OSError, subprocess.SubprocessError):
        tracked = False
    if tracked:
        print("  PASS: .vscode/tasks.json is tracked by git")
    else:
        print("  FAIL: .vscode/tasks.json is not tracked by git")
        passed = False

    tasks_path = ROOT / ".vscode" / "tasks.json"
    if tasks_path.is_file():
        text = tasks_path.read_text(encoding="utf-8")
        if re.search(r'"runOn"\s*:\s*"folderOpen"', text):
            print("  FAIL: tasks.json has runOn: folderOpen")
            passed = False
        else:
            print("  PASS: tasks.json has no runOn: folderOpen")
    else:
        print("  FAIL: .vscode/tasks.json missing")
        passed = False

    settings_path = ROOT / ".vscode" / "settings.json"
    stext = settings_path.read_text(encoding="utf-8") if settings_path.is_file() else ""
    if re.search(r'"chat\.useAgentsMdFile"\s*:\s*false', stext):
        print('  PASS: settings.json sets "chat.useAgentsMdFile": false')
    else:
        print('  FAIL: settings.json must set "chat.useAgentsMdFile": false')
        passed = False

    for key in PINNED_SETTINGS:
        if re.search(r'"' + re.escape(key) + r'"\s*:\s*true', stext):
            print(f'  PASS: settings.json sets "{key}": true')
        else:
            print(f'  FAIL: settings.json must set "{key}": true')
            passed = False

    passed = _check_terminal_env(stext) and passed
    return passed


def _check_terminal_env(stext: str) -> bool:
    """Every VS Code terminal env object must carry the build env from .claude/settings.json."""
    try:
        claude_env = json.loads(_read(ROOT / ".claude" / "settings.json")).get("env") or {}
    except (OSError, ValueError) as exc:
        print(f"  FAIL: cannot read .claude/settings.json env: {exc}")
        return False
    if not claude_env:
        print("  FAIL: .claude/settings.json has no env block to mirror")
        return False
    try:
        vscode = json.loads(stext)
    except ValueError as exc:
        print(f"  FAIL: .vscode/settings.json is not valid JSON: {exc}")
        return False
    passed = True
    for platform in ("windows", "linux", "osx"):
        name = f"terminal.integrated.env.{platform}"
        env = vscode.get(name)
        if not isinstance(env, dict):
            print(f"  FAIL: {name} missing")
            passed = False
            continue
        mismatched = [k for k, v in claude_env.items() if env.get(k) != v]
        if mismatched:
            print(f"  FAIL: {name} does not mirror .claude/settings.json env: {mismatched}")
            passed = False
        else:
            print(f"  PASS: {name} mirrors .claude/settings.json env ({len(claude_env)} keys)")
    return passed


def _frontmatter(text: str) -> str:
    lines = text.split("\n")
    if not lines or lines[0].strip() != "---":
        return ""
    for i in range(1, len(lines)):
        if lines[i].strip() == "---":
            return "\n".join(lines[1:i])
    return ""


def _frontmatter_list(block: str, key: str) -> list:
    """Items of a `key:` list in flow form `[a, b]`, block form `- a`, or comma form `a, b`."""
    lines = block.split("\n")
    for idx, line in enumerate(lines):
        m = re.match(rf"^{key}\s*:\s*(.*)$", line)
        if not m:
            continue
        value = m.group(1).strip()
        if value:
            return [s.strip().strip("\"'") for s in value.strip("[]").split(",") if s.strip()]
        items = []
        for follow in lines[idx + 1:]:
            item = re.match(r"^\s+-\s*(.+)$", follow)
            if not item:
                break
            items.append(item.group(1).strip().strip("\"'"))
        return items
    return []


def _frontmatter_value(block: str, key: str) -> str:
    m = re.search(rf"^{key}\s*:\s*(.*)$", block, re.MULTILINE)
    return m.group(1).strip() if m else ""


def test_orchestrator_agent() -> bool:
    print("\nTEST 9: orchestrator.md is the Copilot coordinator, not a delegation target")
    print("-" * 40)
    path = ROOT / ".ai" / "agents" / "orchestrator.md"
    if not path.is_file():
        print("  FAIL: .ai/agents/orchestrator.md missing")
        return False
    block = _frontmatter(_read(path))
    passed = True

    if _frontmatter_value(block, "disable-model-invocation").lower() == "true":
        print("  PASS: disable-model-invocation: true")
    else:
        print("  FAIL: orchestrator.md must set disable-model-invocation: true")
        passed = False

    others = sorted(p.stem for p in (ROOT / ".ai" / "agents").glob("*.md") if p.stem != "orchestrator")
    listed = _frontmatter_list(block, "agents")
    if sorted(listed) == others:
        print(f"  PASS: agents lists exactly the {len(others)} other agents")
    else:
        missing = sorted(set(others) - set(listed))
        extra = sorted(set(listed) - set(others))
        print(f"  FAIL: agents mismatch — missing {missing}, unexpected {extra}")
        passed = False

    tools = _frontmatter_list(block, "tools")
    if "Task" in tools or "agent" in tools:
        print("  PASS: tools includes the delegation tool (Task / agent)")
    else:
        print("  FAIL: tools must include Task (or agent) so Copilot can delegate")
        passed = False
    return passed


VSCODE_FRAGMENT_DIR = ROOT / ".ai" / "reference" / "templates" / "vscode"
MERGE_RULES = ROOT / ".ai" / "reference" / "templates" / "merge-rules.json"
# Keys that only make sense for working on the template itself; never shipped to projects.
TEMPLATE_DEV_ONLY_KEYS = ("python.testing.", "PIP_CONFIG_FILE")


def _load_json(path: Path, label: str):
    try:
        return json.loads(_read(path))
    except (OSError, ValueError) as exc:
        print(f"  FAIL: {label} is not readable JSON: {exc}")
        return None


def test_vscode_fragments() -> bool:
    print("\nTEST 10: VS Code fragments, merge rules and the template's own .vscode files")
    print("-" * 40)
    passed = True
    settings_frag_path = VSCODE_FRAGMENT_DIR / "settings.json"
    tasks_frag_path = VSCODE_FRAGMENT_DIR / "tasks.json"
    settings_frag = _load_json(settings_frag_path, "vscode/settings.json fragment")
    tasks_frag = _load_json(tasks_frag_path, "vscode/tasks.json fragment")
    rules = _load_json(MERGE_RULES, "merge-rules.json")
    if settings_frag is None or tasks_frag is None or rules is None:
        return False
    print("  PASS: fragments and merge-rules.json are valid JSON")

    leaked = [k for k in settings_frag
              if any(k == d or k.startswith(d) for d in TEMPLATE_DEV_ONLY_KEYS)]
    if leaked:
        print(f"  FAIL: template-only keys must not ship in the fragment: {leaked}")
        passed = False
    else:
        print("  PASS: fragment has no template-only keys")

    own_path = ROOT / ".vscode" / "settings.json"
    own = _load_json(own_path, ".vscode/settings.json")
    if own is None:
        return False
    def _contains(have, want) -> bool:
        # Nested objects may carry extra (dev-only) keys; every fragment key must match.
        if isinstance(want, dict):
            return isinstance(have, dict) and all(_contains(have.get(k), v) for k, v in want.items()) \
                and all(k in have for k in want)
        return have == want

    for key, value in settings_frag.items():
        if not _contains(own.get(key), value):
            print(f"  FAIL: .vscode/settings.json must contain {key!r}: {value!r}")
            passed = False
    if passed:
        print(f"  PASS: .vscode/settings.json contains all {len(settings_frag)} fragment keys with equal values")

    own_tasks = _load_json(ROOT / ".vscode" / "tasks.json", ".vscode/tasks.json")
    if own_tasks is None:
        return False
    own_labels = {t.get("label") for t in own_tasks.get("tasks", []) if isinstance(t, dict)}
    frag_labels = [t.get("label") for t in tasks_frag.get("tasks", [])]
    missing_labels = [label for label in frag_labels if label not in own_labels]
    if missing_labels:
        print(f"  FAIL: .vscode/tasks.json lacks fragment task labels: {missing_labels}")
        passed = False
    else:
        print(f"  PASS: .vscode/tasks.json contains all {len(frag_labels)} fragment task labels")

    pinned = rules.get("vscode", {}).get("pinnedSettings", [])
    not_in_fragment = [k for k in pinned if k not in settings_frag]
    if not pinned or not_in_fragment:
        print(f"  FAIL: merge-rules pinnedSettings must be non-empty and all in the fragment; missing {not_in_fragment}")
        passed = False
    else:
        print(f"  PASS: all {len(pinned)} pinned settings exist in the fragment")

    legacy = rules.get("claude", {}).get("legacyHookCommands")
    if not isinstance(legacy, list) or not legacy:
        print("  FAIL: merge-rules claude.legacyHookCommands must be a non-empty list")
        passed = False
    else:
        print(f"  PASS: {len(legacy)} legacy hook commands listed")
    return passed


def main():
    print("=" * 60)
    print("  Adapter Layout Validation")
    print("=" * 60)
    results = [
        test_no_prompt_variables(),
        test_prompts_dir_absent(),
        test_retired_adapters(),
        test_agent_bodies(),
        test_copilot_runtime_block(),
        test_rules_drift(),
        test_hooks_wiring(),
        test_vscode_files(),
        test_orchestrator_agent(),
        test_vscode_fragments(),
    ]
    print()
    print("=" * 60)
    print(f"  {sum(results)}/{len(results)} tests passed")
    print("=" * 60)
    if all(results):
        print("  ALL ADAPTER TESTS PASSED")
        return 0
    print("  SOME ADAPTER TESTS FAILED")
    return 1


if __name__ == "__main__":
    sys.exit(main())
