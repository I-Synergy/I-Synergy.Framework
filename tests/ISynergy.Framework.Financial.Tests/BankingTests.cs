using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Financial.Tests;

/// <summary>
/// Class BankingTests.
/// </summary>
[TestClass]
public class BankingTests
{
    /// <summary>
    /// Defines the test method CheckAccountBankTest.
    /// </summary>
    [TestMethod]
    public void CheckAccountBankTest()
    {
        bool result = Banking.ElevenTest("150483341");
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Defines the test method CheckAccountGiroTest.
    /// </summary>
    [TestMethod]
    public void CheckAccountGiroTest()
    {
        bool result = Banking.ElevenTest("8318140");
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Defines the test method CheckSofinummerTest.
    /// </summary>
    [TestMethod]
    public void CheckSofinummerTest()
    {
        bool result = Banking.ElevenTest("169649167");
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Defines the test method CheckAccountStringTest.
    /// </summary>
    [TestMethod]
    public void CheckAccountStringTest()
    {
        bool result = Banking.ElevenTest("abcdefghijklmnopqrstuvwxyz");
        Assert.IsFalse(result);
    }

    /// <summary>
    /// Defines the test method CheckAccountNumbersTest.
    /// </summary>
    [TestMethod]
    public void CheckAccountNumbersTest()
    {
        bool result = Banking.ElevenTest("123456789");
        Assert.IsTrue(result);
    }
}
