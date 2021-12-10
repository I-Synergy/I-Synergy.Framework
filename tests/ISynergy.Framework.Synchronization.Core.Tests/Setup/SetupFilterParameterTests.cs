using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SetupFilterParameterTests
    {
        [TestMethod]
        public void SetupFilterParameter_Compare_TwoSetupFilterParameter_ShouldBe_Equals()
        {
            var filterParam1 = new SetupFilterParameter();
            var filterParam2 = new SetupFilterParameter();

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));
            Assert.IsFalse(filterParam1 == filterParam2);

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();

            filterParam2.SchemaName = "";

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.TableName = "Product";
            filterParam2.TableName = "Product";

            filterParam1.SchemaName = string.Empty;

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.SchemaName = "dbo";
            filterParam2.SchemaName = "dbo";

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();

            filterParam1.AllowNull = true;
            filterParam2.AllowNull = true;

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.DbType = null;
            filterParam2.DbType = null;

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.DbType = DbType.String;
            filterParam2.DbType = DbType.String;

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.DefaultValue = "12";
            filterParam2.DefaultValue = "12";

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));

            filterParam1.MaxLength = 100;
            filterParam2.MaxLength = 100;

            Assert.AreEqual(filterParam1, filterParam2);
            Assert.IsTrue(filterParam1.Equals(filterParam2));
        }

        [TestMethod]
        public void SetupFilterParameter_Compare_TwoSetupFilterParameter_ShouldBe_Different()
        {
            var filterParam1 = new SetupFilterParameter();
            var filterParam2 = new SetupFilterParameter();

            filterParam2.SchemaName = "dbo";

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1.TableName = "Product1";
            filterParam2.TableName = "Product2";

            filterParam1.SchemaName = string.Empty;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1.SchemaName = "dbo";
            filterParam2.SchemaName = "SalesLT";

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();

            filterParam1.AllowNull = false;
            filterParam2.AllowNull = true;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();
            filterParam1.DbType = null;
            filterParam2.DbType = DbType.String;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();
            filterParam1.DbType = DbType.String;
            filterParam2.DbType = DbType.Int32;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();
            filterParam1.DefaultValue = "12";
            filterParam2.DefaultValue = null;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));

            filterParam1 = new SetupFilterParameter();
            filterParam2 = new SetupFilterParameter();
            filterParam1.MaxLength = 100;
            filterParam2.MaxLength = 0;

            Assert.AreNotEqual(filterParam1, filterParam2);
            Assert.IsFalse(filterParam1.Equals(filterParam2));
        }

    }
}
