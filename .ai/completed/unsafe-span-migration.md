# Unsafe → Span<T> Migration

**Status:** DONE

## Priority 1 — Replace unsafe with Span<T> in Matrix files

### Transformation rules
- Same-type element-wise 2D array loops → `MemoryMarshal.CreateSpan(ref arr[0,0], arr.Length)` + linear index loop
- Diagonal stride loops (`pr += cols+1`) → direct `[j, j]` 2D indexing
- Type-conversion loops (double→float etc.) → nested `GetLength(0)/GetLength(1)` loops
- Jagged-array-only `unsafe { }` with no pointers → remove the `unsafe` wrapper only
- Add `using System.Runtime.InteropServices;` wherever MemoryMarshal is introduced

### Files

- [ ] Matrix.Subtract.cs (lines 320-335, 387-427, 611-707)
- [ ] Matrix.Multiply.cs (lines 320-335, 388-428, 616-712)
- [ ] Matrix.Divide.Elementwise.cs (lines 320-335, 388-428, 616-712)
- [ ] Matrix.Elementwise.cs (lines 424-440, 496-512 + jagged unsafe wrappers)
- [ ] Matrix.Common.cs (lines 1275-1289)
- [ ] Matrix.Comparisons.cs (lines 130-170, 635-673)
- [ ] Matrix.Comparisons.Elementwise.cs (lines 138-205, 772-839)
- [ ] Matrix.Conversions.cs (lines 1267-1362)
- [ ] Matrix.Conversions.Generated.cs (lines 67-97, 210-240)
- [ ] Matrix.Add.Generated.cs (lines 8414-8522)

## Priority 2 — Bounds guards

- [ ] Matrix.Product.cs — add Release-mode guard for column offset arithmetic
- [ ] LuDecomposition.cs — document row-pointer bounds assumptions

## Priority 3 — Validation

- [ ] NpyFormat.Reader.cs — validate NumPy header `bytes` against buffer size
- [ ] MatNode.cs — validate binary MAT data before pointer cast

## Commit plan
- Commit after P1 complete
- Commit after P2 complete
- Commit after P3 complete
