using ISynergy.Framework.Automations.States;

namespace ISynergy.Framework.Automations.Conditions;

/// <summary>
/// Value holder for a state.
/// </summary>
public class ConditionValue
{
    /// <summary>
    /// Placeholder for IState.
    /// </summary>
    public Type State { get; }

    /// <summary>
    /// Placeholder for the value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="value">The value to create a condition value for.</param>
    public ConditionValue(object value)
    {
        State = ResolveStateType(value);
        Value = value;
    }

    /// <summary>
    /// Default constructor for events.
    /// </summary>
    /// <param name="event">The event name.</param>
    /// <param name="value">The value.</param>
    public ConditionValue(string @event, object value)
    {
        State = typeof(EventState);
        Value = value;
    }

    /// <summary>
    /// Resolves the state type for the given value.
    /// </summary>
    /// <param name="value">The value to resolve.</param>
    /// <returns>The state type.</returns>
    private static Type ResolveStateType(object value)
    {
        return value switch
        {
            int => typeof(IntegerState),
            double => typeof(DoubleState),
            decimal => typeof(DecimalState),
            bool => typeof(BooleanState),
            TimeSpan => typeof(TimeState),
            _ => typeof(StringState)
        };
    }
}
