using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// AOT/trim-safe implementation of <see cref="IEventSerializer"/> backed by a
/// consumer-supplied <see cref="JsonSerializerContext"/>.
/// </summary>
/// <remarks>
/// <para>
/// All concrete event types that can appear in the event stream must be declared on the
/// context with <c>[JsonSerializable(typeof(YourEventType))]</c>:
/// </para>
/// <code>
/// [JsonSerializable(typeof(OrderPlaced))]
/// [JsonSerializable(typeof(OrderShipped))]
/// public partial class OrderEventJsonContext : JsonSerializerContext { }
/// </code>
/// <para>
/// Register via the AOT-safe overload of
/// <c>AddEventSourcingEntityFramework(optionsAction, jsonSerializerContext, eventTypeMap)</c>.
/// </para>
/// </remarks>
public sealed class JsonSourceGeneratedEventSerializer : IEventSerializer
{
    private readonly JsonSerializerContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonSourceGeneratedEventSerializer"/>.
    /// </summary>
    /// <param name="context">
    /// A source-generated <see cref="JsonSerializerContext"/> that has every concrete
    /// event type registered via <c>[JsonSerializable]</c>.
    /// </param>
    public JsonSourceGeneratedEventSerializer(JsonSerializerContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    /// Thrown when the concrete event type is not registered in the
    /// <see cref="JsonSerializerContext"/>.
    /// </exception>
    public string Serialize(IDomainEvent @event)
    {
        var typeInfo = _context.GetTypeInfo(@event.GetType())
            ?? throw new InvalidOperationException(
                $"Type '{@event.GetType().FullName}' is not registered in the provided JsonSerializerContext. " +
                "Add [JsonSerializable(typeof(YourEventType))] to your context class.");

        return JsonSerializer.Serialize(@event, typeInfo);
    }

    /// <inheritdoc />
    public IDomainEvent? Deserialize(string data, Type type)
    {
        var typeInfo = _context.GetTypeInfo(type);
        return typeInfo is null ? null : JsonSerializer.Deserialize(data, typeInfo) as IDomainEvent;
    }
}
