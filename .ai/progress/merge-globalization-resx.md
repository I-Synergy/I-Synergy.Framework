# Merge Globalization .resx Files
Status: IN PROGRESS
Started: 2026-06-03

## Steps
- [x] Read existing translations from Mvvm, UI, Geography, Physics
- [x] Write merged Resources.resx (English - union of all keys)
- [x] Write Resources.de.resx (German)
- [x] Write Resources.fr.resx (French)
- [x] Write Resources.nl.resx (Dutch)
- [x] Write Resources.es.resx (Spanish)
- [x] Write Resources.ar.resx (Arabic)
- [ ] Verify all files

## Notes
- Core has no .resx files to include
- Mvvm has 7 keys with full translations for all languages
- UI has many keys with full translations for all languages
- Geography has 21 keys (EN only - translations provided in task)
- Physics has 56 keys (EN only - translations provided in task)
- AspNetCore.Blazor's 89 keys are a duplicate subset of UI and are skipped
