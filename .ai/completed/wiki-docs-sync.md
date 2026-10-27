# Documentation published to the GitHub wiki

**Status:** SUPERSEDED — replaced by GitHub Pages on `docs/` (2026-10-27)
**Date:** 2026-10-27
**Plan:** `.ai/plans/wiki-docs-sync.md`

> **Superseded.** The user asked whether a repository folder could serve as the wiki without syncing.
> It cannot — the Wiki tab is always a separate, flat `repo.wiki.git`. The decision was therefore to
> drop the sync and publish `docs/` directly with GitHub Pages: `docs/_config.yml` (theme plus
> `jekyll-optional-front-matter` and `jekyll-relative-links`), `docs/index.md` as the landing page, and
> one setting (Settings → Pages → *Deploy from a branch* → `main` / `docs`).
> `scripts/Sync-DocsToWiki.ps1` and `.github/workflows/wiki.yml` were deleted before either reached
> `main`; nothing was ever published to the wiki. The record below documents what the sync did.

## What was built

- `scripts/Sync-DocsToWiki.ps1` — renders `docs/` into a flat wiki page set: deterministic page names
  from the folder path, `/wiki/<Page>` links, `blob/main` URLs for excluded or repository-only
  targets, image copying as `images/<hash>.<ext>`, generated `_Sidebar.md` (`toc.yml` order where a
  folder has one), and an ownership marker on every generated page. `-Check` runs the mapping and
  path-normalisation assertions; `-DryRun` renders and fails on collisions or unresolved links.
- `.github/workflows/wiki.yml` — self-check → render → push to `repo.wiki.git`, three separate steps.
  Triggered by `docs/**` (or the script/workflow) on `main`, plus `workflow_dispatch`.

## Verified locally

- `-Check`: 8 assertions pass.
- `-DryRun`: 414 pages from 464 Markdown files, 50 excluded, 0 collisions, 0 unresolved links, 0 assets.
  Sections: root 10, developers 4, diagrams 1, libraries 398, samples 1.
- Idempotence: rendering twice into separate directories produced no differences (`diff -r`).
- `.github/workflows/wiki.yml` parses (PyYAML; triggers `push`/`workflow_dispatch`).
- One source fix was needed: `docs/developers/maui/awesome_maui_libraries_and_samples.md` linked to a
  `CONTRIBUTING.md` that does not exist; it now points at the upstream
  `jsuarezruiz/awesome-dotnet-maui` file.

## Not verified

- The first real publish. It needs two one-time actions: create the wiki's first page (`Home`) in the
  UI — `repo.wiki.git` does not exist until then — and add the `WIKI_TOKEN` secret (fine-grained PAT,
  *Contents: read and write*). `GITHUB_TOKEN` cannot push the wiki repository.
- The push step's git plumbing on a live wiki (marker-based deletion of stale pages, `images/`
  copying). It is exercised only by that first run.

## Decisions

- **Fine-grained PAT over a classic `repo` PAT** (user choice): least privilege, scoped to this
  repository. Falls back to a classic PAT only if GitHub rejects it for wiki pushes.
- **`toc.yml` order in `_Sidebar.md`** (user choice) rather than alphabetical, matching the existing
  DocFX folder structure.
- **Exclusions** stay as planned: `docs/superpowers/**` and the internal review/analysis reports.
- **Ownership marker instead of a mirror**: the sync never deletes or overwrites a page it did not
  generate, so the wiki can carry hand-written pages alongside the generated set.

## Follow-up

- Create the `Home` page and the `WIKI_TOKEN` secret, then run the workflow once and check the
  `/wiki` navigation and that a hand-written page survives a sync.
