using ISynergy.Framework.Automations.States.Base;

namespace ISynergy.Framework.Automations.States;

/// <summary>
/// Numeric trigger based on a double.
/// </summary>
public class DoubleState : BaseState<double>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="for"></param>
    public DoubleState(double from, double to, TimeSpan @for)
        : base(from, to, @for)
    {
    }
}
