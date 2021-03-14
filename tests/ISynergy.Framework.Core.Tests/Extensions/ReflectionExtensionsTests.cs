using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class ReflectionExtensionsTests.
    /// </summary>
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
        [Fact]
        public void GetIdentityPropertyTest()
        {
            Assert.NotNull(_product.GetIdentityProperty());
            Assert.Equal(nameof(Product.ProductId), _product.GetIdentityProperty().Name);
        }

        /// <summary>
        /// Defines the test method GetNoIdentityPropertyTest.
        /// </summary>
        [Fact]
        public void GetNoIdentityPropertyTest()
        {
            Assert.Null(_productGroup.GetIdentityProperty());
        }

        /// <summary>
        /// Defines the test method GetIdentityValueTest.
        /// </summary>
        [Fact]
        public void GetIdentityValueTest()
        {
            Assert.NotNull(_product.GetIdentityValue());
            Assert.Equal(_productId, _product.GetIdentityValue());
        }

        /// <summary>
        /// Defines the test method GetNoIdentityValueTest.
        /// </summary>
        [Fact]
        public void GetNoIdentityValueTest()
        {
            Assert.Null(_productGroup.GetIdentityValue());
        }

        /// <summary>
        /// Defines the test method HasIdentityPropertyTest.
        /// </summary>
        [Fact]
        public void HasIdentityPropertyTest()
        {
            Assert.True(_product.HasIdentityProperty());
        }

        /// <summary>
        /// Defines the test method HasNoIdentityPropertyTest.
        /// </summary>
        [Fact]
        public void HasNoIdentityPropertyTest()
        {
            Assert.False(_productGroup.HasIdentityProperty());
        }
    }
}
