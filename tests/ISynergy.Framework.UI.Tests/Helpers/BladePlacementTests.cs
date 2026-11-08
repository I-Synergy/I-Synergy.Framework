using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Helpers;

[TestClass]
public class BladePlacementTests
{
    // ── Resolve ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void Resolve_WithAuto_ReturnsEnd()
    {
        Assert.AreEqual(BladeViewAnchor.End, BladePlacement.Resolve(BladeViewAnchor.Auto, false));
        Assert.AreEqual(BladeViewAnchor.End, BladePlacement.Resolve(BladeViewAnchor.Auto, true));
    }

    [TestMethod]
    public void Resolve_WithStart_ReturnsStart()
    {
        Assert.AreEqual(BladeViewAnchor.Start, BladePlacement.Resolve(BladeViewAnchor.Start, false));
        Assert.AreEqual(BladeViewAnchor.Start, BladePlacement.Resolve(BladeViewAnchor.Start, true));
    }

    [TestMethod]
    public void Resolve_WithEnd_ReturnsEnd()
    {
        Assert.AreEqual(BladeViewAnchor.End, BladePlacement.Resolve(BladeViewAnchor.End, false));
        Assert.AreEqual(BladeViewAnchor.End, BladePlacement.Resolve(BladeViewAnchor.End, true));
    }

    // ── IsStartSide(anchor, bool) ────────────────────────────────────────────

    [TestMethod]
    public void IsStartSide_WithAuto_LeftToRight_IsFalse()
    {
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.Auto, false));
    }

    [TestMethod]
    public void IsStartSide_WithAuto_RightToLeft_IsTrue()
    {
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.Auto, true));
    }

    [TestMethod]
    public void IsStartSide_WithStart_LeftToRight_IsTrue()
    {
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.Start, false));
    }

    [TestMethod]
    public void IsStartSide_WithStart_RightToLeft_IsFalse()
    {
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.Start, true));
    }

    [TestMethod]
    public void IsStartSide_WithEnd_LeftToRight_IsFalse()
    {
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.End, false));
    }

    [TestMethod]
    public void IsStartSide_WithEnd_RightToLeft_IsTrue()
    {
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.End, true));
    }

    // ── IsStartSide(anchor, string?) ─────────────────────────────────────────

    [TestMethod]
    public void IsStartSide_WithEnUs_BehavesAsLeftToRight()
    {
        // LTR: Start is on the physical left; End is on the physical right.
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.Start, "en-US"));
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.End, "en-US"));
    }

    [TestMethod]
    public void IsStartSide_WithArSa_BehavesAsRightToLeft()
    {
        // RTL: Start is mirrored to the physical right; End is on the physical left.
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.Start, "ar-SA"));
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.End, "ar-SA"));
    }

    [TestMethod]
    public void IsStartSide_WithHeIl_BehavesAsRightToLeft()
    {
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.Start, "he-IL"));
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.End, "he-IL"));
    }

    [TestMethod]
    public void IsStartSide_WithNullCulture_BehavesAsLeftToRight()
    {
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.Start, (string?)null));
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.End, (string?)null));
    }

    [TestMethod]
    public void IsStartSide_WithEmptyCulture_BehavesAsLeftToRight()
    {
        Assert.IsTrue(BladePlacement.IsStartSide(BladeViewAnchor.Start, string.Empty));
        Assert.IsFalse(BladePlacement.IsStartSide(BladeViewAnchor.End, string.Empty));
    }

    // ── HorizontalSign(anchor, bool) ─────────────────────────────────────────

    [TestMethod]
    public void HorizontalSign_WithAuto_LeftToRight_IsRightEdge()
    {
        Assert.AreEqual(1, BladePlacement.HorizontalSign(BladeViewAnchor.Auto, false));
    }

    [TestMethod]
    public void HorizontalSign_WithAuto_RightToLeft_IsLeftEdge()
    {
        Assert.AreEqual(-1, BladePlacement.HorizontalSign(BladeViewAnchor.Auto, true));
    }

    [TestMethod]
    public void HorizontalSign_WithStart_LeftToRight_IsLeftEdge()
    {
        Assert.AreEqual(-1, BladePlacement.HorizontalSign(BladeViewAnchor.Start, false));
    }

    [TestMethod]
    public void HorizontalSign_WithStart_RightToLeft_IsRightEdge()
    {
        Assert.AreEqual(1, BladePlacement.HorizontalSign(BladeViewAnchor.Start, true));
    }

    [TestMethod]
    public void HorizontalSign_WithEnd_LeftToRight_IsRightEdge()
    {
        Assert.AreEqual(1, BladePlacement.HorizontalSign(BladeViewAnchor.End, false));
    }

    [TestMethod]
    public void HorizontalSign_WithEnd_RightToLeft_IsLeftEdge()
    {
        Assert.AreEqual(-1, BladePlacement.HorizontalSign(BladeViewAnchor.End, true));
    }

    // ── Overload consistency ─────────────────────────────────────────────────

    [TestMethod]
    public void HorizontalSign_OverloadsAgree_ForEveryCombination()
    {
        var anchors = new[] { BladeViewAnchor.Auto, BladeViewAnchor.Start, BladeViewAnchor.End };
        var directions = new[] { false, true };

        foreach (var anchor in anchors)
        {
            foreach (var isRightToLeft in directions)
            {
                var fromAnchor = BladePlacement.HorizontalSign(anchor, isRightToLeft);
                var fromSide = BladePlacement.HorizontalSign(
                    BladePlacement.IsStartSide(anchor, isRightToLeft),
                    isRightToLeft);

                Assert.AreEqual(fromAnchor, fromSide);
            }
        }
    }

    [TestMethod]
    public void HorizontalSign_FromBooleans_MapsLeftToMinusOneAndRightToPlusOne()
    {
        Assert.AreEqual(-1, BladePlacement.HorizontalSign(true, false));
        Assert.AreEqual(1, BladePlacement.HorizontalSign(false, false));
        Assert.AreEqual(-1, BladePlacement.HorizontalSign(true, true));
        Assert.AreEqual(1, BladePlacement.HorizontalSign(false, true));
    }
}
