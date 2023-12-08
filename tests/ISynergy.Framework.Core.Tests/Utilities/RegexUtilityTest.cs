using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Utilities.Tests;

/// <summary>
/// Regex utility test.
/// </summary>
[TestClass]
public class RegexUtilityTest
{
    /// <summary>
    /// Mask to regex test.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="result"></param>
    [DataTestMethod]
    [DataRow("0", @"\d")]
    [DataRow("9", @"[\d]?")]
    [DataRow("#", @"[\d+-]?")]
    [DataRow("L", @"[a-z]")]
    [DataRow(">L", @"[A-Z]")]
    [DataRow("<L", @"[a-z]")]
    [DataRow("?", @"[a-z]?")]
    [DataRow(">?", @"[A-Z]?")]
    [DataRow("<?", @"[a-z]?")]
    [DataRow("&", @"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]")]
    [DataRow("C", @"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]?")]
    [DataRow("A", @"\W")]
    [DataRow(".", @"[.]")]
    [DataRow(",", @"[,]")]
    [DataRow(":", @"[:]")]
    [DataRow("/", @"[/]")]
    [DataRow("$", @"[$]")]
    [DataRow("<", @"")]
    [DataRow(">", @"")]
    [DataRow("|", @"")]
    [DataRow(@"\", @"\")]
    public void MaskToRegExTest(string mask, string result) =>
        Assert.AreEqual(RegexUtility.MaskToRegexStringConverter(mask), result);

    /// <summary>
    /// Mask to regex which do match test.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="value"></param>
    [DataTestMethod]
    [DataRow("000000", "123456")]
    [DataRow("000000", "457890")]
    [DataRow("0000>LL", "5981XC")]
    [DataRow("$ 0.00", "$ 9.95")]
    [DataRow("€ 0,00", "€ 9,95")]
    [DataRow("LLLLLLLLLL", "abcdefghij")]
    [DataRow("<LLLLLLLLLL", "abcdefghij")]
    [DataRow("<L>L<L>L<L>L<L>L<L>L", "aBcDeFgHiJ")]
    [DataRow("<L>L<L>L<L|LLLLL", "aBcDeFGHIJ")]
    public void MaskToRegexDoMatchTest(string mask, string value) =>
        Assert.IsTrue(RegexUtility.MaskToRegexConverter(mask).IsMatch(value));

    /// <summary>
    /// Mask to regex which do not match test.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="value"></param>
    [DataTestMethod]
    [DataRow("000000", "1234")]
    [DataRow("000000", "abcdef")]
    [DataRow("0000>LL", "XC5981")]
    [DataRow("0000>LL", "5981xC")]
    [DataRow("0000>LL", "5981Xc")]
    [DataRow("$ 0,00", "$ 9.95")]
    [DataRow("€ 0.00", "€ 9,95")]
    [DataRow("$ 0,00", "€ 9.95")]
    [DataRow("$ 0.00", "9,95")]
    [DataRow("$0.00", "$ 9.95")]
    [DataRow("LLLLLLLLLL", "abcdefghi")]
    [DataRow("<LLLLLLLLLL", "Abcdefghij")]
    [DataRow("<L>L<L>L<L>L<L>L<L>L", "abcdefghij")]
    [DataRow("<L>L<L>L<L|LLLLL", "aBcDeFgHjJ")]
    public void MaskToRegexDoNotMatchTest(string mask, string value) =>
        Assert.IsFalse(RegexUtility.MaskToRegexConverter(mask).IsMatch(value));
}
