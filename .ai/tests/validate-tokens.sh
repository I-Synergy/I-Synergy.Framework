#!/bin/bash
# Validate token consistency across template files

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEMPLATE_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
FAILED=0
PASSED=0

echo "========================================="
echo "  Token Consistency Validation"
echo "========================================="
echo ""

# Define expected tokens
TOKENS=(
    "{ApplicationName}"
    "{Domain}"
    "{Entity}"
    "{entity}"
    "{entities}"
)

# Function to check if file uses tokens properly
check_token_usage() {
    local file="$1"
    local file_name=$(basename "$file")

    if [ ! -f "$file" ]; then
        return
    fi

    # Check if file should have tokens (templates and some reference files)
    if [[ "$file" == *"/templates/"* ]] || [[ "$file" == *"SKILL.md"* ]]; then
        # Count token usage
        token_count=0
        for token in "${TOKENS[@]}"; do
            count=$(grep -o "$token" "$file" 2>/dev/null | wc -l)
            token_count=$((token_count + count))
        done

        if [ $token_count -gt 0 ]; then
            echo "✅ $file_name uses tokens ($token_count occurrences)"
            PASSED=$((PASSED + 1))
        else
            # Only warn for template files
            if [[ "$file" == *"/templates/"* ]]; then
                echo "⚠️  $file_name has no tokens (expected for templates)"
            fi
        fi
    fi
}

# Function to check for hardcoded project names
check_hardcoded_names() {
    local file="$1"
    local file_name=$(basename "$file")

    if [ ! -f "$file" ]; then
        return
    fi

    # Skip if it's the tokens definition file or project files
    if [[ "$file" == *"tokens.md"* ]] || [[ "$file" == *"project/"* ]]; then
        return
    fi

    # Check for common hardcoded names (examples that shouldn't be in templates)
    hardcoded_patterns=(
        "BudgetTracker"
        "MyApplication"
        "SampleApp"
    )

    for pattern in "${hardcoded_patterns[@]}"; do
        if grep -q "$pattern" "$file" 2>/dev/null; then
            echo "⚠️  $file_name contains hardcoded name: $pattern"
            # Don't fail, just warn
        fi
    done
}

echo "1. Checking template files for token usage..."
if [ -d "$TEMPLATE_ROOT/.ai/reference/templates" ]; then
    for template in "$TEMPLATE_ROOT/.ai/reference/templates"/*; do
        check_token_usage "$template"
    done
else
    echo "❌ Templates directory not found"
    FAILED=$((FAILED + 1))
fi

echo ""

echo "2. Checking skills for token usage..."
if [ -d "$TEMPLATE_ROOT/.ai/skills" ]; then
    for skill_dir in "$TEMPLATE_ROOT/.ai/skills"/*; do
        if [ -d "$skill_dir" ]; then
            if [ -f "$skill_dir/SKILL.md" ]; then
                check_token_usage "$skill_dir/SKILL.md"
            fi
        fi
    done
else
    echo "❌ Skills directory not found"
    FAILED=$((FAILED + 1))
fi

echo ""

echo "3. Checking for hardcoded project names..."
# Check templates
if [ -d "$TEMPLATE_ROOT/.ai/reference/templates" ]; then
    for template in "$TEMPLATE_ROOT/.ai/reference/templates"/*; do
        check_hardcoded_names "$template"
    done
fi

# Check patterns
if [ -d "$TEMPLATE_ROOT/.ai/patterns" ]; then
    for pattern in "$TEMPLATE_ROOT/.ai/patterns"/*.md; do
        check_hardcoded_names "$pattern"
    done
fi

echo ""

echo "4. Verifying token definitions exist..."
if [ -f "$TEMPLATE_ROOT/.ai/reference/tokens.md" ]; then
    echo "✅ Token definitions file exists"
    PASSED=$((PASSED + 1))

    # Check that all expected tokens are defined
    for token in "${TOKENS[@]}"; do
        # Search for token in backticks (as it appears in markdown table)
        if grep -q "\`$token\`" "$TEMPLATE_ROOT/.ai/reference/tokens.md"; then
            echo "✅ Token $token is defined"
            PASSED=$((PASSED + 1))
        else
            echo "❌ Token $token not found in definitions"
            FAILED=$((FAILED + 1))
        fi
    done
else
    echo "❌ Token definitions file not found"
    FAILED=$((FAILED + 1))
fi

echo ""

echo "5. Checking token usage in CLAUDE.md..."
if [ -f "$TEMPLATE_ROOT/CLAUDE.md" ]; then
    token_count=0
    for token in "${TOKENS[@]}"; do
        count=$(grep -o "$token" "$TEMPLATE_ROOT/CLAUDE.md" 2>/dev/null | wc -l)
        token_count=$((token_count + count))
    done

    if [ $token_count -gt 0 ]; then
        echo "✅ CLAUDE.md uses tokens ($token_count occurrences)"
        PASSED=$((PASSED + 1))
    else
        echo "⚠️  CLAUDE.md has no token placeholders"
    fi
else
    echo "❌ CLAUDE.md not found"
    FAILED=$((FAILED + 1))
fi

echo ""
echo "========================================="
echo "Token Validation Summary"
echo "========================================="
echo "✅ Passed: $PASSED"
echo "❌ Failed: $FAILED"
echo "========================================="

if [ $FAILED -eq 0 ]; then
    echo "✅ ALL TOKEN TESTS PASSED"
    exit 0
else
    echo "❌ SOME TOKEN TESTS FAILED"
    exit 1
fi
