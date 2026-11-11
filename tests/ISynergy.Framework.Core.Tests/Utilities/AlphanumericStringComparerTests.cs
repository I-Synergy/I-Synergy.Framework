using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Tests for the refactored <see cref="AlphanumericStringComparer"/> logic
/// (Compare / CompareSegments / CompareSameValueNumericSegments / CompareLeadingZeros / PadToSameLength).
/// </summary>
[TestClass]
public class AlphanumericStringComparerTests
{
    private static readonly string[] NaturalOrder = ["file1", "file2", "file10"];

    private readonly AlphanumericStringComparer _comparer = new();

    // --- Compare: non-string arguments short-circuit to 0 (refactored combined guard) ---

    [TestMethod]
    public void Compare_NullArguments_ReturnsZero()
    {
        Assert.AreEqual(0, _comparer.Compare(null, null));
        Assert.AreEqual(0, _comparer.Compare(null, "abc"));
        Assert.AreEqual(0, _comparer.Compare("abc", null));
    }

    [TestMethod]
    public void Compare_NonStringArguments_ReturnsZero()
    {
        Assert.AreEqual(0, _comparer.Compare(5, "abc"));
        Assert.AreEqual(0, _comparer.Compare("abc", 5));
        Assert.AreEqual(0, _comparer.Compare(1, 2));
    }

    [TestMethod]
    public void Compare_IdenticalStrings_ReturnsZero()
    {
        Assert.AreEqual(0, _comparer.Compare("abc123", "abc123"));
        Assert.AreEqual(0, _comparer.Compare("test", "test"));
    }

    // --- CompareSegments: one/both non-integer -> ordinal string comparison ---

    [TestMethod]
    public void Compare_PureAlphabetic_UsesTextOrdering()
    {
        Assert.IsLessThan(0, _comparer.Compare("abc", "abd"));
        Assert.IsGreaterThan(0, _comparer.Compare("abd", "abc"));
        Assert.IsLessThan(0, _comparer.Compare("abc", "abcd"));
    }

    [TestMethod]
    public void Compare_NonIntegerVsInteger_ComparesAsText()
    {
        // 'a' has a higher ordinal value than '1', so text comparison wins.
        Assert.IsGreaterThan(0, _comparer.Compare("a", "1"));
        Assert.IsLessThan(0, _comparer.Compare("1", "a"));
    }

    // --- CompareSegments: both integers -> numeric comparison ---

    [TestMethod]
    public void Compare_PureNumeric_UsesNumericOrdering()
    {
        Assert.IsLessThan(0, _comparer.Compare("2", "10"));
        Assert.IsGreaterThan(0, _comparer.Compare("10", "2"));
    }

    [TestMethod]
    public void Compare_AlphanumericSegments_UsesNumericOrderingWithinText()
    {
        // "file2" vs "file10" -> numeric segment ordering, not lexicographic.
        Assert.IsLessThan(0, _comparer.Compare("file2", "file10"));
        Assert.IsGreaterThan(0, _comparer.Compare("file10", "file2"));
    }

    // --- CompareSameValueNumericSegments: same value, leading zeros decide (longer sorts first) ---

    [TestMethod]
    public void Compare_SameValueNumericSegments_LongerPaddedSegmentSortsFirst()
    {
        // "01" and "1" both parse to 1; the longer (more leading zeros) sorts first.
        Assert.IsLessThan(0, _comparer.Compare("01", "1"));
        Assert.IsGreaterThan(0, _comparer.Compare("1", "01"));
    }

    [TestMethod]
    public void Compare_SameValueSameLengthNumericSegment_ContributesZero()
    {
        // At index 1 both strings hold the identical numeric segment "1" (same value and same
        // length), which exercises the equal-length return 0 branch; "b" vs "c" still decides.
        Assert.IsLessThan(0, _comparer.Compare("a1b", "a1c"));
        Assert.IsGreaterThan(0, _comparer.Compare("a1c", "a1b"));
    }

    // --- CompareLeadingZeros: different values -> leading zeros dominate, else numeric order ---

    [TestMethod]
    public void Compare_DifferentValueLeadingZeros_MoreLeadingZerosSortsFirst()
    {
        // "01" (1, one zero) vs "2" (2, no zeros): leading-zero count dominates the value.
        Assert.IsLessThan(0, _comparer.Compare("01", "2"));
        Assert.IsGreaterThan(0, _comparer.Compare("2", "01"));
    }

    [TestMethod]
    public void Compare_DifferentValueSameZeroCount_FallsBackToNumeric()
    {
        // "12" vs "9": equal leading-zero counts -> numeric comparison.
        Assert.IsGreaterThan(0, _comparer.Compare("12", "9"));
        Assert.IsLessThan(0, _comparer.Compare("9", "12"));
    }

    // --- PadToSameLength: differing segment counts are padded with empty segments ---

    [TestMethod]
    public void Compare_FewerSegments_PaddedWithEmptySegment_SortsFirst()
    {
        // "a" (["a"]) vs "a1" (["a","1"]): padded "a" gains an empty tail; "" < "1".
        Assert.IsLessThan(0, _comparer.Compare("a", "a1"));
        Assert.IsGreaterThan(0, _comparer.Compare("a1", "a"));
    }

    [TestMethod]
    public void Compare_SegmentCountsDifferButPrefixEqual_UsesPadding()
    {
        // "abc1" vs "abc": the extra "1" vs padded empty tail drives the result.
        Assert.IsGreaterThan(0, _comparer.Compare("abc1", "abc"));
        Assert.IsLessThan(0, _comparer.Compare("abc", "abc1"));
    }

    // --- GetList: apostrophes force a segment split ---

    [TestMethod]
    public void Compare_ApostropheActsAsSegmentSeparator()
    {
        // "a'b" splits into ["a","b"]; "ab" stays ["ab"]. "a" < "ab".
        Assert.IsLessThan(0, _comparer.Compare("a'b", "ab"));
        Assert.AreNotEqual(0, _comparer.Compare("a'b", "ab"));
    }

    // --- Compare: different raw strings that collapse to identical segments fall through to 0 ---

    [TestMethod]
    public void Compare_EqualSegmentsButDifferentRawStrings_ReturnsZero()
    {
        // "a{b" and "ab" both collapse to the single segment "ab". The raw strings differ (so the
        // early s1 == s2 check does not fire) but every segment compares equal to 0, exercising the
        // post-loop return 0 in Compare.
        Assert.AreEqual(0, _comparer.Compare("a{b", "ab"));
        Assert.AreEqual(0, _comparer.Compare("ab", "a{b"));
    }

    // --- End-to-end ordering through the IComparer contract ---

    [TestMethod]
    public void Compare_UsedForSorting_ProducesNaturalOrder()
    {
        var values = new[] { "file10", "file1", "file2" };

        var sorted = values.OrderBy(v => v, Comparer<object>.Create(_comparer.Compare)).ToArray();

        Assert.AreSequenceEqual(NaturalOrder, sorted);
    }

    [TestMethod]
    public void Compare_IsAntisymmetric()
    {
        var left = _comparer.Compare("item2", "item10");
        var right = _comparer.Compare("item10", "item2");

        Assert.AreEqual(Math.Sign(left), -Math.Sign(right));
    }
}
