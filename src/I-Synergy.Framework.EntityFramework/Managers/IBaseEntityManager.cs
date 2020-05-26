using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.EntityFramework.Managers
{
    public interface IBaseEntityManager
    {
        Task<int> AddItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new();
        Task<int> AddUpdateItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new();
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : EntityBase, new();
        ValueTask<TEntity> GetItemByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct;
        Task<int> RemoveItemAsync<TEntity, TId>(TId id, string user, bool soft = false, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TId : struct;
        Task<int> UpdateItemAsync<TEntity, TSource>(TSource e, string user, CancellationToken cancellationToken = default)
            where TEntity : EntityBase, new()
            where TSource : ModelBase, new();
    }
}
