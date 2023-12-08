using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// Trigger based on an event.
/// </summary>
public class EventTrigger<T> : BaseTrigger
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="entity"></param>
    /// <param name="subscription"></param>
    /// <param name="callbackAsync"></param>
    public EventTrigger(
        Guid automationId,
        T entity,
        Action<EventHandler> subscription,
        Func<T, Task> callbackAsync)
        : base(automationId)
    {
        Argument.IsNotNull(entity);

        subscription.Invoke((s, e) =>
        {
            callbackAsync.Invoke(entity).Wait();
        });
    }
}
