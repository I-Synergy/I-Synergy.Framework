using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Synchronization.Core.Tests.Database
{
    [TestClass]
    public class SyncTableTests
    {
        [TestMethod]
        public void SyncTable_Equals_ShouldWork()
        {
            var table1 = new SyncTable("Customer");
            var table2 = new SyncTable("Customer");

            Assert.AreEqual(table1, table2);
            Assert.IsTrue(table1.Equals(table2));

            var table3 = new SyncTable("ProductCategory", "SalesLT");
            var table4 = new SyncTable("ProductCategory", "SalesLT");

            Assert.AreEqual(table3, table4);
            Assert.IsTrue(table3.Equals(table4));
        }

        [TestMethod]
        public void SyncTable_EnsureSchema_Check_ColumnsAndRows()
        {
            var tCustomer = new SyncTable("Customer");
            tCustomer.Columns.Add(new SyncColumn("ID", typeof(Guid)));
            tCustomer.Columns.Add(new SyncColumn("Name", typeof(string)));

            var tCustomerRow = new SyncRow(tCustomer);
            tCustomerRow["ID"] = "A";
            tCustomerRow["Name"] = "B";

            tCustomer.Rows.Add(tCustomerRow);

            tCustomer.EnsureTable(new SyncSet());

            Assert.AreEqual(tCustomer, tCustomer.Columns.Table);
            Assert.AreEqual(tCustomer, tCustomer.Rows.Table);

        }
    }
}
