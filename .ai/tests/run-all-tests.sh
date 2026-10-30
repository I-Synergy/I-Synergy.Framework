#!/bin/bash
# Master test runner for Claude template validation

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FAILED=0
TOTAL=0
SEPARATOR="========================================="

echo "$SEPARATOR"
echo "  Claude Template Validation Suite"
echo "$SEPARATOR"
echo ""
echo "Running comprehensive validation tests..."
echo ""

# Pre-step: ensure folder-level junctions/symlinks exist
# (CI checkouts have real directories; this converts them)
echo "--- Pre-step: Syncing platform junctions ---"
python3 "$SCRIPT_DIR/../scripts/sync-platforms.py" 2>&1
echo ""
echo "--- Validation tests follow ---"
echo ""

# Function to run a test and track results
run_test() {
    local test_name="$1"
    local test_command="$2"

    echo "$SEPARATOR"
    echo "TEST: $test_name"
    echo "$SEPARATOR"

    TOTAL=$((TOTAL + 1))

    if eval "$test_command"; then
        echo ""
        echo "✅ $test_name PASSED"
    else
        echo ""
        echo "❌ $test_name FAILED"
        FAILED=$((FAILED + 1))
    fi

    echo ""
}

# Run all test suites
run_test "1. Directory Structure" "bash '$SCRIPT_DIR/validate-structure.sh'"
run_test "2. YAML Frontmatter" "python3 '$SCRIPT_DIR/validate-skills.py'"
run_test "3. File References" "bash '$SCRIPT_DIR/validate-references.sh'"
run_test "4. Content Quality" "python3 '$SCRIPT_DIR/validate-content.py'"
run_test "5. Token Consistency" "bash '$SCRIPT_DIR/validate-tokens.sh'"
run_test "6. AGENTS.md References" "python3 '$SCRIPT_DIR/validate-agents-md.py'"
run_test "7. Settings & Structure" "python3 '$SCRIPT_DIR/validate-settings.py'"
run_test "8. Trigger Lines" "python3 '$SCRIPT_DIR/validate-triggers.py'"
run_test "9. Prose To Skills" "python3 '$SCRIPT_DIR/validate-prose.py'"
run_test "10. Copilot Integration" "python3 '$SCRIPT_DIR/validate-copilot.py'"
run_test "11. Integration Smoke Tests" "python3 '$SCRIPT_DIR/smoke-test.py'"
run_test "12. Traceability" "python3 '$SCRIPT_DIR/validate-traceability.py'"
run_test "13. Standards" "python3 '$SCRIPT_DIR/validate-standards.py'"
run_test "14. Release Versioning" "python3 '$SCRIPT_DIR/validate-release.py'"

# Final Summary
echo "$SEPARATOR"
echo "  VALIDATION SUITE SUMMARY"
echo "$SEPARATOR"
echo ""
echo "Total test suites: $TOTAL"
echo "Passed: $((TOTAL - FAILED))"
echo "Failed: $FAILED"
echo ""

if [[ $FAILED -eq 0 ]]; then
    echo "✅✅✅ ALL TESTS PASSED ✅✅✅"
    echo ""
    echo "The Claude template is valid and ready to use!"
    echo "$SEPARATOR"
    exit 0
else
    echo "❌❌❌ SOME TESTS FAILED ❌❌❌"
    echo ""
    echo "Please review the errors above and fix them."
    echo "$SEPARATOR"
    exit 1
fi
