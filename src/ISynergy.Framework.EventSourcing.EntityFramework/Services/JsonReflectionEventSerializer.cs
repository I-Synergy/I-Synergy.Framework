using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using System.Text.Json;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// Reflection-based implementation of <see cref="IEventSerializer"/> using
/// <see cref="JsonSerializer"/> with runtime type information.
/// </summary>
/// <remarks>
/// This is the default implementation registered by
/// <see cref="ISynergy.Framework.EventSourcing.EntityFramework.Extensions.ServiceCollectionExtensions.AddEventSourcingEntityFramework(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{Microsoft.EntityFrameworkCore.DbContextOptionsBuilder})"/>.
/// For AOT/trim-safe scenarios use
/// <see cref="JsonSourceGeneratedEventSerializer"/> instead.
/// </remarks>
public sealed class JsonReflectionEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    /// <inheritdoc />
    public string Serialize(IDomainEvent @event) =>
        JsonSerializer.Serialize((object)@event, @event.GetType(), s_options);

    /// <inheritdoc />
    public IDomainEvent? Deserialize(string data, Type type) =>
        JsonSerializer.Deserialize(data, type, s_options) as IDomainEvent;
}
