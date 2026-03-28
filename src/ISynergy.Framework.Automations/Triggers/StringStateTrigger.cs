using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// State trigger based on a string.
/// </summary>
public class StringStateTrigger : BaseTrigger<string>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="callbackAsync"></param>
    public StringStateTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<string> Property)> function,
        string from,
        string to,
        Func<string, Task> callbackAsync)
        : base(automationId, function, from, to, callbackAsync, TimeSpan.Zero)
    {
    }
}
