using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Extensions.Tests;

/// <summary>
/// Class CollectionExtensionsTests.
/// </summary>
[TestClass]
public class CollectionExtensionsTests
{
    /// <summary>
    /// Defines the test method NullObservableCollectionNonFailableTest.
    /// </summary>
    [TestMethod]
    public void NullObservableCollectionNonFailableTest()
    {
        ObservableCollection<object>? list = null;
        Assert.IsNotNull(list.EnsureNotNull());
    }

    /// <summary>
    /// Defines the test method NullObservableCollectionFailableTest.
    /// </summary>
    [TestMethod]
    public void NullObservableCollectionFailableTest()
    {
        Assert.ThrowsException<NullReferenceException>(() =>
        {
            ObservableCollection<object>? list = null;
            foreach (object item in list!)
            {
                Assert.IsNotNull(item);
            }
        });
    }

    /// <summary>
    /// ICollectionTToDataTable Test
    /// </summary>
    [TestMethod]
    public void ICollectionTToDataTableTest()
    {
        List<Product> collection =
        [
            new Product { ProductId = Guid.NewGuid(), Name = "Test1" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test2" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test3" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test4" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test5" }
        ];

        if (collection is ICollection<Product> data)
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

    /// <summary>
    /// ICollectionToDataTable Test
    /// </summary>
    [TestMethod]
    public void ICollectionToDataTableTest()
    {
        List<Product> collection =
        [
            new Product { ProductId = Guid.NewGuid(), Name = "Test1" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test2" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test3" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test4" },
            new Product { ProductId = Guid.NewGuid(), Name = "Test5" }
        ];

        if (collection is ICollection data)
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
