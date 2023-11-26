using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Data.Tests
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
            Product product = new()
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
            Product product = new()
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
            Product product = new(
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
            Product product = new()
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
            Product product = new()
            {
                Name = "Test2",
                Date = DateTimeOffset.Now
            };

            Assert.IsTrue(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_3.
        /// </summary>
        [TestMethod]
        public void CheckIfObjectAfterInitializationIsDirty_3()
        {
            Product product = new(
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
            Product product = new();
            Assert.IsFalse(product.Validate());
            Assert.IsTrue(product.Errors.Count > 0);
        }

        /// <summary>
        /// Test to check if property with Required or Identity attributes and has a non-null value generates an validation error.
        /// </summary>
        [TestMethod]
        public void TestIfNonNullValueGeneratesNoValidationError()
        {
            Product product = new()
            {
                Name = "Test",
                ProductGroups =
                [
                    new ProductGroup { Description = "Test"}
                ],
                Quantity = 1
            };

            Assert.IsTrue(product.Validate());
            Assert.IsTrue(product.Errors.Count == 0);
        }

        [TestMethod]
        public void TestIfProductsAreEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var name = "Test1";
            var quantity = 2;
            var price = 10.0m;

            var product1 = new Product(productId, name, quantity, price);
            var product2 = new Product(productId, name, quantity, price);

            // Act
            var isEqual = product1.Equals(product2);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void TestIfProductsWithoutIdentityAreEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var name = "Test1";
            var quantity = 2;
            var price = 10.0m;

            var product1 = new ProductWithoutIdentity(productId, name, quantity, price);
            var product2 = new ProductWithoutIdentity(productId, name, quantity, price);

            // Act
            var isEqual = product1.Equals(product2);

            // Assert
            Assert.IsTrue(isEqual);
        }
    }
}
