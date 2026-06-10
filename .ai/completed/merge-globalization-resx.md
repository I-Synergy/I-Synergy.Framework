# Merge Globalization .resx Files
Status: DONE
Started: 2026-06-03
Completed: 2026-06-03

## Summary
Generated 7 merged .resx files in `src/ISynergy.Framework.Globalization/Properties/`:

- **Resources.resx** (English) — 199 keys: union of Mvvm (7) + UI + Geography (21) + Physics (56)
- **Resources.de.resx** (German) — 199 keys: Mvvm.de + UI.de + Geography translations + Physics translations
- **Resources.fr.resx** (French) — 199 keys: Mvvm.fr + UI.fr + Geography translations + Physics translations
- **Resources.nl.resx** (Dutch) — 199 keys: Mvvm.nl + UI.nl + Geography translations + Physics translations
- **Resources.es.resx** (Spanish) — 199 keys: Mvvm.es + UI.es + Geography translations + Physics translations
- **Resources.ar.resx** (Arabic) — 199 keys: Mvvm.ar + UI.ar + Geography translations + Physics translations

## Notes
- Core project had no .resx files, so no keys from Core
- `ThisItem` exists in both Mvvm and UI — used UI's value (identical text)
- AspNetCore.Blazor's 89 keys are a duplicate subset of UI and were skipped
- Schema follows Version 2.0 ResX format with xml:space="preserve"
- Keys sorted alphabetically in each file
- Script kept at `generate-resx.ps1` in the output directory for regeneration
