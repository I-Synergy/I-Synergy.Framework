using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

/// <summary>
/// Class ListExtensionsTests.
/// </summary>
[TestClass]
public class ListExtensionsTests
{
    /// <summary>
    /// Defines the test method NullListNonFailableTest.
    /// </summary>
    [TestMethod]
    public void NullListNonFailableTest()
    {
        List<object>? list = null;
        bool result = false;

        foreach (object item in list.EnsureNotNull())
        {
        }

        result = true;

        Assert.IsTrue(result);
    }

    /// <summary>
    /// Defines the test method NullListFailableTest.
    /// </summary>
    [TestMethod]
    public void NullListFailableTest()
    {
        Assert.ThrowsAsync<NullReferenceException>(() =>
        {
            List<object>? list = null;

            foreach (object item in list!)
            {
            }

            return Task.CompletedTask;
        });
    }
}
