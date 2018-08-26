using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Extensions
{
    /// <summary>
    /// IQueryableExtensions
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Counts to pages of entities according to a certain pagesize.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pagesize">Has to be greater than 0.</param>
        /// <param name="cancellationToken"/>
        public static async Task<int> CountPagesAsync<TEntity>(this IQueryable<TEntity> query, int pagesize, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pagesize < 1)
                throw new ArgumentOutOfRangeException("Value must be greater than 0.", "pagesize");

            return (int)Math.Ceiling((double)await query.CountAsync(cancellationToken).ConfigureAwait(false) / pagesize);
        }

        /// <summary>
        /// Applies paging to a queryable. Take note that this should be applied after
        /// any Where-clauses, to make sure you're not missing any results.
        /// </summary>
        /// <param name="query"/>
        /// <param name="page">Has to be non-negative.</param>
        /// <param name="pagesize">Has to be greater than 0.</param>
        public static IQueryable<TEntity> ToPage<TEntity>(this IQueryable<TEntity> query, int page, int pagesize)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException("Value must be non-negative.", "page");
            if (pagesize < 1)
                throw new ArgumentOutOfRangeException("Value must be greater than 0.", "pagesize");

            return query.Skip((page - 1) * pagesize).Take(pagesize);
        }
    }
}