using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// AOT/trim-safe implementation of <see cref="IEventTypeResolver"/> that resolves event
/// types from an explicit compile-time dictionary.
/// </summary>
/// <remarks>
/// <para>
/// Use this instead of <see cref="DefaultEventTypeResolver"/> when publishing with
/// <c>PublishAot</c> or <c>PublishTrimmed</c>.
/// </para>
/// <para>
/// The keys in the map must match the values returned by <see cref="GetTypeName"/>
/// (i.e. the short <c>Type.Name</c>) which are stored as <c>EventType</c> in the event log:
/// </para>
/// <code>
/// new Dictionary&lt;string, Type&gt;
/// {
///     [nameof(OrderPlaced)]   = typeof(OrderPlaced),
///     [nameof(OrderShipped)]  = typeof(OrderShipped),
///     [nameof(OrderCancelled)] = typeof(OrderCancelled),
/// }
/// </code>
/// </remarks>
public sealed class DictionaryEventTypeResolver : IEventTypeResolver
{
    private readonly IReadOnlyDictionary<string, Type> _typeMap;

    /// <summary>
    /// Initializes a new instance of <see cref="DictionaryEventTypeResolver"/>.
    /// </summary>
    /// <param name="typeMap">
    /// A dictionary mapping stored event type names to their CLR types.
    /// </param>
    public DictionaryEventTypeResolver(IReadOnlyDictionary<string, Type> typeMap) =>
        _typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));

    /// <inheritdoc />
    public Type? Resolve(string eventTypeName) =>
        _typeMap.TryGetValue(eventTypeName, out var type) ? type : null;

    /// <inheritdoc />
    /// <remarks>Returns <c>Type.Name</c> (short name, no namespace).</remarks>
    public string GetTypeName(Type eventType) => eventType.Name;
}
