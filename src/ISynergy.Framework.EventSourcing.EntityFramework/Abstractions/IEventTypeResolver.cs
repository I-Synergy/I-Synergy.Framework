namespace ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;

/// <summary>
/// Resolves a CLR <see cref="Type"/> from a stored event type name and provides the canonical
/// type name for a given <see cref="Type"/>, used during event serialization and deserialization.
/// </summary>
public interface IEventTypeResolver
{
    /// <summary>
    /// Returns the <see cref="Type"/> that corresponds to <paramref name="eventTypeName"/>,
    /// or <c>null</c> when no match is found.
    /// </summary>
    /// <param name="eventTypeName">
    /// The type name as stored in <see cref="ISynergy.Framework.EventSourcing.Models.EventRecord.EventType"/>
    /// (typically the full CLR type name, e.g. <c>"MyApp.Orders.OrderCreated"</c>).
    /// </param>
    Type? Resolve(string eventTypeName);

    /// <summary>
    /// Returns the canonical string name for <paramref name="eventType"/> to be stored in the event log.
    /// </summary>
    string GetTypeName(Type eventType);
}
