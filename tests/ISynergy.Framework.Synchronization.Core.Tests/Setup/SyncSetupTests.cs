using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SyncSetupTests
    {
        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_ShouldBe_Equals()
        {
            var setup1 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            var setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Employee", "dbo"));

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Employee", "dbo"));
            setup1.Tables.Add(new SetupTable("Product"));
            setup2.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Product"));

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Product"));
            setup1.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Product"));

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));
        }

        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_ShouldBe_Different()
        {
            var setup1 = new SyncSetup(new string[] { "Product1", "ProductCategory", "Employee" });
            var setup2 = new SyncSetup(new string[] { "Product2", "ProductCategory", "Employee" });

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory1", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory2", "Employee" });

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Employee", ""));
            setup2.Tables.Add(new SetupTable("Employee", "dbo"));

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Employee", ""));
            setup1.Tables.Add(new SetupTable("Product"));
            setup2.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Product"));

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup();
            setup2 = new SyncSetup();
            setup1.Tables.Add(new SetupTable("Product"));
            setup1.Tables.Add(new SetupTable("Employee", "dbo"));
            setup2.Tables.Add(new SetupTable("Employee", ""));
            setup2.Tables.Add(new SetupTable("Product"));

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));
        }

        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_Properties_ShouldBe_Equals()
        {
            var setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            var setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresPrefix = "sp";
            setup2.StoredProceduresPrefix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresSuffix = "sp";
            setup2.StoredProceduresSuffix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersPrefix = "sp";
            setup2.TriggersPrefix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersSuffix = "sp";
            setup2.TriggersSuffix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesPrefix = "sp";
            setup2.TrackingTablesPrefix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesSuffix = "sp";
            setup2.TrackingTablesSuffix = "sp";

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });

            Assert.AreEqual(setup1, setup2);
            Assert.IsTrue(setup1.Equals(setup2));
        }
        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_Properties_ShouldBe_Different()
        {
            var setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            var setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresPrefix = "sp1";
            setup2.StoredProceduresPrefix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresPrefix = null;
            setup2.StoredProceduresPrefix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresSuffix = "sp1";
            setup2.StoredProceduresSuffix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.StoredProceduresSuffix = null;
            setup2.StoredProceduresSuffix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersPrefix = "sp1";
            setup2.TriggersPrefix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersPrefix = "sp1";
            setup2.TriggersPrefix = null;

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersSuffix = "sp1";
            setup2.TriggersSuffix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TriggersSuffix = "sp1";
            setup2.TriggersSuffix = null;

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesPrefix = "sp1";
            setup2.TrackingTablesPrefix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesPrefix = null;
            setup2.TrackingTablesPrefix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesSuffix = "sp1";
            setup2.TrackingTablesSuffix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

            setup1 = new SyncSetup(new string[] { "Employee", "ProductCategory", "Product" });
            setup2 = new SyncSetup(new string[] { "Product", "ProductCategory", "Employee" });
            setup1.TrackingTablesSuffix = null;
            setup2.TrackingTablesSuffix = "sp";

            Assert.AreNotEqual(setup1, setup2);
            Assert.IsFalse(setup1.Equals(setup2));

        }

        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_With_Filters_ShouldBe_Equals()
        {
            var setup1 = new SyncSetup(new string[] { "Customer", "Product", "ProductCategory", "Employee" });
            var setup2 = new SyncSetup(new string[] { "Customer", "Product", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            var addressCustomerFilter = new SetupFilter("CustomerAddress");
            addressCustomerFilter.AddParameter("CompanyName", "Customer");
            addressCustomerFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressCustomerFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(addressCustomerFilter);

            var addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(addressFilter);

            var orderHeaderFilter = new SetupFilter("SalesOrderHeader");
            orderHeaderFilter.AddParameter("CompanyName", "Customer");
            orderHeaderFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderHeaderFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderHeaderFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(orderHeaderFilter);

            var orderDetailsFilter = new SetupFilter("SalesOrderDetail");
            orderDetailsFilter.AddParameter("CompanyName", "Customer");
            orderDetailsFilter.AddJoin(Join.Left, "SalesOrderHeader").On("SalesOrderDetail", "SalesOrderID", "SalesOrderHeader", "SalesOrderID");
            orderDetailsFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderDetailsFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderDetailsFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(orderDetailsFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            var addressCustomerFilter2 = new SetupFilter("CustomerAddress");
            addressCustomerFilter2.AddParameter("CompanyName", "Customer");
            addressCustomerFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressCustomerFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(addressCustomerFilter2);

            var addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(addressFilter2);

            var orderHeaderFilter2 = new SetupFilter("SalesOrderHeader");
            orderHeaderFilter2.AddParameter("CompanyName", "Customer");
            orderHeaderFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderHeaderFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderHeaderFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(orderHeaderFilter2);

            var orderDetailsFilter2 = new SetupFilter("SalesOrderDetail");
            orderDetailsFilter2.AddParameter("CompanyName", "Customer");
            orderDetailsFilter2.AddJoin(Join.Left, "SalesOrderHeader").On("SalesOrderDetail", "SalesOrderID", "SalesOrderHeader", "SalesOrderID");
            orderDetailsFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderDetailsFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderDetailsFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(orderDetailsFilter2);


            Assert.IsTrue(setup1.EqualsByProperties(setup2));
        }

        [TestMethod]
        public void SyncSetup_Compare_TwoSetup_With_Filters_ShouldBe_Different()
        {
            // Check Setup shoul be differents when tables names count is not same
            var setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory" });
            var setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // Check Setup should be differents when tables names are differents
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee1" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee2" });
            Assert.IsFalse(setup1.EqualsByProperties(setup2));


            // Check when Setup Filter names are differente (Customer1 and Customer2)
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer1", "CompanyName");

            var addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer2", "CompanyName");

            var addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // 2) Check when Setup Filter names are differente (Address1 and Address2)
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            addressFilter = new SetupFilter("Address1");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            addressFilter2 = new SetupFilter("Address2");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // 3) Check when Setup Parameter names are differente (CompanyName1 and CompanyName2)
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName1", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName1");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName2", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName2");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // 4) Check when Setup Joins names are differente (CustomerAddress1 and CustomerAddress2)
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress1").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress2").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // 5) Check when Setup Where names are differente (CompanyName1 and CompanyName2)
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName1", "Customer", "CompanyName");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName2", "Customer", "CompanyName");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

            // 6) Check CustomWhere differences
            setup1 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });
            setup2 = new SyncSetup(new string[] { "Customer", "Address", "ProductCategory", "Employee" });

            setup1.Filters.Add("Customer", "CompanyName");

            addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            addressFilter.AddCustomWhere("ID = @ID2");
            setup1.Filters.Add(addressFilter);

            setup2.Filters.Add("Customer", "CompanyName");

            addressFilter2 = new SetupFilter("Address");
            addressFilter2.AddParameter("CompanyName", "Customer");
            addressFilter2.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter2.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter2.AddWhere("CompanyName", "Customer", "CompanyName");
            addressFilter2.AddCustomWhere("ID = @ID1");
            setup2.Filters.Add(addressFilter2);

            Assert.IsFalse(setup1.EqualsByProperties(setup2));

        }
    }
}
