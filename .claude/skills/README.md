# Claude Skills Directory

This directory contains all available Claude skills in proper Claude Skills format.

## Quick Reference

### All Available Skills

| Skill | Description | Special Config |
|-------|-------------|----------------|
| **dotnet-engineer** | .NET/C#/Blazor/MAUI development | - |
| **unit-tester** | MSTest unit testing | - |
| **playwright-tester** | Playwright E2E and UI testing | `allowed-tools: npx playwright *` |
| **code-reviewer** | Code quality and architecture review | - |
| **technical-writer** | Documentation specialist | - |
| **architect** | Solution architecture and system design | - |
| **database-migration** | EF Core migrations and database management | `allowed-tools: dotnet ef *` |
| **api-security** | API security specialist | - |
| **performance-engineer** | Performance optimization | - |
| **devops-engineer** | DevOps and CI/CD specialist | `disable-model-invocation: true` |
| **integration-specialist** | External API integration | - |
| **blazor-specialist** | Blazor UI development | - |
| **maui-specialist** | MAUI mobile development | - |
| **software-security** | Application security specialist | - |
| **security** | Security architect and coordinator | - |

## Invocation Examples

### Development
```
/dotnet-engineer           # Implement CQRS handlers, API endpoints
/blazor-specialist         # Build Blazor components
/maui-specialist          # Create MAUI mobile apps
```

### Testing
```
/unit-tester              # Write MSTest unit tests
/playwright-tester        # Create E2E tests
```

### Database
```
/database-migration       # Create/apply EF Core migrations
```

### Code Quality
```
/code-reviewer           # Review code quality and architecture
/performance-engineer    # Optimize performance
```

### Security
```
/api-security           # API security implementation
/software-security      # Application security
/security              # Overall security strategy
```

### Documentation & Architecture
```
/technical-writer      # Create documentation
/architect            # System architecture and design
```

### DevOps & Integration
```
/devops-engineer          # CI/CD, containers, infrastructure (user-only)
/integration-specialist   # External API integration
```

## Skill Structure

Each skill follows this structure:

```
skill-name/
└── SKILL.md              # Skill definition with YAML frontmatter
```

### SKILL.md Format

```markdown
---
name: skill-name
description: Clear description of what the skill does and when to use it
allowed-tools: tool-pattern-if-needed
disable-model-invocation: true-if-user-only
---

# Skill Content

[Rest of the skill documentation...]
```

## Skills with Special Permissions

### Skills with Tool Access
These skills have access to specific tools:

- **playwright-tester**: Can run `npx playwright *` commands
- **database-migration**: Can run `dotnet ef *` commands

### User-Invocable Only
These skills can ONLY be invoked by the user (not automatically by Claude):

- **devops-engineer**: Deployments have side effects and require explicit user action

## When Claude Automatically Loads Skills

Claude will automatically select and load the appropriate skill based on your request:

| Your Request | Skill Loaded |
|--------------|--------------|
| "Implement a CRUD handler for Budget entity" | `dotnet-engineer` |
| "Write unit tests for BudgetHandler" | `unit-tester` |
| "Create a database migration for Budget table" | `database-migration` |
| "Review this code for security issues" | `code-reviewer` |
| "Build a Blazor component for budget display" | `blazor-specialist` |
| "Create E2E tests for the budget page" | `playwright-tester` |
| "Design the system architecture" | `architect` |
| "Write API documentation" | `technical-writer` |
| "Optimize database queries" | `performance-engineer` |
| "Implement OAuth2 authentication" | `api-security` |
| "Create a mobile app with MAUI" | `maui-specialist` |
| "Integrate with Stripe API" | `integration-specialist` |
| "Deploy to Azure" | `devops-engineer` (requires user invocation) |

## Directory Convention

All skills follow this naming convention:
- **Directory name**: `lowercase-with-hyphens`
- **Skill name in frontmatter**: Same as directory name
- **File name**: Always `SKILL.md`

## Adding New Skills

To add a new skill:

1. Create a new directory: `.claude/skills/new-skill-name/`
2. Create `SKILL.md` with proper YAML frontmatter:
   ```yaml
   ---
   name: new-skill-name
   description: Clear description of what the skill does and when to use it
   ---
   ```
3. Add the skill content below the frontmatter
4. Update this README with the new skill

## More Information

- **Conversion Summary**: See `CONVERSION-SUMMARY.md` for details about the skill conversion process
- **Claude Code Documentation**: Refer to main CLAUDE.md for overall development guidelines
