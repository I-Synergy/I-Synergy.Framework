using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EntityFramework.Extensions.Tests;

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
    [DataTestMethod]
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
    /// <param name="page">The page.</param>
    /// <param name="count">The count.</param>
    [DataTestMethod]
    [DataRow(35, 10, 1, 10)]
    [DataRow(35, 10, 2, 10)]
    [DataRow(35, 10, 3, 10)]
    [DataRow(35, 10, 4, 5)]
    [DataRow(20, 10, 2, 10)]
    [DataRow(8, 10, 1, 8)]
    public void GetPageTest(int size, int pageSize, int page, int count)
    {
        IQueryable<object> list = Enumerable.Repeat(new object(), size).AsQueryable();
        Assert.AreEqual(count, list.ToPage(page, pageSize).Count());
    }
}
