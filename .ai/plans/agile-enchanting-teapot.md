# Consolidate Resources & Localization into ISynergy.Framework.Globalization

**Status:** PLAN (Steps 1, 3, 5A completed manually by user)
**Started:** 2026-06-03

## Context

24 .resx files spread across 6 projects. Consolidate into the existing `ISynergy.Framework.Globalization` project which already contains the localization services.

## Architecture

```
ISynergy.Framework.Globalization   ← leaf project (no framework deps)
  │   Properties/Resources.resx + satellite locales
  │   ILanguageService, LanguageService, etc.
  │
  ↑ referenced ONLY by Core
  │
ISynergy.Framework.Core  ← Mvvm ← UI ← platforms
                        ← Geography, Physics, AspNetCore, etc.
```

## Already Done (by user)

- ✅ Step 1: Globalization csproj fixed (RootNamespace, circular deps, resx entries)
- ✅ Step 3: Core → Globalization reference added; LanguageService typeof updated
- ✅ Step 5A: DI registrations in 6 files updated to single `typeof(ISynergy.Framework.Globalization.Properties.Resources)`

## Remaining Steps

### Step 2: Create merged Resources.resx in Globalization (Subagent: developer)

Merge all .resx files from source projects into Globalization's `Properties/`:

| Source | Keys | Locales |
|--------|------|---------|
| Core | 16 | en/de/fr/nl/es/ar |
| Mvvm | 7 | en/de/fr/nl/es/ar (`ThisItem` dup with UI → skip) |
| UI | 92 | en/de/fr/nl/es/ar |
| AspNetCore.Blazor | 89 | en/de/fr/nl (DUPLICATE of UI → skip entirely) |
| Geography | 21 | en only |
| Physics | 56 | en only |

Create 7 resx files in `src/ISynergy.Framework.Globalization/Properties/`:
- `Resources.resx` — ~192 English keys (all merged)
- `Resources.de.resx` — ALL ~192 keys translated to German
- `Resources.fr.resx` — ALL ~192 keys translated to French
- `Resources.nl.resx` — ALL ~192 keys translated to Dutch
- `Resources.es.resx` — ALL ~192 keys translated to Spanish
- `Resources.ar.resx` — ALL ~192 keys translated to Arabic

**Geography (21 keys)** and **Physics (56 keys)** need translations for all 5 non-English locales. These are currently English-only in their source projects.

### Step 4: Clean up source projects (Subagent: developer)

For Core, Mvvm, UI, AspNetCore.Blazor, Geography, Physics:
- Delete all `.resx` files from `Properties/`
- Delete `Resources.Designer.cs` from `Properties/`
- Remove `<Compile Update>` and `<EmbeddedResource>` entries for Resources from csproj

### Step 5B: Update direct `Properties.Resources.KEY` references (Subagent: developer)

Replace with `ISynergy.Framework.Globalization.Properties.Resources.KEY` in:
- 8 Geography files (UtmGrid.cs, UtmProjection.cs, Angle.cs, EuclidianCoordinate.cs, GeodeticCalculator.cs, GlobalMesh.cs, GlobalMercatorProjection.cs, MercatorProjection.cs)
- 1 file: `UI.WinUI/Hosting/WindowsAppSdkHost.cs`

### Step 5C: NO CHANGES for `ILanguageService.GetString("key")` consumers

~30 files resolve by string name — decoupled from type location.

### Step 6: Verify

- `dotnet build` solution-wide — zero errors
- Run `ISynergy.Framework.Core.Tests` — LanguageServiceTests pass
- grep: `Properties\.Resources\.` only appears in Globalization's Designer.cs
