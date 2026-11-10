using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Edge-case coverage for <see cref="StringExtensions.HexStringToColor"/>.
/// </summary>
[TestClass]
public class StringExtensionsHexColorEdgeCaseTests
{
    [TestMethod]
    public void HexStringToColor_NonAsciiDigits_TakeFormatExceptionGuard()
    {
        // The hex filter uses the regex class "[abcdefABCDEF\d]+". In .NET, \d matches Unicode
        // decimal digits, so Arabic-Indic digits survive ExtractHexDigits (length 6) but are not
        // valid for int.Parse(..., HexNumber). This drives the FormatException guard, which must
        // yield Color.Empty rather than throwing.
        var result = "\u0661\u0662\u0663\u0664\u0665\u0666".HexStringToColor();

        Assert.AreEqual(Color.Empty, result);
    }
}
