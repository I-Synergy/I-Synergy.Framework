using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// Class QueryableExtensionTests.
/// </summary>
[TestClass]
public class QueryableExtensionTests
{
    /// <summary>
    /// Defines the test method GetPageCountTest.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pages">The pages.</param>
    [TestMethod]
    [DataRow(35, 10, 4)]
    [DataRow(20, 10, 2)]
    [DataRow(8, 10, 1)]
    public void GetPageCountTest(int size, int pageSize, int pages)
    {
        IQueryable<object> list = Enumerable.Repeat(new object(), size).AsQueryable();
        Assert.AreEqual(pages, list.CountPages(pageSize));
    }

    /// <summary>
    /// Defines the test method GetPageTest.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageIndex">The page.</param>
    /// <param name="count">The count.</param>
    [TestMethod]
    [DataRow(35, 10, 0, 10)]
    [DataRow(35, 10, 1, 10)]
    [DataRow(35, 10, 2, 10)]
    [DataRow(35, 10, 3, 5)]
    [DataRow(20, 10, 1, 10)]
    [DataRow(8, 10, 0, 8)]
    public void GetPageTest(int size, int pageSize, int pageIndex, int count)
    {
        IQueryable<object> list = Enumerable.Repeat(new object(), size).AsQueryable();
        Assert.AreEqual(count, list.ToPage(pageIndex, pageSize).Count());
    }

    /// <summary>
    /// Defines the test method GetOrderedPageTest: the ordered overload pages a sized sequence.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageIndex">The page.</param>
    /// <param name="count">The count.</param>
    [TestMethod]
    [DataRow(35, 10, 0, 10)]
    [DataRow(35, 10, 1, 10)]
    [DataRow(35, 10, 3, 5)]
    [DataRow(8, 10, 0, 8)]
    public void GetOrderedPageTest(int size, int pageSize, int pageIndex, int count)
    {
        IQueryable<int> list = Enumerable.Range(0, size).AsQueryable();
        var page = list.ToPage(x => x, pageIndex, pageSize).ToList();

        Assert.AreEqual(count, page.Count);
        Assert.AreEqual(pageIndex * pageSize, page[0]);
    }

    /// <summary>
    /// Defines the test method GetOrderedPageOrdersBeforePagingTest: the ordered overload sorts
    /// an unordered sequence before applying Skip/Take.
    /// </summary>
    [TestMethod]
    public void GetOrderedPageOrdersBeforePagingTest()
    {
        IQueryable<int> list = new[] { 5, 3, 1, 4, 2 }.AsQueryable();

        var page = list.ToPage(x => x, 0, 3).ToList();

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, page);
    }

    /// <summary>
    /// Defines the test method GetOrderedPageThrowsOnNegativePageIndexTest.
    /// </summary>
    [TestMethod]
    public void GetOrderedPageThrowsOnNegativePageIndexTest()
    {
        IQueryable<int> list = Enumerable.Range(0, 10).AsQueryable();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToPage(x => x, -1, 10).ToList());
    }

    /// <summary>
    /// Defines the test method GetOrderedPageThrowsOnNonPositivePageSizeTest.
    /// </summary>
    [TestMethod]
    public void GetOrderedPageThrowsOnNonPositivePageSizeTest()
    {
        IQueryable<int> list = Enumerable.Range(0, 10).AsQueryable();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => list.ToPage(x => x, 0, 0).ToList());
    }

    /// <summary>
    /// Defines the test method GetOrderedPageThrowsOnNullOrderByTest.
    /// </summary>
    [TestMethod]
    public void GetOrderedPageThrowsOnNullOrderByTest()
    {
        IQueryable<int> list = Enumerable.Range(0, 10).AsQueryable();

        Assert.ThrowsExactly<ArgumentNullException>(() => list.ToPage<int, int>(null!, 0, 10).ToList());
    }
}
