using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Utilities.Tests;

/// <summary>
/// Class GuidToIntTest.
/// </summary>
[TestClass]
public class GuidToIntTest
{
    /// <summary>
    /// Defines the test method ConvertTest.
    /// </summary>
    [TestMethod]
    public void ConvertTest()
    {
        int number = 1975;
        System.Guid EnryptedGuid = number.ToGuid();
        int result = EnryptedGuid.ToInt();

        Assert.AreEqual(number, result);
    }
}
