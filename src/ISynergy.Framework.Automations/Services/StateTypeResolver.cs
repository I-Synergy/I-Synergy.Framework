using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.States;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Service responsible for resolving state types from values.
/// </summary>
public class StateTypeResolver : IStateTypeResolver
{
    private readonly Dictionary<Type, Type> _typeMappings;
    private readonly Type _defaultStateType;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTypeResolver"/> class.
    /// </summary>
    public StateTypeResolver()
    {
        _typeMappings = new Dictionary<Type, Type>
        {
            { typeof(int), typeof(IntegerState) },
            { typeof(double), typeof(DoubleState) },
            { typeof(decimal), typeof(DecimalState) },
            { typeof(bool), typeof(BooleanState) },
            { typeof(TimeSpan), typeof(TimeState) }
        };
        _defaultStateType = typeof(StringState);
    }

    /// <summary>
    /// Resolves the state type for the given value.
    /// </summary>
    /// <param name="value">The value to resolve a state type for.</param>
    /// <returns>The state type for the value.</returns>
    public Type ResolveStateType(object value)
    {
        if (value is null)
            return _defaultStateType;

        var valueType = value.GetType();

        // Check for exact type match
        if (_typeMappings.TryGetValue(valueType, out var stateType))
        {
            return stateType;
        }

        // Check for nullable types
        var underlyingType = Nullable.GetUnderlyingType(valueType);
        if (underlyingType is not null && _typeMappings.TryGetValue(underlyingType, out stateType))
        {
            return stateType;
        }

        // Default to string state
        return _defaultStateType;
    }
}

