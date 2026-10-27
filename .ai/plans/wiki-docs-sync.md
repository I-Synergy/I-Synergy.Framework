# Publish `docs/` to the GitHub wiki (generated sync)

Status: IMPLEMENTED (first publish waits on the one-time wiki/token setup)
Started: 2026-10-27
Requested by: user — "sync docs/ into the GitHub wiki via a workflow"
Approved by: user (plan as written; fine-grained PAT; `toc.yml` sidebar order)

## Goal

`docs/` stays the single source of truth in the repository. A GitHub Actions workflow renders every
published page into the wiki's flat page namespace, rewrites internal links and images, generates
`_Sidebar.md`, and pushes to `I-Synergy.Framework.wiki.git`. Nobody hand-copies pages.

Not in scope: changing `docs/` content, the wiki's visual design, GitHub Pages.

## What the wiki can hold

- **Flat namespace.** `docs/libraries/ISynergy.Framework.Core/index.md` becomes a page named
  `Libraries-ISynergy.Framework.Core`; wikis have no folders.
- **Its own git repository**, `I-Synergy.Framework.wiki.git`. It does not exist until the first page is
  created in the UI (`git ls-remote` today: `Repository not found`).
- **No PR review, no CI** on pages edited in the browser.
- Links are `/wiki/Page-Name`; images must be files in the wiki repository.

## Page mapping (deterministic)

| Source | Page name |
|---|---|
| `docs/MIGRATION_GUIDE.md` | `MIGRATION-GUIDE` |
| `docs/libraries/index.md` | `Libraries` |
| `docs/libraries/ISynergy.Framework.Core/index.md` | `Libraries-ISynergy.Framework.Core` |
| `docs/developers/maui/nodejs_installation_windows.md` | `Developers-Maui-Nodejs-installation-windows` |

Rules: drop the `docs/` prefix and a trailing `/index`; join path segments with `-`; replace `_` and
spaces with `-`; collapse repeats; upper-case the first character of each segment.
`docs/index.md` (if added) always maps to `Home`. Two sources mapping to one page name is a hard
error — the dry run fails with the collision list. (The table originally showed `MIGRATION_GUIDE` and
a lower-case `nodejs`; both were corrected to the single implemented rule.)

## Exclusions

Default exclusion list, because these are internal working documents rather than library
documentation:

```
docs/superpowers/**
docs/FRAMEWORK_REVIEW_*.md
docs/SOLID_PRINCIPLES_ANALYSIS.md
docs/POTENTIAL_ISSUES_AND_RISKS.md
docs/MVVM_AND_UI_FRAMEWORK_ISSUES_REPORT.md
docs/ARCHITECTURAL_ISSUES_SUMMARY.md
docs/ASYNC_EXCEPTION_ISSUES_FIXED.md
```

Excluded pages keep working links: a link to an excluded page is rewritten to the GitHub `blob/main`
URL instead of a wiki page.

## Link and image rewriting

- Relative `[text](other.md)` links are resolved against the source file, `.md` appended when missing,
  and emitted as `/wiki/<mapped page>`; `#anchors` are preserved.
- Absolute URLs, `mailto:` and links outside `docs/` are left untouched.
- The 5 non-Markdown assets under `docs/` are copied into the wiki as `images/<sha256-8>.<ext>` and
  every referencing `![](...)` is rewritten to that path.
- The sync emits a report of unresolved links (count + list); the workflow fails on unresolved links
  that point inside `docs/`, so drift is caught in CI rather than on the wiki.

## Ownership markers (no clobbering hand edits)

Every generated page ends with `<!-- generated from docs/<path> -->`. The sync only overwrites and only
deletes files carrying that marker. Pages a human created in the wiki are preserved, which makes the
wiki safe for a hand-written landing page or a FAQ.

## `_Sidebar.md`

Generated from the folder tree, using the existing `toc.yml` order where a folder has one
(`docs/developers`, `docs/libraries`, `docs/samples`, `docs/diagrams`) and alphabetical order
otherwise. Top level: `Home`, then one section per first path segment. Also generated, so it carries
no ownership marker — it is overwritten every run.

## Workflow

New `.github/workflows/wiki.yml`:

```yaml
name: Wiki
on:
  push:
    branches: [main]
    paths: ['docs/**', 'scripts/Sync-DocsToWiki.ps1', '.github/workflows/wiki.yml']
  workflow_dispatch:
permissions:
  contents: read           # the wiki push uses a separate token, see below
concurrency:
  group: wiki-sync
  cancel-in-progress: false
```

Job `sync` on `ubuntu-latest` (pwsh is preinstalled):

1. `actions/checkout@v4`.
2. `./scripts/Sync-DocsToWiki.ps1 -OutPath $RUNNER_TEMP/wiki -Repository I-Synergy/I-Synergy.Framework`
   — render, rewrite, sidebar, unresolved-link report.
3. Clone `https://x-access-token:$WIKI_TOKEN@github.com/I-Synergy/I-Synergy.Framework.wiki.git`,
   replace marker-bearing pages, delete stale marker-bearing pages, `git add -A`, commit
   `docs: sync from <sha>` only when something changed, push.

Rendering and pushing are separate steps so a rendering bug cannot touch the live wiki.

## Token decision (the one security-relevant choice)

`GITHUB_TOKEN` cannot push to `repo.wiki.git` — the wiki is a different repository and the job token is
scoped to this one (`Resource not accessible by integration`). So the job needs a stored credential:

- **Option A (needed in practice):** classic PAT with `repo` scope as secret `WIKI_TOKEN`. Broad — it
  can write any repository the account can reach.
- **Option B (least privilege):** fine-grained PAT for this repository only with
  *Contents: read and write* as secret `WIKI_TOKEN`.
- **Option C:** an SSH deploy key registered on the wiki repository, written to a temp file in the job.

Recommendation: **Option B**, falling back to **A** only if fine-grained PATs cannot push the wiki.

## Prerequisites (one-time, by the user)

1. Create the wiki's first page in the UI (e.g. `Home`) — until then the wiki repository does not exist
   and the clone fails.
2. Add the `WIKI_TOKEN` secret with the scope chosen above.

## Files

- `.github/workflows/wiki.yml` — new.
- `scripts/Sync-DocsToWiki.ps1` — new; parameters `-DocsPath`, `-OutPath`, `-Repository`,
  `-BaseBranch`, `-Exclude`, `-DryRun`, `-Check`.
- `.gitignore` — nothing (the rendered wiki is written to `$RUNNER_TEMP`).
- `docs/MIGRATION_GUIDE.md`, `README.md` — a line pointing at the wiki.
- `CHANGELOG.md` — one `### Added` entry.

## Verification

1. `./scripts/Sync-DocsToWiki.ps1 -DryRun` locally: page count per folder, collision list empty,
   unresolved-link list empty, sample rewritten pages inspected by eye.
2. `-DryRun` into a scratch clone of an empty repo, then a second run proves idempotence (no diff).
3. Unit-level checks in the script itself: mapping table assertions and a link-rewrite table, run with
   `-Check` (exit non-zero on any mismatch) — invoked by the workflow before rendering.
4. First workflow run: `/wiki` shows the generated pages, `_Sidebar` navigates, no dead links; a
   hand-created page survives.

## Rollback

Delete the workflow, the script and the secret; the wiki repository is independent of the code
repository, so nothing in `src/`, `tests/` or `docs/` is affected. Pages can be emptied from the wiki
UI or by pushing an empty commit to `repo.wiki.git`.

## Open questions

- Exclusions: is the default list right, or should the internal reports be published too?
- Sidebar: per-folder order from `toc.yml` (proposed) or strictly alphabetical?
- Does the team want the wiki editable at all (i.e. accept drift), given `docs/` stays canonical?
