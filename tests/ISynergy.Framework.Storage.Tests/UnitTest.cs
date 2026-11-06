using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Storage.Tests;

/// <summary>
/// Class UnitTest.
/// </summary>
[TestClass]
public class UnitTest
{
    /// <summary>
    /// Defines the test method Test.
    /// </summary>
    [TestMethod]
    public void Test()
    {
        // Keeps the test project discoverable by the runner (a zero-test run fails under
        // Microsoft.Testing.Platform). The assertion checks a real invariant of the assembly.
        Assert.IsFalse(string.IsNullOrWhiteSpace(typeof(UnitTest).Assembly.GetName().Name));
    }
}
