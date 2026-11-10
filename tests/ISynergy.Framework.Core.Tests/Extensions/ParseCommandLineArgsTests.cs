using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Tests for the refactored <see cref="StringExtensions.ParseCommandLineArgs(string)"/> helper
/// (the skipPairedValue loop / hasPairedValue restructure).
/// </summary>
[TestClass]
public class ParseCommandLineArgsTests
{
    // --- Long form "--key value" ---

    [TestMethod]
    public void ParseCommandLineArgs_LongForm_PairsFlagWithValue()
    {
        var result = "--environment development".ParseCommandLineArgs();

        Assert.HasCount(1, result);
        Assert.AreEqual("development", result["environment"]);
    }

    [TestMethod]
    public void ParseCommandLineArgs_MultipleLongFormFlags_AllPaired()
    {
        var result = "--environment development --config appsettings.json".ParseCommandLineArgs();

        Assert.HasCount(2, result);
        Assert.AreEqual("development", result["environment"]);
        Assert.AreEqual("appsettings.json", result["config"]);
    }

    // --- Short form "-k value" ---

    [TestMethod]
    public void ParseCommandLineArgs_ShortForm_PairsFlagWithValue()
    {
        var result = "-e production".ParseCommandLineArgs();

        Assert.HasCount(1, result);
        Assert.AreEqual("production", result["e"]);
    }

    [TestMethod]
    public void ParseCommandLineArgs_MixedShortAndLong_Paired()
    {
        var result = "-e production --config appsettings.json".ParseCommandLineArgs();

        Assert.HasCount(2, result);
        Assert.AreEqual("production", result["e"]);
        Assert.AreEqual("appsettings.json", result["config"]);
    }

    // --- skipPairedValue branch: value is consumed, not re-parsed as a flag ---

    [TestMethod]
    public void ParseCommandLineArgs_PairedValue_IsConsumedNotReprocessed()
    {
        // The value "--value" is consumed by the -k pairing; only one entry results.
        var result = "-k --value".ParseCommandLineArgs();

        // Next token starts with '-' so no pairing happens at all.
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ParseCommandLineArgs_FlagWithoutValue_IsIgnored()
    {
        var result = "--verbose".ParseCommandLineArgs();

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ParseCommandLineArgs_TwoConsecutiveFlags_NeitherPaired()
    {
        // Neither flag has a non-dash follower, so nothing is captured.
        var result = "--flag1 --flag2".ParseCommandLineArgs();

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ParseCommandLineArgs_FlagAtEndAfterValue_PairsOnlyFirst()
    {
        var result = "--environment development --verbose".ParseCommandLineArgs();

        Assert.HasCount(1, result);
        Assert.AreEqual("development", result["environment"]);
    }

    // --- hasPairedValue: next token beginning with '-' is not treated as a value ---

    [TestMethod]
    public void ParseCommandLineArgs_ValueStartingWithDash_NotConsumedAsValue()
    {
        var result = "--key -5".ParseCommandLineArgs();

        Assert.IsEmpty(result);
    }

    // --- Empty / whitespace handling ---

    [TestMethod]
    public void ParseCommandLineArgs_EmptyString_ReturnsEmptyDictionary()
    {
        var result = string.Empty.ParseCommandLineArgs();

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ParseCommandLineArgs_LowercaseKeys_AreCaseInsensitive()
    {
        var result = "--Environment Development".ParseCommandLineArgs();

        Assert.AreEqual("Development", result["environment"]);
        Assert.AreEqual("Development", result["ENVIRONMENT"]);
    }
}
