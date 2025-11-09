using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.States;
using ISynergy.Framework.Core.Locators;

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
        // Use StateTypeResolver if available, otherwise fall back to type checking
        IStateTypeResolver? resolver = null;
        try
        {
            resolver = ServiceLocator.Default.GetRequiredService<IStateTypeResolver>();
        }
        catch
        {
            // ServiceLocator not available or resolver not registered, use fallback logic
        }

        if (resolver is not null)
        {
            State = resolver.ResolveStateType(value);
        }
        else
        {
            // Fallback to type checking for backward compatibility
            State = ResolveStateTypeFallback(value);
        }

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
    /// Fallback method for resolving state type when resolver is not available.
    /// </summary>
    /// <param name="value">The value to resolve.</param>
    /// <returns>The state type.</returns>
    private static Type ResolveStateTypeFallback(object value)
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
