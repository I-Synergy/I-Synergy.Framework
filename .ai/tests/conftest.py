"""Shared fixtures for the Claude template test suite."""

import sys
from pathlib import Path

import pytest

TESTS_DIR = Path(__file__).parent
TEMPLATE_ROOT = TESTS_DIR.parent.parent


@pytest.fixture(scope="session")
def template_root() -> Path:
    return TEMPLATE_ROOT


@pytest.fixture(scope="session")
def skills_dir(template_root) -> Path:
    return template_root / ".ai" / "skills"
