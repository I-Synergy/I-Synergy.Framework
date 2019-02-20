using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Business.Base
{
    public abstract class BaseEntityManager<TDbContext>
        where TDbContext : DbContext
    {
        public const int DefaultPageSize = 250;

        protected ILogger Logger { get; }
        protected TDbContext Context { get; }

        protected BaseEntityManager(TDbContext context, ILoggerFactory loggerFactory)
        {
            Argument.IsNotNull(nameof(context), context);

            Context = context;
            Logger = loggerFactory.CreateLogger<BaseEntityManager<TDbContext>>();
        }

        protected virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
        {
            return Context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        protected virtual Task<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            return Context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        protected async Task<int> AddItemAsync<TEntity, TSource>(TSource e, string user)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new()
        {
            Argument.IsNotNull(nameof(TSource), e);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            TEntity target = e.Adapt<TSource, TEntity>();

            Context.Add(target);

            return await Context
                    .SaveChangesAsync()
                    .ConfigureAwait(false);
        }

        protected async Task<int> UpdateItemAsync<TEntity, TSource>(TSource e, string user)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new()
        {
            Argument.IsNotNull(nameof(TSource), e);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            int result = 0;

            string targetPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            object sourcePropertyValue = ReflectionExtensions.GetIdentityValue(e);

            if (targetPropertyName != null && sourcePropertyValue != null)
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
                Expression query = Expression.Equal(Expression.Property(parameterExpression, targetPropertyName), Expression.Constant(sourcePropertyValue));
                Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(query, parameterExpression);

                TEntity target = await Context.Set<TEntity>().SingleOrDefaultAsync(predicate).ConfigureAwait(false);
                target = e.Adapt(target);

                Context.Set<TEntity>().Update(target);

                try
                {
                    result = await Context
                        .SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }

            return result;
        }

        protected async Task<int> RemoveItemAsync<TEntity, TId>(TId id, string user, bool soft = false)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            Argument.IsNotNull(nameof(id), id);
            Argument.IsNotNullOrWhitespace(nameof(user), user);

            int result = 0;

            TEntity item = await GetItemByIdAsync<TEntity, TId>(id).ConfigureAwait(false);

            if (item != null)
            {
                try
                {
                    Context.Set<TEntity>().Remove(item);

                    result = await Context.SaveChangesAsync().ConfigureAwait(false);
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
