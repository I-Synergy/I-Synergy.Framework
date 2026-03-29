using ISynergy.Framework.EventSourcing.Abstractions.Events;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;

/// <summary>
/// Serializes and deserializes domain events to/from JSON strings for persistence
/// in the event store.
/// </summary>
/// <remarks>
/// Two implementations are provided:
/// <list type="bullet">
/// <item><see cref="ISynergy.Framework.EventSourcing.EntityFramework.Services.JsonReflectionEventSerializer"/> — reflection-based, suitable for non-AOT scenarios.</item>
/// <item><see cref="ISynergy.Framework.EventSourcing.EntityFramework.Services.JsonSourceGeneratedEventSerializer"/> — AOT/trim-safe, requires a consumer-supplied <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>.</item>
/// </list>
/// </remarks>
public interface IEventSerializer
{
    /// <summary>
    /// Serializes <paramref name="event"/> to a JSON string for storage in the event log.
    /// </summary>
    /// <param name="event">The domain event to serialize. Must not be <c>null</c>.</param>
    /// <returns>A JSON string representation of the event.</returns>
    string Serialize(IDomainEvent @event);

    /// <summary>
    /// Deserializes a JSON string back to an <see cref="IDomainEvent"/> of the given <paramref name="type"/>.
    /// </summary>
    /// <param name="data">The JSON string previously produced by <see cref="Serialize"/>.</param>
    /// <param name="type">The CLR type to deserialize into.</param>
    /// <returns>
    /// The deserialized event cast to <see cref="IDomainEvent"/>, or <c>null</c> when
    /// <paramref name="type"/> is not registered / the data cannot be deserialized.
    /// </returns>
    IDomainEvent? Deserialize(string data, Type type);
}
