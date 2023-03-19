using ISynergy.Framework.Core.Data.TestClasses.Tests;
using ISynergy.Framework.Core.Tests.Data.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
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

        /// <summary>
        /// Defines the test method GetDescriptionTest.
        /// </summary>
        [TestMethod]
        public void GetLocalizedDescriptionTest()
        {
            Assert.AreEqual("Type1", ProductTypes.Type1.GetLocalizedDescription());
            Assert.AreEqual("Type2", ProductTypes.Type2.GetLocalizedDescription());
            Assert.AreEqual("Type3", ProductTypes.Type3.GetLocalizedDescription());
        }

        [TestMethod()]
        public void ToListTest()
        {
            System.Collections.Generic.List<System.Enum> list = typeof(TestEnum).ToList();
            Assert.AreEqual(4, list.Count);
        }
    }
}
