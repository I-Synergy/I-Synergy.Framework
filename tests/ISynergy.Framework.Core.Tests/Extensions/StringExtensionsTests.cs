using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class StringExtensionsTests.
/// </summary>
[TestClass]
public class StringExtensionsTests
{
    private static readonly string[] SplitAtParts = ["ab", "cd", "ef"];
    private static readonly string[] WholeString = ["abc"];
    private static readonly byte[] HexPairs = [0x00, 0x11];
    private static readonly char[] RemoveChars = ['b'];

    // Sample enum used by ToEnum tests.
    private enum SampleEnum
    {
        First,
        Second
    }

    /// <summary>
    /// Defines the test method IncreaseStringNumericSummand1.
    /// </summary>
    [TestMethod]
    public void IncreaseStringNumericSummand1()
    {
        string result = "10".IncreaseString2Long(1);
        Assert.AreEqual("11", result);
    }

    /// <summary>
    /// Defines the test method IncreaseStringNumericSummand3.
    /// </summary>
    [TestMethod]
    public void IncreaseStringNumericSummand3()
    {
        string result = "6281085010557".IncreaseString2Long(3);
        Assert.AreEqual("6281085010560", result);
    }

    /// <summary>
    /// Defines the test method IncreaseStringAlphaNumericSummand1.
    /// </summary>
    [TestMethod]
    public void IncreaseStringAlphaNumericSummand1()
    {
        string result = "A19".IncreaseString2Long(1);
        Assert.AreEqual("A20", result);
    }

    /// <summary>
    /// Defines the test method IncreaseStringAlphaNumericSummand1.
    /// </summary>
    [TestMethod]
    public void IncreaseStringAlphaNumericSummand2()
    {
        string result = "A01".IncreaseString2Long(1);
        Assert.AreEqual("A02", result);
    }

    /// <summary>
    /// Defines the test method IncreaseStringAlphaNumericSummand8.
    /// </summary>
    [TestMethod]
    public void IncreaseStringAlphaNumericSummand8()
    {
        string result = "AZURE02".IncreaseString2Long(8);
        Assert.AreEqual("AZURE10", result);
    }

    /// <summary>
    /// Defines the test method IncreaseStringAlphaNumericComplex.
    /// </summary>
    [TestMethod]
    public void IncreaseStringAlphaNumericComplex()
    {
        string result = "2016AZURE10STAGE001".IncreaseString2Long(99);
        Assert.AreEqual("2016AZURE10STAGE100", result);
    }

    /// <summary>
    /// Defines the test method CovertString2NumericIntegerTest.
    /// </summary>
    [TestMethod]
    public void CovertString2NumericIntegerTest()
    {
        int result = "2016001".CovertString2Numeric();
        Assert.AreEqual(2016001, result);
    }

    /// <summary>
    /// Defines the test method CovertString2NumericNonIntegerTest.
    /// </summary>
    [TestMethod]
    public void CovertString2NumericNonIntegerTest()
    {
        int result = "9999992016001".CovertString2Numeric();
        Assert.AreEqual(0, result);
    }

    [TestMethod()]
    public void ToCapitalizedTest()
    {
        string source = "hallo ik ben ismail. ik ben software developer.";
        string result = "Hallo ik ben ismail. Ik ben software developer.";

        Assert.AreEqual(result, source.ToCapitalized());
    }

    [TestMethod()]
    public void ToCapitalizedFirstLetterTest()
    {
        string source = "hallo ik ben ISMAIL. ik ben SOFTWARE DEVELOPER.";
        string result = "Hallo Ik Ben Ismail. Ik Ben Software Developer.";

        Assert.AreEqual(result, source.ToCapitalizedFirstLetter());
    }

    [TestMethod()]
    public void SplitAndKeepTest()
    {
        string text = "[a link|http://www.google.com]";
        System.Collections.Generic.IList<string> result = text.SplitAndKeep('[', '|', ']');
        Assert.HasCount(5, result);
        Assert.AreEqual("[", result[0]);
        Assert.AreEqual("a link", result[1]);
        Assert.AreEqual("|", result[2]);
        Assert.AreEqual("http://www.google.com", result[3]);
        Assert.AreEqual("]", result[4]);
    }

    // --- HexStringToColor ---

    [TestMethod]
    public void HexStringToColor_ValidSixHexDigits()
    {
        Assert.AreEqual(Color.FromArgb(255, 255, 255), "FFFFFF".HexStringToColor());
        Assert.AreEqual(Color.FromArgb(0, 0, 0), "#000000".HexStringToColor());
        Assert.AreEqual(Color.FromArgb(0x12, 0x34, 0x56), "#123456".HexStringToColor());
    }

    [TestMethod]
    public void HexStringToColor_InvalidLength_ReturnsEmpty()
    {
        Assert.AreEqual(Color.Empty, "FFF".HexStringToColor());
    }

    // --- Numeric predicates ---

    [TestMethod]
    public void NumericPredicates_ReportCorrectly()
    {
        Assert.IsTrue("1.5".IsFloat());
        Assert.IsFalse("abc".IsFloat());
        Assert.IsTrue("1.5".IsDecimal());
        Assert.IsFalse("abc".IsDecimal());
        Assert.IsTrue("42".IsInteger());
        Assert.IsFalse("4x".IsInteger());
    }

    // --- WordWrap ---

    [TestMethod]
    public void WordWrap_ShorterThanWidth_ReturnsSame()
    {
        Assert.AreEqual("short", "short".WordWrap(10));
    }

    [TestMethod]
    public void WordWrap_WrapsAtSpacesAndHardBreaksLongWords()
    {
        // Contains a space (wrap at last space) and a run of letters longer than the width
        // (hard break branch).
        var wrapped = "ab cdefghijklmnop".WordWrap(5);

        Assert.Contains("ab", wrapped);
        Assert.Contains("cdefg", wrapped);
        Assert.Contains(Environment.NewLine, wrapped);
    }

    // --- ExtractHexDigits ---

    [TestMethod]
    public void ExtractHexDigits_KeepsOnlyHexCharacters()
    {
        Assert.AreEqual("abc123DEF", "abcXYZ123-DEF".ExtractHexDigits());
    }

    // --- Chop ---

    [TestMethod]
    [DataRow("", "")]
    [DataRow("a", "")]
    [DataRow("abc", "ab")]
    [DataRow("abc\r\n", "abc")]
    public void Chop_RemovesLastCharacterOrCrLf(string input, string expected)
    {
        Assert.AreEqual(expected, input.Chop());
    }

    // --- CountLines ---

    [TestMethod]
    public void CountLines_CountsNewLines()
    {
        Assert.AreEqual(0, "".CountLines());
        Assert.AreEqual(1, "a".CountLines());
        Assert.AreEqual(2, ("a" + Environment.NewLine + "b").CountLines());
    }

    // --- FirstLine ---

    [TestMethod]
    public void FirstLine_ReturnsTextBeforeFirstNewLine()
    {
        Assert.AreEqual("a", ("a" + Environment.NewLine + "b").FirstLine());
        Assert.AreEqual("abc", "abc".FirstLine());
    }

    // --- IsAlphaNumeric ---

    [TestMethod]
    public void IsAlphaNumeric_DetectsNonAlphanumerics()
    {
        Assert.IsTrue("abc123".IsAlphaNumeric());
        Assert.IsFalse("abc 123".IsAlphaNumeric());
    }

    // --- LastCharAt ---

    [TestMethod]
    public void LastCharAt_ReturnsCharacterFromEnd()
    {
        Assert.AreEqual('c', "abc".LastCharAt(-1));
        Assert.AreEqual('a', "abc".LastCharAt(-3));
    }

    // --- MakeProper ---

    [TestMethod]
    public void MakeProper_UpperFirstRestLower()
    {
        Assert.AreEqual("Hello", "hELLO".MakeProper());
        Assert.AreEqual("", "".MakeProper());
    }

    // --- RemoveLastChar ---

    [TestMethod]
    public void RemoveLastChar_RemovesTrailingCharacter()
    {
        Assert.AreEqual("ab", "abc".RemoveLastChar());
        Assert.AreEqual("", "".RemoveLastChar());
    }

    // --- SplitAt ---

    [TestMethod]
    public void SplitAt_SplitsAtPositions()
    {
        var parts = "abcdef".SplitAt(2, 4).ToArray();
        Assert.AreSequenceEqual(SplitAtParts, parts);
    }

    [TestMethod]
    public void SplitAt_NoValidPositions_YieldsWholeString()
    {
        var none = "abc".SplitAt().ToArray();
        Assert.AreSequenceEqual(WholeString, none);

        var invalid = "abc".SplitAt(0, 5, 99).ToArray();
        Assert.AreSequenceEqual(WholeString, invalid);
    }

    // --- TrimEnd(int) ---

    [TestMethod]
    public void TrimEnd_RemovesTrailingLength()
    {
        Assert.AreEqual("abcd", "abcdef".TrimEnd(2));
        Assert.AreEqual("", "ab".TrimEnd(5));
    }

    // --- ReplaceLastOf ---

    [TestMethod]
    public void ReplaceLastOf_ReplacesOnlyLastOccurrence()
    {
        Assert.AreEqual("aXbYc", "aXbXc".ReplaceLastOf("X", "Y"));
        Assert.AreEqual("abc", "abc".ReplaceLastOf("Z", "Y"));
    }

    // --- ToCamelWithSeparator ---

    [TestMethod]
    public void ToCamelWithSeparator_InsertsSeparatorBetweenWords()
    {
        Assert.AreEqual("Hello_World", "HelloWorld".ToCamelWithSeparator('_'));
        Assert.AreEqual("", "".ToCamelWithSeparator('_'));
    }

    // --- IsNullOrEmptyOrEquals ---

    [TestMethod]
    public void IsNullOrEmptyOrEquals_HandlesNullEmptyAndComparison()
    {
        string? nul = null;
        Assert.IsTrue(nul.IsNullOrEmptyOrEquals(null, StringComparison.Ordinal));
        Assert.IsFalse("a".IsNullOrEmptyOrEquals("b", StringComparison.Ordinal));
        Assert.IsTrue("a".IsNullOrEmptyOrEquals("A", StringComparison.OrdinalIgnoreCase));
    }

    // --- Wrap / BreakLine ---

    [TestMethod]
    public void Wrap_WidthLessThanOne_ReturnsOriginal()
    {
        Assert.AreEqual("abc", "abc".Wrap(0));
    }

    [TestMethod]
    public void Wrap_WrapsLongLines()
    {
        var wrapped = "aaaa bbbb cccc".Wrap(6);

        Assert.Contains(Environment.NewLine, wrapped);
        Assert.Contains("aaaa", wrapped);
        Assert.Contains("cccc", wrapped);
    }

    [TestMethod]
    public void Wrap_MultiLineInput_KeepsShortLines()
    {
        var text = "short" + Environment.NewLine + "alsoshort";
        var wrapped = text.Wrap(50);

        Assert.Contains("short", wrapped);
        Assert.Contains("alsoshort", wrapped);
    }

    // --- Remove ---

    [TestMethod]
    public void Remove_RemovesSpecifiedCharacters()
    {
        Assert.AreEqual("acac", "abcabc".Remove(RemoveChars));
    }

    // --- GetBytes ---

    [TestMethod]
    public void GetBytes_ParsesHexPairs()
    {
        var bytes = "0011AABB".GetBytes(0, 2);
        Assert.AreSequenceEqual(HexPairs, bytes);
    }

    // --- ToEnum ---

    [TestMethod]
    public void ToEnum_ParsesOrFallsBackToDefault()
    {
        Assert.AreEqual(SampleEnum.Second, "Second".ToEnum(SampleEnum.First));
        Assert.AreEqual(SampleEnum.First, "nope".ToEnum(SampleEnum.First));
        string? nul = null;
        Assert.AreEqual(SampleEnum.First, nul.ToEnum(SampleEnum.First));
    }

    // --- Format ---

    [TestMethod]
    public void Format_FormatsArguments()
    {
        Assert.AreEqual("a-b", "{0}-{1}".Format("a", "b"));
    }

    // --- HasMethod ---

    [TestMethod]
    public void HasMethod_DetectsExistingMethod()
    {
        // List<int>.Add has a single, unambiguous overload.
        Assert.IsTrue(StringExtensions.HasMethod<System.Collections.Generic.List<int>>("Add"));
        Assert.IsFalse(StringExtensions.HasMethod<System.Collections.Generic.List<int>>("DoesNotExist"));
    }

    // --- ToSnakeCase ---

    [TestMethod]
    public void ToSnakeCase_InsertsUnderscores()
    {
        Assert.AreEqual("hello_world", "HelloWorld".ToSnakeCase());
    }

    // --- SplitAndKeep (empty) ---

    [TestMethod]
    public void SplitAndKeep_EmptyInput_ReturnsEmptyList()
    {
        Assert.IsEmpty("".SplitAndKeep('.'));
    }

    // --- ToEndingWithDirectorySeparator ---

    [TestMethod]
    public void ToEndingWithDirectorySeparator_Normalizes()
    {
        var sep = Path.DirectorySeparatorChar;
        string? nul = null;

        Assert.AreEqual("", nul.ToEndingWithDirectorySeparator());
        Assert.AreEqual("", "".ToEndingWithDirectorySeparator());
        Assert.AreEqual("abc" + sep, "abc".ToEndingWithDirectorySeparator());
        Assert.AreEqual("abc" + sep, ("abc" + sep).ToEndingWithDirectorySeparator());
    }

    // --- ExtractQueryParameter ---

    [TestMethod]
    public void ExtractQueryParameter_FindsValue()
    {
        Assert.AreEqual("development", "?environment=development&x=1".ExtractQueryParameter("environment"));
        Assert.AreEqual("hello world", "?a=hello%20world".ExtractQueryParameter("a"));
    }

    [TestMethod]
    public void ExtractQueryParameter_ReturnsNullForMissingOrInvalid()
    {
        Assert.IsNull("?a=1".ExtractQueryParameter("missing"));
        string? nul = null;
        Assert.IsNull(nul.ExtractQueryParameter("a"));
        Assert.IsNull("?a=1".ExtractQueryParameter(" "));
    }

    // --- ParseQueryString ---

    [TestMethod]
    public void ParseQueryString_ParsesPairsAndFlags()
    {
        var result = "?a=1&b=2&flag".ParseQueryString();

        Assert.HasCount(3, result);
        Assert.AreEqual("1", result["a"]);
        Assert.AreEqual("2", result["b"]);
        Assert.AreEqual("", result["flag"]);
    }

    [TestMethod]
    public void ParseQueryString_WithoutLeadingQuestionMark()
    {
        var result = "a=1".ParseQueryString();
        Assert.AreEqual("1", result["a"]);
    }

    [TestMethod]
    public void ParseQueryString_EmptyOrNull_ReturnsEmpty()
    {
        string? nul = null;
        Assert.IsEmpty(nul.ParseQueryString());
        Assert.IsEmpty("".ParseQueryString());
    }
}
