namespace ISynergy.Framework.Automations.Abstractions;

/// <summary>
/// Interface for resolving state types from values.
/// </summary>
public interface IStateTypeResolver
{
    /// <summary>
    /// Resolves the state type for the given value.
    /// </summary>
    /// <param name="value">The value to resolve a state type for.</param>
    /// <returns>The state type for the value.</returns>
    Type ResolveStateType(object value);
}

