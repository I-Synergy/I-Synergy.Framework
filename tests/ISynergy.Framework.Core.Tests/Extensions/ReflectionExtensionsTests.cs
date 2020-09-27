using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class ReflectionExtensionsTests
    {
        private Guid _productId;
        private Product _product;
        private ProductGroup _productGroup;

        public ReflectionExtensionsTests()
        {
            _productId = Guid.NewGuid();
            _product = new Product(_productId, "Test", 1, 1m);
            _productGroup = new ProductGroup();
        }

        [Fact]
        public void GetIdentityPropertyTest()
        {
            Assert.NotNull(_product.GetIdentityProperty());
            Assert.Equal(nameof(Product.ProductId), _product.GetIdentityProperty().Name);
        }

        [Fact]
        public void GetNoIdentityPropertyTest()
        {
            Assert.Null(_productGroup.GetIdentityProperty());
        }

        [Fact]
        public void GetIdentityValueTest()
        {
            Assert.NotNull(_product.GetIdentityValue());
            Assert.Equal(_productId, _product.GetIdentityValue());
        }

        [Fact]
        public void GetNoIdentityValueTest()
        {
            Assert.Null(_productGroup.GetIdentityValue());
        }

        [Fact]
        public void HasIdentityPropertyTest()
        {
            Assert.True(_product.HasIdentityProperty());
        }

        [Fact]
        public void HasNoIdentityPropertyTest()
        {
            Assert.False(_productGroup.HasIdentityProperty());
        }
    }
}
