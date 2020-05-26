using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.EntityFramework.Managers
{
    /// <summary>
    /// Class BaseEntityManager.
    /// Implements the <see cref="IBaseEntityManager" />
    /// </summary>
    /// <typeparam name="TDbContext">The type of the t database context.</typeparam>
    /// <seealso cref="IBaseEntityManager" />
    public abstract class BaseEntityManager<TDbContext> : IBaseEntityManager
        where TDbContext : DbContext
    {
        /// <summary>
        /// The default page size
        /// </summary>
        public const int DefaultPageSize = 250;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// The context
        /// </summary>
        protected readonly TDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntityManager{TDbContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        protected BaseEntityManager(TDbContext context, ILogger<BaseEntityManager<TDbContext>> logger)
        {
            Argument.IsNotNull(nameof(context), context);

            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Existses the asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
        {
            return _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Gets the item by identifier asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>ValueTask&lt;TEntity&gt;.</returns>
        public virtual ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            return _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Adds the item asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public Task<int> AddItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new()
        {
            Argument.IsNotNull(nameof(TSource), e);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            var target = e.Adapt<TSource, TEntity>();

            _context.Add(target);

            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// update item as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        public async Task<int> UpdateItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new()
        {
            Argument.IsNotNull(nameof(TSource), e);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            var result = 0;

            var targetPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            var sourcePropertyValue = ReflectionExtensions.GetIdentityValue(e);

            if (targetPropertyName != null && sourcePropertyValue != null)
            {
                var parameterExpression = Expression.Parameter(typeof(TEntity));
                Expression query = Expression.Equal(Expression.Property(parameterExpression, targetPropertyName), Expression.Constant(sourcePropertyValue));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(query, parameterExpression);

                var target = await _context.Set<TEntity>()
                    .SingleOrDefaultAsync(predicate, cancellationToken)
                    .ConfigureAwait(false);

                target = e.Adapt(target);

                _context.Set<TEntity>().Update(target);

                try
                {
                    result = await _context
                        .SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// add update item as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        public async Task<int> AddUpdateItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new()
        {
            Argument.IsNotNull(nameof(TSource), e);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            var result = 0;

            var targetPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            var sourcePropertyValue = ReflectionExtensions.GetIdentityValue(e);

            if (targetPropertyName != null && sourcePropertyValue != null)
            {
                var parameterExpression = Expression.Parameter(typeof(TEntity));
                Expression query = Expression.Equal(Expression.Property(parameterExpression, targetPropertyName), Expression.Constant(sourcePropertyValue));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(query, parameterExpression);

                var target = await _context.Set<TEntity>()
                    .SingleOrDefaultAsync(predicate, cancellationToken)
                    .ConfigureAwait(false);

                if (target is null)
                {
                    target = e.Adapt<TSource, TEntity>();
                    _context.Add(target);
                }
                else
                {
                    target = e.Adapt(target);
                    _context.Set<TEntity>().Update(target);
                }

                try
                {
                    result = await _context
                        .SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// remove item as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="soft">if set to <c>true</c> [soft].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="DbUpdateException"></exception>
        public async Task<int> RemoveItemAsync<TEntity, TId>(TId id, string user, bool soft = false, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            Argument.IsNotNull(nameof(id), id);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            var result = 0;

            var item = await GetItemByIdAsync<TEntity, TId>(id, cancellationToken)
                .ConfigureAwait(false);

            if (item != null)
            {
                try
                {
                    if (soft)
                    {
                        item.IsDeleted = true;
                        _context.Set<TEntity>().Update(item);
                    }
                    else
                    {
                        _context.Set<TEntity>().Remove(item);
                    }

                    result = await _context.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                catch (DbUpdateException ex)
                {
                    if (ex.GetBaseException() is SqlException sqlException)
                    {
                        if (sqlException.Number == 547)
                        {
                            //The DELETE statement conflicted with the REFERENCE constraint
                            throw new DbUpdateException(ExceptionConstants.Error_547, ex);
                        }
                    }
                }
            }

            return result;
        }
    }
}
