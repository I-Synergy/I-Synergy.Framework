using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Synchronization.Core.Tests.Setup
{
    [TestClass]
    public class SetupFilterWhereTests
    {
        [TestMethod]
        public void SetupFilterWhere_Compare_TwoSetupFilterWhere_ShouldBe_Equals()
        {
            var where1 = new SetupFilterWhere();
            var where2 = new SetupFilterWhere();

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));
            Assert.IsFalse(where1 == where2);

            where1 = new SetupFilterWhere();
            where2 = new SetupFilterWhere();

            where2.SchemaName = "";

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));

            where1.TableName = "Product";
            where2.TableName = "Product";

            where1.SchemaName = string.Empty;

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));

            where1.SchemaName = "dbo";
            where2.SchemaName = "dbo";

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));

            where1 = new SetupFilterWhere();
            where2 = new SetupFilterWhere();

            where1.ParameterName = "@param1";
            where2.ParameterName = "@param1";

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));

            where1.ColumnName = "ProductID";
            where2.ColumnName = "ProductID";

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));
        }

        [TestMethod]
        public void SetupFilterWhere_Compare_TwoDifferentSetupFilterWhere_ShouldBe_Different()
        {
            var where1 = new SetupFilterWhere();
            var where2 = new SetupFilterWhere();

            Assert.AreEqual(where1, where2);
            Assert.IsTrue(where1.Equals(where2));

            where1.TableName = "Product1";
            where2.TableName = "Product2";

            Assert.AreNotEqual(where1, where2);
            Assert.IsFalse(where1.Equals(where2));

            where1.TableName = "Product1";
            where2.TableName = "Product1";
            where1.SchemaName = "SalesLT";
            where2.SchemaName = "dbo";

            Assert.AreNotEqual(where1, where2);
            Assert.IsFalse(where1.Equals(where2));

            where1.TableName = "Product1";
            where2.TableName = "Product1";
            where1.SchemaName = "SalesLT";
            where2.SchemaName = null;

            Assert.AreNotEqual(where1, where2);
            Assert.IsFalse(where1.Equals(where2));

            where1 = new SetupFilterWhere();
            where2 = new SetupFilterWhere();

            where1.ParameterName = "Param1";
            where2.ParameterName = "Param2";

            Assert.AreNotEqual(where1, where2);
            Assert.IsFalse(where1.Equals(where2));

            where1 = new SetupFilterWhere();
            where2 = new SetupFilterWhere();

            where1.ColumnName = "Col1";
            where2.ColumnName = "Col2";

            Assert.AreNotEqual(where1, where2);
            Assert.IsFalse(where1.Equals(where2));


        }
    }
}
