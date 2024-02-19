using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Utilities.Tests;

/// <summary>
/// Class TypeExtensionsTests.
/// </summary>
[TestClass]
public class TypeExtensionsTests
{
    /// <summary>
    /// Defines the test method BasicTypeActivatorTest.
    /// </summary>
    /// <param name="type">The type.</param>
    [DataTestMethod]
    [DataRow(typeof(string))]
    [DataRow(typeof(bool))]
    [DataRow(typeof(object))]
    [DataRow(typeof(Guid))]
    [DataRow(typeof(Product))]
    public void BasicTypeActivatorTest(Type type)
    {
        object result = TypeActivator.CreateInstance(type);
        Assert.IsNotNull(result);
    }

}
