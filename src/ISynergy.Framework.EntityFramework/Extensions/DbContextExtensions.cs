using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.EntityFramework.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions;

public static class DbContextExtensions
{
    private const string ErrorEntity = "Entity does not have an identity value.";
    private const string ErrorRecord = "Record does not have an identity value.";

    /// <summary>
    /// Check if item exists the asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="context"></param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public static Task<bool> ExistsAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
    {
        return context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the item by identifier asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <param name="context"></param>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>ValueTask&lt;TEntity&gt;.</returns>
    public static async ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(this DbContext context, TId id, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
        where TId : struct
    {
        var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();

        if (entityPropertyName is null)
            throw new ArgumentException(ErrorEntity);

        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(id));
        var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

        var query = context
            .Set<TEntity>()
            .AsQueryable();

        var navigations = context.Model
            .FindEntityType(typeof(TEntity))
            .GetDerivedTypesInclusive()
            .SelectMany(t => t.GetNavigations())
            .Distinct();

        query = navigations.Aggregate(query, (current, property) => current.Include(property.Name));

        var result = await query
            .SingleOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Get item by identifier as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TRecord">The type of the t model.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <param name="context"></param>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;TModel&gt; representing the asynchronous operation.</returns>
    public static async Task<TRecord> GetItemByIdAsync<TEntity, TRecord, TId>(this DbContext context, TId id, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
        where TRecord : BaseRecord, new()
        where TId : struct
    {
        if (await context.GetItemByIdAsync<TEntity, TId>(id, cancellationToken).ConfigureAwait(false) is { } result)
            return result.Adapt<TRecord>();

        return null;
    }

    /// <summary>
    /// Adds the item asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TRecord">The type of the t source.</typeparam>
    /// <param name="context"></param>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    public static async Task<int> AddItemAsync<TEntity, TRecord>(this DbContext context, TRecord e, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
        where TRecord : BaseRecord, new()
    {
        Argument.IsNotNull(e);

        var target = e.Adapt<TRecord, TEntity>();

        context.Add(target);

        var result = await context
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// update item as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TRecord">The type of the t source.</typeparam>
    /// <param name="context"></param>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Int32.</returns>
    public static async Task<int> UpdateItemAsync<TEntity, TRecord>(this DbContext context, TRecord e, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
        where TRecord : BaseRecord, new()
    {
        Argument.IsNotNull(e);

        var result = 0;

        var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();
        
        if (entityPropertyName is null)
            throw new ArgumentException(ErrorEntity);

        var recordPropertyValue = e.GetIdentityValue();
        
        if (recordPropertyValue is null)
            throw new ArgumentException(ErrorRecord);

        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(recordPropertyValue));
        var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

        var query = context
            .Set<TEntity>()
            .AsQueryable();

        var navigations = context.Model
            .FindEntityType(typeof(TEntity))
            .GetDerivedTypesInclusive()
            .SelectMany(t => t.GetNavigations())
            .Distinct();

        query = navigations.Aggregate(query, (current, property) => current.Include(property.Name));

        var target = await query
            .SingleOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

        target = e.Adapt(target);

        var updates = context.Update(target);

        result = await context
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// add update item as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TRecord">The type of the t source.</typeparam>
    /// <param name="context"></param>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Int32.</returns>
    public static async Task<int> AddUpdateItemAsync<TEntity, TRecord>(this DbContext context, TRecord e, CancellationToken cancellationToken)
        where TEntity : BaseEntity, new()
        where TRecord : BaseRecord, new()
    {
        Argument.IsNotNull(e);

        var result = 0;

        var entityPropertyName = ReflectionExtensions.GetIdentityPropertyName<TEntity>();

        if (entityPropertyName is null)
            throw new ArgumentException(ErrorEntity);

        var recordPropertyValue = e.GetIdentityValue();

        if (recordPropertyValue is null)
            throw new ArgumentException(ErrorRecord);

        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var expression = Expression.Equal(Expression.Property(parameterExpression, entityPropertyName), Expression.Constant(recordPropertyValue));
        var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpression);

        var query = context
            .Set<TEntity>()
            .AsQueryable();

        var navigations = context.Model
            .FindEntityType(typeof(TEntity))
            .GetDerivedTypesInclusive()
            .SelectMany(t => t.GetNavigations())
            .Distinct();

        query = navigations.Aggregate(query, (current, property) => current.Include(property.Name));

        var target = await query
            .SingleOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

        if (target is null)
        {
            target = e.Adapt<TRecord, TEntity>();

            var adds = context.Add(target);

            result = await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            target = e.Adapt(target);

            var updates = context.Update(target);

            result = await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// remove item as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TIdType">The type of the t identifier.</typeparam>
    /// <param name="context"></param>
    /// <param name="id">The identifier.</param>
    /// <param name="soft">if set to <c>true</c> [soft].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException"></exception>
    public static async Task<int> RemoveItemAsync<TEntity, TIdType>(this DbContext context, TIdType id, CancellationToken cancellationToken, bool soft = false)
        where TEntity : BaseEntity, new()
        where TIdType : struct
    {
        Argument.IsNotNull(id);

        var result = 0;

        var item = await context.GetItemByIdAsync<TEntity, TIdType>(id, cancellationToken)
            .ConfigureAwait(false);

        if (item is not null)
        {
            if (soft)
            {
                item.IsDeleted = true;
                context.Update(item);
            }
            else
            {
                context.Remove(item);
            }

            result = await context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return result;
    }
}
