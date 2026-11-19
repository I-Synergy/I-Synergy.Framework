#!/usr/bin/env bash
# Portable Python launcher for the .ai/ hook scripts.
#
# Claude Code runs hook commands through bash (Git Bash on Windows). On Windows,
# `python3` can be the Microsoft Store stub, which exists but fails to run, so
# `python3` is used only when it actually executes. Otherwise `python` is used.
#
# Usage: bash .ai/scripts/hooks/py.sh <script.py> [args...]   (stdin is passed through)

PY=""
if python3 -c '' >/dev/null 2>&1; then
    PY=python3
elif python -c '' >/dev/null 2>&1; then
    PY=python
fi

if [[ -z "$PY" ]]; then
    echo "py.sh: no working python3 or python on PATH" >&2
    exit 127
fi

exec "$PY" "$@"
