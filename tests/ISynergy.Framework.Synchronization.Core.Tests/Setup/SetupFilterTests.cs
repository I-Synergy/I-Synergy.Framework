using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SetupFilterTests
    {
        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_ShouldBe_Equals()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product");
            filter2 = new SetupFilter("Product");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product", "");
            filter2 = new SetupFilter("Product");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product");
            filter2 = new SetupFilter("Product", string.Empty);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product", "dbo");
            filter2 = new SetupFilter("Product", "dbo");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));
        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_ShouldBe_Different()
        {

            var filter1 = new SetupFilter("Product1");
            var filter2 = new SetupFilter("Product2");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product", "");
            filter2 = new SetupFilter("Product", "d");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product", "d");
            filter2 = new SetupFilter("Product", string.Empty);

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter("Product", "dbo");
            filter2 = new SetupFilter("Product", "SalesLT");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));
        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Parameters_ShouldBe_Equals()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product");
            filter2.AddParameter("ProductId", "Product");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product", true);
            filter2.AddParameter("ProductId", "Product", true);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product", true, "12");
            filter2.AddParameter("ProductId", "Product", true, "12");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Int32);
            filter2.AddParameter("ProductId", DbType.Int32);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Int32, true);
            filter2.AddParameter("ProductId", DbType.Int32, true);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Int32, true, "12");
            filter2.AddParameter("ProductId", DbType.Int32, true, "12");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Parameters_ShouldBe_Different()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddParameter("Product", "Product");
            filter2.AddParameter("ProductId", "Product");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product1");
            filter2.AddParameter("ProductId", "Product2");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product", false);
            filter2.AddParameter("ProductId", "Product", true);

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", "Product", true, "2");
            filter2.AddParameter("ProductId", "Product", true, "12");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.String);
            filter2.AddParameter("ProductId", DbType.Int32);

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Int32, true);
            filter2.AddParameter("ProductId", DbType.Int32, false);

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Int32, true, "2");
            filter2.AddParameter("ProductId", DbType.Int32, true, "12");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));
        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_CustomWhere_ShouldBe_Equals()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddCustomWhere("where 1");
            filter2.AddCustomWhere("where 1");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddCustomWhere("");
            filter2.AddCustomWhere("");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1.AddCustomWhere(null);
            filter2.AddCustomWhere(null);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1.AddCustomWhere("");
            filter2.AddCustomWhere(null);

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_CustomWhere_ShouldBe_Different()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddCustomWhere("where 1");
            filter2.AddCustomWhere("where 2");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddCustomWhere("");
            filter2.AddCustomWhere("a");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1.AddCustomWhere("b");
            filter2.AddCustomWhere(null);

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Joins_ShouldBe_Equals()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory", "ProductId");
            filter2.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory", "ProductId");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));
        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Joins_ShouldBe_Different()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddJoin(Join.Inner, "Product1").On("Product", "ProductId", "ProductCategory", "ProductId");
            filter2.AddJoin(Join.Inner, "Product2").On("Product", "ProductId", "ProductCategory", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddJoin(Join.Inner, "Product").On("Product1", "ProductId", "ProductCategory", "ProductId");
            filter2.AddJoin(Join.Inner, "Product").On("Product2", "ProductId", "ProductCategory", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1.AddJoin(Join.Inner, "Product").On("Product", "ProductId1", "ProductCategory", "ProductId");
            filter2.AddJoin(Join.Inner, "Product").On("Product", "ProductId2", "ProductCategory", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory1", "ProductId");
            filter2.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory2", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory", "ProductId1");
            filter2.AddJoin(Join.Inner, "Product").On("Product", "ProductId", "ProductCategory", "ProductId2");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

        }


        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Where_ShouldBe_Equals()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId", "dbo");
            filter2.AddWhere("ProductId", "Product", "ProductId", "dbo");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId");
            filter2.AddWhere("ProductId", "Product", "ProductId");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId", "");
            filter2.AddWhere("ProductId", "Product", "ProductId");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId");
            filter2.AddWhere("ProductId", "Product", "ProductId", "");

            Assert.AreEqual(filter1, filter2);
            Assert.IsTrue(filter1.EqualsByProperties(filter2));

        }

        [TestMethod]
        public void SetupFilter_Compare_TwoSetupFilters_With_Where_ShouldBe_Different()
        {
            var filter1 = new SetupFilter();
            var filter2 = new SetupFilter();


            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId", "dbo");
            filter2.AddWhere("ProductId", "Product", "ProductId", "SalesLT");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId1", "Product", "ProductId");
            filter2.AddWhere("ProductId2", "Product", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId", DbType.Guid);
            filter2.AddParameter("ProductId", DbType.Guid);
            filter1.AddWhere("ProductId", "Product1", "ProductId", "");
            filter2.AddWhere("ProductId", "Product2", "ProductId");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

            filter1 = new SetupFilter();
            filter2 = new SetupFilter();

            filter1.AddParameter("ProductId1", DbType.Guid);
            filter2.AddParameter("ProductId2", DbType.Guid);
            filter1.AddWhere("ProductId", "Product", "ProductId1");
            filter2.AddWhere("ProductId", "Product", "ProductId2");

            Assert.AreNotEqual(filter1, filter2);
            Assert.IsFalse(filter1.EqualsByProperties(filter2));

        }
    }
}
