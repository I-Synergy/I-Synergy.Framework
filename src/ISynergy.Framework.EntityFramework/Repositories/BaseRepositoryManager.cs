using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.EntityFramework.Abstractions.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Repositories
{
    /// <summary>
    /// Class BaseRepositoryManager.
    /// Implements the <see cref="IBaseEntityManager" />
    /// </summary>
    /// <typeparam name="TDbContext">The type of the t database context.</typeparam>
    /// <seealso cref="IBaseEntityManager" />
    public abstract class BaseRepositoryManager<TDbContext> : IBaseEntityManager
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
        protected readonly TDbContext _dataContext;

        /// <summary>
        /// The tenant service
        /// </summary>
        protected readonly ITenantService _tenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepositoryManager{TDbContext}" /> class.
        /// </summary>
        /// <param name="dataContext">The context.</param>
        /// <param name="tenantService">The tenant service.</param>
        /// <param name="logger">The logger.</param>
        protected BaseRepositoryManager(TDbContext dataContext, ITenantService tenantService, ILogger<BaseRepositoryManager<TDbContext>> logger)
        {
            Argument.IsNotNull(dataContext);

            _dataContext = dataContext;
            _tenantService = tenantService;
            _logger = logger;
        }

        /// <summary>
        /// Exists the asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
        {
            return _dataContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// Gets the item by identifier asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>ValueTask&lt;TEntity&gt;.</returns>
        public virtual async ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            Argument.IsNotNull(entityPropertyName);

            if (entityPropertyName is not null)
            {
                var parameterExpression = Expression.Parameter(typeof(TEntity));
                var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(id));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

                var query = _dataContext
                    .Set<TEntity>()
                    .AsQueryable();

                var navigations = _dataContext.Model
                    .FindEntityType(typeof(TEntity))
                    .GetDerivedTypesInclusive()
                    .SelectMany(t => t.GetNavigations())
                    .Distinct();

                foreach (var property in navigations)
                    query = query.Include(property.Name);

                var result = await query
                    .SingleOrDefaultAsync(predicate, cancellationToken)
                    .ConfigureAwait(false);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Get item by identifier as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task&lt;TModel&gt; representing the asynchronous operation.</returns>
        public virtual async Task<TModel> GetItemByIdAsync<TEntity, TModel, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TModel : ModelBase, new()
            where TId : struct
        {
            if (await GetItemByIdAsync<TEntity, TId>(id, cancellationToken).ConfigureAwait(false) is TEntity result)
                return result.Adapt<TModel>();

            return null;
        }

        /// <summary>
        /// Adds the item asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TModel">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> AddItemAsync<TEntity, TModel>(TModel e, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TModel : ModelBase, new()
        {
            Argument.IsNotNull(e);

            var target = e.Adapt<TModel, TEntity>();

            var adds = _dataContext.Add(target);

            var result = await _dataContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// update item as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TModel">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        public async Task<int> UpdateItemAsync<TEntity, TModel>(TModel e, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TModel : ModelBase, new()
        {
            Argument.IsNotNull(e);

            var result = 0;

            var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            Argument.IsNotNull(entityPropertyName);

            var modelPropertyValue = e.GetIdentityValue();
            Argument.IsNotNull(modelPropertyValue);

            if (entityPropertyName is not null && modelPropertyValue is not null)
            {
                var parameterExpression = Expression.Parameter(typeof(TEntity));
                var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(modelPropertyValue));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

                var query = _dataContext
                    .Set<TEntity>()
                    .AsQueryable();

                var navigations = _dataContext.Model
                    .FindEntityType(typeof(TEntity))
                    .GetDerivedTypesInclusive()
                    .SelectMany(t => t.GetNavigations())
                    .Distinct();

                foreach (var property in navigations)
                    query = query.Include(property.Name);

                var target = await query
                    .SingleOrDefaultAsync(predicate, cancellationToken)
                    .ConfigureAwait(false);

                target = e.Adapt(target);

                var updates = _dataContext.Update(target);

                try
                {
                    result = await _dataContext
                        .SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    HandleDatabaseConcurrencyException(concurrencyException);
                }
            }

            return result;
        }

        /// <summary>
        /// add update item as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TModel">The type of the t source.</typeparam>
        /// <param name="e">The e.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        public async Task<int> AddUpdateItemAsync<TEntity, TModel>(TModel e, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TModel : ModelBase, new()
        {
            Argument.IsNotNull(e);

            var result = 0;

            var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
            Argument.IsNotNull(entityPropertyName);

            var modelPropertyValue = e.GetIdentityValue();
            Argument.IsNotNull(modelPropertyValue);

            if (entityPropertyName is not null && modelPropertyValue is not null)
            {
                var parameterExpression = Expression.Parameter(typeof(TEntity));
                var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(modelPropertyValue));
                var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

                var query = _dataContext
                    .Set<TEntity>()
                    .AsQueryable();

                var navigations = _dataContext.Model
                    .FindEntityType(typeof(TEntity))
                    .GetDerivedTypesInclusive()
                    .SelectMany(t => t.GetNavigations())
                    .Distinct();

                foreach (var property in navigations)
                    query = query.Include(property.Name);

                var target = await query
                    .SingleOrDefaultAsync(predicate, cancellationToken)
                    .ConfigureAwait(false);

                try
                {
                    if (target is null)
                    {
                        target = e.Adapt<TModel, TEntity>();

                        var adds = _dataContext.Add(target);

                        result = await _dataContext
                            .SaveChangesAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        target = e.Adapt(target);

                        var updates = _dataContext.Update(target);

                        result = await _dataContext
                            .SaveChangesAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    HandleDatabaseConcurrencyException(concurrencyException);
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
        /// <param name="soft">if set to <c>true</c> [soft].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException"></exception>
        public async Task<int> RemoveItemAsync<TEntity, TId>(TId id, bool soft = false, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct
        {
            Argument.IsNotNull(id);

            var result = 0;

            var item = await GetItemByIdAsync<TEntity, TId>(id, cancellationToken)
                .ConfigureAwait(false);

            if (item is not null)
            {
                try
                {
                    if (soft)
                    {
                        item.IsDeleted = true;
                        var updates = _dataContext.Update(item);
                    }
                    else
                    {
                        var deletes = _dataContext.Remove(item);
                    }

                    result = await _dataContext.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException concurrencyException)
                {
                    HandleDatabaseConcurrencyException(concurrencyException);
                }
                catch (DbUpdateException updateException)
                {
                    HandleDatabaseUpdateException(updateException);
                }
            }

            return result;
        }

        /// <summary>
        /// Handles the database update exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public virtual void HandleDatabaseUpdateException(Exception exception) { }

        /// <summary>
        /// Handles the database concurrency exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public virtual void HandleDatabaseConcurrencyException(Exception exception) { }
    }
}
