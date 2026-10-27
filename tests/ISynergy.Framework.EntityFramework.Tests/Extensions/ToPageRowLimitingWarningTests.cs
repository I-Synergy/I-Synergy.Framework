using ISynergy.Framework.EntityFramework.Entities;
using ISynergy.Framework.EntityFramework.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// Documents the root cause of the upstream row-limiting warning — EF Core raises
/// <c>CoreEventId.RowLimitingOperationWithoutOrderByWarning</c> when a query applies
/// <c>Skip</c>/<c>Take</c> without a preceding <c>OrderBy</c> — and proves that the ordered
/// <see cref="QueryableExtensions.ToPage{TEntity, TKey}(IQueryable{TEntity}, Expression{Func{TEntity, TKey}}, int, int)"/>
/// overload places an <c>OrderBy</c> ahead of <c>Skip</c>/<c>Take</c>.
/// </summary>
[TestClass]
public class ToPageRowLimitingWarningTests
{
    [TestMethod]
    public void OrderedToPage_AppliesOrderByBeforeSkipAndTake()
    {
        using var context = CreateContext();

        var query = context.TestEntities.ToPage(e => e.Id, 1, 5);

        CollectionAssert.AreEqual(
            new[] { "Take", "Skip", "OrderBy" },
            MethodChain(query).Take(3).ToArray());
    }

    [TestMethod]
    public void UnorderedToPage_AppliesSkipAndTakeWithoutOrderBy()
    {
        using var context = CreateContext();

        var query = context.TestEntities.ToPage(1, 5);

        var chain = MethodChain(query);
        CollectionAssert.AreEqual(new[] { "Take", "Skip" }, chain.Take(2).ToArray());
        Assert.IsFalse(chain.Contains("OrderBy"), "The legacy overload is unordered by design.");
    }

    private static TestDataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase($"{nameof(ToPageRowLimitingWarningTests)}_{Guid.NewGuid()}")
            .Options;

        return new TestDataContext(options);
    }

    /// <summary>
    /// Walks the LINQ expression tree from the outermost call through each source,
    /// returning the LINQ method names in application order (outermost first).
    /// </summary>
    private static List<string> MethodChain(IQueryable query)
    {
        var methods = new List<string>();
        for (var expression = query.Expression;
             expression is MethodCallExpression call && call.Arguments.Count > 0;
             expression = call.Arguments[0])
        {
            methods.Add(call.Method.Name);
        }

        return methods;
    }
}
