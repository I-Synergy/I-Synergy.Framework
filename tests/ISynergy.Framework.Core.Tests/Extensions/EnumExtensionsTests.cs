using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Tests.Fixtures.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Extensions
{
    /// <summary>
    /// Class EnumExtensionsTests.
    /// </summary>
    [TestClass]
    public class EnumExtensionsTests
    {
        /// <summary>
        /// Defines the test method GetDescriptionTest.
        /// </summary>
        [TestMethod]
        public void GetDescriptionTest()
        {
            Assert.AreEqual("Description1", ProductTypes.Type1.GetDescription());
            Assert.AreEqual("Description2", ProductTypes.Type2.GetDescription());
            Assert.AreEqual("Description3", ProductTypes.Type3.GetDescription());
        }
    }
}
