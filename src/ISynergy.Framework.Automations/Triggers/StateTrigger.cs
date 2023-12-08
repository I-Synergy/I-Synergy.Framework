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
        Func<(IObservableClass Entity, IProperty<bool> Property)> function,
        bool from,
        bool to,
        Func<bool, Task> callbackAsync)
        : base(automationId, function, from, to, callbackAsync, TimeSpan.Zero)
    {
    }
}

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
        Func<(IObservableClass Entity, IProperty<string> Property)> function,
        string from,
        string to,
        Func<string, Task> callbackAsync)
        : base(automationId, function, from, to, callbackAsync, TimeSpan.Zero)
    {
    }
}
