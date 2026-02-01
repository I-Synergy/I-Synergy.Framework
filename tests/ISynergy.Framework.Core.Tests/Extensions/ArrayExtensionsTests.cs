using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

/// <summary>
/// Class ArrayExtensionsTests.
/// </summary>
[TestClass]
public class ArrayExtensionsTests
{
    /// <summary>
    /// Defines the test method NullArrayNonFailableTest.
    /// </summary>
    [TestMethod]
    public void NullArrayNonFailableTest()
    {
        object[]? list = null;
        bool result = false;

        foreach (object item in list.EnsureNotNull())
        {
        }

        result = true;

        Assert.IsTrue(result);
    }

    /// <summary>
    /// Defines the test method NullArrayFailableTest.
    /// </summary>
    [TestMethod]
    public void NullArrayFailableTest()
    {
        Assert.ThrowsAsync<NullReferenceException>(() =>
        {
            object[]? list = null;

            foreach (object item in list!)
            {
            }

            return Task.CompletedTask;
        });
    }
}
