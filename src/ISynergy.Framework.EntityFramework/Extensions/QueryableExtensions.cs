using System;
using System.Linq;

namespace ISynergy.Framework.EntityFramework.Extensions
{
    /// <summary>
    /// QueryableExtensions
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Counts to pages of entities according to a certain pagesize.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="pagesize">Has to be greater than 0.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value must be greater than 0. - pagesize</exception>
        public static int CountPages<TEntity>(this IQueryable<TEntity> query, int pagesize)
        {
            if (pagesize < 1)
                throw new ArgumentOutOfRangeException(nameof(pagesize), "Value must be greater than 0.");

            var countPages = Convert.ToDecimal(query.Count());
            return Convert.ToInt32(Math.Ceiling(countPages / pagesize));
        }

        /// <summary>
        /// Applies paging to a queryable. Take note that this should be applied after
        /// any Where-clauses, to make sure you're not missing any results.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="page">Has to be non-negative.</param>
        /// <param name="pagesize">Has to be greater than 0.</param>
        /// <returns>IQueryable&lt;TEntity&gt;.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Value must be non-negative. - page</exception>
        /// <exception cref="ArgumentOutOfRangeException">Value must be greater than 0. - pagesize</exception>
        public static IQueryable<TEntity> ToPage<TEntity>(this IQueryable<TEntity> query, int page, int pagesize)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Value must be non-negative.");
            if (pagesize < 1)
                throw new ArgumentOutOfRangeException(nameof(pagesize), "Value must be greater than 0.");

            return query
                .Skip((page - 1) * pagesize)
                .Take(pagesize);
        }
    }
}
