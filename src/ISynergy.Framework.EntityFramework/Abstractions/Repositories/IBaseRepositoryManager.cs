using ISynergy.Framework.Core.Base;
using ISynergy.Framework.EntityFramework.Base;
using System.Linq.Expressions;

namespace ISynergy.Framework.EntityFramework.Abstractions.Repositories;

/// <summary>
/// Interface IBaseEntityManager
/// </summary>
public interface IBaseRepositoryManager
{
    /// <summary>
    /// Adds the item asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> AddItemAsync<TEntity, TSource>(TSource e, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity, new()
        where TSource : BaseRecord, new();

    /// <summary>
    /// Adds the update item asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> AddUpdateItemAsync<TEntity, TSource>(TSource e, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity, new()
        where TSource : BaseRecord, new();

    /// <summary>
    /// Existses the asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : BaseEntity, new();

    /// <summary>
    /// Gets the item by identifier asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>ValueTask&lt;TEntity&gt;.</returns>
    ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity, new()
        where TId : struct;

    /// <summary>
    /// Removes the item asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="soft">if set to <c>true</c> [soft].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> RemoveItemAsync<TEntity, TId>(TId id, bool soft = false, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity, new()
        where TId : struct;

    /// <summary>
    /// Updates the item asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <param name="e">The e.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Int32&gt;.</returns>
    Task<int> UpdateItemAsync<TEntity, TSource>(TSource e, CancellationToken cancellationToken = default)
        where TEntity : BaseEntity, new()
        where TSource : BaseRecord, new();
}
