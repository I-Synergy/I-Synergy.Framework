using ISynergy.Framework.EntityFramework.Base;

namespace ISynergy.Framework.EntityFramework.Events;

/// <summary>
/// A container for passing entities that have been deleted. This is not used for entities that are deleted logically via a bit column.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityDeletedEvent<T> where T : BaseEntity
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityDeletedEvent(T entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Entity
    /// </summary>
    /// <value>The entity.</value>
    public T Entity { get; }
}
