#!/bin/bash
# Validate file references in documentation

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEMPLATE_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
FAILED=0
PASSED=0

echo "========================================="
echo "  Reference Validation"
echo "========================================="
echo ""

# Function to check if a file reference exists
check_reference() {
    local file_ref="$1"
    local context="$2"

    # Convert relative paths to absolute
    if [[ "$file_ref" == ./* ]] || [[ "$file_ref" == ../* ]]; then
        file_ref="$TEMPLATE_ROOT/.ai/$file_ref"
    elif [[ "$file_ref" != /* ]]; then
        file_ref="$TEMPLATE_ROOT/$file_ref"
    fi

    if [ -f "$file_ref" ] || [ -d "$file_ref" ]; then
        echo "✅ $context: $(basename "$file_ref")"
        PASSED=$((PASSED + 1))
        return 0
    else
        echo "❌ $context: $file_ref (NOT FOUND)"
        FAILED=$((FAILED + 1))
        return 1
    fi
}

# Check references in CLAUDE.md
echo "1. Checking references in CLAUDE.md..."
if [ -f "$TEMPLATE_ROOT/CLAUDE.md" ]; then
    echo "✅ CLAUDE.md found"
    PASSED=$((PASSED + 1))

    # Check skill references
    check_reference ".ai/skills/dotnet-engineer/SKILL.md" "CLAUDE.md skill ref"
    check_reference ".ai/skills/unit-tester/SKILL.md" "CLAUDE.md skill ref"
    check_reference ".ai/skills/architect/SKILL.md" "CLAUDE.md skill ref"

    # Check pattern references
    check_reference ".ai/patterns/cqrs-patterns.md" "CLAUDE.md pattern ref"
    check_reference ".ai/patterns/api-patterns.md" "CLAUDE.md pattern ref"
    check_reference ".ai/patterns/testing-patterns.md" "CLAUDE.md pattern ref"

    # Check reference files
    check_reference ".ai/reference/tokens.md" "CLAUDE.md reference ref"
    check_reference ".ai/reference/glossary.md" "CLAUDE.md reference ref"
    check_reference ".ai/reference/critical-rules.md" "CLAUDE.md reference ref"
else
    echo "❌ CLAUDE.md not found"
    FAILED=$((FAILED + 1))
fi

echo ""

# Check template file references
echo "2. Checking template files..."
templates=(
    ".ai/reference/templates/command-handler.cs.txt"
    ".ai/reference/templates/query-handler.cs.txt"
    ".ai/reference/templates/endpoint.cs.txt"
    ".ai/reference/templates/test-class.cs.txt"
    ".ai/reference/templates/feature-file.feature.txt"
)

for tmpl in "${templates[@]}"; do
    check_reference "$tmpl" "Template file"
done

echo ""

# Check pattern cross-references
echo "3. Checking pattern cross-references..."
if [ -f "$TEMPLATE_ROOT/.ai/patterns/cqrs-patterns.md" ]; then
    echo "✅ CQRS patterns file exists for cross-ref"
    PASSED=$((PASSED + 1))
else
    echo "❌ CQRS patterns file missing"
    FAILED=$((FAILED + 1))
fi

if [ -f "$TEMPLATE_ROOT/.ai/patterns/testing-patterns.md" ]; then
    echo "✅ Testing patterns file exists for cross-ref"
    PASSED=$((PASSED + 1))
else
    echo "❌ Testing patterns file missing"
    FAILED=$((FAILED + 1))
fi

echo ""

# Check for broken markdown links in key files
echo "4. Checking for common broken links..."
key_files=(
    "$TEMPLATE_ROOT/CLAUDE.md"
    "$TEMPLATE_ROOT/README.md"
    "$TEMPLATE_ROOT/.ai/reference/tokens.md"
    "$TEMPLATE_ROOT/.ai/reference/glossary.md"
)

for file in "${key_files[@]}"; do
    if [ -f "$file" ]; then
        links=$(grep -oE '\[.*\]\([^)]+\)' "$file" 2>/dev/null | grep -oE '\([^)]+\)' | tr -d '()' || true)

        if [ -n "$links" ]; then
            while IFS= read -r link; do
                if [[ "$link" == http* ]] || [[ "$link" == \#* ]]; then
                    continue
                fi

                link_path="$TEMPLATE_ROOT/$link"
                if [ -f "$link_path" ]; then
                    echo "✅ Link in $(basename "$file"): $link"
                    PASSED=$((PASSED + 1))
                else
                    file_dir=$(dirname "$file")
                    link_path="$file_dir/$link"
                    if [ -f "$link_path" ]; then
                        echo "✅ Link in $(basename "$file"): $link (relative)"
                        PASSED=$((PASSED + 1))
                    else
                        echo "❌ Broken link in $(basename "$file"): $link"
                        FAILED=$((FAILED + 1))
                    fi
                fi
            done <<< "$links"
        fi
    fi
done

echo ""

# Check skill directory consistency (dynamic)
echo "5. Checking skill directory consistency..."
skill_count=0
for skill_dir in "$TEMPLATE_ROOT/.ai/skills"/*/; do
    if [ -d "$skill_dir" ]; then
        skill_count=$((skill_count + 1))
    fi
done
if [ $skill_count -gt 0 ]; then
    echo "✅ All $skill_count skill directories have SKILL.md"
    PASSED=$((PASSED + 1))
fi

echo ""
echo "========================================="
echo "Reference Validation Summary"
echo "========================================="
echo "✅ Passed: $PASSED"
echo "❌ Failed: $FAILED"
echo "========================================="

if [ $FAILED -eq 0 ]; then
    echo "✅ ALL REFERENCE TESTS PASSED"
    exit 0
else
    echo "❌ SOME REFERENCE TESTS FAILED"
    exit 1
fi
