using ISynergy.Framework.Automations.States.Base;

namespace ISynergy.Framework.Automations.States;

/// <summary>
/// Numeric trigger based on an integer.
/// </summary>
public class IntegerState : BaseState<int>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="for"></param>
    public IntegerState(int from, int to, TimeSpan @for)
        : base(from, to, @for)
    {
    }
}
