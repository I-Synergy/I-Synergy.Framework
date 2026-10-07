using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// QueryableExtensions
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Counts to pages of entities according to a certain pageSize.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="pageSize">Has to be greater than 0.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Value must be greater than 0. - pageSize</exception>
    public static int CountPages<TEntity>(this IQueryable<TEntity> query, int pageSize)
    {
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");

        var countPages = Convert.ToDecimal(query.Count());
        return Convert.ToInt32(Math.Ceiling(countPages / pageSize));
    }

    /// <summary>
    /// Applies paging to a queryable. Take note that this should be applied after
    /// any Where-clauses, to make sure you're not missing any results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="pageIndex">Has to be non-negative.</param>
    /// <param name="pageSize">Has to be greater than 0.</param>
    /// <returns>IQueryable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Value must be non-negative. - pageIndex</exception>
    /// <exception cref="ArgumentOutOfRangeException">Value must be greater than 0. - pageSize</exception>
    /// <remarks>
    /// This overload does not apply an ordering. Paging a query that is not ordered yields
    /// non-deterministic results, and EF Core raises
    /// <see cref="Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.RowLimitingOperationWithoutOrderByWarning"/>.
    /// Prefer the ordered overload
    /// <see cref="ToPage{TEntity, TKey}(IQueryable{TEntity}, Expression{Func{TEntity, TKey}}, int, int)"/>,
    /// or order the query before calling this method.
    /// </remarks>
    public static IQueryable<TEntity> ToPage<TEntity>(this IQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        ValidatePaging(pageIndex, pageSize);

        return query
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Orders the query by the specified key and then applies paging, guaranteeing a stable
    /// result order so that EF Core does not raise the row-limiting-without-order warning.
    /// Take note that this should be applied after any Where-clauses, to make sure you're not
    /// missing any results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TKey">The type of the ordering key.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="orderBy">The ordering key selector, applied before <c>Skip</c>/<c>Take</c>.</param>
    /// <param name="pageIndex">Has to be non-negative.</param>
    /// <param name="pageSize">Has to be greater than 0.</param>
    /// <returns>IQueryable&lt;TEntity&gt;.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> or <paramref name="orderBy"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Value must be non-negative. - pageIndex</exception>
    /// <exception cref="ArgumentOutOfRangeException">Value must be greater than 0. - pageSize</exception>
    public static IQueryable<TEntity> ToPage<TEntity, TKey>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TKey>> orderBy,
        int pageIndex,
        int pageSize)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(orderBy);
        ValidatePaging(pageIndex, pageSize);

        return query
            .OrderBy(orderBy)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }

    private static void ValidatePaging(int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Value must be non-negative.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");
    }
}
