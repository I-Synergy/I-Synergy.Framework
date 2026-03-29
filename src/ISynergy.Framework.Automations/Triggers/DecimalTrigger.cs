using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// Numeric trigger based on a decimal.
/// </summary>
public class DecimalTrigger : BaseNumericTrigger<decimal>
{
    /// <summary>
    /// Trigger for integer properties.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="below"></param>
    /// <param name="above"></param>
    /// <param name="callbackAsync"></param>
    public DecimalTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<decimal> Property)> function,
        decimal below,
        decimal above,
        Func<decimal, Task> callbackAsync)
        : base(automationId, function, below, above, callbackAsync)
    {
    }
}
