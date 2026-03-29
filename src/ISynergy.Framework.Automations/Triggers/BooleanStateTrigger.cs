using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// State trigger based on a boolean.
/// </summary>
public class BooleanStateTrigger : BaseTrigger<bool>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="callbackAsync"></param>
    public BooleanStateTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<bool> Property)> function,
        bool from,
        bool to,
        Func<bool, Task> callbackAsync)
        : base(automationId, function, from, to, callbackAsync, TimeSpan.Zero)
    {
    }
}
