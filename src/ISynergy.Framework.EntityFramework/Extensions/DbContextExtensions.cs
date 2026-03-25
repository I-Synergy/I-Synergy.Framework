using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.EntityFramework.Base;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContext"/> providing common CRUD operations.
/// </summary>
public static class DbContextExtensions
{
    private const string ErrorEntity = "Entity does not have an identity value.";

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
    /// <remarks>
    /// This method builds a LINQ predicate and navigations chain using runtime reflection
    /// (<see cref="System.Linq.Expressions.Expression.Parameter(Type)"/>,
    /// <see cref="System.Linq.Expressions.Expression.Property(Expression, string)"/>, and
    /// <see cref="System.Linq.Expressions.Expression.Lambda{TDelegate}(Expression, ParameterExpression[])"/>)
    /// and is therefore not AOT-safe. AOT-publishing applications should use pre-compiled
    /// <c>EF.CompileAsyncQuery</c> delegates in their repositories instead of calling this helper.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static async ValueTask<TEntity?> GetItemByIdAsync<TEntity, TId>(this DbContext context, TId id, CancellationToken cancellationToken)
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

        var entityType = context.Model.FindEntityType(typeof(TEntity));

        if (entityType is null)
            return null;

        var navigations = entityType.GetDerivedTypesInclusive()
            .SelectMany(t => t.GetNavigations())
            .Distinct();

        query = navigations.Aggregate(query, (current, property) => current.Include(property.Name));

        var result = await query
            .SingleOrDefaultAsync(predicate, cancellationToken)
            .ConfigureAwait(false);

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
    /// <remarks>
    /// This method delegates to <see cref="GetItemByIdAsync{TEntity,TId}(DbContext,TId,CancellationToken)"/>
    /// which uses runtime reflection and is not AOT-safe. See that method's remarks for migration guidance.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
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
