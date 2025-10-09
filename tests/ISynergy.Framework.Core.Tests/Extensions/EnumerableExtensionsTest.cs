using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace ISynergy.Framework.Core.Extensions.Tests;

/// <summary>
/// Class EnumerableExtensionsTest.
/// </summary>
[TestClass]
public class EnumerableExtensionsTest
{
    /// <summary>
    /// Defines the test method NullEnumerableNonFailableTest.
    /// </summary>
    [TestMethod]
    public void NullEnumerableNonFailableTest()
    {
        IEnumerable<object>? list = null;
        bool result = false;

        foreach (object item in list.EnsureNotNull())
        {
        }

        result = true;

        Assert.IsTrue(result);
    }

    /// <summary>
    /// Defines the test method NullEnumerableFailableTest.
    /// </summary>
    [TestMethod]
    public void NullEnumerableFailableTest()
    {
        Assert.ThrowsAsync<NullReferenceException>(() =>
        {
            IEnumerable<object>? list = null;

            foreach (object item in list!)
            {
            }

            return Task.CompletedTask;
        });
    }

    [TestMethod]
    public void IEnumerableTToDataTableTest()
    {
        List<Product> collection =
        [
            new Product { ProductId = Guid.NewGuid(), Name = "Test1" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test2" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test3" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test4" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test5" }
        ];

        if (collection is IEnumerable<Product> data)
        {
            System.Data.DataTable dataTable = data.ToDataTable("Test");

            Assert.IsNotNull(dataTable);
            Assert.AreEqual(5, dataTable.Rows.Count);
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Properties)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Errors)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Validator)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.HasErrors)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsDirty)));
        }
        else
        {
            throw new Exception();
        }
    }

    [TestMethod]
    public void IEnumerableToDataTableTest()
    {
        List<Product> collection =
        [
            new Product { ProductId = Guid.NewGuid(), Name = "Test1" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test2" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test3" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test4" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test5" }
        ];

        if (collection is IEnumerable data)
        {
            System.Data.DataTable dataTable = data.ToDataTable(typeof(Product), "Test");

            Assert.IsNotNull(dataTable);
            Assert.AreEqual(5, dataTable.Rows.Count);
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.ProductGroups)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Properties)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Errors)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.Validator)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.HasErrors)));
            Assert.IsFalse(dataTable.Columns.Contains(nameof(Product.IsDirty)));
        }
        else
        {
            throw new Exception();
        }
    }
}
