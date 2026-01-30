using ISynergy.Framework.Automations.States.Base;

namespace ISynergy.Framework.Automations.States;

/// <summary>
/// State trigger based on a boolean.
/// </summary>
public class BooleanState : BaseState<bool>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="for"></param>
    public BooleanState(bool value, TimeSpan @for)
        : base(!value, value, @for)
    {
    }
}
