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
    public static IQueryable<TEntity> ToPage<TEntity>(this IQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Value must be non-negative.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Value must be greater than 0.");

        return query
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }
}
