using ISynergy.Framework.Core.Tests.Fixtures.TestClasses;
using ISynergy.Framework.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Extensions
{
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
            var result = TypeActivator.CreateInstance(type);
            Assert.IsNotNull(result);
        }

    }
}
