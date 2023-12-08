using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.EntityFramework.Events;

/// <summary>
/// A container for entities that are updated.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityUpdatedEvent<T> where T : EntityBase
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityUpdatedEvent(T entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Entity
    /// </summary>
    /// <value>The entity.</value>
    public T Entity { get; }
}
