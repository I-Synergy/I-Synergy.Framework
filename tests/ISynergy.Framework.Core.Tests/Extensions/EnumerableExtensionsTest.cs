using ISynergy.Framework.Core.Data.TestClasses;
using ISynergy.Framework.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace ISynergy.Framework.Core.Extensions;

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

    /// <summary>
    /// Defines the test method CountPagesTest (ceiling page count, matching the EF overload).
    /// </summary>
    [TestMethod]
    [DataRow(35, 10, 4)]
    [DataRow(20, 10, 2)]
    [DataRow(8, 10, 1)]
    [DataRow(0, 10, 0)]
    public void CountPagesTest(int size, int pageSize, int pages)
    {
        IEnumerable<object> list = Enumerable.Repeat(new object(), size);

        Assert.AreEqual(pages, list.CountPages(pageSize));
    }

    /// <summary>
    /// Defines the test method ToPageTest.
    /// </summary>
    [TestMethod]
    [DataRow(35, 10, 0, 10)]
    [DataRow(35, 10, 3, 5)]
    [DataRow(8, 10, 0, 8)]
    public void ToPageTest(int size, int pageSize, int pageIndex, int count)
    {
        IEnumerable<int> list = Enumerable.Range(0, size);

        var page = list.ToPage(pageIndex, pageSize).ToList();

        Assert.AreEqual(count, page.Count);
        Assert.AreEqual(pageIndex * pageSize, page[0]);
    }

    /// <summary>
    /// Defines the test method ToPageOrdersBeforePagingTest: the ordered overload sorts before paging.
    /// </summary>
    [TestMethod]
    public void ToPageOrdersBeforePagingTest()
    {
        IEnumerable<int> list = new[] { 5, 3, 1, 4, 2 };

        var page = list.ToPage(x => x, 0, 3).ToList();

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, page);
    }

    /// <summary>
    /// Defines the test method ToPageThrowsOnNegativePageIndexTest: reports the correct parameter name.
    /// </summary>
    [TestMethod]
    public void ToPageThrowsOnNegativePageIndexTest()
    {
        IEnumerable<int> list = Enumerable.Range(0, 10);

        var exception = Assert.ThrowsExactly<ArgumentBelowZeroException>(() => list.ToPage(-1, 10).ToList());

        Assert.AreEqual("pageIndex", exception.ParamName);
    }

    /// <summary>
    /// Defines the test method ToPageThrowsOnNonPositivePageSizeTest.
    /// </summary>
    [TestMethod]
    public void ToPageThrowsOnNonPositivePageSizeTest()
    {
        IEnumerable<int> list = Enumerable.Range(0, 10);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToPage(0, 0).ToList());
    }
}
