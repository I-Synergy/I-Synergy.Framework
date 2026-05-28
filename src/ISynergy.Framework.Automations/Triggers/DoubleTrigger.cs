using ISynergy.Framework.Automations.Triggers.Base;
using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Automations.Triggers;

/// <summary>
/// Numeric trigger based on a double.
/// </summary>
public class DoubleTrigger : BaseNumericTrigger<double>
{
    /// <summary>
    /// Trigger for integer properties.
    /// </summary>
    /// <param name="automationId"></param>
    /// <param name="function"></param>
    /// <param name="below"></param>
    /// <param name="above"></param>
    /// <param name="callbackAsync"></param>
    public DoubleTrigger(
        Guid automationId,
        Func<(IObservableValidatedClass Entity, IProperty<double> Property)> function,
        double below,
        double above,
        Func<double, Task> callbackAsync)
        : base(automationId, function, below, above, callbackAsync)
    {
    }
}
