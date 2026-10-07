# Splitting the god classes (report §4.4)

**Status:** DONE
**Started:** 2026-10-07
**Client:** Reasonix Code
**Report:** `docs/FRAMEWORK_REVIEW_2026-10-07.md`

## Scope (user, via ask)
All three god classes; deeper redesign (public API change permitted — none was needed). Public and
protected surfaces were kept stable so subclasses and tests are unaffected.

## Steps
- [x] G1: Split `BaseAsyncRelayCommand` into cohesive collaborators
- [x] G2: Split `MessengerService` into cohesive collaborators
- [x] G3: Split `RedBlackTree<T>` into cohesive collaborators
- [x] G4: Verify

## Result
Full solution build: **0 non-`Sample.Maui` errors**. Test sweep (33 projects): **0 failures**.
`git diff --check`: clean.

## Details
- **G1 `BaseAsyncRelayCommand`** — extracted the process-wide allocation pools and the rounded timeout
  cache (weak-reference pool, continuation token-source pool, timeout token cache, cleanup timer,
  process-exit hook, `Cleanup`) into a new internal `CommandExecutionResourcePool`. The command now
  rents/returns resources through it; `CleanupStaticResources` delegates. Public/protected surface
  unchanged.
- **G2 `MessengerService`** — extracted recipient storage into a new internal `MessengerRecipientRegistry`
  (the two weak-reference indexes, register/unregister/cleanup, and snapshot accessors used by dispatch);
  `WeakActionAndToken` was promoted to a top-level internal type. `MessengerService` keeps its full
  public surface and now only dispatches and delegates storage.
- **G3 `RedBlackTree<T>`** — extracted the insert-balancing and delete-balancing fix-ups out of
  `Add`/`Remove` into a dedicated nested `private static class Balancing`
  (`FixAfterInsert`/`FixAfterRemove`). The class became `partial`; the algorithms were ported verbatim
  (`goto`-based), reaching the tree's private rotations/fields as a nested type. Public/ICollection API
  unchanged.

## Notes
- No public API was changed; the `partial`/nested/internal extractions keep subclasses, the `ICollection`
  contracts, and the tree's `check()`/`check_node()` invariant tests working unchanged.
- A transient CS1710 (duplicate `<typeparam>` on the partial type) was introduced and fixed.
- Build rule honoured: node reuse disabled. No committing was performed.
