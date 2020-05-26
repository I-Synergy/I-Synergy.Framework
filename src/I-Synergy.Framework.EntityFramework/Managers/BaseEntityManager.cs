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
    public abstract class BaseEntityManager<TDbContext> : IBaseEntityManager
        where TDbContext : DbContext
    {
        public const int DefaultPageSize = 250;

        protected readonly ILogger _logger;
        protected readonly TDbContext _context;

        protected BaseEntityManager(TDbContext context, ILogger<BaseEntityManager<TDbContext>> logger)
        {
            Argument.IsNotNull(nameof(context), context);

            _context = context;
            _logger = logger;
        }

        public virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
        {
            return _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public virtual ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            return _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

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
