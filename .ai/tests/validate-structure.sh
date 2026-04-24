#!/bin/bash
# Validate directory structure of Claude template

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEMPLATE_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
FAILED=0
PASSED=0

echo "========================================="
echo "  Template Structure Validation"
echo "========================================="
echo ""

# Check required directories
echo "1. Checking required directories..."
required_dirs=(
    ".ai/skills"
    ".ai/patterns"
    ".ai/reference"
    ".ai/reference/templates"
    ".ai/checklists"
    ".ai/project"
    ".ai/tests"
    ".ai/scripts"
)

for dir in "${required_dirs[@]}"; do
    if [ ! -d "$TEMPLATE_ROOT/$dir" ]; then
        echo "❌ Missing directory: $dir"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Directory exists: $dir"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check all skills have SKILL.md (dynamic discovery)
echo "2. Checking skill SKILL.md files..."
skill_count=0
missing_count=0
for skill_dir in "$TEMPLATE_ROOT/.ai/skills"/*/; do
    if [ -d "$skill_dir" ]; then
        skill_name=$(basename "$skill_dir")
        skill_count=$((skill_count + 1))
        if [ ! -f "$skill_dir/SKILL.md" ]; then
            echo "❌ Missing SKILL.md for: $skill_name"
            FAILED=$((FAILED + 1))
            missing_count=$((missing_count + 1))
        fi
    fi
done
if [ $missing_count -eq 0 ] && [ $skill_count -gt 0 ]; then
    echo "✅ All $skill_count skills have SKILL.md"
    PASSED=$((PASSED + 1))
fi

echo ""

# Check pattern files
echo "3. Checking pattern files..."
patterns=(
    "cqrs-patterns.md"
    "api-patterns.md"
    "testing-patterns.md"
    "mvvm.md"
    "microservices.md"
    "service-oriented-architecture.md"
    "object-oriented-programming.md"
    "test-driven-development.md"
)

for pattern in "${patterns[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/.ai/patterns/$pattern" ]; then
        echo "❌ Missing pattern: $pattern"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Pattern exists: $pattern"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check reference files
echo "4. Checking reference files..."
reference_files=(
    "critical-rules.md"
    "forbidden-tech.md"
    "glossary.md"
    "naming-conventions.md"
    "tokens.md"
)

for ref in "${reference_files[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/.ai/reference/$ref" ]; then
        echo "❌ Missing reference: $ref"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Reference exists: $ref"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check project files
echo "5. Checking project files..."
project_files=(
    "architecture.md"
    "domains.md"
    "preferences.md"
    "tech-stack.md"
)

for proj in "${project_files[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/.ai/project/$proj" ]; then
        echo "❌ Missing project file: $proj"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Project file exists: $proj"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check checklist files
echo "6. Checking checklist files..."
if [ ! -f "$TEMPLATE_ROOT/.ai/checklists/pre-submission.md" ]; then
    echo "❌ Missing pre-submission checklist"
    FAILED=$((FAILED + 1))
else
    echo "✅ Pre-submission checklist exists"
    PASSED=$((PASSED + 1))
fi

echo ""

# Check template files
echo "7. Checking template files..."
template_files=(
    "command-handler.cs.txt"
    "query-handler.cs.txt"
    "endpoint.cs.txt"
    "test-class.cs.txt"
    "feature-file.feature.txt"
)

for tmpl in "${template_files[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/.ai/reference/templates/$tmpl" ]; then
        echo "❌ Missing template: $tmpl"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Template exists: $tmpl"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check main documentation files
echo "8. Checking main documentation files..."
main_files=(
    "CLAUDE.md"
    "README.md"
    ".ai/session-context.md"
)

for main in "${main_files[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/$main" ]; then
        echo "❌ Missing main file: $main"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Main file exists: $main"
        PASSED=$((PASSED + 1))
    fi
done

echo ""

# Check scripts
echo "9. Checking scripts..."
scripts=(
    ".ai/scripts/sync-skills.py"
    ".ai/scripts/upgrade-template.py"
)

for script in "${scripts[@]}"; do
    if [ ! -f "$TEMPLATE_ROOT/$script" ]; then
        echo "❌ Missing script: $script"
        FAILED=$((FAILED + 1))
    else
        echo "✅ Script exists: $script"
        PASSED=$((PASSED + 1))
    fi
done

echo ""
echo "========================================="
echo "Structure Validation Summary"
echo "========================================="
echo "✅ Passed: $PASSED"
echo "❌ Failed: $FAILED"
echo "========================================="

if [ $FAILED -eq 0 ]; then
    echo "✅ ALL STRUCTURE TESTS PASSED"
    exit 0
else
    echo "❌ SOME STRUCTURE TESTS FAILED"
    exit 1
fi
