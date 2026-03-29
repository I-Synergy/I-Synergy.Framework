using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// Numeric trigger based on an integer.
/// </summary>
public class IntegerTrigger : BaseNumericTrigger<int>
{
    /// <summary>
    /// Trigger for integer properties.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="below"></param>
    /// <param name="above"></param>
    /// <param name="callbackAsync"></param>
    public IntegerTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<int> Property)> function,
        int below,
        int above,
        Func<int, Task> callbackAsync)
        : base(automationId, function, below, above, callbackAsync)
    {
    }
}
