using ISynergy.Framework.Automations.States.Base;

namespace ISynergy.Framework.Automations.States;

/// <summary>
/// Numeric trigger based on a decimal.
/// </summary>
public class DecimalState : BaseState<decimal>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="for"></param>
    public DecimalState(decimal from, decimal to, TimeSpan @for)
        : base(from, to, @for)
    {
    }
}
