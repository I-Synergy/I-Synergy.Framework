# Fix all non-Mathematics build warnings and the SonarCloud analysis error

Status: DONE
Branch: `fix/build-warnings-and-sonar`
Source: GitHub Actions run 42532046072, job 128895030909 (commit `289cea6de`)

## Findings

1. **SonarCloud error** (step *End SonarCloud analysis*, exit 1):

   ```
   ERROR: Invalid regular expression 'src/ISynergy.Framework.Mathematics/**'
   for setting 'sonar.issue.ignore.allfile'
   ```

   The value is not in this repository. The SonarCloud settings API returns it with
   `"parentOrigin":"INSTANCE"`, i.e. an organization-level *Ignore All Issues on Files*
   entry that holds a glob. Scanner 12.0 validates it as a regular expression and aborts.

2. **Warnings**: the Release build produced 2 562 warning lines, 544 distinct
   (file, line, column, rule) locations. 272 are in `src/ISynergy.Framework.Mathematics`
   (the Math.NET port, already ignored in SonarCloud by the org setting); 272 are outside
   it. GitHub only shows 10 annotations per step, which is why the run page shows 12.

## Scope (agreed with the user)

- Fix all 272 non-Mathematics warnings in `src/`, `samples/` and `tools/`.
- Suppress the Mathematics port's diagnostics in its own csproj (documented), not by
  editing ported code.
- Fix the SonarCloud failure and the two GitHub Actions deprecation warnings.

## Steps

- [x] Fix the SonarCloud `sonar.issue.ignore.allfile` failure in `ci.yml`
- [x] Bump deprecated GitHub Actions (Node 20 -> Node 24 majors)
- [x] Suppress the port's analyzer diagnostics in the Mathematics csproj
- [x] Silence analyzers for generated `obj/` sources (Strada-generated Razor file)
- [x] Fix Core / CQRS / Automations / Geography / IO / MessageBus warnings
- [x] Fix Mvvm / UI / UI.Maui / UI.WinUI / UI.WPF / UI.Blazor warnings
- [x] Fix Sample.WinUI warnings
- [x] Fix remaining sample / performance / tools warnings
- [x] Build the solution in Release with no warnings in the touched areas
- [x] Run the test suite
- [x] Review the final diff and update `.ai/progress/`
