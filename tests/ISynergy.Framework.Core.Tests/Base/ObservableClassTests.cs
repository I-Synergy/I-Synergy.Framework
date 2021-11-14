using ISynergy.Framework.Core.Tests.Fixtures.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Base
{
    /// <summary>
    /// Class ObservableClassTests.
    /// </summary>
    [TestClass]
    public class ObservableClassTests
    {
        // Check when object is initialized that it's clean.
        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_1.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsClean_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };
            product.MarkAsClean();

            Assert.IsFalse(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_2.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsClean_2()
        {
            var product = new Product
            {
                Name = "Test2"
            };
            product.MarkAsClean();

            Assert.IsFalse(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_3.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsClean_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100);

            Assert.IsFalse(product.IsDirty);
        }

        // Check when object is initialized that it's not dirty
        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_1.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsDirty_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };

            Assert.IsTrue(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_2.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsDirty_2()
        {
            var product = new Product { Name = "Test2" };
            product.Date = DateTimeOffset.Now;

            Assert.IsTrue(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_3.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsDirty_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100)
            {
                Date = null
            };

            Assert.IsTrue(product.IsDirty);
        }

        /// <summary>
        /// Test to check if property with Required or Identity attributes and has a null value generates an validation error.
        /// </summary>
        [TestMethod]
        public void TestIfNullValueGeneratesValidationError()
        {
            var product = new Product();
            product.Validate();
            Assert.IsTrue(product.Errors.Count > 0);
        }

        /// <summary>
        /// Test to check if property with Required or Identity attributes and has a non-null value generates an validation error.
        /// </summary>
        [TestMethod]
        public void TestIfNonNullValueGeneratesNoValidationError()
        {
            var product = new Product();
            product.Name = "Test";
            product.Validate();
            Assert.IsTrue(product.Errors.Count == 0);
        }
    }
}
