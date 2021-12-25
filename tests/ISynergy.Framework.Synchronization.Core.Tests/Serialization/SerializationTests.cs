using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Tests.Serialization
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void Test_Schema_DataContractSerializer()
        {
            var schemaSerializer = new DataContractSerializer(typeof(SyncSet));
            var inSchema = CreateSchema();
            byte[] bin = null;
            SyncSet outSchema;

            using (var ms = new MemoryStream())
            {
                schemaSerializer.WriteObject(ms, inSchema);
                bin = ms.ToArray();
            }

            using (var fs = new FileStream("Datacontract_Schema.xml", FileMode.Create))
                fs.Write(bin, 0, bin.Length);

            using (var ms = new MemoryStream(bin))
                outSchema = schemaSerializer.ReadObject(ms) as SyncSet;

            Assertions(outSchema);
        }


        [TestMethod]
        public void Test_Schema_JsonSerializer()
        {
            var inSchema = CreateSchema();

            var serializer = new JsonSerializer();
            byte[] bin = null;
            SyncSet outSchema;

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                using (var jsonWriter = new JsonTextWriter(writer))
                    serializer.Serialize(jsonWriter, inSchema);
                bin = ms.ToArray();
            }

            // for readiness
            using (var fs = new FileStream("Json_schema.json", FileMode.Create))
                fs.Write(bin, 0, bin.Length);

            Assert.IsNotNull(bin);

            using (var ms = new MemoryStream(bin))
            using (var sr = new StreamReader(ms))
            using (var reader = new JsonTextReader(sr))
                outSchema = serializer.Deserialize<SyncSet>(reader);

            Assertions(outSchema);
        }

        private void Assertions(SyncSet outSchema)
        {
            // Call the EnsureSchema to propagate schema to all entities
            outSchema.EnsureSchema();

            Assert.IsNotNull(outSchema);
            Assert.IsTrue(outSchema.Tables.Count > 0);
            Assert.IsTrue(outSchema.Filters.Count > 0);
            Assert.IsTrue(outSchema.Relations.Count > 0);
            Assert.AreEqual(2, outSchema.Tables.Count);
            Assert.AreEqual(1, outSchema.Relations.Count);
            Assert.AreEqual(1, outSchema.Filters.Count);

            var tbl1 = outSchema.Tables[0];
            Assert.AreEqual("ServiceTickets", tbl1.TableName);
            Assert.IsNull(tbl1.SchemaName);
            Assert.AreEqual(SyncDirection.Bidirectional, tbl1.SyncDirection);
            Assert.IsNotNull(tbl1.Schema);
            Assert.AreEqual(outSchema, tbl1.Schema);
            Assert.AreEqual(8, tbl1.Columns.Count);
            Assert.AreEqual(tbl1, tbl1.Columns.Table);
            Assert.IsTrue(tbl1.Columns.InnerCollection.Count > 0);

            var col = tbl1.Columns[0];
            Assert.AreEqual("ServiceTicketID", col.ColumnName);
            Assert.IsTrue(col.AllowDBNull);
            Assert.AreEqual(10, col.AutoIncrementSeed);
            Assert.AreEqual(1, col.AutoIncrementStep);
            Assert.IsTrue(col.IsAutoIncrement);
            Assert.IsFalse(col.IsCompute);
            Assert.IsTrue(col.IsReadOnly);
            Assert.AreEqual(0, col.Ordinal);

            // check orders on others columns
            Assert.AreEqual(7, tbl1.Columns["CustomerID"].Ordinal);

            var tbl2 = outSchema.Tables[1];
            Assert.AreEqual("Product", tbl2.TableName);
            Assert.AreEqual("SalesLT", tbl2.SchemaName);
            Assert.AreEqual(SyncDirection.UploadOnly, tbl2.SyncDirection);
            Assert.IsNotNull(tbl2.Schema);
            Assert.AreEqual(outSchema, tbl2.Schema);
            Assert.AreEqual(2, tbl2.Columns.Count);
            Assert.AreEqual(tbl2, tbl2.Columns.Table);
            Assert.IsTrue(tbl2.Columns.InnerCollection.Count > 0);
            Assert.AreEqual(1, tbl2.PrimaryKeys.Count);
            Assert.AreEqual("Id", tbl2.PrimaryKeys[0]);

            // Check Filters
            Assert.IsTrue(outSchema.Filters.Count > 0);
            var sf = outSchema.Filters[0];
            Assert.AreEqual("Product", sf.TableName);
            Assert.AreEqual("SalesLT", sf.SchemaName);
            Assert.AreEqual(outSchema, sf.Schema);
            Assert.AreEqual(2, sf.Parameters.Count);
            Assert.AreEqual(1, sf.Joins.Count);
            Assert.AreEqual(1, sf.CustomWheres.Count);
            Assert.AreEqual(1, sf.Wheres.Count);

            // Parameter 01
            Assert.AreEqual("Title", sf.Parameters[0].Name);
            Assert.AreEqual(20, sf.Parameters[0].MaxLength);
            Assert.AreEqual("'Bikes'", sf.Parameters[0].DefaultValue);
            Assert.IsFalse(sf.Parameters[0].AllowNull);
            Assert.IsNull(sf.Parameters[0].TableName);
            Assert.IsNull(sf.Parameters[0].SchemaName);
            Assert.AreEqual(outSchema, sf.Parameters[0].Schema);

            // Parameter 02
            Assert.AreEqual("LastName", sf.Parameters[1].Name);
            Assert.AreEqual(0, sf.Parameters[1].MaxLength);
            Assert.IsNull(sf.Parameters[1].DefaultValue);
            Assert.IsTrue(sf.Parameters[1].AllowNull);
            Assert.AreEqual("Customer", sf.Parameters[1].TableName);
            Assert.AreEqual("SalesLT", sf.Parameters[1].SchemaName);
            Assert.AreEqual(outSchema, sf.Parameters[1].Schema);

            // Joins
            Assert.AreEqual(Join.Right, sf.Joins[0].JoinEnum);
            Assert.AreEqual("SalesLT.ProductCategory", sf.Joins[0].TableName);
            Assert.AreEqual("LCN", sf.Joins[0].LeftColumnName);
            Assert.AreEqual("SalesLT.Product", sf.Joins[0].LeftTableName);
            Assert.AreEqual("RCN", sf.Joins[0].RightColumnName);
            Assert.AreEqual("SalesLT.ProductCategory", sf.Joins[0].RightTableName);

            // Wheres
            Assert.AreEqual("Title", sf.Wheres[0].ColumnName);
            Assert.AreEqual("Title", sf.Wheres[0].ParameterName);
            Assert.AreEqual("SalesLT", sf.Wheres[0].SchemaName);
            Assert.AreEqual("Product", sf.Wheres[0].TableName);

            // Customer Wheres
            Assert.AreEqual("1 = 1", sf.CustomWheres[0]);



            // Check Relations
            Assert.IsTrue(outSchema.Relations.Count > 0);
            var rel = outSchema.Relations[0];
            Assert.AreEqual("AdventureWorks_Product_ServiceTickets", rel.RelationName);
            Assert.IsTrue(rel.ParentKeys.Count > 0);
            Assert.IsTrue(rel.Keys.Count > 0);
            var c = rel.Keys.ToList()[0];
            Assert.AreEqual("ProductId", c.ColumnName);
            Assert.AreEqual("ServiceTickets", c.TableName);
            Assert.IsNull(c.SchemaName);
            var p = rel.ParentKeys.ToList()[0];
            Assert.AreEqual("ProductId", p.ColumnName);
            Assert.AreEqual("Product", p.TableName);
            Assert.AreEqual("SalesLT", p.SchemaName);

        }

        private static SyncSet CreateSchema()
        {
            var set = new SyncSet();

            var tbl = new SyncTable("ServiceTickets", null);
            tbl.OriginalProvider = "SqlServerProvider";
            tbl.SyncDirection = SyncDirection.Bidirectional;

            set.Tables.Add(tbl);

            var c = SyncColumn.Create<int>("ServiceTicketID");
            c.DbType = 8;
            c.AllowDBNull = true;
            c.IsAutoIncrement = true;
            c.AutoIncrementStep = 1;
            c.AutoIncrementSeed = 10;
            c.IsCompute = false;
            c.IsReadOnly = true;
            tbl.Columns.Add(c);

            tbl.Columns.Add(SyncColumn.Create<string>("Title"));
            tbl.Columns.Add(SyncColumn.Create<string>("Description"));
            tbl.Columns.Add(SyncColumn.Create<int>("StatusValue"));
            tbl.Columns.Add(SyncColumn.Create<int>("EscalationLevel"));
            tbl.Columns.Add(SyncColumn.Create<DateTime>("Opened"));
            tbl.Columns.Add(SyncColumn.Create<DateTime>("Closed"));
            tbl.Columns.Add(SyncColumn.Create<int>("CustomerID"));

            tbl.PrimaryKeys.Add("ServiceTicketID");

            // Add Second tables
            var tbl2 = new SyncTable("Product", "SalesLT");
            tbl2.SyncDirection = SyncDirection.UploadOnly;

            tbl2.Columns.Add(SyncColumn.Create<int>("Id"));
            tbl2.Columns.Add(SyncColumn.Create<string>("Title"));
            tbl2.PrimaryKeys.Add("Id");

            set.Tables.Add(tbl2);


            // Add Filters
            var sf = new SyncFilter("Product", "SalesLT");
            sf.Parameters.Add(new SyncFilterParameter { Name = "Title", DbType = DbType.String, MaxLength = 20, DefaultValue = "'Bikes'" });
            sf.Parameters.Add(new SyncFilterParameter { Name = "LastName", TableName = "Customer", SchemaName = "SalesLT", AllowNull = true });
            sf.Wheres.Add(new SyncFilterWhereSideItem { ColumnName = "Title", ParameterName = "Title", SchemaName = "SalesLT", TableName = "Product" });
            sf.Joins.Add(new SyncFilterJoin { JoinEnum = Join.Right, TableName = "SalesLT.ProductCategory", LeftColumnName = "LCN", LeftTableName = "SalesLT.Product", RightColumnName = "RCN", RightTableName = "SalesLT.ProductCategory" });
            sf.CustomWheres.Add("1 = 1");
            set.Filters.Add(sf);

            // Add Relations
            var keys = new[] { new SyncColumnIdentifier("ProductId", "ServiceTickets") };
            var parentKeys = new[] { new SyncColumnIdentifier("ProductId", "Product", "SalesLT") };
            var rel = new SyncRelation("AdventureWorks_Product_ServiceTickets", keys, parentKeys);

            set.Relations.Add(rel);

            return set;
        }
    }
}
