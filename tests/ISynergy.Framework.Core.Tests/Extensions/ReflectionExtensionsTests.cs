using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ReflectionExtensionsTests.
    /// </summary>
    [TestClass]
    public class ReflectionExtensionsTests
    {
        /// <summary>
        /// The product identifier
        /// </summary>
        private Guid _productId;
        /// <summary>
        /// The product
        /// </summary>
        private Product _product;
        /// <summary>
        /// The product group
        /// </summary>
        private ProductGroup _productGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionExtensionsTests" /> class.
        /// </summary>
        public ReflectionExtensionsTests()
        {
            _productId = Guid.NewGuid();
            _product = new Product(_productId, "Test", 1, 1m);
            _productGroup = new ProductGroup();
        }

        /// <summary>
        /// Defines the test method GetIdentityPropertyTest.
        /// </summary>
        [TestMethod]
        public void GetIdentityPropertyTest()
        {
            Assert.IsNotNull(_product.GetIdentityProperty());
            Assert.AreEqual(nameof(Product.ProductId), _product.GetIdentityProperty().Name);
        }

        /// <summary>
        /// Defines the test method GetNoIdentityPropertyTest.
        /// </summary>
        [TestMethod]
        public void GetNoIdentityPropertyTest()
        {
            Assert.IsNull(_productGroup.GetIdentityProperty());
        }

        /// <summary>
        /// Defines the test method GetIdentityValueTest.
        /// </summary>
        [TestMethod]
        public void GetIdentityValueTest()
        {
            Assert.IsNotNull(_product.GetIdentityValue());
            Assert.AreEqual(_productId, _product.GetIdentityValue());
        }

        /// <summary>
        /// Defines the test method GetNoIdentityValueTest.
        /// </summary>
        [TestMethod]
        public void GetNoIdentityValueTest()
        {
            Assert.IsNull(_productGroup.GetIdentityValue());
        }

        /// <summary>
        /// Defines the test method HasIdentityPropertyTest.
        /// </summary>
        [TestMethod]
        public void HasIdentityPropertyTest()
        {
            Assert.IsTrue(_product.HasIdentityProperty());
        }

        /// <summary>
        /// Defines the test method HasNoIdentityPropertyTest.
        /// </summary>
        [TestMethod]
        public void HasNoIdentityPropertyTest()
        {
            Assert.IsFalse(_productGroup.HasIdentityProperty());
        }
    }
}
