using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SetupTableTests
    {


        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_ShouldBe_Equals()
        {
            var table1 = new SetupTable("Customer");
            var table2 = new SetupTable("Customer");

            Assert.AreEqual(table1, table2);
            Assert.IsTrue(table1.Equals(table2));

            var table3 = new SetupTable("ProductCategory", "SalesLT");
            var table4 = new SetupTable("ProductCategory", "SalesLT");

            Assert.AreEqual(table3, table4);
            Assert.IsTrue(table3.Equals(table4));
            Assert.IsFalse(table3 == table4);
        }

        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_ShouldBe_Different()
        {
            var table1 = new SetupTable("Customer1");
            var table2 = new SetupTable("Customer2");

            Assert.AreNotEqual(table1, table2);
            Assert.IsFalse(table1.Equals(table2));

            var table3 = new SetupTable("ProductCategory", "dbo");
            var table4 = new SetupTable("ProductCategory", "SalesLT");

            Assert.AreNotEqual(table3, table4);
            Assert.IsFalse(table3.Equals(table4));
        }

        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_WithSameSyncDirection_ShouldBe_Equals()
        {
            var table1 = new SetupTable("Customer");
            var table2 = new SetupTable("Customer");

            table1.SyncDirection = SyncDirection.Bidirectional;
            table2.SyncDirection = SyncDirection.Bidirectional;

            Assert.AreEqual(table1, table2);
            Assert.IsTrue(table1.Equals(table2));
        }

        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_WithDifferentSyncDirection_ShouldBe_Different()
        {
            var table1 = new SetupTable("Customer");
            var table2 = new SetupTable("Customer");

            table1.SyncDirection = SyncDirection.UploadOnly;
            table2.SyncDirection = SyncDirection.Bidirectional;

            Assert.AreNotEqual(table1, table2);
            Assert.IsFalse(table1.Equals(table2));
        }

        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_WithSameColumns_ShouldBe_Equals()
        {
            var table1 = new SetupTable("Customer");
            var table2 = new SetupTable("Customer");

            table1.Columns.Add("CustomerID");
            table2.Columns.Add("CustomerID");

            Assert.AreEqual(table1, table2);
            Assert.IsTrue(table1.Equals(table2));
        }
        [TestMethod]
        public void SetupTable_Compare_TwoSetupTables_WithDifferentColumns_ShouldBe_Equals()
        {
            var table1 = new SetupTable("Customer");
            var table2 = new SetupTable("Customer");

            table1.Columns.Add("CustomerID");

            Assert.AreNotEqual(table1, table2);
            Assert.IsFalse(table1.Equals(table2));

            table2.Columns.Add("ID");

            Assert.AreNotEqual(table1, table2);
            Assert.IsFalse(table1.Equals(table2));

        }
    }
}
