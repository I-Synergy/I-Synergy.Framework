using AppColorHelper = ISynergy.Framework.UI.Helpers.ColorHelper;
using Windows.UI;

namespace ISynergy.Framework.UI.WinUI.Helpers;

[TestClass]
public class ColorHelperTests
{
    // ── HexStringToColor ─────────────────────────────────────────────────────

    [TestMethod]
    public void HexStringToColor_WithHash_ReturnsCorrectColor()
    {
        var color = AppColorHelper.HexStringToColor("#0078D7");

        Assert.AreEqual(255, color.A, "Alpha must be 255 (fully opaque)");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0x78, color.G);
        Assert.AreEqual(0xD7, color.B);
    }

    [TestMethod]
    public void HexStringToColor_WithoutHash_ReturnsCorrectColor()
    {
        var color = AppColorHelper.HexStringToColor("0078D7");

        Assert.AreEqual(255, color.A, "Alpha must be 255 (fully opaque)");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0x78, color.G);
        Assert.AreEqual(0xD7, color.B);
    }

    [TestMethod]
    public void HexStringToColor_AlphaIsAlways255()
    {
        var color = AppColorHelper.HexStringToColor("FF0000");

        Assert.AreEqual(255, color.A, "Alpha must always be 255 regardless of input");
    }

    [TestMethod]
    public void HexStringToColor_InvalidLength_ReturnsTransparent()
    {
        var color = AppColorHelper.HexStringToColor("FFF");

        // Microsoft.UI.Colors.Transparent requires the WinUI COM host; compare by channel values instead.
        Assert.AreEqual(0, color.A, "Invalid hex must return transparent (A=0)");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
    }

    [TestMethod]
    public void HexStringToColor_PureRed_ReturnsCorrectChannels()
    {
        var color = AppColorHelper.HexStringToColor("FF0000");

        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
    }

    [TestMethod]
    public void HexStringToColor_White_ReturnsAllChannels255()
    {
        var color = AppColorHelper.HexStringToColor("FFFFFF");

        Assert.AreEqual(255, color.A);
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(255, color.G);
        Assert.AreEqual(255, color.B);
    }

    [TestMethod]
    public void HexStringToColor_Black_ReturnsAllChannels0ExceptAlpha()
    {
        var color = AppColorHelper.HexStringToColor("000000");

        Assert.AreEqual(255, color.A, "Alpha must be 255");
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
    }

    // ── GetColorFromHexString ────────────────────────────────────────────────

    [TestMethod]
    public void GetColorFromHexString_6Chars_AlphaIs255()
    {
        // Regression: was byte.Parse("1") = 1 instead of 255
        var color = AppColorHelper.GetColorFromHexString("0078D7");

        Assert.AreEqual(255, color.A, "Alpha must be 255, not 1 (regression: byte.Parse(\"1\"))");
    }

    [TestMethod]
    public void GetColorFromHexString_6CharsWithHash_AlphaIs255()
    {
        var color = AppColorHelper.GetColorFromHexString("#0078D7");

        Assert.AreEqual(255, color.A, "Alpha must be 255 after stripping #");
    }

    [TestMethod]
    public void GetColorFromHexString_6Chars_ReturnsCorrectRgb()
    {
        var color = AppColorHelper.GetColorFromHexString("0078D7");

        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0x78, color.G);
        Assert.AreEqual(0xD7, color.B);
    }

    [TestMethod]
    public void GetColorFromHexString_8Chars_UsesAlphaFromString()
    {
        var color = AppColorHelper.GetColorFromHexString("FF0078D7");

        Assert.AreEqual(0xFF, color.A);
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(0x78, color.G);
        Assert.AreEqual(0xD7, color.B);
    }

    [TestMethod]
    public void GetColorFromHexString_8CharsWithPartialAlpha_PreservesAlpha()
    {
        var color = AppColorHelper.GetColorFromHexString("800078D7");

        Assert.AreEqual(0x80, color.A, "Alpha from 8-char string must be preserved");
    }

    [TestMethod]
    public void GetColorFromHexString_InvalidLength_ThrowsInvalidCastException()
    {
        Assert.ThrowsExactly<InvalidCastException>(() => AppColorHelper.GetColorFromHexString("FFF"));
    }

    // ── GetSolidColorBrushFromHexString ──────────────────────────────────────
    // SolidColorBrush is a COM-backed XAML type that requires the WinUI application
    // host (ActivationFactory COM registration). These tests cannot run in a plain
    // unit test process. The alpha=255 regression is covered above via GetColorFromHexString.

    [TestMethod]
    [Ignore("SolidColorBrush requires WinUI XAML runtime (REGDB_E_CLASSNOTREG in unit test host)")]
    public void GetSolidColorBrushFromHexString_6Chars_AlphaIs255()
    {
        var brush = AppColorHelper.GetSolidColorBrushFromHexString("0078D7");

        Assert.AreEqual(255, brush.Color.A);
    }

    [TestMethod]
    [Ignore("SolidColorBrush requires WinUI XAML runtime (REGDB_E_CLASSNOTREG in unit test host)")]
    public void GetSolidColorBrushFromHexString_6Chars_ReturnsCorrectColor()
    {
        var brush = AppColorHelper.GetSolidColorBrushFromHexString("0078D7");

        Assert.AreEqual(0, brush.Color.R);
        Assert.AreEqual(0x78, brush.Color.G);
        Assert.AreEqual(0xD7, brush.Color.B);
    }

    [TestMethod]
    [Ignore("SolidColorBrush requires WinUI XAML runtime (REGDB_E_CLASSNOTREG in unit test host)")]
    public void GetSolidColorBrushFromHexString_8Chars_PreservesAlpha()
    {
        var brush = AppColorHelper.GetSolidColorBrushFromHexString("800078D7");

        Assert.AreEqual(0x80, brush.Color.A);
    }

    [TestMethod]
    [Ignore("SolidColorBrush requires WinUI XAML runtime (REGDB_E_CLASSNOTREG in unit test host)")]
    public void GetSolidColorBrushFromHexString_InvalidLength_ThrowsInvalidCastException()
    {
        Assert.ThrowsExactly<InvalidCastException>(() => AppColorHelper.GetSolidColorBrushFromHexString("FFF"));
    }
}
